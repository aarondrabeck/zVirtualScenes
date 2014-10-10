using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using zvs.Entities;
using System.Data.Entity;
using System.Threading.Tasks;

namespace zvs.WPF.Groups
{
    /// <summary>
    /// Interaction logic for GroupEditor.xaml
    /// </summary>
    public partial class GroupEditor : Window, IDisposable
    {
        private zvsContext context;

        public GroupEditor()
        {
            context = new zvsContext();

            InitializeComponent();

            zvsContext.ChangeNotifications<Device>.OnEntityAdded += GroupEditor_onEntityAdded;
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
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                System.Windows.Data.CollectionViewSource groupsViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("groupsViewSource")));
                await context.Groups
                    .Include(o => o.Devices)
                    .ToListAsync();

                await context.Devices.ToListAsync();
                groupsViewSource.Source = context.Groups.Local;
            }

            DeviceLst.MinimalistDisplay = false;

            EvaluateRemoveBtnUsability();
            EvaluateAddEditBtnsUsability();
            EvaluategroupsDevicesLstVwEnable();
        }

        void GroupEditor_onEntityAdded(object sender, NotifyEntityChangeContext.ChangeNotifications<Device>.EntityAddedArgs e)
        {
            if (context == null)
                return;

            this.Dispatcher.Invoke(new Action(async () =>
            {
                foreach (var ent in context.ChangeTracker.Entries<Device>())
                    await ent.ReloadAsync();
            }));
        }
        private void GroupEditor_Closed_1(object sender, EventArgs e)
        {
            zvsContext.ChangeNotifications<Device>.OnEntityAdded -= GroupEditor_onEntityAdded;
            context.Dispose();
        }

        private void EvaluateAddEditBtnsUsability()
        {
            if (GroupCmbBx.Items.Count > 0)
            {
                this.RemoveBtn.IsEnabled = true;
                this.EditBtn.IsEnabled = true;
            }
            else
            {
                this.RemoveBtn.IsEnabled = false;
                this.EditBtn.IsEnabled = false;
            }
        }

        private async void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            GroupNameEditor nameWindow = new GroupNameEditor(null);
            nameWindow.Owner = this;

            if (nameWindow.ShowDialog() ?? false)
            {
                Group new_g = new Group()
                {
                    Name = nameWindow.GroupName
                };

                context.Groups.Local.Add(new_g);

                var result = await context.TrySaveChangesAsync();
                if (result.HasError)
                    ((App)App.Current).zvsCore.log.Error(result.Message);

                GroupCmbBx.SelectedItem = GroupCmbBx.Items.OfType<Group>().FirstOrDefault(o => o.Name == new_g.Name);

            }
        }

        private async void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            Group g = (Group)GroupCmbBx.SelectedItem;
            if (g != null)
            {
                if (
                    MessageBox.Show("Are you sure you want to delete the '" + g.Name + "' group?",
                                    "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {

                    context.Groups.Local.Remove(g);

                    var result = await context.TrySaveChangesAsync();
                    if (result.HasError)
                        ((App)App.Current).zvsCore.log.Error(result.Message);
                }
            }

        }

        private async void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            Group g = (Group)GroupCmbBx.SelectedItem;
            if (g == null)
                return;

            GroupNameEditor nameWindow = new GroupNameEditor(g.Name);
            nameWindow.Owner = this;

            if (nameWindow.ShowDialog() ?? false)
            {
                g.Name = nameWindow.GroupName;

                var result = await context.TrySaveChangesAsync();
                if (result.HasError)
                    ((App)App.Current).zvsCore.log.Error(result.Message);
            }
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
                List<Device> devices = (List<Device>)e.Data.GetData("deviceList");

                Group selected_group = (Group)GroupCmbBx.SelectedItem;
                if (selected_group != null)
                {
                    groupsDevicesLstVw.SelectedItems.Clear();

                    foreach (Device device in devices)
                    {
                        Device d2 = await context.Devices.FirstOrDefaultAsync(o => o.Id == device.Id);
                        if (d2 != null)
                        {
                            //If not already in the group...
                            if (!selected_group.Devices.Contains(d2))
                            {
                                selected_group.Devices.Add(d2);
                                groupsDevicesLstVw.SelectedItems.Add(d2);
                                DeviceLst.DeviceGrid.SelectedItems.Clear();
                            }
                            else
                            {
                                MessageBox.Show(string.Format("{0} is already a member of the '{1}' group.", device.Name, selected_group.Name),
                                                "Already a member",
                                                MessageBoxButton.OK,
                                                MessageBoxImage.Error);
                            }
                        }
                    }

                    var result = await context.TrySaveChangesAsync();
                    if (result.HasError)
                        ((App)App.Current).zvsCore.log.Error(result.Message);

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
            if (this.groupsDevicesLstVw.SelectedItems.Count > 0)
                this.RemoveSelected.IsEnabled = true;
            else
                this.RemoveSelected.IsEnabled = false;
        }

        private async Task RemoveSelectedGroupDevicesAsync()
        {
            Group selected_group = (Group)GroupCmbBx.SelectedItem;
            if (selected_group != null && groupsDevicesLstVw.SelectedItems.Count > 0)
            {
                if (MessageBox.Show(string.Format("Are you sure you want to remove the {0} selected devices from this group?", groupsDevicesLstVw.SelectedItems.Count),
                                         "Remove Devices?",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Error) == MessageBoxResult.Yes)
                {
                    Device[] devicesToRemove = new Device[groupsDevicesLstVw.SelectedItems.Count];
                    groupsDevicesLstVw.SelectedItems.CopyTo(devicesToRemove, 0);

                    foreach (Device gd in devicesToRemove)
                        selected_group.Devices.Remove(gd);

                    var result = await context.TrySaveChangesAsync();
                    if (result.HasError)
                        ((App)App.Current).zvsCore.log.Error(result.Message);
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
            if (GroupCmbBx.SelectedItem == null)
                groupsDevicesLstVw.IsEnabled = false;
            else
                groupsDevicesLstVw.IsEnabled = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.context == null)
                {
                    return;
                }

                context.Dispose();
            }
        }
    }
}
