using System;
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
    public partial class DeviceDetailsWindow : Window
    {
        private int DeviceID = 0;

        public DeviceDetailsWindow(int deviceID)
        {
            this.DeviceID = deviceID;
            InitializeComponent();

            ZvsContext.ChangeNotifications<Device>.OnEntityAdded += DeviceDetailsWindow_onEntityAdded;
            ZvsContext.ChangeNotifications<Device>.OnEntityDeleted += DeviceDetailsWindow_onEntityDeleted;
            ZvsContext.ChangeNotifications<Device>.OnEntityUpdated += DeviceDetailsWindow_onEntityUpdated;
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
            this.Dispatcher.Invoke(new Action(async () =>
            {
                if (e.NewEntity.Id == DeviceID)
                    await LoadDeviceAsync();
            }));
        }

        void DeviceDetailsWindow_onEntityDeleted(object sender, NotifyEntityChangeContext.ChangeNotifications<Device>.EntityDeletedArgs e)
        {
            this.Dispatcher.Invoke(new Action(async () =>
            {
                if (e.DeletedEntity.Id == DeviceID)
                    await LoadDeviceAsync();
            }));
        }

        void DeviceDetailsWindow_onEntityAdded(object sender, NotifyEntityChangeContext.ChangeNotifications<Device>.EntityAddedArgs e)
        {
            this.Dispatcher.Invoke(new Action(async () =>
            {
                if (e.AddedEntity.Id == DeviceID)
                    await LoadDeviceAsync();
            }));
        }

        private void DeviceDetailsWindow_Closed_1(object sender, EventArgs e)
        {
            ZvsContext.ChangeNotifications<Device>.OnEntityAdded -= DeviceDetailsWindow_onEntityAdded;
            ZvsContext.ChangeNotifications<Device>.OnEntityDeleted -= DeviceDetailsWindow_onEntityDeleted;
            ZvsContext.ChangeNotifications<Device>.OnEntityUpdated -= DeviceDetailsWindow_onEntityUpdated;
        }

        private async Task LoadDeviceAsync()
        {
            using (var context = new ZvsContext())
            {
                var d = await context.Devices
                    .Include(o => o.Type)
                    .FirstOrDefaultAsync(dv => dv.Id == DeviceID);

                if (d == null)
                {
                    MessageBox.Show("Device not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                    return;
                }

                DeviceNameTextBlock.Text = d.Name;
                this.Title = string.Format("'{0}' Details", d.Name);
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
                            DeviceCurrentStatus.Visibility = System.Windows.Visibility.Collapsed;
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
                    if (d.CurrentLevelInt.HasValue)
                    {
                        var level = d.CurrentLevelInt.Value;

                        if (level >= 0 && level <= 20)
                            level = 21;

                        level = level / 100;

                        var da = new DoubleAnimation();
                        da.From = IconImg.Opacity;
                        da.To = level;
                        da.Duration = new Duration(TimeSpan.FromSeconds(1));
                        IconImg.BeginAnimation(OpacityProperty, da);
                    }
                }

            }
        }

        private void SelectionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (ListBoxItem)SelectionList.SelectedItem;
            if (item != null)
            {
                ContentStackPanel.Children.Clear();

                if (item.Name.Equals("COMMANDS"))
                {
                    ContentStackPanel.Children.Add(new DeviceCommands(DeviceID));
                }
                else if (item.Name.Equals("PROPERTIES"))
                {
                    ContentStackPanel.Children.Add(new DeviceProperties(DeviceID));
                }
                else if (item.Name.Equals("VALUES"))
                {
                    ContentStackPanel.Children.Add(new DeviceValues(DeviceID));
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
