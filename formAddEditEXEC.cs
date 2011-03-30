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
    public partial class formAddEditEXEC : Form
    {
        private formzVirtualScenes _zVirtualScenesMain;
        private Scene theScene;
        private Action theAction;
        private int InsertPosition;
        private bool EditMode;
        private int sceneIndex;

        public formAddEditEXEC(formzVirtualScenes zVirtualScenesMain, Scene scene, Action action)
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

        public formAddEditEXEC(formzVirtualScenes zVirtualScenesMain, Scene scene, int PositionOfNewItem)
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
            txtb_path.Text = theAction.EXEPath;
            checkBoxSkipDark.Checked = theAction.SkipWhenDark;
            checkBoxSkipLight.Checked = theAction.SkipWhenLight;
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            //VALIDATE ENTRY            
            if (txtb_path.Text == "")
            {
                MessageBox.Show("Please select a executable.", _zVirtualScenesMain.ProgramName);
                return;
            }            

            //CREATE ACTION
            theAction.Name = "Launch App";
            theAction.Type = Action.ActionTypes.LauchAPP;
            theAction.EXEPath = txtb_path.Text;
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

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "Please select a file to run in this action.";
            fdlg.Filter = "All files (*.*)|*.*|All files (*.*)|*.*";
            fdlg.FilterIndex = 2;

            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                Action EXEAction = new Action();
                txtb_path.Text = fdlg.FileName;
            }
        }

        private void formAddEditEXEC_Load(object sender, EventArgs e)
        {
            ActiveControl = buttonBrowse;
        }
    }       
}
