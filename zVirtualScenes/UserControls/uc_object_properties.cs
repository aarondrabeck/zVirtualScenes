using System;
using System.Data;
using System.Windows.Forms;
using zVirtualScenesAPI;
using zVirtualScenesCommon.DatabaseCommon;
using zVirtualScenesCommon.Util;
using System.Drawing;
using zVirtualScenesApplication.Globals;
using System.Collections.Generic;
using zVirtualScenesAPI.Structs;

namespace zVirtualScenesApplication.UserControls
{
    public partial class uc_object_properties : UserControl
    {        
        private int _objectId;

        public uc_object_properties()
        {
            InitializeComponent();
        }

        public void UpdateObject(int objectId)
        {
            _objectId = objectId;
            pnlSettings.Controls.Clear();           
            int top = 0;

            #region Properties
            DataTable _loadedProperties = API.Object.Properties.GetObjectProperties();

            /// Columns:
            /// id
            /// txt_property_friendly_name
            /// txt_property_name
            /// txt_default_value
            /// property_type
            /// 
            Label PropertiesLabel = new Label();
            PropertiesLabel.Text = "Object Properties:";
            PropertiesLabel.Font = new System.Drawing.Font(PropertiesLabel.Font.Name, PropertiesLabel.Font.Size, FontStyle.Bold);
            PropertiesLabel.Top = top;
            PropertiesLabel.Left = 0;
            PropertiesLabel.Height = 23;
            pnlSettings.Controls.Add(PropertiesLabel);
            top += 25;
            foreach (DataRow dr in _loadedProperties.Rows)
            {
                int left = 0;
                //Get Value
                string value = API.Object.Properties.GetObjectPropertyValue(objectId, dr["txt_property_name"].ToString());

                string value2 = API.Object.Properties.GetObjectPropertyValue(17, "DEFAULONLEVEL");
                int pType;
                int.TryParse(dr["property_type"].ToString(), out pType);
                ParamType propertyType = (ParamType)pType;

                #region Input Boxes
                //TODO: Setup options table for ObjectPropertyTypeOptions LIST types
                List<string> options = API.Object.Properties.GetObjectPropertyOptions(dr["txt_property_name"].ToString());
                left = GlobalMethods.DrawDynamicUserInputBoxes(pnlSettings, propertyType, top, left, dr["txt_property_name"].ToString() + "-proparg", dr["txt_property_friendly_name"].ToString(), options, value, "");

                #endregion

                #region Add Button
                Button btn = new Button();
                btn.Name = dr["txt_property_name"].ToString();
                btn.Text = "Save " + dr["txt_property_friendly_name"].ToString();
                btn.Click += new EventHandler(btn_Click_Property_Set);
                btn.Tag = propertyType;
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

            switch ((ParamType)btn.Tag)
            {
                case ParamType.NONE:
                    break;
                case ParamType.DECIMAL:
                case ParamType.INTEGER:
                case ParamType.BYTE:
                    //NumericUpDown have built in self validation
                    if (pnlSettings.Controls.ContainsKey(btn.Name + "-proparg"))
                        arg = ((NumericUpDown)pnlSettings.Controls[btn.Name + "-proparg"]).Value.ToString();
                    break;
                case ParamType.BOOL:
                    if (pnlSettings.Controls.ContainsKey(btn.Name + "-proparg"))
                        arg = ((CheckBox)pnlSettings.Controls[btn.Name + "-proparg"]).Checked.ToString();
                    break;
                case ParamType.STRING:
                    if (pnlSettings.Controls.ContainsKey(btn.Name + "-proparg"))
                        arg = ((TextBox)pnlSettings.Controls[btn.Name + "-proparg"]).Text;
                    break;
                case ParamType.LIST:
                    if (pnlSettings.Controls.ContainsKey(btn.Name + "-proparg"))
                        arg = ((ComboBox)pnlSettings.Controls[btn.Name + "-proparg"]).Text;
                    break;
            }
            API.Object.Properties.SetObjectPropertyValue(btn.Name, _objectId, arg);
        }
        
    }

    
}
