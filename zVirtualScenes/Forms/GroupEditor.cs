using System;
using System.Data;
using System.Windows.Forms;
using zVirtualScenesCommon.DatabaseCommon;
using zVirtualScenesAPI;

namespace zVirtualScenesApplication.Forms
{
    public partial class GroupEditor : Form
    {
        private DataTable _groups;
        private DataTable _added;
        private DataTable _notAdded;
        private int _groupId;

        public GroupEditor()
        {
            InitializeComponent();
        }

        private void GroupEditor_Load(object sender, EventArgs e)
        {
            UpdateGroupsList();

            //Select the first item if there is one
            if (cboGroups.Items.Count > 0) { cboGroups.SelectedIndex = 0; }
        }

        private void UpdateGroupsList()
        {
            _groups = DatabaseControl.GetGroups();

            cboGroups.Items.Clear();

            foreach (DataRow dr in _groups.Rows)
                cboGroups.Items.Add(dr["txt_group_name"].ToString());
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (cboGroups.SelectedIndex > -1)
            {
                if (
                    MessageBox.Show("Are you sure you want to delete the " + cboGroups.SelectedItem + " group?",
                                    "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                    DialogResult.Yes)
                {
                    DatabaseControl.DeleteGroup(_groupId);
                    UpdateGroupsList();
                    cboGroups.SelectedIndex = cboGroups.Items.Count - 1;
                }
            }            
        }

        private void cboGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            int.TryParse(_groups.Rows[cboGroups.SelectedIndex]["id"].ToString(), out _groupId);
            UpdateObjectLists();
        }

        private void UpdateObjectLists()
        {
            lstAdded.Items.Clear();
            lstNotAdded.Items.Clear();

            if (cboGroups.SelectedIndex > -1)
            {
                _added = DatabaseControl.GetGroupObjects(_groupId);
                _notAdded = DatabaseControl.GetGroupLeftOverObjects(_groupId);

                lstAdded.DataSource = _added;
                lstNotAdded.DataSource = _notAdded;
            }
        }

        private void btnAddAll_Click(object sender, EventArgs e)
        {
            foreach (DataRow dr in _notAdded.Rows)
            {
                int objectId;
                int.TryParse(dr["id"].ToString(), out objectId);
                DatabaseControl.AddObjectToGroup(_groupId, objectId);
            }

            UpdateObjectLists();
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            foreach (DataRow dr in _added.Rows)
            {
                int objectId;
                int.TryParse(dr["id"].ToString(), out objectId);
                DatabaseControl.RemoveObjectFromGroup(_groupId, objectId);
            }

            UpdateObjectLists();
        }

        private void btnAddOne_Click(object sender, EventArgs e)
        {            
            foreach(DataRowView dr in lstNotAdded.SelectedObjects) 
            {           
                int objectId;
                int.TryParse(dr["id"].ToString(), out objectId);
                DatabaseControl.AddObjectToGroup(_groupId, objectId);                
            }

            UpdateObjectLists();
        }

        private void btnRemoveOne_Click(object sender, EventArgs e)
        {
            foreach (DataRowView dr in lstAdded.SelectedObjects) 
            {
                int objectId;
                int.TryParse(dr["id"].ToString(), out objectId);
                DatabaseControl.RemoveObjectFromGroup(_groupId, objectId);               
            }
            UpdateObjectLists();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            AddEditGroupName FormAddEditGroupName = new AddEditGroupName("");
            FormAddEditGroupName.ShowDialog();

            if (FormAddEditGroupName.DialogResult == DialogResult.OK)
            {
                DatabaseControl.AddGroup(FormAddEditGroupName.gName);
                UpdateGroupsList();
                cboGroups.SelectedItem = FormAddEditGroupName.gName;
            }
            FormAddEditGroupName.Dispose();
        }

        private void btnEdit_Click_1(object sender, EventArgs e)
        {
            AddEditGroupName FormAddEditGroupName = new AddEditGroupName(cboGroups.SelectedItem.ToString());
            FormAddEditGroupName.ShowDialog();

            if (FormAddEditGroupName.DialogResult == DialogResult.OK)
            {
                int.TryParse(_groups.Rows[cboGroups.SelectedIndex]["id"].ToString(), out _groupId);

                DatabaseControl.SetGroupName(_groupId, FormAddEditGroupName.gName);
                UpdateGroupsList();
                cboGroups.SelectedItem = FormAddEditGroupName.gName;
            }
            FormAddEditGroupName.Dispose();
        }

        private void GroupEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Escape))
            {
                this.Close();
            }
        }
    }
}
