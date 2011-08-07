using System;
using System.Data;
using System.Windows.Forms;
using zVirtualScenesCommon.DatabaseCommon;
using zVirtualScenesAPI;
using System.Collections.Generic;
using zVirtualScenesAPI.Structs;
using System.ComponentModel;
using System.Collections;

namespace zVirtualScenesApplication.Forms
{
    public partial class AddEditSceneBuiltinCMD : Form
    {
        public SceneCommands SceneCMDtoEdit = null;
        public Command CMD = new Command();
        public string argument = "";

        private List<zwObject> _masterDevices = new List<zwObject>();
        private CheckBox cb = new CheckBox();
        private NumericUpDown Numeric = new NumericUpDown();
        private TextBox tbx = new TextBox();
        private ComboBox cmbo = new ComboBox();
        
        public AddEditSceneBuiltinCMD(List<zwObject> devices)
        {
            InitializeComponent();
            _masterDevices = devices;
        }

        private void AddEditSceneCMD_Load(object sender, EventArgs e)
        {
            ActiveControl = comboBoxCommand;
            comboBoxCommand.Focus();
            
            List<Command> commands = API.Commands.GetBuiltinCommandsasCMD();
            comboBoxCommand.DataSource = commands;

            if (SceneCMDtoEdit != null)
            {
                Command cmd = commands.Find(c => c.CommandId == SceneCMDtoEdit.CommandId && c.cmdtype == SceneCMDtoEdit.cmdtype);

                if (cmd != null)
                    comboBoxCommand.SelectedItem = cmd;
            }
        }     

