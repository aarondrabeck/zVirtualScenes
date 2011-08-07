using System;
using System.Data;
using System.Windows.Forms;
using zVirtualScenesAPI;
using zVirtualScenesCommon.DatabaseCommon;
using zVirtualScenesCommon.Util;
using System.Drawing;
using System.Collections.Generic;
using zVirtualScenesApplication.Globals;

namespace zVirtualScenesApplication.UserControls
{
    public partial class uc_plugin_properties_form : UserControl
    {
        private DataTable _loadedSettings;
        private string _PluginAPIName;
        private MainForm mainForm;

        public uc_plugin_properties_form()
        {
            InitializeComponent();
        }
        
        public void PopulatePluginSettings(string PluginAPIName, MainForm f)
        {
            mainForm = f;
            _PluginAPIName = PluginAPIName;
            pnlSettings.Controls.Clear();
            int top = 5;

            #region Populate Builtin Settings
            IEnumerable<Plugin> allplugins = this.mainForm.pm.GetPlugins();
            foreach (Plugin p in allplugins)
            {
                if (p.GetAPIName().Equals(PluginAPIName))
                {                    
                    cbEnablePlugin.Checked = p.Enabled;
                    cbEnablePlugin.Text = "Enable " + p.GetAPIName();
                    labelPluginTitle.Text = "Plugin - " + p.GetAPIName();
                    cbEnablePlugin.Tag = p;
                }
            }
            #endregion

            #region Populate Dynamic User Input Objects
            _loadedSettings = DatabaseControl.GetAllPluginSettings(PluginAPIName); 
            Label SettingsLabel = new Label();
            SettingsLabel.Text = "Settings: " + (_loadedSettings == null ? "None" : "");
            SettingsLabel.Font = new System.Drawing.Font(SettingsLabel.Font.Name, SettingsLabel.Font.Size, FontStyle.Bold);
            SettingsLabel.Top = top;
            SettingsLabel.Left = 10;
            SettingsLabel.Height = 23;
            pnlSettings.Controls.Add(SettingsLabel);
            top += 25;

            if (_loadedSettings != null)
            {
                foreach (DataRow dr in _loadedSettings.Rows)
                {
                    int left = 10;                  

                    ParamType type = API.PluginSettings.GetPluginSettingType(PluginAPIName, dr["txt_setting_name"].ToString());

                    #region Label Description
                    if (type != ParamType.BOOL)
                    {
                        Label cntlLabel = new Label();
                        cntlLabel.Text = dr["txt_setting_name"].ToString() + ":";
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
                    List<string> options = API.PluginSettings.GetPluginSettingOptions(PluginAPIName, dr["txt_setting_name"].ToString());

                    string Identifer = dr["id"] + "-plugin_setting_arg";
                    left = GlobalMethods.DrawDynamicUserInputBoxes(pnlSettings, (ParamType)type, top, left, Identifer, dr["txt_setting_name"].ToString(), options, dr["txt_setting_value"].ToString(), type);
                    
                    #endregion

                    #region Label Description

                    Label CmdLabel = new Label();
                    CmdLabel.Text = dr["txt_setting_description"].ToString();
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            //Save Builtin Settings
            #region Enable/Disable
            IEnumerable<Plugin> allplugins = this.mainForm.pm.GetPlugins();

            foreach (Plugin p in allplugins)
            {
                if (p.Equals((Plugin)cbEnablePlugin.Tag))
                {
                    if (cbEnablePlugin.Checked)
                    {
                        //Enable
                        if (!p.Enabled)
                        {
                            p.Enabled = true;
                            p.Initialize();
                            p.Start();
                        }
                    }
                    else
                    {
                        //Disable
                        if (p.Enabled)
                        {
                            p.Enabled = false;

                            if (p.IsRunning)
                                p.Stop();
                        }
                    }

                }
            }
            #endregion

            //Save Dynamic Settings
            #region Dynamic Settings
            string errors = string.Empty; 

            if (_loadedSettings != null)
            {
                foreach (DataRow dr in _loadedSettings.Rows)  //For Each Plugin Setting
                {
                    string Identifer = dr["id"] + "-plugin_setting_arg";
                    foreach (Control c in pnlSettings.Controls)  //Find Control
                    {
                        if (c.Name.Equals(Identifer))
                        {
                            switch ((ParamType)c.Tag)  //Save entered setting depending on type.
                            {
                                case ParamType.NONE:
                                    break;                               
                                case ParamType.DECIMAL:
                                case ParamType.INTEGER:
                                case ParamType.SHORT:
                                case ParamType.BYTE:
                                    errors += API.PluginSettings.SetPluginSetting(_PluginAPIName, dr["txt_setting_name"].ToString(), ((NumericUpDown)c).Value.ToString());                                      
                                    break;
                                case ParamType.BOOL:
                                    errors += API.PluginSettings.SetPluginSetting(_PluginAPIName, dr["txt_setting_name"].ToString(), ((CheckBox)c).Checked.ToString());                                        
                                    break;
                                case ParamType.STRING:
                                    errors += API.PluginSettings.SetPluginSetting(_PluginAPIName, dr["txt_setting_name"].ToString(), ((TextBox)c).Text);                                       
                                    break;
                                case ParamType.LIST:
                                    errors += API.PluginSettings.SetPluginSetting(_PluginAPIName, dr["txt_setting_name"].ToString(), ((ComboBox)c).Text);                                        
                                    break;
                            }
                        }
                    }
                }
            }

            
            #endregion            

            if (string.IsNullOrEmpty(errors))
            {
                labelStatus.Text = _PluginAPIName + " Settings Saved!";
            }
            else
            {
                labelStatus.Text = "Save Failed.";
                MessageBox.Show("Error saving " + _PluginAPIName + " settings." + Environment.NewLine + Environment.NewLine + errors, API.GetProgramNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }        
    }
}
