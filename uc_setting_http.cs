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
    public partial class uc_setting_http : UserControl
    {
        private formzVirtualScenes formzVirtualScenesMain;

        public uc_setting_http()
        {
            InitializeComponent();
            txtb_httpPort.LostFocus += new EventHandler(txtb_httpPort_LostFocus);
        }

        public void LoadSettings(formzVirtualScenes form)
        {
            this.formzVirtualScenesMain = form;

            //Http Listen
            txtb_httpPort.Text = Convert.ToString(formzVirtualScenesMain.zVScenesSettings.ZHTTPPort);            
            checkBoxHTTPEnable.Checked = formzVirtualScenesMain.zVScenesSettings.zHTTPListenEnabled;
            txtb_exampleURL.Text = "http://localhost:" + formzVirtualScenesMain.zVScenesSettings.ZHTTPPort + "/zVirtualScene?cmd=RunScene&Scene=1";
        }

        private void txtb_httpPort_LostFocus(object sender, EventArgs e)
        {
            try
            {
                formzVirtualScenesMain.zVScenesSettings.ZHTTPPort = Convert.ToInt32(txtb_httpPort.Text);
                restartserver();
            }
            catch
            {
                txtb_httpPort.Text = formzVirtualScenesMain.zVScenesSettings.ZHTTPPort.ToString();
                MessageBox.Show("Invalid HTTP Port.", formzVirtualScenesMain.ProgramName);
            }
        }

        private void checkBoxHTTPEnable_CheckedChanged(object sender, EventArgs e)
        {
            formzVirtualScenesMain.zVScenesSettings.zHTTPListenEnabled = checkBoxHTTPEnable.Checked;
            restartserver();           
        }

        private void restartserver()
        {
            formzVirtualScenesMain.httpInt.Stop();

            if (checkBoxHTTPEnable.Checked)
                formzVirtualScenesMain.httpInt.Start();
        } 
    }
}
