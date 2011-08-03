using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Windows.Forms;
using zVirtualScenesAPI;
using zVirtualScenesApplication.PluginSystem;
using zVirtualScenesCommon.DatabaseCommon;

namespace zVirtualScenesApplication.TestForms
{
    public partial class TestForm : Form
    {
        private static PluginManager pm;

        private DataTable objects;

        public TestForm()
        {
            InitializeComponent();
        }

        private void TestForm_Load(object sender, EventArgs e)
        {
            // Test loading the plugins
            pm = new PluginManager();
   
            // Get the list of plugins and print them out to the listbox
            IEnumerable<Plugin> plugins = pm.GetPlugins();

            foreach (var plugin in plugins)
            {
                lstPlugins.Items.Add(plugin.ToString());
            }

            Thread t = new Thread(pm.RunCommand);
            t.Start();
        }

        private void btnUpdateObjectList_Click(object sender, EventArgs e)
        {
            lstObjects.Items.Clear();
            objects = DatabaseControl.GetObjects(false);
            foreach (DataRow dr in objects.Rows)
            {
                lstObjects.Items.Add(dr["txt_object_name"].ToString());
            }
        }

        private void lstObjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstObjects.SelectedIndex > -1)
            {
                int objectId;
                int.TryParse(objects.Rows[lstObjects.SelectedIndex]["id"].ToString(), out objectId);
                objectSettingsForm1.UpdateObject(objectId);
            }
        }
    }
}
