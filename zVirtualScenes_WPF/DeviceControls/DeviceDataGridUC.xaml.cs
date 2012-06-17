using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using System.ComponentModel;
using zVirtualScenesModel;
using System.Collections.ObjectModel;


namespace zVirtualScenes_WPF.DeviceControls
{
    /// <summary>
    /// Interaction logic for DeviceDataGridUC.xaml
    /// </summary>
    public partial class DeviceDataGridUC : UserControl
    {
        private zvsLocalDBEntities context;

        public DeviceDataGridUC()
        {
            InitializeComponent();
        }

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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Do not load your data at design time.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                context = new zvsLocalDBEntities();

                //Load your data here and assign the result to the CollectionViewSource.
                System.Windows.Data.CollectionViewSource myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["devicesViewSource"];
                context.devices.ToList();
                myCollectionViewSource.Source = context.devices.Local;
            }

            zvsLocalDBEntities.onDevicesChanged += zvsLocalDBEntities_onDevicesChanged;
            zvsLocalDBEntities.onGroup_DevicesChanged += zvsLocalDBEntities_onGroup_DevicesChanged;
            zvsLocalDBEntities.onGroupsChanged += zvsLocalDBEntities_onGroupsChanged;
        }

        void zvsLocalDBEntities_onGroupsChanged(object sender, zvsLocalDBEntities.onEntityChangedEventArgs args)
        {
            if (MinimalistDisplay)
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    if (context != null)
                    {
                        if (args.ChangeType == System.Data.EntityState.Added)
                        {
                            //Gets new devices
                            context.groups.ToList();
                        }
                        else
                        {
                            //Reloads context from DB when modifcations happen
                            foreach (var ent in context.ChangeTracker.Entries<group>())
                                ent.Reload();
                        }
                    }
                }));
            }
        }

        void zvsLocalDBEntities_onGroup_DevicesChanged(object sender, zvsLocalDBEntities.onEntityChangedEventArgs args)
        {
            if (MinimalistDisplay)
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    if (context != null)
                    {
                        if (args.ChangeType == System.Data.EntityState.Added)
                        {
                            //Gets new devices
                            context.group_devices.ToList();
                        }
                        else
                        {
                            //Reloads context from DB when modifcations happen
                            foreach (var ent in context.ChangeTracker.Entries<group_devices>())
                                ent.Reload();
                        }
                    }
                }));
            }
        }

        void zvsLocalDBEntities_onDevicesChanged(object sender, zvsLocalDBEntities.onEntityChangedEventArgs args)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (context != null)
                {
                    if (args.ChangeType == System.Data.EntityState.Added)
                    {
                        //Gets new devices
                        context.devices.ToList();
                    }
                    else
                    {
                        //Reloads context from DB when modifcations happen
                        foreach (var ent in context.ChangeTracker.Entries<device>())
                            ent.Reload();
                    }
                }
            }));
        }

        private void UserControl_Unloaded_1(object sender, RoutedEventArgs e)
        {
            zvsLocalDBEntities.onDevicesChanged -= zvsLocalDBEntities_onDevicesChanged;
            zvsLocalDBEntities.onGroup_DevicesChanged -= zvsLocalDBEntities_onGroup_DevicesChanged;
            zvsLocalDBEntities.onGroupsChanged -= zvsLocalDBEntities_onGroupsChanged;
           
        }

        ////User Events

        private void UserControl_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //if (_MinimalistDisplay)
            //{
            //    ////Context Menus
            //    if (DeviceGrid.SelectedItems.Count > 0)
            //    {
            //        device[] SelectedItemsCopy = new device[DeviceGrid.SelectedItems.Count];
            //        DeviceGrid.SelectedItems.CopyTo(SelectedItemsCopy, 0);

            //        ContextMenu menu = new ContextMenu();

            //        MenuItem delete = new MenuItem();
            //        delete.Header = "Remove " + (DeviceGrid.SelectedItems.Count > 1 ? DeviceGrid.SelectedItems.Count + " Devices" : "Device");
            //        delete.Click += (s, args) =>
            //        {
            //            if (
            //           MessageBox.Show("Are you sure you want to delete the selected devices?",
            //                           "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            //            {
            //                foreach (device selectedDevice in SelectedItemsCopy)
            //                {
            //                    device d = context.devices.FirstOrDefault(o => o.id == selectedDevice.id);
            //                    if (d != null)
            //                        context.devices.Remove(d);
            //                }

            //                context.SaveChanges();
            //            }
            //        };

            //        menu.Items.Add(delete);
            //        ContextMenu = menu;
            //    }
            //    else
            //        ContextMenu = null;
            //}
        }

        private void DeleteSelectedItems()
        {
            if (DeviceGrid.SelectedItems.Count > 0)
            {
                device[] SelectedItemsCopy = new device[DeviceGrid.SelectedItems.Count];
                DeviceGrid.SelectedItems.CopyTo(SelectedItemsCopy, 0);

                foreach (device selectedDevice in SelectedItemsCopy)
                {
                    device d = context.devices.FirstOrDefault(o => o.id == selectedDevice.id);
                    if (d != null)
                        context.devices.Remove(d);
                }
                context.SaveChanges();
            }
        }

        //Drag
        private static bool IsMouseOverDragImage(object sender, Point mousePosition)
        {
            if (sender is Visual)
            {
                HitTestResult hit = VisualTreeHelper.HitTest(sender as Visual, mousePosition);

                if (hit == null) return false;

                DependencyObject dObj = hit.VisualHit;
                while (dObj != null)
                {
                    if (dObj is Image)
                    {
                        Image image = (Image)dObj;
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
                    DataObject dataObject = new DataObject("objects", DeviceGrid.SelectedItems);

                    var devices = DeviceGrid.SelectedItems.OfType<device>().ToList();
                    if (devices.Count > 0)
                        dataObject.SetData("deviceList", devices);

                    var scenes = DeviceGrid.SelectedItems.OfType<scene>().ToList();
                    if (scenes.Count > 0)
                        dataObject.SetData("sceneList", scenes);

                    DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Move);
                }
            }
        }

        private void DeviceGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                //have to add , UpdateSourceTrigger=PropertyChanged to have the data updated intime for this event
                context.SaveChanges();
                ////device.CallOnContextUpdated();
            }
        }

        private void OpenDeviceDetails(device d)
        {
            DeviceDetailsWindow deviceDetailsWindow = new DeviceDetailsWindow(d.id);
            deviceDetailsWindow.Owner = Application.Current.MainWindow;
            deviceDetailsWindow.Show();
        }

        private void SettingBtn_Click_1(object sender, RoutedEventArgs e)
        {
            Object obj = ((FrameworkElement)sender).DataContext;
            if (obj is device)
            {
                var device = (device)obj;
                if (device != null)
                {
                    OpenDeviceDetails(device);
                }
            }
        }

        string _searchstr = string.Empty;
        private void Filter_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            if (textbox != null)
            {
                _searchstr = textbox.Text;
                System.Windows.Data.CollectionViewSource myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["devicesViewSource"];
                ICollectionView view = myCollectionViewSource.View;
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
            if (item is device)
            {
                device d = (device)item;
                if (d.friendly_name.ToLower().Contains(_searchstr.ToLower()))
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

        private void Grid_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (_MinimalistDisplay)
                {

                    if (MessageBox.Show("Are you sure you want to delete the selected devices?",
                                        "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        DeleteSelectedItems();
                    }
                }
                e.Handled = true;
            }
        }
    }
}
