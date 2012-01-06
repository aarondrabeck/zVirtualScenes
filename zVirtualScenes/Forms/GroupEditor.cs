using System;
using System.Data;
using System.Windows.Forms;
using zVirtualScenesCommon.Entity;
using zVirtualScenesAPI;
using System.Linq;
using System.Collections.Generic;
using System.Data.Objects;

namespace zVirtualScenesApplication.Forms
{
    public partial class GroupEditor : Form
    {               
        public GroupEditor()
        {
            InitializeComponent();             
        }

        private void GroupEditor_Load(object sender, EventArgs e)
        {
            RebindGroupList();
            if (cboGroups.Items.Count > 0) { cboGroups.SelectedIndex = 0; }
        }

        private void RebindGroupList()
        {
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                cboGroups.DataSource = db.groups.OfType<group>().Execute(MergeOption.AppendOnly);
            }
            cboGroups.DisplayMember = "name";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnNewGroup_Click(object sender, EventArgs e)
        {
            AddEditGroupName FormAddEditGroupName = new AddEditGroupName("");
            FormAddEditGroupName.ShowDialog();

            if (FormAddEditGroupName.DialogResult == DialogResult.OK)
            {
                group new_g = group.Creategroup(0, FormAddEditGroupName.gName);

                using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                {
                    db.groups.AddObject(new_g);
                    db.SaveChanges();
                }
                RebindGroupList();
                cboGroups.SelectedIndex = cboGroups.Items.Count - 1;
            }
        }

        private void btnDeleteGroup_Click(object sender, EventArgs e)
        {
            if (cboGroups.SelectedIndex > -1)
            {
                group g = (group)cboGroups.SelectedItem;
                if (g != null)
                {
                    if (
                        MessageBox.Show("Are you sure you want to delete the " + g.name + " group?",
                                        "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                        DialogResult.Yes)
                    {
                        using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                        {
                            db.groups.DeleteObject(g);
                            db.SaveChanges();

                            zvsEntityControl.CallDeviceModified(this, "group");
                        }
                        cboGroups.SelectedIndex = cboGroups.Items.Count - 1;
                    }
                }
            }    
        }

        private void cboGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            RebindGroup();
        }

        private void RebindGroup()
        {
            group g = (group)cboGroups.SelectedItem;
            if (g != null)
            {
                if (g != null)
                {
                    using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                    {
                        lstAdded.DataSource = db.group_devices.Where(dg => dg.group_id == g.id).Select(d => d.device);

                        var device_query = from d in db.devices
                                           where !d.group_devices.Any(gd => gd.group_id == g.id)
                                           select d;
                        lstNotAdded.DataSource = device_query;

                    }
                }
            }
            else
            {
                lstNotAdded.DataSource = null;
                lstNotAdded.ClearObjects(); 
                lstAdded.DataSource = null;
                lstAdded.ClearObjects(); 
            }
        }

        private void btnAddAll_Click(object sender, EventArgs e)
        {
             group selected_group = (group)cboGroups.SelectedItem;

             if (selected_group != null)
             {
                 using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                 {
                     foreach (device d in lstNotAdded.Objects)
                     {
                         db.group_devices.AddObject(new group_devices { device_id = d.id, group_id = selected_group.id });
                     }
                     db.SaveChanges();
                 }
                 zvsEntityControl.CallDeviceModified(this, "group");
                 RebindGroup();
             }
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                group selected_group = db.groups.FirstOrDefault(g => g.id == ((group)cboGroups.SelectedItem).id);
                if (selected_group != null)
                {                    
                    foreach (device d in lstAdded.Objects)
                    {
                        group_devices device = selected_group.group_devices.FirstOrDefault(gd => gd.device_id == d.id);
                        db.group_devices.DeleteObject(device);
                    }
                    db.SaveChanges();
                    
                    zvsEntityControl.CallDeviceModified(this, "group");
                    RebindGroup();
                }
            }
        }

        private void btnAddOne_Click(object sender, EventArgs e)
        {
            group selected_group = (group)cboGroups.SelectedItem;
     
            if (selected_group != null)
            {
                using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                {
                    foreach (device d in lstNotAdded.SelectedObjects)
                    {
                        db.group_devices.AddObject(new group_devices { device_id = d.id, group_id = selected_group.id });
                    }
                    db.SaveChanges();
                }
                zvsEntityControl.CallDeviceModified(this, "group");
                RebindGroup();
            }
        }

        private void btnRemoveOne_Click(object sender, EventArgs e)
        {
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                group selected_group = db.groups.FirstOrDefault(g => g.id == ((group)cboGroups.SelectedItem).id); 
                if (selected_group != null)
                {

                    foreach (device d in lstAdded.SelectedObjects)
                    {
                        group_devices device = selected_group.group_devices.FirstOrDefault(gd => gd.device_id == d.id);
                        db.group_devices.DeleteObject(device);
                    }
                    db.SaveChanges();

                    zvsEntityControl.CallDeviceModified(this, "group");
                    RebindGroup();
                }
            }
        }       

        private void btnEdit_Click_1(object sender, EventArgs e)
        {
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                group selected_group = db.groups.FirstOrDefault(g => g.id == ((group)cboGroups.SelectedItem).id);
                if (selected_group != null)
                {
                    AddEditGroupName FormAddEditGroupName = new AddEditGroupName(selected_group.name);
                    FormAddEditGroupName.ShowDialog();


                    if (FormAddEditGroupName.DialogResult == DialogResult.OK)
                    {
                        selected_group.name = FormAddEditGroupName.gName;
                        db.SaveChanges();
                        zvsEntityControl.CallDeviceModified(this, "group");
                    }
                }
            }
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
