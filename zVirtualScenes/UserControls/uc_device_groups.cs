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
    public partial class uc_device_groups : UserControl
    {
        public uc_device_groups()
        {
            InitializeComponent();
        }

        public void UpdateControl(long device_id)
        {
            listBoxGroups.Items.Clear();

            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                device device = db.devices.FirstOrDefault(d => d.id == device_id);
                if (device != null)                
                    foreach (group_devices gd in device.group_devices)                    
                        listBoxGroups.Items.Add(gd.group.name);
            }
        }
    }
}
