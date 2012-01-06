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
        private long? scenecmd_id_to_edit;
        private long? selected_scene_id;
        private bool editing; 

        private CheckBox cb = new CheckBox();
        private NumericUpDown Numeric = new NumericUpDown();
        private TextBox tbx = new TextBox();
        private ComboBox cmbo = new ComboBox();

        /// <summary>
        /// If new and not an edit, send scenecmdList and the scene_cmd will be added to the list when save is clicked.
        /// </summary>
        /// <param name="scene_cmd"></param>
        /// <param name="scenecmdList"></param>
        public AddEditSceneBuiltinCMD(long? scenecmd_id_to_edit, long? selected_scene_id = null)
        {
            this.scenecmd_id_to_edit = scenecmd_id_to_edit;
            this.selected_scene_id = selected_scene_id;
            InitializeComponent();           
        }
        
        private void AddEditSceneCMD_Load(object sender, EventArgs e)
        {
            ActiveControl = comboBoxCommand;
            comboBoxCommand.Focus();
            comboBoxCommand.DisplayMember = "friendly_name";
            editing = false; 

             using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
             {
                 comboBoxCommand.DataSource = db.builtin_commands.OfType<builtin_commands>().Execute(MergeOption.AppendOnly);

                 //If we are editing, populate combo box
                 if (scenecmd_id_to_edit.HasValue)
                 {
                     scene_commands scmd = db.scene_commands.FirstOrDefault(c => c.id == scenecmd_id_to_edit.Value);
                     if (scmd != null)
                     {
                         editing = true;
                         builtin_commands b_cmd = db.builtin_commands.FirstOrDefault(c => c.id == scmd.command_id);
                         if (b_cmd != null)
                             comboBoxCommand.SelectedItem = b_cmd;
                     }
                 }
                 else 
                 {
                     if (!selected_scene_id.HasValue)
                     {
                         //Must pass a scene id if you do not have a scene to edit. 
                         this.Close();
                     }

                 }
             }                      
        }     

        private void comboBoxCommand_SelectedIndexChanged(object sender, EventArgs e)
        {
            builtin_commands selected_cmd = (builtin_commands)comboBoxCommand.SelectedItem;

            scene_commands scmd = null;
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                if (editing)
                {
                    scmd = db.scene_commands.FirstOrDefault(c => c.id == scenecmd_id_to_edit.Value);

                    //error
                    if (scmd == null)
                        this.Close(); 
                }
                
                //Do Custom things for some Builtin Commands
                switch (selected_cmd.name)
                {
                    case "REPOLL_ME":
                        panelUserInputControls.Controls.Clear();
                        cmbo.Width = 269;
                        cmbo.DropDownStyle = ComboBoxStyle.DropDownList;

                        cmbo.DataSource = null;
                        cmbo.DataSource = db.devices.OfType<device>().Execute(MergeOption.AppendOnly);
                        cmbo.DisplayMember = "friendly_name";

                        if (cmbo.Items.Count > 0)
                            cmbo.SelectedIndex = 0;

                        if (editing)
                        {
                            long d_id = 0;
                            long.TryParse(scmd.arg, out d_id);
                            cmbo.SelectedItem = db.devices.FirstOrDefault(d => d.id == d_id);
                        }

                        panelUserInputControls.Controls.Add(cmbo);
                        break;
                    case "GROUP_ON":
                    case "GROUP_OFF":
                        panelUserInputControls.Controls.Clear();
                        cmbo.Width = 269;
                        cmbo.DropDownStyle = ComboBoxStyle.DropDownList;

                        cmbo.DataSource = null;
                        cmbo.DataSource = db.groups.OfType<group>().Execute(MergeOption.AppendOnly);
                        cmbo.DisplayMember = "name";

                        if (cmbo.Items.Count > 0)
                            cmbo.SelectedIndex = 0;

                        if (editing)
                        {
                            long g_id = 0;
                            long.TryParse(scmd.arg, out g_id);
                            cmbo.SelectedItem = db.groups.FirstOrDefault(g => g.id == g_id);
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

                                if (editing)
                                {
                                    bool bvalue = false;
                                    bool.TryParse(scmd.arg, out bvalue);
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

                                if (editing)
                                    cmbo.SelectedItem = scmd.arg;

                                panelUserInputControls.Controls.Add(cmbo);
                                break;
                            case Data_Types.STRING:
                                panelUserInputControls.Controls.Clear();
                                panelUserInputControls.Controls.Add(tbx);
                                if (editing)
                                {
                                    tbx.SelectedText = scmd.arg;
                                }
                                break;
                            case Data_Types.INTEGER:
                                panelUserInputControls.Controls.Clear();
                                Numeric.Maximum = Int64.MaxValue;
                                Numeric.Minimum = Int64.MinValue;
                                panelUserInputControls.Controls.Add(Numeric);

                                if (editing)
                                {
                                    int ivalue = 0;
                                    int.TryParse(scmd.arg, out ivalue);
                                    Numeric.Value = ivalue;
                                }
                                break;
                            case Data_Types.BYTE:
                                panelUserInputControls.Controls.Clear();
                                Numeric.Maximum = Byte.MaxValue;
                                Numeric.Minimum = Byte.MinValue;
                                panelUserInputControls.Controls.Add(Numeric);

                                if (editing)
                                {
                                    byte bvalue = 0;
                                    byte.TryParse(scmd.arg, out bvalue);
                                    Numeric.Value = bvalue;
                                }
                                break;
                            case Data_Types.DECIMAL:
                                panelUserInputControls.Controls.Clear();
                                Numeric.Maximum = Decimal.MaxValue;
                                Numeric.Minimum = Decimal.MinValue;
                                panelUserInputControls.Controls.Add(Numeric);

                                if (editing)
                                {
                                    decimal dvalue = 0;
                                    decimal.TryParse(scmd.arg, out dvalue);
                                    Numeric.Value = dvalue;
                                }
                                break;
                        }
                        break;
                }
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

            scene_commands scmd;
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                if (editing)
                    scmd = db.scene_commands.FirstOrDefault(c => c.id == scenecmd_id_to_edit.Value);
                else
                {
                    scmd = new scene_commands();
                    scmd.scene_id = selected_scene_id.Value;
                }
                scmd.command_type_id = (int)command_types.builtin;
                scmd.command_id = selected_cmd.id;

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
                                scmd.arg = d.id.ToString();

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
                                scmd.arg = g.id.ToString();

                            break;
                        }
                    default:
                        switch ((Data_Types)selected_cmd.arg_data_type)
                        {
                            case Data_Types.NONE:
                                scmd.arg = string.Empty;
                                break;
                            case Data_Types.BOOL:
                                scmd.arg = cb.Checked.ToString();
                                break;
                            case Data_Types.LIST:
                                if (String.IsNullOrEmpty(cmbo.Text))
                                {
                                    MessageBox.Show("Please select a vaule!", zvsEntityControl.zvsNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    return;
                                }
                                scmd.arg = cmbo.Text;
                                break;
                            case Data_Types.STRING:
                                if (String.IsNullOrEmpty(tbx.Text))
                                {
                                    MessageBox.Show("Please enter a vaule!", zvsEntityControl.zvsNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    return;
                                }
                                scmd.arg = tbx.Text;
                                break;
                            case Data_Types.BYTE:
                            case Data_Types.DECIMAL:
                            case Data_Types.INTEGER:
                            case Data_Types.SHORT:
                                scmd.arg = Numeric.Value.ToString();
                                break;
                        }
                        break;
                }

                if (!editing)
                    db.scene_commands.AddObject(scmd);

                db.SaveChanges();
            }
            zvsEntityControl.CallSceneModified(this, null);

            this.Close();
        }     

    }
}
