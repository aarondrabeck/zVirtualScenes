using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace zVirtualScenesApplication 
{

    public class ZWaveDevice : INotifyPropertyChanged //use INotifyPropertyChanged to update binded listviews in the GUI on data changes
    {
        public formzVirtualScenes zVirtualScenesMain;
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
        
        #endregion

        //Constructor
        public ZWaveDevice(formzVirtualScenes zvsm)
        {
            zVirtualScenesMain = zvsm;
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

            //Light Switch Socket Format 
            //byData = System.Text.Encoding.UTF8.GetBytes("DEVICE~Bedroom Lights~0~60~MultiLevelSceneSwitch" + Environment.NewLine);
            //workerSocket.Send(byData);
            //byData = System.Text.Encoding.UTF8.GetBytes("DEVICE~Garage Light~1~0~BinarySwitch" + Environment.NewLine);
            //workerSocket.Send(byData);
            //byData = System.Text.Encoding.UTF8.GetBytes("DEVICE~Thermostat~3~75~Thermostat" + Environment.NewLine);
            //workerSocket.Send(byData);
            //byData = System.Text.Encoding.UTF8.GetBytes("DEVICE~Electric Blinds~4~100~WindowCovering" + Environment.NewLine);
            //workerSocket.Send(byData);
            //byData = System.Text.Encoding.UTF8.GetBytes("DEVICE~Motion Detector~5~0~Sensor" + Environment.NewLine);
            //workerSocket.Send(byData);
            //byData = System.Text.Encoding.UTF8.GetBytes("DEVICE~House (AWAY MODE)~6~0~Status" + Environment.NewLine);
            //workerSocket.Send(byData);
            public string ToLightSwitchSocketString()
            {                
                if (Type == ZWaveDeviceTypes.BinarySwitch)                    
                    return "DEVICE~" + _Name + "~" + this.NodeID + "~" + this.Level + "~" + "BinarySwitch";
                else if (Type == ZWaveDeviceTypes.Thermostat)
                    return "DEVICE~" + _Name + "~" + this.NodeID + "~" + Temp + "~" + this.Type;
                else
                    return "DEVICE~" + _Name + "~" + this.NodeID + "~" + this.Level + "~" + this.Type;
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
                else
                    return this.Level + "%";
            }

            public string GetStatus()
            {
                if (Type == ZWaveDeviceTypes.BinarySwitch)
                {
                    return (Level > 0 ? "State: ON": "State: OFF");
                }
                else if (Type == ZWaveDeviceTypes.Thermostat)
                    return  "Mode:" + Enum.GetName(typeof(ZWaveDevice.ThermostatMode), this.HeatCoolMode) +
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
                //ONLY INCLUDE NOT ACTION PROPERTIES HERE
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

