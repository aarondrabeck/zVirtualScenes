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
using zVirtualScenesModel;

namespace zVirtualScenes_WPF.DeviceControls
{
    /// <summary>
    /// Interaction logic for DeviceCommands.xaml
    /// </summary>
    public partial class DeviceCommands : UserControl
    {
        private BitmapImage icon = new BitmapImage(new Uri("pack://application:,,,/zVirtualScenes_WPF;component/Images/send_signal.png"));
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
                #region Device Commands
                foreach (device_commands d_cmd in d.device_commands.OrderByDescending(c => c.sort_order))
                {
                    device_commands device_command = d_cmd;
                    switch ((Data_Types)d_cmd.arg_data_type)
                    {
                        case Data_Types.NONE:
                            {
                                ButtonControl bc = new ButtonControl(d_cmd.friendly_name, d_cmd.description, () =>
                                {
                                    device_command_que cmd = new device_command_que()
                                    {
                                        device_id = d.id,
                                        device_command_id = device_command.id,
                                        arg = ""
                                    };

                                    device_command_que.Run(cmd, context);
                                }, icon);
                                DeviceCommandsStkPnl.Children.Add(bc);
                                break;
                            }
                        case Data_Types.BOOL:
                            {
                                //get the current value from the value table list
                                bool DefaultValue = false;
                                device_values dv = d.device_values.FirstOrDefault(v => v.value_id == d_cmd.custom_data2);
                                if (dv != null)
                                {
                                    bool.TryParse(dv.value2, out DefaultValue);
                                }

                                CheckboxControl control = new CheckboxControl(d_cmd.friendly_name, d_cmd.description, DefaultValue, (isChecked) =>
                                {
                                    device_command_que cmd = new device_command_que()
                                    {
                                        device_id = d.id,
                                        device_command_id = device_command.id,
                                        arg = isChecked.ToString()
                                    };

                                    device_command_que.Run(cmd, context);
                                }, icon);
                                DeviceCommandsStkPnl.Children.Add(control);

                                break;
                            }
                        case Data_Types.DECIMAL:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                device_values dv = d.device_values.FirstOrDefault(v => v.value_id == d_cmd.custom_data2);
                                if (dv != null)
                                {
                                    DefaultValue = dv.value2;
                                }

                                NumericControl control = new NumericControl(d_cmd.friendly_name,
                                    d_cmd.description,
                                    DefaultValue,
                                    NumericControl.NumberType.Decimal,
                                    (value) =>
                                    {
                                        device_command_que cmd = new device_command_que()
                                        {
                                            device_id = d.id,
                                            device_command_id = device_command.id,
                                            arg = value
                                        };

                                        device_command_que.Run(cmd, context);
                                    },
                                    icon);
                                DeviceCommandsStkPnl.Children.Add(control);

                                break;
                            }
                        case Data_Types.INTEGER:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                device_values dv = d.device_values.FirstOrDefault(v => v.value_id == d_cmd.custom_data2);
                                if (dv != null)
                                {
                                    DefaultValue = dv.value2;
                                }

                                NumericControl control = new NumericControl(d_cmd.friendly_name,
                                    d_cmd.description,
                                    DefaultValue,
                                    NumericControl.NumberType.Integer,
                                    (value) =>
                                    {
                                        device_command_que cmd = new device_command_que()
                                        {
                                            device_id = d.id,
                                            device_command_id = device_command.id,
                                            arg = value
                                        };

                                        device_command_que.Run(cmd, context);
                                    },
                                    icon);
                                DeviceCommandsStkPnl.Children.Add(control);

                                break;
                            }
                        case Data_Types.BYTE:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                device_values dv = d.device_values.FirstOrDefault(v => v.value_id == d_cmd.custom_data2);
                                if (dv != null)
                                {
                                    DefaultValue = dv.value2;
                                }

                                NumericControl control = new NumericControl(d_cmd.friendly_name,
                                    d_cmd.description,
                                    DefaultValue,
                                    NumericControl.NumberType.Byte,
                                    (value) =>
                                    {
                                        device_command_que cmd = new device_command_que()
                                        {
                                            device_id = d.id,
                                            device_command_id = device_command.id,
                                            arg = value
                                        };

                                        device_command_que.Run(cmd, context);
                                    },
                                    icon);
                                DeviceCommandsStkPnl.Children.Add(control);

