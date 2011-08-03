using System;
using System.Data;
using System.Windows.Forms;
using zVirtualScenesCommon.DatabaseCommon;
using zVirtualScenesAPI;
using zVirtualScenesAPI.Structs;

namespace zVirtualScenesApplication.Forms
{
    public partial class ActivateGroup : Form
    {
        private DataTable _groups;

        public ActivateGroup()
        {
            InitializeComponent();
        }

        private void GroupEditor_Load(object sender, EventArgs e)
        {
            UpdateGroupsList();

            //Select the first item if there is one
            if (comboBoxGroups.Items.Count > 0) { comboBoxGroups.SelectedIndex = 0; }
        }

        private void UpdateGroupsList()
        {
            _groups = DatabaseControl.GetGroups();

            comboBoxGroups.Items.Clear();

            foreach (DataRow dr in _groups.Rows)
                comboBoxGroups.Items.Add(dr["txt_group_name"].ToString());
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonOn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(comboBoxGroups.Text))
            {
                int cmdId = API.Commands.GetBuiltinCommandId("GROUP_ON");
                API.Commands.InstallQueCommandAndProcess(new QuedCommand { CommandId = cmdId, cmdtype = cmdType.Builtin, Argument = comboBoxGroups.Text});
            }
            else
                MessageBox.Show("No Group Selected!", API.GetProgramNameAndVersion,MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
        }

        private void buttonOff_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(comboBoxGroups.Text))
            {
                int cmdId = API.Commands.GetBuiltinCommandId("GROUP_OFF");
                API.Commands.InstallQueCommandAndProcess(new QuedCommand { CommandId = cmdId, cmdtype = cmdType.Builtin, Argument = comboBoxGroups.Text });
            }
            else
                MessageBox.Show("No Group Selected!", API.GetProgramNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
