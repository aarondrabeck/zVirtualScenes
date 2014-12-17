using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Animation;
using zvs.DataModel;
using zvs.Processor;

namespace zvs.WPF.TriggerControls
{
    /// <summary>
    /// Interaction logic for TriggerGridUC.xaml
    /// </summary>
    public partial class TriggerGridUc
    {
        private readonly ZvsContext _context;
        private readonly App _app = (App)Application.Current;
        private IFeedback<LogEntry> Log { get; set; }

        public TriggerGridUc()
        {
            _context = new ZvsContext(_app.EntityContextConnection);
            Log = new DatabaseFeedback(_app.EntityContextConnection) { Source = "Trigger Editor" };
            InitializeComponent();

            NotifyEntityChangeContext.ChangeNotifications<DeviceValueTrigger>.OnEntityAdded += TriggerGridUC_onEntityAdded;
            NotifyEntityChangeContext.ChangeNotifications<DeviceValueTrigger>.OnEntityDeleted += TriggerGridUC_onEntityDeleted;
            NotifyEntityChangeContext.ChangeNotifications<DeviceValueTrigger>.OnEntityUpdated += TriggerGridUC_onEntityUpdated;
        }

        void TriggerGridUC_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<DeviceValueTrigger>.EntityUpdatedArgs e)
        {
            if (_context == null)
                return;

            Dispatcher.Invoke(new Action(async () =>
            {
                foreach (var ent in _context.ChangeTracker.Entries<DeviceValueTrigger>())
                    await ent.ReloadAsync();
            }));
        }

        void TriggerGridUC_onEntityDeleted(object sender, NotifyEntityChangeContext.ChangeNotifications<DeviceValueTrigger>.EntityDeletedArgs e)
        {
            if (_context == null)
                return;

            Dispatcher.Invoke(new Action(async () =>
            {
                foreach (var ent in _context.ChangeTracker.Entries<DeviceValueTrigger>())
                    await ent.ReloadAsync();
            }));
        }

        void TriggerGridUC_onEntityAdded(object sender, NotifyEntityChangeContext.ChangeNotifications<DeviceValueTrigger>.EntityAddedArgs e)
        {
            if (_context == null)
                return;

            Dispatcher.Invoke(new Action(async () =>
            {
                await _context.DeviceValueTriggers.ToListAsync();
            }));
        }

#if DEBUG
        ~TriggerGridUc()
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
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                await _context.DeviceValueTriggers
                    .Include(o => o.DeviceValue)
                    .ToListAsync();

                //Load your data here and assign the result to the CollectionViewSource.
                var myCollectionViewSource = (CollectionViewSource)Resources["DeviceValueTriggersViewSource"];

                myCollectionViewSource.Source = _context.DeviceValueTriggers.Local;
            }

#if DEBUG
            sw.Stop();
            Debug.WriteLine("ScheduledTask grid initialized in {0}", sw.Elapsed.ToString() as object);
#endif
        }

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e) { }
        private void UserControl_Unloaded_1(object sender, RoutedEventArgs e)
        {
            var parent = Window.GetWindow(this);
            //Check if the parent window is closing  or if this is just being removed from the visual tree temporarily
            if (parent != null && parent.IsActive) return;

            NotifyEntityChangeContext.ChangeNotifications<DeviceValueTrigger>.OnEntityAdded -= TriggerGridUC_onEntityAdded;
            NotifyEntityChangeContext.ChangeNotifications<DeviceValueTrigger>.OnEntityDeleted -= TriggerGridUC_onEntityDeleted;
            NotifyEntityChangeContext.ChangeNotifications<DeviceValueTrigger>.OnEntityUpdated -= TriggerGridUC_onEntityUpdated;
        }

        private void AddTriggerBtn_Click(object sender, RoutedEventArgs e)
        {
            var newWindow = new TriggerEditorWindow(0, _context) { Owner = _app.ZvsWindow, Title = "Add Trigger" };
            newWindow.Show();
            newWindow.Closing += async (s, a) =>
            {
                if (newWindow.Canceled) return;
                _context.DeviceValueTriggers.Add(newWindow.Trigger);

                await SaveChangesAsync();
            };
        }

        private void SettingBtn_Click_1(object sender, RoutedEventArgs e)
        {
            var trigger = TriggerGrid.SelectedItem as DeviceValueTrigger;
            if (trigger == null) return;
            var newWindow = new TriggerEditorWindow(trigger.Id, _context)
            {
                Owner = _app.ZvsWindow,
                Title = string.Format("Edit Trigger '{0}', ", trigger.Name)
            };
            newWindow.Show();
            newWindow.Closing += async (s, a) =>
            {
                if (newWindow.Canceled) return;
                await SaveChangesAsync();
            };
        }

        private async Task SaveChangesAsync()
        {
            var result = await _context.TrySaveChangesAsync(_app.Cts.Token);
            if (result.HasError)
                await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving trigger. {0}", result.Message);

            SignalImg.Opacity = 1;
            var da = new DoubleAnimation { From = 1, To = 0, Duration = new Duration(TimeSpan.FromSeconds(.8)) };
            SignalImg.BeginAnimation(OpacityProperty, da);
        }

        private async void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
        {
            var trigger = (DeviceValueTrigger)TriggerGrid.SelectedItem;
            if (trigger == null) return;
           
            if (MessageBox.Show(string.Format("Are you sure you want to delete the '{0}' trigger?", trigger.Name),
                "Are you sure?",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;
            _context.DeviceValueTriggers.Local.Remove(trigger);

            await SaveChangesAsync();
        }
    }
}
