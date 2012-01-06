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
        private long device_id; 
        public uc_device_basic()
        {
            InitializeComponent();            
        }

        public void UpdateControl(long device_id)
        {
            this.device_id = device_id;
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                device device = db.devices.FirstOrDefault(d => d.id == device_id);
                if (device != null)
                    textBoxName.Text = device.friendly_name;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //Object Name
            if (String.IsNullOrEmpty(textBoxName.Text))
                MessageBox.Show("Invalid Object Name", zvsEntityControl.zvsNameAndVersion);
            else
            {
                using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                {
                    device d = db.devices.FirstOrDefault(o => o.id == device_id);
                    if (d != null)
                    {
                        d.friendly_name = textBoxName.Text;
                        db.SaveChanges();

                        //Call event
                        zvsEntityControl.CallDeviceModified(this, "friendly_name");                        
                    }
                }
            }            
        }

        
    }
}
