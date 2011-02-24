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
    public partial class formSceneProperties : Form
    {
        private formzVirtualScenes _zVirtualScenesMain;
        private int _SelectedSceneIndex;

        public formSceneProperties(formzVirtualScenes zVirtualScenesMain, int SelectedSceneIndex)
        {
            InitializeComponent();
            _zVirtualScenesMain = zVirtualScenesMain;
            _SelectedSceneIndex = SelectedSceneIndex;

            txtb_sceneName.Text = _zVirtualScenesMain.MasterScenes[_SelectedSceneIndex].Name;  
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            Scene selectedscene = _zVirtualScenesMain.MasterScenes[_SelectedSceneIndex];
            //HANDLE Scene NAME CHANGE
            if (txtb_sceneName.Text != "")            
                selectedscene.Name = txtb_sceneName.Text;            
            else
            {
                MessageBox.Show("Invalid scene name.", _zVirtualScenesMain.ProgramName);
                return;
            }
            this.Close();
        }
    }       
}
