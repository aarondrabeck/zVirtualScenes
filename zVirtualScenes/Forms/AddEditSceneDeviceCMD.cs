using System;
using System.Data;
using System.Windows.Forms;
using zVirtualScenesAPI;
using System.Collections.Generic;
using System.ComponentModel;
using zVirtualScenesCommon.Entity;
using System.Data.Objects;
using zVirtualScenesCommon;
using System.Linq;

namespace zVirtualScenesApplication.Forms
{
    public partial class AddEditSceneDeviceCMD : Form
    {
        private IBindingList _sceneCMDList; 
        private device _device;
        private scene_commands _scene_cmd;

        private CheckBox cb = new CheckBox();
        private NumericUpDown Numeric = new NumericUpDown();
        private TextBox tbx = new TextBox();
        private ComboBox cmbo = new ComboBox();

        /// <summary>
        /// Send list if new
        /// </summary>
        /// <param name="d"></param>
        /// <param name="sceneCD"></param>
        /// <param name="sceneCMDlist"></param>
        public AddEditSceneDeviceCMD(device d, scene_commands sceneCD, IBindingList sceneCMDlist = null)
        {
            _sceneCMDList = sceneCMDlist;
            _device = d;
            _scene_cmd = sceneCD;
            InitializeComponent();
        }

        private void AddEditSceneCMD_Load(object sender, EventArgs e)
        {
            if (_device != null)
            {
                comboBoxDeviceCommand.DataSource = _device.device_commands;
                comboBoxDeviceCommand.DisplayMember = "friendly_name";

                comboBoxTypeCommands.DataSource = _device.device_types.device_type_commands;
                comboBoxTypeCommands.DisplayMember = "friendly_name";

                this.Text = "Scene Command for '" + _device.friendly_name + "'";
            }

            switch((command_types)_scene_cmd.command_type_id)
            {
                case command_types.device_command:
                    {
                        radioBtnDeviceCMD.Checked = true;

                        device_commands dc = _device.device_commands.SingleOrDefault(c => c.id == _scene_cmd.command_id);
                        if (dc != null)
                        {
                            comboBoxDeviceCommand.SelectedItem = dc;
                        }
                        break;
                    }
                case command_types.device_type_command:
                    {
                        radioBtnTypeCommand.Checked = true;

                        device_type_commands dtc = _device.device_types.device_type_commands.SingleOrDefault(c => c.id == _scene_cmd.command_id);
                        if (dtc != null)
                        {
                            comboBoxTypeCommands.SelectedItem = dtc;
                        }
                        break;
                    }
                default:
                    //IE NEW COMMANDS
                    radioBtnTypeCommand.Checked = true;
                    break;
            }            
        }

        private void comboBoxTypeCommands_SelectedIndexChanged(object sender, EventArgs e)
        {
            device_type_commands selected_cmd = (device_type_commands)comboBoxTypeCommands.SelectedItem;

            if (selected_cmd != null)
            {
                AddDynamicInputControl((Data_Types)selected_cmd.arg_data_type,
                                        selected_cmd.friendly_name,
                                        selected_cmd.device_type_command_options.Select(o => o.option).ToList(),
                                        _scene_cmd.arg);
            }
            else
            {
                panelUserInputControls.Controls.Clear();
            }
        }

