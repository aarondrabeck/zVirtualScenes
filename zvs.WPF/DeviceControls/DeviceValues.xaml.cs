using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using zvs.Entities;
using zvs.Processor;


namespace zvs.WPF.DeviceControls
{
    /// <summary>
    /// Interaction logic for DeviceValues.xaml
    /// </summary>
    public partial class DeviceValues : UserControl, IDisposable
    {
        private App app = (App)Application.Current;
        private zvsContext context;
        private int DeviceID = 0;
        private Device device;

        public DeviceValues(int deviceID)
        {
            this.DeviceID = deviceID;
            context = new zvsContext();

            InitializeComponent();

            zvsContext.ChangeNotifications<DeviceValue>.OnEntityAdded += DeviceValues_onEntityAdded;
            zvsContext.ChangeNotifications<DeviceValue>.OnEntityUpdated += DeviceValues_onEntityUpdated;
            zvsContext.ChangeNotifications<DeviceValue>.OnEntityDeleted += DeviceValues_onEntityDeleted;
        }
                
        private async void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {           
            device = await context.Devices
                .Include(o => o.Values)
                .FirstOrDefaultAsync(dv => dv.Id == DeviceID);

            if (device == null)
            {
                MessageBox.Show("Device not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            device.Values.OrderBy(dv => dv.Id);

            // Do not load your data at design time.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                //Load your data here and assign the result to the CollectionViewSource.
                System.Windows.Data.CollectionViewSource myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["DeviceValueViewSource"];
                myCollectionViewSource.Source = device.Values;
            }

        }

        void DeviceValues_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.EntityUpdatedArgs e)
        {
            if (context == null)
                return;

            this.Dispatcher.Invoke(new Action(async () =>
            {
                foreach (var ent in context.ChangeTracker.Entries<DeviceValue>())
                    await ent.ReloadAsync();
            }));
        }

        void DeviceValues_onEntityDeleted(object sender, NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.EntityDeletedArgs e)
        {
            if (context == null)
                return;

            this.Dispatcher.Invoke(new Action(async () =>
            {
                foreach (var ent in context.ChangeTracker.Entries<DeviceValue>())
                    await ent.ReloadAsync();
            }));
        }

        void DeviceValues_onEntityAdded(object sender, NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.EntityAddedArgs e)
        {
            if (context == null || device == null)
                return;

            this.Dispatcher.Invoke(new Action(async () =>
            {
                await context.DeviceValues
                   .Where(o => o.DeviceId == device.Id)
                   .ToListAsync();
            }));
        }

        private void DataGrid_Unloaded_1(object sender, RoutedEventArgs e)
        {
            zvsContext.ChangeNotifications<DeviceValue>.OnEntityAdded -= DeviceValues_onEntityAdded;
            zvsContext.ChangeNotifications<DeviceValue>.OnEntityUpdated -= DeviceValues_onEntityUpdated;
            zvsContext.ChangeNotifications<DeviceValue>.OnEntityDeleted -= DeviceValues_onEntityDeleted;
        }

        private async void RepollLnk_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (device != null)
            {
                BuiltinCommand cmd = await context.BuiltinCommands.FirstOrDefaultAsync(c => c.UniqueIdentifier == "REPOLL_ME");
                if (cmd != null)
                {
                    CommandProcessor cp = new CommandProcessor(app.zvsCore);
                    await cp.RunCommandAsync(this, cmd, device.Id.ToString());
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.context == null)
                {
                    return;
                }

                context.Dispose();
            }
        }
    }
}
