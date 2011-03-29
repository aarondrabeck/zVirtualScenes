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

            textBoxJabberPassword.LostFocus += new EventHandler(textBoxJabberPassword_LostFocus);
            textBoxJabberUser.LostFocus += new EventHandler(textBoxJabberUser_LostFocus);
            textBoxJabberServer.LostFocus += new EventHandler(textBoxJabberServer_LostFocus);
            textBoxJabberUserTo.LostFocus += new EventHandler(textBoxJabberUserTo_LostFocus);
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

        private void textBoxJabberPassword_LostFocus(object sender, EventArgs e)
        {
            formzVirtualScenesMain.zVScenesSettings.JabberPassword = textBoxJabberPassword.Text;
            restartServer();    
        }

        private void textBoxJabberUser_LostFocus(object sender, EventArgs e)
        {
            formzVirtualScenesMain.zVScenesSettings.JabberUser = textBoxJabberUser.Text;
            restartServer();    
        }

        private void textBoxJabberServer_LostFocus(object sender, EventArgs e)
        {
            formzVirtualScenesMain.zVScenesSettings.JabberServer = textBoxJabberServer.Text;
            restartServer();    
        }

        private void textBoxJabberUserTo_LostFocus(object sender, EventArgs e)
        {
            formzVirtualScenesMain.zVScenesSettings.JabberSendToUser = textBoxJabberUserTo.Text;
        }

        private void checkBoxJabberEnabled_CheckedChanged(object sender, EventArgs e)
        {
            formzVirtualScenesMain.zVScenesSettings.JabberEnanbled = checkBoxJabberEnabled.Checked;
            restartServer();            
        }

        private void checkBoxJabberVerbose_CheckedChanged(object sender, EventArgs e)
        {
            formzVirtualScenesMain.zVScenesSettings.JabberVerbose = checkBoxJabberVerbose.Checked;
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
    }
}
