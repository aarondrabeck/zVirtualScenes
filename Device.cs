using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace zVirtualScenesApplication 
{

    public class Device : INotifyPropertyChanged //use INotifyPropertyChanged to update binded listviews in the GUI on data changes
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public zVirtualScenes zVirtualScenesMain;


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
        private string _CurrentState;
        public string CurrentState
        {
            get { return _CurrentState; }
            set { GlobalFunctions.Set(this, "CurrentState", ref _CurrentState, value, PropertyChanged); }
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
        #endregion

        //Constructor
        public Device(zVirtualScenes zvsm)
        {
            zVirtualScenesMain = zvsm;
            this.HomeID = 0;
            this.NodeID = 0;
            _Name = "Default Device";
            _Level = 0;
            this.Type = "Unknown"; 
            _FanMode = -1;
            _HeatCoolMode = -1;
            _CoolPoint = -1;
            _HeatPoint = -1;
        }

        public enum ThermostatFanMode
        {
            DoNotChange = -1,
            AutoLow = 0,
            OnLow = 1,
            AutoHigh = 2,
            OnHigh = 3,
        }

        public enum ThermostatMode
        {
            DoNotChange = -1,
            Off = 0,
            Heat = 1,
            Cool = 2,
            Auto = 3,
            AuxiliaryOrEmergencyHeat = 4,
            Resume = 5,
            FanOnly = 6,
            Furnace = 7,
            DryAir = 8,
            MoistAir = 9,
            AutoChangeover = 10,
            HeatEcon = 11,
            CoolEcon = 12,
        }

        public enum EnergyMode
        {
            DoNotChange = -1,
            EnergySavingMode = 0,           
            ComfortMode = 255,
        }

        #region Methods

            public override string ToString()
            {
                return _Name + " - ID:" + NodeID + " - " + GetFomattedType();
            }

            //Light Switch Socket Format - DEVICE~Bedroom Lights~0~60~MultiLevelSceneSwitch
            public string ToLightSwitchSocketString()
            {
                if (Type != null && Type.Contains("MultilevelPowerSwitch"))
                    return "DEVICE~" + _Name + "~" + this.NodeID + "~" + _Level + "~" + this.Type;
                else if (Type != null && (Type.Contains("GeneralThermostatV2") || Type.Contains("GeneralThermostat")))
                    return "DEVICE~" + _Name + "~" + this.NodeID + "~" + Temp + "~" + this.Type;
                return "Unknown Device";
            }


            public string GetFomattedType()
            {
                if (Type != null && Type.Contains("MultilevelPowerSwitch"))
                {
                    if (Level > 99)
                        return "Level: Unknown ON State";
                    else
                        return "Level: " + _Level + "%";
                }
                else if (Type != null && (Type.Contains("GeneralThermostatV2") || Type.Contains("GeneralThermostat")))
                    return  "Mode:" + Enum.GetName(typeof(Device.ThermostatMode), _HeatCoolMode) +
                            " | Fan:" + Enum.GetName(typeof(Device.ThermostatFanMode), _FanMode) +
                            " | SetPoint:" + Enum.GetName(typeof(Device.EnergyMode), _Level) + "(" + _CoolPoint.ToString() + "/" + _HeatPoint.ToString() + ")" +
                            " | Currently:" + Temp + "° " + _CurrentState;
                return "Unknown Device";
            }

            public string GlbUniqueID()
            {
                return this.HomeID.ToString() + this.NodeID.ToString();
            }

            //Type casting device to Action
            public static implicit operator Action(Device instance)
            {
                Action action = new Action();
                action.Type = instance.Type;
                action.Name = instance.Name;
                action.NodeID = instance.NodeID;
                action.HomeID = instance.HomeID;
                action.Level = instance.Level; 
                action.Level = instance.Level;
                action.Temp = instance.Temp;             
                return action;
            }

        #endregion



    }
}
