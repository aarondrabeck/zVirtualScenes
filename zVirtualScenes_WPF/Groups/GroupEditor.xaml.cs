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
using System.Windows.Shapes;
using zVirtualScenesCommon.Entity;
using zVirtualScenes_WPF.DeviceControls;

namespace zVirtualScenes_WPF.Groups
{
    /// <summary>
    /// Interaction logic for GroupEditor.xaml
    /// </summary>
    public partial class GroupEditor : Window
    {
        public GroupEditor()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadComboBox();


        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void LoadComboBox()
        {
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                GroupCmbBx.ItemsSource = db.groups.ToList();
            }

            if (GroupCmbBx.Items.Count > 0)
                GroupCmbBx.SelectedIndex = 0;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            GroupNameEditor nameWindow = new GroupNameEditor(null);
            nameWindow.Owner = this;

            if (nameWindow.ShowDialog() ?? false)
            {
                group new_g = group.Creategroup(0, nameWindow.GroupName);

                using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                {
                    db.groups.AddObject(new_g);
                    db.SaveChanges();
                    zvsEntityControl.CallonSaveChanges(this, new List<zVirtualScenesCommon.Entity.zvsEntityControl.Tables>() { zvsEntityControl.Tables.group });
                }
                LoadComboBox();
                GroupCmbBx.SelectedItem = GroupCmbBx.Items.OfType<group>().FirstOrDefault(o => o.name == new_g.name);
            }

        }

        private void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            group g = (group)GroupCmbBx.SelectedItem;
            if (g != null)
            {
                if (
                    MessageBox.Show("Are you sure you want to delete the '" + g.name + "' group?",
                                    "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    group.RemoveGroup(g);
                    LoadComboBox();
                }
            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            group g = (group)GroupCmbBx.SelectedItem;
            if (g != null)
            {
                GroupNameEditor nameWindow = new GroupNameEditor(g.name);
                nameWindow.Owner = this;

                if (nameWindow.ShowDialog() ?? false)
                {
                    group.RenameGroup(g, nameWindow.GroupName);
                    LoadComboBox();
                    GroupCmbBx.SelectedItem = GroupCmbBx.Items.OfType<group>().FirstOrDefault(o => o.name == g.name);
                }
            }
        }

        private void GroupCmbBx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadDevices();
        }

        private void LoadDevices()
        {
            groupsDevicesLstVw.Items.Clear();
            //DeviceShortDragSourceListUC.ClearList();

            group g = (group)GroupCmbBx.SelectedItem;
            if (g != null)
            {
                using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                {
                    foreach (device d in db.group_devices.Where(dg => dg.group_id == g.id).Select(d => d.device).OrderBy(d=>d.friendly_name).ToList())
                    {
                       // DeviceListItemShort item = new DeviceListItemShort();
                      //  item.Update(d);
                      //  groupsDevicesLstVw.Items.Add(item);
                    }

                    if (groupsDevicesLstVw.Items.Count > 0)
                        groupsDevicesLstVw.SelectedIndex = 0;

                    var device_query = from d in db.devices
                                       where !d.group_devices.Any(gd => gd.group_id == g.id)
                                       select d;

                    foreach (device d in device_query.OrderBy(d => d.friendly_name).ToList())
                    {
                       // DeviceShortDragSourceListUC.AddDevice(d);
                    }
                }
            }
        }

        private void groupsDevicesLstVw_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                {
                    group selected_group = db.groups.FirstOrDefault(g => g.id == ((group)GroupCmbBx.SelectedItem).id);
                    if (selected_group != null)
                    {
                       //// foreach (DeviceListItemShort item in groupsDevicesLstVw.SelectedItems)
                       // {
                       //     group_devices device = selected_group.group_devices.FirstOrDefault(gd => gd.device_id == item.device.id);

                       //     if (device != null)
                       //     {
                       //         db.group_devices.DeleteObject(device);
                       //         db.SaveChanges();

                       //         zvsEntityControl.CallDeviceModified(item.device.id, "group");
                       //     }
                       // }
                    }
                }
                LoadDevices();
            }
        }

        private void groupsDevicesLstVw_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetData("devices") != null && e.Data.GetData("devices").GetType() == typeof(List<device>))
            {
                e.Effects = DragDropEffects.Move;
            }
            e.Effects = DragDropEffects.None;
        }

        private void groupsDevicesLstVw_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData("devices") != null && e.Data.GetData("devices").GetType() == typeof(List<device>))
            {
                List<device> devices = (List<device>)e.Data.GetData("devices");

                group selected_group = (group)GroupCmbBx.SelectedItem;
                if (selected_group != null)
                {
                    foreach (device device in devices)
                    {
                        using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                        {
                            if (!db.group_devices.Any(o => o.device_id == device.id && o.group_id == selected_group.id))
                            {
                                db.group_devices.AddObject(new group_devices { device_id = device.id, group_id = selected_group.id });
                                db.SaveChanges();
                                
                            }
                            else
                            {
                                MessageBox.Show(string.Format("{0} is already a member of the '{1}' group.", device.friendly_name, selected_group.name),
                                                "Already a member",
                                                MessageBoxButton.OK,
                                                MessageBoxImage.Error);
                            }
                        }
                        zvsEntityControl.CallonSaveChanges(null, new List<zVirtualScenesCommon.Entity.zvsEntityControl.Tables>() { zvsEntityControl.Tables.group_device });
                    }
                    LoadDevices();

                    //Select all the items
                    groupsDevicesLstVw.SelectedItems.Clear();
                    //foreach (DeviceListItemShort item in groupsDevicesLstVw.Items)
                    //{
                    //    if (devices.Any(o => o.id == item.device.id))
                    //        groupsDevicesLstVw.SelectedItems.Add(item);
                    //}
                    groupsDevicesLstVw.Focus();
                }

                e.Effects = DragDropEffects.Move;


            }
            e.Handled = true;
        }
    }
}
