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
    public partial class AddEditBasicTriggers : Form
    {
        private long? trigger_to_edit_id = null;
        /// <summary>
        /// If editing pass the device_value_triggers to edit
        /// </summary>
        /// <param name="triggerToEdit"></param>
        public AddEditBasicTriggers(long? trigger_to_edit_id)
        {
            this.trigger_to_edit_id = trigger_to_edit_id;
            InitializeComponent();
        }

        private void AddEditEvent_Load(object sender, EventArgs e)
        {
            labelTitle.Text = (trigger_to_edit_id.HasValue) ? "Edit Trigger" :"Create Trigger";
            cmbo_scene.DisplayMember = "friendly_name";
            cmbo_devices.DisplayMember = "friendly_name";
            cmbo_devicevalues.DisplayMember = "label_name";

            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                //Load cmb box values                
                cmbo_devices.DataSource = db.devices.OfType<device>().Execute(MergeOption.AppendOnly);                
                cmbo_scene.DataSource = db.scenes.OfType<scene>().Execute(MergeOption.AppendOnly);
                cmbo_operator.DataSource = Enum.GetValues(typeof(device_value_triggers.TRIGGER_OPERATORS));

                //prefill if editing
                if (trigger_to_edit_id.HasValue)
                {
                    device_value_triggers trigger_to_edit = db.device_value_triggers.FirstOrDefault(o => o.id == trigger_to_edit_id.Value);
                    if (trigger_to_edit != null)
                    {
                        if (trigger_to_edit.trigger_operator.HasValue)
                            cmbo_operator.Text = Enum.GetName(typeof(device_value_triggers.TRIGGER_OPERATORS), trigger_to_edit.trigger_operator.Value);
                        cmbo_devices.SelectedItem = trigger_to_edit.device_values.device;
                        cmbo_devicevalues.SelectedItem = trigger_to_edit.device_values;
                        cmbo_scene.SelectedItem = trigger_to_edit.scene;
                        txTriggertValue.Text = trigger_to_edit.trigger_value;
                        txt_name.Text = trigger_to_edit.Name;
                        checkBoxEnabled.Checked = trigger_to_edit.enabled;
                    }
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
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

        private void btn_Save_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txt_name.Text))
            {
                MessageBox.Show("Please enter a name for this trigger.", zvsEntityControl.zvsNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                ActiveControl = txt_name;
                return; 
            }

            if (string.IsNullOrEmpty(txTriggertValue.Text))
            {
                MessageBox.Show("Please enter a trigger value for this trigger.", zvsEntityControl.zvsNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                ActiveControl = txt_name;
                return;
            }

            device_values selected_device_value = (device_values)cmbo_devicevalues.SelectedItem;
            if (selected_device_value == null)
            {
                MessageBox.Show("Please select a device value for this trigger.", zvsEntityControl.zvsNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                ActiveControl = cmbo_devicevalues;
                return; 
            }

            scene selected_scene = (scene)cmbo_scene.SelectedItem;
            if (selected_scene == null)
            {
                MessageBox.Show("Please select a scene for this trigger.", zvsEntityControl.zvsNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                ActiveControl = cmbo_scene;
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
                trigger_to_edit.trigger_value = txTriggertValue.Text;
                trigger_to_edit.trigger_operator = (int)cmbo_operator.SelectedItem;
                trigger_to_edit.scene_id = selected_scene.id;
                trigger_to_edit.Name = txt_name.Text;
                trigger_to_edit.enabled = checkBoxEnabled.Checked;
                trigger_to_edit.trigger_type = (int)device_value_triggers.TRIGGER_TYPE.Basic;

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