        private void comboBoxDeviceCommand_SelectedIndexChanged(object sender, EventArgs e)
        {
            device_commands selected_cmd = (device_commands)comboBoxDeviceCommand.SelectedItem;
            if (selected_cmd != null)
            {
                AddDynamicInputControl((Data_Types)selected_cmd.arg_data_type,
                                        selected_cmd.friendly_name,
                                        selected_cmd.device_command_options.Select(o => o.name).ToList(),
                                        _scene_cmd.arg);
            }
            else
            {
                panelUserInputControls.Controls.Clear();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _scene_cmd.device_id = _device.id;            

            //Update the command type, command id, and arg
            if (radioBtnTypeCommand.Checked)
            {
                device_type_commands selected_cmd = (device_type_commands)comboBoxTypeCommands.SelectedItem;
                if (selected_cmd == null)
                {
                    MessageBox.Show("Please select a command!", zvsEntityControl.zvsNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                _scene_cmd.command_id = selected_cmd.id;
                _scene_cmd.command_type_id = (int)command_types.device_type_command;
                string userInput = GetUserInput((Data_Types)selected_cmd.arg_data_type);

                if (userInput == null)
                    return;
                else
                    _scene_cmd.arg = userInput; 
            }
            else
            {
                device_commands selected_cmd = (device_commands)comboBoxDeviceCommand.SelectedItem;
                if (selected_cmd == null)
                {
                    MessageBox.Show("Please select a command!", zvsEntityControl.zvsNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                _scene_cmd.command_id = selected_cmd.id;
                _scene_cmd.command_type_id = (int)command_types.device_command;

                string userInput = GetUserInput((Data_Types)selected_cmd.arg_data_type);

                if (userInput == null)
                    return;
                else
                    _scene_cmd.arg = userInput; 
            }

            if (_sceneCMDList != null)
                _sceneCMDList.Add(_scene_cmd);

            zvsEntityControl.zvsContext.SaveChanges();            

            this.Close();
        }

        private void AddDynamicInputControl(Data_Types d, string FriendlyName, List<string> OptionList, string PrefilledValue = null)
        {
            switch (d)
            {
                case Data_Types.NONE:
                    panelUserInputControls.Controls.Clear();
                    break;
                case Data_Types.BOOL:
                    panelUserInputControls.Controls.Clear();
                    cb.Text = FriendlyName;
                    panelUserInputControls.Controls.Add(cb);

                    if (PrefilledValue != null)
                    {
                        bool bvalue = false;
                        bool.TryParse(PrefilledValue, out bvalue);
                        cb.Checked = bvalue;
                    }

                    break;
                case Data_Types.LIST:
                    panelUserInputControls.Controls.Clear();
                    cmbo.DropDownStyle = ComboBoxStyle.DropDownList;
                    cmbo.DataSource = OptionList;

                    if (cmbo.Items.Count > 0)
                        cmbo.SelectedIndex = 0;

                    if (PrefilledValue != null)
                        cmbo.SelectedItem = OptionList.SingleOrDefault(o=> o.ToString() == PrefilledValue);

                    panelUserInputControls.Controls.Add(cmbo);
                    break;
                case Data_Types.STRING:

                    panelUserInputControls.Controls.Clear();
                    panelUserInputControls.Controls.Add(tbx);
                    if (PrefilledValue != null)
                    {
                        tbx.SelectedText = PrefilledValue;
                    }
                    break;
                case Data_Types.INTEGER:
                    panelUserInputControls.Controls.Clear();
                    Numeric.Maximum = Int64.MaxValue;
                    Numeric.Minimum = Int64.MinValue;
                    panelUserInputControls.Controls.Add(Numeric);

                    if (PrefilledValue != null)
                    {
                        int ivalue = 0;
                        int.TryParse(PrefilledValue, out ivalue);
                        Numeric.Value = ivalue;
                    }
                    break;
                case Data_Types.BYTE:
                    panelUserInputControls.Controls.Clear();
                    Numeric.Maximum = Byte.MaxValue;
                    Numeric.Minimum = Byte.MinValue;
                    panelUserInputControls.Controls.Add(Numeric);

                    if (PrefilledValue != null)
                    {
                        byte bvalue = 0;
                        byte.TryParse(PrefilledValue, out bvalue);
                        Numeric.Value = bvalue;
                    }
                    break;
                case Data_Types.DECIMAL:
                    panelUserInputControls.Controls.Clear();
                    Numeric.Maximum = Decimal.MaxValue;
                    Numeric.Minimum = Decimal.MinValue;
                    panelUserInputControls.Controls.Add(Numeric);

                    if (PrefilledValue != null)
                    {
                        decimal dvalue = 0;
                        decimal.TryParse(PrefilledValue, out dvalue);
                        Numeric.Value = dvalue;
                    }
                    break;
            }
        }

        private string GetUserInput(Data_Types d)
        {
            switch (d)
            {
                case Data_Types.BOOL:
                    {
                        return cb.Checked.ToString();
                    }
                case Data_Types.LIST:
                    if (String.IsNullOrEmpty(cmbo.Text))
                    {
                        MessageBox.Show("Please select a vaule!", zvsEntityControl.zvsNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return null;
                    }
                    return cmbo.Text;                    
                case Data_Types.STRING:
                    if (String.IsNullOrEmpty(tbx.Text))
                    {
                        MessageBox.Show("Please enter a vaule!", zvsEntityControl.zvsNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return null;
                    }
                    return tbx.Text;
                case Data_Types.BYTE:
                case Data_Types.DECIMAL:
                case Data_Types.INTEGER:
                case Data_Types.SHORT:
                    {
                        return Numeric.Value.ToString();
                    }
                default:
                    {
                        return string.Empty;
                    }
                    

            }
        }

        private void radioBtnDeviceCMD_CheckedChanged(object sender, EventArgs e)
        {
            if (radioBtnDeviceCMD.Checked)
            {
                comboBoxDeviceCommand_SelectedIndexChanged(this, new EventArgs());
                comboBoxDeviceCommand.Visible = true;
                comboBoxTypeCommands.Visible = false;
            }
        }

        private void radioBtnTypeCommand_CheckedChanged(object sender, EventArgs e)
        {
            if (radioBtnTypeCommand.Checked)
            {
                comboBoxTypeCommands_SelectedIndexChanged(this, new EventArgs());
                comboBoxDeviceCommand.Visible = false;
                comboBoxTypeCommands.Visible = true;
            }

        }
    }
}
