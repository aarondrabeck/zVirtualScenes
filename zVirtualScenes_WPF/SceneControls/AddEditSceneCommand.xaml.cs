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
using System.Windows.Shapes;
using zVirtualScenes_WPF.DynamicActionControls;
using zVirtualScenesModel;

namespace zVirtualScenes_WPF.SceneControls
{
    /// <summary>
    /// Interaction logic for AddEditSceneCommand.xaml
    /// </summary>
    public partial class AddEditSceneCommand : Window
    {
        private zvsLocalDBEntities context;
        private scene_commands scene_command;
        private device _device;
        private BitmapImage icon = new BitmapImage(new Uri("pack://application:,,,/zVirtualScenes_WPF;component/Images/save_check.png"));
        private string arg = string.Empty;

        public AddEditSceneCommand(zvsLocalDBEntities context, scene_commands scene_command)
        {
            this.context = context;
            this.scene_command = scene_command;
            _device = context.devices.FirstOrDefault(o => o.id == scene_command.device_id);

            if (_device == null)
                this.Close();
            else
                this.Title = string.Format("'{0}' Commands", _device.friendly_name);

            InitializeComponent();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {

            this.Title = string.Format("Scene command for '{0}'", _device.friendly_name);
            if ((scene_commands.command_types)scene_command.command_type_id == scene_commands.command_types.device_type_command)
            {
                DeviceTypeCmdRadioBtn.IsChecked = true;
            }
            else
            {
                DeviceCmdRadioBtn.IsChecked = true;
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CmdsCmboBox.SelectedItem == null)
            {
                MessageBox.Show("You must select a command", "No Command Selected");
                return;
            }

            if (CmdsCmboBox.SelectedItem is device_type_commands)
            {
                device_type_commands d_cmd = (device_type_commands)CmdsCmboBox.SelectedItem;
                scene_command.command_id = d_cmd.id;
                scene_command.command_type_id = (int)scene_commands.command_types.device_type_command;
            }
            else if (CmdsCmboBox.SelectedItem is device_commands)
            {
                device_commands d_cmd = (device_commands)CmdsCmboBox.SelectedItem;
                scene_command.command_id = d_cmd.id;
                scene_command.command_type_id = (int)scene_commands.command_types.device_command;
            }

            scene_command.arg = arg;

            this.DialogResult = true;
            this.Close();
        }

        private void CmdsCmboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ArgSckPnl.Children.Clear();
            if (CmdsCmboBox.SelectedItem != null)
            {
                if (CmdsCmboBox.SelectedItem is device_type_commands)
                {
                    device_type_commands d_cmd = (device_type_commands)CmdsCmboBox.SelectedItem;
                    #region Device Type Commands
                    switch ((Data_Types)d_cmd.arg_data_type)
                    {
                        case Data_Types.NONE:
                            {
                                ArgSckPnl.Children.Add(new TextBlock()
                                {
                                    Text = "None",
                                    Margin = new Thickness(30, 0, 0, 0)
                                });
                                break;
                            }
                        case Data_Types.BOOL:
                            {
                                //get the current value from the value table list
                                bool DefaultValue = false;
                                if (!bool.TryParse(scene_command.arg, out DefaultValue))
                                {
                                    device_values dv = _device.device_values.FirstOrDefault(v => v.value_id == d_cmd.custom_data2);
                                    if (dv != null)
                                    {
                                        bool.TryParse(dv.value2, out DefaultValue);
                                    }
                                }
                                arg = DefaultValue.ToString();

                                CheckboxControl control = new CheckboxControl(d_cmd.friendly_name, d_cmd.description, DefaultValue, (isChecked) =>
                                {
                                    arg = isChecked.ToString();
                                }, icon);
                                ArgSckPnl.Children.Add(control);

                                break;
                            }
                        case Data_Types.DECIMAL:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                if (!string.IsNullOrEmpty(scene_command.arg))
                                {
                                    DefaultValue = scene_command.arg;
                                }
                                else
                                {
                                    device_values dv = _device.device_values.FirstOrDefault(v => v.value_id == d_cmd.custom_data2);
                                    if (dv != null)
                                    {
                                        DefaultValue = dv.value2;
                                    }
                                }
                                arg = DefaultValue;
                                NumericControl control = new NumericControl(d_cmd.friendly_name,
                                    d_cmd.description,
                                    DefaultValue,
                                    NumericControl.NumberType.Decimal,
                                    (value) =>
                                    {
                                        arg = value;
                                    }, icon);
                                ArgSckPnl.Children.Add(control);

                                break;
                            }
                        case Data_Types.INTEGER:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                if (!string.IsNullOrEmpty(scene_command.arg))
                                {
                                    DefaultValue = scene_command.arg;
                                }
                                else
                                {
                                    device_values dv = _device.device_values.FirstOrDefault(v => v.value_id == d_cmd.custom_data2);
                                    if (dv != null)
                                    {
                                        DefaultValue = dv.value2;
                                    }
                                }
                                arg = DefaultValue;
                                NumericControl control = new NumericControl(d_cmd.friendly_name,
                                    d_cmd.description,
                                    DefaultValue,
                                   NumericControl.NumberType.Integer,
                                    (value) =>
                                    {
                                        arg = value;
                                    }, icon);
                                ArgSckPnl.Children.Add(control);

                                break;
                            }
                        case Data_Types.SHORT:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                if (!string.IsNullOrEmpty(scene_command.arg))
                                {
                                    DefaultValue = scene_command.arg;
                                }
                                else
                                {
                                    device_values dv = _device.device_values.FirstOrDefault(v => v.value_id == d_cmd.custom_data2);
                                    if (dv != null)
                                    {
                                        DefaultValue = dv.value2;
                                    }
                                }
                                arg = DefaultValue;
                                NumericControl control = new NumericControl(d_cmd.friendly_name,
                                    d_cmd.description,
                                    DefaultValue,
                                    NumericControl.NumberType.Short,
                                    (value) =>
                                    {
                                        arg = value;
                                    }, icon);
                                ArgSckPnl.Children.Add(control);

                                break;
                            }
                        case Data_Types.BYTE:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                if (!string.IsNullOrEmpty(scene_command.arg))
                                {
                                    DefaultValue = scene_command.arg;
                                }
                                else
                                {
                                    device_values dv = _device.device_values.FirstOrDefault(v => v.value_id == d_cmd.custom_data2);
                                    if (dv != null)
                                    {
                                        DefaultValue = dv.value2;
                                    }
                                }
                                arg = DefaultValue;
                                NumericControl control = new NumericControl(d_cmd.friendly_name,
                                    d_cmd.description,
                                    DefaultValue,
                                   NumericControl.NumberType.Byte,
                                    (value) =>
                                    {
                                        arg = value;
                                    }, icon);
                                ArgSckPnl.Children.Add(control);

                                break;
                            }
                        case Data_Types.STRING:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0"; if (!string.IsNullOrEmpty(scene_command.arg))
                                {
                                    DefaultValue = scene_command.arg;
                                }
                                else
                                {
                                    device_values dv = _device.device_values.FirstOrDefault(v => v.value_id == d_cmd.custom_data2);
                                    if (dv != null)
                                    {
                                        DefaultValue = dv.value2;
                                    }
                                }
                                arg = DefaultValue;
                                StringControl control = new StringControl(d_cmd.friendly_name,
                                    d_cmd.description,
                                    DefaultValue,
                                    (value) =>
                                    {
                                        arg = value;
                                    }, icon);
                                ArgSckPnl.Children.Add(control);

