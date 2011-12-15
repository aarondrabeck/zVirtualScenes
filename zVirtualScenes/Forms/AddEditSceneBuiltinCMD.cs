using System;
using System.Data;
using System.Windows.Forms;
using zVirtualScenesAPI;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;
using zVirtualScenesCommon.Entity;
using zVirtualScenesCommon;
using System.Linq;
using System.Data.Objects;

namespace zVirtualScenesApplication.Forms
{
    public partial class AddEditSceneBuiltinCMD : Form
    {        
        private scene_commands _scene_command;
        private IBindingList _scenecmdList;

        private CheckBox cb = new CheckBox();
        private NumericUpDown Numeric = new NumericUpDown();
        private TextBox tbx = new TextBox();
        private ComboBox cmbo = new ComboBox();

        /// <summary>
        /// If new and not an edit, send scenecmdList and the scene_cmd will be added to the list when save is clicked.
        /// </summary>
        /// <param name="scene_cmd"></param>
        /// <param name="scenecmdList"></param>
        public AddEditSceneBuiltinCMD(scene_commands scene_cmd, IBindingList scenecmdList = null)
        {
            _scenecmdList = scenecmdList;
            _scene_command = scene_cmd;
            InitializeComponent();           
        }
        
        private void AddEditSceneCMD_Load(object sender, EventArgs e)
        {
            ActiveControl = comboBoxCommand;
            comboBoxCommand.Focus();

            comboBoxCommand.DataSource = zvsEntityControl.zvsContext.builtin_commands;
            comboBoxCommand.DisplayMember = "friendly_name";

            //if editing
            builtin_commands b_cmd = zvsEntityControl.zvsContext.builtin_commands.FirstOrDefault(c => c.id == _scene_command.command_id);
            if(b_cmd != null)
                comboBoxCommand.SelectedItem = b_cmd;           
        }     

