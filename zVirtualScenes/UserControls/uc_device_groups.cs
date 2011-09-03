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

        public void UpdateControl(device d)
        {
            listBoxGroups.Items.Clear();

            foreach (group_devices gd in d.group_devices)
            {
                listBoxGroups.Items.Add(gd.group.name);
            }
        }
    }
}