                                break;
                            }
                        case Data_Types.LIST:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                if (!string.IsNullOrEmpty(scene_command.arg))
                                {
                                    DefaultValue = scene_command.arg;
                                }
                                else
                                {
                                    device_values dv = _device.device_values.FirstOrDefault(v => v.value_id == d_cmd.custom_data2);
                                    if (dv != null)
                                    {
                                        DefaultValue = dv.value2;
                                    }
                                }
                                arg = DefaultValue;
                                ComboboxControl control = new ComboboxControl(d_cmd.friendly_name,
                                    d_cmd.description,
                                    d_cmd.device_type_command_options.Select(o => o.options).ToList(),
                                    DefaultValue,
                                    (value) =>
                                    {
                                        arg = value;
                                    }, icon);
                                ArgSckPnl.Children.Add(control);

                                break;
                            }

                    }
                    #endregion
                }

                if (CmdsCmboBox.SelectedItem is device_commands)
                {
                    device_commands d_cmd = (device_commands)CmdsCmboBox.SelectedItem;
                    #region Device Commands

                    switch ((Data_Types)d_cmd.arg_data_type)
                    {
                        case Data_Types.NONE:
                            {
                                ArgSckPnl.Children.Add(new TextBlock()
                                {
                                    Text = "None",
                                    Margin = new Thickness(30, 0, 0, 0)
                                });
                                break;
                            }
                        case Data_Types.BOOL:
                            {
                                //get the current value from the value table list
                                bool DefaultValue = false;

                                if (!bool.TryParse(scene_command.arg, out DefaultValue))
                                {
                                    device_values dv = _device.device_values.FirstOrDefault(v => v.value_id == d_cmd.custom_data2);
                                    if (dv != null)
                                    {
                                        bool.TryParse(dv.value2, out DefaultValue);
                                    }
                                }
                                arg = DefaultValue.ToString();

                                CheckboxControl control = new CheckboxControl(d_cmd.friendly_name, d_cmd.description, DefaultValue, (isChecked) =>
                                {
                                    arg = isChecked.ToString();
                                }, icon);
                                ArgSckPnl.Children.Add(control);

                                break;
                            }
                        case Data_Types.DECIMAL:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                if (!string.IsNullOrEmpty(scene_command.arg))
                                {
                                    DefaultValue = scene_command.arg;
                                }
                                else
                                {
                                    device_values dv = _device.device_values.FirstOrDefault(v => v.value_id == d_cmd.custom_data2);
                                    if (dv != null)
                                    {
                                        DefaultValue = dv.value2;
                                    }
                                }
                                arg = DefaultValue;

                                NumericControl control = new NumericControl(d_cmd.friendly_name,
                                    d_cmd.description,
                                    DefaultValue,
                                    NumericControl.NumberType.Decimal,
                                    (value) =>
                                    {
                                        arg = value;
                                    }, icon);
                                ArgSckPnl.Children.Add(control);

                                break;
                            }
                        case Data_Types.INTEGER:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                if (!string.IsNullOrEmpty(scene_command.arg))
                                {
                                    DefaultValue = scene_command.arg;
                                }
                                else
                                {
                                    device_values dv = _device.device_values.FirstOrDefault(v => v.value_id == d_cmd.custom_data2);
                                    if (dv != null)
                                    {
                                        DefaultValue = dv.value2;
                                    }
                                }
                                arg = DefaultValue;
                                NumericControl control = new NumericControl(d_cmd.friendly_name,
                                    d_cmd.description,
                                    DefaultValue,
                                    NumericControl.NumberType.Integer,
                                    (value) =>
                                    {
                                        arg = value;
                                    }, icon);
                                ArgSckPnl.Children.Add(control);

                                break;
                            }
                        case Data_Types.BYTE:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                if (!string.IsNullOrEmpty(scene_command.arg))
                                {
                                    DefaultValue = scene_command.arg;
                                }
                                else
                                {
                                    device_values dv = _device.device_values.FirstOrDefault(v => v.value_id == d_cmd.custom_data2);
                                    if (dv != null)
                                    {
                                        DefaultValue = dv.value2;
                                    }
                                }
                                arg = DefaultValue;
                                NumericControl control = new NumericControl(d_cmd.friendly_name,
                                    d_cmd.description,
                                    DefaultValue,
                                    NumericControl.NumberType.Byte,
                                    (value) =>
                                    {
                                        arg = value;
                                    }, icon);
                                ArgSckPnl.Children.Add(control);

                                break;
                            }
                        case Data_Types.SHORT:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                if (!string.IsNullOrEmpty(scene_command.arg))
                                {
                                    DefaultValue = scene_command.arg;
                                }
                                else
                                {
                                    device_values dv = _device.device_values.FirstOrDefault(v => v.value_id == d_cmd.custom_data2);
                                    if (dv != null)
                                    {
                                        DefaultValue = dv.value2;
                                    }
                                }
                                arg = DefaultValue;
                                NumericControl control = new NumericControl(d_cmd.friendly_name,
                                    d_cmd.description,
                                    DefaultValue,
                                    NumericControl.NumberType.Short,
                                    (value) =>
                                    {
                                        arg = value;
                                    }, icon);
                                ArgSckPnl.Children.Add(control);

                                break;
                            }

                        case Data_Types.STRING:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                if (!string.IsNullOrEmpty(scene_command.arg))
                                {
                                    DefaultValue = scene_command.arg;
                                }
                                else
                                {
                                    device_values dv = _device.device_values.FirstOrDefault(v => v.value_id == d_cmd.custom_data2);
                                    if (dv != null)
                                    {
                                        DefaultValue = dv.value2;
                                    }
                                }
                                arg = DefaultValue;
                                StringControl control = new StringControl(d_cmd.friendly_name,
                                    d_cmd.description,
                                    DefaultValue,
                                    (value) =>
                                    {
                                        arg = value;
                                    }, icon);
                                ArgSckPnl.Children.Add(control);

                                break;
                            }
                        case Data_Types.LIST:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                if (!string.IsNullOrEmpty(scene_command.arg))
                                {
                                    DefaultValue = scene_command.arg;
                                }
                                else
                                {
                                    device_values dv = _device.device_values.FirstOrDefault(v => v.value_id == d_cmd.custom_data2);
                                    if (dv != null)
                                    {
                                        DefaultValue = dv.value2;
                                    }
                                }
                                arg = DefaultValue;
                                ComboboxControl control = new ComboboxControl(d_cmd.friendly_name,
                                    d_cmd.description,
                                    d_cmd.device_command_options.Select(o => o.name).ToList(),
                                    DefaultValue,
                                    (value) =>
                                    {
                                        arg = value;
                                    }, icon);
                                ArgSckPnl.Children.Add(control);

                                break;
                            }
                    }
                    #endregion
                }
            }
        }

        private void DeviceCmdRadioBtn_Checked_1(object sender, RoutedEventArgs e)
        {
            DeviceTypeCmdRadioBtn.IsChecked = false;

            _device.device_commands.ToList();
            CmdsCmboBox.ItemsSource = _device.device_commands;

            //Is there a cmd already assigned?
            device_commands cmd = _device.device_commands.FirstOrDefault(o => o.id == scene_command.command_id);
            if (cmd != null)
            {
                CmdsCmboBox.SelectedItem = cmd;
            }
            else
            {
                if (CmdsCmboBox.Items.Count > 0)
                    CmdsCmboBox.SelectedIndex = 0;
            }
        }

        private void DeviceTypeCmdRadioBtn_Checked_1(object sender, RoutedEventArgs e)
        {
            DeviceCmdRadioBtn.IsChecked = false;

            _device.device_types.device_type_commands.ToList();
            CmdsCmboBox.ItemsSource = _device.device_types.device_type_commands;

            //Is there a cmd already assigned?
            device_type_commands cmd = _device.device_types.device_type_commands.FirstOrDefault(o => o.id == scene_command.command_id);
            if (cmd != null)
            {
                CmdsCmboBox.SelectedItem = cmd;
            }
            else
            {
                if (CmdsCmboBox.Items.Count > 0)
                    CmdsCmboBox.SelectedIndex = 0;
            }

        }
    }
}
