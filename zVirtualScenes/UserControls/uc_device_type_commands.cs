using System;
using System.Data;
using System.Windows.Forms;
using zVirtualScenesAPI;
using zVirtualScenesCommon.Util;
using System.Drawing;
using zVirtualScenesApplication.Globals;
using System.Collections.Generic;
using zVirtualScenesCommon.Entity;
using System.Linq;

using zVirtualScenesCommon;

namespace zVirtualScenesApplication.UserControls
{
    public partial class uc_device_type_commands : UserControl
    {
        private long device_id; 

        public uc_device_type_commands()
        {
            InitializeComponent();
        }

        public void UpdateObject(long device_id)
        {
            this.device_id = device_id; 
            pnlSettings.Controls.Clear();
            int top = 0;

            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                device device = db.devices.FirstOrDefault(d => d.id == device_id);
                if (device == null)
                    return;

                #region Object Commands

                Label CommandLabel = new Label();
                CommandLabel.Text = "'" + device.device_types.friendly_name + "' commands:" + (device.device_types.device_type_commands.Count > 0 ? "" : " NSone.");
                CommandLabel.AutoSize = true;
                CommandLabel.Font = new System.Drawing.Font(CommandLabel.Font.Name, CommandLabel.Font.Size, FontStyle.Bold);
                CommandLabel.Top = top;
                CommandLabel.Left = 0;
                CommandLabel.Height = 23;
                pnlSettings.Controls.Add(CommandLabel);
                top += 25;

                foreach (device_type_commands d_cmd in device.device_types.device_type_commands)
                {
                    int left = 0;
                    #region Add Input Control Depending on type
                    left = GlobalMethods.DrawDynamicUserInputBoxes(pnlSettings,
                                                                    (Data_Types)d_cmd.arg_data_type,
                                                                    top,
                                                                    left,
                                                                    d_cmd.id.ToString(),
                                                                    d_cmd.friendly_name,
                                                                    d_cmd.device_type_command_options.Select(o => o.option).ToList(),
                                                                    d_cmd.custom_data1,
                                                                    d_cmd);
                    #endregion

                    #region Add Button
                    Button btn = new Button();
                    btn.Name = d_cmd.id.ToString();
                    btn.Text = d_cmd.friendly_name;
                    btn.Click += btnClick;
                    btn.Tag = d_cmd;
                    btn.Top = top;
                    btn.Left = left;
                    toolTip1.SetToolTip(btn, d_cmd.description);

                    pnlSettings.Controls.Add(btn);

                    using (Graphics cg = this.CreateGraphics())
                    {
                        SizeF size = cg.MeasureString(btn.Text, btn.Font);
                        size.Width += 10; //add some padding
                        btn.Width = (int)size.Width;
                        left = (int)size.Width + left + 5;
                    }
                    #endregion

                    #region Label

                    Label CmdLabel = new Label();
                    CmdLabel.Text = d_cmd.help;
                    CmdLabel.Top = top + 5;
                    CmdLabel.Left = left;
                    CmdLabel.Height = 23;
                    pnlSettings.Controls.Add(CmdLabel);

                    using (Graphics cg = this.CreateGraphics())
                    {
                        SizeF size = cg.MeasureString(CmdLabel.Text, CmdLabel.Font);
                        size.Width += 6; //add some padding
                        CmdLabel.Width = (int)size.Width;
                        left += (int)size.Width + 5;
                    }

                    #endregion
                    top += 35;
                }

                #endregion
            }
        }

        private void btnClick(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string arg = String.Empty;

            device_type_commands dc = (device_type_commands)btn.Tag;

            switch ((Data_Types)dc.arg_data_type)
            {
                case Data_Types.NONE:
                    break;
                case Data_Types.DECIMAL:
                case Data_Types.INTEGER:
                case Data_Types.SHORT:
                case Data_Types.BYTE:
                    //NumericUpDown have built in self validation
                    if (pnlSettings.Controls.ContainsKey(dc.id.ToString()))
                        arg = ((NumericUpDown)pnlSettings.Controls[dc.id.ToString()]).Value.ToString();
                    break;
                case Data_Types.BOOL:
                    if (pnlSettings.Controls.ContainsKey(dc.id.ToString()))
                        arg = ((CheckBox)pnlSettings.Controls[dc.id.ToString()]).Checked.ToString();
                    break;
                case Data_Types.STRING:
                    if (pnlSettings.Controls.ContainsKey(dc.id.ToString()))
                        arg = ((TextBox)pnlSettings.Controls[dc.id.ToString()]).Text;
                    break;
                case Data_Types.LIST:
                    if (pnlSettings.Controls.ContainsKey(dc.id.ToString()))
                        arg = ((ComboBox)pnlSettings.Controls[dc.id.ToString()]).Text;
                    break;
            }

            device_type_command_que cmd = device_type_command_que.Createdevice_type_command_que(0, dc.id, device_id, arg);
            device_type_command_que.Run(cmd);
        }
    }

}
