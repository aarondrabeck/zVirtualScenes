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
        private static string LOG_INTERFACE = "CONTROL THINK";
        public formzVirtualScenes formzVirtualScenesMain;
        public readonly ZWaveController ControlThinkController = new ZWaveController();
        public BackgroundWorker ReloadDevicesWorker;

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

        private void ReloadDevicesWorker_DoWork(object sender, DoWorkEventArgs e)
        {
             ControlThinkGetDevicesResult result = new ControlThinkGetDevicesResult();

             if (formzVirtualScenesMain.refresher.isRefreshing)
             {
                 result.NoErrors = false;
                 result.fatalError = true;
                 result.ErrorDescription = "Cannot refresh when repolling. Try agian later."; 
             }  

            //Connect to ThinkStick
            if (!ControlThinkController.IsConnected & !result.fatalError)
            {
                try
                {
                    //throw new System.Exception("error connectings");
                    ControlThinkController.SynchronizingObject = this;                    
                    ControlThinkController.Connect();                    
                }
                catch (Exception ex)
                {
                    result.NoErrors = false;
                    result.fatalError = true;
                    result.ErrorDescription = ex.Message;
                }
            }

            //Get ZWave Devices
            if (ControlThinkController.IsConnected && result.NoErrors)
            {
                foreach (ControlThink.ZWave.Devices.ZWaveDevice device in ControlThinkController.Devices)
                {
                    //Store device info for speed 
                    ControlThink.ZWave.Devices.ZWaveDevice DeviceFoundOnNetowrk = device;

                    try
                    {
                        //throw new System.Exception("error test");
                        formzVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "Found " + DeviceFoundOnNetowrk.ToString() + ".", LOG_INTERFACE);

                        if (!DeviceFoundOnNetowrk.ToString().Contains("Controller")) //Do not include ZWave controllers for now...
                        {
                            //Save ControlThink Device to zVirtualScenesDevice
                            ZWaveDevice newDevice = new ZWaveDevice();
                            newDevice.HomeID = ControlThinkController.HomeID;
                            newDevice.NodeID = DeviceFoundOnNetowrk.NodeID;
                            newDevice.Level = DeviceFoundOnNetowrk.Level;

                            //BINARY SWITCHES
                            if (DeviceFoundOnNetowrk is ControlThink.ZWave.Devices.BinarySwitch)
                            {
                                newDevice.Type = ZWaveDevice.ZWaveDeviceTypes.BinarySwitch;
                                newDevice.Name = "Binary Switch";
                            }
                            //MULTILEVEL
                            else if (DeviceFoundOnNetowrk is ControlThink.ZWave.Devices.MultilevelSwitch)
                            {
                                newDevice.Type = ZWaveDevice.ZWaveDeviceTypes.MultiLevelSwitch;
                                newDevice.Name = "Multi-level Switch";
                            }
                            //Therostats
                            else if (DeviceFoundOnNetowrk is ControlThink.ZWave.Devices.Thermostat)
                            {
                                newDevice.Type = ZWaveDevice.ZWaveDeviceTypes.Thermostat;
                                newDevice.Name = "Thermostat";

                                ControlThink.ZWave.Devices.Specific.GeneralThermostatV2 thermostat = (ControlThink.ZWave.Devices.Specific.GeneralThermostatV2)DeviceFoundOnNetowrk;
                                newDevice.Temp = (int)thermostat.ThermostatTemperature.ToFahrenheit();
                                newDevice.CoolPoint = (int)thermostat.ThermostatSetpoints[ThermostatSetpointType.Cooling1].Temperature.ToFahrenheit();
                                newDevice.HeatPoint = (int)thermostat.ThermostatSetpoints[ThermostatSetpointType.Heating1].Temperature.ToFahrenheit();
                                newDevice.FanMode = (int)thermostat.ThermostatFanMode;
                                newDevice.HeatCoolMode = (int)thermostat.ThermostatMode;
                                newDevice.Level = thermostat.Level;
                                newDevice.CurrentState = thermostat.ThermostatOperatingState.ToString() + "-" + thermostat.ThermostatFanMode.ToString();
                            }
                            else
                            {
                                formzVirtualScenesMain.AddLogEntry(UrgencyLevel.WARNING, "Device type  " + DeviceFoundOnNetowrk.ToString() + " UNKNOWN.", LOG_INTERFACE);
                            }                            
                            result.NewDeviceList.Add(newDevice);                            
                        }
                    }
                    catch (Exception ex)
                    {
                        result.NoErrors = false;
                        result.ErrorDescription = ex.Message;
                    }
                }                 
            }
            e.Result = result;
            return;
        }

        private void ReloadDevicesWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ControlThinkGetDevicesResult result = (ControlThinkGetDevicesResult)e.Result;
            if (result != null && !result.fatalError)
            {
                if(!result.NoErrors)
                    formzVirtualScenesMain.AddLogEntry(UrgencyLevel.WARNING, result.ErrorDescription, LOG_INTERFACE); 

                formzVirtualScenesMain.MasterDevices.Clear();

                foreach (ZWaveDevice newDevice in result.NewDeviceList)
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
                            if(newDevice.GroupName != "")
                                formzVirtualScenesMain.groups.Add(newDevice.GroupName);
                        }
                    }

                    formzVirtualScenesMain.MasterDevices.Add(newDevice);
                }
                formzVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "ControlThink USB Loaded " + formzVirtualScenesMain.MasterDevices.Count() + " Devices.", LOG_INTERFACE);
            }
            else
                formzVirtualScenesMain.AddLogEntry(UrgencyLevel.ERROR, result.ErrorDescription, LOG_INTERFACE); 
                          
        }

        public void ConnectAndLoadDevices()
        {
            if (ReloadDevicesWorker.IsBusy)
                formzVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "ControlThink Reload Already Running.", LOG_INTERFACE);
            else
            {
                ReloadDevicesWorker.RunWorkerAsync();
            }
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
    }

    public class ControlThinkGetDevicesResult
    {
        public bool NoErrors;
        public bool fatalError;
        public BindingList<ZWaveDevice> NewDeviceList = new BindingList<ZWaveDevice>();
        public string ErrorDescription; 

        public ControlThinkGetDevicesResult()
        {
            fatalError = false; 
            NoErrors = true; 
        }
    }
}