        private void comboBoxCommand_SelectedIndexChanged(object sender, EventArgs e)
        {
            Command selected_cmd = (Command)comboBoxCommand.SelectedItem;
            toolTip1.SetToolTip(comboBoxCommand, selected_cmd.HelpText);


            //Do Custom things for some Builtin Commands
            switch (selected_cmd.Name)
            {
                case "REPOLL_ME":
                    panelUserInputControls.Controls.Clear();
                    cmbo.Width = 269;
                    cmbo.DropDownStyle = ComboBoxStyle.DropDownList;

                    if (cmbo.Items.Count > 0)
                        cmbo.Items.Clear();

                    foreach (DataRow dr in API.Object.GetObjects(true).Rows)
                    {                         
                        int id = 0;
                        int.TryParse(dr["id"].ToString(), out id);
                        KeyValuePair<int, string> objNameID = new KeyValuePair<int, string>(id, dr["txt_object_name"].ToString());
                        //cmbo.DisplayMember = "Value";
                        cmbo.Items.Add(objNameID);
                    }

                    if (cmbo.Items.Count > 0)
                        cmbo.SelectedIndex = 0; 

                    if (SceneCMDtoEdit != null)
                    {
                        int id = 0;
                        int.TryParse(SceneCMDtoEdit.Argument, out id);
                        cmbo.SelectedItem = new KeyValuePair<int, string>(id, API.Object.GetObjectName(id));
                    }
                    panelUserInputControls.Controls.Add(cmbo);
                    break;
                case "GROUP_ON":
                    panelUserInputControls.Controls.Clear();
                    cmbo.Width = 269;
                    cmbo.DropDownStyle = ComboBoxStyle.DropDownList;

                    if (cmbo.Items.Count > 0)
                        cmbo.Items.Clear();

                    foreach (DataRow dr in API.Groups.GetGroups().Rows)
                        cmbo.Items.Add(dr["txt_group_name"].ToString());

                    if (cmbo.Items.Count > 0)
                        cmbo.SelectedIndex = 0; 

                    if (SceneCMDtoEdit != null)
                    {
                        cmbo.SelectedItem = SceneCMDtoEdit.Argument;
                    }
                    panelUserInputControls.Controls.Add(cmbo);
                    break;
                case "GROUP_OFF":
                    panelUserInputControls.Controls.Clear();
                    cmbo.Width = 269;
                    cmbo.DropDownStyle = ComboBoxStyle.DropDownList;

                    if (cmbo.Items.Count > 0)
                        cmbo.Items.Clear();

                    foreach (DataRow dr in API.Groups.GetGroups().Rows)
                        cmbo.Items.Add(dr["txt_group_name"].ToString());

                    if (cmbo.Items.Count > 0)
                        cmbo.SelectedIndex = 0; 

                    if (SceneCMDtoEdit != null)
                    {
                        cmbo.SelectedItem = SceneCMDtoEdit.Argument;
                    }
                    panelUserInputControls.Controls.Add(cmbo);
                    break;
                default:
                    switch (selected_cmd.paramType)
                    {
                        case ParamType.NONE:
                            panelUserInputControls.Controls.Clear();
                            break;
                        case ParamType.BOOL:
                            panelUserInputControls.Controls.Clear();
                            cb.Text = selected_cmd.FriendlyName;
                            panelUserInputControls.Controls.Add(cb);

                            if (SceneCMDtoEdit != null)
                            {
                                bool bvalue = false;
                                bool.TryParse(SceneCMDtoEdit.Argument, out bvalue);
                                cb.Checked = bvalue;
                            }

                            break;
                        case ParamType.LIST:
                            panelUserInputControls.Controls.Clear();
                            cmbo.Width = 269;
                            cmbo.DropDownStyle = ComboBoxStyle.DropDownList;

                            switch (selected_cmd.cmdtype)
                            {
                                case cmdType.Builtin:
                                    cmbo.DataSource = API.Commands.GetBuiltinCommandOptions(selected_cmd.CommandId);
                                    break;
                            }

                            if (cmbo.Items.Count > 0)
                                cmbo.SelectedIndex = 0; 

                            if (SceneCMDtoEdit != null)                            
                                cmbo.SelectedItem = SceneCMDtoEdit.Argument;
                            
                            panelUserInputControls.Controls.Add(cmbo);
                            break;
                        case ParamType.STRING:
                            panelUserInputControls.Controls.Clear();
                            panelUserInputControls.Controls.Add(tbx);
                            if (SceneCMDtoEdit != null)
                            {
                                tbx.SelectedText = SceneCMDtoEdit.Argument;
                            }
                            break;
                        case ParamType.INTEGER:
                            panelUserInputControls.Controls.Clear();
                            Numeric.Maximum = Int64.MaxValue;
                            Numeric.Minimum = Int64.MinValue;
                            panelUserInputControls.Controls.Add(Numeric);

                            if (SceneCMDtoEdit != null)
                            {
                                int ivalue = 0;
                                int.TryParse(SceneCMDtoEdit.Argument, out ivalue);
                                Numeric.Value = ivalue;
                            }
                            break;
                        case ParamType.BYTE:
                            panelUserInputControls.Controls.Clear();
                            Numeric.Maximum = Byte.MaxValue;
                            Numeric.Minimum = Byte.MinValue;
                            panelUserInputControls.Controls.Add(Numeric);

                            if (SceneCMDtoEdit != null)
                            {
                                byte bvalue = 0;
                                byte.TryParse(SceneCMDtoEdit.Argument, out bvalue);
                                Numeric.Value = bvalue;
                            }
                            break;
                        case ParamType.DECIMAL:
                            panelUserInputControls.Controls.Clear();
                            Numeric.Maximum = Decimal.MaxValue;
                            Numeric.Minimum = Decimal.MinValue;
                            panelUserInputControls.Controls.Add(Numeric);

                            if (SceneCMDtoEdit != null)
                            {
                                decimal dvalue = 0;
                                decimal.TryParse(SceneCMDtoEdit.Argument, out dvalue);
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
            CMD = (Command)comboBoxCommand.SelectedItem;

            //Do Custom things for some Builtin Commands
            switch (CMD.Name)
            {
                case "REPOLL_ME":
                    if (cmbo.SelectedItem == null)
                    {
                        MessageBox.Show("Please select a object!", API.GetProgramNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    KeyValuePair<int, string> objNameID = (KeyValuePair<int, string>)cmbo.SelectedItem;
                    argument = objNameID.Key.ToString();
                    break;
                case "GROUP_ON":
                    if (String.IsNullOrEmpty(cmbo.Text))
                    {
                        MessageBox.Show("Please select a vaule!", API.GetProgramNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    argument = cmbo.Text;
                    break;
                case "GROUP_OFF":
                    if (String.IsNullOrEmpty(cmbo.Text))
                    {
                        MessageBox.Show("Please select a vaule!", API.GetProgramNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    argument = cmbo.Text;
                    break;
                default:
                    switch (CMD.paramType)
                    {
                        case ParamType.NONE:
                            argument = string.Empty;
                            break;
                        case ParamType.BOOL:
                            argument = cb.Checked.ToString();
                            break;
                        case ParamType.LIST:
                            if (String.IsNullOrEmpty(cmbo.Text))
                            {
                                MessageBox.Show("Please select a vaule!", API.GetProgramNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return;
                            }
                            argument = cmbo.Text;
                            break;
                        case ParamType.STRING:
                            if (String.IsNullOrEmpty(tbx.Text))
                            {
                                MessageBox.Show("Please enter a vaule!", API.GetProgramNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return;
                            }
                            argument = tbx.Text;
                            break;
                        case ParamType.BYTE:
                        case ParamType.DECIMAL:
                        case ParamType.INTEGER:
                        case ParamType.SHORT:
                            argument = Numeric.Value.ToString();
                            break;
                    }
                    break;
            }          

            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

      

    }
}
