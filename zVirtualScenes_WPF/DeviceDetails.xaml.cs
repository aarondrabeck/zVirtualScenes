﻿using System;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using zVirtualScenes_WPF.DeviceControls;
using zVirtualScenesModel;

namespace zVirtualScenes_WPF
{
    /// <summary>
    /// Interaction logic for DeviceDetails.xaml
    /// </summary>
    public partial class DeviceDetails : Window
    {        
        private int DeviceID = 0;

        public DeviceDetails(int DeviceID)
        {
            this.DeviceID = DeviceID;
            InitializeComponent();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            zvsLocalDBEntities.onDevicesChanged += zvsLocalDBEntities_onDevicesChanged;
            LoadDevice();
            SelectionList.SelectedIndex = 0;
        }
        
        private void Window_Unloaded_1(object sender, RoutedEventArgs e)
        {
            zvsLocalDBEntities.onDevicesChanged -= zvsLocalDBEntities_onDevicesChanged;           
        }

        void zvsLocalDBEntities_onDevicesChanged(object sender, zvsLocalDBEntities.onEntityChangedEventArgs args)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                LoadDevice();
            }));
        }

        private void LoadDevice()
        {
            using (zvsLocalDBEntities context = new zvsLocalDBEntities())
            {
                device d = context.devices.FirstOrDefault(dv => dv.id == DeviceID);

                if (d == null)
                    this.Close();
                else
                {
                    DeviceNameTextBlock.Text = d.friendly_name;
                    DeviceCurrentStatus.Text = d.current_status;

                    switch (d.device_types.name)
                    {
                        case "THERMOSTAT":
                            {
                                IconImg.Source = new BitmapImage(new Uri("pack://application:,,,/zVirtualScenes_WPF;component/Images/thermometer.png"));

                                //TODO: USE GLOBAL SETTING
                                DeviceCurrentStatus.Text += "F";
                                break;
                            }
                        case "DIMMER":
                            {
                                IconImg.Source = new BitmapImage(new Uri("pack://application:,,,/zVirtualScenes_WPF;component/Images/bulb.png"));

                                //TODO: USE GLOBAL SETTING
                                DeviceCurrentStatus.Text += "%";
                                break;
                            }
                        case "SWITCH":
                            {
                                IconImg.Source = new BitmapImage(new Uri("pack://application:,,,/zVirtualScenes_WPF;component/Images/switch.png"));
                                break;
                            }
                        case "CONTROLLER":
                            {
                                IconImg.Source = new BitmapImage(new Uri("pack://application:,,,/zVirtualScenes_WPF;component/Images/controler.png"));
                                break;
                            }
                        case "DOORLOCK":
                            {
                                IconImg.Source = new BitmapImage(new Uri("pack://application:,,,/zVirtualScenes_WPF;component/Images/doorlock.png"));
                                break;
                            }
                    }

                    if (d.device_types.name.Equals("DIMMER"))
                    {
                        double level = 0;
                        if (double.TryParse(d.current_status, out level))
                        {
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
            }
        }
        
    }
}
