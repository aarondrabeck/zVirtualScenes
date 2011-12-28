using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using zVirtualScenesCommon.Entity;

namespace zVirtualScenesApplication.Forms
{
    public partial class AdvancedScripting : Form
    {
        private device_value_triggers trigger_to_edit = null;
        private IBindingList alltriggers;

        /// <summary>
        /// If editing pass the device_value_triggers to edit
        /// </summary>
        /// <param name="triggerToEdit"></param>
        public AdvancedScripting(IBindingList eventList, device_value_triggers triggerToEdit)
        {
            alltriggers = eventList;
            trigger_to_edit = triggerToEdit;

            InitializeComponent();
        }

        private void AdvancedScripting_Load(object sender, EventArgs e)
        {
            cmbo_devices.DisplayMember = "friendly_name";
            cmbo_devices.DataSource = zvsEntityControl.zvsContext.devices;

            if (trigger_to_edit != null)
            {
                cmbo_devices.SelectedItem = trigger_to_edit.device_values.device;
                cmbo_devicevalues.SelectedItem = trigger_to_edit.device_values;
                txt_name.Text = trigger_to_edit.Name;
                ckEnabled.Checked = trigger_to_edit.enabled;
                txt_script.Text = trigger_to_edit.trigger_script;
            }
        }

        private void cmbo_devices_SelectedIndexChanged(object sender, EventArgs e)
        {
            device selected_device = (device)cmbo_devices.SelectedItem;

            if (selected_device != null)
            {
                cmbo_devicevalues.DisplayMember = "label_name";
                cmbo_devicevalues.DataSource = selected_device.device_values;
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

            //if we are editing dont create a new 
            device_value_triggers newtrigger;
            if (trigger_to_edit != null)
                newtrigger = trigger_to_edit;
            else
                newtrigger = new device_value_triggers();

            newtrigger.device_value_id = selected_device_value.id;
            newtrigger.enabled = true;
            newtrigger.trigger_script = txt_script.Text;
            newtrigger.Name = txt_name.Text;
            newtrigger.enabled = ckEnabled.Checked;
            newtrigger.trigger_type = (int)device_value_triggers.TRIGGER_TYPE.Advanced;

            //if we are not editing add new trigger to trigger list
            if (trigger_to_edit == null)
                alltriggers.Add(newtrigger);

            zvsEntityControl.zvsContext.SaveChanges();
            this.Close();            
        }
    }
}
