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

namespace zVirtualScenes_WPF.DeviceControls
{
    /// <summary>
    /// interaction logic for DeviceList.xaml
    /// </summary>
    public partial class DeviceList : UserControl
    {
        /// <summary>
        /// Dictionary to do quick add removes to ListView
        /// </summary>
        private HybridDictionary DevicesListViewItems = new HybridDictionary();

        public DeviceList()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            device.DeviceAdded += new device.DeviceAddRemoveEventHandler(device_DeviceAdded);
            device.DeviceRemoved += new device.DeviceAddRemoveEventHandler(device_DeviceRemoved);
            zvsEntityControl.DeviceModified += new zvsEntityControl.DeviceModifiedEventHandler(zvsEntityControl_DeviceModified);

            //Preload the list
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                foreach (device d in db.devices.OrderBy(d => d.friendly_name))
                {
                    AddDevice(d);

                }
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            device.DeviceAdded -= new device.DeviceAddRemoveEventHandler(device_DeviceAdded);
            device.DeviceRemoved -= new device.DeviceAddRemoveEventHandler(device_DeviceRemoved);
            zvsEntityControl.DeviceModified -= new zvsEntityControl.DeviceModifiedEventHandler(zvsEntityControl_DeviceModified);

            DevicesListViewItems.Clear();
            DevicesLstVw.Items.Clear();
        }

        private void zvsEntityControl_DeviceModified(int device_id, string PropertyModified)
        {

            device d = DevicesLstVw.Items.OfType<device>().FirstOrDefault(o => o.id == device_id);
            if (d != null)
            {
                DataGridRow row = (DataGridRow)DevicesLstVw.ItemContainerGenerator.ContainerFromItem(d);
                if (row != null)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        //Animate updates
                        row.Background = new SolidColorBrush(Colors.LightGreen);
                        ColorAnimation animation = new ColorAnimation();
                        animation.To = Colors.White;
                        animation.Duration = new Duration(TimeSpan.FromSeconds(2));
                        row.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);

                        device new_device;
                        using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                        {
                            //refresh this device
                            new_device = db.devices.FirstOrDefault(o => o.id == d.id);
                        }
                        if (d != null)
                        {
                            row.Item = new_device;

                        }
                    }));
                }
            }




        }

        private void device_DeviceRemoved(int DeviceID)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (DevicesListViewItems.Contains(DeviceID))
                {
                    DevicesLstVw.Items.Remove(DevicesListViewItems[DeviceID]);
                    DevicesListViewItems.Remove(DeviceID);
                }
            }));
        }

        private void device_DeviceAdded(int DeviceID)
        {
            //Add any new devices 
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                device d = db.devices.FirstOrDefault(o => o.id == DeviceID);
                if (d != null)
                    AddDevice(d);
            }
        }

        private void AddDevice(device d)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (!DevicesListViewItems.Contains(d.id))
                {
                    //DeviceListItem deviceControl = new DeviceListItem();
                    //deviceControl.Update(d);

                    //ListViewItem item = new ListViewItem();
                    //item.Content = deviceControl;

                    DevicesListViewItems.Add(d.id, d);
                    DevicesLstVw.Items.Add(d);
                }
            }));
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (DevicesLstVw.SelectedItems.Count > 0)
                {
                    ListViewItem[] SelectedItemsCopy = new ListViewItem[DevicesLstVw.SelectedItems.Count];
                    DevicesLstVw.SelectedItems.CopyTo(SelectedItemsCopy, 0);

                    foreach (ListViewItem item in SelectedItemsCopy)
                        ((DeviceListItem)item.Content).Delete();

                }
            }
        }

        private void UserControl_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DevicesLstVw.SelectedItems.Count > 0)
            {
                ListViewItem[] SelectedItemsCopy = new ListViewItem[DevicesLstVw.SelectedItems.Count];
                DevicesLstVw.SelectedItems.CopyTo(SelectedItemsCopy, 0);

                ContextMenu menu = new ContextMenu();

                MenuItem delete = new MenuItem();
                delete.Header = "Remove " + (DevicesLstVw.SelectedItems.Count > 1 ? DevicesLstVw.SelectedItems.Count + " Devices" : "Device");
                delete.Click += (s, args) =>
                {
                    foreach (ListViewItem item in SelectedItemsCopy)
                        ((DeviceListItem)item.Content).Delete();
                };

                menu.Items.Add(delete);

                ContextMenu = menu;
            }
            else
                ContextMenu = null;
        }


        private void DevicesLstVw_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (DevicesLstVw.SelectedItems.Count == 0)
                    return;

                //List<device> selectedDevices = new List<device>();
                // foreach (device d in DevicesLstVw.SelectedItems.OfType<device>().ToList())                
                //     selectedDevices.Add(d);


                DataObject dataObject = new DataObject("devices", DevicesLstVw.SelectedItems.OfType<device>().ToList());
                DragDrop.DoDragDrop(DevicesLstVw, dataObject, DragDropEffects.Move);
            }
        }
    }
}
