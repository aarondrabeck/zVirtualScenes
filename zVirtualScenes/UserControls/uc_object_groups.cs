using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using zVirtualScenesAPI;

namespace zVirtualScenesApplication.UserControls
{
    public partial class uc_object_groups : UserControl
    {
        public uc_object_groups()
        {
            InitializeComponent();
        }

        public void UpdateControl(int objId)
        {
            listBoxGroups.Items.Clear();
            
            DataTable dt = API.Groups.GetObjectGroups(objId);

            foreach (DataRow dr in dt.Rows)
            {
                listBoxGroups.Items.Add(dr["txt_group_name"].ToString());
            }
        }
    }
}
