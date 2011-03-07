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
    public partial class formAddEditActionBinSwitch : Form
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
        public formAddEditActionBinSwitch(formzVirtualScenes zVirtualScenesMain, int SelectedSceneIndex, int SelectedSceneActionIndex, ZWaveDevice selectedDevice = null )
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

            #region Binary Switch Specific Fields
            comboBoxBinaryONOFF.SelectedIndex = (TheAction.Level > 0 ? 1 : 0);
            labelMomentaryMode.Text = "Momentary Mode: " + (TheAction.MomentaryOnMode ? "ON" : "OFF");
            #endregion
                      
        }

        private bool UpdateBinarySwitchAction()
        {
            //ERROR CHECK INPUTS
            TheAction.Level = (byte)comboBoxBinaryONOFF.SelectedIndex;
            return true;
        }

        private void btn_RunCommand_Click(object sender, EventArgs e)
        {
            if (UpdateBinarySwitchAction())
            {
                ActionResult result = TheAction.Run(_zVirtualScenesMain.ControlThinkController);
                _zVirtualScenesMain.LogThis((int)result.ResultType, "GUI: [USER] " + result.Description);
                lbl_Status.Text = result.ResultType + " " + result.Description;
            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            if (UpdateBinarySwitchAction())
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
    }       
}
