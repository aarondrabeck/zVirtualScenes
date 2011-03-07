using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace zVirtualScenesApplication
{
    public partial class formAddEditActionThermostat : Form
    {
        private formzVirtualScenes _zVirtualScenesMain;
        private ZWaveDevice _SelectedDevice;
        private int _SelectedSceneIndex;
        private int _SelectedSceneActionIndex;
        Action TheAction = new Action();
        private bool CreateAction = false;
        
        /// <summary>
        /// Creates OR Edits Action
        /// </summary>
        /// <param name="zVirtualScenesMain">Main Form</param>
        /// <param name="SelectedSceneIndex">Index of Selected Scene</param>
        /// <param name="SelectedSceneActionIndex">Selected Scene Action Index</param>
        /// <param name="selectedDevice">OPTIONAL: ONLY USED IN CREATE NEW ACTION</param>
        public formAddEditActionThermostat(formzVirtualScenes zVirtualScenesMain, int SelectedSceneIndex, int SelectedSceneActionIndex, ZWaveDevice selectedDevice = null)
        {
            InitializeComponent();
            _zVirtualScenesMain = zVirtualScenesMain;
            _SelectedDevice = selectedDevice;
            _SelectedSceneIndex = SelectedSceneIndex;
            _SelectedSceneActionIndex = SelectedSceneActionIndex;

            if (selectedDevice != null)
                CreateAction = true;

            if (!CreateAction)
            {
                groupBoxAction.Text = "Edit Action";

                btn_Save.Text = "Save Action"; 
                TheAction = _zVirtualScenesMain.MasterScenes[SelectedSceneIndex].Actions[SelectedSceneActionIndex];
            }
            else
            {
                groupBoxAction.Text = "Create New Action";
                btn_Save.Text = "Add Action to '" + _zVirtualScenesMain.MasterScenes[SelectedSceneIndex].Name + "'"; 
                //Convert Device to Action id this is a new action and not an edit
                TheAction = (Action)_SelectedDevice;
            }
            #region Load Common Feilds into form fields
            lbl_Status.Text = "";
            label_DeviceName.Text = "Node " + TheAction.NodeID + ",  '" + TheAction.Name + "'";
            #endregion

            #region Thermostat Specific Fields            

            //Fill Thermo Option Dropdowns
            comboBoxHeatCoolMode.DataSource = Enum.GetNames(typeof(ZWaveDevice.ThermostatMode));
            comboBoxFanMode.DataSource = Enum.GetNames(typeof(ZWaveDevice.ThermostatFanMode));
            comboBoxEnergyMode.DataSource = Enum.GetNames(typeof(ZWaveDevice.EnergyMode));

            //Set Action Options 
            if (TheAction.HeatPoint != -1)
            {
                checkBoxeditHP.Checked = true;
                txtbx_HeatPoint.Text = TheAction.HeatPoint.ToString();
            }

            if (TheAction.CoolPoint != -1)
            {
                checkBoxeditCP.Checked = true;
                textBoxCoolPoint.Text = TheAction.CoolPoint.ToString();
            }

            comboBoxHeatCoolMode.SelectedItem = Enum.GetName(typeof(ZWaveDevice.ThermostatMode), TheAction.HeatCoolMode);
            comboBoxEnergyMode.SelectedItem = Enum.GetName(typeof(ZWaveDevice.ThermostatFanMode), TheAction.EngeryMode);
            comboBoxFanMode.SelectedItem = Enum.GetName(typeof(ZWaveDevice.EnergyMode), TheAction.FanMode);
            
            #endregion

        }
 
        private bool UpdateThermostatAction(Action myBinSwitchAction)
        {
            //ERROR CHECK INPUTS
            TheAction.HeatCoolMode = (int)Enum.Parse(typeof(ZWaveDevice.ThermostatMode), comboBoxHeatCoolMode.SelectedValue.ToString());
            TheAction.FanMode = (int)Enum.Parse(typeof(ZWaveDevice.ThermostatFanMode), comboBoxFanMode.SelectedValue.ToString());
            TheAction.EngeryMode = (int)Enum.Parse(typeof(ZWaveDevice.EnergyMode), comboBoxEnergyMode.SelectedValue.ToString());

            //Make sure atleast one thermo action was chosen
            if (TheAction.HeatCoolMode == -1 && TheAction.FanMode == -1 && TheAction.EngeryMode == -1 && checkBoxeditHP.Checked == false && checkBoxeditCP.Checked == false)
            {
                MessageBox.Show("Please select at least one Temperature Mode.", _zVirtualScenesMain.ProgramName);
                return false;
            }

            if (checkBoxeditHP.Checked == true)
            {
                try { TheAction.HeatPoint = Convert.ToInt32(txtbx_HeatPoint.Text); }
                catch
                {
                    MessageBox.Show("Invalid Heat Point.", _zVirtualScenesMain.ProgramName);
                    return false;
                }
            }

            if (checkBoxeditCP.Checked == true)
            {
                try { TheAction.CoolPoint = Convert.ToInt32(textBoxCoolPoint.Text); }
                catch
                {
                    MessageBox.Show("Invalid Cool Point..", _zVirtualScenesMain.ProgramName);
                    return false;
                }
            }

            return true;
        }       

        private void btn_RunCommand_Click(object sender, EventArgs e)
        {
            if (UpdateThermostatAction(TheAction))
            {
                ActionResult result = TheAction.Run(_zVirtualScenesMain.ControlThinkController);
                _zVirtualScenesMain.LogThis((int)result.ResultType, "GUI: [USER] " + result.Description);
                lbl_Status.Text = result.ResultType + " " + result.Description;
            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            if (UpdateThermostatAction(TheAction))
            {
                if (!CreateAction) //replace action
                {
                    _zVirtualScenesMain.MasterScenes[_SelectedSceneIndex].Actions.RemoveAt(_SelectedSceneActionIndex);
                    _zVirtualScenesMain.MasterScenes[_SelectedSceneIndex].Actions.Insert(_SelectedSceneActionIndex, TheAction);
                    _zVirtualScenesMain.SelectListBoxActionItem(_SelectedSceneActionIndex);
                }
                else
                {
                    if (_SelectedSceneActionIndex == -1)  //First Action in Scene
                        _SelectedSceneActionIndex = 0;
                    else
                        _SelectedSceneActionIndex++;  //Add item below cuurent selection

                    _zVirtualScenesMain.MasterScenes[_SelectedSceneIndex].Actions.Insert(_SelectedSceneActionIndex, TheAction);
                    _zVirtualScenesMain.SelectListBoxActionItem(_SelectedSceneActionIndex);
                }

                this.Close();
            }
        }
       
        private void checkBoxeditHP_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxeditHP.Checked == true)
                txtbx_HeatPoint.Enabled = true;
            else
                txtbx_HeatPoint.Enabled = false;
        }

        private void checkBoxeditCP_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxeditCP.Checked == true)
                textBoxCoolPoint.Enabled = true;
            else
                textBoxCoolPoint.Enabled = false;
        }
    }       
}
