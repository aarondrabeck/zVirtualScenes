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
    public partial class AddEditTriggers : Form
    {
        private device_value_triggers trigger_to_edit = null;
        private IBindingList alltriggers;
        /// <summary>
        /// If editing pass the device_value_triggers to edit
        /// </summary>
        /// <param name="triggerToEdit"></param>
        public AddEditTriggers(IBindingList eventList, device_value_triggers triggerToEdit)
        {
            alltriggers = eventList;
            trigger_to_edit = triggerToEdit;

            InitializeComponent();
        }

        private void AddEditEvent_Load(object sender, EventArgs e)
        {
            labelTitle.Text = (trigger_to_edit == null) ? "Create Trigger" : "Edit Trigger";

            //Load cmb box values
            cmbo_devices.DisplayMember = "friendly_name";
            cmbo_devices.DataSource = zvsEntityControl.zvsContext.devices;

            cmbo_scene.DisplayMember = "friendly_name";
            cmbo_scene.DataSource = zvsEntityControl.zvsContext.scenes;

            cmbo_operator.DataSource = Enum.GetValues(typeof(device_value_triggers.TRIGGER_OPERATORS)); 

            //prefill if editing
            if (trigger_to_edit != null)
            {
                if(trigger_to_edit.trigger_operator.HasValue)
                    cmbo_operator.Text = Enum.GetName(typeof(device_value_triggers.TRIGGER_OPERATORS), trigger_to_edit.trigger_operator.Value); 
                cmbo_devices.SelectedItem = trigger_to_edit.device_values.device;
                cmbo_devicevalues.SelectedItem = trigger_to_edit.device_values;
                cmbo_scene.SelectedItem = trigger_to_edit.scene;
                txTriggertValue.Text = trigger_to_edit.trigger_value;
                txt_name.Text = trigger_to_edit.Name;
                checkBoxEnabled.Checked = trigger_to_edit.enabled;
            }

            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            device selected_device = (device)cmbo_devices.SelectedItem;

            if (selected_device != null)
            {
                cmbo_devicevalues.DisplayMember = "label_name";
                cmbo_devicevalues.DataSource = selected_device.device_values;
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

            //if we are editing dont create a new 
            device_value_triggers newtrigger;
            if (trigger_to_edit != null)
                newtrigger = trigger_to_edit;
            else
                newtrigger = new device_value_triggers(); 

            newtrigger.device_value_id = selected_device_value.id;
            newtrigger.enabled = true; 
            newtrigger.trigger_value = txTriggertValue.Text;
            newtrigger.trigger_operator = (int)cmbo_operator.SelectedItem;
            newtrigger.scene_id = selected_scene.id;
            newtrigger.Name = txt_name.Text;
            newtrigger.enabled = checkBoxEnabled.Checked;

            //if we are not editing add new trigger to trigger list
            if (trigger_to_edit == null)
                alltriggers.Add(newtrigger);

            zvsEntityControl.zvsContext.SaveChanges();
            this.Close();            
        }

        private void cmbo_devicevalues_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
