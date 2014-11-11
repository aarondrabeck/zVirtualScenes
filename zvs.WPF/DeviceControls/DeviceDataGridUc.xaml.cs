using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.ComponentModel;
using zvs.DataModel;
using System.Data.Entity;
using System.Threading.Tasks;
using zvs.Processor;


namespace zvs.WPF.DeviceControls
{
    /// <summary>
    /// Interaction logic for DeviceDataGridUC.xaml
    /// </summary>
    public partial class DeviceDataGridUc : IDisposable
    {
        private ZvsContext Context { get; set; }
        private IFeedback<LogEntry> Log { get; set; }
        private readonly App _app = (App)Application.Current;

        public DeviceDataGridUc()
        {
            Context = new ZvsContext(_app.EntityContextConnection);
            Log = new DatabaseFeedback(_app.EntityContextConnection) { Source = "Device Grid" };

            InitializeComponent();

            NotifyEntityChangeContext.ChangeNotifications<Device>.OnEntityAdded += DeviceDataGridUC_onEntityAdded;
            NotifyEntityChangeContext.ChangeNotifications<Device>.OnEntityDeleted += DeviceDataGridUC_onEntityDeleted;
            NotifyEntityChangeContext.ChangeNotifications<Device>.OnEntityUpdated += DeviceDataGridUC_onEntityUpdated;

            NotifyEntityChangeContext.ChangeNotifications<Group>.OnEntityAdded += DeviceDataGridUC_onEntityAdded;
            NotifyEntityChangeContext.ChangeNotifications<Group>.OnEntityDeleted += DeviceDataGridUC_onEntityDeleted;
            NotifyEntityChangeContext.ChangeNotifications<Group>.OnEntityUpdated += DeviceDataGridUC_onEntityUpdated;

            //TODO: LISTEN FOR CHANGES TO THE DEVICE_GROUPS LINKING TABLE

        }

        void DeviceDataGridUC_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<Group>.EntityUpdatedArgs e)
        {
            if (Context == null)
                return;

            if (!MinimalistDisplay)
                return;

            Dispatcher.Invoke(new Action(async () =>
            {
                //Reloads context from DB when modifications happen
                foreach (var ent in Context.ChangeTracker.Entries<Group>())
                    await ent.ReloadAsync();
            }));

        }

        void DeviceDataGridUC_onEntityDeleted(object sender, NotifyEntityChangeContext.ChangeNotifications<Group>.EntityDeletedArgs e)
        {
            if (Context == null)
                return;

            if (!MinimalistDisplay)
                return;

            Dispatcher.Invoke(new Action(async () =>
            {
                //Reloads context from DB when modifications happen
                foreach (var ent in Context.ChangeTracker.Entries<Group>())
                    await ent.ReloadAsync();
            }));
        }

        void DeviceDataGridUC_onEntityAdded(object sender, NotifyEntityChangeContext.ChangeNotifications<Group>.EntityAddedArgs e)
        {
            if (Context == null)
                return;

            if (!MinimalistDisplay)
                return;

            Dispatcher.Invoke(new Action(async () =>
            {
                await Context.Groups.ToListAsync();
            }));
        }

        void DeviceDataGridUC_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<Device>.EntityUpdatedArgs e)
        {
            if (Context == null)
                return;

            Dispatcher.Invoke(async () =>
            {
                //Update the primitives used in this user control
                var device = Context.Devices.Local.FirstOrDefault(o => o.Id == e.NewEntity.Id);
                if (device == null)
                    return;

                device.DeviceTypeId = e.NewEntity.DeviceTypeId;
                device.CurrentLevelInt = e.NewEntity.CurrentLevelInt;
                device.CurrentLevelText = e.NewEntity.CurrentLevelText;
                device.LastHeardFrom = e.NewEntity.LastHeardFrom;
                device.NodeNumber = e.NewEntity.NodeNumber;
                device.Name = e.NewEntity.Name;
                device.Location = e.NewEntity.Location;

                // var entry = context.Entry(e.NewEntity);
                // entry.State = EntityState.Unchanged;

                if (e.NewEntity.DeviceTypeId != e.OldEntity.DeviceTypeId)
                    await Context.DeviceTypes.FirstOrDefaultAsync(o => o.Id == e.NewEntity.DeviceTypeId);

            });
        }

        void DeviceDataGridUC_onEntityDeleted(object sender, NotifyEntityChangeContext.ChangeNotifications<Device>.EntityDeletedArgs e)
        {
            if (Context == null)
                return;

            Dispatcher.Invoke(new Action(async () =>
            {
                Context.Devices.Local.Remove(e.DeletedEntity);
                Context.Entry(e.DeletedEntity).State = EntityState.Unchanged;

                //Reloads context from DB when modifications happen
                foreach (var ent in Context.ChangeTracker.Entries<Device>())
                    await ent.ReloadAsync();
            }));
        }

        void DeviceDataGridUC_onEntityAdded(object sender, NotifyEntityChangeContext.ChangeNotifications<Device>.EntityAddedArgs e)
        {
            if (Context == null)
                return;

            Dispatcher.Invoke(() =>
            {
                Context.Devices.Local.Add(e.AddedEntity);
                Context.Entry(e.AddedEntity).State = EntityState.Unchanged;

            });
        }

        private async void UserControl_Initialized(object sender, EventArgs e)
        {

#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
#endif
            // Do not load your data at design time.
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                await Context.Devices
                       .Include(o => o.Groups)
                       .Include(o => o.Type)
                       .ToListAsync();

                //Load your data here and assign the result to the CollectionViewSource.
                var myCollectionViewSource = (CollectionViewSource)Resources["devicesViewSource"];
                myCollectionViewSource.Source = Context.Devices.Local;


            }

