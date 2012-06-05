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
using zVirtualScenes_WPF.DeviceControls;
using System.Data.Objects;
using System.ComponentModel;
using zVirtualScenesModel;

namespace zVirtualScenes_WPF.Groups
{
    /// <summary>
    /// Interaction logic for GroupEditor.xaml
    /// </summary>
    public partial class GroupEditor : Window
    {
        private zvsLocalDBEntities context;

        public GroupEditor()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            context = new zvsLocalDBEntities();
            zvsLocalDBEntities.onDevicesChanged += zvsLocalDBEntities_onDevicesChanged;

            // Do not load your data at design time.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                System.Windows.Data.CollectionViewSource groupsViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("groupsViewSource")));
                context.groups.ToList();
                context.devices.ToList();
                groupsViewSource.Source = context.groups.Local;
            }            

            DeviceLst.AdvancedDisplay = false;

            EvaluateRemoveBtnUsability();
            EvaluateAddEditBtnsUsability();
            EvaluategroupsDevicesLstVwEnable();

            
        }

        void zvsLocalDBEntities_onDevicesChanged(object sender, zvsLocalDBEntities.onEntityChangedEventArgs args)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (args.ChangeType != System.Data.EntityState.Added)
                {                   
                    //Reloads context from DB when modifcations happen
                    foreach (var ent in context.ChangeTracker.Entries<device>())
                        ent.Reload();

                    groupsDevicesLstVw.Items.Refresh();
                }
            }));
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            zvsLocalDBEntities.onDevicesChanged -= zvsLocalDBEntities_onDevicesChanged;
            context.Dispose();
        }

        private void EvaluateAddEditBtnsUsability()
        {
            if (GroupCmbBx.Items.Count > 0)
            {
                this.RemoveBtn.IsEnabled = true;
                this.EditBtn.IsEnabled = true;
            }
            else
            {
                this.RemoveBtn.IsEnabled = false;
                this.EditBtn.IsEnabled = false;
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            GroupNameEditor nameWindow = new GroupNameEditor(null);
            nameWindow.Owner = this;

            if (nameWindow.ShowDialog() ?? false)
            {
                group new_g = new group()
                {
                    name = nameWindow.GroupName
                };

                context.groups.Local.Add(new_g);
                context.SaveChanges();

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

                    context.groups.Local.Remove(g);
                    context.SaveChanges();
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
                    g.name = nameWindow.GroupName;
                    context.SaveChanges();
                }
            }
        }

        private void groupsDevicesLstVw_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetData("deviceList") != null && e.Data.GetData("deviceList").GetType() == typeof(List<device>))
            {
                e.Effects = DragDropEffects.Link;
            }
            e.Effects = DragDropEffects.None;
        }

        private void groupsDevicesLstVw_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData("deviceList") != null && e.Data.GetData("deviceList").GetType() == typeof(List<device>))
            {
                List<device> devices = (List<device>)e.Data.GetData("deviceList");

                group selected_group = (group)GroupCmbBx.SelectedItem;
                if (selected_group != null)
                {
                    groupsDevicesLstVw.SelectedItems.Clear();

                    foreach (device device in devices)
                    {
                        //If not already in the group...
                        if (!selected_group.group_devices.Any(o => o.device_id == device.id && o.group_id == selected_group.id))
                        {
                            group_devices gd = new group_devices
                            {
                                device_id = device.id,
                                group_id = selected_group.id
                            };

                            context.group_devices.Local.Add(gd);
                            groupsDevicesLstVw.SelectedItems.Add(gd);
                            DeviceLst.DeviceGrid.SelectedItems.Clear();
                        }
                        else
                        {
                            MessageBox.Show(string.Format("{0} is already a member of the '{1}' group.", device.friendly_name, selected_group.name),
                                            "Already a member",
                                            MessageBoxButton.OK,
                                            MessageBoxImage.Error);
                        }
                    }
                    context.SaveChanges();

                    groupsDevicesLstVw.Focus();
                }
                e.Effects = DragDropEffects.Move;
            }
            e.Handled = true;
        }

        private void groupsDevicesLstVw_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EvaluateRemoveBtnUsability();
        }

        private void EvaluateRemoveBtnUsability()
        {
            if (this.groupsDevicesLstVw.SelectedItems.Count > 0)
                this.RemoveSelected.IsEnabled = true;
            else
                this.RemoveSelected.IsEnabled = false;
        }

        private void RemoveSelectedGroupDevices()
        {
            group selected_group = (group)GroupCmbBx.SelectedItem;
            if (selected_group != null && groupsDevicesLstVw.SelectedItems.Count > 0)
            {
                group_devices[] SelectedItemsCopy = new group_devices[groupsDevicesLstVw.SelectedItems.Count];
                groupsDevicesLstVw.SelectedItems.CopyTo(SelectedItemsCopy, 0);

                if (MessageBox.Show(string.Format("Are you sure you want to remove the {0} selected devices from this group?", groupsDevicesLstVw.SelectedItems.Count),
                                         "Remove Devices?",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Error) == MessageBoxResult.Yes)
                {

                    foreach (group_devices gd in SelectedItemsCopy)
                        context.group_devices.Remove(gd);
                    context.SaveChanges();
                }
            }


        }

        private void groupsDevicesLstVw_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                RemoveSelectedGroupDevices();
            }
        }

        private void RemoveSelected_Click(object sender, RoutedEventArgs e)
        {
            RemoveSelectedGroupDevices();
        }

        private void GroupCmbBx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EvaluateAddEditBtnsUsability();
            EvaluategroupsDevicesLstVwEnable();
        }

        private void EvaluategroupsDevicesLstVwEnable()
        {
            if (GroupCmbBx.SelectedItem == null)
                groupsDevicesLstVw.IsEnabled = false;
            else
                groupsDevicesLstVw.IsEnabled = true;
        }
    }
}
