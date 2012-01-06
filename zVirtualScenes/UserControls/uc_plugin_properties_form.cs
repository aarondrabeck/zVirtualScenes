using System;
using System.Data;
using System.Windows.Forms;
using zVirtualScenesAPI;
using zVirtualScenesCommon.Util;
using System.Drawing;
using System.Collections.Generic;
using zVirtualScenesApplication.Globals;
using zVirtualScenesCommon;
using System.Data.Entity;
using System.Linq;
using zVirtualScenesCommon.Entity;

namespace zVirtualScenesApplication.UserControls
{
    public partial class uc_plugin_properties_form : UserControl
    {
        private MainForm mainForm;
        private string APIName;

        public uc_plugin_properties_form()
        {           
            InitializeComponent();
        }

        public void PopulatePluginSettings(string APIName, MainForm f)
        {
            labelStatus.Text = "";
            this.APIName = APIName;
            this.mainForm = f;

            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                plugin p = db.plugins.FirstOrDefault(o => o.name.Equals(APIName)); 
                if (p != null)
                {
                    pnlSettings.Controls.Clear();
                    int top = 5;

                    #region Populate Builtin Settings
                    Plugin zvsPlugin = this.mainForm.pm.GetPlugins().FirstOrDefault(pl => pl.Name == p.name);

                    if (zvsPlugin != null)
                    {
                        cbEnablePlugin.Checked = zvsPlugin.Enabled;
                        labelPluginTitle.Text = p.friendly_name + " for zVirtualScenes";
                        lblPluginDescription.Text = p.description;
                        cbEnablePlugin.Tag = zvsPlugin;
                    }
                    else
                        return;

                    #endregion

                    #region Populate Dynamic User Input Objects

                    if (p.plugin_settings != null)
                    {
                        foreach (plugin_settings ps in p.plugin_settings)
                        {
                            int left = 10;

                            #region Label Description
                            if ((Data_Types)ps.value_data_type != Data_Types.BOOL)
                            {
                                Label cntlLabel = new Label();
                                cntlLabel.Text = ps.friendly_name + ":";
                                cntlLabel.Top = top + 5;
                                cntlLabel.Left = left;
                                cntlLabel.Height = 23;
                                pnlSettings.Controls.Add(cntlLabel);

                                using (Graphics cg = this.CreateGraphics())
                                {
                                    SizeF size = cg.MeasureString(cntlLabel.Text, cntlLabel.Font);
                                    cntlLabel.Width = (int)size.Width;
                                    left += (int)size.Width;
                                }
                            }

                            #endregion

                            #region Add Input Control Depending on type
                            left = GlobalMethods.DrawDynamicUserInputBoxes(pnlSettings,
                                                                            (Data_Types)ps.value_data_type,
                                                                            top,
                                                                            left,
                                                                            ps.id + "-plugin_setting_arg",
                                                                            ps.friendly_name,
                                                                            ps.plugin_setting_options.Select(s => s.option).ToList(),
                                                                            ps.value,
                                                                            (Data_Types)ps.value_data_type);

                            #endregion

                            #region Label Description

                            Label CmdLabel = new Label();
                            CmdLabel.Text = ps.description;
                            CmdLabel.Top = top + 5;
                            CmdLabel.Left = left;
                            CmdLabel.Height = 23;
                            pnlSettings.Controls.Add(CmdLabel);

                            using (Graphics cg = this.CreateGraphics())
                            {
                                SizeF size = cg.MeasureString(CmdLabel.Text, CmdLabel.Font);
                                size.Width += 6; //add some padding
                                CmdLabel.Width = (int)size.Width;
                                left = (int)size.Width + left + 5;
                            }

                            #endregion

                            top += 35;
                        }
                    }
                    #endregion
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {           
            //Save Dynamic Settings
            #region Dynamic Settings
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                plugin p = db.plugins.FirstOrDefault(o => o.name.Equals(APIName));
                if (p != null)
                {
                    //SAVE PLUGIN SETTINGS
                    if (p.plugin_settings != null)
                    {
                        //SAVE SETTIMGS
                        foreach (plugin_settings ps in p.plugin_settings)  //For Each Plugin Setting
                        {
                            string Identifer = ps.id + "-plugin_setting_arg";
                            foreach (Control c in pnlSettings.Controls)  //Find Control
                            {
                                if (c.Name.Equals(Identifer))
                                {
                                    switch ((Data_Types)c.Tag)  //Save entered setting depending on type.
                                    {
                                        case Data_Types.NONE:
                                            break;
                                        case Data_Types.DECIMAL:
                                        case Data_Types.INTEGER:
                                        case Data_Types.SHORT:
                                        case Data_Types.BYTE:
                                        case Data_Types.COMPORT:
                                            ps.value = ((NumericUpDown)c).Value.ToString();
                                            break;
                                        case Data_Types.BOOL:
                                            ps.value = ((CheckBox)c).Checked.ToString();
                                            break;
                                        case Data_Types.STRING:
                                            ps.value = ((TextBox)c).Text;
                                            break;
                                        case Data_Types.LIST:
                                            ps.value = ((ComboBox)c).Text;
                                            break;
                                    }
                                }
                            }
                        }
                    }

                    //Save Builtin Settings
                    #region Enable/Disable
                    Plugin zvsPlugin = (Plugin)cbEnablePlugin.Tag;
                    if (zvsPlugin != null)
                    {
                        if (cbEnablePlugin.Checked)
                        {
                            //Enable
                            if (!zvsPlugin.Enabled)
                            {
                                zvsPlugin.Enabled = true;
                                p.enabled = true;
                                zvsPlugin.Initialize();
                                zvsPlugin.Start();
                            }
                        }
                        else
                        {
                            //Disable
                            if (zvsPlugin.Enabled)
                            {
                                zvsPlugin.Enabled = false;
                                p.enabled = false;


                                if (zvsPlugin.IsRunning)
                                    zvsPlugin.Stop();
                            }
                        }
                    }
                    #endregion

                    db.SaveChanges();
                    labelStatus.Text = string.Format("{0} - {1} settings saved.", DateTime.Now.ToLongTimeString(),p.friendly_name); 
                }
            }                    
            //labelStatus.Text = "Save Failed.";
            //MessageBox.Show("Error saving " + _p.friendly_name + " settings." + Environment.NewLine + Environment.NewLine + ex.Message, zvsEntityControl.zvsNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            #endregion

           
        }
    }
}
