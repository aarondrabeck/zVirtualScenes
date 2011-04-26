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
            BindingList<ZWaveDevice> DevicesFound = new BindingList<ZWaveDevice>();

             if (formzVirtualScenesMain.refresher.isRefreshing)             
                 throw new Exception("Cannot refresh when repolling. Try agian later.");             

            //Connect to ThinkStick
            if (!ControlThinkController.IsConnected)
            {
                    ControlThinkController.SynchronizingObject = this;                    
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
                formzVirtualScenesMain.refresher.RePollDevices();
            }                          
        }
        
        public void ConnectAndFindDevices()
        {
            if (ReloadDevicesWorker.IsBusy)
            {
                formzVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "ControlThink Reload Already Running.", LOG_INTERFACE);
                return;
            }
            if(formzVirtualScenesMain.refresher.isRefreshing)
            {
                formzVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "ControlThink USB is busy refreshing, cannot look for new devices at this time.", LOG_INTERFACE);
                return;
            }
             
                ReloadDevicesWorker.RunWorkerAsync();
            
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
}
