using System;
using System.Data;
using System.Windows.Forms;
using zVirtualScenesAPI;
using zVirtualScenesCommon.Util;
using System.Drawing;
using System.Collections.Generic;
using zVirtualScenesApplication.Globals;
using zVirtualScenesCommon;
using System.Data.Entity;
using System.Linq;
using zVirtualScenesCommon.Entity;

namespace zVirtualScenesApplication.UserControls
{
    public partial class uc_general_properties : UserControl
    {
        public uc_general_properties()
        {
            InitializeComponent();
        }

        private void uc_general_properties_Load(object sender, EventArgs e)
        {
            lbl_SettingsHeader.Text = string.Format("{0} Settings", zvsEntityControl.zvsNameAndVersion);
            txtb_Degree.Text = program_options.GetProgramOption("TempAbbreviation");

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtb_Degree.Text))
            {
                MessageBox.Show("Invalid temperature unit abbreviation.", zvsEntityControl.zvsNameAndVersion);
                return;
            }

            program_options.DefineOrUpdateProgramOption(new program_options { name = "TempAbbreviation", value = txtb_Degree.Text } );
        }
    }
}
