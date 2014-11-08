using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.ComponentModel;
using zvs.DataModel;
using System.Data.Entity;
using System.Threading.Tasks;


namespace zvs.WPF.DeviceControls
{
    /// <summary>
    /// Interaction logic for DeviceDataGridUC.xaml
    /// </summary>
    public partial class DeviceDataGridUC : UserControl, IDisposable
    {
        private ZvsContext context;
        public DeviceDataGridUC()
        {
            context = new ZvsContext();

            InitializeComponent();

            ZvsContext.ChangeNotifications<Device>.OnEntityAdded += DeviceDataGridUC_onEntityAdded;
            ZvsContext.ChangeNotifications<Device>.OnEntityDeleted += DeviceDataGridUC_onEntityDeleted;
            ZvsContext.ChangeNotifications<Device>.OnEntityUpdated += DeviceDataGridUC_onEntityUpdated;

            ZvsContext.ChangeNotifications<Group>.OnEntityAdded += DeviceDataGridUC_onEntityAdded;
            ZvsContext.ChangeNotifications<Group>.OnEntityDeleted += DeviceDataGridUC_onEntityDeleted;
            ZvsContext.ChangeNotifications<Group>.OnEntityUpdated += DeviceDataGridUC_onEntityUpdated;

            //TODO: LISTEN FOR CHANGES TO THE DEVICE_GROUPS LINKING TABLE

        }

        void DeviceDataGridUC_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<Group>.EntityUpdatedArgs e)
        {
            if (context == null)
                return;

            if (!MinimalistDisplay)
                return;

            this.Dispatcher.Invoke(new Action(async () =>
            {
                //Reloads context from DB when modifications happen
                foreach (var ent in context.ChangeTracker.Entries<Group>())
                    await ent.ReloadAsync();
            }));

        }

        void DeviceDataGridUC_onEntityDeleted(object sender, NotifyEntityChangeContext.ChangeNotifications<Group>.EntityDeletedArgs e)
        {
            if (context == null)
                return;

            if (!MinimalistDisplay)
                return;

            this.Dispatcher.Invoke(new Action(async () =>
            {
                //Reloads context from DB when modifications happen
                foreach (var ent in context.ChangeTracker.Entries<Group>())
                    await ent.ReloadAsync();
            }));
        }

        void DeviceDataGridUC_onEntityAdded(object sender, NotifyEntityChangeContext.ChangeNotifications<Group>.EntityAddedArgs e)
        {
            if (context == null)
                return;

            if (!MinimalistDisplay)
                return;

            this.Dispatcher.Invoke(new Action(async () =>
            {
                await context.Groups.ToListAsync();
            }));
        }

        void DeviceDataGridUC_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<Device>.EntityUpdatedArgs e)
        {
            if (context == null)
                return;

            this.Dispatcher.Invoke(async () =>
            {
                //Update the primitives used in this user control
                var device = context.Devices.Local.FirstOrDefault(o => o.Id == e.NewEntity.Id);
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
                    await context.DeviceTypes.FirstOrDefaultAsync(o => o.Id == e.NewEntity.DeviceTypeId);

            });
        }

        void DeviceDataGridUC_onEntityDeleted(object sender, NotifyEntityChangeContext.ChangeNotifications<Device>.EntityDeletedArgs e)
        {
            if (context == null)
                return;

            this.Dispatcher.Invoke(new Action(async () =>
            {
                context.Devices.Local.Remove(e.DeletedEntity);
                var entry = context.Entry(e.DeletedEntity).State = EntityState.Unchanged;

                //Reloads context from DB when modifications happen
                foreach (var ent in context.ChangeTracker.Entries<Device>())
                    await ent.ReloadAsync();
            }));
        }

        void DeviceDataGridUC_onEntityAdded(object sender, NotifyEntityChangeContext.ChangeNotifications<Device>.EntityAddedArgs e)
        {
            if (context == null)
                return;

            this.Dispatcher.Invoke(() =>
            {
                context.Devices.Local.Add(e.AddedEntity);
                context.Entry(e.AddedEntity).State = EntityState.Unchanged;

            });
        }

        private async void UserControl_Initialized(object sender, EventArgs e)
        {

#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
#endif
            // Do not load your data at design time.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                await context.Devices
                       .Include(o => o.Groups)
                       .Include(o => o.Type)
                       .ToListAsync();

                //Load your data here and assign the result to the CollectionViewSource.
                var myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["devicesViewSource"];
                myCollectionViewSource.Source = context.Devices.Local;


            }

