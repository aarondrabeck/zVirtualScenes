using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ControlThink.ZWave.Devices;
using System.Threading;

namespace zVirtualScenesApplication
{
    public class ControlThinkRepoller
    {
        public static string LOG_INTERFACE = "REPOLLER"; 
        public formzVirtualScenes zVirtualScenesMain;
        public delegate void DeviceInfoChangeEventHandler(string GlbUniqueID, changeType TypeOfChange);
        public event DeviceInfoChangeEventHandler DeviceInfoChange;
        public System.Timers.Timer RepollTimer = new System.Timers.Timer();
        public volatile bool isRefreshing = false;         

        public ControlThinkRepoller()
        {
            RepollTimer.Elapsed += new System.Timers.ElapsedEventHandler(tmr_Elapsed);
        }

        /// <summary>
        /// This continually looks for changes in device modes.  When one is detected, it will change MasterDeviceList and call the DeviceInfoChangeEventHandler. 
        /// </summary>
        public void Start()
        {             
            RepollTimer.Start();
            RepollTimer.Interval = zVirtualScenesMain.zVScenesSettings.PollingInterval * 1000;
        }

        public void UpdateInterval()
        {
            RepollTimer.Interval = zVirtualScenesMain.zVScenesSettings.PollingInterval * 1000;
        }

        private void tmr_Elapsed(object sender, EventArgs e)
        {
            RePollDevices();
        }

