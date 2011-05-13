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
    public partial class formPropertiesThermostat : Form
    {
        private formzVirtualScenes _zVirtualScenesMain;
        private ZWaveDevice _DeviceToEdit;

        public formPropertiesThermostat(formzVirtualScenes zVirtualScenesMain, ZWaveDevice DeviceToEdit)
        {
            InitializeComponent();
            _zVirtualScenesMain = zVirtualScenesMain;
            _DeviceToEdit = DeviceToEdit;

            #region Load Common Feilds into form fields

            groupBoxDeviceOptions.Text = "Node " + _DeviceToEdit.NodeID + ",  '" + _DeviceToEdit.Name + "' Options";
            txtb_deviceName.Text = _DeviceToEdit.Name;
            textBoxRepolling.Text = _DeviceToEdit.RepollInterval.ToString();
            txtb_GroupName.Text = _DeviceToEdit.GroupName;

            checkBoxPerDEviceJabberEnable.Checked = _DeviceToEdit.SendJabberNotifications;
            checkBoxDisplayinLightSwitch.Checked = _DeviceToEdit.ShowInLightSwitchGUI;
            checkBoxGrowlEnabled.Checked = _DeviceToEdit.SendGrowlNotifications;
            txtb_GroupName.AutoCompleteCustomSource = _zVirtualScenesMain.GetGroupsAutoCompleteCollection();
            textBoxRepolling.Text = _DeviceToEdit.RepollInterval.ToString();

            #endregion

            #region Thermostat Specific Fields
            textBoxMaxAlert.Text = _DeviceToEdit.MaxAlertTemp.ToString();
            textBoxMinAlert.Text = _DeviceToEdit.MinAlertTemp.ToString();
            comboBoxJabberNotifLevel.SelectedIndex = _DeviceToEdit.NotificationDetailLevel - 1;
            #endregion

        }

        private void btn_SaveOptions_Click(object sender, EventArgs e)
        {            
            #region Validate and Save Common Feilds into MasterDevices list

            //HANDLE DEVICE NAME CHANGE
            if (txtb_deviceName.Text != "")
            {
                _DeviceToEdit.Name = txtb_deviceName.Text;

                //Replace name in each Action
                foreach (Scene scene in _zVirtualScenesMain.MasterScenes)
                {
                    foreach (Action action in scene.Actions)
                    {
                        if (action.GlbUniqueID() == _DeviceToEdit.GlbUniqueID())
                            action.Name = _DeviceToEdit.Name;
                    }
                }
            }
            else
            {
                MessageBox.Show("Invalid device name.", _zVirtualScenesMain.ProgramName);
                return;
            }

            //repoll interval
            if (textBoxRepolling.Text != "" && Convert.ToInt32(textBoxRepolling.Text) >= 0)
            {
                _DeviceToEdit.RepollInterval = Convert.ToInt32(textBoxRepolling.Text);
                _zVirtualScenesMain.ControlThinkInt.UpdatePollingIntervalsAllDevices();
            }
            else
            {
                MessageBox.Show("Invalid interval.", _zVirtualScenesMain.ProgramName);
                return;
            }

            //HANDLE DEVICE GROUP NAME CHNAGE
            if (txtb_GroupName.Text != "")            
                _DeviceToEdit.GroupName = txtb_GroupName.Text;
            else
            {
                MessageBox.Show("Invalid group name.", _zVirtualScenesMain.ProgramName);
                return;
            }
            _zVirtualScenesMain.refreshGroups();

            //Jabber Notifications
            _DeviceToEdit.SendJabberNotifications = checkBoxPerDEviceJabberEnable.Checked;

            //LightSwitch Options
            _DeviceToEdit.ShowInLightSwitchGUI = checkBoxDisplayinLightSwitch.Checked;

            _DeviceToEdit.SendGrowlNotifications = checkBoxGrowlEnabled.Checked;

            #endregion

            #region Validate and Save Thermostat Specific Fields into MasterDevices list
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
            _DeviceToEdit.MaxAlertTemp = maxtemp;

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
            _DeviceToEdit.MinAlertTemp = mintemp;

            //Notification Level
            _DeviceToEdit.NotificationDetailLevel = comboBoxJabberNotifLevel.SelectedIndex + 1;
            #endregion

            #region Save into SavedZWaveDeviceUserSettings List which gets serialized.

            foreach (ZWaveDeviceUserSettings cDevice in _zVirtualScenesMain.SavedZWaveDeviceUserSettings)
            {
                if (_DeviceToEdit.GlbUniqueID() == cDevice.GlbUniqueID())
                {
                    _zVirtualScenesMain.SavedZWaveDeviceUserSettings.Remove(cDevice);
                    break;
                }
            }
            _zVirtualScenesMain.SavedZWaveDeviceUserSettings.Add((ZWaveDeviceUserSettings)_DeviceToEdit);

            #endregion             

            this.Close();
        }

        private void formPropertiesThermostat_Load(object sender, EventArgs e)
        {
            ActiveControl = txtb_deviceName;

        }

        private void txtb_deviceName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btn_SaveOptions_Click((object)sender, (EventArgs)e);
        }

        private void txtb_GroupName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)            
                btn_SaveOptions_Click((object)sender, (EventArgs)e);
            
        }

        private void textBoxRepolling_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar != Keys.Back)
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), "\\d+"))
                    e.Handled = true;
            }
        }

    }
}
