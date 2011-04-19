using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using ControlThink.ZWave;
using ControlThink.ZWave.Devices;
using System.Windows.Forms;
using System.ComponentModel;

namespace zVirtualScenesApplication
{
    public class Action : INotifyPropertyChanged //use INotifyPropertyChanged to update binded listviews in the GUI on data changes
    {
        GlobalFunctions GlbFnct = new GlobalFunctions();
        public event PropertyChangedEventHandler PropertyChanged;

        #region Properties and Variables
        //Properties that require PropertyChangedEvent to fire to sync GUI
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { GlobalFunctions.Set(this, "Name", ref _Name, value, PropertyChanged); }
        }

        private byte _Level;
        public byte Level
        {
            get { return _Level; }
            set { GlobalFunctions.Set(this, "Level", ref _Level, value, PropertyChanged); }
        }
        
        private int _FanMode;
        public int FanMode
        {
            get { return _FanMode; }
            set { GlobalFunctions.Set(this, "FanMode", ref _FanMode, value, PropertyChanged); }
        }
        private int _HeatCoolMode;
        public int HeatCoolMode
        {
            get { return _HeatCoolMode; }
            set { GlobalFunctions.Set(this, "HeatCoolMode", ref _HeatCoolMode, value, PropertyChanged); }
        }
        private int _EngeryMode;
        public int EngeryMode
        {
            get { return _EngeryMode; }
            set { GlobalFunctions.Set(this, "EngeryMode", ref _EngeryMode, value, PropertyChanged); }
        }
        private int _HeatPoint;
        public int HeatPoint
        {
            get { return _HeatPoint; }
            set { GlobalFunctions.Set(this, "HeatPoint", ref _HeatPoint, value, PropertyChanged); }
        }
        private int _CoolPoint;
        public int CoolPoint
        {
            get { return _CoolPoint; }
            set { GlobalFunctions.Set(this, "CoolPoint", ref _CoolPoint, value, PropertyChanged); }
        }
        private int _Temp;
        public int Temp
        {
            get { return _Temp; }
            set { GlobalFunctions.Set(this, "Temp", ref _Temp, value, PropertyChanged); }
        }
        private string _EXEPath;
        public string EXEPath
        {
            get { return _EXEPath; }
            set { GlobalFunctions.Set(this, "EXEPath", ref _EXEPath, value, PropertyChanged); }
        }

        //Standard Properties        
        public uint HomeID { get; set; }
        public byte NodeID { get; set; }
        public ZWaveDevice.ZWaveDeviceTypes ZWaveType { get; set; }
        public ActionTypes Type { get; set; }
        public int Mode { get; set; }
        public bool MomentaryOnMode { get; set; }
        public int MomentaryTimespan { get; set; }
        public bool SkipWhenLight;
        public bool SkipWhenDark;

        /// <summary>
        /// In Milliseconds
        /// </summary>
        public int TimerDuration { get; set; } 

        #endregion

        public Action()
        {
            _Name = "";
            _Level = 0;
            _FanMode = -1; ;
            _HeatCoolMode = -1;
            _EngeryMode = -1; 
            _HeatPoint = -1;
            _CoolPoint = -1;
            _EXEPath = "";
            this.Type = ActionTypes.ZWaveDevice;
            this.ZWaveType = ZWaveDevice.ZWaveDeviceTypes.Unknown;
            this.NodeID = 0;                       
            this.Mode = -1;
            this.Temp = 0;
            this.HomeID = 0;
            this.TimerDuration = 5000;
            this.MomentaryOnMode = false;
            this.MomentaryTimespan = 1;
            this.SkipWhenDark = false;
            this.SkipWhenLight = false;
        }

        public enum ActionTypes
        {
            ZWaveDevice = 1,
            LauchAPP = 2,
            DelayTimer = 3
        }

        public override string ToString()
        {
            if (Type == ActionTypes.ZWaveDevice)
            {
                return _Name + " - ID:" + NodeID + " - " + GetFomattedType();
            }
            else if (Type == ActionTypes.LauchAPP)
            {
                return "Launch: " + EXEPath;
            }
            else if (Type == ActionTypes.DelayTimer)
            {
                return "Wait " + this.TimerDuration / 1000 + " second(s).";
            }
            return "Action Type Unknown!";
        }

        public string TypeIcon()
        {
            if (this.Type == ActionTypes.ZWaveDevice)
            {
                if (this.ZWaveType == ZWaveDevice.ZWaveDeviceTypes.Thermostat)
                    return "20zwave-thermostat.png";
                else if (this.ZWaveType == ZWaveDevice.ZWaveDeviceTypes.MultiLevelSwitch)
                    return "20dimmer.png";
                else if (this.ZWaveType == ZWaveDevice.ZWaveDeviceTypes.BinarySwitch)
                    return "20switch.png";
                else
                    return "20radio2.png";                 
            }                
            else if (Type == ActionTypes.LauchAPP)
                return "20exe.png";
            else if (Type == ActionTypes.DelayTimer)
                return "20delay.png";

            return "";
        }

