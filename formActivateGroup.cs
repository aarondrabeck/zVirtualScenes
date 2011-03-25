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
    public partial class formActivateGroup : Form
    {
        private formzVirtualScenes _zVirtualScenesMain;

        public formActivateGroup(formzVirtualScenes zVirtualScenesMain)
        {
            InitializeComponent();
            _zVirtualScenesMain = zVirtualScenesMain;            
            

            foreach(string group in _zVirtualScenesMain.groups)            
                 comboBoxGroups.Items.Add(group);

            try { comboBoxGroups.SelectedIndex = 0; }
            catch { }
        }

        private void btn_On_Click(object sender, EventArgs e)
        {
            string selectedgroup = (string)comboBoxGroups.SelectedItem;

            SceneResult result = _zVirtualScenesMain.ActivateGroup(selectedgroup, 99);
            _zVirtualScenesMain.AddLogEntry((UrgencyLevel)result.ResultType, result.Description); 
            
        }

        private void btn_OFF_Click(object sender, EventArgs e)
        {
            string selectedgroup = (string)comboBoxGroups.SelectedItem;

            SceneResult result = _zVirtualScenesMain.ActivateGroup(selectedgroup, 0);
            _zVirtualScenesMain.AddLogEntry((UrgencyLevel)result.ResultType, result.Description); 
        }

        private void button_Set_Click(object sender, EventArgs e)
        {
            string selectedgroup = (string)comboBoxGroups.SelectedItem;

            byte level;
            try
            {
                level = Convert.ToByte(textBoxLevel.Text);
            }
            catch
            {
                MessageBox.Show("Invalid Level", _zVirtualScenesMain.ProductName);
                return;
            }

            foreach (ZWaveDevice device in _zVirtualScenesMain.MasterDevices)
            {
                if (device.Type == ZWaveDevice.ZWaveDeviceTypes.BinarySwitch || device.Type == ZWaveDevice.ZWaveDeviceTypes.MultiLevelSwitch)
                    if (device.GroupName == selectedgroup)
                    {
                        Action action = (Action)device;
                        action.Level = level;
                        action.Run(_zVirtualScenesMain);
                    }
            }


        }

        
    }       
}
