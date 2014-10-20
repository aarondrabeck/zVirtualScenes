using System;
using System.Data.Entity;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using zvs.DataModel;


namespace zvs.WPF.TriggerControls
{
    /// <summary>
    /// Interaction logic for TriggerGridUC.xaml
    /// </summary>
    public partial class TriggerGridUC : UserControl
    {
        private ZvsContext context;
        private App app = (App)Application.Current;

        public TriggerGridUC()
        {
            context = new ZvsContext();

            InitializeComponent();

            ZvsContext.ChangeNotifications<DeviceValueTrigger>.OnEntityAdded += TriggerGridUC_onEntityAdded;
            ZvsContext.ChangeNotifications<DeviceValueTrigger>.OnEntityDeleted += TriggerGridUC_onEntityDeleted;
            ZvsContext.ChangeNotifications<DeviceValueTrigger>.OnEntityUpdated += TriggerGridUC_onEntityUpdated;
            ZvsContext.ChangeNotifications<StoredCommand>.OnEntityUpdated += ScheduledTaskCreator_onEntityUpdated;
        }

        void ScheduledTaskCreator_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<StoredCommand>.EntityUpdatedArgs e)
        {
            if (context == null)
                return;

            this.Dispatcher.Invoke(new Action(async () =>
            {
                //Reloads context from DB when modifications happen
                foreach (var ent in context.ChangeTracker.Entries<StoredCommand>())
                    await ent.ReloadAsync();
            }));
        }

        void TriggerGridUC_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<DeviceValueTrigger>.EntityUpdatedArgs e)
        {
            if (context == null)
                return;

            this.Dispatcher.Invoke(new Action(async () =>
            {
                foreach (var ent in context.ChangeTracker.Entries<DeviceValueTrigger>())
                    await ent.ReloadAsync();
            }));
        }

        void TriggerGridUC_onEntityDeleted(object sender, NotifyEntityChangeContext.ChangeNotifications<DeviceValueTrigger>.EntityDeletedArgs e)
        {
            if (context == null)
                return;

            this.Dispatcher.Invoke(new Action(async () =>
            {
                foreach (var ent in context.ChangeTracker.Entries<DeviceValueTrigger>())
                    await ent.ReloadAsync();
            }));
        }

        void TriggerGridUC_onEntityAdded(object sender, NotifyEntityChangeContext.ChangeNotifications<DeviceValueTrigger>.EntityAddedArgs e)
        {
            if (context == null)
                return;

            this.Dispatcher.Invoke(new Action(async () =>
            {
                await context.DeviceValueTriggers.ToListAsync();
            }));
        }

#if DEBUG
        ~TriggerGridUC()
        {
            //Cannot write to log here, it has been disposed. 
            Debug.WriteLine("TriggerGridUC Deconstructed");
        }
#endif

        private async void UserControl_Initialized(object sender, EventArgs e)
        {
#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
#endif
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                await context.DeviceValueTriggers
                    .Include(o => o.DeviceValue)
                    .ToListAsync();

                //Load your data here and assign the result to the CollectionViewSource.
                System.Windows.Data.CollectionViewSource myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["device_value_triggersViewSource"];

                myCollectionViewSource.Source = context.DeviceValueTriggers.Local;
            }

#if DEBUG
            sw.Stop();
            Debug.WriteLine("Trigger grid initialized in {0}", sw.Elapsed.ToString() as object);
#endif
        }

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e) { }
        private void UserControl_Unloaded_1(object sender, RoutedEventArgs e)
        {
            Window parent = Window.GetWindow(this);
            //Check if the parent window is closing  or if this is just being removed from the visual tree temporarily
            if (parent == null || !parent.IsActive)
            {
                ZvsContext.ChangeNotifications<DeviceValueTrigger>.OnEntityAdded -= TriggerGridUC_onEntityAdded;
                ZvsContext.ChangeNotifications<DeviceValueTrigger>.OnEntityDeleted -= TriggerGridUC_onEntityDeleted;
                ZvsContext.ChangeNotifications<DeviceValueTrigger>.OnEntityUpdated -= TriggerGridUC_onEntityUpdated;
            }
        }

        private async void TriggerGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                //have to add , UpdateSourceTrigger=PropertyChanged to have the data updated in time for this event
                var result = await context.TrySaveChangesAsync();
                if (result.HasError)
                    ((App)App.Current).ZvsEngine.log.Error(result.Message);
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
                    TriggerEditorWindow new_window = new TriggerEditorWindow(trigger.Id, context);
                    new_window.Owner = app.ZvsWindow;
                    new_window.Title = string.Format("Edit Trigger '{0}', ", trigger.Name);
                    new_window.Show();
                    new_window.Closing += async (s, a) =>
                    {
                        if (!new_window.Canceled)
                        {
                            var result = await context.TrySaveChangesAsync();
                            if (result.HasError)
                                ((App)App.Current).ZvsEngine.log.Error(result.Message);
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
                            ((App)App.Current).ZvsEngine.log.Error(result.Message);
                    }
                }

                e.Handled = true;
            }
        }

        private void AddTriggerBtn_Click(object sender, RoutedEventArgs e)
        {
            TriggerEditorWindow new_window = new TriggerEditorWindow(0, context);
            new_window.Owner = app.ZvsWindow;
            new_window.Title = "Add Trigger";
            new_window.Show();
            new_window.Closing += async (s, a) =>
            {
                if (!new_window.Canceled)
                {
                    context.DeviceValueTriggers.Add(new_window.Trigger);

                    var result = await context.TrySaveChangesAsync();
                    if (result.HasError)
                        ((App)App.Current).ZvsEngine.log.Error(result.Message);
                }
            };
        }
    }
}
