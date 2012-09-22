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
using zvs.Entities;
using zvs.WPF.DynamicActionControls;

namespace zvs.WPF.Commands
{
    /// <summary>
    /// Interaction logic for CommandBuilder.xaml
    /// </summary>
    public partial class CommandBuilder : Window
    {
        private zvsContext Context;
        private StoredCommand StoredCommand;
        private BitmapImage icon = new BitmapImage(new Uri("pack://application:,,,/zVirtualScenes;component/Images/save_check.png"));

        //temp variable to save selected built-in command data
        private string SelectedBuiltinArg = string.Empty;

        //temp variable to save selected device command data
        private string SelectedDeviceArg = string.Empty;

        public CommandBuilder(zvsContext context, StoredCommand storedCommand)
        {
            Context = context;
            StoredCommand = storedCommand;
            InitializeComponent();
        }

        private void Close_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            InitializeDeviceCommands();
            InitializeBuiltinCommands();
            InitializeJavaScriptCommands();
        }

        private void InitializeDeviceCommands()
        {
            //Fill the device combo box from db
            Context.Devices.ToList();
            DevicesCmboBox.ItemsSource = Context.Devices.Local;
            if (DevicesCmboBox.Items.Count > 0)
                DevicesCmboBox.SelectedIndex = 0;

            //If we are editing, ie. we get passed a StoredCommand with data, 
            //preselect the correct tab and item.
            if (StoredCommand.Command is DeviceCommand ||
                StoredCommand.Command is DeviceTypeCommand)
            {
                DeviceTab.IsSelected = true;
                if (StoredCommand.Device != null)
                {
                    //Preselect device
                    DevicesCmboBox.SelectedItem = StoredCommand.Device;

                    //Preselect device command
                    if (StoredCommand.Command != null)
                    {
                        if (StoredCommand.Command is DeviceCommand)
                        {
                            DeviceCommand cmd = Context.DeviceCommands.FirstOrDefault(o => o.CommandId == StoredCommand.Command.CommandId);
                            if (cmd != null)
                                DeviceCmdsCmboBox.SelectedItem = cmd;
                        }
                        if (StoredCommand.Command is DeviceTypeCommand)
                        {
                            DeviceTypeCommand cmd = Context.DeviceTypeCommands.FirstOrDefault(o => o.CommandId == StoredCommand.Command.CommandId);
                            if (cmd != null)
                                DeviceCmdsCmboBox.SelectedItem = cmd;
                        }
                    }
                }
            }
        }

        private void InitializeBuiltinCommands()
        {
            //Fill the Built-in command box with available command from the DB
            Context.BuiltinCommands.ToList();
            BuiltinCmdsCmboBox.ItemsSource = Context.BuiltinCommands.Local;
            if (BuiltinCmdsCmboBox.Items.Count > 0)
                BuiltinCmdsCmboBox.SelectedIndex = 0;

            //If we are editing, ie. we get passed a StoredCommand with data, 
            //preselect the correct tab and item.
            if (StoredCommand.Command is BuiltinCommand)
            {
                BuiltinTab.IsSelected = true;
                BuiltinCommand cmd = (BuiltinCommand)StoredCommand.Command;
                //set builtinArg so the UI can fill to the preselected arg
                SelectedBuiltinArg = StoredCommand.Argument;
                //preselect the built-in command
                BuiltinCmdsCmboBox.SelectedItem = cmd;
            }
        }

        private void InitializeJavaScriptCommands()
        {
            // Do not load your data at design time.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                System.Windows.Data.CollectionViewSource CmdsViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("JSCmdsViewSource")));
                Context.JavaScriptCommands.ToList();
                CmdsViewSource.Source = Context.JavaScriptCommands.Local;
            }

