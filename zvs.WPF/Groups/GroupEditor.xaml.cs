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
using zvs.WPF.DeviceControls;
using System.Data.Objects;
using System.ComponentModel;

using System.Diagnostics;
using zvs.Entities;

namespace zvs.WPF.Groups
{
    /// <summary>
    /// Interaction logic for GroupEditor.xaml
    /// </summary>
    public partial class GroupEditor : Window
    {
        private zvsContext context;

        public GroupEditor()
        {
            InitializeComponent();
        }

        ~GroupEditor()
        {
            //Cannot write to log here, it has been disposed. 
            Debug.WriteLine("GroupEditor Deconstructed.");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            context = new zvsContext();
            zvsContext.onDevicesChanged += zvsContext_onDevicesChanged;

            // Do not load your data at design time.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                System.Windows.Data.CollectionViewSource groupsViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("groupsViewSource")));
                context.Groups.ToList();
                context.Devices.ToList();
                groupsViewSource.Source = context.Groups.Local;
            }

            DeviceLst.MinimalistDisplay = false;

            EvaluateRemoveBtnUsability();
            EvaluateAddEditBtnsUsability();
            EvaluategroupsDevicesLstVwEnable();
        }

        private void GroupEditor_Closed_1(object sender, EventArgs e)
        {
            zvsContext.onDevicesChanged -= zvsContext_onDevicesChanged;
            context.Dispose();
        }

        void zvsContext_onDevicesChanged(object sender, zvsContext.onEntityChangedEventArgs args)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (context != null)
                {
                    if (args.ChangeType != System.Data.EntityState.Added)
                    {
                        //Reloads context from DB when modifications happen
                        foreach (var ent in context.ChangeTracker.Entries<Device>())
                            ent.Reload();
                    }
                }
            }));
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
                Group new_g = new Group()
                {
                    Name = nameWindow.GroupName
                };

                context.Groups.Local.Add(new_g);
                context.SaveChanges();

                GroupCmbBx.SelectedItem = GroupCmbBx.Items.OfType<Group>().FirstOrDefault(o => o.Name == new_g.Name);

            }
        }

        private void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            Group g = (Group)GroupCmbBx.SelectedItem;
            if (g != null)
            {
                if (
                    MessageBox.Show("Are you sure you want to delete the '" + g.Name + "' group?",
                                    "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {

                    context.Groups.Local.Remove(g);
                    context.SaveChanges();
                }
            }

        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            Group g = (Group)GroupCmbBx.SelectedItem;
            if (g != null)
            {
                GroupNameEditor nameWindow = new GroupNameEditor(g.Name);
                nameWindow.Owner = this;

                if (nameWindow.ShowDialog() ?? false)
                {
                    g.Name = nameWindow.GroupName;
                    context.SaveChanges();
                }
            }
        }

        private void groupsDevicesLstVw_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetData("deviceList") != null && e.Data.GetData("deviceList").GetType() == typeof(List<Device>))
            {
                e.Effects = DragDropEffects.Link;
            }
            e.Effects = DragDropEffects.None;
        }

        private void groupsDevicesLstVw_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData("deviceList") != null && e.Data.GetData("deviceList").GetType() == typeof(List<Device>))
            {
                List<Device> devices = (List<Device>)e.Data.GetData("deviceList");

                Group selected_group = (Group)GroupCmbBx.SelectedItem;
                if (selected_group != null)
                {
                    groupsDevicesLstVw.SelectedItems.Clear();

                    foreach (Device device in devices)
                    {
                        Device d2 = context.Devices.FirstOrDefault(o => o.DeviceId == device.DeviceId); ;
                        if (d2 != null)
                        {
                            //If not already in the group...
                            if (!selected_group.Devices.Contains(d2))
                            {
                                selected_group.Devices.Add(d2);
                                groupsDevicesLstVw.SelectedItems.Add(d2);
                                DeviceLst.DeviceGrid.SelectedItems.Clear();
                            }
                            else
                            {
                                MessageBox.Show(string.Format("{0} is already a member of the '{1}' group.", device.Name, selected_group.Name),
                                                "Already a member",
                                                MessageBoxButton.OK,
                                                MessageBoxImage.Error);
                            }
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
            Group selected_group = (Group)GroupCmbBx.SelectedItem;
            if (selected_group != null && groupsDevicesLstVw.SelectedItems.Count > 0)
            {
                if (MessageBox.Show(string.Format("Are you sure you want to remove the {0} selected devices from this group?", groupsDevicesLstVw.SelectedItems.Count),
                                         "Remove Devices?",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Error) == MessageBoxResult.Yes)
                {
                    Device[] devicesToRemove = new Device[groupsDevicesLstVw.SelectedItems.Count];
                    groupsDevicesLstVw.SelectedItems.CopyTo(devicesToRemove, 0);

                    foreach (Device gd in devicesToRemove)
                        selected_group.Devices.Remove(gd);

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
