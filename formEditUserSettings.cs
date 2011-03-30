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
    public partial class formEditUserSettings : Form
    {
        public formEditUserSettings(formzVirtualScenes zVirtualScenesMain)
        {
            InitializeComponent();

            uc_setting_general.LoadSettings(zVirtualScenesMain);
            uc_setting_jabber1.LoadSettings(zVirtualScenesMain);
            uc_setting_lightswitch1.LoadSettings(zVirtualScenesMain);
            uc_setting_http1.LoadSettings(zVirtualScenesMain);
            uc_setting_xmlsocket1.LoadSettings(zVirtualScenesMain);

            listBoxCategory.SelectedIndex = 0;       
        }

        private void HideSettingUserControl()
        {
            uc_setting_general.Visible = false;
            uc_setting_jabber1.Visible = false;
            uc_setting_lightswitch1.Visible = false;
            uc_setting_http1.Visible = false;
            uc_setting_xmlsocket1.Visible = false;
        }

        private void listBoxCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            HideSettingUserControl();

            if (listBoxCategory.SelectedItem.Equals(" General"))
            {
                uc_setting_general.Visible = true;
            }
            else if (listBoxCategory.SelectedItem.Equals(" Jabber"))
            {
                uc_setting_jabber1.Visible = true;
            }
            else if (listBoxCategory.SelectedItem.Equals(" Light Switch"))
            {
                uc_setting_lightswitch1.Visible = true;
            }
            else if (listBoxCategory.SelectedItem.Equals(" HTTP"))
            {
                uc_setting_http1.Visible = true;
            }
            else if (listBoxCategory.SelectedItem.Equals(" XML Socket"))
            {
                uc_setting_xmlsocket1.Visible = true;
            }
            
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }        
    }       
}
