using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
        private IFeedback<LogEntry> Log { get; }
        private readonly App _app = (App)Application.Current;

        public GroupEditorUserControl()
        {
            Log = new DatabaseFeedback(_app.EntityContextConnection) { Source = "Group Editor" };
            InitializeComponent();

            NotifyEntityChangeContext.ChangeNotifications<Device>.OnEntityAdded += GroupEditor_onEntityAdded;

            NotifyEntityChangeContext.ChangeNotifications<Group>.OnEntityDeleted += ChangeNotificationsOnOnEntityDeleted;
            NotifyEntityChangeContext.ChangeNotifications<Group>.OnEntityUpdated += GroupEditorUserControl_OnEntityUpdated;
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
                var groupsViewSource = ((CollectionViewSource)(FindResource("GroupsViewSource")));
                await Context.Groups
                    .Include(o => o.Devices)
                    .ToListAsync();

                await Context.Devices.ToListAsync();
                groupsViewSource.Source = Context.Groups.Local;
            }
        }

        private void GroupEditor_onEntityAdded(object sender, NotifyEntityChangeContext.ChangeNotifications<Device>.EntityAddedArgs e)
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

            NotifyEntityChangeContext.ChangeNotifications<Group>.OnEntityDeleted -= ChangeNotificationsOnOnEntityDeleted;
            NotifyEntityChangeContext.ChangeNotifications<Group>.OnEntityUpdated -= GroupEditorUserControl_OnEntityUpdated;
            Context.Dispose();
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

        private async void ButtonRemoveDevice_OnClick(object sender, RoutedEventArgs e)
        {
            var selectedGroup = GroupsDataGrid.SelectedItem as Group;
            if (selectedGroup == null || GroupsDevicesLstVw.SelectedItems.Count <= 0) return;
            if (MessageBox.Show(
                $"Are you sure you want to remove the {GroupsDevicesLstVw.SelectedItems.Count} selected devices from this group?",
                "Remove Devices?",
                MessageBoxButton.YesNo,
                MessageBoxImage.Error) != MessageBoxResult.Yes)
                return;

            var devicesToRemove = new Device[GroupsDevicesLstVw.SelectedItems.Count];
            GroupsDevicesLstVw.SelectedItems.CopyTo(devicesToRemove, 0);

            foreach (var gd in devicesToRemove)
                selectedGroup.Devices.Remove(gd);

            await SaveChangesAsync();
        }

        private async void ButtonAddDevice_OnClick(object sender, RoutedEventArgs e)
        {
            var group = GroupsDataGrid.SelectedItem as Group;
            if (group == null) return;

            var deviceWindow = new DeviceMultiselectWindow(new ZvsEntityContextConnection(), group.Devices.Select(o => o.Id).ToList())
            {
                Owner = Window.GetWindow(this)
            };
            var result = deviceWindow.ShowDialog();
            if (!result.HasValue || !result.Value) return;

            var selectDeviceIds = deviceWindow.SelectedDevices.Select(o => o.Id);
            var devicesToAdd = await Context.Devices.Where(o => selectDeviceIds.Contains(o.Id) && o.Groups.All(p => p.Id != group.Id)).ToListAsync();

            foreach (var device in devicesToAdd)
            {
                group.Devices.Add(device);
            }
            await SaveChangesAsync();
        }

        private async void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
        {
            var group = GroupsDataGrid.SelectedItem as Group;
            if (group == null) return;

            if (MessageBox.Show($"Are you sure you want to delete the '{@group.Name}' group?",
                "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;
            Context.Groups.Local.Remove(group);
            await SaveChangesAsync();
            GroupsDataGrid.Focus();
        }

        private async Task SaveChangesAsync()
        {
            var result = await Context.TrySaveChangesAsync(_app.Cts.Token);
            if (result.HasError)
                await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving group. {0}", result.Message);

            SignalImg.Opacity = 1;
            var da = new DoubleAnimation { From = 1, To = 0, Duration = new Duration(TimeSpan.FromSeconds(.8)) };
            SignalImg.BeginAnimation(OpacityProperty, da);
        }

        void GroupEditorUserControl_OnEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<Group>.EntityUpdatedArgs e)
        {
            if (Context == null)
                return;

            Dispatcher.Invoke(() =>
            {
                //Update the primitives used in this user control
                var group = Context.Groups.Local.FirstOrDefault(o => o.Id == e.NewEntity.Id);
                if (group == null)
                    return;

                group.Name = e.NewEntity.Name;
                group.Description = e.NewEntity.Description;
            });
        }

        private void ChangeNotificationsOnOnEntityDeleted(object sender, NotifyEntityChangeContext.ChangeNotifications<Group>.EntityDeletedArgs entityDeletedArgs)
        {
            if (Context == null)
                return;

            Dispatcher.Invoke(new Action(async () =>
            {
                Context.Groups.Local.Remove(entityDeletedArgs.DeletedEntity);
                //Context.Entry(e.DeletedEntity).State = EntityState.Unchanged;

                //Reloads context from DB when modifications happen
                foreach (var ent in Context.ChangeTracker.Entries<Group>())
                    await ent.ReloadAsync();
            }));
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
    }
}
