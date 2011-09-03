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
                labelLevel.Text = "";
                uc_object_values_grid1.Visible = false;
                progressBar1.Visible = false;
                trackBar1.Visible = false;
                pictureBoxMain.Image = null;
                progressBar1.Value = 0;
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

                            if (level > 99)
                            {
                                progressBar1.Value = 99;
                                trackBar1.Value = 99;
                            }
                            else
                            {
                                progressBar1.Value = level;
                                trackBar1.Value = level;
                            }

                            trackBar1.Visible = true;
                            progressBar1.Visible = true;
                            labelLevel.Text = level + "%";
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


                            labelLevel.Text = (level > 0 ? "On" : "Off");
                            progressBar1.Value = (level > 0 ? 99 : 0);
                            progressBar1.Visible = true;
                            uc_object_values_grid1.Visible = true;

                            trackBar1.Value = (level > 0 ? 99 : 0);
                            trackBar1.Visible = true;

                            if (level > 0)
                                pictureBoxMain.Image = zVirtualScenesApplication.Properties.Resources.lb_on;
                            else
                                pictureBoxMain.Image = zVirtualScenesApplication.Properties.Resources.lb_off;
                            
                        }
                        break;
                    }
                case "THERMOSTAT":
                        {
                            device_values dv_temp = d.device_values.SingleOrDefault(v=>v.label_name=="Temperature");
                            if (dv_temp != null)
                            {
                                int temp = 0;
                                int.TryParse(dv_temp.value, out temp);

                                labelLevel.Text = temp + " degrees";
                                progressBar1.Visible = true;
                                uc_object_values_grid1.Visible = true;
                                progressBar1.Value = temp;
                                pictureBoxMain.Image = zVirtualScenesApplication.Properties.Resources.thermometer;
                                
                            }
                            break;
                        }
                case "CONTROLLER":
                        {
                            labelLevel.Text = "";
                            progressBar1.Visible = false;
                            uc_object_values_grid1.Visible = true;
                            pictureBoxMain.Image = zVirtualScenesApplication.Properties.Resources.controler3;
                            break;
                        }
                case "DOORLOCK":
                        {
                            labelLevel.Text = "";
                            progressBar1.Visible = false;
                            uc_object_values_grid1.Visible = true;
                            pictureBoxMain.Image = zVirtualScenesApplication.Properties.Resources.doorlock;
                            break;
                        }
                default:
                    progressBar1.Value = 0;
                    pictureBoxMain.Image = null;
                    break;
            }
        }

        private void progressBar1_MouseClick(object sender, MouseEventArgs e)
        {

            // NOTE: Limiting to dimmer for now till I have time to get the rest right.
            if (_device.device_types.name == "DIMMER")
            {
                decimal pos;
                pos = ((decimal)e.X / (decimal)progressBar1.Width) * 100;
                pos = Convert.ToInt32(pos);

                if (pos >= progressBar1.Minimum && pos <= progressBar1.Maximum)
                {
                    progressBar1.Value = (int)pos;

                    // TODO: Make this more "smart" about picking what command to use

                    //TODO: FIX 2.5
                    //int commandId = zvsAPI.Commands.GetObjectCommandId(_zwObj.ID, "DYNAMIC_CMD_BASIC");
                    //zvsAPI.Commands.InstallQueCommandAndProcess(new QuedCommand { cmdtype = commandScopeType.Object, CommandId = commandId, ObjectId = _zwObj.ID, Argument = progressBar1.Value.ToString() });
                }
            }
        }

        private void labelLevel_Click(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            TrackBar myTB;
            myTB = (System.Windows.Forms.TrackBar)sender;
            if (_device.device_types.name == "SWITCH")
            {
                if (trackBar1.Value > 50)
                    trackBar1.Value = 99;
                if (trackBar1.Value < 50)
                    trackBar1.Value = 0;
            }
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {


        }

        private void trackBar1_MouseUp(object sender, MouseEventArgs e)
        {
            changeDevice();
        }

        private void trackBar1_KeyUp(object sender, KeyEventArgs e)
        {
            changeDevice();
        }

        private void changeDevice()
        {
            if (_device.device_types.name == "DIMMER")
            {
                //TODO: 2.5
                //int commandId = zvsAPI.Commands.GetObjectCommandId(_zwObj.ID, "DYNAMIC_CMD_BASIC");
               // zvsAPI.Commands.InstallQueCommandAndProcess(new QuedCommand { cmdtype = commandScopeType.Object, CommandId = commandId, ObjectId = _zwObj.ID, Argument = trackBar1.Value.ToString() });

            }
            if (_device.device_types.name == "SWITCH")
            {
                if (trackBar1.Value > 50)
                {
                    //TODO: 2.5
                    //int commandId = zvsAPI.Commands.GetObjectTypeCommandId(_zwObj.Type_ID, "TURNON");
                    //zvsAPI.Commands.InstallQueCommandAndProcess(new QuedCommand { cmdtype = commandScopeType.ObjectType, CommandId = commandId, ObjectId = _zwObj.ID });
                }
                if (trackBar1.Value < 50)
                {
                    //TODO: 2.5
                   // int commandId = zvsAPI.Commands.GetObjectTypeCommandId(_zwObj.Type_ID, "TURNOFF");
                    //zvsAPI.Commands.InstallQueCommandAndProcess(new QuedCommand { cmdtype = commandScopeType.ObjectType, CommandId = commandId, ObjectId = _zwObj.ID });

                }
            }
        }
    }
}
