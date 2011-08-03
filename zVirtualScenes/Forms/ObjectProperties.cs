using System;
using System.Windows.Forms;
using zVirtualScenesApplication.PluginSystem;
using zVirtualScenesAPI;
using System.ComponentModel;
using zVirtualScenesCommon.DatabaseCommon;
using zVirtualScenesCommon.Util;

namespace zVirtualScenesApplication.Forms
{
    public partial class ObjectProperties : Form
    {
        BindingList<MenuItem> menu = new BindingList<MenuItem>();
        int _objId;
        string _objName;
        string _pluginName;

        public ObjectProperties(int objectId)
        {
            InitializeComponent();
            _objId = objectId;

            //Object Name
            _objName = API.Object.GetObjectName(objectId);

            // Get info about the plugin
            _pluginName = DatabaseControl.GetObjectPluginAPIName(objectId);
            if (string.IsNullOrEmpty(_pluginName))
            {
                Logger.WriteToLog(UrgencyLevel.ERROR, "Error getting plugin name", "OBJECTPROPFORM");
                return;
            }

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
            if (_pluginName.Equals("OPENZWAVE"))
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
                mi.Name = "Commands";
                mi.Group = group;
                menu.Add(mi);

                mi = new MenuItem();
                mi.Name = "Properties";
                mi.Group = group;
                menu.Add(mi);

                mi = new MenuItem();
                mi.Name = "Groups";
                mi.Group = group;
                menu.Add(mi);                

                dataListViewMenu.DataSource = menu;

                if (dataListViewMenu.Items.Count > 0)
                    dataListViewMenu.SelectedIndex = 0;

                if (!String.IsNullOrEmpty(_objName))
                {
                    this.Text = "'" + _objName + "' Properties";
                    textBoxObjName.Text = _objName;
                }
            }
        }
        
        private void HideAllUserControls()
        {
            uc_object_values_grid2.Visible = false;
            uc_object_commands1.Visible = false;
            uc_object_properties1.Visible = false;
            uc_object_groups1.Visible = false;
            uc_object_basic1.Visible = false; 
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
                    uc_object_values_grid2.UpdateControl(_objId);
                    uc_object_values_grid2.Visible = true;
                }
                else if (mi.Name.Equals("Commands"))
                {
                    uc_object_commands1.UpdateObject(_objId);
                    uc_object_commands1.Visible = true;
                }
                else if (mi.Name.Equals("Properties"))
                {
                    uc_object_properties1.UpdateObject(_objId);
                    uc_object_properties1.Visible = true;
                }
                else if (mi.Name.Equals("Groups"))
                {
                    uc_object_groups1.UpdateControl(_objId);
                    uc_object_groups1.Visible = true;
                }
                else if (mi.Name.Equals("Basic"))
                {
                    uc_object_basic1.UpdateControl(_objId);
                    uc_object_basic1.Visible = true;
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