#if DEBUG
            sw.Stop();
            Debug.WriteLine("Device grid initialized in {0}", sw.Elapsed.ToString() as object);
#endif
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["devicesViewSource"];
            myCollectionViewSource.SortDescriptions.Clear();
            myCollectionViewSource.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

        }

#if DEBUG
        ~DeviceDataGridUC()
        {
            //Cannot write to log here, it has been disposed. 
            Debug.WriteLine("DeviceDataGridUC Deconstructed");
        }
#endif

        private bool _ShowMore = false;
        public bool ShowMore
        {
            get
            {
                return _ShowMore;
            }
            set
            {
                _ShowMore = value;
                if (value == false)
                {
                    DateCol.Visibility = System.Windows.Visibility.Collapsed;
                    DeviceTypeCol.Visibility = System.Windows.Visibility.Collapsed;
                    GroupCol.Visibility = System.Windows.Visibility.Collapsed;
                    NodeID.Visibility = System.Windows.Visibility.Collapsed;

                }
                else
                {
                    DateCol.Visibility = System.Windows.Visibility.Visible;
                    DeviceTypeCol.Visibility = System.Windows.Visibility.Visible;
                    GroupCol.Visibility = System.Windows.Visibility.Visible;
                    NodeID.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }

        private bool _MinimalistDisplay = true;
        public bool MinimalistDisplay
        {
            get
            {
                return _MinimalistDisplay;
            }
            set
            {
                _MinimalistDisplay = value;
                if (value == false)
                {
                    DateCol.Visibility = System.Windows.Visibility.Collapsed;
                    LevelCol.Visibility = System.Windows.Visibility.Collapsed;
                    DeviceTypeCol.Visibility = System.Windows.Visibility.Collapsed;
                    GroupCol.Visibility = System.Windows.Visibility.Collapsed;
                    NodeID.Visibility = System.Windows.Visibility.Collapsed;
                    SettingsCol.Visibility = System.Windows.Visibility.Collapsed;

                }
                else
                {
                    DateCol.Visibility = System.Windows.Visibility.Visible;
                    LevelCol.Visibility = System.Windows.Visibility.Visible;
                    DeviceTypeCol.Visibility = System.Windows.Visibility.Visible;
                    GroupCol.Visibility = System.Windows.Visibility.Visible;
                    SettingsCol.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }

        private void UserControl_Unloaded_1(object sender, RoutedEventArgs e)
        {
            var parent = Window.GetWindow(this);
            //Check if the parent window is closing  or if this is just being removed from the visual tree temporarily
            if (parent == null || !parent.IsActive)
            {
                ZvsContext.ChangeNotifications<Device>.OnEntityAdded -= DeviceDataGridUC_onEntityAdded;
                ZvsContext.ChangeNotifications<Device>.OnEntityDeleted -= DeviceDataGridUC_onEntityDeleted;
                ZvsContext.ChangeNotifications<Device>.OnEntityUpdated -= DeviceDataGridUC_onEntityUpdated;

                ZvsContext.ChangeNotifications<Group>.OnEntityAdded -= DeviceDataGridUC_onEntityAdded;
                ZvsContext.ChangeNotifications<Group>.OnEntityDeleted -= DeviceDataGridUC_onEntityDeleted;
                ZvsContext.ChangeNotifications<Group>.OnEntityUpdated -= DeviceDataGridUC_onEntityUpdated;
            }
        }

        public static Window FindParentWindow(DependencyObject child)
        {
            var parent = VisualTreeHelper.GetParent(child);

            //CHeck if this is the end of the tree
            if (parent == null) return null;

            var parentWindow = parent as Window;
            if (parentWindow != null)
            {
                return parentWindow;
            }
            else
            {
                //use recursion until it reaches a Window
                return FindParentWindow(parent);
            }
        }

        ////User Events
        private async Task DeleteSelectedItemsAsync()
        {
            if (DeviceGrid.SelectedItems.Count > 0)
            {
                var SelectedItemsCopy = new Device[DeviceGrid.SelectedItems.Count];
                DeviceGrid.SelectedItems.CopyTo(SelectedItemsCopy, 0);

                foreach (var selectedDevice in SelectedItemsCopy)
                {
                    var d = await context.Devices.FirstOrDefaultAsync(o => o.Id == selectedDevice.Id);
                    if (d != null)
                    {
                        //Check for device dependencies
                        foreach (var dvt in await context.DeviceValueTriggers.Where(t => t.DeviceValue.Device.Id == d.Id).ToListAsync())
                        {
                            var result = MessageBox.Show(
                                string.Format("Deleting device '{0}' will delete trigger '{1}', would you like continue?",
                                                d.Name,
                                                dvt.Name),
                                "Device Delete Warning",
                                MessageBoxButton.YesNo);

                            if (result == MessageBoxResult.Yes)
                            {
                                context.DeviceValueTriggers.Local.Remove(dvt);

                                var saveResult = await context.TrySaveChangesAsync();
                                if (saveResult.HasError)
                                    ((App)App.Current).ZvsEngine.log.Error(saveResult.Message);
                            }
                            else
                                return;
                        }

                        context.Devices.Local.Remove(d);

                        var r = await context.TrySaveChangesAsync();
                        if (r.HasError)
                            ((App)App.Current).ZvsEngine.log.Error(r.Message);
                    }
                }
            }
        }

        //Drag
        private static bool IsMouseOverDragImage(object sender, Point mousePosition)
        {
            if (sender is Visual)
            {
                var hit = VisualTreeHelper.HitTest(sender as Visual, mousePosition);

                if (hit == null) return false;

                var dObj = hit.VisualHit;
                while (dObj != null)
                {
                    if (dObj is Image)
                    {
                        var image = (Image)dObj;
                        if (image.Name == "DragImage")
                            return true;
                    }

                    if ((dObj is Visual) || (dObj is Visual3D)) dObj = VisualTreeHelper.GetParent(dObj);
                    else dObj = LogicalTreeHelper.GetParent(dObj);
                }
            }
            return false;
        }

        private void DeviceGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //if the mouse is down over the drag icon, do the drag
            if (IsMouseOverDragImage(sender, e.GetPosition(sender as IInputElement)))
            {
                if (DeviceGrid.SelectedItems.Count > 0)
                {
                    var dataObject = new DataObject("objects", DeviceGrid.SelectedItems);

                    var devices = DeviceGrid.SelectedItems.OfType<Device>().ToList();
                    if (devices.Count > 0)
                        dataObject.SetData("deviceList", devices);

                    var scenes = DeviceGrid.SelectedItems.OfType<Scene>().ToList();
                    if (scenes.Count > 0)
                        dataObject.SetData("sceneList", scenes);

                    DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Move);
                }
            }
        }

        private async void DeviceGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                //have to add , UpdateSourceTrigger=PropertyChanged to have the data updated intime for this event
                var result = await context.TrySaveChangesAsync();
                if (result.HasError)
                    ((App)App.Current).ZvsEngine.log.Error(result.Message);
                ////device.CallOnContextUpdated();
            }
        }

        private void OpenDeviceDetails(Device d)
        {
            var app = (App)Application.Current;
            var deviceDetailsWindow = new DeviceDetailsWindow(d.Id);
            deviceDetailsWindow.Owner = app.ZvsWindow;
            deviceDetailsWindow.Show();
        }

        private void SettingBtn_Click_1(object sender, RoutedEventArgs e)
        {
            var obj = ((FrameworkElement)sender).DataContext;
            if (obj is Device)
            {
                var device = (Device)obj;
                if (device != null)
                {
                    OpenDeviceDetails(device);
                }
            }
        }

        string _searchstr = string.Empty;
        private void Filter_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            var textbox = sender as TextBox;
            if (textbox != null)
            {
                _searchstr = textbox.Text;
                var myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["devicesViewSource"];
                var view = myCollectionViewSource.View;
                if (!string.IsNullOrEmpty(_searchstr))
                {


                    view.Filter = new Predicate<object>(filter);
                }
                else
                {
                    view.Filter = null;
                }
            }
        }

        private bool filter(object item)
        {
            if (item is Device)
            {
                var d = (Device)item;
                if (d.Name.ToLower().Contains(_searchstr.ToLower()))
                {
                    return true;
                }
            }
            return false;
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
            if (e.Key == Key.Delete)
            {
                if (_MinimalistDisplay)
                {

                    if (MessageBox.Show("Are you sure you want to delete the selected devices?",
                                        "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        await DeleteSelectedItemsAsync();
                    }
                }
                e.Handled = true;
            }
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
