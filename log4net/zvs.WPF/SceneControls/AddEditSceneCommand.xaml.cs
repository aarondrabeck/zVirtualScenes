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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using zvs.WPF.DynamicActionControls;
using zvs.Entities;


namespace zvs.WPF.SceneControls
{
    /// <summary>
    /// Interaction logic for AddEditSceneCommand.xaml
    /// </summary>
    public partial class AddEditSceneCommand : Window
    {
        private zvsContext context;
        private SceneCommand scene_command;
        private Device _device;
        private BitmapImage icon = new BitmapImage(new Uri("pack://application:,,,/zVirtualScenes;component/Images/save_check.png"));
        private string arg = string.Empty;

        public AddEditSceneCommand(zvsContext context, SceneCommand scene_command)
        {
            this.context = context;
            this.scene_command = scene_command;
            _device = scene_command.Device;

            if (_device == null)
                this.Close();
            else
                this.Title = string.Format("'{0}' Commands", _device.Name);

            InitializeComponent();
        }

        ~AddEditSceneCommand()
        {
            Debug.WriteLine("AddEditSceneCommand Deconstructed.");
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {

            this.Title = string.Format("Scene command for '{0}'", _device.Name);
            if ( scene_command.Command is DeviceTypeCommand)
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

            if (CmdsCmboBox.SelectedItem is DeviceTypeCommand)
            {
                DeviceTypeCommand d_cmd = (DeviceTypeCommand)CmdsCmboBox.SelectedItem;
                scene_command.Command = d_cmd;
            }
            else if (CmdsCmboBox.SelectedItem is DeviceCommand)
            {
                DeviceCommand d_cmd = (DeviceCommand)CmdsCmboBox.SelectedItem;
                scene_command.Command = d_cmd;
            }

            scene_command.Argument = arg;

            this.DialogResult = true;
            this.Close();
        }

        private void CmdsCmboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ArgSckPnl.Children.Clear();
            if (CmdsCmboBox.SelectedItem != null)
            {
                if (CmdsCmboBox.SelectedItem is DeviceTypeCommand)
                {
                    DeviceTypeCommand d_cmd = (DeviceTypeCommand)CmdsCmboBox.SelectedItem;
                    #region Device Type Commands
                    switch (d_cmd.ArgumentType)
                    {
                        case DataType.NONE:
                            {
                                ArgSckPnl.Children.Add(new TextBlock()
                                {
                                    Text = "None",
                                    Margin = new Thickness(30, 0, 0, 0)
                                });
                                break;
                            }
                        case DataType.BOOL:
                            {
                                //get the current value from the value table list
                                bool DefaultValue = false;
                                if (!bool.TryParse(scene_command.Argument, out DefaultValue))
                                {
                                    DeviceValue dv = _device.Values.FirstOrDefault(v => v.UniqueIdentifier == d_cmd.CustomData2);
                                    if (dv != null)
                                    {
                                        bool.TryParse(dv.Value, out DefaultValue);
                                    }
                                }
                                arg = DefaultValue.ToString();

                                CheckboxControl control = new CheckboxControl(d_cmd.Name, d_cmd.Description, DefaultValue, (isChecked) =>
                                {
                                    arg = isChecked.ToString();
                                }, icon);
                                ArgSckPnl.Children.Add(control);

                                break;
                            }
                        case DataType.DECIMAL:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                if (!string.IsNullOrEmpty(scene_command.Argument))
                                {
                                    DefaultValue = scene_command.Argument;
                                }
                                else
                                {
                                    DeviceValue dv = _device.Values.FirstOrDefault(v => v.UniqueIdentifier == d_cmd.CustomData2);
                                    if (dv != null)
                                    {
                                        DefaultValue = dv.Value;
                                    }
                                }
                                arg = DefaultValue;
                                NumericControl control = new NumericControl(d_cmd.Name,
                                    d_cmd.Description,
                                    DefaultValue,
                                    NumericControl.NumberType.Decimal,
                                    (value) =>
                                    {
                                        arg = value;
                                    }, icon);
                                ArgSckPnl.Children.Add(control);

                                break;
                            }
                        case DataType.INTEGER:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                if (!string.IsNullOrEmpty(scene_command.Argument))
                                {
                                    DefaultValue = scene_command.Argument;
                                }
                                else
                                {
                                    DeviceValue dv = _device.Values.FirstOrDefault(v => v.UniqueIdentifier == d_cmd.CustomData2);
                                    if (dv != null)
                                    {
                                        DefaultValue = dv.Value;
                                    }
                                }
                                arg = DefaultValue;
                                NumericControl control = new NumericControl(d_cmd.Name,
                                    d_cmd.Description,
                                    DefaultValue,
                                   NumericControl.NumberType.Integer,
                                    (value) =>
                                    {
                                        arg = value;
                                    }, icon);
                                ArgSckPnl.Children.Add(control);

                                break;
                            }
                        case DataType.SHORT:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                if (!string.IsNullOrEmpty(scene_command.Argument))
                                {
                                    DefaultValue = scene_command.Argument;
                                }
                                else
                                {
                                    DeviceValue dv = _device.Values.FirstOrDefault(v => v.UniqueIdentifier == d_cmd.CustomData2);
                                    if (dv != null)
                                    {
                                        DefaultValue = dv.Value;
                                    }
                                }
                                arg = DefaultValue;
                                NumericControl control = new NumericControl(d_cmd.Name,
                                    d_cmd.Description,
                                    DefaultValue,
                                    NumericControl.NumberType.Short,
                                    (value) =>
                                    {
                                        arg = value;
                                    }, icon);
                                ArgSckPnl.Children.Add(control);

                                break;
                            }
                        case DataType.BYTE:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                if (!string.IsNullOrEmpty(scene_command.Argument))
                                {
                                    DefaultValue = scene_command.Argument;
                                }
                                else
                                {
                                    DeviceValue dv = _device.Values.FirstOrDefault(v => v.UniqueIdentifier == d_cmd.CustomData2);
                                    if (dv != null)
                                    {
                                        DefaultValue = dv.Value;
                                    }
                                }
                                arg = DefaultValue;
                                NumericControl control = new NumericControl(d_cmd.Name,
                                    d_cmd.Description,
                                    DefaultValue,
                                   NumericControl.NumberType.Byte,
                                    (value) =>
                                    {
                                        arg = value;
                                    }, icon);
                                ArgSckPnl.Children.Add(control);

                                break;
                            }
                        case DataType.STRING:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0"; if (!string.IsNullOrEmpty(scene_command.Argument))
                                {
                                    DefaultValue = scene_command.Argument;
                                }
                                else
                                {
                                    DeviceValue dv = _device.Values.FirstOrDefault(v => v.UniqueIdentifier == d_cmd.CustomData2);
                                    if (dv != null)
                                    {
                                        DefaultValue = dv.Value;
                                    }
                                }
                                arg = DefaultValue;
                                StringControl control = new StringControl(d_cmd.Name,
                                    d_cmd.Description,
                                    DefaultValue,
                                    (value) =>
                                    {
                                        arg = value;
                                    }, icon);
                                ArgSckPnl.Children.Add(control);

