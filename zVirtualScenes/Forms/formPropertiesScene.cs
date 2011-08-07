using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using zVirtualScenesApplication.Structs;
using zVirtualScenesAPI;
using zVirtualScenesApplication.Globals;

namespace zVirtualScenesApplication
{
    public partial class formPropertiesScene : Form
    {
        public Scene _scene;

        public formPropertiesScene()
        {
            InitializeComponent();          
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {           
            if (!string.IsNullOrEmpty(txtb_sceneName.Text))
                API.Scenes.UpdateName(_scene.id, txtb_sceneName.Text);
            else
            {
                MessageBox.Show("Invalid scene name.", API.GetProgramNameAndVersion);
                return;
            }   
        }

        private void formSceneProperties_Load(object sender, EventArgs e)
        {
            txtb_sceneName.Text = _scene.txt_name;
            ActiveControl = txtb_sceneName;
            CreateDynamicProperties(); 
        }

        private void CreateDynamicProperties()
        {
            if (_scene != null)
            {
                pnlSceneProperties.Controls.Clear();
                int top = 0;

                #region Properties

                DataTable _loadedProperties = API.Scenes.Properties.GetSceneProperties();
                                
                top += 10;
                foreach (DataRow dr in _loadedProperties.Rows)
                {
                    int left = 0;
                    //Get Value
                    string value = API.Scenes.Properties.GetScenePropertyValue(_scene.id, dr["txt_property_name"].ToString());
                                       
                    int pType;
                    int.TryParse(dr["property_type_id"].ToString(), out pType);
                    ParamType propertyType = (ParamType)pType;
                    
                    #region Input Boxes                   
                    List<string> options = API.Scenes.Properties.GetScenePropertyOptions(dr["txt_property_name"].ToString());
                    left = GlobalMethods.DrawDynamicUserInputBoxes(pnlSceneProperties, propertyType, top, left, dr["txt_property_name"].ToString() + "-proparg", dr["txt_property_name"].ToString(), options, value, "");

                    #endregion

                    #region Add Button
                    Button btn = new Button();
                    btn.Name = dr["txt_property_name"].ToString();
                    btn.Text = (propertyType == ParamType.NONE ? "" : "Save ") + dr["txt_property_name"].ToString();
                    btn.Click += new EventHandler(btn_Click_Property_Set);
                    btn.Tag = propertyType;
                    btn.Top = top;
                    btn.Left = left;
                    pnlSceneProperties.Controls.Add(btn);

                    using (Graphics cg = this.CreateGraphics())
                    {
                        SizeF size = cg.MeasureString(btn.Text, btn.Font);
                        size.Width += 10; //add some padding
                        btn.Width = (int)size.Width;
                        left = (int)size.Width + left + 5;
                    }
                    #endregion

                    #region Add Description
                    Label lbl = new Label();
                    lbl.Text = dr["txt_property_description"].ToString();
                    lbl.Top = top +4;
                    lbl.Left = left;
                    lbl.AutoSize = false;
                    pnlSceneProperties.Controls.Add(lbl);

                    using (Graphics cg = this.CreateGraphics())
                    {
                        SizeF size = cg.MeasureString(lbl.Text, lbl.Font);
                        size.Width += 10; //add some padding
                        lbl.Width = (int)size.Width;
                        left = (int)size.Width + left + 5;
                    }
                    #endregion

                    top += 35;
                }
                #endregion
            }
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
                case ParamType.SHORT:
                case ParamType.BYTE:
                    //NumericUpDown have built in self validation
                    if (pnlSceneProperties.Controls.ContainsKey(btn.Name + "-proparg"))
                        arg = ((NumericUpDown)pnlSceneProperties.Controls[btn.Name + "-proparg"]).Value.ToString();
                    break;
                case ParamType.BOOL:
                    if (pnlSceneProperties.Controls.ContainsKey(btn.Name + "-proparg"))
                        arg = ((CheckBox)pnlSceneProperties.Controls[btn.Name + "-proparg"]).Checked.ToString();
                    break;
                case ParamType.STRING:
                    if (pnlSceneProperties.Controls.ContainsKey(btn.Name + "-proparg"))
                        arg = ((TextBox)pnlSceneProperties.Controls[btn.Name + "-proparg"]).Text;
                    break;
                case ParamType.LIST:
                    if (pnlSceneProperties.Controls.ContainsKey(btn.Name + "-proparg"))
                        arg = ((ComboBox)pnlSceneProperties.Controls[btn.Name + "-proparg"]).Text;
                    break;
            }
            API.Scenes.Properties.SetScenePropertyValue(btn.Name, _scene.id, arg);
        }

        private void txtb_sceneName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btn_Save_Click((object)sender, (EventArgs)e);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }       
}
