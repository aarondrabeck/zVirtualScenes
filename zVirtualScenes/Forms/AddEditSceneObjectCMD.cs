using System;
using System.Data;
using System.Windows.Forms;
using zVirtualScenesCommon.DatabaseCommon;
using zVirtualScenesAPI;
using System.Collections.Generic;
using zVirtualScenesAPI.Structs;
using System.ComponentModel;

namespace zVirtualScenesApplication.Forms
{
    public partial class AddEditSceneObjectCMD : Form
    {
        public SceneCommands SceneCMDtoEdit = null;
        public Command CMD = new Command();
        public string argument = "";
        public int ObjectID = 0;

        private List<zwObject> _masterDevices = new List<zwObject>();
        private CheckBox cb = new CheckBox();
        private NumericUpDown Numeric = new NumericUpDown();
        private TextBox tbx = new TextBox();
        private ComboBox cmbo = new ComboBox();
        
        public AddEditSceneObjectCMD(List<zwObject> devices)
        {
            InitializeComponent();
            _masterDevices = devices;
        }

        private void AddEditSceneCMD_Load(object sender, EventArgs e)
        {
            ActiveControl = comboBoxObjects;
            comboBoxObjects.Focus(); 

            comboBoxObjects.DataSource = _masterDevices;

            if (SceneCMDtoEdit != null)
            {
                zwObject obj = _masterDevices.Find(d => d.ID == SceneCMDtoEdit.ObjectId);
                if (obj != null)
                    comboBoxObjects.SelectedItem = obj;
            }
        }

        private void comboBoxObjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            zwObject selectedObj = (zwObject)comboBoxObjects.SelectedItem;
            ObjectID = selectedObj.ID;
            //Get Commands 

            List<Command> commands = API.Commands.GetAllObjectCommandsForObjectasCMD(selectedObj.ID);
            commands.AddRange(API.Commands.GetAllObjectTypeCommandsForObjectasCMD(selectedObj.ID));
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
                    cmbo.DropDownStyle = ComboBoxStyle.DropDownList;                   

                    switch (selected_cmd.cmdtype)
                    {
                        case cmdType.Object:
                            cmbo.DataSource = API.Commands.GetObjectCommandOptions(selected_cmd.CommandId);
                            break;
                        case cmdType.ObjectType:
                            cmbo.DataSource = API.Commands.GetObjectTypeCommandOptions(selected_cmd.CommandId);
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
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            CMD = (Command)comboBoxCommand.SelectedItem;

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

            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

      

    }
}