                                break;
                            }
                        case DataType.LIST:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                if (!string.IsNullOrEmpty(scene_command.Argument))
                                {
                                    DefaultValue = scene_command.Argument;
                                }
                                else
                                {
                                    DeviceValue dv = _device.Values.FirstOrDefault(v => v.UniqueIdentifier == d_cmd.CustomData2);
                                    if (dv != null)
                                    {
                                        DefaultValue = dv.Value;
                                    }
                                }
                                arg = DefaultValue;
                                ComboboxControl control = new ComboboxControl(d_cmd.Name,
                                    d_cmd.Description,
                                    d_cmd.Options.Select(o => o.Name).ToList(),
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

                if (CmdsCmboBox.SelectedItem is DeviceCommand)
                {
                    DeviceCommand d_cmd = (DeviceCommand)CmdsCmboBox.SelectedItem;
                    #region Device Commands

                    switch (d_cmd.ArgumentType)
                    {
                        case DataType.NONE:
                            {
                                ArgSckPnl.Children.Add(new TextBlock()
                                {
                                    Text = "None",
                                    Margin = new Thickness(30, 0, 0, 0)
                                });
                                break;
                            }
                        case DataType.BOOL:
                            {
                                //get the current value from the value table list
                                bool DefaultValue = false;

                                if (!bool.TryParse(scene_command.Argument, out DefaultValue))
                                {
                                    DeviceValue dv = _device.Values.FirstOrDefault(v => v.UniqueIdentifier == d_cmd.CustomData2);
                                    if (dv != null)
                                    {
                                        bool.TryParse(dv.Value, out DefaultValue);
                                    }
                                }
                                arg = DefaultValue.ToString();

                                CheckboxControl control = new CheckboxControl(d_cmd.Name, d_cmd.Description, DefaultValue, (isChecked) =>
                                {
                                    arg = isChecked.ToString();
                                }, icon);
                                ArgSckPnl.Children.Add(control);

                                break;
                            }
                        case DataType.DECIMAL:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                if (!string.IsNullOrEmpty(scene_command.Argument))
                                {
                                    DefaultValue = scene_command.Argument;
                                }
                                else
                                {
                                    DeviceValue dv = _device.Values.FirstOrDefault(v => v.UniqueIdentifier == d_cmd.CustomData2);
                                    if (dv != null)
                                    {
                                        DefaultValue = dv.Value;
                                    }
                                }
                                arg = DefaultValue;

                                NumericControl control = new NumericControl(d_cmd.Name,
                                    d_cmd.Description,
                                    DefaultValue,
                                    NumericControl.NumberType.Decimal,
                                    (value) =>
                                    {
                                        arg = value;
                                    }, icon);
                                ArgSckPnl.Children.Add(control);

                                break;
                            }
                        case DataType.INTEGER:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                if (!string.IsNullOrEmpty(scene_command.Argument))
                                {
                                    DefaultValue = scene_command.Argument;
                                }
                                else
                                {
                                    DeviceValue dv = _device.Values.FirstOrDefault(v => v.UniqueIdentifier == d_cmd.CustomData2);
                                    if (dv != null)
                                    {
                                        DefaultValue = dv.Value;
                                    }
                                }
                                arg = DefaultValue;
                                NumericControl control = new NumericControl(d_cmd.Name,
                                    d_cmd.Description,
                                    DefaultValue,
                                    NumericControl.NumberType.Integer,
                                    (value) =>
                                    {
                                        arg = value;
                                    }, icon);
                                ArgSckPnl.Children.Add(control);

                                break;
                            }
                        case DataType.BYTE:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                if (!string.IsNullOrEmpty(scene_command.Argument))
                                {
                                    DefaultValue = scene_command.Argument;
                                }
                                else
                                {
                                    DeviceValue dv = _device.Values.FirstOrDefault(v => v.UniqueIdentifier == d_cmd.CustomData2);
                                    if (dv != null)
                                    {
                                        DefaultValue = dv.Value;
                                    }
                                }
                                arg = DefaultValue;
                                NumericControl control = new NumericControl(d_cmd.Name,
                                    d_cmd.Description,
                                    DefaultValue,
                                    NumericControl.NumberType.Byte,
                                    (value) =>
                                    {
                                        arg = value;
                                    }, icon);
                                ArgSckPnl.Children.Add(control);

                                break;
                            }
                        case DataType.SHORT:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                if (!string.IsNullOrEmpty(scene_command.Argument))
                                {
                                    DefaultValue = scene_command.Argument;
                                }
                                else
                                {
                                    DeviceValue dv = _device.Values.FirstOrDefault(v => v.UniqueIdentifier == d_cmd.CustomData2);
                                    if (dv != null)
                                    {
                                        DefaultValue = dv.Value;
                                    }
                                }
                                arg = DefaultValue;
                                NumericControl control = new NumericControl(d_cmd.Name,
                                    d_cmd.Description,
                                    DefaultValue,
                                    NumericControl.NumberType.Short,
                                    (value) =>
                                    {
                                        arg = value;
                                    }, icon);
                                ArgSckPnl.Children.Add(control);

