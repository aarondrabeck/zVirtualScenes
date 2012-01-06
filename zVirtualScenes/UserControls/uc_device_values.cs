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
        public uc_device_values()
        {
            InitializeComponent();
            labelDeviceName.Text = "";
            uc_object_values_grid1.Visible = false;
            pictureBoxMain.Image = null;
        }

        public void UpdateControl(long device_id)
        {
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                device device = db.devices.FirstOrDefault(d => d.id == device_id);
                if (device == null)
                {
                    labelDeviceName.Text = "";
                    uc_object_values_grid1.Visible = false;
                    pictureBoxMain.Image = null;
                }
                else
                {
                    uc_object_values_grid1.UpdateControl(device_id);
                    labelDeviceName.Text = device.friendly_name;
                    switch (device.device_types.name)
                    {
                        case "DIMMER":
                            {
                                device_values dv_basic = device.device_values.FirstOrDefault(v => v.label_name == "Basic");

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
                                device_values dv_basic = device.device_values.FirstOrDefault(v => v.label_name == "Basic");
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

                                device_values dv_temp = device.device_values.FirstOrDefault(v => v.label_name == "Temperature");
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
                        case "SENSOR":
                            {
                                uc_object_values_grid1.Visible = true;
                                pictureBoxMain.Image = zVirtualScenesApplication.Properties.Resources.sensor_128;
                                break;
                            }
                        default:
                            pictureBoxMain.Image = null;
                            break;
                    }
                }
            }
        }        
    }
}
