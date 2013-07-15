using System;
using System.Collections.Generic;
using System.Data.Entity;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using zvs.Entities;


namespace zvs.WPF.TriggerControls
{
    /// <summary>
    /// Interaction logic for TriggerGridUC.xaml
    /// </summary>
    public partial class TriggerGridUC : UserControl
    {
        private zvsContext context;
        private App app = (App)Application.Current;

        public TriggerGridUC()
        {
            InitializeComponent();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                context = new zvsContext();


                //Load your data here and assign the result to the CollectionViewSource.
                System.Windows.Data.CollectionViewSource myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["device_value_triggersViewSource"];

                myCollectionViewSource.Source = context.DeviceValueTriggers.Local;
                context.DeviceValueTriggers.ToList();
            }
            zvsContext.onDeviceValueTriggersChanged += zvsContext_onDeviceValueTriggersChanged;
        }

        ~TriggerGridUC()
        {
            //Cannot write to log here, it has been disposed. 
            Debug.WriteLine("TriggerGridUC Deconstructed");
        }

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
        }

        private void UserControl_Unloaded_1(object sender, RoutedEventArgs e)
        {
            Window parent = Window.GetWindow(this);
            //Check if the parent window is closing  or if this is just being removed from the visual tree temporarily
            if (parent == null || !parent.IsActive)
            {
                zvsContext.onDeviceValueTriggersChanged -= zvsContext_onDeviceValueTriggersChanged;
            }
        }

        private void zvsContext_onDeviceValueTriggersChanged(object sender, zvsContext.onEntityChangedEventArgs args)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (context != null)
                {
                    if (args.ChangeType == EntityState.Added)
                    {
                        //Gets new devices
                        context.DeviceValueTriggers.ToList();
                    }
                    else
                    {
                        //Reloads context from DB when modifications happen
                        foreach (var ent in context.ChangeTracker.Entries<DeviceValueTrigger>())
                            ent.Reload();
                    }
                }
            }));
        }

        private async void TriggerGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                //have to add , UpdateSourceTrigger=PropertyChanged to have the data updated intime for this event
                var result = await context.TrySaveChangesAsync();
                if (result.HasError)
                    ((App)App.Current).zvsCore.log.Error(result.Message);
            }
        }

        private void SettingBtn_Click_1(object sender, RoutedEventArgs e)
        {
            Object obj = ((FrameworkElement)sender).DataContext;
            if (obj is DeviceValueTrigger)
            {
                var trigger = (DeviceValueTrigger)obj;
                if (trigger != null)
                {
                    TriggerEditorWindow new_window = new TriggerEditorWindow(trigger, context);
                    new_window.Owner = app.zvsWindow;
                    new_window.Title = string.Format("Edit Trigger '{0}', ", trigger.Name);
                    new_window.Show();
                    new_window.Closing += async (s, a) =>
                    {
                        if (!new_window.Canceled)
                        {
                            var result = await context.TrySaveChangesAsync();
                            if (result.HasError)
                                ((App)App.Current).zvsCore.log.Error(result.Message);
                        }
                    };
                }
            }
        }

        private async void Grid_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                DeviceValueTrigger trigger = (DeviceValueTrigger)TriggerGrid.SelectedItem;
                if (trigger != null)
                {
                    if (MessageBox.Show(string.Format("Are you sure you want to delete the '{0}' trigger?", trigger.Name), "Are you sure?",
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        context.DeviceValueTriggers.Local.Remove(trigger);

                        var result = await context.TrySaveChangesAsync();
                        if (result.HasError)
                            ((App)App.Current).zvsCore.log.Error(result.Message);
                    }
                }

                e.Handled = true;
            }
        }

        private void AddTriggerBtn_Click(object sender, RoutedEventArgs e)
        {
            DeviceValueTrigger trigger = new DeviceValueTrigger();
            trigger.Name = "New Trigger";
            TriggerEditorWindow new_window = new TriggerEditorWindow(trigger, context);
            new_window.Owner = app.zvsWindow;
            new_window.Title = "Add Trigger";
            new_window.Show();
            new_window.Closing += async (s, a) =>
            {
                if (!new_window.Canceled)
                {
                    context.DeviceValueTriggers.Add(trigger);

                    var result = await context.TrySaveChangesAsync();
                    if (result.HasError)
                        ((App)App.Current).zvsCore.log.Error(result.Message);
                }
            };
        }

    }
}
