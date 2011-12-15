using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using zVirtualScenesAPI;
using zVirtualScenesCommon.Entity;

namespace zVirtualScenesApplication
{
    public partial class ProgramSettings : Form
    {
        BindingList<MenuItem> menu = new BindingList<MenuItem>();
        MainForm _mainForm;

        public ProgramSettings(MainForm mainForm)
        {
            _mainForm = mainForm;
            InitializeComponent();

            this.colSettings.GroupKeyGetter = delegate(object rowObject)
            {
                return ((MenuItem)rowObject).Group;
            };

            this.colSettings.GroupKeyToTitleConverter = delegate(object groupKey)
            {
                return ((String)groupKey);
            };     
        }       

        private void ProgramSettings_Load(object sender, EventArgs e)
        {
            menu.Clear();

            //Add Built-in Items
            MenuItem mi = new MenuItem();
            mi.Name = "General";
            mi.Group = "zVirtualSceness";
            menu.Add(mi);           

            dataListViewMenu.DataSource = menu;

            IEnumerable<Plugin> allplugins = _mainForm.pm.GetPlugins();

            foreach (Plugin p in allplugins)
            {
                MenuItem plugin_mi = new MenuItem();
                plugin_mi.Name = p.ToString();
                plugin_mi.API = p.Name;
                plugin_mi.Group = "Plugins";
                menu.Add(plugin_mi);               
            }

            if (dataListViewMenu.Items.Count > 0)
                dataListViewMenu.SelectedIndex = 0;

            ActiveControl = dataListViewMenu;           
            
        }

        public class MenuItem
        {
            public string API;
            public string Name;
            public string Group;
        }

        private void dataListViewMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dataListViewMenu.SelectedObject != null)
            {
                MenuItem mi = (MenuItem)dataListViewMenu.SelectedObject;
                HideAllUserControls();

                if (mi.Group.Equals("Plugins"))
                {
                    plugin plgin = zvsEntityControl.zvsContext.plugins.FirstOrDefault(p => p.name == mi.API);
                    if (plgin != null)
                    {
                        uc_plugin_properties_form1.PopulatePluginSettings(plgin, _mainForm);
                        uc_plugin_properties_form1.Visible = true;
                    }
                }
                else
                {
                    //TODO: Load a General Settings Control 
                    //Add Entitys Connection Setting UC
                }
            }
        }

        private void HideAllUserControls()
        {
            uc_plugin_properties_form1.Visible = false; 
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDone_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Escape))
            {
                this.Close();
            }
        }

    }       
}
