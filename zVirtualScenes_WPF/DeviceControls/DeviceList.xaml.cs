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
using zVirtualScenesCommon.Entity;
using System.Collections.Specialized;
using System.Windows.Media.Animation;
using System.Data.Objects;

namespace zVirtualScenes_WPF.DeviceControls
{
    /// <summary>
    /// interaction logic for DeviceList.xaml
    /// </summary>
    public partial class DeviceList : UserControl
    {
        private zvsEntities2 context;

        public DeviceList()
        {
            InitializeComponent();

        }

        private bool isContextDisposed = false; 
        private bool isEditing = false;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            context = new zvsEntities2(zvsEntityControl.GetzvsConnectionString);
            isContextDisposed = false; 

            CollectionViewSource customersViewSource = ((CollectionViewSource)(FindResource("zvsEntities2devicesViewSource")));
            var deviceQuery = context.devices;
            deviceQuery.Include("group_devices");
            deviceQuery.Include("group_devices/group");
                       
            customersViewSource.Source = deviceQuery.Execute(MergeOption.AppendOnly);

            zvsEntityControl.onSaveChanges += new zvsEntityControl.onSaveChangesEventHandler(zvsEntityControl_onSaveChanges);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            zvsEntityControl.onSaveChanges -= new zvsEntityControl.onSaveChangesEventHandler(zvsEntityControl_onSaveChanges);
            isContextDisposed = true;
            context.Dispose();
        }

        private void zvsEntityControl_onSaveChanges(zvsEntityControl.onSaveChangesEventArgs args)
        {
            if (args.TablesChanged.Contains(zvsEntityControl.Tables.device))
                context.Refresh(RefreshMode.StoreWins, context.devices);

            //refresh the data from methods.
            if (args.TablesChanged.Contains(zvsEntityControl.Tables.group_device))
            {
                if (!isEditing)
                {
                    CollectionViewSource customersViewSource = ((CollectionViewSource)(FindResource("zvsEntities2devicesViewSource")));
                    var deviceQuery = context.devices;
                    deviceQuery.Include("group_devices");
                    deviceQuery.Include("group_devices/group");

                    customersViewSource.Source = deviceQuery.Execute(MergeOption.AppendOnly);
                    DeviceGrid.Items.Refresh();
                }
            }
        }

