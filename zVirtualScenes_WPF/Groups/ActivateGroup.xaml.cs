using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using zVirtualScenesModel;

namespace zVirtualScenes_WPF.Groups
{
    /// <summary>
    /// Interaction logic for ActivateGroup.xaml
    /// </summary>
    public partial class ActivateGroup : Window
    {
        private zvsLocalDBEntities context;
        private bool isLoaded = false;

        public ActivateGroup()
        {
            InitializeComponent();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            zvsLocalDBEntities.onGroupsChanged += zvsLocalDBEntities_onGroupsChanged;
            context = new zvsLocalDBEntities();
            context.groups.ToList();

            System.Windows.Data.CollectionViewSource groupViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("groupViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            groupViewSource.Source = context.groups.Local;

            isLoaded = true;
        }

        void zvsLocalDBEntities_onGroupsChanged(object sender, zvsLocalDBEntities.onEntityChangedEventArgs args)
        {
            this.Dispatcher.Invoke(new Action(() =>
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

                    GroupsCmbBx.Items.Refresh();
                }
            }));
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            zvsLocalDBEntities.onGroupsChanged -= zvsLocalDBEntities_onGroupsChanged;
            context.Dispose();
        }

        private void DoneBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AllOnBtn_Click(object sender, RoutedEventArgs e)
        {
            group g = (group)GroupsCmbBx.SelectedItem;
            if (g != null)
            {
                builtin_commands group_on_cmd = context.builtin_commands.FirstOrDefault(c => c.name == "GROUP_ON");
                if (group_on_cmd != null)
                    group_on_cmd.Run(context, g.id.ToString());
            }
        }

        private void AllOffBtn_Click_1(object sender, RoutedEventArgs e)
        {
            group g = (group)GroupsCmbBx.SelectedItem;
            if (g != null)
            {
                builtin_commands group_on_cmd = context.builtin_commands.FirstOrDefault(c => c.name == "GROUP_OFF");
                if (group_on_cmd != null)
                    group_on_cmd.Run(context, g.id.ToString());
            }
        }

        private void Window_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }

        private void GroupsCmbBx_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (isLoaded)
            {
                if (GroupsCmbBx.SelectedItem == null)
                {
                    AllOffBtn.IsEnabled = false;
                    AllOnBtn.IsEnabled = false;
                }
                else
                {
                    AllOffBtn.IsEnabled = true;
                    AllOnBtn.IsEnabled = true;
                }
            }
        }
    }
}
