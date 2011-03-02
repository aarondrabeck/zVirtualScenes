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
    public partial class formCreateTimeDelay : Form
    {
        private formzVirtualScenes _zVirtualScenesMain;
        private int _SelectedSceneIndex;

        public formCreateTimeDelay(formzVirtualScenes zVirtualScenesMain, int SelectedSceneIndex)
        {
            InitializeComponent();
            _zVirtualScenesMain = zVirtualScenesMain;
            _SelectedSceneIndex = SelectedSceneIndex;            
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            Scene selectedscene = _zVirtualScenesMain.MasterScenes[_SelectedSceneIndex];
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
            
            Action delayAction = new Action();
            delayAction.Type = "DelayTimer";
            delayAction.TimerDuration = duration * 1000;
            _zVirtualScenesMain.MasterScenes[_SelectedSceneIndex].Actions.Add(delayAction);

            this.Close();
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

    }       
}
