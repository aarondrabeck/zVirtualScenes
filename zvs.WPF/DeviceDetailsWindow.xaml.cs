using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using zvs.WPF.DeviceControls;
using zvs.DataModel;
using System.Data.Entity;

namespace zvs.WPF
{
    /// <summary>
    /// Interaction logic for DeviceDetails.xaml
    /// </summary>
    public partial class DeviceDetailsWindow
    {
        private int DeviceId { get; set; } 
        private App App { get; set; } 

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

                DeviceNameTextBlock.Text = string.Format("{0} {1}", d.Location, d.Name);
                Title = string.Format("{0} {1} Details", d.Location, d.Name);
                DeviceCurrentStatus.Text = d.CurrentLevelText;

                switch (d.Type.UniqueIdentifier)
                {
                    case "THERMOSTAT":
                        {
                            IconImg.Source = new BitmapImage(new Uri("pack://application:,,,/zVirtualScenes;component/Images/thermometer.png"));
                            break;
                        }
                    case "DIMMER":
                        {
                            IconImg.Source = new BitmapImage(new Uri("pack://application:,,,/zVirtualScenes;component/Images/bulb.png"));
                            break;
                        }
                    case "SWITCH":
                        {
                            IconImg.Source = new BitmapImage(new Uri("pack://application:,,,/zVirtualScenes;component/Images/switch.png"));
                            break;
                        }
                    case "CONTROLLER":
                        {
                            DeviceCurrentStatus.Visibility = Visibility.Collapsed;
                            IconImg.Source = new BitmapImage(new Uri("pack://application:,,,/zVirtualScenes;component/Images/controler.png"));
                            break;
                        }
                    case "DOORLOCK":
                        {
                            IconImg.Source = new BitmapImage(new Uri("pack://application:,,,/zVirtualScenes;component/Images/doorlock.png"));
                            break;
                        }
                }

                if (d.Type.UniqueIdentifier.Equals("DIMMER"))
                {
                    var level = d.CurrentLevelInt;

                    if (level >= 0 && level <= 20)
                        level = 21;

                    level = level / 100;

                    var da = new DoubleAnimation
                    {
                        From = IconImg.Opacity,
                        To = level,
                        Duration = new Duration(TimeSpan.FromSeconds(1))
                    };
                    IconImg.BeginAnimation(OpacityProperty, da);
                }

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
