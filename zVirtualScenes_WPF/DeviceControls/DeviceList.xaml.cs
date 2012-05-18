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
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Media3D;

namespace zVirtualScenes_WPF.DeviceControls
{
    /// <summary>
    /// interaction logic for DeviceList.xaml
    /// </summary>
    public partial class DeviceList : UserControl
    {
        private zvsEntities2 context = zvsEntityControl.SharedContext;

        private bool _AdvancedDisplay = true;
        public bool AdvancedDisplay
        {
            get
            {
                return _AdvancedDisplay;
            }
            set 
            {
                _AdvancedDisplay = value;
                if (value == false)
                {
                    Date.Visibility = System.Windows.Visibility.Collapsed;
                    LevelTxt.Visibility = System.Windows.Visibility.Collapsed;
                    LevelBar.Visibility = System.Windows.Visibility.Collapsed;
                    devicetype.Visibility = System.Windows.Visibility.Collapsed;
                    group2.Visibility = System.Windows.Visibility.Collapsed;
                    group.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    Date.Visibility = System.Windows.Visibility.Visible;
                    LevelTxt.Visibility = System.Windows.Visibility.Visible;
                    LevelBar.Visibility = System.Windows.Visibility.Visible;
                    devicetype.Visibility = System.Windows.Visibility.Visible;
                    group2.Visibility = System.Windows.Visibility.Visible;
                    group.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }

        public DeviceList()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Do not load your data at design time.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                //Load your data here and assign the result to the CollectionViewSource.
                System.Windows.Data.CollectionViewSource myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["devicesViewSource"];
                myCollectionViewSource.Source = GetDeviceQuery().Execute(MergeOption.AppendOnly);
            }
        }

        private System.Data.Objects.ObjectQuery<device> GetDeviceQuery()
        {
            var deviceQuery = context.devices;
            deviceQuery.Include("group_devices");
            deviceQuery.Include("group_devices/group");
            return deviceQuery;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }

        ////User Events
        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (_AdvancedDisplay)
            {
                if (e.Key == Key.Delete)
                {
                    if (
                        MessageBox.Show("Are you sure you want to delete the selected devices?",
                                        "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        DeleteSelectedItems();
                    }
                    e.Handled = true;
                }
            }
        }

        private void UserControl_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_AdvancedDisplay)
            {
                //Context Menus
                if (DeviceGrid.SelectedItems.Count > 0)
                {
                    device[] SelectedItemsCopy = new device[DeviceGrid.SelectedItems.Count];
                    DeviceGrid.SelectedItems.CopyTo(SelectedItemsCopy, 0);

                    ContextMenu menu = new ContextMenu();

                    MenuItem delete = new MenuItem();
                    delete.Header = "Remove " + (DeviceGrid.SelectedItems.Count > 1 ? DeviceGrid.SelectedItems.Count + " Devices" : "Device");
                    delete.Click += (s, args) =>
                    {
                        if (
                       MessageBox.Show("Are you sure you want to delete the selected devices?",
                                       "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            lock (context)
                            {
                                foreach (device selectedDevice in SelectedItemsCopy)
                                {
                                    device d = context.devices.FirstOrDefault(o => o.id == selectedDevice.id);
                                    if (d != null)
                                        context.devices.DeleteObject(d);
                                }

                                context.SaveChanges();
                            }
                        }
                    };

                    menu.Items.Add(delete);
                    ContextMenu = menu;
                }
                else
                    ContextMenu = null;
            }
        }

        private void DeleteSelectedItems()
        {
            if (DeviceGrid.SelectedItems.Count > 0)
            {
                device[] SelectedItemsCopy = new device[DeviceGrid.SelectedItems.Count];
                DeviceGrid.SelectedItems.CopyTo(SelectedItemsCopy, 0);

                lock (context)
                {
                    foreach (device selectedDevice in SelectedItemsCopy)
                    {
                        device d = context.devices.FirstOrDefault(o => o.id == selectedDevice.id);
                        if (d != null)
                            context.devices.DeleteObject(d);
                    }
                    context.SaveChanges();
                }

                //zvsEntityControl.CallonSaveChanges(null,
                //      new List<zVirtualScenesCommon.Entity.zvsEntityControl.onSaveChangesEventArgs.Tables>() 
                //       { 
                //           zVirtualScenesCommon.Entity.zvsEntityControl.onSaveChangesEventArgs.Tables.device 
                //       },
                //      zvsEntityControl.onSaveChangesEventArgs.ChangeType.AddRemove);
            }
        }
    }
}
