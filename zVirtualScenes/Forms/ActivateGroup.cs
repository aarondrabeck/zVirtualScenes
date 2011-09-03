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
            ObjectQuery<group> groupQuery = zvsEntityControl.zvsContext.groups;
            comboBoxGroups.DataSource = groupQuery.Execute(MergeOption.AppendOnly);
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
                    builtin_commands group_on_cmd = zvsEntityControl.zvsContext.builtin_commands.SingleOrDefault(c => c.name == "GROUP_ON");
                    builtin_command_que cmd = builtin_command_que.Createbuiltin_command_que(0, g.id.ToString(), group_on_cmd.id);
                    builtin_command_que.Run(cmd);    
                }
            }
            else
                MessageBox.Show("No Group Selected!", zvsEntityControl.GetProgramNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void buttonOff_Click(object sender, EventArgs e)
        {
            if (comboBoxGroups.SelectedIndex > -1)
            {
                group g = (group)comboBoxGroups.SelectedItem;
                if (g != null)
                {
                    builtin_commands group_on_cmd = zvsEntityControl.zvsContext.builtin_commands.SingleOrDefault(c => c.name == "GROUP_OFF");
                    builtin_command_que cmd = builtin_command_que.Createbuiltin_command_que(0, g.id.ToString(), group_on_cmd.id);
                    builtin_command_que.Run(cmd);                        
                }
            }
            else
                MessageBox.Show("No Group Selected!", zvsEntityControl.GetProgramNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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

