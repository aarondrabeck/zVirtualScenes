using System;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using zVirtualScenes_WPF.DynamicActionControls;
using zVirtualScenes_WPF.DynamicSettingsControls;
using zVirtualScenesModel;

namespace zVirtualScenes_WPF.DeviceControls
{
    /// <summary>
    /// Interaction logic for DeviceCommands.xaml
    /// </summary>
    public partial class DeviceCommands : UserControl
    {
        private zvsLocalDBEntities context;
        private int DeviceID = 0;

        public DeviceCommands(int DeviceID)
        {
            this.DeviceID = DeviceID;
            InitializeComponent();
        }

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            context = new zvsLocalDBEntities();
            LoadCommands();
        }

        private void UserControl_Unloaded_1(object sender, RoutedEventArgs e)
        {
            context.Dispose();
        }

        private void LoadCommands()
        {
            DeviceCommandsStkPnl.Children.Clear();
            TypeCommandsStkPnl.Children.Clear();

            device d = context.devices.FirstOrDefault(dv => dv.id == DeviceID);
            if (d != null)
            {
                foreach (device_type_commands d_cmd in d.device_types.device_type_commands)
                {
                    device_type_commands device_type_command = d_cmd;
                    switch ((Data_Types)d_cmd.arg_data_type)
                    {
                        case Data_Types.NONE:
                        {
                            ButtonControl bc = new ButtonControl(d_cmd.friendly_name, d_cmd.description, () =>
                            {
                                device_type_command_que cmd = new device_type_command_que()
                                {
                                    device_id = d.id,
                                    device_type_command_id = device_type_command.id,
                                    arg = ""
                                };

                                device_type_command_que.Run(cmd, context);
                            });
                            TypeCommandsStkPnl.Children.Add(bc);
                            break;
                        }
                    }
                }

            }
        }

        
    }
}
