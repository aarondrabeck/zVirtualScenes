using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ControlThink.ZWave.Devices;

namespace zVirtualScenesApplication
{
    class ControlThinkRefresh
    {
        formzVirtualScenes zVirtualScenesMain;
        public delegate void DeviceInfoChangeEventHandler(string GlbUniqueID, string TypeOfChange);
        public event DeviceInfoChangeEventHandler DeviceInfoChange;

        public ControlThinkRefresh(formzVirtualScenes _zVirtualScenesMain)
        {
            this.zVirtualScenesMain = _zVirtualScenesMain;
        }


        /// <summary>
        /// This continually looks for changes in device modes.  When one is detected, it will change MasterDeviceList and call the DeviceInfoChangeEventHandler. 
        /// </summary>
        public void RefreshThread()
        {
            while (true)
            {
                if (zVirtualScenesMain.ControlThinkController.IsConnected)
                {
                    foreach (ZWaveDevice device in zVirtualScenesMain.ControlThinkController.Devices)
                    {
                        try
                        {
                            //Allowed Z-wave Devices
                            if (device is ControlThink.ZWave.Devices.Specific.MultilevelPowerSwitch ||
                               device is ControlThink.ZWave.Devices.Specific.GeneralThermostatV2 ||
                               device is ControlThink.ZWave.Devices.Specific.GeneralThermostat)
                            {
                                foreach (Device thisDevice in zVirtualScenesMain.MasterDevices)
                                {
                                    if (zVirtualScenesMain.ControlThinkController.HomeID.ToString() + device.NodeID.ToString() == thisDevice.GlbUniqueID())
                                    {
                                        #region MultilevelPowerSwitch
                                        if (device is ControlThink.ZWave.Devices.Specific.MultilevelPowerSwitch)
                                        {
                                            if (device.Level != thisDevice.Level)
                                            {
                                                thisDevice.prevLevel = thisDevice.Level;
                                                thisDevice.Level = device.Level; //set MasterDeviceList
                                                this.DeviceInfoChange(thisDevice.GlbUniqueID(), "level"); //call event                                                
                                            }
                                        }
                                        #endregion
                                        else if (device is ControlThink.ZWave.Devices.Specific.GeneralThermostatV2 || device is ControlThink.ZWave.Devices.Specific.GeneralThermostat)
                                        {
                                            ControlThink.ZWave.Devices.Specific.GeneralThermostatV2 thermostat = (ControlThink.ZWave.Devices.Specific.GeneralThermostatV2)device;

                                            if ((int)thermostat.ThermostatTemperature.ToFahrenheit() != thisDevice.Temp)
                                            {
                                                thisDevice.prevTemp = thisDevice.Temp; //Save old temp
                                                thisDevice.Temp = (int)thermostat.ThermostatTemperature.ToFahrenheit(); //Save new Temp
                                                this.DeviceInfoChange(thisDevice.GlbUniqueID(), "Temp"); //call event 
                                            }

                                            if (thisDevice.CoolPoint != (int)thermostat.ThermostatSetpoints[ThermostatSetpointType.Cooling1].Temperature.ToFahrenheit())
                                            {
                                                thisDevice.prevCoolPoint = thisDevice.CoolPoint; //Save old temp
                                                thisDevice.CoolPoint = (int)thermostat.ThermostatSetpoints[ThermostatSetpointType.Cooling1].Temperature.ToFahrenheit();
                                                this.DeviceInfoChange(thisDevice.GlbUniqueID(), "CoolPoint"); //call event
                                            }

                                            if (thisDevice.HeatPoint != (int)thermostat.ThermostatSetpoints[ThermostatSetpointType.Heating1].Temperature.ToFahrenheit())
                                            {
                                                thisDevice.prevHeatPoint = thisDevice.HeatPoint; //Save old temp
                                                thisDevice.HeatPoint = (int)thermostat.ThermostatSetpoints[ThermostatSetpointType.Heating1].Temperature.ToFahrenheit();
                                                this.DeviceInfoChange(thisDevice.GlbUniqueID(), "HeatPoint"); //call event
                                            }

                                            if (thisDevice.FanMode != (int)thermostat.ThermostatFanMode)
                                            {
                                                thisDevice.prevFanMode = thisDevice.FanMode; //Save old temp
                                                thisDevice.FanMode = (int)thermostat.ThermostatFanMode;
                                                this.DeviceInfoChange(thisDevice.GlbUniqueID(), "FanMode"); //call event
                                            }

                                            if (thisDevice.HeatCoolMode != (int)thermostat.ThermostatMode)
                                            {
                                                thisDevice.prevHeatCoolMode = thisDevice.HeatCoolMode; //Save old temp
                                                thisDevice.HeatCoolMode = (int)thermostat.ThermostatMode;
                                                this.DeviceInfoChange(thisDevice.GlbUniqueID(), "HeatCoolMode"); //call event
                                            }

                                            if (thisDevice.Level != thermostat.Level)
                                            {
                                                thisDevice.prevLevel = thisDevice.Level; //Save old temp
                                                thisDevice.Level = thermostat.Level;
                                                this.DeviceInfoChange(thisDevice.GlbUniqueID(), "Level"); //call event
                                            }

                                            if (thisDevice.CurrentState != thermostat.ThermostatOperatingState.ToString() + "-" + thermostat.ThermostatFanMode.ToString())
                                            {
                                                thisDevice.prevCurrentState = thisDevice.CurrentState; //Save old temp
                                                thisDevice.CurrentState = thermostat.ThermostatOperatingState.ToString() + "-" + thermostat.ThermostatFanMode.ToString();
                                                this.DeviceInfoChange(thisDevice.GlbUniqueID(), "CurrentState"); //call event
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            zVirtualScenesMain.LogThis(2, "ControlThink USB Trouble Getting Device Status: " + ex.Message);
                        }
                    }
                }
                Thread.Sleep(10000);
            }

        }
    }
}
