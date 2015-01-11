using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using zvs.DataModel;
using zvs.WPF.DynamicActionControls;
using System.Data.Entity;

namespace zvs.WPF.Commands
{
    /// <summary>
    /// Interaction logic for CommandBuilder.xaml
    /// </summary>
    public partial class CommandBuilder
    {
        private ZvsContext Context { get; set; }
        private IStoredCommand StoredCommand { get; set; }
        private readonly BitmapImage _icon = new BitmapImage(new Uri("pack://application:,,,/zVirtualScenes;component/Images/save_check.png"));

        //temp variable to save selected built-in command data
        private string _selectedBuiltinArg = string.Empty;

        //temp variable to save selected device command data
        private string _selectedDeviceArg = string.Empty;

        public CommandBuilder(ZvsContext context, IStoredCommand storedCommand)
        {
            Context = context;
            StoredCommand = storedCommand;
            InitializeComponent();
        }

        private void Close_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            await InitializeDeviceCommandsAsync();
            await InitializeBuiltinCommandsAsync();
            await InitializeJavaScriptCommandsAsync();
        }

        private async Task InitializeDeviceCommandsAsync()
        {
            //Fill the device combo box from db
            await Context.Devices.ToListAsync();

            DevicesCmboBox.ItemsSource = Context.Devices.Local.OrderBy(o => o.Name);
            if (DevicesCmboBox.Items.Count > 0)
                DevicesCmboBox.SelectedIndex = 0;

            //If we are editing, ie. we get passed a StoredCommand with data, 
            //preselect the correct tab and item.
            var command = StoredCommand.Command as DeviceCommand;
            if (command != null)
            {
                DeviceTab.IsSelected = true;

                var dc = command;
                DevicesCmboBox.SelectedItem = dc.Device;

                //Preselect device command
                var cmd = await Context.DeviceCommands.FirstOrDefaultAsync(o => o.Id == command.Id);
                if (cmd != null)
                    DeviceCmdsCmboBox.SelectedItem = cmd;
            }
            else if (StoredCommand.Command is DeviceTypeCommand)
            {
                DeviceTab.IsSelected = true;

                int dId = int.TryParse(StoredCommand.Argument2, out dId) ? dId : 0;
                var d = await Context.Devices.FirstOrDefaultAsync(o => o.Id == dId);
                if (d != null)
                    DevicesCmboBox.SelectedItem = d;

                //Preselect device type command
                var cmd = await Context.DeviceTypeCommands.FirstOrDefaultAsync(o => o.Id == StoredCommand.Command.Id);
                if (cmd != null)
                    DeviceCmdsCmboBox.SelectedItem = cmd;
            }
        }

        private async Task InitializeBuiltinCommandsAsync()
        {
            //Fill the Built-in command box with available command from the DB
            await Context.BuiltinCommands.ToListAsync();

            BuiltinCmdsCmboBox.ItemsSource = Context.BuiltinCommands.Local;
            if (BuiltinCmdsCmboBox.Items.Count > 0)
                BuiltinCmdsCmboBox.SelectedIndex = 0;

            //If we are editing, ie. we get passed a StoredCommand with data, 
            //preselect the correct tab and item.
            var command = StoredCommand.Command as BuiltinCommand;
            if (command != null)
            {
                BuiltinTab.IsSelected = true;
                var cmd = command;
                //set builtinArg so the UI can fill to the preselected arg
                _selectedBuiltinArg = StoredCommand.Argument;
                //preselect the built-in command
                BuiltinCmdsCmboBox.SelectedItem = cmd;
            }
        }

        private async Task InitializeJavaScriptCommandsAsync()
        {
            // Do not load your data at design time.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                var cmdsViewSource = ((System.Windows.Data.CollectionViewSource)(FindResource("JSCmdsViewSource")));
                await Context.JavaScriptCommands.Include(o => o.Options).ToListAsync();
                cmdsViewSource.Source = Context.JavaScriptCommands.Local;
            }