        public string ActionToString()
        {
            if (Type == ActionTypes.ZWaveDevice)
                return GetFomattedType();
            else if (Type == ActionTypes.LauchAPP)
                return "Start: " + EXEPath;
            else if (Type == ActionTypes.DelayTimer)
                return "Wait " + this.TimerDuration / 1000 + " second(s).";

            return "Unknown";
        }

        public ActionResult Run(formzVirtualScenes formzVirtualScenesMAIN)
        {
            if (this.SkipWhenLight && !formzVirtualScenesMAIN.isDark())
                return new ActionResult { ResultType = ActionResult.ResultTypes.Success, Description = "Action skipped because it is light out." };

            if (this.SkipWhenDark && formzVirtualScenesMAIN.isDark())
                return new ActionResult { ResultType = ActionResult.ResultTypes.Success, Description = "Action skipped because it is dark out." };

            if (Type == ActionTypes.ZWaveDevice)
            {                
                if (formzVirtualScenesMAIN.ControlThinkInt.ControlThinkController.IsConnected)
                {
                    #region BinarySwitchs

                    if (this.ZWaveType == ZWaveDevice.ZWaveDeviceTypes.BinarySwitch)
                    {
                        foreach (ControlThink.ZWave.Devices.ZWaveDevice device in formzVirtualScenesMAIN.ControlThinkInt.ControlThinkController.Devices)
                        {
                            if (device.NodeID == this.NodeID)
                            {
                                try
                                {
                                    if (!MomentaryOnMode)
                                    {
                                        device.Level = _Level;
                                        return new ActionResult { ResultType = ActionResult.ResultTypes.Success, Description = "Ran Action. '" + this.Name + "' level set to " + (this.Level > 0 ? "ON" : "OFF") + "." };
                                    }
                                    else
                                    {
                                        device.Level = 255;
                                        System.Threading.Thread.Sleep(this.MomentaryTimespan * 1000);
                                        device.Level = 0;
                                        return new ActionResult { ResultType = ActionResult.ResultTypes.Success, Description = "Ran Action. '" + this.Name + "' toggled on momentarily." };
                                    }
                                }
                                catch (Exception e)
                                {
                                    return new ActionResult { ResultType = ActionResult.ResultTypes.Error, Description = "Failed to set '" + this.Name + ". - " + e.Message };

                                }

                            }
                        }
                    }

                    #endregion

                    #region MultiLevel Switch

                    else if (this.ZWaveType == ZWaveDevice.ZWaveDeviceTypes.MultiLevelSwitch)
                    {
                        foreach (ControlThink.ZWave.Devices.ZWaveDevice device in formzVirtualScenesMAIN.ControlThinkInt.ControlThinkController.Devices)
                        {
                            if (device.NodeID == this.NodeID)
                            {
                                try
                                {
                                    device.Level = _Level;
                                    return new ActionResult { ResultType = ActionResult.ResultTypes.Success, Description = "Ran Action. '" + this.Name + "' level set to " + this.Level.ToString() + "." };
                                }
                                catch (Exception e)
                                {
                                    return new ActionResult { ResultType = ActionResult.ResultTypes.Error, Description = "Failed to set '" + this.Name + ". - " + e.Message };

                                }
                            }
                        }
                    }

                    #endregion

                    #region Thermostat

                    else if (this.ZWaveType == ZWaveDevice.ZWaveDeviceTypes.Thermostat)
                    {

                        if (_HeatCoolMode == -1 && _FanMode == -1 && _EngeryMode == -1 && _HeatPoint == -1 && _CoolPoint == -1)
                        {
                            return new ActionResult { ResultType = ActionResult.ResultTypes.Error, Description = "Failed to set Thermostat. Nothing to set!" };
                        }

                        foreach (ControlThink.ZWave.Devices.ZWaveDevice device in formzVirtualScenesMAIN.ControlThinkInt.ControlThinkController.Devices)
                        {
                            if (device.NodeID == this.NodeID)
                            {
                                string ActionLog = "Ran Action. '" + this.Name + "'";
                                try
                                {
                                    ControlThink.ZWave.Devices.Specific.GeneralThermostatV2 thermostat = (ControlThink.ZWave.Devices.Specific.GeneralThermostatV2)device;

                                    //Set Heat Cool Mode
                                    if (_HeatCoolMode != -1)
                                    {
                                        thermostat.ThermostatMode = (ControlThink.ZWave.Devices.ThermostatMode)_HeatCoolMode;
                                        ActionLog += " Mode set to: " + Enum.GetName(typeof(ZWaveDevice.ThermostatMode), _HeatCoolMode) + "  ";
                                    }

                                    if (_FanMode != -1)
                                    {
                                        thermostat.ThermostatFanMode = (ControlThink.ZWave.Devices.ThermostatFanMode)_FanMode;
                                        ActionLog += " FanMode set to: " + Enum.GetName(typeof(ZWaveDevice.ThermostatFanMode), _FanMode) + "  ";
                                    }

                                    if (_EngeryMode != -1)
                                    {
                                        thermostat.Level = (byte)_EngeryMode;
                                        ActionLog += " EnergyMode set to: " + Enum.GetName(typeof(ZWaveDevice.EnergyMode), _EngeryMode) + "  ";
                                    }

                                    if (_CoolPoint != -1)
                                    {
                                        thermostat.ThermostatSetpoints[ThermostatSetpointType.Cooling1].Temperature = new Temperature(_CoolPoint, TemperatureScale.Fahrenheit);
                                        ActionLog += " CoolPoint set to: " + _CoolPoint.ToString() + "  ";
                                    }

                                    if (_HeatPoint != -1)
                                    {
                                        thermostat.ThermostatSetpoints[ThermostatSetpointType.Heating1].Temperature = new Temperature(_HeatPoint, TemperatureScale.Fahrenheit);
                                        ActionLog += " HeatPoint set to: " + _HeatPoint.ToString() + "  ";
                                    }

                                    return new ActionResult { ResultType = ActionResult.ResultTypes.Success, Description = ActionLog };

                                }
                                catch (Exception e)
                                {
                                    return new ActionResult { ResultType = ActionResult.ResultTypes.Error, Description = "Failed to set Thermostat. Mode might not be allowed. - " + e.Message };
                                }
                            }
                        }
                    }
                    #endregion
                }
                else
                    return new ActionResult { ResultType = ActionResult.ResultTypes.Error, Description = "ControlThink USB Controller Disconnected." };
            }
            else if (this.Type == ActionTypes.LauchAPP)
            {
                try
                {
                    System.Diagnostics.Process.Start(EXEPath);
                    return new ActionResult { ResultType = ActionResult.ResultTypes.Success, Description = "Ran Action. Launched (" + EXEPath + ") ." };
                }
                catch (Exception e)
                {
                    return new ActionResult { ResultType = ActionResult.ResultTypes.Error, Description = "Failed to launch (" + EXEPath + ") - " + e };
                }
            }
            else if (this.Type == ActionTypes.DelayTimer)
            {
                System.Threading.Thread.Sleep((int)this.TimerDuration);
            }

           return new ActionResult { ResultType= ActionResult.ResultTypes.Success, Description = "Ran action on " + this.Type + "(" + this.Name +")." }; 
        }

