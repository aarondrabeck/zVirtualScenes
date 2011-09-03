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
    public partial class uc_device_properties : UserControl
    {
        private device _d; 

        public uc_device_properties()
        {
            InitializeComponent();
        }

        public void UpdateObject(device d)
        {
            _d = d;
            pnlSettings.Controls.Clear();
            int top = 0;

            #region Properties
     
            Label PropertiesLabel = new Label();
            PropertiesLabel.Text = "Device Properties:";
            PropertiesLabel.Font = new System.Drawing.Font(PropertiesLabel.Font.Name, PropertiesLabel.Font.Size, FontStyle.Bold);
            PropertiesLabel.Top = top;
            PropertiesLabel.Left = 0;
            PropertiesLabel.Height = 23;
            pnlSettings.Controls.Add(PropertiesLabel);
            top += 25;
            
            foreach (device_propertys dp in zvsEntityControl.zvsContext.device_propertys)
            {
                int left = 0;
                //Get Value
                string value = device_property_values.GetDevicePropertyValue(d.id, dp.name);
                
                Data_Types dataType = (Data_Types)dp.value_data_type;

                #region Input Boxes 
                left = GlobalMethods.DrawDynamicUserInputBoxes(pnlSettings, 
                                                                dataType, 
                                                                top, 
                                                                left, 
                                                                dp.name + "-proparg", 
                                                                dp.friendly_name, 
                                                                dp.device_property_options.Select(o=> o.name).ToList(), 
                                                                value, 
                                                                "");

                #endregion

                #region Add Button
                Button btn = new Button();
                btn.Name = dp.name;
                btn.Text = "Save " + dp.friendly_name;
                btn.Click += new EventHandler(btn_Click_Property_Set);
                btn.Tag = dataType;
                btn.Top = top;
                btn.Left = left;
                pnlSettings.Controls.Add(btn);

                using (Graphics cg = this.CreateGraphics())
                {
                    SizeF size = cg.MeasureString(btn.Text, btn.Font);
                    size.Width += 10; //add some padding
                    btn.Width = (int)size.Width;
                    left = (int)size.Width + left + 5;
                }
                #endregion

                top += 35;
            }
            #endregion

        }

        void btn_Click_Property_Set(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string arg = String.Empty;

            switch ((Data_Types)btn.Tag)
            {
                case Data_Types.NONE:
                    break;
                case Data_Types.DECIMAL:
                case Data_Types.INTEGER:
                case Data_Types.SHORT:
                case Data_Types.BYTE:
                    //NumericUpDown have built in self validation
                    if (pnlSettings.Controls.ContainsKey(btn.Name + "-proparg"))
                        arg = ((NumericUpDown)pnlSettings.Controls[btn.Name + "-proparg"]).Value.ToString();
                    break;
                case Data_Types.BOOL:
                    if (pnlSettings.Controls.ContainsKey(btn.Name + "-proparg"))
                        arg = ((CheckBox)pnlSettings.Controls[btn.Name + "-proparg"]).Checked.ToString();
                    break;
                case Data_Types.STRING:
                    if (pnlSettings.Controls.ContainsKey(btn.Name + "-proparg"))
                        arg = ((TextBox)pnlSettings.Controls[btn.Name + "-proparg"]).Text;
                    break;
                case Data_Types.LIST:
                    if (pnlSettings.Controls.ContainsKey(btn.Name + "-proparg"))
                        arg = ((ComboBox)pnlSettings.Controls[btn.Name + "-proparg"]).Text;
                    break;
            }

            device_propertys dp = zvsEntityControl.zvsContext.device_propertys.SingleOrDefault(p => p.name == btn.Name);

            if(dp != null)
            {
                device_property_values dpv = _d.device_property_values.SingleOrDefault(v => v.device_property_id == dp.id);
                if(dpv != null)
                {
                    dpv.value = arg;
                }
                else
                {
                    zvsEntityControl.zvsContext.device_property_values.AddObject(new device_property_values { device_id = _d.id, value = arg, device_property_id = dp.id });
                }
                zvsEntityControl.zvsContext.SaveChanges();
            }
        }
        
    }    
}
