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
    public partial class formPropertiesScene : Form
    {
        public bool isOpen { get; set; }
        public formzVirtualScenes _zVirtualScenesMain;
        public Scene _SceneToEdit;

        public formPropertiesScene()
        {
            InitializeComponent();
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.formSceneProperties_Close);
            this.isOpen = false;            
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {           
            //HANDLE Scene NAME CHANGE
            if (txtb_sceneName.Text != "")
                _SceneToEdit.Name = txtb_sceneName.Text;            
            else
            {
                MessageBox.Show("Invalid scene name.", _zVirtualScenesMain.ProgramName);
                return;
            }

            _SceneToEdit.ShowInLightSwitchGUI = checkBoxDisplayinLightSwitch.Checked; 

            //Global Hotkey
            _SceneToEdit.GlobalHotKey = (int)Enum.Parse(typeof(zVirtualScenesApplication.formzVirtualScenes.CustomHotKeys), comboBoxHotKeys.SelectedValue.ToString());

            //NOAA
            _SceneToEdit.ActivateAtSunrise = checkBoxSunrise.Checked;
            _SceneToEdit.ActivateAtSunset = checkBoxSunset.Checked;

            this.Close();
        }

        public void SetGlobalHotKey(string HotKeyString)
        {
            comboBoxHotKeys.SelectedIndex = (int)Enum.Parse(typeof(zVirtualScenesApplication.formzVirtualScenes.CustomHotKeys), HotKeyString);
        }

        private void formSceneProperties_Close(object sender, EventArgs e)
        {
            this.isOpen = false;
        }

        private void formSceneProperties_Load(object sender, EventArgs e)
        {
            this.isOpen = true;

            txtb_sceneName.Text = _SceneToEdit.Name;
            comboBoxHotKeys.DataSource = Enum.GetNames(typeof(zVirtualScenesApplication.formzVirtualScenes.CustomHotKeys));
            comboBoxHotKeys.SelectedIndex = _SceneToEdit.GlobalHotKey;
            checkBoxDisplayinLightSwitch.Checked = _SceneToEdit.ShowInLightSwitchGUI;

            //NOAA
            checkBoxSunrise.Checked =  _SceneToEdit.ActivateAtSunrise;
            checkBoxSunset.Checked = _SceneToEdit.ActivateAtSunset;
        }
    }       
}
