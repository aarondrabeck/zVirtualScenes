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

namespace zVirtualScenes_WPF.TriggerControls
{
    /// <summary>
    /// Interaction logic for TriggerGridUC.xaml
    /// </summary>
    public partial class TriggerGridUC : UserControl
    {
        private zvsLocalDBEntities context;

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
            }

            zvsLocalDBEntities.onDeviceValueTriggersChanged += zvsLocalDBEntities_onDeviceValueTriggersChanged;
        }

        void zvsLocalDBEntities_onDeviceValueTriggersChanged(object sender, zvsLocalDBEntities.onEntityChangedEventArgs args)
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
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(TriggerEditorWindow))
                {
                    window.Activate();
                    return;
                }
            }

            device_value_triggers trigger = new device_value_triggers();

            TriggerEditorWindow new_window = new TriggerEditorWindow(trigger);
            new_window.Owner = Application.Current.MainWindow;
            new_window.Show();
        }
    }
}
