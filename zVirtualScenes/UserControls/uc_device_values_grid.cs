using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using zVirtualScenesApplication.Structs;
using zVirtualScenesAPI;
using zVirtualScenesCommon.Entity;
using System.Linq;

namespace zVirtualScenesApplication.UserControls
{
    public partial class uc_device_values_grid : UserControl
    {
        public uc_device_values_grid()
        {
            InitializeComponent();
        }

        public void Clear()
        {
            dataListViewStates.DataSource = null;
            dataListViewStates.ClearObjects();
        }

        public void UpdateControl(long device_id)
        {
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                device device = db.devices.FirstOrDefault(d => d.id == device_id);

                if(device!=null)
                    dataListViewStates.DataSource = device.device_values.ToList();
            }
        }

        
    }    
}
