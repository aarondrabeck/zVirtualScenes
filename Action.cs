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
        public string Type { get; set; }
        public int Mode { get; set; }
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
            this.Type = "";
            this.NodeID = 0;                       
            this.Mode = -1;
            this.Temp = 0;
            this.HomeID = 0;

        }

        public override string ToString()
        {
            if(this.Type == "LauchAPP")
            {
                return "Launch: " + EXEPath;
            }
            else if (this.Type == "DelayTimer")
            {
                return "Wait " + this.TimerDuration / 1000 + " second(s)."; 
            }
            else
                return _Name + " - ID:" + NodeID + " - " + GetFomattedType();
        }

        public ActionResult Run(ZWaveController ControlThinkController)
        {

            #region Switches

            if (this.Type.Contains("BinaryPowerSwitch"))
            {
                foreach (ZWaveDevice device in ControlThinkController.Devices)
                {
                    if (device.NodeID == this.NodeID)
                    {
                        device.Level = _Level;
                        return new ActionResult { ResultType = ActionResult.ResultTypes.Success, Description = "Ran Action. '" + this.Name + "' level set to " + (this.Level > 0 ? "ON" : "OFF") + "." };
                    }
                }
            }  

            if (this.Type.Contains("MultilevelPowerSwitch"))
            {
                foreach (ZWaveDevice device in ControlThinkController.Devices)
                {
                    if (device.NodeID == this.NodeID)
                    {                         
                        device.Level = _Level;
                        return new ActionResult { ResultType = ActionResult.ResultTypes.Success, Description = "Ran Action. '" + this.Name + "' level set to " + this.Level.ToString() + "." };
                    }
                }
            }               
            #endregion

            #region Thermostat
            else if (this.Type.Contains("GeneralThermostat") )
            {
                foreach (ZWaveDevice device in ControlThinkController.Devices)
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
                                ActionLog += " Mode set to: " + Enum.GetName(typeof(Device.ThermostatMode), _HeatCoolMode) + "  ";
                            }

                            if (_FanMode != -1)
                            {
                                thermostat.ThermostatFanMode = (ControlThink.ZWave.Devices.ThermostatFanMode)_FanMode;
                                ActionLog += " FanMode set to: " + Enum.GetName(typeof(Device.ThermostatFanMode), _FanMode) + "  ";
                            }

                            if (_EngeryMode != -1)                       
                            {     
                                thermostat.Level = (byte)_EngeryMode;
                                ActionLog += " EnergyMode set to: " + Enum.GetName(typeof(Device.EnergyMode), _EngeryMode) + "  ";
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

                            return new ActionResult { ResultType = ActionResult.ResultTypes.Success, Description = ActionLog};
                    
                        }
                        catch (Exception e)
                        {
                            return new ActionResult { ResultType = ActionResult.ResultTypes.Error, Description = "Failed to set Thermostat. Mode might not be allowed. - " + e};
                        }
                    }
                }
            }
            #endregion

            #region Non Zwave
            else if (this.Type == "LauchAPP")
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
            else if (this.Type == "DelayTimer")
            {
                System.Threading.Thread.Sleep((int)this.TimerDuration);
            }
            #endregion

           return new ActionResult { ResultType= ActionResult.ResultTypes.Success, Description = "Ran action on " + this.Type + "(" + this.Name +")." }; 
        }

        public string GetFomattedType()
        {                       
            if (Type != null && Type.Contains("BinaryPowerSwitch"))
            {
                return (Level > 0 ? "State: ON" : "State: OFF");
            }
            if (Type != null && Type.Contains("MultilevelPowerSwitch"))
            {
                if (Level > 255)
                    return "Level: Unknown ON State";
                else
                    return "Level: " + Level + "%";
            }
            else if (Type != null && Type.Contains("GeneralThermostat"))
            {
                string actions = "(";

                //Set Heat Cool Mode
                if (_HeatCoolMode != -1)
                    actions += " Set Mode: " + Enum.GetName(typeof(Device.ThermostatMode), _HeatCoolMode); 
                
                if (_FanMode != -1)
                    actions += " Set FanMode: " + Enum.GetName(typeof(Device.ThermostatFanMode), _FanMode);

                if (_EngeryMode != -1)
                {
                      if (_EngeryMode == 0) //set EnergySavingMode
                         actions += " Set EnergySavingMode.";
                     else if (_EngeryMode == 255) //ComfortMode
                         actions += " Set ComfortMode.";                    
                }

                if(_CoolPoint != -1)
                    actions += " Set CustomCoolpoint " + _CoolPoint.ToString() + "°.";

                if(_HeatPoint != -1)
                    actions += " Set CustomHeatpoint " + _HeatPoint.ToString() + "°.";
                   
                actions += ")";
                return actions;
            }
           
            return "Unknown Device"; 
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