                                break;
                            }
                        case Data_Types.SHORT:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                device_values dv = d.device_values.FirstOrDefault(v => v.value_id == d_cmd.custom_data2);
                                if (dv != null)
                                {
                                    DefaultValue = dv.value2;
                                }

                                NumericControl control = new NumericControl(d_cmd.friendly_name,
                                    d_cmd.description,
                                    DefaultValue,
                                    NumericControl.NumberType.Short,
                                    (value) =>
                                    {
                                        device_command_que cmd = new device_command_que()
                                        {
                                            device_id = d.id,
                                            device_command_id = device_command.id,
                                            arg = value
                                        };

                                        device_command_que.Run(cmd, context);
                                    },
                                    icon);
                                DeviceCommandsStkPnl.Children.Add(control);

                                break;
                            }

                        case Data_Types.STRING:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                device_values dv = d.device_values.FirstOrDefault(v => v.value_id == d_cmd.custom_data2);
                                if (dv != null)
                                {
                                    DefaultValue = dv.value2;
                                }

                                StringControl control = new StringControl(d_cmd.friendly_name,
                                    d_cmd.description,
                                    DefaultValue,
                                    (value) =>
                                    {
                                        device_command_que cmd = new device_command_que()
                                        {
                                            device_id = d.id,
                                            device_command_id = device_command.id,
                                            arg = value
                                        };

                                        device_command_que.Run(cmd, context);
                                    },
                                    icon);
                                DeviceCommandsStkPnl.Children.Add(control);

                                break;
                            }
                        case Data_Types.LIST:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                device_values dv = d.device_values.FirstOrDefault(v => v.value_id == d_cmd.custom_data2);
                                if (dv != null)
                                {
                                    DefaultValue = dv.value2;
                                }

                                ComboboxControl control = new ComboboxControl(d_cmd.friendly_name,
                                    d_cmd.description,
                                    d_cmd.device_command_options.Select(o => o.name).ToList(),
                                    DefaultValue,
                                    (value) =>
                                    {
                                        device_command_que cmd = new device_command_que()
                                        {
                                            device_id = d.id,
                                            device_command_id = device_command.id,
                                            arg = value
                                        };

                                        device_command_que.Run(cmd, context);
                                    },
                                    icon);
                                DeviceCommandsStkPnl.Children.Add(control);

                                break;
                            }
                    }
                }
                #endregion

                #region Device Type Commands
                foreach (device_type_commands d_cmd in d.device_types.device_type_commands.OrderByDescending(c => c.sort_order))
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
                                },
                                    icon);
                                TypeCommandsStkPnl.Children.Add(bc);
                                break;
                            }
                        case Data_Types.BOOL:
                            {
                                //get the current value from the value table list
                                bool DefaultValue = false;
                                device_values dv = d.device_values.FirstOrDefault(v => v.value_id == d_cmd.custom_data2);
                                if (dv != null)
                                {
                                    bool.TryParse(dv.value2, out DefaultValue);
                                }

                                CheckboxControl control = new CheckboxControl(d_cmd.friendly_name, d_cmd.description, DefaultValue, (isChecked) =>
                                {
                                    device_type_command_que cmd = new device_type_command_que()
                                    {
                                        device_id = d.id,
                                        device_type_command_id = device_type_command.id,
                                        arg = isChecked.ToString()
                                    };

                                    device_type_command_que.Run(cmd, context);
                                },
                                    icon);
                                TypeCommandsStkPnl.Children.Add(control);

                                break;
                            }
                        case Data_Types.DECIMAL:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                device_values dv = d.device_values.FirstOrDefault(v => v.value_id == d_cmd.custom_data2);
                                if (dv != null)
                                {
                                    DefaultValue = dv.value2;
                                }

                                NumericControl control = new NumericControl(d_cmd.friendly_name,
                                    d_cmd.description,
                                    DefaultValue,
                                    NumericControl.NumberType.Decimal,
                                    (value) =>
                                    {
                                        device_type_command_que cmd = new device_type_command_que()
                                        {
                                            device_id = d.id,
                                            device_type_command_id = device_type_command.id,
                                            arg = value
                                        };

                                        device_type_command_que.Run(cmd, context);
                                    },
                                    icon);
                                TypeCommandsStkPnl.Children.Add(control);

                                break;
                            }
                        case Data_Types.INTEGER:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                device_values dv = d.device_values.FirstOrDefault(v => v.value_id == d_cmd.custom_data2);
                                if (dv != null)
                                {
                                    DefaultValue = dv.value2;
                                }

                                NumericControl control = new NumericControl(d_cmd.friendly_name,
                                    d_cmd.description,
                                    DefaultValue,
                                   NumericControl.NumberType.Integer,
                                    (value) =>
                                    {
                                        device_type_command_que cmd = new device_type_command_que()
                                        {
                                            device_id = d.id,
                                            device_type_command_id = device_type_command.id,
                                            arg = value
                                        };

                                        device_type_command_que.Run(cmd, context);
                                    },
                                    icon);
                                TypeCommandsStkPnl.Children.Add(control);

                                break;
                            }
                        case Data_Types.SHORT:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                device_values dv = d.device_values.FirstOrDefault(v => v.value_id == d_cmd.custom_data2);
                                if (dv != null)
                                {
                                    DefaultValue = dv.value2;
                                }

                                NumericControl control = new NumericControl(d_cmd.friendly_name,
                                    d_cmd.description,
                                    DefaultValue,
                                    NumericControl.NumberType.Short,
                                    (value) =>
                                    {
                                        device_type_command_que cmd = new device_type_command_que()
                                        {
                                            device_id = d.id,
                                            device_type_command_id = device_type_command.id,
                                            arg = value
                                        };

                                        device_type_command_que.Run(cmd, context);
                                    },
                                    icon);
                                TypeCommandsStkPnl.Children.Add(control);

                                break;
                            }
                        case Data_Types.BYTE:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                device_values dv = d.device_values.FirstOrDefault(v => v.value_id == d_cmd.custom_data2);
                                if (dv != null)
                                {
                                    DefaultValue = dv.value2;
                                }

                                NumericControl control = new NumericControl(d_cmd.friendly_name,
                                    d_cmd.description,
                                    DefaultValue,
                                   NumericControl.NumberType.Byte,
                                    (value) =>
                                    {
                                        device_type_command_que cmd = new device_type_command_que()
                                        {
                                            device_id = d.id,
                                            device_type_command_id = device_type_command.id,
                                            arg = value
                                        };

                                        device_type_command_que.Run(cmd, context);
                                    },
                                    icon);
                                TypeCommandsStkPnl.Children.Add(control);

                                break;
                            }
                        case Data_Types.STRING:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                device_values dv = d.device_values.FirstOrDefault(v => v.value_id == d_cmd.custom_data2);
                                if (dv != null)
                                {
                                    DefaultValue = dv.value2;
                                }

                                StringControl control = new StringControl(d_cmd.friendly_name,
                                    d_cmd.description,
                                    DefaultValue,
                                    (value) =>
                                    {
                                        device_type_command_que cmd = new device_type_command_que()
                                        {
                                            device_id = d.id,
                                            device_type_command_id = device_type_command.id,
                                            arg = value
                                        };

                                        device_type_command_que.Run(cmd, context);
                                    },
                                    icon);
                                TypeCommandsStkPnl.Children.Add(control);

                                break;
                            }
                        case Data_Types.LIST:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                device_values dv = d.device_values.FirstOrDefault(v => v.value_id == d_cmd.custom_data2);
                                if (dv != null)
                                {
                                    DefaultValue = dv.value2;
                                }

                                ComboboxControl control = new ComboboxControl(d_cmd.friendly_name,
                                    d_cmd.description,
                                    d_cmd.device_type_command_options.Select(o => o.options).ToList(),
                                    DefaultValue,
                                    (value) =>
                                    {
                                        device_type_command_que cmd = new device_type_command_que()
                                        {
                                            device_id = d.id,
                                            device_type_command_id = device_type_command.id,
                                            arg = value
                                        };

                                        device_type_command_que.Run(cmd, context);
                                    },
                                    icon);
                                TypeCommandsStkPnl.Children.Add(control);

                                break;
                            }
                    }
                }
                #endregion
            }

            if (DeviceCommandsStkPnl.Children.Count == 0)
                DeviceGrpBx.Visibility = System.Windows.Visibility.Collapsed;

            if (TypeCommandsStkPnl.Children.Count == 0)
                TypeGrpBx.Visibility = System.Windows.Visibility.Collapsed;
            
        }


    }
}
