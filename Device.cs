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
        public formzVirtualScenes zVirtualScenesMain;
        public event PropertyChangedEventHandler PropertyChanged;

        #region Properties and Variables
        //Properties that require PropertyChangedEvent to fire to sync GUI
        public uint HomeID { get; set; }
        public byte NodeID { get; set; }
        public string Type { get; set; }//Properties that require PropertyChangedEvent to fire to sync GUI
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { GlobalFunctions.Set(this, "Name", ref _Name, value, PropertyChanged); }
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
        
        
        #endregion

        //Constructor
        public Device(formzVirtualScenes zvsm)
        {
            zVirtualScenesMain = zvsm;
            this.HomeID = 0;
            this.NodeID = 0;
            _Name = "Default Device";
            this.Level = 0;
            this.Type = "Unknown"; 
            this.FanMode = -1;
            this.HeatCoolMode = -1;
            this.CoolPoint = -1;
            this.HeatPoint = -1;
            this.MinAlertTemp = 40;
            this.MaxAlertTemp = 90;
            this.NotificationDetailLevel = 1;
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
                return "(" + NodeID + ") " + _Name + " - " + GetFomattedType();
            }

            //Light Switch Socket Format - DEVICE~Bedroom Lights~0~60~MultiLevelSceneSwitch
            public string ToLightSwitchSocketString()
            {
                if (Type != null && Type.Contains("MultilevelPowerSwitch"))
                    return "DEVICE~" + _Name + "~" + this.NodeID + "~" + this.Level + "~" + this.Type;
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
                        return "Level: " + this.Level + "%";
                }
                else if (Type != null && (Type.Contains("GeneralThermostatV2") || Type.Contains("GeneralThermostat")))
                    return  "Mode:" + Enum.GetName(typeof(Device.ThermostatMode), this.HeatCoolMode) +
                            " | Fan:" + Enum.GetName(typeof(Device.ThermostatFanMode), this.FanMode) +
                            " | SetPoint:" + Enum.GetName(typeof(Device.EnergyMode), this.Level) + "(" + this.CoolPoint.ToString() + "/" + this.HeatPoint.ToString() + ")" +
                            " | Currently:" + Temp + "° " + this.CurrentState;
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
                //ONLY INCLUDE NOT ACTION PROPERTIES HERE
                action.Type = instance.Type;
                action.Name = instance.Name;
                action.NodeID = instance.NodeID;
                action.HomeID = instance.HomeID;
                action.Temp = instance.Temp;             
                return action;
            }

        #endregion



    }
}