            //If we are editing, ie. we get passed a StoredCommand with data, 
            //preselect the correct tab and item.
            if (StoredCommand.Command is JavaScriptCommand)
            {
                JavaScriptTab.IsSelected = true;
                JavaScriptCommand cmd = (JavaScriptCommand)StoredCommand.Command;
                JavaScriptCmboBox.SelectedItem = cmd;
            }
        }

        private void BuiltinCmdsCmboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            BuiltinArgSckPnl.Children.Clear();
            BuiltinCommand selected_cmd = (BuiltinCommand)BuiltinCmdsCmboBox.SelectedItem;

            switch (selected_cmd.UniqueIdentifier)
            {
                #region Do Custom things for some Builtin Commands
                case "REPOLL_ME":
                    {
                        string default_value = string.Empty;

                        //Lookup the device involved in the command
                        int deviceID = 0;
                        if (int.TryParse(SelectedBuiltinArg, out deviceID))
                        {
                            Device d = Context.Devices.FirstOrDefault(o => o.DeviceId == deviceID);
                            if (d != null)
                            {
                                default_value = d.Name;
                                SelectedBuiltinArg = d.DeviceId.ToString();
                            }
                        }

                        //If this is a new command or we cannot find the old device, just preselect the first device.
                        if (string.IsNullOrEmpty(default_value))
                        {
                            Device d = Context.Devices.FirstOrDefault();
                            if (d != null)
                            {
                                default_value = d.Name;
                                SelectedBuiltinArg = d.DeviceId.ToString();
                            }
                        }

                        ComboboxControl control = new ComboboxControl(selected_cmd.Name,
                            selected_cmd.Description,
                            Context.Devices.Select(o => o.Name).ToList(),
                            default_value,
                            (value) =>
                            {
                                Device d = Context.Devices.FirstOrDefault(o => o.Name == value);
                                if (d != null)
                                    SelectedBuiltinArg = d.DeviceId.ToString();
                            }, icon);
                        BuiltinArgSckPnl.Children.Add(control);
                        break;
                    }
                case "GROUP_ON":
                case "GROUP_OFF":
                    {
                        string default_value = string.Empty;

                        //Lookup the group involved in the command
                        int groupID = 0;
                        if (int.TryParse(SelectedBuiltinArg, out groupID))
                        {
                            Group g = Context.Groups.FirstOrDefault(o => o.GroupId == groupID);
                            if (g != null)
                            {
                                default_value = g.Name;
                                SelectedBuiltinArg = g.GroupId.ToString();
                            }
                        }

                        //If this is a new command or we cannot find the old group, just preselect the first group.
                        if (string.IsNullOrEmpty(default_value))
                        {
                            Group g = Context.Groups.FirstOrDefault();
                            if (g != null)
                            {
                                default_value = g.Name;
                                SelectedBuiltinArg = g.GroupId.ToString();
                            }
                        }

                        ComboboxControl control = new ComboboxControl(selected_cmd.Name,
                            selected_cmd.Description,
                            Context.Groups.Select(o => o.Name).ToList(),
                            default_value,
                            (value) =>
                            {
                                Group g = Context.Groups.FirstOrDefault(o => o.Name == value);
                                if (g != null)
                                    SelectedBuiltinArg = g.GroupId.ToString();
                            }, icon);
                        BuiltinArgSckPnl.Children.Add(control);
                    }
                    break;
                case "RUN_SCENE":
                    {
                        string default_value = string.Empty;

                        //Try to match supplied arg (sceneID) with a scene
                        int sceneID = 0;
                        if (int.TryParse(SelectedBuiltinArg, out sceneID))
                        {
                            Scene s = Context.Scenes.FirstOrDefault(o => o.SceneId == sceneID);
                            if (s != null)
                            {
                                default_value = s.Name;
                            }
                        }

                        //If this is a new command or we cannot find the old scene, 
                        //just preselect the first scene.
                        if (string.IsNullOrEmpty(default_value))
                        {
                            Scene s = Context.Scenes.FirstOrDefault();
                            if (s != null)
                            {
                                default_value = s.Name;
                                SelectedBuiltinArg = s.SceneId.ToString();
                            }
                        }

                        ComboboxControl control = new ComboboxControl(selected_cmd.Name,
                            selected_cmd.Description,
                            Context.Scenes.Select(o => o.Name).ToList(),
                            default_value,
                            (value) =>
                            {
                                Scene s = Context.Scenes.FirstOrDefault(o => o.Name == value);
                                if (s != null)
                                    SelectedBuiltinArg = s.SceneId.ToString();
                            }, icon);
                        BuiltinArgSckPnl.Children.Add(control);
                        break;
                    }
                #endregion
                default:
                    {
                        #region Built-in Commands
                        switch (selected_cmd.ArgumentType)
                        {
                            case DataType.NONE:
                                {
                                    BuiltinArgSckPnl.Children.Add(new TextBlock()
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
                                    bool.TryParse(SelectedBuiltinArg, out DefaultValue);
                                    SelectedBuiltinArg = DefaultValue.ToString();

                                    CheckboxControl control = new CheckboxControl(selected_cmd.Name,
                                        selected_cmd.Description,
                                        DefaultValue, (isChecked) =>
                                        {
                                            SelectedBuiltinArg = isChecked.ToString();
                                        }, icon);
                                    BuiltinArgSckPnl.Children.Add(control);

                                    break;
                                }
                            case DataType.DECIMAL:
                                {
                                    string DefaultValue = "0";
                                    if (!string.IsNullOrEmpty(SelectedBuiltinArg))
                                        DefaultValue = SelectedBuiltinArg;
                                    SelectedBuiltinArg = DefaultValue;

                                    NumericControl control = new NumericControl(selected_cmd.Name,
                                        selected_cmd.Description,
                                        DefaultValue,
                                        NumericControl.NumberType.Decimal,
                                        (value) =>
                                        {
                                            SelectedBuiltinArg = value;
                                        }, icon);
                                    BuiltinArgSckPnl.Children.Add(control);

                                    break;
                                }
                            case DataType.INTEGER:
                                {
                                    string DefaultValue = "0";
                                    if (!string.IsNullOrEmpty(SelectedBuiltinArg))
                                        DefaultValue = SelectedBuiltinArg;
                                    SelectedBuiltinArg = DefaultValue;

                                    NumericControl control = new NumericControl(selected_cmd.Name,
                                        selected_cmd.Description,
                                        DefaultValue,
                                        NumericControl.NumberType.Integer,
                                        (value) =>
                                        {
                                            SelectedBuiltinArg = value;
                                        }, icon);
                                    BuiltinArgSckPnl.Children.Add(control);

                                    break;
                                }
                            case DataType.BYTE:
                                {
                                    string DefaultValue = "0";
                                    if (!string.IsNullOrEmpty(SelectedBuiltinArg))
                                        DefaultValue = SelectedBuiltinArg;
                                    SelectedBuiltinArg = DefaultValue;

                                    NumericControl control = new NumericControl(selected_cmd.Name,
                                        selected_cmd.Description,
                                        DefaultValue,
                                        NumericControl.NumberType.Byte,
                                        (value) =>
                                        {
                                            SelectedBuiltinArg = value;
                                        }, icon);
                                    BuiltinArgSckPnl.Children.Add(control);

                                    break;
                                }
                            case DataType.SHORT:
                                {
                                    string DefaultValue = "0";
                                    if (!string.IsNullOrEmpty(SelectedBuiltinArg))
                                        DefaultValue = SelectedBuiltinArg;
                                    SelectedBuiltinArg = DefaultValue;

                                    NumericControl control = new NumericControl(selected_cmd.Name,
                                        selected_cmd.Description,
                                        DefaultValue,
                                        NumericControl.NumberType.Short,
                                        (value) =>
                                        {
                                            SelectedBuiltinArg = value;
                                        }, icon);
                                    BuiltinArgSckPnl.Children.Add(control);

                                    break;
                                }

                            case DataType.STRING:
                                {
                                    //get the current value from the value table list
                                    string DefaultValue = "0";
                                    if (!string.IsNullOrEmpty(SelectedBuiltinArg))
                                        DefaultValue = SelectedBuiltinArg;
                                    SelectedBuiltinArg = DefaultValue;

                                    StringControl control = new StringControl(selected_cmd.Name,
                                        selected_cmd.Description,
                                        DefaultValue,
                                        (value) =>
                                        {
                                            SelectedBuiltinArg = value;
                                        }, icon);
                                    BuiltinArgSckPnl.Children.Add(control);

                                    break;
                                }
                            case DataType.LIST:
                                {
                                    string DefaultValue = "";
                                    string option = selected_cmd.Options.FirstOrDefault().Name;

                                    if (option != null)
                                        DefaultValue = option;

                                    if (!string.IsNullOrEmpty(SelectedBuiltinArg))
                                        DefaultValue = SelectedBuiltinArg;
                                    SelectedBuiltinArg = DefaultValue;

                                    ComboboxControl control = new ComboboxControl(selected_cmd.Name,
                                        selected_cmd.Description,
                                        selected_cmd.Options.Select(o => o.Name).ToList(),
                                        DefaultValue,
                                        (value) =>
                                        {
                                            SelectedBuiltinArg = value;
                                        }, icon);
                                    BuiltinArgSckPnl.Children.Add(control);

                                    break;
                                }
                        }
                        #endregion
                        break;
                    }
            }
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DeviceTab.IsSelected)
            {
                if (DevicesCmboBox.SelectedItem == null)
                {
                    MessageBox.Show("You must select a device", "No Device Selected");
                    return;
                }

                if (DeviceCmdsCmboBox.SelectedItem == null)
                {
                    MessageBox.Show("You must select a command", "No Command Selected");
                    return;
                }

                //Set Device
                StoredCommand.Device = (Device)DevicesCmboBox.SelectedItem;

                //Set Command and Arg
                if (DeviceCmdsCmboBox.SelectedItem is DeviceCommand)
                {
                    StoredCommand.Command = (DeviceCommand)DeviceCmdsCmboBox.SelectedItem;
                    StoredCommand.Argument = SelectedDeviceArg;
                }
                if (DeviceCmdsCmboBox.SelectedItem is DeviceTypeCommand)
                {
                    StoredCommand.Command = (DeviceTypeCommand)DeviceCmdsCmboBox.SelectedItem;
                    StoredCommand.Argument = SelectedDeviceArg;
                }

                this.DialogResult = true;
                this.Close();
            }

            if (BuiltinTab.IsSelected)
            {
                if (BuiltinCmdsCmboBox.SelectedItem == null)
                {
                    MessageBox.Show("You must select a command", "No Command Selected");
                    return;
                }

                //Set Command and Arg
                if (BuiltinCmdsCmboBox.SelectedItem is BuiltinCommand)
                {
                    StoredCommand.Command = (BuiltinCommand)BuiltinCmdsCmboBox.SelectedItem;
                    StoredCommand.Argument = SelectedBuiltinArg;
                }

                this.DialogResult = true;
                this.Close();
            }

            if (JavaScriptTab.IsSelected)
            {
                if (JavaScriptCmboBox.SelectedItem == null)
                {
                    MessageBox.Show("You must select a command", "No Command Selected");
                    return;
                }

                //Set Command and Arg
                if (JavaScriptCmboBox.SelectedItem is JavaScriptCommand)
                    StoredCommand.Command = (JavaScriptCommand)JavaScriptCmboBox.SelectedItem;

                this.DialogResult = true;
                this.Close();
            }


        }

        private void DevicesCmboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            Device SelectedDevice = (Device)DevicesCmboBox.SelectedItem;
            if (SelectedDevice != null)
            {
                List<object> DeviceCommands = new List<object>();
                SelectedDevice.Type.Commands.ToList().ForEach(o => DeviceCommands.Add(o));
                SelectedDevice.Commands.ToList().ForEach(o => DeviceCommands.Add(o));

                DeviceCmdsCmboBox.ItemsSource = DeviceCommands;
                if (DeviceCmdsCmboBox.Items.Count > 0)
                    DeviceCmdsCmboBox.SelectedIndex = 0;


            }
        }

        private void DeviceCmdsCmboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            Device selectedDevice = (Device)DevicesCmboBox.SelectedItem;
            DeviceArgSckPnl.Children.Clear();
            if (DeviceCmdsCmboBox.SelectedItem != null)
            {
                if (DeviceCmdsCmboBox.SelectedItem is DeviceTypeCommand)
                {
                    DeviceTypeCommand d_cmd = (DeviceTypeCommand)DeviceCmdsCmboBox.SelectedItem;
                    #region Device Type Commands
                    switch (d_cmd.ArgumentType)
                    {
                        case DataType.NONE:
                            {
                                DeviceArgSckPnl.Children.Add(new TextBlock()
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
                                if (!bool.TryParse(StoredCommand.Argument, out DefaultValue))
                                {
                                    DeviceValue dv = selectedDevice.Values.FirstOrDefault(v => v.UniqueIdentifier == d_cmd.CustomData2);
                                    if (dv != null)
                                    {
                                        bool.TryParse(dv.Value, out DefaultValue);
                                    }
                                }
                                SelectedDeviceArg = DefaultValue.ToString();

                                CheckboxControl control = new CheckboxControl(d_cmd.Name, d_cmd.Description, DefaultValue, (isChecked) =>
                                {
                                    SelectedDeviceArg = isChecked.ToString();
                                }, icon);
                                DeviceArgSckPnl.Children.Add(control);

                                break;
                            }
                        case DataType.DECIMAL:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                if (!string.IsNullOrEmpty(StoredCommand.Argument))
                                {
                                    DefaultValue = StoredCommand.Argument;
                                }
                                else
                                {
                                    DeviceValue dv = selectedDevice.Values.FirstOrDefault(v => v.UniqueIdentifier == d_cmd.CustomData2);
                                    if (dv != null)
                                    {
                                        DefaultValue = dv.Value;
                                    }
                                }
                                SelectedDeviceArg = DefaultValue;
                                NumericControl control = new NumericControl(d_cmd.Name,
                                    d_cmd.Description,
                                    DefaultValue,
                                    NumericControl.NumberType.Decimal,
                                    (value) =>
                                    {
                                        SelectedDeviceArg = value;
                                    }, icon);
                                DeviceArgSckPnl.Children.Add(control);

                                break;
                            }
                        case DataType.INTEGER:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                if (!string.IsNullOrEmpty(StoredCommand.Argument))
                                {
                                    DefaultValue = StoredCommand.Argument;
                                }
                                else
                                {
                                    DeviceValue dv = selectedDevice.Values.FirstOrDefault(v => v.UniqueIdentifier == d_cmd.CustomData2);
                                    if (dv != null)
                                    {
                                        DefaultValue = dv.Value;
                                    }
                                }
                                SelectedDeviceArg = DefaultValue;
                                NumericControl control = new NumericControl(d_cmd.Name,
                                    d_cmd.Description,
                                    DefaultValue,
                                   NumericControl.NumberType.Integer,
                                    (value) =>
                                    {
                                        SelectedDeviceArg = value;
                                    }, icon);
                                DeviceArgSckPnl.Children.Add(control);

                                break;
                            }
                        case DataType.SHORT:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                if (!string.IsNullOrEmpty(StoredCommand.Argument))
                                {
                                    DefaultValue = StoredCommand.Argument;
                                }
                                else
                                {
                                    DeviceValue dv = selectedDevice.Values.FirstOrDefault(v => v.UniqueIdentifier == d_cmd.CustomData2);
                                    if (dv != null)
                                    {
                                        DefaultValue = dv.Value;
                                    }
                                }
                                SelectedDeviceArg = DefaultValue;
                                NumericControl control = new NumericControl(d_cmd.Name,
                                    d_cmd.Description,
                                    DefaultValue,
                                    NumericControl.NumberType.Short,
                                    (value) =>
                                    {
                                        SelectedDeviceArg = value;
                                    }, icon);
                                DeviceArgSckPnl.Children.Add(control);

                                break;
                            }
                        case DataType.BYTE:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                if (!string.IsNullOrEmpty(StoredCommand.Argument))
                                {
                                    DefaultValue = StoredCommand.Argument;
                                }
                                else
                                {
                                    DeviceValue dv = selectedDevice.Values.FirstOrDefault(v => v.UniqueIdentifier == d_cmd.CustomData2);
                                    if (dv != null)
                                    {
                                        DefaultValue = dv.Value;
                                    }
                                }
                                SelectedDeviceArg = DefaultValue;
                                NumericControl control = new NumericControl(d_cmd.Name,
                                    d_cmd.Description,
                                    DefaultValue,
                                   NumericControl.NumberType.Byte,
                                    (value) =>
                                    {
                                        SelectedDeviceArg = value;
                                    }, icon);
                                DeviceArgSckPnl.Children.Add(control);

                                break;
                            }
                        case DataType.STRING:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0"; if (!string.IsNullOrEmpty(StoredCommand.Argument))
                                {
                                    DefaultValue = StoredCommand.Argument;
                                }
                                else
                                {
                                    DeviceValue dv = selectedDevice.Values.FirstOrDefault(v => v.UniqueIdentifier == d_cmd.CustomData2);
                                    if (dv != null)
                                    {
                                        DefaultValue = dv.Value;
                                    }
                                }
                                SelectedDeviceArg = DefaultValue;
                                StringControl control = new StringControl(d_cmd.Name,
                                    d_cmd.Description,
                                    DefaultValue,
                                    (value) =>
                                    {
                                        SelectedDeviceArg = value;
                                    }, icon);
                                DeviceArgSckPnl.Children.Add(control);

                                break;
                            }
                        case DataType.LIST:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                if (!string.IsNullOrEmpty(StoredCommand.Argument))
                                {
                                    DefaultValue = StoredCommand.Argument;
                                }
                                else
                                {
                                    DeviceValue dv = selectedDevice.Values.FirstOrDefault(v => v.UniqueIdentifier == d_cmd.CustomData2);
                                    if (dv != null)
                                    {
                                        DefaultValue = dv.Value;
                                    }
                                }
                                SelectedDeviceArg = DefaultValue;
                                ComboboxControl control = new ComboboxControl(d_cmd.Name,
                                    d_cmd.Description,
                                    d_cmd.Options.Select(o => o.Name).ToList(),
                                    DefaultValue,
                                    (value) =>
                                    {
                                        SelectedDeviceArg = value;
                                    }, icon);
                                DeviceArgSckPnl.Children.Add(control);

                                break;
                            }

                    }
                    #endregion
                }

                if (DeviceCmdsCmboBox.SelectedItem is DeviceCommand)
                {
                    DeviceCommand d_cmd = (DeviceCommand)DeviceCmdsCmboBox.SelectedItem;
                    #region Device Commands

                    switch (d_cmd.ArgumentType)
                    {
                        case DataType.NONE:
                            {
                                DeviceArgSckPnl.Children.Add(new TextBlock()
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

                                if (!bool.TryParse(StoredCommand.Argument, out DefaultValue))
                                {
                                    DeviceValue dv = selectedDevice.Values.FirstOrDefault(v => v.UniqueIdentifier == d_cmd.CustomData2);
                                    if (dv != null)
                                    {
                                        bool.TryParse(dv.Value, out DefaultValue);
                                    }
                                }
                                SelectedDeviceArg = DefaultValue.ToString();

                                CheckboxControl control = new CheckboxControl(d_cmd.Name, d_cmd.Description, DefaultValue, (isChecked) =>
                                {
                                    SelectedDeviceArg = isChecked.ToString();
                                }, icon);
                                DeviceArgSckPnl.Children.Add(control);

                                break;
                            }
                        case DataType.DECIMAL:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                if (!string.IsNullOrEmpty(StoredCommand.Argument))
                                {
                                    DefaultValue = StoredCommand.Argument;
                                }
                                else
                                {
                                    DeviceValue dv = selectedDevice.Values.FirstOrDefault(v => v.UniqueIdentifier == d_cmd.CustomData2);
                                    if (dv != null)
                                    {
                                        DefaultValue = dv.Value;
                                    }
                                }
                                SelectedDeviceArg = DefaultValue;

                                NumericControl control = new NumericControl(d_cmd.Name,
                                    d_cmd.Description,
                                    DefaultValue,
                                    NumericControl.NumberType.Decimal,
                                    (value) =>
                                    {
                                        SelectedDeviceArg = value;
                                    }, icon);
                                DeviceArgSckPnl.Children.Add(control);

                                break;
                            }
                        case DataType.INTEGER:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                if (!string.IsNullOrEmpty(StoredCommand.Argument))
                                {
                                    DefaultValue = StoredCommand.Argument;
                                }
                                else
                                {
                                    DeviceValue dv = selectedDevice.Values.FirstOrDefault(v => v.UniqueIdentifier == d_cmd.CustomData2);
                                    if (dv != null)
                                    {
                                        DefaultValue = dv.Value;
                                    }
                                }
                                SelectedDeviceArg = DefaultValue;
                                NumericControl control = new NumericControl(d_cmd.Name,
                                    d_cmd.Description,
                                    DefaultValue,
                                    NumericControl.NumberType.Integer,
                                    (value) =>
                                    {
                                        SelectedDeviceArg = value;
                                    }, icon);
                                DeviceArgSckPnl.Children.Add(control);

                                break;
                            }
                        case DataType.BYTE:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                if (!string.IsNullOrEmpty(StoredCommand.Argument))
                                {
                                    DefaultValue = StoredCommand.Argument;
                                }
                                else
                                {
                                    DeviceValue dv = selectedDevice.Values.FirstOrDefault(v => v.UniqueIdentifier == d_cmd.CustomData2);
                                    if (dv != null)
                                    {
                                        DefaultValue = dv.Value;
                                    }
                                }
                                SelectedDeviceArg = DefaultValue;
                                NumericControl control = new NumericControl(d_cmd.Name,
                                    d_cmd.Description,
                                    DefaultValue,
                                    NumericControl.NumberType.Byte,
                                    (value) =>
                                    {
                                        SelectedDeviceArg = value;
                                    }, icon);
                                DeviceArgSckPnl.Children.Add(control);

                                break;
                            }
                        case DataType.SHORT:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                if (!string.IsNullOrEmpty(StoredCommand.Argument))
                                {
                                    DefaultValue = StoredCommand.Argument;
                                }
                                else
                                {
                                    DeviceValue dv = selectedDevice.Values.FirstOrDefault(v => v.UniqueIdentifier == d_cmd.CustomData2);
                                    if (dv != null)
                                    {
                                        DefaultValue = dv.Value;
                                    }
                                }
                                SelectedDeviceArg = DefaultValue;
                                NumericControl control = new NumericControl(d_cmd.Name,
                                    d_cmd.Description,
                                    DefaultValue,
                                    NumericControl.NumberType.Short,
                                    (value) =>
                                    {
                                        SelectedDeviceArg = value;
                                    }, icon);
                                DeviceArgSckPnl.Children.Add(control);

                                break;
                            }

                        case DataType.STRING:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                if (!string.IsNullOrEmpty(StoredCommand.Argument))
                                {
                                    DefaultValue = StoredCommand.Argument;
                                }
                                else
                                {
                                    DeviceValue dv = selectedDevice.Values.FirstOrDefault(v => v.UniqueIdentifier == d_cmd.CustomData2);
                                    if (dv != null)
                                    {
                                        DefaultValue = dv.Value;
                                    }
                                }
                                SelectedDeviceArg = DefaultValue;
                                StringControl control = new StringControl(d_cmd.Name,
                                    d_cmd.Description,
                                    DefaultValue,
                                    (value) =>
                                    {
                                        SelectedDeviceArg = value;
                                    }, icon);
                                DeviceArgSckPnl.Children.Add(control);

                                break;
                            }
                        case DataType.LIST:
                            {
                                //get the current value from the value table list
                                string DefaultValue = "0";
                                if (!string.IsNullOrEmpty(StoredCommand.Argument))
                                {
                                    DefaultValue = StoredCommand.Argument;
                                }
                                else
                                {
                                    DeviceValue dv = selectedDevice.Values.FirstOrDefault(v => v.UniqueIdentifier == d_cmd.CustomData2);
                                    if (dv != null)
                                    {
                                        DefaultValue = dv.Value;
                                    }
                                }
                                SelectedDeviceArg = DefaultValue;
                                ComboboxControl control = new ComboboxControl(d_cmd.Name,
                                    d_cmd.Description,
                                    d_cmd.Options.Select(o => o.Name).ToList(),
                                    DefaultValue,
                                    (value) =>
                                    {
                                        SelectedDeviceArg = value;
                                    }, icon);
                                DeviceArgSckPnl.Children.Add(control);

                                break;
                            }
                    }
                    #endregion
                }
            }
        }
    }
}