        public void RePollDevices(byte node = 0)
        {
            //Check if we are in the middle of loading devices from stick.
            if(zVirtualScenesMain.ControlThinkInt.ReloadDevicesWorker.IsBusy)
                return;

            //Check if we are already repolling.
            if (isRefreshing)
            {
                zVirtualScenesMain.AddLogEntry(UrgencyLevel.WARNING, "A request to repoll was called while a previous request was still working.  Consider lowering the repolling interval.", LOG_INTERFACE);
                return;
            }
            else
                isRefreshing = true;

            int NoResponseErrors = 0;
            int TimeoutErrors = 0;
            int OtherErrors = 0;
            DateTime Start = DateTime.Now;

            //Make sure the stick is connected.
            if (zVirtualScenesMain.ControlThinkInt.ControlThinkController.IsConnected)
            {

                zVirtualScenesMain.ControlThinkInt.ControlThinkController.SynchronizingObject = this;
                //For each device on Control Stick 
                foreach (ControlThink.ZWave.Devices.ZWaveDevice device in zVirtualScenesMain.ControlThinkInt.ControlThinkController.Devices)
                {
                    if (node != 0)  //If a node is sent, only repoll that node.
                    {
                        if (device.NodeID != node)
                            continue;
                    }                    

                    try
                    {
                        //If device type on Control Stick is allowed
                        //Do not include ZWave controllers for now...
                        if (!device.ToString().Contains("Controller")) 
                        {
                            //get type
                            string devicetype = device.ToString().Replace("ControlThink.ZWave.Devices.Specific.", "");

                            //for each device previously saved in memory
                            foreach (ZWaveDevice thisDevice in zVirtualScenesMain.MasterDevices)
                            {
                                //if Control Stick device == device in memory
                                if (zVirtualScenesMain.ControlThinkInt.ControlThinkController.HomeID.ToString() + device.NodeID.ToString() == thisDevice.GlbUniqueID())
                                {
                                    #region DETECT LEVEL CHANGES IN ALL DEVICES
                                    byte level = device.Level;
                                    //Check to see if any device state/level has changed.
                                    if (level != thisDevice.Level)
                                    {
                                        thisDevice.prevLevel = thisDevice.Level;
                                        thisDevice.Level = level; //set MasterDeviceList
                                        this.DeviceInfoChange(thisDevice.GlbUniqueID(), changeType.LevelChanged); //call event                                                
                                    }
                                    #endregion

                                    #region DETECT THERMOSTAT SPECIFIC CHANGES
                                    if (devicetype.Contains("GeneralThermostat"))
                                    {
                                        ControlThink.ZWave.Devices.Specific.GeneralThermostatV2 thermostat = (ControlThink.ZWave.Devices.Specific.GeneralThermostatV2)device;

                                        int coolpoint = (int)thermostat.ThermostatSetpoints[ThermostatSetpointType.Cooling1].Temperature.ToFahrenheit();
                                        //If ThermostatSetpoints[x] returns 0 C and gets converted to 32 F recheck to make sure it is the intended figure. 
                                        //Either the ControlThink Stick or certian thermostats falsely return Heat and Cool points of 32 F, upon a 
                                        //second query they return the proper value therefor we will requery if we initially get 32
                                        if (coolpoint == 32)
                                        {
                                            Thread.Sleep(200);
                                            coolpoint = (int)thermostat.ThermostatSetpoints[ThermostatSetpointType.Cooling1].Temperature.ToFahrenheit();
                                        }

                                        int heatpoint = (int)thermostat.ThermostatSetpoints[ThermostatSetpointType.Heating1].Temperature.ToFahrenheit();
                                        if (heatpoint == 32)
                                        {
                                            Thread.Sleep(200);
                                            heatpoint = (int)thermostat.ThermostatSetpoints[ThermostatSetpointType.Heating1].Temperature.ToFahrenheit();
                                        }

                                        int currenttemp = (int)thermostat.ThermostatTemperature.ToFahrenheit();
                                        int fanmode = (int)thermostat.ThermostatFanMode;
                                        int mode = (int)thermostat.ThermostatMode;
                                        level = thermostat.Level;
                                        string currentstate = thermostat.ThermostatOperatingState.ToString() + "-" + thermostat.ThermostatFanMode.ToString();


                                        if (thisDevice.Temp != currenttemp)
                                        {
                                            thisDevice.prevTemp = thisDevice.Temp; //Save old temp
                                            thisDevice.Temp = currenttemp; //Save new Temp
                                            this.DeviceInfoChange(thisDevice.GlbUniqueID(), changeType.TempChanged); //call event 
                                        }

                                        if (thisDevice.CoolPoint != coolpoint)
                                        {
                                            thisDevice.prevCoolPoint = thisDevice.CoolPoint;
                                            thisDevice.CoolPoint = coolpoint;
                                            this.DeviceInfoChange(thisDevice.GlbUniqueID(), changeType.CoolPointChanged); //call event
                                        }

                                        if (thisDevice.HeatPoint != heatpoint)
                                        {
                                            thisDevice.prevHeatPoint = thisDevice.HeatPoint;
                                            thisDevice.HeatPoint = heatpoint;
                                            this.DeviceInfoChange(thisDevice.GlbUniqueID(), changeType.HeatPointChanged); //call event
                                        }

                                        if (thisDevice.FanMode != fanmode)
                                        {
                                            thisDevice.prevFanMode = thisDevice.FanMode;
                                            thisDevice.FanMode = fanmode;
                                            this.DeviceInfoChange(thisDevice.GlbUniqueID(), changeType.FanModeChanged); //call event
                                        }

                                        if (thisDevice.HeatCoolMode != mode)
                                        {
                                            thisDevice.prevHeatCoolMode = thisDevice.HeatCoolMode;
                                            thisDevice.HeatCoolMode = mode;
                                            this.DeviceInfoChange(thisDevice.GlbUniqueID(), changeType.HeatCoolModeChanged); //call event
                                        }

                                        if (thisDevice.CurrentState != currentstate)
                                        {
                                            thisDevice.prevCurrentState = thisDevice.CurrentState;
                                            thisDevice.CurrentState = currentstate;
                                            this.DeviceInfoChange(thisDevice.GlbUniqueID(), changeType.CurrentStateChanged); //call event
                                        }
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("timed out"))
                            TimeoutErrors++;
                        else if (ex.Message.Contains("did not respond"))
                            NoResponseErrors++;
                        else
                            OtherErrors++;
                    }
                }
            }
            //if we get this far, repolling is finished.  Log it and release thread.
            isRefreshing = false;

            //Log this repoll
            TimeSpan RepollTime = DateTime.Now.Subtract(Start);
            string formattedTimeSpan = string.Format("{0:D2} m, {1:D2} s, {2:D2} ms", RepollTime.Minutes, RepollTime.Seconds, RepollTime.Milliseconds);            
            zVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "Repoll finished in " + formattedTimeSpan + ". (" + TimeoutErrors + " timeout, " + NoResponseErrors + " non-response, " + OtherErrors + " crital error.)" , LOG_INTERFACE);
                
        }

        public enum changeType
        {
            LevelChanged = 0,
            TempChanged = 1,
            CoolPointChanged = 2,
            HeatPointChanged = 3,
            FanModeChanged = 4,
            HeatCoolModeChanged = 5,
            CurrentStateChanged = 6
        }
    }
}
