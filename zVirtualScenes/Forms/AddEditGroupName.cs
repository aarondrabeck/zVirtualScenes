using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using zVirtualScenesAPI;
using zVirtualScenesCommon.Entity;

namespace zVirtualScenesApplication.Forms
{
    public partial class AddEditGroupName : Form
    {
        public string gName; 

        public AddEditGroupName(string GroupName)
        {
            gName = GroupName;
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtbName.Text))
            {
                MessageBox.Show("You must enter a name for the group!", zvsEntityControl.GetProgramNameAndVersion, MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            else
            {
                DialogResult = System.Windows.Forms.DialogResult.OK;
                gName = txtbName.Text;
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
        
        private void txtbName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                btnOK_Click((object)sender, (EventArgs)e);
                e.Handled = true;
            }
        }

        private void AddEditGroupName_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Escape))
            {
                this.Close();
            }
        }
    }
}
