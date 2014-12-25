using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using zvs.DataModel;

namespace zvs.WPF.DeviceControls
{
    /// <summary>
    /// Interaction logic for DeviceDetails.xaml
    /// </summary>
    public partial class DeviceDetailsWindow
    {
        private int DeviceId { get; set; } 
        private App App { get; set; }

        public Device Device
        {
            get { return (Device)GetValue(DeviceProperty); }
            set { SetValue(DeviceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Device.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeviceProperty =
            DependencyProperty.Register("Device", typeof(Device), typeof(DeviceDetailsWindow), new PropertyMetadata(new Device()));


        public DeviceDetailsWindow(int deviceId)
        {
            App = (App)Application.Current;
            DeviceId = deviceId;
            InitializeComponent();

            NotifyEntityChangeContext.ChangeNotifications<Device>.OnEntityAdded += DeviceDetailsWindow_onEntityAdded;
            NotifyEntityChangeContext.ChangeNotifications<Device>.OnEntityDeleted += DeviceDetailsWindow_onEntityDeleted;
            NotifyEntityChangeContext.ChangeNotifications<Device>.OnEntityUpdated += DeviceDetailsWindow_onEntityUpdated;
        }

#if DEBUG
        ~DeviceDetailsWindow()
        {
            //Cannot write to log here, it has been disposed. 
            Debug.WriteLine("DeviceDetailsWindow Deconstructed.");
        }
#endif

        private async void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            await LoadDeviceAsync();
            SelectionList.SelectedIndex = 0;
        }

        void DeviceDetailsWindow_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<Device>.EntityUpdatedArgs e)
        {
            Dispatcher.Invoke(new Action(async () =>
            {
                if (e.NewEntity.Id == DeviceId)
                    await LoadDeviceAsync();
            }));
        }

        void DeviceDetailsWindow_onEntityDeleted(object sender, NotifyEntityChangeContext.ChangeNotifications<Device>.EntityDeletedArgs e)
        {
            Dispatcher.Invoke(new Action(async () =>
            {
                if (e.DeletedEntity.Id == DeviceId)
                    await LoadDeviceAsync();
            }));
        }

        void DeviceDetailsWindow_onEntityAdded(object sender, NotifyEntityChangeContext.ChangeNotifications<Device>.EntityAddedArgs e)
        {
            Dispatcher.Invoke(new Action(async () =>
            {
                if (e.AddedEntity.Id == DeviceId)
                    await LoadDeviceAsync();
            }));
        }

        private void DeviceDetailsWindow_Closed_1(object sender, EventArgs e)
        {
            NotifyEntityChangeContext.ChangeNotifications<Device>.OnEntityAdded -= DeviceDetailsWindow_onEntityAdded;
            NotifyEntityChangeContext.ChangeNotifications<Device>.OnEntityDeleted -= DeviceDetailsWindow_onEntityDeleted;
            NotifyEntityChangeContext.ChangeNotifications<Device>.OnEntityUpdated -= DeviceDetailsWindow_onEntityUpdated;
        }

        private async Task LoadDeviceAsync()
        {
            using (var context = new ZvsContext(App.EntityContextConnection))
            {
                var d = await context.Devices
                    .Include(o => o.Type)
                    .FirstOrDefaultAsync(dv => dv.Id == DeviceId);

                if (d == null)
                {
                    MessageBox.Show("Device not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                    return;
                }

                Device = d;
            }
        }

        private void SelectionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (ListBoxItem)SelectionList.SelectedItem;
            if (item == null) return;
            ContentStackPanel.Children.Clear();

            if (item.Name.Equals("COMMANDS"))
            {
                ContentStackPanel.Children.Add(new DeviceCommands(DeviceId));
            }
            else if (item.Name.Equals("PROPERTIES"))
            {
                ContentStackPanel.Children.Add(new DeviceProperties(DeviceId));
            }
            else if (item.Name.Equals("VALUES"))
            {
                ContentStackPanel.Children.Add(new DeviceValues(DeviceId));
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
