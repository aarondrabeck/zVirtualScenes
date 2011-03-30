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
    public partial class uc_setting_xmlsocket : UserControl
    {
        private formzVirtualScenes formzVirtualScenesMain;

        public uc_setting_xmlsocket()
        {
            InitializeComponent();
        }

        public void LoadSettings(formzVirtualScenes form)
        {
            this.formzVirtualScenesMain = form;

            //XML Socket
            checkBoxEnableSocketInt.Checked = formzVirtualScenesMain.zVScenesSettings.XMLSocketEnabled;
            checkBoxSocketVerbose.Checked = formzVirtualScenesMain.zVScenesSettings.XMLSocketVerbose;
            textBoxSocketListenPort.Text = formzVirtualScenesMain.zVScenesSettings.XMLSocketPort.ToString();
            textBoxSocketConnectionLimit.Text = formzVirtualScenesMain.zVScenesSettings.XMLSocketMaxConnections.ToString();
            checkBoxAllowiViewer.Checked = formzVirtualScenesMain.zVScenesSettings.XMLSocketAllowiViewer;
            checkBoxAllowAndroid.Checked = formzVirtualScenesMain.zVScenesSettings.XMLSocketAllowAndroid;
            textBoxAndroidPassword.Text = formzVirtualScenesMain.zVScenesSettings.XMLSocketAndroidPassword;     
        }

        private void checkBoxHideAndroidPassword_CheckedChanged(object sender, EventArgs e)
        {
            textBoxAndroidPassword.UseSystemPasswordChar = checkBoxHideAndroidPassword.Checked;
        }

        private void restartServer()
        {
            formzVirtualScenesMain.SocketInt.StopListening();

            if (checkBoxEnableSocketInt.Checked)
                formzVirtualScenesMain.SocketInt.StartListening();
            else
                formzVirtualScenesMain.SocketInt.StopListening();
        }

        private void checkBoxEnableSocketInt_Leave(object sender, EventArgs e)
        {
            formzVirtualScenesMain.zVScenesSettings.XMLSocketEnabled = checkBoxEnableSocketInt.Checked;

            if (checkBoxEnableSocketInt.Checked)
                formzVirtualScenesMain.SocketInt.StartListening();
            else
                formzVirtualScenesMain.SocketInt.StopListening();
        }

        private void checkBoxSocketVerbose_Leave(object sender, EventArgs e)
        {
            formzVirtualScenesMain.zVScenesSettings.XMLSocketVerbose = checkBoxSocketVerbose.Checked;
        }

        private void textBoxSocketListenPort_Leave(object sender, EventArgs e)
        {
            try
            {
                formzVirtualScenesMain.zVScenesSettings.XMLSocketPort = Convert.ToInt32(textBoxSocketListenPort.Text);
                restartServer();
            }
            catch
            {
                textBoxSocketListenPort.Text = formzVirtualScenesMain.zVScenesSettings.XMLSocketPort.ToString();
                MessageBox.Show("Invalid XML Socket Port.", formzVirtualScenesMain.ProgramName);
            }
        }

        private void textBoxSocketConnectionLimit_Leave(object sender, EventArgs e)
        {
            try
            {
                formzVirtualScenesMain.zVScenesSettings.XMLSocketMaxConnections = Convert.ToInt32(textBoxSocketConnectionLimit.Text);
                restartServer();
            }
            catch
            {
                textBoxSocketConnectionLimit.Text = formzVirtualScenesMain.zVScenesSettings.XMLSocketMaxConnections.ToString();
                MessageBox.Show("Invalid XML Socket Max Connections.", formzVirtualScenesMain.ProgramName);
            }
        }

        private void checkBoxAllowiViewer_Leave(object sender, EventArgs e)
        {
            formzVirtualScenesMain.zVScenesSettings.XMLSocketAllowiViewer = checkBoxAllowiViewer.Checked;
        }

        private void checkBoxAllowAndroid_Leave(object sender, EventArgs e)
        {
            formzVirtualScenesMain.zVScenesSettings.XMLSocketAllowAndroid = checkBoxAllowAndroid.Checked;
        }

        private void textBoxAndroidPassword_Leave(object sender, EventArgs e)
        {
            formzVirtualScenesMain.zVScenesSettings.XMLSocketAndroidPassword = textBoxAndroidPassword.Text;
        }
    }
}
