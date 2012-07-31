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
using System.Windows.Navigation;
using System.Windows.Shapes;
using zVirtualScenesModel;

namespace zVirtualScenesGUI.TriggerControls
{
    /// <summary>
    /// Interaction logic for TriggerGridUC.xaml
    /// </summary>
    public partial class TriggerGridUC : UserControl
    {
        private zvsLocalDBEntities context;
        private App app = (App)Application.Current;

        public TriggerGridUC()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                context = new zvsLocalDBEntities();

                //Load your data here and assign the result to the CollectionViewSource.
                System.Windows.Data.CollectionViewSource myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["device_value_triggersViewSource"];
                context.device_value_triggers.ToList();
                myCollectionViewSource.Source = context.device_value_triggers.Local;

                //Load your data here and assign the result to the CollectionViewSource.
                System.Windows.Data.CollectionViewSource JSTrigggerViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["JSTriggerViewSource"];
                context.javascript_triggers.ToList();
                JSTrigggerViewSource.Source = context.javascript_triggers.Local;
            }

            zvsLocalDBEntities.onDeviceValueTriggersChanged += zvsLocalDBEntities_onDeviceValueTriggersChanged;
        }

        void zvsLocalDBEntities_onDeviceValueTriggersChanged(object sender, zvsLocalDBEntities.onEntityChangedEventArgs args)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (context != null)
                {
                    if (args.ChangeType == System.Data.EntityState.Added)
                    {
                        //Gets new devices
                        context.device_value_triggers.ToList();
                    }
                    else
                    {
                        //Reloads context from DB when modifcations happen
                        foreach (var ent in context.ChangeTracker.Entries<device_value_triggers>())
                            ent.Reload();
                    }
                }
            }));
        }

        private void UserControl_Unloaded_1(object sender, RoutedEventArgs e)
        {
            zvsLocalDBEntities.onDeviceValueTriggersChanged -= zvsLocalDBEntities_onDeviceValueTriggersChanged;
        }

        private void TriggerGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                //have to add , UpdateSourceTrigger=PropertyChanged to have the data updated intime for this event
                context.SaveChanges();
            }
        }

        private void SettingBtn_Click_1(object sender, RoutedEventArgs e)
        {
            Object obj = ((FrameworkElement)sender).DataContext;
            if (obj is device_value_triggers)
            {
                var trigger = (device_value_triggers)obj;
                if (trigger != null)
                {
                    TriggerEditorWindow new_window = new TriggerEditorWindow(trigger, context);
                    new_window.Owner = app.zvsWindow;
                    new_window.Title = string.Format("Edit Trigger '{0}', ", trigger.Name);
                    new_window.Show();
                    new_window.Closing += (s, a) =>
                    {
                        if (!new_window.Canceled)
                        {
                            context.SaveChanges();
                        }
                    };
                }
            }
        }

        private void Grid_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                device_value_triggers trigger = (device_value_triggers)TriggerGrid.SelectedItem;
                if (trigger != null)
                {
                    if (MessageBox.Show(string.Format("Are you sure you want to delete the '{0}' trigger?", trigger.Name), "Are you sure?",
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        context.device_value_triggers.Local.Remove(trigger);
                        context.SaveChanges();
                    }
                }

                e.Handled = true;
            }
        }

        private void AddTriggerBtn_Click(object sender, RoutedEventArgs e)
        {
            device_value_triggers trigger = new device_value_triggers();
            trigger.Name = "New Trigger";
            TriggerEditorWindow new_window = new TriggerEditorWindow(trigger, context);
            new_window.Owner = app.zvsWindow;
            new_window.Title = "Add Trigger";
            new_window.Show();
            new_window.Closing += (s, a) =>
            {
                if (!new_window.Canceled)
                {
                    context.device_value_triggers.Add(trigger);
                    context.SaveChanges();
                }
            };
        }

        private void AddJSTriggerBtn_Click(object sender, RoutedEventArgs e)
        {
            javascript_triggers trigger = new javascript_triggers();
            trigger.Name = "New JS Trigger";
            JSTriggerEditorWindow new_window = new JSTriggerEditorWindow(trigger, context);
            new_window.Owner = app.zvsWindow;
            new_window.Title = "Add JS Trigger";
            new_window.Show();
            new_window.Closing += (s, a) =>
            {
                if (!new_window.Canceled)
                {
                    context.javascript_triggers.Add(trigger);
                    context.SaveChanges();
                }
            };
        }

        private void JSTriggerGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                //have to add , UpdateSourceTrigger=PropertyChanged to have the data updated intime for this event
                context.SaveChanges();
            }
        }

        private void JSTriggerGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                javascript_triggers trigger = (javascript_triggers)TriggerGrid.SelectedItem;
                if (trigger != null)
                {
                    if (MessageBox.Show(string.Format("Are you sure you want to delete the '{0}' javascript trigger?", trigger.Name), "Are you sure?",
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        context.javascript_triggers.Local.Remove(trigger);
                        context.SaveChanges();
                    }
                }

                e.Handled = true;
            }
        }

        private void JSTrigger_SettingBtn_Click(object sender, RoutedEventArgs e)
        {
            Object obj = ((FrameworkElement)sender).DataContext;
            if (obj is javascript_triggers)
            {
                var trigger = (javascript_triggers)obj;
                if (trigger != null)
                {
                    JSTriggerEditorWindow new_window = new JSTriggerEditorWindow(trigger, context);
                    new_window.Owner = app.zvsWindow;
                    new_window.Title = string.Format("Edit Trigger '{0}', ", trigger.Name);
                    new_window.Show();
                    new_window.Closing += (s, a) =>
                    {
                        if (!new_window.Canceled)
                        {
                            context.SaveChanges();
                        }
                    };
                }
            }
        }
    }
}
