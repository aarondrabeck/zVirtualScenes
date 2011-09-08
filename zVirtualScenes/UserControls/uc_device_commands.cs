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
    public partial class uc_device_commands : UserControl
    {
        private device _d; 

        public uc_device_commands()
        {
            InitializeComponent();
        }

        public void UpdateObject(device d)
        {
            _d = d;
            pnlSettings.Controls.Clear();
            int top = 0;

            #region Object Commands

            Label CommandLabel = new Label();
            CommandLabel.Text = "'" + _d.friendly_name + "' specific commands:" + (_d.device_commands.Count > 0 ? "" : " None.");
            CommandLabel.AutoSize = true;
            CommandLabel.Font = new System.Drawing.Font(CommandLabel.Font.Name, CommandLabel.Font.Size, FontStyle.Bold);
            CommandLabel.Top = top;
            CommandLabel.Left = 0;
            CommandLabel.Height = 23;
            pnlSettings.Controls.Add(CommandLabel);
            top += 25;
            
            foreach (device_commands d_cmd in _d.device_commands)
            {
                //get the current value from the value table list
                string current_value = string.Empty;
                device_values dv = d_cmd.device.device_values.SingleOrDefault(v => v.value_id == d_cmd.custom_data2);
                if (dv != null)
                    current_value = dv.value;

                int left = 0;
                #region Add Input Control Depending on type
                left = GlobalMethods.DrawDynamicUserInputBoxes(pnlSettings,
                                                                (Data_Types)d_cmd.arg_data_type,
                                                                top,
                                                                left,
                                                                d_cmd.id.ToString(),
                                                                d_cmd.friendly_name, 
                                                                d_cmd.device_command_options.Select(o => o.name).ToList(),
                                                                current_value,
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
                pnlSettings.Controls.Add(btn);
                toolTip1.SetToolTip(btn, d_cmd.description);

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

        private void btnClick(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string arg = String.Empty;

            device_commands dc = (device_commands)btn.Tag;

            switch ((Data_Types)(Data_Types)dc.arg_data_type)
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

            device_command_que cmd = device_command_que.Createdevice_command_que(0, _d.id, dc.id, arg);
            device_command_que.Run(cmd);            
        }
    }

}
