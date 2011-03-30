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
        private Scene theScene;
        private Action theAction;
        private int InsertPosition;
        private bool EditMode;
        private int sceneIndex;

        /// <summary>
        /// Edit Thermostat Action
        /// </summary>
        /// <param name="zVirtualScenesMain"></param>
        /// <param name="scene"></param>
        /// <param name="action"></param>
        public formAddEditActionThermostat(formzVirtualScenes zVirtualScenesMain, Scene scene, Action action)
        {
            //Edit Items
            InitializeComponent();
            this._zVirtualScenesMain = zVirtualScenesMain;
            this.theScene = scene;
            this.sceneIndex = _zVirtualScenesMain.MasterScenes.IndexOf(this.theScene);
            this.EditMode = true;

            this.groupBoxAction.Text = "Edit Action";
            this.btn_Save.Text = "&Save";
            this.theAction = action;
            this.InsertPosition = _zVirtualScenesMain.MasterScenes[sceneIndex].Actions.IndexOf(theAction);

            LoadGui();
        }

        /// <summary>
        /// Create New Thermostat Action
        /// </summary>
        /// <param name="zVirtualScenesMain"></param>
        /// <param name="scene"></param>
        /// <param name="device"></param>
        /// <param name="PositionOfNewItem"></param>
        public formAddEditActionThermostat(formzVirtualScenes zVirtualScenesMain, Scene scene, ZWaveDevice device, int PositionOfNewItem)
        {
            //Add Items
            InitializeComponent();
            this._zVirtualScenesMain = zVirtualScenesMain;
            this.theScene = scene;
            this.sceneIndex = _zVirtualScenesMain.MasterScenes.IndexOf(this.theScene);
            this.EditMode = false;
            
            this.groupBoxAction.Text = "Create New Action";
            this.btn_Save.Text = "&Add Action to '" + scene.Name + "'";
            this.theAction = (Action)device;
            this.InsertPosition = PositionOfNewItem;

            LoadGui();
        }

        private void LoadGui()
        {
            #region Load Common Feilds into form fields
            lbl_Status.Text = "";
            label_DeviceName.Text = "Node " + theAction.NodeID + ",  '" + theAction.Name + "'";
            checkBoxSkipDark.Checked = theAction.SkipWhenDark;
            checkBoxSkipLight.Checked = theAction.SkipWhenLight;
            #endregion

            #region Thermostat Specific Fields            

            //Fill Thermo Option Dropdowns
            comboBoxHeatCoolMode.DataSource = Enum.GetNames(typeof(ZWaveDevice.ThermostatMode));
            comboBoxFanMode.DataSource = Enum.GetNames(typeof(ZWaveDevice.ThermostatFanMode));
            comboBoxEnergyMode.DataSource = Enum.GetNames(typeof(ZWaveDevice.EnergyMode));

            //Set Action Options 
            if (theAction.HeatPoint != -1)
            {
                checkBoxeditHP.Checked = true;
                txtbx_HeatPoint.Text = theAction.HeatPoint.ToString();
            }

            if (theAction.CoolPoint != -1)
            {
                checkBoxeditCP.Checked = true;
                textBoxCoolPoint.Text = theAction.CoolPoint.ToString();
            }

            comboBoxHeatCoolMode.SelectedItem = Enum.GetName(typeof(ZWaveDevice.ThermostatMode), theAction.HeatCoolMode);
            comboBoxEnergyMode.SelectedItem = Enum.GetName(typeof(ZWaveDevice.ThermostatFanMode), theAction.EngeryMode);
            comboBoxFanMode.SelectedItem = Enum.GetName(typeof(ZWaveDevice.EnergyMode), theAction.FanMode);
            
            #endregion
        }
 
        private bool UpdateThermostatAction(Action myBinSwitchAction)
        {
            //ERROR CHECK INPUTS
            theAction.HeatCoolMode = (int)Enum.Parse(typeof(ZWaveDevice.ThermostatMode), comboBoxHeatCoolMode.SelectedValue.ToString());
            theAction.FanMode = (int)Enum.Parse(typeof(ZWaveDevice.ThermostatFanMode), comboBoxFanMode.SelectedValue.ToString());
            theAction.EngeryMode = (int)Enum.Parse(typeof(ZWaveDevice.EnergyMode), comboBoxEnergyMode.SelectedValue.ToString());

            //Make sure atleast one thermo action was chosen
            if (theAction.HeatCoolMode == -1 && theAction.FanMode == -1 && theAction.EngeryMode == -1 && checkBoxeditHP.Checked == false && checkBoxeditCP.Checked == false)
            {
                MessageBox.Show("Please select at least one Temperature Mode.", _zVirtualScenesMain.ProgramName);
                return false;
            }

            if (checkBoxeditHP.Checked == true)
            {
                try { theAction.HeatPoint = Convert.ToInt32(txtbx_HeatPoint.Text); }
                catch
                {
                    MessageBox.Show("Invalid Heat Point.", _zVirtualScenesMain.ProgramName);
                    return false;
                }
            }

            if (checkBoxeditCP.Checked == true)
            {
                try { theAction.CoolPoint = Convert.ToInt32(textBoxCoolPoint.Text); }
                catch
                {
                    MessageBox.Show("Invalid Cool Point..", _zVirtualScenesMain.ProgramName);
                    return false;
                }
            }

            theAction.SkipWhenDark = checkBoxSkipDark.Checked;
            theAction.SkipWhenLight = checkBoxSkipLight.Checked;

            return true;
        }       

        private void btn_RunCommand_Click(object sender, EventArgs e)
        {
            if (UpdateThermostatAction(theAction))
            {
                ActionResult result = theAction.Run(_zVirtualScenesMain);
                _zVirtualScenesMain.AddLogEntry((UrgencyLevel)result.ResultType, "GUI: [USER] " + result.Description);
                lbl_Status.Text = result.ResultType + " " + result.Description;
            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {

            if (_zVirtualScenesMain.MasterScenes[this.sceneIndex].isRunning)
            {
                MessageBox.Show("Cannot modify scene when it is running.", _zVirtualScenesMain.ProgramName);
                return;
            }

            if (UpdateThermostatAction(theAction))
            {
                //SAVE            
                if (EditMode) //replace action so delete before add.       
                    _zVirtualScenesMain.MasterScenes[sceneIndex].Actions.Remove(theAction);

                if (this.InsertPosition == -1)  //First Action in Scene
                    _zVirtualScenesMain.MasterScenes[sceneIndex].Actions.Add(theAction);
                else
                    _zVirtualScenesMain.MasterScenes[sceneIndex].Actions.Insert(InsertPosition, theAction);

                _zVirtualScenesMain.SelectListBoxActionItem(theAction);
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

        private void formAddEditActionThermostat_Load(object sender, EventArgs e)
        {
            ActiveControl = comboBoxHeatCoolMode;
        }
    }       
}