                                break;
                            }

                        case DataType.STRING:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                if (!string.IsNullOrEmpty(scene_command.Argument))
                                {
                                    DefaultValue = scene_command.Argument;
                                }
                                else
                                {
                                    DeviceValue dv = _device.Values.FirstOrDefault(v => v.UniqueIdentifier == d_cmd.CustomData2);
                                    if (dv != null)
                                    {
                                        DefaultValue = dv.Value;
                                    }
                                }
                                arg = DefaultValue;
                                StringControl control = new StringControl(d_cmd.Name,
                                    d_cmd.Description,
                                    DefaultValue,
                                    (value) =>
                                    {
                                        arg = value;
                                    }, icon);
                                ArgSckPnl.Children.Add(control);

                                break;
                            }
                        case DataType.LIST:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                if (!string.IsNullOrEmpty(scene_command.Argument))
                                {
                                    DefaultValue = scene_command.Argument;
                                }
                                else
                                {
                                    DeviceValue dv = _device.Values.FirstOrDefault(v => v.UniqueIdentifier == d_cmd.CustomData2);
                                    if (dv != null)
                                    {
                                        DefaultValue = dv.Value;
                                    }
                                }
                                arg = DefaultValue;
                                ComboboxControl control = new ComboboxControl(d_cmd.Name,
                                    d_cmd.Description,
                                    d_cmd.Options.Select(o => o.Name).ToList(),
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

            _device.Commands.ToList();
            CmdsCmboBox.ItemsSource = _device.Commands;

            if (scene_command.Command == null)
            {
                if (CmdsCmboBox.Items.Count > 0)
                    CmdsCmboBox.SelectedIndex = 0;
                return;
            }

            //Is there a cmd already assigned?
            DeviceCommand cmd = _device.Commands.FirstOrDefault(o => o.CommandId == scene_command.Command.CommandId);
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

            _device.Type.Commands.ToList();
            CmdsCmboBox.ItemsSource = _device.Type.Commands;

            if (scene_command.Command == null)
            {
                if (CmdsCmboBox.Items.Count > 0)
                    CmdsCmboBox.SelectedIndex = 0;
                return;
            }

            //Is there a cmd already assigned?
            DeviceTypeCommand cmd = _device.Type.Commands.FirstOrDefault(o => o.CommandId == scene_command.Command.CommandId);
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
