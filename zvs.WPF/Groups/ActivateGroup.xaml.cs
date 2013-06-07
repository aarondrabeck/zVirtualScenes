using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using zvs.Entities;
using zvs.Processor;


namespace zvs.WPF.Groups
{
    /// <summary>
    /// Interaction logic for ActivateGroup.xaml
    /// </summary>
    public partial class ActivateGroup : Window
    {
        private App app = (App)Application.Current;
        private zvsContext context;
        private bool isLoaded = false;

        public ActivateGroup()
        {
            InitializeComponent();
        }

        ~ActivateGroup()
        {
            //Cannot write to log here, it has been disposed. 
            Debug.WriteLine("ActivateGroup Deconstructed.");
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            zvsContext.onGroupsChanged += zvsContext_onGroupsChanged;
            context = new zvsContext();
            context.Groups.ToList();

            System.Windows.Data.CollectionViewSource groupViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("groupViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            groupViewSource.Source = context.Groups.Local;

            isLoaded = true;

            EvaluateSlection();
        }

        void zvsContext_onGroupsChanged(object sender, zvsContext.onEntityChangedEventArgs args)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (context != null)
                {
                    if (args.ChangeType == System.Data.EntityState.Added)
                    {
                        //Gets new devices
                        context.Groups.ToList();
                    }
                    else
                    {
                        //Reloads context from DB when modifications happen
                        foreach (var ent in context.ChangeTracker.Entries<Group>())
                            ent.Reload();
                    }
                }
            }));
        }

        private void ActivateGroup_Closed_1(object sender, EventArgs e)
        {
            zvsContext.onGroupsChanged -= zvsContext_onGroupsChanged;
            context.Dispose();
        }

        private void DoneBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void AllOnBtn_Click(object sender, RoutedEventArgs e)
        {
            Group g = (Group)GroupsCmbBx.SelectedItem;
            if (g != null)
            {
                BuiltinCommand group_on_cmd = context.BuiltinCommands.FirstOrDefault(c => c.UniqueIdentifier == "GROUP_ON");
                if (group_on_cmd != null)
                {
                    CommandProcessor cp = new CommandProcessor(app.zvsCore);
                    await cp.RunCommandAsync(this, group_on_cmd.Id, g.Id.ToString());
                }
            }
        }

        private async void AllOffBtn_Click_1(object sender, RoutedEventArgs e)
        {
            Group g = (Group)GroupsCmbBx.SelectedItem;
            if (g != null)
            {
                BuiltinCommand group_off_cmd = context.BuiltinCommands.FirstOrDefault(c => c.UniqueIdentifier == "GROUP_OFF");
                if (group_off_cmd != null)
                {
                    CommandProcessor cp = new CommandProcessor(app.zvsCore);
                    await cp.RunCommandAsync(this, group_off_cmd.Id, g.Id.ToString());
                }
            }
        }

        private void Window_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }

        private void GroupsCmbBx_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            EvaluateSlection();
        }

        private void EvaluateSlection()
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
