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
        private Scene theScene;
        private Action theAction;
        private int InsertPosition;
        private bool EditMode;
        private int sceneIndex;

        /// <summary>
        /// Edit Multilevel Switch Action
        /// </summary>
        /// <param name="zVirtualScenesMain"></param>
        /// <param name="scene"></param>
        /// <param name="action"></param>
        public formAddEditActionMultiLevelSwitch(formzVirtualScenes zVirtualScenesMain, Scene scene, Action action)
        {
            //Edit Items
            InitializeComponent();
            this._zVirtualScenesMain = zVirtualScenesMain;
            this.theScene = scene;
            this.sceneIndex = _zVirtualScenesMain.MasterScenes.IndexOf(this.theScene);
            this.EditMode = true;

            this.groupBoxAction.Text = "Edit Action";
            this.btn_Save.Text = "Save Action";
            this.theAction = action;
            this.InsertPosition = _zVirtualScenesMain.MasterScenes[sceneIndex].Actions.IndexOf(theAction);

            LoadGui();
        }
        
        /// <summary>
        /// Create New Multilevel Switch Action
        /// </summary>
        /// <param name="zVirtualScenesMain"></param>
        /// <param name="scene"></param>
        /// <param name="device"></param>
        /// <param name="PositionOfNewItem"></param>
        public formAddEditActionMultiLevelSwitch(formzVirtualScenes zVirtualScenesMain, Scene scene, ZWaveDevice device, int PositionOfNewItem)
        {
            //Add Items
            InitializeComponent();
            this._zVirtualScenesMain = zVirtualScenesMain;
            this.theScene = scene;
            this.sceneIndex = _zVirtualScenesMain.MasterScenes.IndexOf(this.theScene);
            this.EditMode = false;
            
            this.groupBoxAction.Text = "Create New Action";
            this.btn_Save.Text = "Add Action to '" + scene.Name + "'";
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

            #region MultiLevel Switch Specific Fields
            txtbox_level.Text = theAction.Level.ToString();
            #endregion
        } 
         
        private bool UpdateMultiLevelSwitchAction()
        {            
            //ERROR CHECK INPUTS
            theAction.SkipWhenDark = checkBoxSkipDark.Checked;
            theAction.SkipWhenLight = checkBoxSkipLight.Checked;
            try
            {
                byte level = Convert.ToByte(txtbox_level.Text);
                if (level < 100 || level == 255)
                    theAction.Level = level;
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

            if (UpdateMultiLevelSwitchAction())
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
                this.Close();
            }  
        }

    }       
}
