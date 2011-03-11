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
        private int _SelectedSceneIndex;
        private int _SelectedSceneActionIndex;
        Action TheAction = new Action();
        private bool _CreateAction;

        public formAddEditEXEC(formzVirtualScenes zVirtualScenesMain, int SelectedSceneIndex, int SelectedSceneActionIndex, bool CreateAction = false)
        {
            InitializeComponent();

            _zVirtualScenesMain = zVirtualScenesMain;
            _SelectedSceneIndex = SelectedSceneIndex;
            _SelectedSceneActionIndex = SelectedSceneActionIndex;
            _CreateAction = CreateAction;

            if (!_CreateAction)
            {
                groupBoxAction.Text = "Edit Action";
                btn_Save.Text = "Save Action";
                TheAction = _zVirtualScenesMain.MasterScenes[SelectedSceneIndex].Actions[SelectedSceneActionIndex];
            }
            else
            {
                groupBoxAction.Text = "Create New Action";
                btn_Save.Text = "Add Action";
            }
            
            txtb_path.Text = TheAction.EXEPath;
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
            TheAction.Name = "Launch App";
            TheAction.Type = Action.ActionTypes.LauchAPP;
            TheAction.EXEPath = txtb_path.Text;                   

            //SAVE
            if (!_CreateAction) //replace action
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
    }       
}