#if DEBUG
            sw.Stop();
            Debug.WriteLine("Device grid initialized in {0}", sw.Elapsed.ToString() as object);
#endif
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var myCollectionViewSource = (CollectionViewSource)Resources["devicesViewSource"];
            myCollectionViewSource.SortDescriptions.Clear();
            myCollectionViewSource.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

        }

#if DEBUG
        ~DeviceDataGridUc()
        {
            //Cannot write to log here, it has been disposed. 
            Debug.WriteLine("DeviceDataGridUC Deconstructed");
        }
#endif

        private bool _showMore;
        public bool ShowMore
        {
            get
            {
                return _showMore;
            }
            set
            {
                _showMore = value;
                if (value == false)
                {
                    DateCol.Visibility = Visibility.Collapsed;
                    DeviceTypeCol.Visibility = Visibility.Collapsed;
                    GroupCol.Visibility = Visibility.Collapsed;
                    NodeID.Visibility = Visibility.Collapsed;

                }
                else
                {
                    DateCol.Visibility = Visibility.Visible;
                    DeviceTypeCol.Visibility = Visibility.Visible;
                    GroupCol.Visibility = Visibility.Visible;
                    NodeID.Visibility = Visibility.Visible;
                }
            }
        }

        private bool _minimalistDisplay = true;
        public bool MinimalistDisplay
        {
            get
            {
                return _minimalistDisplay;
            }
            set
            {
                _minimalistDisplay = value;
                if (value == false)
                {
                    DateCol.Visibility = Visibility.Collapsed;
                    LevelCol.Visibility = Visibility.Collapsed;
                    DeviceTypeCol.Visibility = Visibility.Collapsed;
                    GroupCol.Visibility = Visibility.Collapsed;
                    NodeID.Visibility = Visibility.Collapsed;
                    SettingsCol.Visibility = Visibility.Collapsed;

                }
                else
                {
                    DateCol.Visibility = Visibility.Visible;
                    LevelCol.Visibility = Visibility.Visible;
                    DeviceTypeCol.Visibility = Visibility.Visible;
                    GroupCol.Visibility = Visibility.Visible;
                    SettingsCol.Visibility = Visibility.Visible;
                }
            }
        }

        private void UserControl_Unloaded_1(object sender, RoutedEventArgs e)
        {
            var parent = Window.GetWindow(this);
            //Check if the parent window is closing  or if this is just being removed from the visual tree temporarily
            if (parent == null || !parent.IsActive)
            {
                NotifyEntityChangeContext.ChangeNotifications<Device>.OnEntityAdded -= DeviceDataGridUC_onEntityAdded;
                NotifyEntityChangeContext.ChangeNotifications<Device>.OnEntityDeleted -= DeviceDataGridUC_onEntityDeleted;
                NotifyEntityChangeContext.ChangeNotifications<Device>.OnEntityUpdated -= DeviceDataGridUC_onEntityUpdated;

                NotifyEntityChangeContext.ChangeNotifications<Group>.OnEntityAdded -= DeviceDataGridUC_onEntityAdded;
                NotifyEntityChangeContext.ChangeNotifications<Group>.OnEntityDeleted -= DeviceDataGridUC_onEntityDeleted;
                NotifyEntityChangeContext.ChangeNotifications<Group>.OnEntityUpdated -= DeviceDataGridUC_onEntityUpdated;
            }
        }

        public static Window FindParentWindow(DependencyObject child)
        {
            var parent = VisualTreeHelper.GetParent(child);

            //CHeck if this is the end of the tree
            if (parent == null) return null;

            var parentWindow = parent as Window;
            return parentWindow ?? FindParentWindow(parent);
        }

        ////User Events
        private async Task DeleteSelectedItemsAsync()
        {
            if (DeviceGrid.SelectedItems.Count > 0)
            {
                var selectedItemsCopy = new Device[DeviceGrid.SelectedItems.Count];
                DeviceGrid.SelectedItems.CopyTo(selectedItemsCopy, 0);

                foreach (var selectedDevice in selectedItemsCopy)
                {
                    var device = selectedDevice;
                    var d = await Context.Devices.FirstOrDefaultAsync(o => o.Id == device.Id);
                    if (d == null) continue;
                    //Check for device dependencies
                    foreach (var dvt in await Context.DeviceValueTriggers.Where(t => t.DeviceValue.Device.Id == d.Id).ToListAsync())
                    {
                        var windowResult = MessageBox.Show(
                            string.Format("Deleting device '{0}' will delete trigger '{1}', would you like continue?",
                                d.Name,
                                dvt.Name),
                            "Device Delete Warning",
                            MessageBoxButton.YesNo);

                        if (windowResult == MessageBoxResult.Yes)
                        {
                            Context.DeviceValueTriggers.Local.Remove(dvt);

                            var result = await Context.TrySaveChangesAsync(_app.Cts.Token);
                            if (result.HasError)
                                await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error deleting device triggers. {0}", result.Message);
                        }
                        else
                            return;
                    }

                    Context.Devices.Local.Remove(d);

                    var r2 = await Context.TrySaveChangesAsync(_app.Cts.Token);
                    if (r2.HasError)
                        await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error deleting device. {0}", r2.Message);
                }
            }
        }

        //Drag
        private static bool IsMouseOverDragImage(object sender, Point mousePosition)
        {
            if (!(sender is Visual)) return false;
            var hit = VisualTreeHelper.HitTest(sender as Visual, mousePosition);

            if (hit == null) return false;

            var dObj = hit.VisualHit;
            while (dObj != null)
            {
                var obj = dObj as Image;
                if (obj != null)
                {
                    var image = obj;
                    if (image.Name == "DragImage")
                        return true;
                }

                if ((dObj is Visual) || (dObj is Visual3D)) dObj = VisualTreeHelper.GetParent(dObj);
                else dObj = LogicalTreeHelper.GetParent(dObj);
            }
            return false;
        }

        private void DeviceGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //if the mouse is down over the drag icon, do the drag
            if (!IsMouseOverDragImage(sender, e.GetPosition(sender as IInputElement))) return;
            if (DeviceGrid.SelectedItems.Count <= 0) return;
            var dataObject = new DataObject("objects", DeviceGrid.SelectedItems);

            var devices = DeviceGrid.SelectedItems.OfType<Device>().ToList();
            if (devices.Count > 0)
                dataObject.SetData("deviceList", devices);

            var scenes = DeviceGrid.SelectedItems.OfType<Scene>().ToList();
            if (scenes.Count > 0)
                dataObject.SetData("sceneList", scenes);

            DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Move);
        }

        private async void DeviceGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction != DataGridEditAction.Commit) return;
            //have to add , UpdateSourceTrigger=PropertyChanged to have the data updated intime for this event
            var result = await Context.TrySaveChangesAsync(_app.Cts.Token);
            if (result.HasError)
                await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving device. {0}", result.Message);
            ////device.CallOnContextUpdated();
        }

        private void OpenDeviceDetails(Device d)
        {
            var app = (App)Application.Current;
            var deviceDetailsWindow = new DeviceDetailsWindow(d.Id) {Owner = app.ZvsWindow};
            deviceDetailsWindow.Show();
        }

        private void SettingBtn_Click_1(object sender, RoutedEventArgs e)
        {
            var device = (((FrameworkElement)sender).DataContext) as Device;
            if (device == null) return;

            OpenDeviceDetails(device);
        }

        string _searchstr = string.Empty;
        private void Filter_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            var textbox = sender as TextBox;
            if (textbox == null) return;
            _searchstr = textbox.Text;
            var myCollectionViewSource = (CollectionViewSource)Resources["devicesViewSource"];
            var view = myCollectionViewSource.View;
            view.Filter = !string.IsNullOrEmpty(_searchstr) ?  new Predicate<object>(DeviceNameFilter) : null;
        }

        private bool DeviceNameFilter(object item)
        {
            var device = item as Device;
            return device != null && device.Name.ToLower().Contains(_searchstr.ToLower());
        }

        private void ShowMoreBtn_Checked_1(object sender, RoutedEventArgs e)
        {
            ShowMore = true;
        }

        private void ShowMoreBtn_Unchecked_1(object sender, RoutedEventArgs e)
        {
            ShowMore = false;
        }

        private async void Grid_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Delete) return;
            if (_minimalistDisplay)
            {

                if (MessageBox.Show("Are you sure you want to delete the selected devices?",
                    "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    await DeleteSelectedItemsAsync();
                }
            }
            e.Handled = true;
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
