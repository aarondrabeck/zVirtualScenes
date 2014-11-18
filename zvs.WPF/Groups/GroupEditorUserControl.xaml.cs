using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;
using zvs.DataModel;
using zvs.Processor;

namespace zvs.WPF.Groups
{
    /// <summary>
    /// Interaction logic for GroupEditorUserControl.xaml
    /// </summary>
    public partial class GroupEditorUserControl
    {
        private ZvsContext Context { get; set; }
        private IFeedback<LogEntry> Log { get; set; }
        private readonly App _app = (App) Application.Current;

        public GroupEditorUserControl()
        {
            Log = new DatabaseFeedback(_app.EntityContextConnection) {Source = "Group Editor"};
            InitializeComponent();
            NotifyEntityChangeContext.ChangeNotifications<Device>.OnEntityAdded += GroupEditor_onEntityAdded;
        }

#if DEBUG
        ~GroupEditorUserControl()
        {
            //Cannot write to log here, it has been disposed. 
            Debug.WriteLine("GroupEditor Deconstructed.");
        }
#endif

        private async void GroupEditorUserControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            Context = new ZvsContext(_app.EntityContextConnection);
            // Do not load your data at design time.
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                var groupsViewSource = ((CollectionViewSource) (FindResource("GroupsViewSource")));
                await Context.Groups
                    .Include(o => o.Devices)
                    .ToListAsync();

                await Context.Devices.ToListAsync();
                groupsViewSource.Source = Context.Groups.Local;
            }
        }

        private void GroupEditor_onEntityAdded(object sender,
            NotifyEntityChangeContext.ChangeNotifications<Device>.EntityAddedArgs e)
        {
            if (Context == null)
                return;

            Dispatcher.Invoke(new Action(async () =>
            {
                foreach (var ent in Context.ChangeTracker.Entries<Device>())
                    await ent.ReloadAsync();
            }));
        }

        private void GroupEditorUserControl_OnUnloaded(object sender, RoutedEventArgs e)
        {
            NotifyEntityChangeContext.ChangeNotifications<Device>.OnEntityAdded -= GroupEditor_onEntityAdded;
            Context.Dispose();
        }


        private async Task RemoveSelectedGroupDevicesAsync()
        {
            var selectedGroup = GroupsDataGrid.SelectedItem as Group;
            if (selectedGroup != null && GroupsDevicesLstVw.SelectedItems.Count > 0)
            {
                if (MessageBox.Show(
                    string.Format("Are you sure you want to remove the {0} selected devices from this group?",
                        GroupsDevicesLstVw.SelectedItems.Count),
                    "Remove Devices?",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Error) == MessageBoxResult.Yes)
                {
                    var devicesToRemove = new Device[GroupsDevicesLstVw.SelectedItems.Count];
                    GroupsDevicesLstVw.SelectedItems.CopyTo(devicesToRemove, 0);

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

        private async void RemoveSelected_OnClick(object sender, RoutedEventArgs e)
        {
            await RemoveSelectedGroupDevicesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (Context == null)
            {
                return;
            }

            Context.Dispose();
        }

        private async void GroupsDataGrid_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var dg = sender as DataGrid;
            if (dg == null) return;
            var dgr = (DataGridRow) (dg.ItemContainerGenerator.ContainerFromIndex(dg.SelectedIndex));
            if (e.Key != Key.Delete || dgr.IsEditing) return;
            e.Handled = true;

            if (!(dgr.Item is Group)) return;
            var @group = (Group) dgr.Item;
            if (@group != null)
            {
                e.Handled = !await DeleteTask(@group);
            }
        }

        private async Task<bool> DeleteTask(Group group)
        {
            if (MessageBox.Show(string.Format("Are you sure you want to delete the '{0}' group?", group.Name),
                "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return false;
            Context.Groups.Local.Remove(group);
            await SaveChangesAsync();
            GroupsDataGrid.Focus();
            return true;
        }

        private async void GroupsDataGrid_OnRowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction != DataGridEditAction.Commit) return;
            var group = e.Row.DataContext as Group;
            if (group != null)
            {
                if (group.Name == null)
                {
                    group.Name = "New Group";
                }
            }

            //have to add , UpdateSourceTrigger=PropertyChanged to have the data updated in time for this event
            await SaveChangesAsync();
        }

        private async Task SaveChangesAsync()
        {
            var result = await Context.TrySaveChangesAsync(_app.Cts.Token);
            if (result.HasError)
                await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving scheduled task. {0}", result.Message);

            SignalImg.Opacity = 1;
            var da = new DoubleAnimation {From = 1, To = 0, Duration = new Duration(TimeSpan.FromSeconds(.8))};
            SignalImg.BeginAnimation(OpacityProperty, da);
        }

        private void GroupsDataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GroupsDataGrid.SelectedItem == null ||
                GroupsDataGrid.SelectedItem.ToString().Equals("{NewItemPlaceholder}"))
            {
                DetailsPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                DetailsPanel.Visibility = Visibility.Visible;
            }
        }

        private void AddDeviceButton_OnClick(object sender, RoutedEventArgs e)
        {
            var device = new DeviceMultiselectWindow(new ZvsEntityContextConnection());
            device.ShowDialog();
        }

        private async void TurnOffButton_OnClick(object sender, RoutedEventArgs e)
        {
            var g = GroupsDataGrid.SelectedItem as Group;
            if (g == null) return;

            var groupOffCmd = await Context.BuiltinCommands.FirstOrDefaultAsync(c => c.UniqueIdentifier == "GROUP_OFF");
            if (groupOffCmd == null) return;

            await
                Log.ReportResultAsync(
                    await
                        _app.ZvsEngine.RunCommandAsync(groupOffCmd.Id, g.Id.ToString(CultureInfo.InvariantCulture),
                            String.Empty, _app.Cts.Token), _app.Cts.Token);
        }

        private async void TurnOnButton_OnClick(object sender, RoutedEventArgs e)
        {
            var g = GroupsDataGrid.SelectedItem as Group;
            if (g == null) return;

            var groupOffCmd = await Context.BuiltinCommands.FirstOrDefaultAsync(c => c.UniqueIdentifier == "GROUP_ON");
            if (groupOffCmd == null) return;

            await
                Log.ReportResultAsync(
                    await
                        _app.ZvsEngine.RunCommandAsync(groupOffCmd.Id, g.Id.ToString(CultureInfo.InvariantCulture),
                            String.Empty, _app.Cts.Token), _app.Cts.Token);
        }
    }
}
