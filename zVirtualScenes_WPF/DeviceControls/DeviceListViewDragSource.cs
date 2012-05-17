using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Collections.Specialized;
using zVirtualScenesCommon.Entity;
using System.Windows.Input;

namespace zVirtualScenes_WPF.DeviceControls
{
    public class DeviceListViewDragSource : ListView
    {
        /// <summary>
        /// Dictionary to do quick add removes to ListView
        /// </summary>
        private HybridDictionary DevicesListViewItems = new HybridDictionary();

        public DeviceListViewDragSource()
        {
            Loaded += new RoutedEventHandler(DeviceListViewDragSource_Loaded);
            Unloaded += new RoutedEventHandler(DeviceListViewDragSource_Unloaded);
            MouseMove += new MouseEventHandler(DeviceListViewDragSource_MouseMove);
            MouseLeftButtonDown += new MouseButtonEventHandler(DeviceListViewDragSource_MouseLeftButtonDown);
        }

        void DeviceListViewDragSource_Unloaded(object sender, RoutedEventArgs e)
        {
            device.Added -= new device.DeviceAddRemoveEventHandler(device_DeviceAdded);
            device.Removed -= new device.DeviceAddRemoveEventHandler(device_DeviceRemoved);

            DevicesListViewItems.Clear();
            this.Items.Clear();
        }

        void DeviceListViewDragSource_Loaded(object sender, RoutedEventArgs e)
        {
            device.Added += new device.DeviceAddRemoveEventHandler(device_DeviceAdded);
            device.Removed += new device.DeviceAddRemoveEventHandler(device_DeviceRemoved);              
        }
               
        private void device_DeviceRemoved(int DeviceID)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (DevicesListViewItems.Contains(DeviceID))
                {
                    this.Items.Remove(DevicesListViewItems[DeviceID]);
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

        public void AddDevice(device d)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (!DevicesListViewItems.Contains(d.id))
                {
                    DeviceListItemShort deviceControl = new DeviceListItemShort();
                    deviceControl.Update(d);

                    DragableListViewItem item = new DragableListViewItem();
                    item.Content = deviceControl;

                    DevicesListViewItems.Add(d.id, item);
                    this.Items.Add(item);
                }
            }));
        }
        public void ClearList()
        {
            this.Items.Clear();
            DevicesListViewItems.Clear();
        }

        Point start;
        void DeviceListViewDragSource_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.start = e.GetPosition(null);
        }

        void DeviceListViewDragSource_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (this.SelectedItems.Count == 0)
                    return;

                List<device> selectedDevices = new List<device>();
                foreach (DragableListViewItem item in this.SelectedItems)
                {
                    selectedDevices.Add(((DeviceListItemShort)item.Content).device);
                }
                                
                DataObject dataObject = new DataObject("devices", selectedDevices);
                DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Move);
            } 
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DragableListViewItem();
        }

        protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            base.OnGiveFeedback(e);
            // These Effects values are set in the drop target's
            // DragOver event handler.
            if (e.Effects.HasFlag(DragDropEffects.Move))
            {
                Mouse.SetCursor(Cursors.Cross);
            }
            else
            {
                Mouse.SetCursor(Cursors.No);
            }
            e.Handled = true;
        }
    }
}
