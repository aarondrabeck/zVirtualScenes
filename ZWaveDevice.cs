using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Xml.Serialization;

namespace zVirtualScenesApplication 
{

    public class ZWaveDevice : INotifyPropertyChanged //use INotifyPropertyChanged to update binded listviews in the GUI on data changes
    {
        //[XmlIgnore]
        //public formzVirtualScenes zVirtualScenesMain;
        public event PropertyChangedEventHandler PropertyChanged;

        #region Properties and Variables
        //Properties that require PropertyChangedEvent to fire to sync GUI
        public uint HomeID { get; set; }
        public byte NodeID { get; set; }
        public ZWaveDeviceTypes Type { get; set; }//Properties that require PropertyChangedEvent to fire to sync GUI
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { GlobalFunctions.Set(this, "Name", ref _Name, value, PropertyChanged); }
        }

        private string _GroupName;
        public string GroupName
        {
            get { return _GroupName; }
            set { GlobalFunctions.Set(this, "GroupName", ref _GroupName, value, PropertyChanged); }
        }

        public bool SendJabberNotifications { get; set; }

        public byte Level { get; set; }
        public byte prevLevel { get; set; }

        public int Temp { get; set; }
        public int prevTemp { get; set; }

        public int FanMode { get; set; }
        public int prevFanMode { get; set; }

        public int HeatCoolMode { get; set; }
        public int prevHeatCoolMode { get; set; }

        public string CurrentState { get; set; }
        public string prevCurrentState { get; set; }

        public int HeatPoint { get; set; }
        public int prevHeatPoint { get; set; }

        public int CoolPoint { get; set; }
        public int prevCoolPoint { get; set; }

        public int MinAlertTemp { get; set; }
        public int MaxAlertTemp { get; set; }

        public int NotificationDetailLevel { get; set; }
        public bool ShowInLightSwitchGUI { get; set; }
        public bool MomentaryOnMode { get; set; }
        public bool SendGrowlNotifications { get; set; }
        public int MomentaryTimespan { get; set; }

        public int RepollInterval { get; set; }
        public bool SubscribedToPollTimer { get; set; }

        [XmlIgnore]
        public RepollingTimer PollTimer = new RepollingTimer();

        #endregion

        //Constructor
        public ZWaveDevice()
        {
            this.HomeID = 0;
            this.NodeID = 0;
            _Name = "Default Device";
            this.Level = 0;
            this.Type = ZWaveDeviceTypes.Unknown;
            this.FanMode = -1;
            this.HeatCoolMode = -1;
            this.CoolPoint = -1;
            this.HeatPoint = -1;
            this.MinAlertTemp = 40;
            this.MaxAlertTemp = 90;
            this.NotificationDetailLevel = 1;
            _GroupName = "<None>";
            this.ShowInLightSwitchGUI = true;
            this.MomentaryOnMode = false;
            this.SendGrowlNotifications = true;
            this.MomentaryTimespan = 0;
            this.RepollInterval = 300;
            this.SubscribedToPollTimer = false; 
        }

        public enum ZWaveDeviceTypes
        {
            Unknown = -1,
            BinarySwitch = 1,
            MultiLevelSwitch = 2,
            Thermostat = 3,
            Sensor = 4
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
            return "(" + NodeID + ") " + _Name + " - " + GetStatus();
        }        

        public string DeviceIcon()
        {            
            if (this.Type == ZWaveDeviceTypes.Thermostat)
                return "20zwave-thermostat.png";
            else if (this.Type == ZWaveDeviceTypes.MultiLevelSwitch)
                return "20dimmer.png";
            else if (this.Type == ZWaveDeviceTypes.BinarySwitch)
                return "20switch.png";
            else
                return "20radio2.png";                 
        }

        public int GetLevelMeter()
        {
            if (Type == ZWaveDeviceTypes.Thermostat)
                return this.Temp;
            else
                return this.Level;
        }

        public string GetLevelText()
        {
            if (Type == ZWaveDeviceTypes.Thermostat)
                return this.Temp + " F";
            else if (Type == ZWaveDeviceTypes.BinarySwitch)            
                return (Level > 0 ? "ON" : "OFF");
            else
                return this.Level + "%";
        }

        public string GetStatus()
        {
            if (Type == ZWaveDeviceTypes.BinarySwitch)
            {
                return (Level > 0 ? "State: ON" : "State: OFF");
            }
            else if (Type == ZWaveDeviceTypes.Thermostat)
                return "Mode:" + Enum.GetName(typeof(ZWaveDevice.ThermostatMode), this.HeatCoolMode) +
                        " | Fan:" + Enum.GetName(typeof(ZWaveDevice.ThermostatFanMode), this.FanMode) +
                        " | SetPoint:" + Enum.GetName(typeof(ZWaveDevice.EnergyMode), this.Level) + "(" + this.CoolPoint.ToString() + "/" + this.HeatPoint.ToString() + ")" +
                        " | Currently:" + Temp + "° " + this.CurrentState;
            else
            {
                if (Level > 99)
                    return "Level: Unknown ON State";
                else
                    return "Level: " + this.Level + "%";
            }
        }

        public string GetMode()
        {
            if (Type == ZWaveDeviceTypes.Thermostat)
                return Enum.GetName(typeof(ZWaveDevice.ThermostatMode), this.HeatCoolMode);
            else
                return null;
        }

        public string GetFanMode()
        {
            if (Type == ZWaveDeviceTypes.Thermostat)
                return Enum.GetName(typeof(ZWaveDevice.ThermostatFanMode), this.FanMode);
            else
                return null;
        }

        public string GetSetPoint()
        {
            if (Type == ZWaveDeviceTypes.Thermostat)
                return Enum.GetName(typeof(ZWaveDevice.EnergyMode), this.Level) + "(" + this.CoolPoint.ToString() + "/" + this.HeatPoint.ToString() + ")";
            else
                return null;
        }

        public string GetCurrentState()
        {
            if (Type == ZWaveDeviceTypes.Thermostat)
                return this.CurrentState;
            else
                return null;
        }

        public string GlbUniqueID()
        {
            return this.HomeID.ToString() + this.NodeID.ToString();
        }

        //Type casting device to Action
        public static implicit operator Action(ZWaveDevice instance)
        {
            Action action = new Action();
            action.Type = Action.ActionTypes.ZWaveDevice;
            action.ZWaveType = instance.Type;
            action.Name = instance.Name;
            action.NodeID = instance.NodeID;
            action.HomeID = instance.HomeID;
            action.Temp = instance.Temp;
            action.MomentaryOnMode = instance.MomentaryOnMode;
            action.MomentaryTimespan = instance.MomentaryTimespan;
            return action;
        }
    }
        #endregion
    }

