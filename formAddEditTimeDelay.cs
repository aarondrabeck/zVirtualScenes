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
    public partial class formAddEditTimeDelay : Form
    {
        private formzVirtualScenes _zVirtualScenesMain;
        private Scene theScene;
        private Action theAction;
        private int InsertPosition;
        private bool EditMode;
        private int sceneIndex;

        public formAddEditTimeDelay(formzVirtualScenes zVirtualScenesMain, Scene scene, Action action)
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

        public formAddEditTimeDelay(formzVirtualScenes zVirtualScenesMain, Scene scene, int PositionOfNewItem)
        {
            //Add Items
            InitializeComponent();
            this._zVirtualScenesMain = zVirtualScenesMain;
            this.theScene = scene;
            this.sceneIndex = _zVirtualScenesMain.MasterScenes.IndexOf(this.theScene);
            this.EditMode = false;

            this.groupBoxAction.Text = "Create New Action";
            this.btn_Save.Text = "&Add";
            this.theAction = new Action();
            this.InsertPosition = PositionOfNewItem;

            LoadGui();
        }

        private void LoadGui()
        {
            txtb_duration.Text = Convert.ToString(theAction.TimerDuration / 1000);
            checkBoxSkipDark.Checked = theAction.SkipWhenDark;
            checkBoxSkipLight.Checked = theAction.SkipWhenLight;
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            //VALIDATE ENTRY            
            int duration = 0;
            //HANDLE Scene NAME CHANGE
            try
            {
                duration = Convert.ToInt32(txtb_duration.Text);
                if (duration < 0)
                    throw new ArgumentException("Invalid duration.");
            }
            catch
            {
                MessageBox.Show("Invalid duration.", _zVirtualScenesMain.ProgramName);
                return;
            }

            //CREATE ACTION
            theAction.Name = "Delay";
            theAction.Type = Action.ActionTypes.DelayTimer;
            theAction.TimerDuration = duration * 1000;
            theAction.SkipWhenDark = checkBoxSkipDark.Checked;
            theAction.SkipWhenLight = checkBoxSkipLight.Checked;

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

        private void formAddEditTimeDelay_Load(object sender, EventArgs e)
        {
            ActiveControl = txtb_duration;
        }

        private void txtb_duration_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btn_Save_Click((object)sender, (EventArgs)e);
            }
        }
    }       
}
