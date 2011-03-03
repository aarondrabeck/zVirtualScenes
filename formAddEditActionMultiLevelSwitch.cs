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
    public partial class formAddEditActionMultiLevelSwitch : Form
    {
        private formzVirtualScenes _zVirtualScenesMain;
        private int _SelectedDeviceIndex;
        private int _SelectedSceneIndex;
        private int _SelectedSceneActionIndex;
        Action TheAction = new Action();
        private bool _edit;

        public formAddEditActionMultiLevelSwitch(formzVirtualScenes zVirtualScenesMain, bool edit, int SelectedDeviceIndex, int SelectedSceneIndex, int SelectedSceneActionIndex)
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

            #region MultiLevel Switch Specific Fields
            txtbox_level.Text = TheAction.Level.ToString();
            #endregion
           
        }
         
        private bool UpdateMultiLevelSwitchAction()
        {            
            //ERROR CHECK INPUTS
            try
            {
                byte level = Convert.ToByte(txtbox_level.Text);
                if (level < 100 || level == 255)
                    TheAction.Level = level;
                else
                    throw new ArgumentException("Invalid Level.");
            }
            catch
            {
                MessageBox.Show("Invalid Level.", _zVirtualScenesMain.ProgramName);
                return false;
            }
            return true;
        }

        private void btn_RunCommand_Click(object sender, EventArgs e)
        {
            if (UpdateMultiLevelSwitchAction())
            {
                ActionResult result = TheAction.Run(_zVirtualScenesMain.ControlThinkController);
                _zVirtualScenesMain.LogThis((int)result.ResultType, "GUI: [USER] " + result.Description);
                lbl_Status.Text = result.ResultType + " " + result.Description;
            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {   
            if (UpdateMultiLevelSwitchAction())
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
