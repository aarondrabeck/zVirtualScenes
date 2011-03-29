using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace zVirtualScenesApplication
{
    public partial class uc_setting_lightswitch : UserControl
    {
        private formzVirtualScenes formzVirtualScenesMain;

        public uc_setting_lightswitch()
        {
            InitializeComponent();
            textBoxLSLimit.LostFocus += new EventHandler(textBoxLSLimit_LostFocus);
            textBoxLSpwd.LostFocus += new EventHandler(textBoxLSpwd_LostFocus);
            textBoxPort.LostFocus += new EventHandler(textBoxPort_LostFocus);
        }

        public void LoadSettings(formzVirtualScenes form)
        {
            this.formzVirtualScenesMain = form;

            //LightSwitch
            textBoxLSLimit.Text = Convert.ToString(formzVirtualScenesMain.zVScenesSettings.LightSwitchMaxConnections);
            textBoxLSpwd.Text = Convert.ToString(formzVirtualScenesMain.zVScenesSettings.LightSwitchPassword);
            textBoxPort.Text = Convert.ToString(formzVirtualScenesMain.zVScenesSettings.LightSwitchPort);
            checkBoxLSEnabled.Checked = formzVirtualScenesMain.zVScenesSettings.LightSwitchEnabled;
            checkBoxLSDebugVerbose.Checked = formzVirtualScenesMain.zVScenesSettings.LightSwitchVerbose;
            checkBoxLSSortDevices.Checked = formzVirtualScenesMain.zVScenesSettings.LightSwitchSortDeviceList;
           
        }

        private void textBoxLSLimit_LostFocus(object sender, EventArgs e)
        {
            try
            {
                formzVirtualScenesMain.zVScenesSettings.LightSwitchMaxConnections = Convert.ToInt32(textBoxLSLimit.Text);
                restartserver();  
            }
            catch
            {
                textBoxLSLimit.Text = formzVirtualScenesMain.zVScenesSettings.LightSwitchMaxConnections.ToString(); 
                MessageBox.Show("Invalid LightSwitch Max Connections.", formzVirtualScenesMain.ProgramName);
            }
        }

        private void textBoxLSpwd_LostFocus(object sender, EventArgs e)
        {
            formzVirtualScenesMain.zVScenesSettings.LightSwitchPassword = textBoxLSpwd.Text;
        }

        private void textBoxPort_LostFocus(object sender, EventArgs e)
        {
            try
            {
                formzVirtualScenesMain.zVScenesSettings.LightSwitchPort = Convert.ToInt32(textBoxPort.Text);
                restartserver();
            }
            catch
            {                
                MessageBox.Show("Invalid LightSwitch Port.", formzVirtualScenesMain.ProgramName);
                textBoxPort.Text = formzVirtualScenesMain.zVScenesSettings.LightSwitchPort.ToString();
            }
        }

        private void checkBox_HideLSPassword_CheckedChanged(object sender, EventArgs e)
        {
            textBoxLSpwd.UseSystemPasswordChar = checkBox_HideLSPassword.Checked;
        }

        private void checkBoxLSSortDevices_CheckedChanged(object sender, EventArgs e)
        {
            formzVirtualScenesMain.zVScenesSettings.LightSwitchSortDeviceList = checkBoxLSSortDevices.Checked;
        }

        private void checkBoxLSDebugVerbose_CheckedChanged(object sender, EventArgs e)
        {
            formzVirtualScenesMain.zVScenesSettings.LightSwitchVerbose = checkBoxLSDebugVerbose.Checked;
        }

        private void checkBoxLSEnabled_CheckedChanged(object sender, EventArgs e)
        {
            formzVirtualScenesMain.zVScenesSettings.LightSwitchEnabled = checkBoxLSEnabled.Checked;
            restartserver();           
        }

        private void restartserver()
        {
            if (checkBoxLSEnabled.Checked)
                formzVirtualScenesMain.LightSwitchInt.OpenLightSwitchSocket();
            else
                formzVirtualScenesMain.LightSwitchInt.CloseLightSwitchSocket();
        } 
    }
}
