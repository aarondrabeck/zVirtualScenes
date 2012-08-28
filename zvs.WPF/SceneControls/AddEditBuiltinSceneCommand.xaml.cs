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
    public partial class AddEditBuiltinSceneCommand : Window
    {
        private zvsContext context;
        private SceneCommand scene_command;
        private BitmapImage icon = new BitmapImage(new Uri("pack://application:,,,/zVirtualScenes;component/Images/save_check.png"));
        private string arg = string.Empty;

        public AddEditBuiltinSceneCommand(zvsContext context, SceneCommand scene_command)
        {
            this.context = context;
            this.scene_command = scene_command;

            InitializeComponent();
        }

        ~AddEditBuiltinSceneCommand()
        {
            Debug.WriteLine("AddEditBuiltinSceneCommand Deconstructed.");
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            //Make sure we got passed a built-in command...
            if (scene_command.Command is BuiltinCommand)
            {
                this.Close();
            }
            else
            {
                context.BuiltinCommands.ToList();
                CmdsCmboBox.ItemsSource = context.BuiltinCommands.Local;

                //select existing command if there was one
                BuiltinCommand cmd = (BuiltinCommand)scene_command.Command;
                if (cmd != null)
                    CmdsCmboBox.SelectedItem = cmd;
                else
                {
                    if (CmdsCmboBox.Items.Count > 0)
                        CmdsCmboBox.SelectedIndex = 0;
                }
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

            if (CmdsCmboBox.SelectedItem is BuiltinCommand)
            {
                BuiltinCommand d_cmd = (BuiltinCommand)CmdsCmboBox.SelectedItem;
                scene_command.Command = d_cmd;
            }

            scene_command.Argument = arg;

            this.DialogResult = true;
            this.Close();
        }

        private void CmdsCmboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ArgSckPnl.Children.Clear();
            BuiltinCommand selected_cmd = (BuiltinCommand)CmdsCmboBox.SelectedItem;

            switch (selected_cmd.UniqueIdentifier)
            {
                #region Do Custom things for some Builtin Commands
                case "REPOLL_ME":
                    {
                        string default_value = string.Empty;

                        //Lookup the device involved in the command
                        int deviceID = 0;
                        if (int.TryParse(scene_command.Argument, out deviceID))
                        {
                            Device d = context.Devices.FirstOrDefault(o => o.DeviceId == deviceID);
                            if (d != null)
                            {
                                default_value = d.Name;
                                arg = d.DeviceId.ToString();
                            }
                        }

                        //If this is a new command or we cannot find the old device, just preselect the first device.
                        if (string.IsNullOrEmpty(default_value))
                        {
                            Device d = context.Devices.FirstOrDefault();
                            if (d != null)
                            {
                                default_value = d.Name;
                                arg = d.DeviceId.ToString();
                            }
                        }

                        ComboboxControl control = new ComboboxControl(selected_cmd.Name,
                            selected_cmd.Description,
                            context.Devices.Select(o => o.Name).ToList(),
                            default_value,
                            (value) =>
                            {
                                Device d = context.Devices.FirstOrDefault(o => o.Name == value);
                                if (d != null)
                                    arg = d.DeviceId.ToString();
                            }, icon);
                        ArgSckPnl.Children.Add(control);
                        break;
                    }
                case "GROUP_ON":
                case "GROUP_OFF":
                    {
                        string default_value = string.Empty;

                        //Lookup the group involved in the command
                        int groupID = 0;
                        if (int.TryParse(scene_command.Argument, out groupID))
                        {
                            Group g = context.Groups.FirstOrDefault(o => o.GroupId == groupID);
                            if (g != null)
                            {
                                default_value = g.Name;
                                arg = g.GroupId.ToString();
                            }
                        }

                        //If this is a new command or we cannot find the old group, just preselect the first group.
                        if (string.IsNullOrEmpty(default_value))
                        {
                            Group g = context.Groups.FirstOrDefault();
                            if (g != null)
                            {
                                default_value = g.Name;
                                arg = g.GroupId.ToString();
                            }
                        }

                        ComboboxControl control = new ComboboxControl(selected_cmd.Name,
                            selected_cmd.Description,
                            context.Groups.Select(o => o.Name).ToList(),
                            default_value,
                            (value) =>
                            {
                                Group g = context.Groups.FirstOrDefault(o => o.Name == value);
                                if (g != null)
                                    arg = g.GroupId.ToString();
                            }, icon);
                        ArgSckPnl.Children.Add(control);
                    }
                    break;
                #endregion
                default:
                    {
                        #region Built-in Commands
                        switch (selected_cmd.ArgumentType)
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
                                    bool.TryParse(scene_command.Argument, out DefaultValue);
                                    arg = DefaultValue.ToString();

                                    CheckboxControl control = new CheckboxControl(selected_cmd.Name,
                                        selected_cmd.Description,
                                        DefaultValue, (isChecked) =>
                                        {
                                            arg = isChecked.ToString();
                                        }, icon);
                                    ArgSckPnl.Children.Add(control);

                                    break;
                                }
                            case DataType.DECIMAL:
                                {
                                    string DefaultValue = "0";
                                    if (!string.IsNullOrEmpty(scene_command.Argument))
                                        DefaultValue = scene_command.Argument;
                                    arg = DefaultValue;

                                    NumericControl control = new NumericControl(selected_cmd.Name,
                                        selected_cmd.Description,
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
                                    string DefaultValue = "0";
                                    if (!string.IsNullOrEmpty(scene_command.Argument))
                                        DefaultValue = scene_command.Argument;
                                    arg = DefaultValue;

                                    NumericControl control = new NumericControl(selected_cmd.Name,
                                        selected_cmd.Description,
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
                                    string DefaultValue = "0";
                                    if (!string.IsNullOrEmpty(scene_command.Argument))
                                        DefaultValue = scene_command.Argument;
                                    arg = DefaultValue;

                                    NumericControl control = new NumericControl(selected_cmd.Name,
                                        selected_cmd.Description,
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
                                    string DefaultValue = "0";
                                    if (!string.IsNullOrEmpty(scene_command.Argument))
                                        DefaultValue = scene_command.Argument;
                                    arg = DefaultValue;

                                    NumericControl control = new NumericControl(selected_cmd.Name,
                                        selected_cmd.Description,
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
                                        DefaultValue = scene_command.Argument;
                                    arg = DefaultValue;

                                    StringControl control = new StringControl(selected_cmd.Name,
                                        selected_cmd.Description,
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
                                    string DefaultValue = "";
                                    string option = selected_cmd.Options.FirstOrDefault().Name;

                                    if (option != null)
                                        DefaultValue = option;

                                    if (!string.IsNullOrEmpty(scene_command.Argument))
                                        DefaultValue = scene_command.Argument;
                                    arg = DefaultValue;

                                    ComboboxControl control = new ComboboxControl(selected_cmd.Name,
                                        selected_cmd.Description,
                                        selected_cmd.Options.Select(o => o.Name).ToList(),
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
                        break;
                    }
            }
        }
    }
}