        private void comboBoxCommand_SelectedIndexChanged(object sender, EventArgs e)
        {
            builtin_commands selected_cmd = (builtin_commands)comboBoxCommand.SelectedItem;

            //Do Custom things for some Builtin Commands
            switch (selected_cmd.name)
            {
                case "REPOLL_ME":
                    panelUserInputControls.Controls.Clear();
                    cmbo.Width = 269;
                    cmbo.DropDownStyle = ComboBoxStyle.DropDownList;

                    cmbo.DataSource = null;                     
                    ObjectQuery<device> deviceListQuery = zvsEntityControl.zvsContext.devices;            
                    cmbo.DataSource = deviceListQuery.Execute(MergeOption.AppendOnly);
                    cmbo.DisplayMember = "friendly_name";
                                        
                    if (cmbo.Items.Count > 0)
                        cmbo.SelectedIndex = 0;

                    if (_scene_command.arg != null)
                    {
                        long d_id = 0;
                        long.TryParse(_scene_command.arg, out d_id);
                        cmbo.SelectedItem = zvsEntityControl.zvsContext.devices.FirstOrDefault(d => d.id == d_id);
                    }

                    panelUserInputControls.Controls.Add(cmbo);
                    break;
                case "GROUP_ON":
                case "GROUP_OFF":
                    panelUserInputControls.Controls.Clear();
                    cmbo.Width = 269;
                    cmbo.DropDownStyle = ComboBoxStyle.DropDownList;

                    cmbo.DataSource = null;
                    ObjectQuery<group> dgroupQuery = zvsEntityControl.zvsContext.groups;
                    cmbo.DataSource = dgroupQuery.Execute(MergeOption.AppendOnly);
                    cmbo.DisplayMember = "name";
                
                    if (cmbo.Items.Count > 0)
                        cmbo.SelectedIndex = 0;

                    if (_scene_command.arg != null)
                    {
                        long g_id = 0;
                        long.TryParse(_scene_command.arg, out g_id);
                        cmbo.SelectedItem = zvsEntityControl.zvsContext.groups.FirstOrDefault(g => g.id == g_id);
                    }

                    panelUserInputControls.Controls.Add(cmbo);
                    break;
                default:
                    switch ((Data_Types)selected_cmd.arg_data_type)
                    {
                        case Data_Types.NONE:
                            panelUserInputControls.Controls.Clear();
                            break;
                        case Data_Types.BOOL:
                            panelUserInputControls.Controls.Clear();
                            cb.Text = selected_cmd.friendly_name;
                            panelUserInputControls.Controls.Add(cb);

                            if (_scene_command.arg != null)
                            {
                                bool bvalue = false;
                                bool.TryParse(_scene_command.arg, out bvalue);
                                cb.Checked = bvalue;
                            }

                            break;
                        case Data_Types.LIST:
                            panelUserInputControls.Controls.Clear();
                            cmbo.Width = 269;
                            cmbo.DropDownStyle = ComboBoxStyle.DropDownList;
                            cmbo.DataSource = selected_cmd.builtin_command_options.Select(o => o.name).ToList();                                
                     
                            if (cmbo.Items.Count > 0)
                                cmbo.SelectedIndex = 0;

                            if (_scene_command.arg != null)
                                    cmbo.SelectedItem = _scene_command.arg ;
                            
                            panelUserInputControls.Controls.Add(cmbo);
                            break;
                        case Data_Types.STRING:
                            panelUserInputControls.Controls.Clear();
                            panelUserInputControls.Controls.Add(tbx);
                            if (_scene_command.arg != null)
                            {
                                tbx.SelectedText = _scene_command.arg;
                            }
                            break;
                        case Data_Types.INTEGER:
                            panelUserInputControls.Controls.Clear();
                            Numeric.Maximum = Int64.MaxValue;
                            Numeric.Minimum = Int64.MinValue;
                            panelUserInputControls.Controls.Add(Numeric);

                            if (_scene_command.arg != null)
                            {
                                int ivalue = 0;
                                int.TryParse(_scene_command.arg, out ivalue);
                                Numeric.Value = ivalue;
                            }
                            break;
                        case Data_Types.BYTE:
                            panelUserInputControls.Controls.Clear();
                            Numeric.Maximum = Byte.MaxValue;
                            Numeric.Minimum = Byte.MinValue;
                            panelUserInputControls.Controls.Add(Numeric);

                            if (_scene_command.arg != null)
                            {
                                byte bvalue = 0;
                                byte.TryParse(_scene_command.arg, out bvalue);
                                Numeric.Value = bvalue;
                            }
                            break;
                        case Data_Types.DECIMAL:
                            panelUserInputControls.Controls.Clear();
                            Numeric.Maximum = Decimal.MaxValue;
                            Numeric.Minimum = Decimal.MinValue;
                            panelUserInputControls.Controls.Add(Numeric);

                            if (_scene_command.arg != null)
                            {
                                decimal dvalue = 0;
                                decimal.TryParse(_scene_command.arg, out dvalue);
                                Numeric.Value = dvalue;
                            }
                            break;
                    }
                    break;
            }  
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            builtin_commands selected_cmd = (builtin_commands)comboBoxCommand.SelectedItem;
                       
            _scene_command.command_type_id = (int)command_types.builtin;
            _scene_command.command_id = selected_cmd.id;

            //Do Custom things for some Builtin Commands
            switch (selected_cmd.name)
            {
                case "REPOLL_ME":
                    {
                        device d = (device)cmbo.SelectedItem;

                        if (d == null)
                        {
                            MessageBox.Show("Please select a deice!", zvsEntityControl.zvsNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                        else
                            _scene_command.arg = d.id.ToString();
                        
                        break;
                    }
                case "GROUP_ON":
                case "GROUP_OFF":
                    {
                        group g = (group)cmbo.SelectedItem;
                        if (g == null)
                        {
                            MessageBox.Show("Please select a group!", zvsEntityControl.zvsNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                        else
                            _scene_command.arg = g.id.ToString();

                        break;
                    }
                default:
                    switch ((Data_Types) selected_cmd.arg_data_type)
                    {
                        case Data_Types.NONE:
                            _scene_command.arg = string.Empty;
                            break;
                        case Data_Types.BOOL:
                            _scene_command.arg = cb.Checked.ToString();
                            break;
                        case Data_Types.LIST:
                            if (String.IsNullOrEmpty(cmbo.Text))
                            {
                                MessageBox.Show("Please select a vaule!", zvsEntityControl.zvsNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return;
                            }
                            _scene_command.arg = cmbo.Text;
                            break;
                        case Data_Types.STRING:
                            if (String.IsNullOrEmpty(tbx.Text))
                            {
                                MessageBox.Show("Please enter a vaule!", zvsEntityControl.zvsNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return;
                            }
                            _scene_command.arg = tbx.Text;
                            break;
                        case Data_Types.BYTE:
                        case Data_Types.DECIMAL:
                        case Data_Types.INTEGER:
                        case Data_Types.SHORT:
                            _scene_command.arg = Numeric.Value.ToString();
                            break;
                    }
                    break;
            }

            if (_scenecmdList != null)
                _scenecmdList.Add(_scene_command);

            zvsEntityControl.zvsContext.SaveChanges(); 

            this.Close();
        }

      

    }
}
