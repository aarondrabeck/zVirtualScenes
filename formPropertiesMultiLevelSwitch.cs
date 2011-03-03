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
    public partial class formPropertiesMultiLevelSwitch : Form
    {
        private formzVirtualScenes _zVirtualScenesMain;
        private int _SelectedDeviceIndex;

        public formPropertiesMultiLevelSwitch(formzVirtualScenes zVirtualScenesMain, int SelectedDeviceIndex, int SelectedSceneIndex)
        {
            InitializeComponent();

            _zVirtualScenesMain = zVirtualScenesMain;
            _SelectedDeviceIndex = SelectedDeviceIndex;

            #region Load Common Feilds into form fields
            groupBoxDeviceOptions.Text = "Node " + _zVirtualScenesMain.MasterDevices[_SelectedDeviceIndex].NodeID + ",  '" + _zVirtualScenesMain.MasterDevices[_SelectedDeviceIndex].Name + "' Options";
            txtb_deviceName.Text = _zVirtualScenesMain.MasterDevices[_SelectedDeviceIndex].Name;

            if (_zVirtualScenesMain.MasterDevices[_SelectedDeviceIndex].SendJabberNotifications == true)
                checkBoxPerDEviceJabberEnable.Checked = true;
            else
                checkBoxPerDEviceJabberEnable.Checked = false;

            #endregion           
        }
 
        private void btn_SaveOptions_Click(object sender, EventArgs e)
        {
            Device selecteddevice = _zVirtualScenesMain.MasterDevices[_SelectedDeviceIndex];

            #region Validate and Save Common Feilds into MasterDevices list
            //HANDLE DEVICE NAME CHANGE
            if (txtb_deviceName.Text != "")
            {
                selecteddevice.Name = txtb_deviceName.Text;

                //Replace name in each Action
                foreach (Scene scene in _zVirtualScenesMain.MasterScenes)
                {
                    foreach (Action action in scene.Actions)
                    {
                        if (action.GlbUniqueID() == selecteddevice.GlbUniqueID())
                            action.Name = selecteddevice.Name;
                    }
                }
            }
            else
            {
                MessageBox.Show("Invalid device name.", _zVirtualScenesMain.ProgramName);
                return;
            }

            //Jabber Notifications
            if (checkBoxPerDEviceJabberEnable.Checked == true)
                _zVirtualScenesMain.MasterDevices[_SelectedDeviceIndex].SendJabberNotifications = true;
            else
                _zVirtualScenesMain.MasterDevices[_SelectedDeviceIndex].SendJabberNotifications = false;
            #endregion

            #region Save into CustomDeviceProperties List which gets serialized.
            bool found = false;
            foreach (CustomDeviceProperties cDevice in _zVirtualScenesMain.CustomDeviceProperties)
            {
                if (selecteddevice.GlbUniqueID() == cDevice.GlbUniqueID())
                {
                    cDevice.Name = selecteddevice.Name;
                    cDevice.SendJabberNotifications = selecteddevice.SendJabberNotifications;
                    found = true;
                }
            }
            if (!found)
            {
                CustomDeviceProperties newcDevice = new CustomDeviceProperties();
                newcDevice.HomeID = selecteddevice.HomeID;
                newcDevice.NodeID = selecteddevice.NodeID;
                newcDevice.Name = selecteddevice.Name;
                newcDevice.SendJabberNotifications = selecteddevice.SendJabberNotifications;
                _zVirtualScenesMain.CustomDeviceProperties.Add(newcDevice);
            }

            #endregion

            this.Close();
        }
       
    }       
}
