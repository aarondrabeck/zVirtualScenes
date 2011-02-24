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
    public partial class formDeviceProperties : Form
    {
        private formzVirtualScenes _zVirtualScenesMain;
        private int _SelectedDeviceIndex;

        public formDeviceProperties(formzVirtualScenes zVirtualScenesMain, int SelectedDeviceIndex)
        {
            InitializeComponent();
            _zVirtualScenesMain = zVirtualScenesMain;
            _SelectedDeviceIndex = SelectedDeviceIndex;

            //Load up current values
            txtb_deviceName.Text = _zVirtualScenesMain.MasterDevices[_SelectedDeviceIndex].Name;
            groupBoxDevice.Text = "Node: " + _zVirtualScenesMain.MasterDevices[_SelectedDeviceIndex].NodeID + "  -  Name: " + _zVirtualScenesMain.MasterDevices[_SelectedDeviceIndex].Name;

            if (_zVirtualScenesMain.MasterDevices[_SelectedDeviceIndex].SendJabberNotifications == true)
                checkBoxPerDEviceJabberEnable.Checked = true;
            else
                checkBoxPerDEviceJabberEnable.Checked = false;

            if (_zVirtualScenesMain.MasterDevices[_SelectedDeviceIndex].Type.Contains("GeneralThermostat"))
            {
                textBoxMaxAlert.Text = _zVirtualScenesMain.MasterDevices[_SelectedDeviceIndex].MaxAlertTemp.ToString();
                textBoxMinAlert.Text = _zVirtualScenesMain.MasterDevices[_SelectedDeviceIndex].MinAlertTemp.ToString();
                comboBoxJabberNotifLevel.SelectedIndex = _zVirtualScenesMain.MasterDevices[_SelectedDeviceIndex].NotificationDetailLevel - 1;
            }
            else
            {
                textBoxMaxAlert.Enabled = false;
                textBoxMinAlert.Enabled = false;
                comboBoxJabberNotifLevel.Enabled = false;
            }


        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            Device selecteddevice = _zVirtualScenesMain.MasterDevices[_SelectedDeviceIndex];
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

            //TEMP SPECIFIC DEVICES
            if (_zVirtualScenesMain.MasterDevices[_SelectedDeviceIndex].Type.Contains("GeneralThermostat"))
            {
                //MAX Temp            
                int maxtemp;
                try
                {
                    maxtemp = Convert.ToInt32(textBoxMaxAlert.Text);
                }
                catch
                {
                    MessageBox.Show("Max Temp not valid.", _zVirtualScenesMain.ProgramName);
                    return;
                }
                selecteddevice.MaxAlertTemp = maxtemp;

                //Min Temp            
                int mintemp;
                try
                {
                    mintemp = Convert.ToInt32(textBoxMinAlert.Text);
                }
                catch
                {
                    MessageBox.Show("Max Temp not valid.", _zVirtualScenesMain.ProgramName);
                    return;
                }
                selecteddevice.MinAlertTemp = mintemp;

                //Notification Level
                selecteddevice.NotificationDetailLevel = comboBoxJabberNotifLevel.SelectedIndex + 1;
            }

            //Save into custom List for furture use
            bool found = false;
            foreach (CustomDeviceProperties cDevice in _zVirtualScenesMain.CustomDeviceProperties)
            {
                if (selecteddevice.GlbUniqueID() == cDevice.GlbUniqueID())
                {
                    cDevice.Name = selecteddevice.Name;
                    cDevice.SendJabberNotifications = selecteddevice.SendJabberNotifications;
                    cDevice.MaxAlertTemp = selecteddevice.MaxAlertTemp;
                    cDevice.MinAlertTemp = selecteddevice.MinAlertTemp;
                    cDevice.NotificationDetailLevel = selecteddevice.NotificationDetailLevel;
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
                newcDevice.MaxAlertTemp = selecteddevice.MaxAlertTemp;
                newcDevice.MinAlertTemp = selecteddevice.MinAlertTemp;
                newcDevice.NotificationDetailLevel = selecteddevice.NotificationDetailLevel;

                _zVirtualScenesMain.CustomDeviceProperties.Add(newcDevice);
            }

            this.Close();
        }
    }       
}
