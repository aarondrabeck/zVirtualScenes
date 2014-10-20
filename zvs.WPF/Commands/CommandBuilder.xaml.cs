using System;
using System.Collections.Generic;
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
    public partial class CommandBuilder : Window
    {
        private ZvsContext Context;
        private StoredCommand StoredCommand;
        private BitmapImage icon = new BitmapImage(new Uri("pack://application:,,,/zVirtualScenes;component/Images/save_check.png"));

        //temp variable to save selected built-in command data
        private string SelectedBuiltinArg = string.Empty;

        //temp variable to save selected device command data
        private string SelectedDeviceArg = string.Empty;

        public CommandBuilder(ZvsContext context, StoredCommand storedCommand)
        {
            Context = context;
            StoredCommand = storedCommand;
            InitializeComponent();
        }

        private void Close_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
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
            if (StoredCommand.Command is DeviceCommand)
            {
                DeviceTab.IsSelected = true;

                DeviceCommand dc = (DeviceCommand)StoredCommand.Command;
                DevicesCmboBox.SelectedItem = dc.Device;

                //Preselect device command
                var cmd = await Context.DeviceCommands.FirstOrDefaultAsync(o => o.Id == StoredCommand.Command.Id);
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
                DeviceTypeCommand cmd = await Context.DeviceTypeCommands.FirstOrDefaultAsync(o => o.Id == StoredCommand.Command.Id);
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

        private async Task InitializeJavaScriptCommandsAsync()
        {
            // Do not load your data at design time.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                System.Windows.Data.CollectionViewSource CmdsViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("JSCmdsViewSource")));
                await Context.JavaScriptCommands.Include(o => o.Options).ToListAsync();
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

        private async void BuiltinCmdsCmboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
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
                        int deviceID = int.TryParse(StoredCommand.Argument, out deviceID) ? deviceID : 0;
                        var device = await Context.Devices.FirstOrDefaultAsync(o => o.Id == deviceID);

                        if (device == null)
                        {
                            //If this is a new command or we cannot find the old device, just preselect the first device.
                            device = await Context.Devices.FirstOrDefaultAsync();
                        }

                        if (device != null)
                        {
                            default_value = device.Name;
                            SelectedBuiltinArg = device.Id.ToString();
                        }

                        ComboboxControl control = new ComboboxControl(selected_cmd.Name,
                            selected_cmd.Description,
                            await Context.Devices.Select(o => o.Name).ToListAsync(),
                            default_value,
                            async (value) =>
                            {
                                var d = await Context.Devices.FirstOrDefaultAsync(o => o.Name == value);
                                if (d != null)
                                    SelectedBuiltinArg = d.Id.ToString();
                            }, icon);
                        BuiltinArgSckPnl.Children.Add(control);
                        break;
                    }
                case "GROUP_ON":
                case "GROUP_OFF":
                    {
                        string default_value = string.Empty;

                        //Lookup the group involved in the command
                        int groupID = int.TryParse(StoredCommand.Argument, out groupID) ? groupID : 0;
                        var group = await Context.Groups.FirstOrDefaultAsync(o => o.Id == groupID);

                        if (group == null)
                            group = await Context.Groups.FirstOrDefaultAsync(); //If this is a new command or we cannot find the old group, just preselect the first group.

                        if (group != null)
                        {
                            default_value = group.Name;
                            SelectedBuiltinArg = group.Id.ToString();
                        }

                        ComboboxControl control = new ComboboxControl(selected_cmd.Name,
                            selected_cmd.Description,
                            await Context.Groups.Select(o => o.Name).ToListAsync(),
                            default_value,
                            async (value) =>
                            {
                                Group g = await Context.Groups.FirstOrDefaultAsync(o => o.Name == value);
                                if (g != null)
                                    SelectedBuiltinArg = g.Id.ToString();
                            }, icon);
                        BuiltinArgSckPnl.Children.Add(control);
                    }
                    break;
                case "RUN_SCENE":
                    {
                        string default_value = string.Empty;

                        //Try to match supplied arg (sceneID) with a scene
                        int sceneID = int.TryParse(StoredCommand.Argument, out sceneID) ? sceneID : 0;
                        var scene = await Context.Scenes.FirstOrDefaultAsync(o => o.Id == sceneID);

                        //If this is a new command or we cannot find the old scene, 
                        //just preselect the first scene.
                        if (scene == null)
                            scene = await Context.Scenes.FirstOrDefaultAsync();

                        if (scene != null)
                        {
                            default_value = scene.Name;
                            SelectedBuiltinArg = scene.Id.ToString();
                        }

                        ComboboxControl control = new ComboboxControl(selected_cmd.Name,
                            selected_cmd.Description,
                            await Context.Scenes.Select(o => o.Name).ToListAsync(),
                            default_value,
                            async (value) =>
                            {
                                var s = await Context.Scenes.FirstOrDefaultAsync(o => o.Name == value);
                                if (s != null)
                                    SelectedBuiltinArg = s.Id.ToString();
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
                                    SelectedBuiltinArg = string.Empty;
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
                                    bool.TryParse(StoredCommand.Argument, out DefaultValue);
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
                                    if (!string.IsNullOrEmpty(StoredCommand.Argument))
                                        DefaultValue = StoredCommand.Argument;
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
                                    if (!string.IsNullOrEmpty(StoredCommand.Argument))
                                        DefaultValue = StoredCommand.Argument;
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
                                    if (!string.IsNullOrEmpty(StoredCommand.Argument))
                                        DefaultValue = StoredCommand.Argument;
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
                                    if (!string.IsNullOrEmpty(StoredCommand.Argument))
                                        DefaultValue = StoredCommand.Argument;
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
                                    if (!string.IsNullOrEmpty(StoredCommand.Argument))
                                        DefaultValue = StoredCommand.Argument;
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

                                    if (!string.IsNullOrEmpty(StoredCommand.Argument))
                                        DefaultValue = StoredCommand.Argument;
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
                if (DeviceCmdsCmboBox.SelectedItem is DeviceCommand)
                {
                    StoredCommand.Command = (DeviceCommand)DeviceCmdsCmboBox.SelectedItem;
                    StoredCommand.Argument = SelectedDeviceArg;
                }
                if (DeviceCmdsCmboBox.SelectedItem is DeviceTypeCommand)
                {
                    StoredCommand.Command = (DeviceTypeCommand)DeviceCmdsCmboBox.SelectedItem;
                    StoredCommand.Argument = SelectedDeviceArg;
                    StoredCommand.Argument2 = ((Device)DevicesCmboBox.SelectedItem).Id.ToString();
                }

                await StoredCommand.SetTargetObjectNameAsync(Context);
                StoredCommand.SetDescription(Context);

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

                await StoredCommand.SetTargetObjectNameAsync(Context);
                StoredCommand.SetDescription(Context);

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

                await StoredCommand.SetTargetObjectNameAsync(Context);
                StoredCommand.SetDescription(Context);

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

        private async void DeviceCmdsCmboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
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
                                    var dv = await Context.DeviceValues.FirstOrDefaultAsync(o => o.DeviceId == selectedDevice.Id && o.UniqueIdentifier == d_cmd.CustomData2);
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
                                decimal DefaultValue = 0;
                                if (!decimal.TryParse(StoredCommand.Argument, out DefaultValue))
                                {
                                    var dv = await Context.DeviceValues.FirstOrDefaultAsync(o => o.DeviceId == selectedDevice.Id && o.UniqueIdentifier == d_cmd.CustomData2);
                                    if (dv != null)
                                        decimal.TryParse(dv.Value, out DefaultValue);
                                }
                                SelectedDeviceArg = DefaultValue.ToString();
                                NumericControl control = new NumericControl(d_cmd.Name,
                                    d_cmd.Description,
                                    DefaultValue.ToString(),
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
                                int DefaultValue = 0;
                                if (!int.TryParse(StoredCommand.Argument, out DefaultValue))
                                {
                                    var dv = await Context.DeviceValues.FirstOrDefaultAsync(o => o.DeviceId == selectedDevice.Id && o.UniqueIdentifier == d_cmd.CustomData2);
                                    if (dv != null)
                                        int.TryParse(dv.Value, out DefaultValue);
                                }
                                SelectedDeviceArg = DefaultValue.ToString();
                                NumericControl control = new NumericControl(d_cmd.Name,
                                    d_cmd.Description,
                                    DefaultValue.ToString(),
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
                                short DefaultValue = 0;
                                if (!short.TryParse(StoredCommand.Argument, out DefaultValue))
                                {
                                    var dv = await Context.DeviceValues.FirstOrDefaultAsync(o => o.DeviceId == selectedDevice.Id && o.UniqueIdentifier == d_cmd.CustomData2);
                                    if (dv != null)
                                        short.TryParse(dv.Value, out DefaultValue);
                                }
                                SelectedDeviceArg = DefaultValue.ToString();
                                NumericControl control = new NumericControl(d_cmd.Name,
                                    d_cmd.Description,
                                    DefaultValue.ToString(),
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
                                byte DefaultValue = 0;
                                if (!byte.TryParse(StoredCommand.Argument, out DefaultValue))
                                {
                                    var dv = await Context.DeviceValues.FirstOrDefaultAsync(o => o.DeviceId == selectedDevice.Id && o.UniqueIdentifier == d_cmd.CustomData2);
                                    if (dv != null)
                                        byte.TryParse(dv.Value, out DefaultValue);
                                }
                                SelectedDeviceArg = DefaultValue.ToString();
                                NumericControl control = new NumericControl(d_cmd.Name,
                                    d_cmd.Description,
                                    DefaultValue.ToString(),
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
                                string DefaultValue = "0";
                                if (!string.IsNullOrEmpty(StoredCommand.Argument))
                                {
                                    DefaultValue = StoredCommand.Argument;
                                }
                                else
                                {
                                    var dv = await Context.DeviceValues.FirstOrDefaultAsync(o => o.DeviceId == selectedDevice.Id && o.UniqueIdentifier == d_cmd.CustomData2);
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
                                    var dv = await Context.DeviceValues.FirstOrDefaultAsync(o => o.DeviceId == selectedDevice.Id && o.UniqueIdentifier == d_cmd.CustomData2);
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
                                    var dv = await Context.DeviceValues.FirstOrDefaultAsync(o => o.DeviceId == selectedDevice.Id && o.UniqueIdentifier == d_cmd.CustomData2);
                                    if (dv != null)
                                        bool.TryParse(dv.Value, out DefaultValue);
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
                                decimal DefaultValue = 0;
                                if (!decimal.TryParse(StoredCommand.Argument, out DefaultValue))
                                {
                                    var dv = await Context.DeviceValues.FirstOrDefaultAsync(o => o.DeviceId == selectedDevice.Id && o.UniqueIdentifier == d_cmd.CustomData2);
                                    if (dv != null)
                                        decimal.TryParse(dv.Value, out DefaultValue);
                                }
                                SelectedDeviceArg = DefaultValue.ToString();

                                NumericControl control = new NumericControl(d_cmd.Name,
                                    d_cmd.Description,
                                    DefaultValue.ToString(),
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
                                int DefaultValue = 0;
                                if (!int.TryParse(StoredCommand.Argument, out DefaultValue))
                                {
                                    var dv = await Context.DeviceValues.FirstOrDefaultAsync(o => o.DeviceId == selectedDevice.Id && o.UniqueIdentifier == d_cmd.CustomData2);
                                    if (dv != null)
                                        int.TryParse(dv.Value, out DefaultValue);
                                }
                                SelectedDeviceArg = DefaultValue.ToString();
                                NumericControl control = new NumericControl(d_cmd.Name,
                                    d_cmd.Description,
                                    DefaultValue.ToString(),
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
                                byte DefaultValue = 0;
                                if (!byte.TryParse(StoredCommand.Argument, out DefaultValue))
                                {
                                    var dv = await Context.DeviceValues.FirstOrDefaultAsync(o => o.DeviceId == selectedDevice.Id && o.UniqueIdentifier == d_cmd.CustomData2);
                                    if (dv != null)
                                        byte.TryParse(dv.Value, out DefaultValue);
                                }
                                SelectedDeviceArg = DefaultValue.ToString();
                                NumericControl control = new NumericControl(d_cmd.Name,
                                    d_cmd.Description,
                                    DefaultValue.ToString(),
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
                                short DefaultValue = 0;
                                if (!short.TryParse(StoredCommand.Argument, out DefaultValue))
                                {
                                    var dv = await Context.DeviceValues.FirstOrDefaultAsync(o => o.DeviceId == selectedDevice.Id && o.UniqueIdentifier == d_cmd.CustomData2);
                                    if (dv != null)
                                        short.TryParse(dv.Value, out DefaultValue);
                                }
                                SelectedDeviceArg = DefaultValue.ToString();
                                NumericControl control = new NumericControl(d_cmd.Name,
                                    d_cmd.Description,
                                    DefaultValue.ToString(),
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
                                    var dv = await Context.DeviceValues.FirstOrDefaultAsync(o => o.DeviceId == selectedDevice.Id && o.UniqueIdentifier == d_cmd.CustomData2);
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
                                    var dv = await Context.DeviceValues.FirstOrDefaultAsync(o => o.DeviceId == selectedDevice.Id && o.UniqueIdentifier == d_cmd.CustomData2);
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
