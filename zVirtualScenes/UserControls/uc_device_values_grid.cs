using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using zVirtualScenesApplication.Structs;
using zVirtualScenesAPI;
using zVirtualScenesCommon.Entity;

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

        public void UpdateControl(device d)
        {
            dataListViewStates.DataSource = d.device_values; 
        }

        
    }    
}
