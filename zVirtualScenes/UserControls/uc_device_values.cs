using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using zVirtualScenesApplication.Structs;
using zVirtualScenesAPI;
using System.Collections.Generic;
using zVirtualScenesCommon.Entity;
using System.Linq;

namespace zVirtualScenesApplication.UserControls
{
    public partial class uc_device_values : UserControl
    {

        private device _device;

        public uc_device_values()
        {
            InitializeComponent();
            UpdateControl(null);
        }

        public void UpdateControl(device d)
        {
            if (d == null)
            {
                labelDeviceName.Text = "";
                uc_object_values_grid1.Visible = false;
                pictureBoxMain.Image = null;
                return;
            }

            _device = d;
            uc_object_values_grid1.UpdateControl(d);

            labelDeviceName.Text = d.friendly_name;

            switch (d.device_types.name)
            {
                case "DIMMER":
                    {
                        device_values dv_basic = d.device_values.SingleOrDefault(v => v.label_name == "Basic");
                        
                        if (dv_basic != null)
                        {
                            int level = 0;
                            int.TryParse(dv_basic.value, out level);                           
                            uc_object_values_grid1.Visible = true;


                            if (level > 0)
                                pictureBoxMain.Image = zVirtualScenesApplication.Properties.Resources.lb_on;
                            else
                                pictureBoxMain.Image = zVirtualScenesApplication.Properties.Resources.lb_off;
                        }

                        break;
                    }
                case "SWITCH":
                    {
                        device_values dv_basic = d.device_values.SingleOrDefault(v=>v.label_name=="Basic");
                        if (dv_basic != null)
                        {
                            int level = 0;
                            int.TryParse(dv_basic.value, out level);                          
                            uc_object_values_grid1.Visible = true;
                          
                            if (level > 0)
                                pictureBoxMain.Image = zVirtualScenesApplication.Properties.Resources.lb_on;
                            else
                                pictureBoxMain.Image = zVirtualScenesApplication.Properties.Resources.lb_off;
                            
                        }
                        break;
                    }
                case "THERMOSTAT":
                        {
                           
                            uc_object_values_grid1.Visible = true;                                
                            pictureBoxMain.Image = zVirtualScenesApplication.Properties.Resources.thermometer;
                                
                            device_values dv_temp = d.device_values.SingleOrDefault(v=>v.label_name=="Temperature");
                            if (dv_temp != null)
                            {
                                int temp = 0;
                                int.TryParse(dv_temp.value, out temp);                               
                            }
                            break;
                        }
                case "CONTROLLER":
                        {
                            uc_object_values_grid1.Visible = true;
                            pictureBoxMain.Image = zVirtualScenesApplication.Properties.Resources.controler3;
                            break;
                        }
                case "DOORLOCK":
                        {                            
                            uc_object_values_grid1.Visible = true;
                            pictureBoxMain.Image = zVirtualScenesApplication.Properties.Resources.doorlock;
                            break;
                        }
                default:                    
                    pictureBoxMain.Image = null;
                    break;
            }
        }
        
    }
}
