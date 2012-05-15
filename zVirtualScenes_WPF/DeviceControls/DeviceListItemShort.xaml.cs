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
    public partial class DeviceListItemShort : UserControl
    {
        public DeviceListItemShort()
        {
            InitializeComponent();
        }       

        public device device;

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

        //This prevent unwanted behavior #1

        protected override void OnMouseEnter(MouseEventArgs e)
        {

            //base.OnMouseEnter(e);

        }



        //This prevent unwanted behavior #2 (part 1 of 2)

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {

            if (!IsSelected

            || Keyboard.IsKeyDown(Key.LeftShift)

            || Keyboard.IsKeyDown(Key.LeftCtrl)

            || Keyboard.IsKeyDown(Key.RightShift)

            || Keyboard.IsKeyDown(Key.RightCtrl))
            {

                base.OnMouseLeftButtonDown(e);

            }

        }

        //This prevent unwanted behavior #2 (part 2 of 2)
        bool IsSelected = true;
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {

            if (!IsSelected

            || Keyboard.IsKeyDown(Key.LeftShift)

            || Keyboard.IsKeyDown(Key.LeftCtrl)

            || Keyboard.IsKeyDown(Key.RightShift)

            || Keyboard.IsKeyDown(Key.RightCtrl))
            {

                base.OnMouseLeftButtonUp(e);

            }

            else
            {

                base.OnMouseLeftButtonDown(e);

            }

        }
    }
}
