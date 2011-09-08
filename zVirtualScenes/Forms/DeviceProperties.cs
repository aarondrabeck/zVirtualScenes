using System;
using System.Windows.Forms;
using zVirtualScenesApplication.PluginSystem;
using zVirtualScenesAPI;
using System.ComponentModel;
using zVirtualScenesCommon.Util;
using zVirtualScenesCommon.Entity;

namespace zVirtualScenesApplication.Forms
{
    public partial class DeviceProperties : Form
    {
        private BindingList<MenuItem> menu = new BindingList<MenuItem>();
        private device _d;

        public DeviceProperties(device d)
        {
            InitializeComponent();
            _d = d;

            this.colSettings.GroupKeyGetter = delegate(object rowObject)
            {
                return ((MenuItem)rowObject).Group;
            };

            this.colSettings.GroupKeyToTitleConverter = delegate(object groupKey)
            {
                return ((String)groupKey);
            };

            HideAllUserControls();
        }

        private void ObjectProperties_Load(object sender, EventArgs e)
        {
            if (_d.device_types.plugin.name.Equals("OPENZWAVE"))
            {
                string group = "Open ZWave Device";
                MenuItem mi = new MenuItem();
                mi.Name = "Basic";
                mi.Group = group;
                menu.Add(mi);
                
                mi = new MenuItem();
                mi.Name = "Values";
                mi.Group = group;
                menu.Add(mi);

                mi = new MenuItem();
                mi.Name = "Device Specific Commands";
                mi.Group = group;
                menu.Add(mi);

                mi = new MenuItem();
                mi.Name = "Generic Commands";
                mi.Group = group;
                menu.Add(mi);                

                mi = new MenuItem();
                mi.Name = "Groups";
                mi.Group = group;
                menu.Add(mi);

                mi = new MenuItem();
                mi.Name = "Properties";
                mi.Group = group;
                menu.Add(mi);

                dataListViewMenu.DataSource = menu;

                if (dataListViewMenu.Items.Count > 0)
                    dataListViewMenu.SelectedIndex = 0;

                this.Text = "'" + _d.friendly_name + "' Properties";
                textBoxObjName.Text = _d.friendly_name;
                
            }
        }
        
        private void HideAllUserControls()
        {
            uc_device_values_grid2.Visible = false;
            uc_device_commands1.Visible = false;
            uc_device_properties1.Visible = false;
            uc_device_groups1.Visible = false;
            uc_device_basic1.Visible = false;
            uc_device_type_commands1.Visible = false;
        }        

        public class MenuItem
        {
            public string Name;
            public string Group;
        }
       
        private void dataListViewMenu_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (dataListViewMenu.SelectedObject != null)
            {
                MenuItem mi = (MenuItem)dataListViewMenu.SelectedObject;
                HideAllUserControls();

                if (mi.Name.Equals("Values"))
                {
                    uc_device_values_grid2.UpdateControl(_d);
                    uc_device_values_grid2.Visible = true;
                }
                else if (mi.Name.Equals("Device Specific Commands"))
                {
                    uc_device_commands1.UpdateObject(_d);
                    uc_device_commands1.Visible = true;
                }
                else if (mi.Name.Equals("Generic Commands"))
                {
                    uc_device_type_commands1.UpdateObject(_d);
                    uc_device_type_commands1.Visible = true;
                }
                else if (mi.Name.Equals("Properties"))
                {
                    uc_device_properties1.UpdateObject(_d);
                    uc_device_properties1.Visible = true;
                }
                else if (mi.Name.Equals("Groups"))
                {
                    uc_device_groups1.UpdateControl(_d);
                    uc_device_groups1.Visible = true;
                }
                else if (mi.Name.Equals("Basic"))
                {
                    uc_device_basic1.UpdateControl(_d);
                    uc_device_basic1.Visible = true;
                }                
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ObjectProperties_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Escape))
            {
                this.Close();
            }
        }

        

    }
}
