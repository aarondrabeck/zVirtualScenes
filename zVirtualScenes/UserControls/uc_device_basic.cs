using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using zVirtualScenesAPI;
using zVirtualScenesCommon.Entity;

namespace zVirtualScenesApplication.UserControls
{
    public partial class uc_device_basic : UserControl
    {
        private device _d;

        public uc_device_basic()
        {
            InitializeComponent();
            
        }

        public void UpdateControl(device d)
        {            
            textBoxName.Text = d.friendly_name;
            _d = d;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_d != null)
            {
                //Object Name
                if (String.IsNullOrEmpty(textBoxName.Text))
                    MessageBox.Show("Invalid Object Name", zvsEntityControl.zvsNameAndVersion);
                else
                {
                    _d.friendly_name = textBoxName.Text;
                    zvsEntityControl.zvsContext.SaveChanges();
                }
            }
        }
    }
}
