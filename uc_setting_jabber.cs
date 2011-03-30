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
    public partial class uc_setting_jabber : UserControl
    {
        private formzVirtualScenes formzVirtualScenesMain;

        public uc_setting_jabber()
        { 
            InitializeComponent();
        }

        public void LoadSettings(formzVirtualScenes form)
        {
            this.formzVirtualScenesMain = form;

            //JAbber
            textBoxJabberPassword.Text = formzVirtualScenesMain.zVScenesSettings.JabberPassword;
            textBoxJabberUser.Text = formzVirtualScenesMain.zVScenesSettings.JabberUser;
            textBoxJabberServer.Text = formzVirtualScenesMain.zVScenesSettings.JabberServer;
            textBoxJabberUserTo.Text = formzVirtualScenesMain.zVScenesSettings.JabberSendToUser;
            checkBoxJabberEnabled.Checked = formzVirtualScenesMain.zVScenesSettings.JabberEnanbled;
            checkBoxJabberVerbose.Checked = formzVirtualScenesMain.zVScenesSettings.JabberVerbose;          
        }

        private void checkBox_HideJabberPassword_CheckedChanged(object sender, EventArgs e)
        {
            textBoxJabberPassword.UseSystemPasswordChar = checkBox_HideJabberPassword.Checked;
        }

        private void restartServer()
        {
            if (checkBoxJabberEnabled.Checked)
            {
                formzVirtualScenesMain.jabber.Connect();
            }
            else
            {
                formzVirtualScenesMain.jabber.Disconnect();
            }
        }

        private void checkBoxJabberEnabled_Leave(object sender, EventArgs e)
        {
            formzVirtualScenesMain.zVScenesSettings.JabberEnanbled = checkBoxJabberEnabled.Checked;

            if (checkBoxJabberEnabled.Checked)
            {
                formzVirtualScenesMain.jabber.Connect();
            }
            else
            {
                formzVirtualScenesMain.jabber.Disconnect();
            }
        }

        private void checkBoxJabberVerbose_Leave(object sender, EventArgs e)
        {
            formzVirtualScenesMain.zVScenesSettings.JabberVerbose = checkBoxJabberVerbose.Checked;
        }

        private void textBoxJabberUser_Leave(object sender, EventArgs e)
        {
            formzVirtualScenesMain.zVScenesSettings.JabberUser = textBoxJabberUser.Text;
            restartServer();    
        }

        private void textBoxJabberPassword_Leave(object sender, EventArgs e)
        {
            formzVirtualScenesMain.zVScenesSettings.JabberPassword = textBoxJabberPassword.Text;
            restartServer();  
        }

        private void textBoxJabberServer_Leave(object sender, EventArgs e)
        {
            formzVirtualScenesMain.zVScenesSettings.JabberServer = textBoxJabberServer.Text;
            restartServer();   
        }

        private void textBoxJabberUserTo_Leave(object sender, EventArgs e)
        {
            formzVirtualScenesMain.zVScenesSettings.JabberSendToUser = textBoxJabberUserTo.Text;
        }
    }
}
