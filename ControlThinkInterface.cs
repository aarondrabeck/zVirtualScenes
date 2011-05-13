using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ControlThink.ZWave;
using System.Threading;
using ControlThink.ZWave.Devices;
using System.Windows.Forms;
using System.ComponentModel;

namespace zVirtualScenesApplication
{

    public class ControlThinkInterface
    {
        public static string LOG_INTERFACE = "CONTROL THINK";
        public formzVirtualScenes formzVirtualScenesMain;
        public readonly ZWaveController ControlThinkController = new ZWaveController();
        public BackgroundWorker ReloadDevicesWorker;

        //Repoller
        public delegate void DeviceInfoChangeEventHandler(string GlbUniqueID, changeType TypeOfChange, bool verbose);
        public event DeviceInfoChangeEventHandler DeviceInfoChange;

        public ControlThinkInterface()
        {
            ReloadDevicesWorker = new BackgroundWorker();
            ReloadDevicesWorker.DoWork += new DoWorkEventHandler(ReloadDevicesWorker_DoWork);
            ReloadDevicesWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ReloadDevicesWorker_RunWorkerCompleted);

            ControlThinkController.Connected += new System.EventHandler(ControlThinkUSBConnectedEvent);
            ControlThinkController.Disconnected += new System.EventHandler(ControlThinkUSBDisconnectEvent);
            ControlThinkController.ControllerNotResponding += new System.EventHandler(ControlThinkUSBNotRespondingEvent);
            ControlThinkController.LevelChanged += new ZWaveController.LevelChangedEventHandler(ControlThinkController_LevelChanged);
        }       

        #region Initial Connection and Discover Zwave Devices on the USB Stick

