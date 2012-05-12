using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using zVirtualScenesCommon.Entity;
using System.Windows.Media.Animation;

namespace zVirtualScenes_WPF.DeviceControls
{
    /// <summary>
    /// interaction logic for BaseDeviceControl.xaml
    /// </summary>
    public partial class DeviceListItem : UserControl
    {
        private device device;

        public DeviceListItem()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            zvsEntityControl.DeviceModified += new zvsEntityControl.DeviceModifiedEventHandler(zvsEntityControl_DeviceModified);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            zvsEntityControl.DeviceModified -= new zvsEntityControl.DeviceModifiedEventHandler(zvsEntityControl_DeviceModified);
        }

        private void zvsEntityControl_DeviceModified(int device_id, string PropertyModified)
        {
            if (this.device.id == device_id)
            {
                using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                {
                    //refresh this device
                    device d = db.devices.FirstOrDefault(o => o.id == this.device.id);
                    if (d != null)
                    {
                        Update(d);

                        Dispatcher.Invoke(new Action(() =>
                        {
                            //Animate updates
                            this.Background = new SolidColorBrush(Colors.LightGreen);
                            ColorAnimation animation = new ColorAnimation();
                            animation.To = Colors.Transparent;
                            animation.Duration = new Duration(TimeSpan.FromSeconds(2));
                            this.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
                        }));
                    }
                }
            }
        }

        public void Update(device device)
        {
            this.device = device;

            if (device != null)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    NodeIDtxt.Text = device.node_id.ToString();
                    Nametxt.Text = device.friendly_name;
                    Typetxt.Text = device.device_types.friendly_name;
                    Leveltxt.Text = device.GetLevelText();
                    LevelBar.Value = device.GetLevelMeter();
                    Groupstxt.Text = device.GetGroups;

                    if (device.last_heard_from.HasValue)
                        DateUpdatedtxt.Text = device.last_heard_from.Value.ToString();

                    if (device.device_types.name.Equals("THERMOSTAT"))
                    {
                        byte red = (byte)(0 * 2.55);
                        byte bluegreen = (byte)(255 - (0 * 2.55));
                        LevelBar.Foreground = new SolidColorBrush(Color.FromArgb(255, red, bluegreen, bluegreen));
                    }
                    else
                        LevelBar.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 202, 0));

                    //Images
                    if (device.device_types.name.Equals("THERMOSTAT"))
                        IconImg.Source = new BitmapImage(new Uri(@"pack://application:,,,/zVirtualScenes_WPF;component/Images/20thermostat.png"));
                    else if (device.device_types.name.Equals("DIMMER"))
                        IconImg.Source = new BitmapImage(new Uri(@"pack://application:,,,/zVirtualScenes_WPF;component/Images/20bulb.png"));
                    else if (device.device_types.name.Equals("SWITCH"))
                        IconImg.Source = new BitmapImage(new Uri(@"pack://application:,,,/zVirtualScenes_WPF;component/Images/20switch.png"));
                    else if (device.device_types.name.Equals("CONTROLLER"))
                        IconImg.Source = new BitmapImage(new Uri(@"pack://application:,,,/zVirtualScenes_WPF;component/Images/20controller.png"));
                    else if (device.device_types.name.Equals("DOORLOCK"))
                        IconImg.Source = new BitmapImage(new Uri(@"pack://application:,,,/zVirtualScenes_WPF;component/Images/20doorlock.png"));
                    else
                        IconImg.Source = new BitmapImage(new Uri(@"pack://application:,,,/zVirtualScenes_WPF;component/Images/20radio.png"));

                }));
            }

        }

        public void Delete()
        {
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                device d = db.devices.FirstOrDefault(o => o.id == device.id);
                if (d != null)
                {
                    db.devices.DeleteObject(d);
                    db.SaveChanges();

                    device.CallRemoved(device.id);
                }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Delete();
        }

    }
}
