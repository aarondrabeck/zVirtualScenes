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
        private int _SelectedDeviceIndex;
        private int _SelectedSceneIndex;
        private int _SelectedSceneActionIndex;
        Action TheAction = new Action();
        private bool _edit;

        public formAddEditActionBinSwitch(formzVirtualScenes zVirtualScenesMain, bool edit, int SelectedDeviceIndex, int SelectedSceneIndex, int SelectedSceneActionIndex)
        {
            InitializeComponent();

            _zVirtualScenesMain = zVirtualScenesMain;
            _SelectedDeviceIndex = SelectedDeviceIndex;
            _SelectedSceneIndex = SelectedSceneIndex;
            _SelectedSceneActionIndex = SelectedSceneActionIndex;
            _edit = edit;

            if (edit)
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
                TheAction = (Action)(_zVirtualScenesMain.MasterDevices[_SelectedDeviceIndex]);
            }

            #region Load Common Feilds into form fields
            lbl_Status.Text = "";
            label_DeviceName.Text = "Node " + TheAction.NodeID + ",  '" + TheAction.Name + "'";
            #endregion

            #region Binary Switch Specific Fields
            comboBoxBinaryONOFF.SelectedIndex = (TheAction.Level > 0 ? 1 : 0);
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
                if (_edit) //replace action
                {
                    _zVirtualScenesMain.MasterScenes[_SelectedSceneIndex].Actions.RemoveAt(_SelectedSceneActionIndex);
                    _zVirtualScenesMain.MasterScenes[_SelectedSceneIndex].Actions.Insert(_SelectedSceneActionIndex, TheAction);
                    _zVirtualScenesMain.SelectListBoxActionItem(_SelectedSceneActionIndex);
                }
                else //add new action                
                {
                    _zVirtualScenesMain.MasterScenes[_SelectedSceneIndex].Actions.Add(TheAction);
                    _zVirtualScenesMain.SelectListBoxActionItem(_zVirtualScenesMain.MasterScenes[_SelectedSceneIndex].Actions.Count() - 1);
                }

                this.Close();
            }
        }        
    }       
}
