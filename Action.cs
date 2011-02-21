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

        //Standard Properties
        
        public uint HomeID { get; set; }
        public byte NodeID { get; set; }
        public string Type { get; set; }
        public int Mode { get; set; }        

        #endregion

        public Action()
        {
            _Name = "";
            _Level = 0;
            _FanMode = -1; ;
            _HeatCoolMode = -1;
            _EngeryMode = -1; 
            _HeatPoint = -1;
            CoolPoint = -1; 
            this.Type = "";
            this.NodeID = 0;                       
            this.Mode = -1;
            this.Temp = 0;
            this.HomeID = 0;

        }

        public override string ToString()
        {
            return _Name + " - ID:" + NodeID + " - " + GetFomattedType();
        }

        
        public void RunAction(zVirtualScenes zVirtualScenesMain)
        {
            if (this.Type.Contains("MultilevelPowerSwitch"))
            {
                foreach (ZWaveDevice device in zVirtualScenesMain.ControlThinkController.Devices)
                {
                    if (device.NodeID == this.NodeID)
                        device.Level = _Level;
                }

                //Update Level in MasterDevices rather than repolling controller
                foreach (Device device in zVirtualScenesMain.MasterDevices)
                {
                    if (device.NodeID == this.NodeID)
                    {
                        device.Level = _Level;
                    }
                }
            }
            else if (Type.Contains("GeneralThermostatV2") || Type.Contains("GeneralThermostat"))
            {
                foreach (ZWaveDevice device in zVirtualScenesMain.ControlThinkController.Devices)
                {
                    if (device.NodeID == this.NodeID)
                    {
                        try
                        {
                            ControlThink.ZWave.Devices.Specific.GeneralThermostatV2 thermostat = (ControlThink.ZWave.Devices.Specific.GeneralThermostatV2)device;

                            //Set Heat Cool Mode
                            if (_HeatCoolMode != -1)
                                thermostat.ThermostatMode = (ControlThink.ZWave.Devices.ThermostatMode)_HeatCoolMode;

                            if (_FanMode != -1)
                                thermostat.ThermostatFanMode = (ControlThink.ZWave.Devices.ThermostatFanMode)_FanMode;


                            if (_EngeryMode != -1)
                            {
                                if (_EngeryMode == 0) //set EnergySavingMode
                                    thermostat.Level = 0;
                                else if (_EngeryMode == 255) //ComfortMode
                                    thermostat.Level = 255;
                            }

                            if (_CoolPoint != -1)
                                thermostat.ThermostatSetpoints[ThermostatSetpointType.Cooling1].Temperature = new Temperature(_CoolPoint, TemperatureScale.Fahrenheit);

                            if (_HeatPoint != -1)
                                thermostat.ThermostatSetpoints[ThermostatSetpointType.Heating1].Temperature = new Temperature(_HeatPoint, TemperatureScale.Fahrenheit);
                        }
                        catch (Exception e)
                        {
                            zVirtualScenesMain.LogThis(2, "Failed to set Thermostat. Mode might not be allowed. - " + e);
                        }
                    }
                        
                }
                //REPOLL TERMOSTAT
                zVirtualScenesMain.ControlThinkGetDevices();
            }
        }

        public string GetFomattedType()
        {
            if (Type != null && Type.Contains("MultilevelPowerSwitch"))
            {
                if (Level > 255)
                    return "Level: Unknown ON State";
                else
                    return "Level: " + Level + "%";
            }
            else if (Type != null && (Type.Contains("GeneralThermostatV2") || Type.Contains("GeneralThermostat")))
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
}
