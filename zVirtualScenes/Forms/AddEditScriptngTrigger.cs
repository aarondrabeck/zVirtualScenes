using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using zVirtualScenesCommon.Entity;
using System.Data.Objects;

namespace zVirtualScenesApplication.Forms
{
    public partial class AddEditScriptngTrigger : Form
    {
        private long? trigger_to_edit_id = 0;
        /// <summary>
        /// If editing pass the device_value_triggers to edit
        /// </summary>
        /// <param name="triggerToEdit"></param>
        public AddEditScriptngTrigger(long? trigger_to_edit_id)
        {
            this.trigger_to_edit_id = trigger_to_edit_id;
            InitializeComponent();
        }

        private void AdvancedScripting_Load(object sender, EventArgs e)
        {
            labelTitle.Text = (trigger_to_edit_id.HasValue) ? "Edit Scripted Trigger" : "Create Scripted Trigger";
            cmbo_devicevalues.DisplayMember = "label_name";
            cmbo_devices.DisplayMember = "friendly_name";
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {                
                //populate device box
                cmbo_devices.DataSource = db.devices.OfType<device>().Execute(MergeOption.AppendOnly);

                //populate trigger values
                if (trigger_to_edit_id.HasValue)
                {
                    device_value_triggers trigger_to_edit = db.device_value_triggers.FirstOrDefault(o => o.id == trigger_to_edit_id.Value);
                    if (trigger_to_edit != null)
                    {
                        cmbo_devices.SelectedItem = trigger_to_edit.device_values.device;
                        cmbo_devicevalues.SelectedItem = trigger_to_edit.device_values;
                        txt_name.Text = trigger_to_edit.Name;
                        ckEnabled.Checked = trigger_to_edit.enabled;
                        txt_script.Text = trigger_to_edit.trigger_script;
                    }
                }

            }
            
        }

        private void cmbo_devices_SelectedIndexChanged(object sender, EventArgs e)
        {
            long device_id = ((device)cmbo_devices.SelectedItem).id;

            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                device device = db.devices.FirstOrDefault(d => d.id == device_id);
                //populate device values
                if (device != null)   
                    cmbo_devicevalues.DataSource = device.device_values;
                
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txt_name.Text))
            {
                MessageBox.Show("Please enter a name for this trigger.", zvsEntityControl.zvsNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                ActiveControl = txt_name;
                return;
            }

            if (string.IsNullOrEmpty(txt_script.Text))
            {
                MessageBox.Show("Please enter a script for this trigger.", zvsEntityControl.zvsNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                ActiveControl = txt_script;
                return;
            }

            device_values selected_device_value = (device_values)cmbo_devicevalues.SelectedItem;
            if (selected_device_value == null)
            {
                MessageBox.Show("Please select a device value for this trigger.", zvsEntityControl.zvsNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                ActiveControl = cmbo_devicevalues;
                return;
            }

            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                device_value_triggers trigger_to_edit;
                if (trigger_to_edit_id.HasValue)
                {
                    trigger_to_edit = db.device_value_triggers.FirstOrDefault(o => o.id == trigger_to_edit_id.Value);
                }
                else
                {
                    trigger_to_edit = new device_value_triggers();
                }

                trigger_to_edit.device_value_id = selected_device_value.id;
                trigger_to_edit.enabled = true;
                trigger_to_edit.trigger_script = txt_script.Text;
                trigger_to_edit.Name = txt_name.Text;
                trigger_to_edit.enabled = ckEnabled.Checked;
                trigger_to_edit.trigger_type = (int)device_value_triggers.TRIGGER_TYPE.Advanced;
           
                //if we are not editing add new trigger to trigger list
                if (!trigger_to_edit_id.HasValue)
                    db.device_value_triggers.AddObject(trigger_to_edit);

                db.SaveChanges();
            }
            zvsEntityControl.CallTriggerModified(this, "modified");            
            this.Close();            
        }
    }
}
