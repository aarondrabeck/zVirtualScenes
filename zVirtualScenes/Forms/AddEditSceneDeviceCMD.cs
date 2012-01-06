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
        private long? scenecmd_id_to_edit;
        private long device_id;
        private long? scene_id;
        private bool editing;
        private int positionInList; 

        private CheckBox cb = new CheckBox();
        private NumericUpDown Numeric = new NumericUpDown();
        private TextBox tbx = new TextBox();
        private ComboBox cmbo = new ComboBox();

        public AddEditSceneDeviceCMD(long? scenecmd_id_to_edit, long device_id, long? scene_id = null, int? positionInList = 0)
        {
            this.scenecmd_id_to_edit = scenecmd_id_to_edit;
            this.device_id = device_id;
            this.scene_id = scene_id;
            this.positionInList = positionInList.Value;

            InitializeComponent();
        }

        private void AddEditSceneCMD_Load(object sender, EventArgs e)
        {
            comboBoxCommands.DisplayMember = "friendly_name";

            //determine if editing
            editing = false;
            if (scenecmd_id_to_edit.HasValue)
            {
                editing = true;
            }          
            else
            {
                if (!scene_id.HasValue)
                    this.Close(); 
            }

            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                device device = db.devices.FirstOrDefault(d => d.id == device_id);
                //Populate combo boxes
                if (device != null)
                {
                    this.Text = "Scene Command for '" + device.friendly_name + "'";

                    if (editing)
                    {
                        scene_commands scmd = db.scene_commands.FirstOrDefault(c => c.id == scenecmd_id_to_edit.Value);
                        if (scmd != null)
                        {
                            switch ((command_types)scmd.command_type_id)
                            {
                                case command_types.device_command:
                                    {
                                        radioBtnDeviceCMD.Checked = true;
                                        comboBoxCommands.DataSource = device.device_commands;

                                        device_commands dc = device.device_commands.FirstOrDefault(c => c.id == scmd.command_id);
                                        if (dc != null)
                                            comboBoxCommands.SelectedItem = dc;

                                        break;
                                    }
                                case command_types.device_type_command:
                                    {
                                        radioBtnTypeCommand.Checked = true;
                                        comboBoxCommands.DataSource = device.device_types.device_type_commands;
                                        
                                        device_type_commands dtc = device.device_types.device_type_commands.FirstOrDefault(c => c.id == scmd.command_id);
                                        if (dtc != null)
                                            comboBoxCommands.SelectedItem = dtc;

                                        break;
                                    }
                            }
                        }                        
                    }
                    else
                        radioBtnDeviceCMD.Checked = true;
                }
            }            
        }

        private void comboBoxTypeCommands_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                string prefilledValue = string.Empty; 
                if (editing)
                {
                    scene_commands scmd = db.scene_commands.FirstOrDefault(c => c.id == scenecmd_id_to_edit.Value);
                    if (scmd != null)                    
                        prefilledValue = scmd.arg; 
                    
                }


                if (radioBtnDeviceCMD.Checked)
                {
                    device_commands selected_cmd = db.device_commands.FirstOrDefault(c => c.id == ((device_commands)comboBoxCommands.SelectedItem).id);
                    if (selected_cmd != null)
                    {
                        
                            AddDynamicInputControl((Data_Types)selected_cmd.arg_data_type,
                                                    selected_cmd.friendly_name,
                                                    selected_cmd.device_command_options.Select(o => o.name).ToList(),
                                                    prefilledValue);                       
                    }
                    else
                    {
                        panelUserInputControls.Controls.Clear();
                    }
                }
                else
                {
                    device_type_commands selected_cmd = db.device_type_commands.FirstOrDefault(c => c.id == ((device_type_commands)comboBoxCommands.SelectedItem).id);
                    if (selected_cmd != null)
                    {                        
                            AddDynamicInputControl((Data_Types)selected_cmd.arg_data_type,
                                                    selected_cmd.friendly_name,
                                                    selected_cmd.device_type_command_options.Select(o => o.option).ToList(),
                                                    prefilledValue);                        
                    }
                    else
                    {
                        panelUserInputControls.Controls.Clear();
                    }
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
           scene_commands scmd;
           using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
           {
               if (editing)
                   scmd = db.scene_commands.FirstOrDefault(c => c.id == scenecmd_id_to_edit.Value);
               else
               {
                   scmd = new scene_commands();
                   scmd.device_id = device_id;
                   scmd.scene_id = scene_id.Value;
                   scmd.sort_order = positionInList;
               }

               //Update the command type, command id, and arg
               if (radioBtnTypeCommand.Checked)
               {
                   device_type_commands selected_cmd = (device_type_commands)comboBoxCommands.SelectedItem;
                   if (selected_cmd == null)
                   {
                       MessageBox.Show("Please select a command!", zvsEntityControl.zvsNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                       return;
                   }
                   scmd.command_id = selected_cmd.id;
                   scmd.command_type_id = (int)command_types.device_type_command;
                   string userInput = GetUserInput((Data_Types)selected_cmd.arg_data_type);

                   if (userInput == null)
                       return;
                   else
                       scmd.arg = userInput;
               }
               else
               {
                   device_commands selected_cmd = (device_commands)comboBoxCommands.SelectedItem;
                   if (selected_cmd == null)
                   {
                       MessageBox.Show("Please select a command!", zvsEntityControl.zvsNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                       return;
                   }

                   scmd.command_id = selected_cmd.id;
                   scmd.command_type_id = (int)command_types.device_command;

                   string userInput = GetUserInput((Data_Types)selected_cmd.arg_data_type);

                   if (userInput == null)
                       return;
                   else
                       scmd.arg = userInput;
               }

               if (!editing)
                   db.scene_commands.AddObject(scmd); 

               db.SaveChanges();
           }
           zvsEntityControl.CallSceneModified(this, null);

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
                        cmbo.Text = PrefilledValue;

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
                using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                {
                    device device = db.devices.FirstOrDefault(d => d.id == device_id);
                    //Populate combo boxes
                    if (device != null)
                    {
                        comboBoxCommands.DataSource = device.device_commands;
                    }
                }
            }
        }

        private void radioBtnTypeCommand_CheckedChanged(object sender, EventArgs e)
        {
            if (radioBtnTypeCommand.Checked)
            {
                using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                {
                    device device = db.devices.FirstOrDefault(d => d.id == device_id);
                    //Populate combo boxes
                    if (device != null)
                    {
                        comboBoxCommands.DataSource = device.device_types.device_type_commands;
                    }
                }
                
            }

        }
    }
}
