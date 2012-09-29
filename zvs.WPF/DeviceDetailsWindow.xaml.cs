using System;
using System.Collections.Generic;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using zvs.WPF.DeviceControls;
using zvs.Entities;


namespace zvs.WPF
{
    /// <summary>
    /// Interaction logic for DeviceDetails.xaml
    /// </summary>
    public partial class DeviceDetailsWindow : Window
    {
        private int DeviceID = 0;

        public DeviceDetailsWindow(int DeviceID)
        {
            this.DeviceID = DeviceID;
            InitializeComponent();
        }

        ~DeviceDetailsWindow()
        {
            //Cannot write to log here, it has been disposed. 
            Debug.WriteLine("DeviceDetailsWindow Deconstructed.");
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            zvsContext.onDevicesChanged += zvsContext_onDevicesChanged;
            LoadDevice();
            SelectionList.SelectedIndex = 0;
        }

        private void DeviceDetailsWindow_Closed_1(object sender, EventArgs e)
        {
            zvsContext.onDevicesChanged -= zvsContext_onDevicesChanged;           
        }

        void zvsContext_onDevicesChanged(object sender, zvsContext.onEntityChangedEventArgs args)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                LoadDevice();
            }));
        }

        private void LoadDevice()
        {
            using (zvsContext context = new zvsContext())
            {
                Device d = context.Devices.FirstOrDefault(dv => dv.DeviceId == DeviceID);

                if (d == null)
                    this.Close();
                else
                {
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
                            double level = d.CurrentLevelInt.Value;

                            if (level >= 0 && level <= 20)
                                level = 21;

                            level = level / 100;

                            DoubleAnimation da = new DoubleAnimation();
                            da.From = IconImg.Opacity;
                            da.To = level;
                            da.Duration = new Duration(TimeSpan.FromSeconds(1));
                            IconImg.BeginAnimation(OpacityProperty, da);
                        }
                    }
                }
            }
        }

        private void SelectionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)SelectionList.SelectedItem;
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
