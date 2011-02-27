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
            

            comboBoxHotKeys.DataSource = Enum.GetNames(typeof(zVirtualScenesApplication.formzVirtualScenes.CustomHotKeys));
            comboBoxHotKeys.SelectedIndex = _zVirtualScenesMain.MasterScenes[_SelectedSceneIndex].GlobalHotKey;
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

            //Global Hotkey
            selectedscene.GlobalHotKey = (int)Enum.Parse(typeof(zVirtualScenesApplication.formzVirtualScenes.CustomHotKeys), comboBoxHotKeys.SelectedValue.ToString());

            this.Close();
        }

        public void SetGlobalHotKey(string HotKeyString)
        {
            comboBoxHotKeys.SelectedIndex = (int)Enum.Parse(typeof(zVirtualScenesApplication.formzVirtualScenes.CustomHotKeys), HotKeyString);
        }
    }       
}
