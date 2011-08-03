using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using zVirtualScenesApplication.Structs;
using zVirtualScenesAPI;

namespace zVirtualScenesApplication
{
    public partial class formPropertiesScene : Form
    {
        public Scene _scene;

        public formPropertiesScene()
        {
            InitializeComponent();
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.formSceneProperties_Close);            
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {           
            //HANDLE Scene NAME CHANGE
            if (!string.IsNullOrEmpty(txtb_sceneName.Text))
                API.Scenes.UpdateName(_scene.id, txtb_sceneName.Text);
            else
            {
                MessageBox.Show("Invalid scene name.", API.GetProgramNameAndVersion);
                return;
            }

            //_scene.ShowInLightSwitchGUI = checkBoxDisplayinLightSwitch.Checked; 

            ////Global Hotkey
            //_scene.GlobalHotKey = (int)Enum.Parse(typeof(zVirtualScenesApplication.formzVirtualScenes.CustomHotKeys), comboBoxHotKeys.SelectedValue.ToString());

            ////NOAA
            //_scene.ActivateAtSunrise = checkBoxSunrise.Checked;
            //_scene.ActivateAtSunset = checkBoxSunset.Checked;

            this.Close();
        }

        //public void SetGlobalHotKey(string HotKeyString)
        //{
        //    comboBoxHotKeys.SelectedIndex = (int)Enum.Parse(typeof(zVirtualScenesApplication.formzVirtualScenes.CustomHotKeys), HotKeyString);
        //}

        private void formSceneProperties_Close(object sender, EventArgs e)
        {
        }

        private void formSceneProperties_Load(object sender, EventArgs e)
        {

            txtb_sceneName.Text = _scene.txt_name;
            //comboBoxHotKeys.DataSource = Enum.GetNames(typeof(zVirtualScenesApplication.formzVirtualScenes.CustomHotKeys));
            //comboBoxHotKeys.SelectedIndex = _scene.GlobalHotKey;
            //checkBoxDisplayinLightSwitch.Checked = _scene.ShowInLightSwitchGUI;

            ////NOAA
            //checkBoxSunrise.Checked =  _scene.ActivateAtSunrise;
            //checkBoxSunset.Checked = _scene.ActivateAtSunset;

            ActiveControl = txtb_sceneName;
        }

        private void txtb_sceneName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btn_Save_Click((object)sender, (EventArgs)e);
            }
        }
    }       
}
