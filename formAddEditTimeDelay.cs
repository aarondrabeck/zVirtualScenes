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
        private int _SelectedSceneIndex;
        private int _SelectedSceneActionIndex;
        Action TheAction = new Action();
        private bool _CreateAction;

        public formAddEditTimeDelay(formzVirtualScenes zVirtualScenesMain, int SelectedSceneIndex, int SelectedSceneActionIndex, bool CreateAction = false)
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

            txtb_duration.Text = Convert.ToString(TheAction.TimerDuration / 1000);
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
            TheAction.Type = Action.ActionTypes.DelayTimer;
            TheAction.TimerDuration = duration * 1000;                       

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
    }       
}