        private void DeviceGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            isEditing = true;
            Console.WriteLine("DeviceGrid_BeginningEdit");
        }

        private void DevicesLstVw_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //Delay the SaveChanges until the CellEdit has ended.
            DeviceGrid.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (!isContextDisposed)
                    {
                        context.SaveChanges();
                        zvsEntityControl.CallonSaveChanges(this, new List<zVirtualScenesCommon.Entity.zvsEntityControl.Tables>() { zvsEntityControl.Tables.device });
                    }
                }
                ), System.Windows.Threading.DispatcherPriority.Background);

            isEditing = false;
        }

        //private void UserControl_Loaded(object sender, RoutedEventArgs e)
        //{
        //    //this.DevicesLstVw.celled
        //    //device.Added += new device.DeviceAddRemoveEventHandler(device_Added);
        //    //device.Removed += new device.DeviceAddRemoveEventHandler(device_Removed);
        //    //device.Changed += new device.DeviceModifiedEventHandler(device_Changed);

        //    //Preload the list
        //    //using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
        //    //{
        //    //    foreach (device d in db.devices.OrderBy(d => d.friendly_name))
        //    //    {
        //    //        AddDevice(d);
        //    //    }
        //    //}
        //}

        //private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        //{
        //    device.Added -= new device.DeviceAddRemoveEventHandler(device_Added);
        //    device.Removed -= new device.DeviceAddRemoveEventHandler(device_Removed);
        //    device.Changed -= new device.DeviceModifiedEventHandler(device_Changed);

        //    deviceDictionary.Clear();
        //}

        //private void device_Changed(int device_id, string PropertyModified)
        //{
        //    if (deviceDictionary.Contains(device_id))
        //    {
        //        device updated_device;
        //        using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
        //        {
        //            //refresh this device
        //            updated_device = db.devices.FirstOrDefault(o => o.id == device_id);
        //        }
        //        if (updated_device != null)
        //        {
        //            //Replace it in the dictionary
        //            deviceDictionary[device_id] = updated_device;

        //            Dispatcher.Invoke(new Action(() =>
        //            {
        //                DevicesLstVw.Items.Refresh();
        //            }));
        //        }
        //    }

        //    if (deviceDictionary.Contains(device_id))
        //    {
        //        DataGridRow row = (DataGridRow)DevicesLstVw.ItemContainerGenerator.ContainerFromItem(deviceDictionary[device_id]);
        //        if (row != null)
        //        {
        //            Dispatcher.Invoke(new Action(() =>
        //            {
        //                //Animate updates
        //                row.Background = new SolidColorBrush(Colors.LightGreen);
        //                ColorAnimation animation = new ColorAnimation();
        //                animation.To = Colors.White;
        //                animation.Duration = new Duration(TimeSpan.FromSeconds(2));
        //                row.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
        //            }));
        //        }
        //    }
        //}

        //private void device_Removed(int device_id)
        //{
        //    Dispatcher.Invoke(new Action(() =>
        //    {
        //        if (deviceDictionary.Contains(device_id))
        //        {
        //            deviceDictionary.Remove(device_id);

        //            Dispatcher.Invoke(new Action(() =>
        //            {
        //                DevicesLstVw.Items.Refresh();
        //            }));
        //        }
        //    }));
        //}

        //private void device_Added(int DeviceID)
        //{
        //    //Add any new devices 
        //    using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
        //    {
        //        device d = db.devices.FirstOrDefault(o => o.id == DeviceID);
        //        if (d != null)
        //            AddDevice(d);
        //    }
        //}

        //private void AddDevice(device d)
        //{
        //    Dispatcher.Invoke(new Action(() =>
        //    {
        //        if (!deviceDictionary.Contains(d.id))
        //        {
        //            deviceDictionary.Add(d.id, d);
        //            DevicesLstVw.Items.Refresh();
        //        }
        //    }));
        //}


        ////User Events
        //private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Delete)
        //    {
        //        if (DevicesLstVw.SelectedItems.Count > 0)
        //        {
        //            device[] SelectedItemsCopy = new device[DevicesLstVw.SelectedItems.Count];
        //            DevicesLstVw.SelectedItems.CopyTo(SelectedItemsCopy, 0);

        //            foreach (device selectedDevice in SelectedItemsCopy)
        //            {
        //                device.RemoveByID(selectedDevice.id);
        //            }



        //            //using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
        //            //{
        //            //    foreach (device selectedDevice in SelectedItemsCopy)
        //            //    {
        //            //        device.RemoveByID(selectedDevice.id, db);
        //            //    }
        //            //}
        //        }
        //    }
        //}

        //private void UserControl_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (DevicesLstVw.SelectedItems.Count > 0)
        //    {
        //        device[] SelectedItemsCopy = new device[DevicesLstVw.SelectedItems.Count];
        //        DevicesLstVw.SelectedItems.CopyTo(SelectedItemsCopy, 0);

        //        ContextMenu menu = new ContextMenu();

        //        MenuItem delete = new MenuItem();
        //        delete.Header = "Remove " + (DevicesLstVw.SelectedItems.Count > 1 ? DevicesLstVw.SelectedItems.Count + " Devices" : "Device");
        //        delete.Click += (s, args) =>
        //        {
        //            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
        //            {
        //                foreach (device selectedDevice in SelectedItemsCopy)
        //                {
        //                    device.RemoveByID(selectedDevice.id, db);
        //                }
        //            }
        //        };

        //        menu.Items.Add(delete);
        //        ContextMenu = menu;
        //    }
        //    else
        //        ContextMenu = null;
        //}


        //Dragging       
        private void DeviceGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (DeviceGrid.SelectedItems.Count == 0)
                    return;

                DataObject dataObject = new DataObject("devices", DeviceGrid.SelectedItems.OfType<device>().ToList());
                DragDrop.DoDragDrop(DeviceGrid, dataObject, DragDropEffects.Move);
            }
        }


    }
}