        public string GetFomattedType()
        {
            if (this.ZWaveType == ZWaveDevice.ZWaveDeviceTypes.BinarySwitch)
            {
                return (Level > 0 ? "Set State: ON" : "Set State: OFF");
            }
            else if (this.ZWaveType == ZWaveDevice.ZWaveDeviceTypes.MultiLevelSwitch)
            {
                if (Level > 255)
                    return "Set Level: Unknown ON State";
                else
                    return "Set Level: " + Level + "%";
            }
            else if (this.ZWaveType == ZWaveDevice.ZWaveDeviceTypes.Thermostat)
            {
                string actions = "";

                //Set Heat Cool Mode
                if (_HeatCoolMode != -1)                
                    actions += (actions == "" ? "" : ", ") + "Set Mode to " + Enum.GetName(typeof(ZWaveDevice.ThermostatMode), _HeatCoolMode);

                if (_FanMode != -1)
                    actions += (actions == "" ? "" : ", ") + "Set FanMode to " + Enum.GetName(typeof(ZWaveDevice.ThermostatFanMode), _FanMode);

                if (_EngeryMode != -1)
                {
                    if (_EngeryMode == 0) //set EnergySavingMode
                        actions += (actions == "" ? "" : ", ") + "Enable Energy Saving Mode";
                    else if (_EngeryMode == 255) //ComfortMode
                        actions += (actions == "" ? "" : ", ") + "Enable Comfort Mode";
                }

                if (_CoolPoint != -1)
                    actions += (actions == "" ? "" : ", ") + "Set Coolpoint to " + _CoolPoint.ToString() + "°";

                if (_HeatPoint != -1)
                    actions += (actions == "" ? "" : ", ") + "Set Heatpoint to " + _HeatPoint.ToString() + "°";

                return actions;
            }
           
            return "Unknown Action "; 
        }

        public string GlbUniqueID()
        {
            return this.HomeID.ToString() + this.NodeID.ToString();
        }

    }

    public class ActionResult
    {
        public enum ResultTypes
        {
            Success = 1,
            Error = 2
        }

        public ResultTypes ResultType { get; set; }
        public string Description { get; set; }
    }
}
