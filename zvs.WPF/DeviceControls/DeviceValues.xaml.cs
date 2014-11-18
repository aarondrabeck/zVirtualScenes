using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using zvs.DataModel;
using zvs.Processor;


namespace zvs.WPF.DeviceControls
{
    /// <summary>
    /// Interaction logic for DeviceValues.xaml
    /// </summary>
    public partial class DeviceValues : IDisposable
    {
        private readonly App _app = (App)Application.Current;
        private IFeedback<LogEntry> Log { get; set; }
        private readonly ZvsContext _context;
        private int DeviceId { get; set; }
        private Device Device { get; set; }

        public DeviceValues(int deviceId)
        {
            DeviceId = deviceId;
            _context = new ZvsContext(_app.EntityContextConnection);
            Log = new DatabaseFeedback(_app.EntityContextConnection) { Source = "Device Values Viewer" };
            InitializeComponent();

            NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.OnEntityAdded += DeviceValues_onEntityAdded;
            NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.OnEntityUpdated += DeviceValues_onEntityUpdated;
            NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.OnEntityDeleted += DeviceValues_onEntityDeleted;
        }

        private async void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            Device = await _context.Devices
                .Include(o => o.Values)
                .FirstOrDefaultAsync(dv => dv.Id == DeviceId);

            if (Device == null)
            {
                MessageBox.Show("Device not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Do not load your data at design time.
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            //Load your data here and assign the result to the CollectionViewSource.
            var myCollectionViewSource = (CollectionViewSource)Resources["DeviceValueViewSource"];
            myCollectionViewSource.Source = Device.Values.OrderBy(dv => dv.Id); ;
        }

        void DeviceValues_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.EntityUpdatedArgs e)
        {
            if (_context == null)
                return;

            Dispatcher.Invoke(new Action(async () =>
            {
                foreach (var ent in _context.ChangeTracker.Entries<DeviceValue>())
                    await ent.ReloadAsync();
            }));
        }

        void DeviceValues_onEntityDeleted(object sender, NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.EntityDeletedArgs e)
        {
            if (_context == null)
                return;

            Dispatcher.Invoke(new Action(async () =>
            {
                foreach (var ent in _context.ChangeTracker.Entries<DeviceValue>())
                    await ent.ReloadAsync();
            }));
        }

        void DeviceValues_onEntityAdded(object sender, NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.EntityAddedArgs e)
        {
            if (_context == null || Device == null)
                return;

            Dispatcher.Invoke(new Action(async () =>
            {
                await _context.DeviceValues
                   .Where(o => o.DeviceId == Device.Id)
                   .ToListAsync();
            }));
        }

        private void DataGrid_Unloaded_1(object sender, RoutedEventArgs e)
        {
            NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.OnEntityAdded -= DeviceValues_onEntityAdded;
            NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.OnEntityUpdated -= DeviceValues_onEntityUpdated;
            NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.OnEntityDeleted -= DeviceValues_onEntityDeleted;
        }

        private async void RepollLnk_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (Device == null) return;
            var cmd = await _context.BuiltinCommands.FirstOrDefaultAsync(c => c.UniqueIdentifier == "REPOLL_ME");
            if (cmd == null) return;

            await Log.ReportResultAsync(await _app.ZvsEngine.RunCommandAsync(cmd.Id, Device.Id.ToString(CultureInfo.InvariantCulture), String.Empty, _app.Cts.Token), _app.Cts.Token);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (_context == null)
            {
                return;
            }
            _context.Dispose();
        }
    }
}
