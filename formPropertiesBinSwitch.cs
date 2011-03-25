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
    public partial class formPropertiesBinSwitch : Form
    {
        private formzVirtualScenes _zVirtualScenesMain;
        private ZWaveDevice _DeviceToEdit;

        public formPropertiesBinSwitch(formzVirtualScenes zVirtualScenesMain, ZWaveDevice DeviceToEdit)
        {
            InitializeComponent();
            _zVirtualScenesMain = zVirtualScenesMain;
            _DeviceToEdit = DeviceToEdit;

            #region Load Common Feilds into form fields

            groupBoxDeviceOptions.Text = "Node " + _DeviceToEdit.NodeID + ",  '" + _DeviceToEdit.Name + "' Options";
            txtb_deviceName.Text = _DeviceToEdit.Name;
            txtb_GroupName.Text = _DeviceToEdit.GroupName;

            checkBoxPerDEviceJabberEnable.Checked = _DeviceToEdit.SendJabberNotifications;
            checkBoxDisplayinLightSwitch.Checked = _DeviceToEdit.ShowInLightSwitchGUI;
            checkBoxMomentaryOn.Checked = _DeviceToEdit.MomentaryOnMode;
            checkBoxGrowlEnabled.Checked = _DeviceToEdit.SendGrowlNotifications;
            textBoxMomentaryOnTimeSpan.Text = DeviceToEdit.MomentaryTimespan.ToString();
            txtb_GroupName.AutoCompleteCustomSource = _zVirtualScenesMain.GetGroupsAutoCompleteCollection();

            #endregion

        }

        private void btn_SaveOptions_Click(object sender, EventArgs e)
        {
            #region Validate and Save Common Feilds into MasterDevices list

            //HANDLE DEVICE NAME CHANGE
            if (txtb_deviceName.Text != "")
            {
                _DeviceToEdit.Name = txtb_deviceName.Text;                
            }
            else
            {
                MessageBox.Show("Invalid device name.", _zVirtualScenesMain.ProgramName);
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

            //Momentary Mode
            _DeviceToEdit.MomentaryOnMode = checkBoxMomentaryOn.Checked;            
            if (textBoxMomentaryOnTimeSpan.Text != "")
            {
                _DeviceToEdit.MomentaryTimespan = Convert.ToInt32(textBoxMomentaryOnTimeSpan.Text);
            }
            else
            {
                MessageBox.Show("Momentary on time.", _zVirtualScenesMain.ProgramName);
                return;
            }

            _DeviceToEdit.SendGrowlNotifications = checkBoxGrowlEnabled.Checked;

            //Replace name in each Action
            foreach (Scene scene in _zVirtualScenesMain.MasterScenes)
            {
                foreach (Action action in scene.Actions)
                {
                    if (action.GlbUniqueID() == _DeviceToEdit.GlbUniqueID())
                    {
                        action.Name = _DeviceToEdit.Name;
                        action.MomentaryOnMode = _DeviceToEdit.MomentaryOnMode;
                        action.MomentaryTimespan = _DeviceToEdit.MomentaryTimespan;
                    }
                }
            }

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

    }
}
