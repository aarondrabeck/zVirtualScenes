using System;
using System.Data;
using System.Windows.Forms;
using System.Data.Objects;
using System.Linq;
using zVirtualScenesAPI;
using zVirtualScenesCommon.Entity;


namespace zVirtualScenesApplication.Forms
{
    public partial class ActivateGroup : Form
    {
        public ActivateGroup()
        {
            InitializeComponent();
        }

        private void GroupEditor_Load(object sender, EventArgs e)
        {
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                comboBoxGroups.DataSource = db.groups.OfType<group>().Execute(MergeOption.AppendOnly);
            }
            comboBoxGroups.DisplayMember = "name";

            //Select the first group if there is one
            if (comboBoxGroups.Items.Count > 0) { comboBoxGroups.SelectedIndex = 0; }
        }
  
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonOn_Click(object sender, EventArgs e)
        {
            if (comboBoxGroups.SelectedIndex > -1)
            {
                group g = (group)comboBoxGroups.SelectedItem;
                if (g != null)
                {
                    using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                    {
                        builtin_commands group_on_cmd = db.builtin_commands.FirstOrDefault(c => c.name == "GROUP_ON");
                        if (group_on_cmd != null)
                            group_on_cmd.Run(g.id.ToString());
                    }
                }
            }
            else
                MessageBox.Show("No Group Selected!", zvsEntityControl.zvsNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void buttonOff_Click(object sender, EventArgs e)
        {
            if (comboBoxGroups.SelectedIndex > -1)
            {
                group g = (group)comboBoxGroups.SelectedItem;
                if (g != null)
                {
                    using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                    {
                        builtin_commands group_off_cmd = db.builtin_commands.FirstOrDefault(c => c.name == "GROUP_OFF");
                        if (group_off_cmd != null)
                            group_off_cmd.Run(g.id.ToString());
                    }
                }
            }
            else
                MessageBox.Show("No Group Selected!", zvsEntityControl.zvsNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void btnClose_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Escape))
            {
                this.Close();
            }
        }

    }
}

