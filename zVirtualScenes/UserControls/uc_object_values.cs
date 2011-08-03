using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using zVirtualScenesApplication.Structs;
using zVirtualScenesCommon.DatabaseCommon;
using zVirtualScenesAPI.Structs;
using zVirtualScenesAPI;
using System.Collections.Generic;

namespace zVirtualScenesApplication.UserControls
{
    public partial class uc_object_values : UserControl
    {
    
        private zwObject _zwObj;

        public uc_object_values()
        {
            InitializeComponent();
            UpdateControl(null);
        }

        public void UpdateControl(zwObject zwObj)
        {
            if (zwObj == null)
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

            _zwObj = zwObj;
            uc_object_values_grid1.UpdateControl(zwObj.ID);

            labelDeviceName.Text = _zwObj.Name;
            
            switch (zwObj.Type)
            {
                case "DIMMER":

                    if (zwObj.Level > 99)
                    {
                        progressBar1.Value = 99;
                        trackBar1.Value = 99;
                    }
                    else
                    {
                        progressBar1.Value = zwObj.Level;
                        trackBar1.Value = zwObj.Level;
                    }

                    trackBar1.Visible = true;
                    progressBar1.Visible = true;
                    labelLevel.Text = zwObj.Level + "%";
                    uc_object_values_grid1.Visible = true;


                    if (zwObj.Level > 0)
                        pictureBoxMain.Image = zVirtualScenesApplication.Properties.Resources.lb_on;
                    else
                        pictureBoxMain.Image = zVirtualScenesApplication.Properties.Resources.lb_off;
                    break;                  

                case "SWITCH":
                    labelLevel.Text = (zwObj.On ? "On" : "Off");
                    progressBar1.Value = (zwObj.On ? 99 : 0);
                    progressBar1.Visible = true;
                    uc_object_values_grid1.Visible = true;

                    trackBar1.Value = (zwObj.On ? 99 : 0);
                    trackBar1.Visible = true;

                    if (zwObj.On)
                        pictureBoxMain.Image = zVirtualScenesApplication.Properties.Resources.lb_on;
                    else
                        pictureBoxMain.Image = zVirtualScenesApplication.Properties.Resources.lb_off;
                    break;
                case "THERMOSTAT":
                    labelLevel.Text = (int)zwObj.Temperature + " degrees"; 
                    progressBar1.Visible = true;
                    uc_object_values_grid1.Visible = true;
                    progressBar1.Value = (int) zwObj.Temperature;
                    pictureBoxMain.Image = zVirtualScenesApplication.Properties.Resources.thermometer;
                    break;
                case "CONTROLLER":
                    labelLevel.Text = "";
                    progressBar1.Visible = false;
                    uc_object_values_grid1.Visible = true;
                    pictureBoxMain.Image = zVirtualScenesApplication.Properties.Resources.controler3;
                    break;
                case "DOORLOCK":
                    labelLevel.Text = "";
                    progressBar1.Visible = false;
                    uc_object_values_grid1.Visible = true;
                    pictureBoxMain.Image = zVirtualScenesApplication.Properties.Resources.doorlock;
                    break;
                default:
                    progressBar1.Value = 0;
                    pictureBoxMain.Image = null;
                    break;
            }
        }

        private void progressBar1_MouseClick(object sender, MouseEventArgs e)
        {
            
            // NOTE: Limiting to dimmer for now till I have time to get the rest right.
            if (_zwObj.Type == "DIMMER")
            {
                decimal pos;
                pos = ((decimal) e.X/(decimal) progressBar1.Width)*100;
                pos = Convert.ToInt32(pos);

                if (pos >= progressBar1.Minimum && pos <= progressBar1.Maximum)
                {
                    progressBar1.Value = (int) pos;

                    // TODO: Make this more "smart" about picking what command to use
                    int commandId = API.Commands.GetObjectCommandId(_zwObj.ID, "DYNAMIC_CMD_BASIC");
                    API.Commands.InstallQueCommandAndProcess(new QuedCommand { cmdtype = cmdType.Object, CommandId = commandId, ObjectId = _zwObj.ID, Argument = progressBar1.Value.ToString() });
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
            if (_zwObj.Type == "SWITCH")
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
            if (_zwObj.Type == "DIMMER")
            {
                int commandId = API.Commands.GetObjectCommandId(_zwObj.ID, "DYNAMIC_CMD_BASIC");

                API.Commands.InstallQueCommandAndProcess(new QuedCommand { cmdtype = cmdType.Object, CommandId = commandId, ObjectId = _zwObj.ID, Argument = trackBar1.Value.ToString() });

            }
            if (_zwObj.Type == "SWITCH")
            {
                if (trackBar1.Value > 50)
                {
                    int commandId = API.Commands.GetObjectTypeCommandId(_zwObj.Type_ID, "TURNON");
                    API.Commands.InstallQueCommandAndProcess(new QuedCommand { cmdtype = cmdType.ObjectType, CommandId = commandId, ObjectId = _zwObj.ID });                   
                }
                if (trackBar1.Value < 50)
                {
                    int commandId = API.Commands.GetObjectTypeCommandId(_zwObj.Type_ID, "TURNOFF");
                    API.Commands.InstallQueCommandAndProcess(new QuedCommand { cmdtype = cmdType.ObjectType, CommandId = commandId, ObjectId = _zwObj.ID });
                    
                }
            }   
        }
    }    
}