            //If we are editing, ie. we get passed a StoredCommand with data, 
            //preselect the correct tab and item.
            var command = StoredCommand.Command as JavaScriptCommand;
            if (command != null)
            {
                JavaScriptTab.IsSelected = true;
                var cmd = command;
                JavaScriptCmboBox.SelectedItem = cmd;
            }
        }

        private async void BuiltinCmdsCmboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            BuiltinArgSckPnl.Children.Clear();
            var selectedCmd = (BuiltinCommand)BuiltinCmdsCmboBox.SelectedItem;

            switch (selectedCmd.UniqueIdentifier)
            {
                #region Do Custom things for some Builtin Commands
                case "REPOLL_ME":
                    {
                        Device selectedDevice = null;

                        //Lookup the device involved in the command
                        int deviceId = int.TryParse(StoredCommand.Argument, out deviceId) ? deviceId : 0;
                        var device = await Context.Devices.FirstOrDefaultAsync(o => o.Id == deviceId) ??
                                     await Context.Devices.FirstOrDefaultAsync();

                        if (device != null)
                        {
                            selectedDevice = device;
                            _selectedBuiltinArg = device.Id.ToString(CultureInfo.InvariantCulture);
                        }
                        var control = new ComboboxControl(obj =>
                            {
                                var d = obj as Device;
                                if (d != null)
                                    _selectedBuiltinArg = d.Id.ToString(CultureInfo.InvariantCulture);

                                return Task.FromResult(0);
                            },
                            _icon,
                            await Context.Devices.ToListAsync())
                        {
                            Header = selectedCmd.Name,
                            Description = selectedCmd.Description,
                            SelectedItem = selectedDevice
                        };
                        BuiltinArgSckPnl.Children.Add(control);
                        break;
                    }
                case "GROUP_ON":
                case "GROUP_OFF":
                    {
                        Group selectedGroup = null;

                        //Lookup the group involved in the command
                        int groupId = int.TryParse(StoredCommand.Argument, out groupId) ? groupId : 0;
                        var group = await Context.Groups.FirstOrDefaultAsync(o => o.Id == groupId) ??
                                    await Context.Groups.FirstOrDefaultAsync();

                        if (group != null)
                        {
                            selectedGroup = group;
                            _selectedBuiltinArg = group.Id.ToString(CultureInfo.InvariantCulture);
                        }

                        var control = new ComboboxControl(arg =>
                             {
                                 var g = arg as Group;
                                 if (g != null)
                                     _selectedBuiltinArg = g.Id.ToString(CultureInfo.InvariantCulture);
                                 
                                 return Task.FromResult(0);

                             },
                                 _icon,
                            await Context.Groups.ToListAsync())
                        {
                            Header = selectedCmd.Name,
                            Description = selectedCmd.Description,
                            SelectedItem = selectedGroup,
                        };
                        control.ComboBox.DisplayMemberPath = "Name";
                        BuiltinArgSckPnl.Children.Add(control);
                    }
                    break;
                case "RUN_SCENE":
                    {
                        Scene selectedScene = null;

                        //Try to match supplied arg (sceneID) with a scene
                        int sceneId = int.TryParse(StoredCommand.Argument, out sceneId) ? sceneId : 0;
                        var scene = await Context.Scenes.FirstOrDefaultAsync(o => o.Id == sceneId) ??
                                    await Context.Scenes.FirstOrDefaultAsync();

                        //If this is a new command or we cannot find the old scene, 
                        //just preselect the first scene.

                        if (scene != null)
                        {
                            selectedScene = scene;
                            _selectedBuiltinArg = scene.Id.ToString(CultureInfo.InvariantCulture);
                        }

                        var control = new ComboboxControl(arg =>
                            {
                                var s = arg as Scene;
                                if (s != null)
                                    _selectedBuiltinArg = s.Id.ToString(CultureInfo.InvariantCulture);

                                return Task.FromResult(0);

                            },
                                 _icon,
                             await Context.Scenes.ToListAsync())
                        {
                            Header = selectedCmd.Name,
                            Description = selectedCmd.Description,
                            SelectedItem = selectedScene
                        };
                        BuiltinArgSckPnl.Children.Add(control);
                        break;
                    }
                #endregion
                default:
                    {
                        #region Built-in Commands
                        switch (selectedCmd.ArgumentType)
                        {
                            case DataType.NONE:
                                {
                                    _selectedBuiltinArg = string.Empty;
                                    BuiltinArgSckPnl.Children.Add(new TextBlock
                                    {
                                        Text = "None",
                                        Margin = new Thickness(30, 0, 0, 0)
                                    });
                                    break;
                                }
                            case DataType.BOOL:
                                {
                                    //get the current value from the value table list
                                    bool defaultValue;
                                    bool.TryParse(StoredCommand.Argument, out defaultValue);
                                    _selectedBuiltinArg = defaultValue.ToString();

                                    var control = new CheckboxControl(isChecked =>
                                        {
                                            _selectedBuiltinArg = isChecked.ToString();
                                            return Task.FromResult(0);
                                        },
                                        _icon)
                                    {
                                        Header = selectedCmd.Name,
                                        Description = selectedCmd.Description,
                                        Value = defaultValue
                                    };
                                    BuiltinArgSckPnl.Children.Add(control);

                                    break;
                                }
                            case DataType.INTEGER:
                            case DataType.DECIMAL:
                            case DataType.BYTE:
                            case DataType.SHORT:
                                {
                                    var defaultValue = "0";
                                    if (!string.IsNullOrEmpty(StoredCommand.Argument))
                                        defaultValue = StoredCommand.Argument;
                                    _selectedBuiltinArg = defaultValue;

                                    var control = new NumericControl(value =>
                                        {
                                            _selectedBuiltinArg = value;
                                            return Task.FromResult(0);
                                        },
                                        _icon, selectedCmd.ArgumentType)
                                    {
                                        Header = selectedCmd.Name,
                                        Description = selectedCmd.Description,
                                        Value = defaultValue
                                    };
                                    BuiltinArgSckPnl.Children.Add(control);
                                    break;
                                }
                            case DataType.STRING:
                                {
                                    //get the current value from the value table list
                                    var defaultValue = "0";
                                    if (!string.IsNullOrEmpty(StoredCommand.Argument))
                                        defaultValue = StoredCommand.Argument;
                                    _selectedBuiltinArg = defaultValue;

                                    var control = new StringControl(value =>
                                        {
                                            _selectedBuiltinArg = value;
                                            return Task.FromResult(0);
                                        },
                                        _icon)
                                    {
                                        Header = selectedCmd.Name,
                                        Description = selectedCmd.Description,
                                        Value = defaultValue
                                    };
                                    BuiltinArgSckPnl.Children.Add(control);

                                    break;
                                }
                            case DataType.LIST:
                                {
                                    var defaultValue = "";
                                    var firstOrDefault = selectedCmd.Options.FirstOrDefault();
                                    if (firstOrDefault != null)
                                    {
                                        var option = firstOrDefault.Name;

                                        if (option != null)
                                            defaultValue = option;
                                    }

                                    if (!string.IsNullOrEmpty(StoredCommand.Argument))
                                        defaultValue = StoredCommand.Argument;
                                    _selectedBuiltinArg = defaultValue;

                                    var control = new ComboboxControl(value =>
                                        {
                                            _selectedBuiltinArg = value.ToString();
                                            return Task.FromResult(0);
                                        },
                                 _icon,
                                 selectedCmd.Options.Select(o => o.Name).ToList())
                                    {
                                        Header = selectedCmd.Name,
                                        Description = selectedCmd.Description,
                                        SelectedItem = defaultValue
                                    };
                                    BuiltinArgSckPnl.Children.Add(control);

                                    break;
                                }
                        }
                        #endregion
                        break;
                    }
            }
        }

        private async void OKBtn_Click(object sender, RoutedEventArgs e)
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

                //Set Command and Arg
                var item = DeviceCmdsCmboBox.SelectedItem as DeviceCommand;
                if (item != null)
                {
                    StoredCommand.Command = item;
                    StoredCommand.Argument = _selectedDeviceArg;
                }
                var command = DeviceCmdsCmboBox.SelectedItem as DeviceTypeCommand;
                if (command != null)
                {
                    StoredCommand.Command = command;
                    StoredCommand.Argument = _selectedDeviceArg;
                    StoredCommand.Argument2 = ((Device)DevicesCmboBox.SelectedItem).Id.ToString(CultureInfo.InvariantCulture);
                }

                await StoredCommand.SetTargetObjectNameAsync(Context);
                StoredCommand.SetDescription();

                DialogResult = true;
                Close();
            }

            if (BuiltinTab.IsSelected)
            {
                if (BuiltinCmdsCmboBox.SelectedItem == null)
                {
                    MessageBox.Show("You must select a command", "No Command Selected");
                    return;
                }

                //Set Command and Arg
                var item = BuiltinCmdsCmboBox.SelectedItem as BuiltinCommand;
                if (item != null)
                {
                    StoredCommand.Command = item;
                    StoredCommand.Argument = _selectedBuiltinArg;
                }

                await StoredCommand.SetTargetObjectNameAsync(Context);
                StoredCommand.SetDescription();

                DialogResult = true;
                Close();
            }

            if (JavaScriptTab.IsSelected)
            {
                if (JavaScriptCmboBox.SelectedItem == null)
                {
                    MessageBox.Show("You must select a command", "No Command Selected");
                    return;
                }

                //Set Command and Arg
                var item = JavaScriptCmboBox.SelectedItem as JavaScriptCommand;
                if (item != null)
                    StoredCommand.Command = item;

                await StoredCommand.SetTargetObjectNameAsync(Context);
                StoredCommand.SetDescription();

                DialogResult = true;
                Close();
            }
        }

        private void DevicesCmboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            var selectedDevice = (Device)DevicesCmboBox.SelectedItem;
            if (selectedDevice == null) return;
            var deviceCommands = new List<object>();
            selectedDevice.Type.Commands.ToList().ForEach(deviceCommands.Add);
            selectedDevice.Commands.ToList().ForEach(deviceCommands.Add);

            DeviceCmdsCmboBox.ItemsSource = deviceCommands;
            if (DeviceCmdsCmboBox.Items.Count > 0)
                DeviceCmdsCmboBox.SelectedIndex = 0;
        }

        private async void DeviceCmdsCmboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            var selectedDevice = (Device)DevicesCmboBox.SelectedItem;
            DeviceArgSckPnl.Children.Clear();

            var deviceTypeCommand = DeviceCmdsCmboBox.SelectedItem as DeviceTypeCommand;
            if (deviceTypeCommand != null)
            {
                #region Device Type Commands
                switch (deviceTypeCommand.ArgumentType)
                {
                    case DataType.NONE:
                        {
                            DeviceArgSckPnl.Children.Add(new TextBlock
                            {
                                Text = "None",
                                Margin = new Thickness(30, 0, 0, 0)
                            });
                            break;
                        }
                    case DataType.BOOL:
                        {
                            //get the current value from the value table list
                            bool defaultValue;
                            if (!bool.TryParse(StoredCommand.Argument, out defaultValue))
                            {
                                var dv = await Context.DeviceValues.FirstOrDefaultAsync(o => o.DeviceId == selectedDevice.Id && o.UniqueIdentifier == deviceTypeCommand.CustomData2);
                                if (dv != null)
                                {
                                    bool.TryParse(dv.Value, out defaultValue);
                                }
                            }
                            _selectedDeviceArg = defaultValue.ToString();

                            var control = new CheckboxControl(isChecked =>
                            {
                                _selectedDeviceArg = isChecked.ToString();
                                return Task.FromResult(0);
                            }, _icon)
                            {
                                Header = deviceTypeCommand.Name,
                                Description = deviceTypeCommand.Description,
                                Value = defaultValue
                            };
                            DeviceArgSckPnl.Children.Add(control);

                            break;
                        }
                    case DataType.DECIMAL:
                    case DataType.INTEGER:
                    case DataType.BYTE:
                    case DataType.SHORT:
                        {
                            //get the current value from the value table list
                            var defaultValue = "0";
                            var dv = await Context.DeviceValues.FirstOrDefaultAsync(o => o.DeviceId == selectedDevice.Id && o.UniqueIdentifier == deviceTypeCommand.CustomData2);
                            if (dv != null)
                                defaultValue = dv.Value;
                            _selectedDeviceArg = defaultValue;
                            var control = new NumericControl(value =>
                                {
                                    _selectedDeviceArg = value;
                                    return Task.FromResult(0);
                                },
                                        _icon, deviceTypeCommand.ArgumentType)
                            {
                                Header = deviceTypeCommand.Name,
                                Description = deviceTypeCommand.Description,
                                Value = defaultValue
                            };
                            DeviceArgSckPnl.Children.Add(control);

                            break;
                        }
                    case DataType.STRING:
                        {
                            //get the current value from the value table list
                            var defaultValue = "0";
                            if (!string.IsNullOrEmpty(StoredCommand.Argument))
                            {
                                defaultValue = StoredCommand.Argument;
                            }
                            else
                            {
                                var dv = await Context.DeviceValues.FirstOrDefaultAsync(o => o.DeviceId == selectedDevice.Id && o.UniqueIdentifier == deviceTypeCommand.CustomData2);
                                if (dv != null)
                                {
                                    defaultValue = dv.Value;
                                }
                            }
                            _selectedDeviceArg = defaultValue;
                            var control = new StringControl(
                                value =>
                                {
                                    _selectedDeviceArg = value;
                                    return Task.FromResult(0);
                                },
                                        _icon)
                            {
                                Header = deviceTypeCommand.Name,
                                Description = deviceTypeCommand.Description,
                                Value = defaultValue
                            };
                            DeviceArgSckPnl.Children.Add(control);

                            break;
                        }
                    case DataType.LIST:
                        {
                            //get the current value from the value table list
                            var defaultValue = "0";
                            if (!string.IsNullOrEmpty(StoredCommand.Argument))
                            {
                                defaultValue = StoredCommand.Argument;
                            }
                            else
                            {
                                var dv = await Context.DeviceValues.FirstOrDefaultAsync(o => o.DeviceId == selectedDevice.Id && o.UniqueIdentifier == deviceTypeCommand.CustomData2);
                                if (dv != null)
                                {
                                    defaultValue = dv.Value;
                                }
                            }
                            _selectedDeviceArg = defaultValue;
                            var control = new ComboboxControl(value =>
                                {
                                    _selectedDeviceArg = value.ToString();
                                    return Task.FromResult(0);
                                },
                                _icon,
                                deviceTypeCommand.Options.Select(o => o.Name).ToList())
                            {
                                Header = deviceTypeCommand.Name,
                                Description = deviceTypeCommand.Description,
                                SelectedItem = defaultValue
                            };
                            DeviceArgSckPnl.Children.Add(control);

                            break;
                        }

                }
                #endregion
            }

            var item = DeviceCmdsCmboBox.SelectedItem as DeviceCommand;
            if (item != null)
            {
                var dCmd = item;
                #region Device Commands

                switch (dCmd.ArgumentType)
                {
                    case DataType.NONE:
                        {
                            DeviceArgSckPnl.Children.Add(new TextBlock
                            {
                                Text = "None",
                                Margin = new Thickness(30, 0, 0, 0)
                            });
                            break;
                        }
                    case DataType.BOOL:
                        {
                            //get the current value from the value table list
                            bool defaultValue;
                            if (!bool.TryParse(StoredCommand.Argument, out defaultValue))
                            {
                                var dv = await Context.DeviceValues.FirstOrDefaultAsync(o => o.DeviceId == selectedDevice.Id && o.UniqueIdentifier == dCmd.CustomData2);
                                if (dv != null)
                                    bool.TryParse(dv.Value, out defaultValue);
                            }
                            _selectedDeviceArg = defaultValue.ToString();

                            var control = new CheckboxControl(isChecked =>
                            {
                                _selectedDeviceArg = isChecked.ToString();
                                return Task.FromResult(0);
                            }, _icon)
                            {
                                Header = dCmd.Name,
                                Description = dCmd.Description,
                                Value = defaultValue
                            };
                            DeviceArgSckPnl.Children.Add(control);

                            break;
                        }
                    case DataType.DECIMAL:
                    case DataType.INTEGER:
                    case DataType.BYTE:
                    case DataType.SHORT:
                        {
                            //get the current value from the value table list
                            var defaultValue = StoredCommand.Argument;
                            if (defaultValue == "0")
                            {
                                var dv = await Context.DeviceValues.FirstOrDefaultAsync(o => o.DeviceId == selectedDevice.Id && o.UniqueIdentifier == dCmd.CustomData2);
                                if (dv != null)
                                    defaultValue = dv.Value;
                            }

                            _selectedDeviceArg = defaultValue;

                            var control = new NumericControl(value =>
                            {
                                _selectedDeviceArg = value;
                                return Task.FromResult(0);
                            },
                                        _icon, dCmd.ArgumentType)
                            {
                                Header = dCmd.Name,
                                Description = dCmd.Description,
                                Value = defaultValue
                            };
                            DeviceArgSckPnl.Children.Add(control);
                            break;
                        }
                    case DataType.STRING:
                        {
                            //get the current value from the value table list
                            var defaultValue = "";
                            if (!string.IsNullOrEmpty(StoredCommand.Argument))
                            {
                                defaultValue = StoredCommand.Argument;
                            }
                            else
                            {
                                var dv = await Context.DeviceValues.FirstOrDefaultAsync(o => o.DeviceId == selectedDevice.Id && o.UniqueIdentifier == dCmd.CustomData2);
                                if (dv != null)
                                {
                                    defaultValue = dv.Value;
                                }
                            }
                            _selectedDeviceArg = defaultValue;
                            var control = new StringControl(
                                value =>
                                {
                                    _selectedDeviceArg = value;
                                    return Task.FromResult(0);
                                },
                                        _icon)
                            {
                                Header = dCmd.Name,
                                Description = dCmd.Description,
                                Value = defaultValue
                            };
                            DeviceArgSckPnl.Children.Add(control);

                            break;
                        }
                    case DataType.LIST:
                        {
                            //get the current value from the value table list
                            var defaultValue = "0";
                            if (!string.IsNullOrEmpty(StoredCommand.Argument))
                            {
                                defaultValue = StoredCommand.Argument;
                            }
                            else
                            {
                                var dv = await Context.DeviceValues.FirstOrDefaultAsync(o => o.DeviceId == selectedDevice.Id && o.UniqueIdentifier == dCmd.CustomData2);
                                if (dv != null)
                                {
                                    defaultValue = dv.Value;
                                }
                            }
                            _selectedDeviceArg = defaultValue;
                            var control = new ComboboxControl(value =>
                                {
                                    _selectedDeviceArg = value.ToString();
                                    return Task.FromResult(0);
                                },
                                _icon,
                                dCmd.Options.Select(o => o.Name).ToList())
                            {
                                Header = dCmd.Name,
                                Description = dCmd.Description,
                                SelectedItem = defaultValue
                            };
                            DeviceArgSckPnl.Children.Add(control);

                            break;
                        }
                }
                #endregion
            }
        }
    }
}