        public void ConnectAndFindDevices()
        {
            if (this.ReloadDevicesWorker.IsBusy)
            {
                formzVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "ControlThink Reload Already Running.", LOG_INTERFACE);
                return;
            }           
            ReloadDevicesWorker.RunWorkerAsync();
        }

        private void ReloadDevicesWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //if (this.RepollDevicesWorker.IsBusy)            
            //    throw new Exception("ControlThink USB is busy repolling, cannot look for new devices at this time.");
                
            BindingList<ZWaveDevice> DevicesFound = new BindingList<ZWaveDevice>();            

            //Connect to ThinkStick
            if (!ControlThinkController.IsConnected)
            {
                    //ControlThinkController.SynchronizingObject = this;                    
                    ControlThinkController.Connect();                    
            }

            //Get ZWave Devices
            if (ControlThinkController.IsConnected)
            {
                foreach (ControlThink.ZWave.Devices.ZWaveDevice device in ControlThinkController.Devices)
                {
                    try
                    {
                        formzVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "Found " + device.ToString() + ".", LOG_INTERFACE);

                        if (!device.ToString().Contains("Controller")) //Do not include ZWave controllers for now...
                        {
                            //Save ControlThink Device to zVirtualScenesDevice
                            ZWaveDevice newDevice = new ZWaveDevice();
                            newDevice.HomeID = ControlThinkController.HomeID;
                            newDevice.NodeID = device.NodeID;

                            //BINARY SWITCHES
                            if (device is ControlThink.ZWave.Devices.BinarySwitch)
                            {
                                newDevice.Type = ZWaveDevice.ZWaveDeviceTypes.BinarySwitch;
                                newDevice.Name = "Binary Switch";                                
                            }
                            //MULTILEVEL
                            else if (device is ControlThink.ZWave.Devices.MultilevelSwitch)
                            {
                                newDevice.Type = ZWaveDevice.ZWaveDeviceTypes.MultiLevelSwitch;
                                newDevice.Name = "Multi-level Switch";                                
                            }
                            //Therostats
                            else if (device is ControlThink.ZWave.Devices.Thermostat)
                            {
                                newDevice.Type = ZWaveDevice.ZWaveDeviceTypes.Thermostat;
                                newDevice.Name = "Thermostat";
                            }
                            else
                            {
                                formzVirtualScenesMain.AddLogEntry(UrgencyLevel.WARNING, "Device type  " + device.ToString() + " UNKNOWN.", LOG_INTERFACE);
                            }
                            DevicesFound.Add(newDevice);                            
                        }
                    }
                    catch (Exception ex)
                    {
                        formzVirtualScenesMain.AddLogEntry(UrgencyLevel.ERROR, ex.Message, LOG_INTERFACE);
                    }                    
                }                 
            }
            e.Result = DevicesFound;
        }       

        private void ReloadDevicesWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                formzVirtualScenesMain.AddLogEntry(UrgencyLevel.ERROR, e.Error.Message, LOG_INTERFACE);
            else
            {
                BindingList<ZWaveDevice> DevicesFound = (BindingList<ZWaveDevice>)e.Result;

                formzVirtualScenesMain.MasterDevices.Clear();

                foreach (ZWaveDevice newDevice in DevicesFound)
                {
                    //Overwirte Name from the Custom Device Saved Data if present.
                    foreach (ZWaveDeviceUserSettings PreviouslySavedDevice in formzVirtualScenesMain.SavedZWaveDeviceUserSettings)
                    {
                        if (newDevice.GlbUniqueID() == PreviouslySavedDevice.GlbUniqueID())
                        {
                            newDevice.Name = PreviouslySavedDevice.Name;
                            newDevice.NotificationDetailLevel = PreviouslySavedDevice.NotificationDetailLevel;
                            newDevice.SendJabberNotifications = PreviouslySavedDevice.SendJabberNotifications;
                            newDevice.MaxAlertTemp = PreviouslySavedDevice.MaxAlertTemp;
                            newDevice.MinAlertTemp = PreviouslySavedDevice.MinAlertTemp;
                            newDevice.GroupName = PreviouslySavedDevice.GroupName;
                            newDevice.ShowInLightSwitchGUI = PreviouslySavedDevice.ShowInLightSwitchGUI;
                            newDevice.MomentaryOnMode = PreviouslySavedDevice.MomentaryOnMode;
                            newDevice.MomentaryTimespan = PreviouslySavedDevice.MomentaryTimespan;
                            if (newDevice.GroupName != "")
                                formzVirtualScenesMain.groups.Add(newDevice.GroupName);
                        }
                    }
                    formzVirtualScenesMain.MasterDevices.Add(newDevice);
                }
                formzVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "ControlThink USB Loaded " + formzVirtualScenesMain.MasterDevices.Count() + " Devices.", LOG_INTERFACE);

                //setup per device polling now that we have a list of devices.
                UpdatePollingIntervalsAllDevices();
            }                          
        }

        #endregion

        #region Repoll devices

        public void UpdatePollingIntervalsAllDevices()
        {
            if (ControlThinkController.IsConnected)
            {
                foreach (ControlThink.ZWave.Devices.ZWaveDevice device in ControlThinkController.Devices)
                {
                    if (!device.ToString().Contains("Controller")) //Do not include ZWave controllers
                    {                      

                        //Look for user set polling levels for this device
                        foreach (ZWaveDevice thisDevice in formzVirtualScenesMain.MasterDevices)
                        {
                            //All devices check for level
                            if (this.ControlThinkController.HomeID.ToString() + device.NodeID.ToString() == thisDevice.GlbUniqueID())
                            {
                                if (thisDevice.RepollInterval > 0)
                                {
                                    device.PollInterval = new TimeSpan(0, 0, thisDevice.RepollInterval);
                                    device.PollEnabled = true;
                                }
                                else
                                {
                                    device.PollEnabled = false;
                                }
                            }
                           
                            //Some devices like general thermos need to be polled for more than just level.
                            if (thisDevice.Type == ZWaveDevice.ZWaveDeviceTypes.Thermostat)
                            {
                                if (thisDevice.RepollInterval > 0)
                                {
                                    if (!thisDevice.SubscribedToPollTimer)
                                    {
                                        thisDevice.PollTimer.Elapsed += new System.Timers.ElapsedEventHandler(PollTimer_Elapsed);
                                        thisDevice.PollTimer._node = device.NodeID;
                                        thisDevice.SubscribedToPollTimer = true; 
                                    }

                                    thisDevice.PollTimer.Interval = thisDevice.RepollInterval *1000;
                                    thisDevice.PollTimer.Start();
                                }
                                else
                                {
                                    thisDevice.PollTimer.Stop();
                                }
                            }
                        }
                    }                    
                }
            }
        }

        void PollTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            RepollingTimer timer = (RepollingTimer)sender;
            formzVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "TIMER ELASPED. NODE: " + timer._node + ".", LOG_INTERFACE);        
        }

        private void tmr_Elapsed(object sender, RepollingTimerArgs e)
        {
            
        }

        private void SubscribetoDeviceEvents()
        {
            if (ControlThinkController.IsConnected)
            {
                foreach (ControlThink.ZWave.Devices.ZWaveDevice device in ControlThinkController.Devices)
                {                    
                    if (!device.ToString().Contains("Controller")) //Do not include ZWave controllers
                    { 
                        device.PollFailed += new ControlThink.ZWave.Devices.ZWaveDevice.PollFailedEventHandler(device_PollFailed);
                        device.LevelChanged += new ControlThink.ZWave.Devices.ZWaveDevice.LevelChangedEventHandler(device_LevelChanged);

                        //string devicetype = device.ToString().Replace("ControlThink.ZWave.Devices.Specific.", "");
                        //if (devicetype.Contains("GeneralThermostat"))
                        //{
                        //    foreach (ZWaveDevice thisDevice in formzVirtualScenesMain.MasterDevices)
                        //    {
                        //        if (this.ControlThinkController.HomeID.ToString() + device.NodeID.ToString() == thisDevice.GlbUniqueID())
                        //        {
                                    
                        //        }                              
                        //    }
                        //}
                    }                    
                }
            }
        }

        #endregion

        #region USB Events

        //void device_LevelChanged_therm(object sender, LevelChangedEventArgs e)
        //{
        //    #region DETECT LEVEL CHANGES IN ALL DEVICES
        //    byte level = device.Level;
        //    //Check to see if any device state/level has changed.
        //    if (level != thisDevice.Level)
        //    {
        //        thisDevice.prevLevel = thisDevice.Level;
        //        thisDevice.Level = level; //set MasterDeviceList
        //        this.DeviceInfoChange(thisDevice.GlbUniqueID(), changeType.LevelChanged, VerboseRepoll); //call event                                                
        //    }
        //    #endregion

        //    #region DETECT THERMOSTAT SPECIFIC CHANGES
        //    if (devicetype.Contains("GeneralThermostat"))
        //    {
        //        ControlThink.ZWave.Devices.Specific.GeneralThermostatV2 thermostat = (ControlThink.ZWave.Devices.Specific.GeneralThermostatV2)device;

        //        int coolpoint = (int)thermostat.ThermostatSetpoints[ThermostatSetpointType.Cooling1].Temperature.ToFahrenheit();
        //        //If ThermostatSetpoints[x] returns 0 C and gets converted to 32 F recheck to make sure it is the intended figure. 
        //        //Either the ControlThink Stick or certian thermostats falsely return Heat and Cool points of 32 F, upon a 
        //        //second query they return the proper value therefor we will requery if we initially get 32
        //        if (coolpoint == 32)
        //        {
        //            Thread.Sleep(200);
        //            coolpoint = (int)thermostat.ThermostatSetpoints[ThermostatSetpointType.Cooling1].Temperature.ToFahrenheit();
        //        }

        //        int heatpoint = (int)thermostat.ThermostatSetpoints[ThermostatSetpointType.Heating1].Temperature.ToFahrenheit();
        //        if (heatpoint == 32)
        //        {
        //            Thread.Sleep(200);
        //            heatpoint = (int)thermostat.ThermostatSetpoints[ThermostatSetpointType.Heating1].Temperature.ToFahrenheit();
        //        }

        //        int currenttemp = (int)thermostat.ThermostatTemperature.ToFahrenheit();
        //        int fanmode = (int)thermostat.ThermostatFanMode;
        //        int mode = (int)thermostat.ThermostatMode;
        //        level = thermostat.Level;
        //        string currentstate = thermostat.ThermostatOperatingState.ToString() + "-" + thermostat.ThermostatFanMode.ToString();


        //        if (thisDevice.Temp != currenttemp)
        //        {
        //            thisDevice.prevTemp = thisDevice.Temp; //Save old temp
        //            thisDevice.Temp = currenttemp; //Save new Temp
        //            this.DeviceInfoChange(thisDevice.GlbUniqueID(), changeType.TempChanged, VerboseRepoll); //call event 
        //        }

        //        if (thisDevice.CoolPoint != coolpoint)
        //        {
        //            thisDevice.prevCoolPoint = thisDevice.CoolPoint;
        //            thisDevice.CoolPoint = coolpoint;
        //            this.DeviceInfoChange(thisDevice.GlbUniqueID(), changeType.CoolPointChanged, VerboseRepoll); //call event
        //        }

        //        if (thisDevice.HeatPoint != heatpoint)
        //        {
        //            thisDevice.prevHeatPoint = thisDevice.HeatPoint;
        //            thisDevice.HeatPoint = heatpoint;
        //            this.DeviceInfoChange(thisDevice.GlbUniqueID(), changeType.HeatPointChanged, VerboseRepoll); //call event
        //        }

        //        if (thisDevice.FanMode != fanmode)
        //        {
        //            thisDevice.prevFanMode = thisDevice.FanMode;
        //            thisDevice.FanMode = fanmode;
        //            this.DeviceInfoChange(thisDevice.GlbUniqueID(), changeType.FanModeChanged, VerboseRepoll); //call event
        //        }

        //        if (thisDevice.HeatCoolMode != mode)
        //        {
        //            thisDevice.prevHeatCoolMode = thisDevice.HeatCoolMode;
        //            thisDevice.HeatCoolMode = mode;
        //            this.DeviceInfoChange(thisDevice.GlbUniqueID(), changeType.HeatCoolModeChanged, VerboseRepoll); //call event
        //        }

        //        if (thisDevice.CurrentState != currentstate)
        //        {
        //            thisDevice.prevCurrentState = thisDevice.CurrentState;
        //            thisDevice.CurrentState = currentstate;
        //            this.DeviceInfoChange(thisDevice.GlbUniqueID(), changeType.CurrentStateChanged, VerboseRepoll); //call event
        //        }
        //    }
        //    #endregion
        //}

        private void device_LevelChanged(object sender, LevelChangedEventArgs e)
        {
            ControlThink.ZWave.Devices.ZWaveDevice device = (ControlThink.ZWave.Devices.ZWaveDevice)sender;
            //formzVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "ZWave device sent level change notification. Node: " + device.NodeID + " level: " + e.Level + ".", LOG_INTERFACE);

            //for each device previously discovered
            foreach (ZWaveDevice thisDevice in formzVirtualScenesMain.MasterDevices)
            {
                //if Control Stick device == device in memory
                if (this.ControlThinkController.HomeID.ToString() + device.NodeID.ToString() == thisDevice.GlbUniqueID())
                {
                    thisDevice.prevLevel = thisDevice.Level;  //Save last level
                    thisDevice.Level = e.Level; //set MasterDeviceList

                    if(thisDevice.prevLevel != thisDevice.Level)
                        this.DeviceInfoChange(thisDevice.GlbUniqueID(), changeType.LevelChanged, true); //call event   
                }
            }
        }

        private void device_PollFailed(object sender, PollFailedEventArgs e)
        {
            ControlThink.ZWave.Devices.ZWaveDevice device = (ControlThink.ZWave.Devices.ZWaveDevice)sender;
            formzVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "Poll Failed. Node: " + device.NodeID + " property: " + e.PropertyName.ToString() + ".", LOG_INTERFACE);
        }

        /// <summary>
        /// THIS IS ONLY HERE TO PROVE EVEN LUTRON DEVICES DO NOT SEND DEVICE LEVEL CHANGES PROPERLY
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ControlThinkController_LevelChanged(object sender, ControlThink.ZWave.ZWaveController.LevelChangedEventArgs e)
        {
            formzVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "ZWave device sent level change notification. Node: " + e.OriginDevice.NodeID + ".", LOG_INTERFACE);
            //formzVirtualScenesMain.refresher.RePollDevices(e.OriginDevice.NodeID);
        }

        private void ControlThinkUSBConnectedEvent(object sender, EventArgs e)
        {
            formzVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "ControlThink USB Connected to HomeId - " + Convert.ToString(ControlThinkController.HomeID),LOG_INTERFACE);
            SubscribetoDeviceEvents();           
        }

        private void ControlThinkUSBDisconnectEvent(object sender, EventArgs e)
        {
            formzVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "ControlThink USB Disconnected.",LOG_INTERFACE);
        }

        private void ControlThinkUSBNotRespondingEvent(object sender, EventArgs e)
        {
            formzVirtualScenesMain.AddLogEntry(UrgencyLevel.WARNING, "ControlThink USB Not Responding.",LOG_INTERFACE);
            try
            {
                ControlThinkController.Disconnect();
            }
            catch
            {
            }
        }

        #endregion 

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
