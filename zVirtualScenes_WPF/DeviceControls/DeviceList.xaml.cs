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

            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                foreach (device d in db.devices)
                    AddDevice(d);
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            device.DeviceAdded -= new device.DeviceAddRemoveEventHandler(device_DeviceAdded);
            device.DeviceRemoved -= new device.DeviceAddRemoveEventHandler(device_DeviceRemoved);

            DevicesListViewItems.Clear();
            DevicesLstVw.Items.Clear();
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
                    DeviceListItem deviceControl = new DeviceListItem();
                    deviceControl.Update(d);

                    ListViewItem item = new ListViewItem();
                    item.Content = deviceControl;

                    DevicesListViewItems.Add(d.id, item);
                    DevicesLstVw.Items.Add(item);
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
                delete.Header = "Remove " + (DevicesLstVw.SelectedItems.Count > 1 ? DevicesLstVw.SelectedItems.Count +" Devices" : "Device");
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
    }
}
