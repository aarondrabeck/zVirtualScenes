using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using zvs.DataModel;
using System.Data.Entity;
using System.Threading.Tasks;
using zvs.Processor;

namespace zvs.WPF.Groups
{
    /// <summary>
    /// Interaction logic for GroupEditor.xaml
    /// </summary>
    public partial class GroupEditor : IDisposable
    {
        private ZvsContext Context { get; set; }
        private IFeedback<LogEntry> Log { get; set; }
        private readonly App _app = (App)Application.Current;

        public GroupEditor()
        {
            Context = new ZvsContext(_app.EntityContextConnection);
            Log = new DatabaseFeedback(_app.EntityContextConnection) { Source = "Group Editor" };

            InitializeComponent();

            NotifyEntityChangeContext.ChangeNotifications<Device>.OnEntityAdded += GroupEditor_onEntityAdded;
        }

#if DEBUG
        ~GroupEditor()
        {
            //Cannot write to log here, it has been disposed. 
            Debug.WriteLine("GroupEditor Deconstructed.");
        }
#endif

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Do not load your data at design time.
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                var groupsViewSource = ((CollectionViewSource)(FindResource("groupsViewSource")));
                await Context.Groups
                    .Include(o => o.Devices)
                    .ToListAsync();

                await Context.Devices.ToListAsync();
                groupsViewSource.Source = Context.Groups.Local;
            }

            DeviceLst.MinimalistDisplay = false;

            EvaluateRemoveBtnUsability();
            EvaluateAddEditBtnsUsability();
            EvaluategroupsDevicesLstVwEnable();
        }

        void GroupEditor_onEntityAdded(object sender, NotifyEntityChangeContext.ChangeNotifications<Device>.EntityAddedArgs e)
        {
            if (Context == null)
                return;

            Dispatcher.Invoke(new Action(async () =>
            {
                foreach (var ent in Context.ChangeTracker.Entries<Device>())
                    await ent.ReloadAsync();
            }));
        }
        private void GroupEditor_Closed_1(object sender, EventArgs e)
        {
            NotifyEntityChangeContext.ChangeNotifications<Device>.OnEntityAdded -= GroupEditor_onEntityAdded;
            Context.Dispose();
        }

        private void EvaluateAddEditBtnsUsability()
        {
            if (GroupCmbBx.Items.Count > 0)
            {
                RemoveBtn.IsEnabled = true;
                EditBtn.IsEnabled = true;
            }
            else
            {
                RemoveBtn.IsEnabled = false;
                EditBtn.IsEnabled = false;
            }
        }

        private async void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var nameWindow = new GroupNameEditor(null) {Owner = this};

            if (!(nameWindow.ShowDialog() ?? false)) return;
            var newG = new Group
            {
                Name = nameWindow.GroupName
            };

            Context.Groups.Local.Add(newG);

            var result = await Context.TrySaveChangesAsync(_app.Cts.Token);
            if (result.HasError)
                await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving new group. {0}", result.Message);

            GroupCmbBx.SelectedItem = GroupCmbBx.Items.OfType<Group>().FirstOrDefault(o => o.Name == newG.Name);
        }

        private async void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            var g = (Group)GroupCmbBx.SelectedItem;
            if (g == null) return;
            if (MessageBox.Show("Are you sure you want to delete the '" + g.Name + "' group?",
                "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;
            Context.Groups.Local.Remove(g);

            var result = await Context.TrySaveChangesAsync(_app.Cts.Token);
            if (result.HasError)
                await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error removing group. {0}", result.Message);
        }

        private async void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            var g = (Group)GroupCmbBx.SelectedItem;
            if (g == null)
                return;

            var nameWindow = new GroupNameEditor(g.Name) {Owner = this};

            if (!(nameWindow.ShowDialog() ?? false)) return;
            g.Name = nameWindow.GroupName;

            var result = await Context.TrySaveChangesAsync(_app.Cts.Token);
            if (result.HasError)
                await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error editing group. {0}", result.Message);
        }

        private void groupsDevicesLstVw_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetData("deviceList") != null && e.Data.GetData("deviceList").GetType() == typeof(List<Device>))
            {
                e.Effects = DragDropEffects.Link;
            }
            e.Effects = DragDropEffects.None;
        }

        private async void groupsDevicesLstVw_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData("deviceList") != null && e.Data.GetData("deviceList").GetType() == typeof(List<Device>))
            {
                var devices = (List<Device>)e.Data.GetData("deviceList");

                var selectedGroup = (Group)GroupCmbBx.SelectedItem;
                if (selectedGroup != null)
                {
                    groupsDevicesLstVw.SelectedItems.Clear();

                    foreach (var device in devices)
                    {
                        var device1 = device;
                        var d2 = await Context.Devices.FirstOrDefaultAsync(o => o.Id == device1.Id);
                        if (d2 == null) continue;
                        //If not already in the group...
                        if (!selectedGroup.Devices.Contains(d2))
                        {
                            selectedGroup.Devices.Add(d2);
                            groupsDevicesLstVw.SelectedItems.Add(d2);
                            DeviceLst.DeviceGrid.SelectedItems.Clear();
                        }
                        else
                        {
                            MessageBox.Show(string.Format("{0} is already a member of the '{1}' group.", device.Name, selectedGroup.Name),
                                "Already a member",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                        }
                    }

                    var result = await Context.TrySaveChangesAsync(_app.Cts.Token);
                    if (result.HasError)
                        await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving group. {0}", result.Message);

                    groupsDevicesLstVw.Focus();
                }
                e.Effects = DragDropEffects.Move;
            }
            e.Handled = true;
        }

        private void groupsDevicesLstVw_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EvaluateRemoveBtnUsability();
        }

        private void EvaluateRemoveBtnUsability()
        {
            RemoveSelected.IsEnabled = groupsDevicesLstVw.SelectedItems.Count > 0;
        }

        private async Task RemoveSelectedGroupDevicesAsync()
        {
            var selectedGroup = (Group)GroupCmbBx.SelectedItem;
            if (selectedGroup != null && groupsDevicesLstVw.SelectedItems.Count > 0)
            {
                if (MessageBox.Show(string.Format("Are you sure you want to remove the {0} selected devices from this group?", groupsDevicesLstVw.SelectedItems.Count),
                                         "Remove Devices?",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Error) == MessageBoxResult.Yes)
                {
                    var devicesToRemove = new Device[groupsDevicesLstVw.SelectedItems.Count];
                    groupsDevicesLstVw.SelectedItems.CopyTo(devicesToRemove, 0);

                    foreach (var gd in devicesToRemove)
                        selectedGroup.Devices.Remove(gd);

                    var result = await Context.TrySaveChangesAsync(_app.Cts.Token);
                    if (result.HasError)
                        await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving group. {0}", result.Message);
                }
            }
        }

        private async void groupsDevicesLstVw_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                await RemoveSelectedGroupDevicesAsync();
            }
        }

        private async void RemoveSelected_Click(object sender, RoutedEventArgs e)
        {
            await RemoveSelectedGroupDevicesAsync();
        }

        private void GroupCmbBx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EvaluateAddEditBtnsUsability();
            EvaluategroupsDevicesLstVwEnable();
        }

        private void EvaluategroupsDevicesLstVwEnable()
        {
            groupsDevicesLstVw.IsEnabled = GroupCmbBx.SelectedItem != null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Context == null)
                {
                    return;
                }

                Context.Dispose();
            }
        }
    }
}
