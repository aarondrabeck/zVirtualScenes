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
    public partial class AddEditBuiltinSceneCommand : Window
    {
        private zvsLocalDBEntities context;
        private scene_commands scene_command;
        private BitmapImage icon = new BitmapImage(new Uri("pack://application:,,,/zVirtualScenes_WPF;component/Images/save_check.png"));
        private string arg = string.Empty;

        public AddEditBuiltinSceneCommand(zvsLocalDBEntities context, scene_commands scene_command)
        {
            this.context = context;
            this.scene_command = scene_command;

            InitializeComponent();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            //Make sure we got passed a built-in command...
            if ((scene_commands.command_types)scene_command.command_type_id != scene_commands.command_types.builtin)
            {
                this.Close();
            }
            else
            {
                context.builtin_commands.ToList();
                CmdsCmboBox.ItemsSource = context.builtin_commands.Local;

                //select existing command if there was one
                builtin_commands cmd = context.builtin_commands.FirstOrDefault(o => o.id == scene_command.command_id);
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

            if (CmdsCmboBox.SelectedItem is builtin_commands)
            {
                builtin_commands d_cmd = (builtin_commands)CmdsCmboBox.SelectedItem;
                scene_command.command_id = d_cmd.id;
            }

            scene_command.arg = arg;

            this.DialogResult = true;
            this.Close();
        }

        private void CmdsCmboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ArgSckPnl.Children.Clear();
            builtin_commands selected_cmd = (builtin_commands)CmdsCmboBox.SelectedItem;


            switch (selected_cmd.name)
            {
                #region Do Custom things for some Builtin Commands
                case "REPOLL_ME":
                    {
                        string default_value = string.Empty;

                        //Lookup the device involved in the command
                        int deviceID = 0;
                        if (int.TryParse(scene_command.arg, out deviceID))
                        {
                            device d = context.devices.FirstOrDefault(o => o.id == deviceID);
                            if (d != null)
                            {
                                default_value = d.friendly_name;
                                arg = d.id.ToString();
                            }
                        }

                        //If this is a new command or we cannot find the old device, just preselect the first device.
                        if (string.IsNullOrEmpty(default_value))
                        {
                            device d = context.devices.FirstOrDefault();
                            if (d != null)
                            {
                                default_value = d.friendly_name;
                                arg = d.id.ToString();
                            }
                        }

                        ComboboxControl control = new ComboboxControl(selected_cmd.friendly_name,
                            selected_cmd.description,
                            context.devices.Select(o => o.friendly_name).ToList(),
                            default_value,
                            (value) =>
                            {
                                device d = context.devices.FirstOrDefault(o => o.friendly_name == value);
                                if (d != null)
                                    arg = d.id.ToString();
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
                        if (int.TryParse(scene_command.arg, out groupID))
                        {
                            group g = context.groups.FirstOrDefault(o => o.id == groupID);
                            if (g != null)
                            {
                                default_value = g.name;
                                arg = g.id.ToString();
                            }
                        }

                        //If this is a new command or we cannot find the old group, just preselect the first group.
                        if (string.IsNullOrEmpty(default_value))
                        {
                            group g = context.groups.FirstOrDefault();
                            if (g != null)
                            {
                                default_value = g.name;
                                arg = g.id.ToString();
                            }
                        }

                        ComboboxControl control = new ComboboxControl(selected_cmd.friendly_name,
                            selected_cmd.description,
                            context.groups.Select(o => o.name).ToList(),
                            default_value,
                            (value) =>
                            {
                                group g = context.groups.FirstOrDefault(o => o.name == value);
                                if (g != null)
                                    arg = g.id.ToString();
                            }, icon);
                        ArgSckPnl.Children.Add(control);
                    }
                    break;
                #endregion
                default:
                    {
                        #region Built-in Commands
                        switch ((Data_Types)selected_cmd.arg_data_type)
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
                                    bool.TryParse(scene_command.arg, out DefaultValue);
                                    arg = DefaultValue.ToString();

                                    CheckboxControl control = new CheckboxControl(selected_cmd.friendly_name,
                                        selected_cmd.description,
                                        DefaultValue, (isChecked) =>
                                        {
                                            arg = isChecked.ToString();
                                        }, icon);
                                    ArgSckPnl.Children.Add(control);

                                    break;
                                }
                            case Data_Types.DECIMAL:
                                {
                                    string DefaultValue = "0";
                                    if (!string.IsNullOrEmpty(scene_command.arg))
                                        DefaultValue = scene_command.arg;
                                    arg = DefaultValue;

                                    NumericControl control = new NumericControl(selected_cmd.friendly_name,
                                        selected_cmd.description,
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
                                    string DefaultValue = "0";
                                    if (!string.IsNullOrEmpty(scene_command.arg))
                                        DefaultValue = scene_command.arg;
                                    arg = DefaultValue;

                                    NumericControl control = new NumericControl(selected_cmd.friendly_name,
                                        selected_cmd.description,
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
                                    string DefaultValue = "0";
                                    if (!string.IsNullOrEmpty(scene_command.arg))
                                        DefaultValue = scene_command.arg;
                                    arg = DefaultValue;

                                    NumericControl control = new NumericControl(selected_cmd.friendly_name,
                                        selected_cmd.description,
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
                                    string DefaultValue = "0";
                                    if (!string.IsNullOrEmpty(scene_command.arg))
                                        DefaultValue = scene_command.arg;
                                    arg = DefaultValue;

                                    NumericControl control = new NumericControl(selected_cmd.friendly_name,
                                        selected_cmd.description,
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
                                        DefaultValue = scene_command.arg;
                                    arg = DefaultValue;

                                    StringControl control = new StringControl(selected_cmd.friendly_name,
                                        selected_cmd.description,
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
                                    string DefaultValue = "";
                                    string option = selected_cmd.builtin_command_options.FirstOrDefault().name;

                                    if (option != null)
                                        DefaultValue = option;

                                    if (!string.IsNullOrEmpty(scene_command.arg))
                                        DefaultValue = scene_command.arg;
                                    arg = DefaultValue;

                                    ComboboxControl control = new ComboboxControl(selected_cmd.friendly_name,
                                        selected_cmd.description,
                                        selected_cmd.builtin_command_options.Select(o => o.name).ToList(),
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
