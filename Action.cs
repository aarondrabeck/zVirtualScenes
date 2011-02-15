using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;

namespace zVirtualScenesApplication
{


    public class Action 
    {
        GlobalFunctions GlbFnct = new GlobalFunctions();
        public string Name { get; set; }
        public int ID { get; set; }
        public int Level { get; set; }
        public string Type { get; set; }
        public int Mode { get; set; }
        public int Temp { get; set; }

        public Action()
        {
            this.Name = "";
            this.Type = "";
            this.ID = 0;
            this.Level = 0;            
            this.Mode = -1;
            this.Temp = 0;
        }

        public override string ToString()
        {
            return Name + " - ID:" + ID + " - " + GetFomattedType();
        }

        public string GetFomattedType()
        {
            if (Type != null && Type.Contains("MultilevelPowerSwitch"))
            {
                if (Level == 255)
                    return "Level: " + Level + " (100%)";
                else
                    return "Level: " + Level + "%";
            }
            else if (Type != null && Type.Contains("GeneralThermostatV2"))
            {
                if (Mode == -1) //NO MODE AS IN DEVICE STATUS FETCHES
                    return "Temp: " + Temp + "°";
                else if (Mode == 0 || Mode == 6 || Mode == 7)
                    return "Mode: " + GetModeName(Mode);
                else if (Mode == 1 || Mode == 2 || Mode == 3 || Mode == 4 || Mode == 5)
                    return "Temp: " + Temp + "°" + " - Mode: " + GetModeName(Mode);
            }
           
            return "Unknown Device"; 
        }      
        
        public string ExecuteThisAction(Settings _settings)
        {
            string cmd = null;

            if (Type.Contains("MultilevelPowerSwitch"))
            {
                cmd = GlbFnct.GetZComURL(_settings) + "command=device&id=" + this.ID + "&level=" + this.Level;
                GlbFnct.HTTPSend(cmd);
            }
            else if (Type.Contains("GeneralThermostatV2"))
            {
               if (Mode == 0 || Mode == 6 || Mode == 7)
                   cmd = GlbFnct.GetZComURL(_settings) + "command=thermmode&id=" + this.ID + "&mode=" + this.Mode;
                else if (Mode == 1 || Mode == 2 || Mode == 3 || Mode == 4 || Mode == 5)
                   cmd = GlbFnct.GetZComURL(_settings) + "command=thermmode&id=" + this.ID + "&mode=" + this.Mode + "&temp=" + this.Temp;               
                GlbFnct.HTTPSend(cmd);                
            }
            return cmd;
        }

        public int GetModeID(string ModeName)
        {           
            switch(ModeName)
            {
                case "OFF":
                    return 0;
                case "AUTO": 
                   return 1;
                case "HEAT":
                    return 2;
                case "COOL":
                    return 3;
                case "FAN LOW":
                    return 4;
                case "FAN AUTO":
                    return 5;
                case "ENERGY SAVING MODE":
                    return 6;
                case "COMFORT MODE":
                    return 7;
            }
            return -1; 
        }

        public string GetModeName(int mode)
        {           
            switch(mode)
            {
                case 0:
                    return "OFF";
                case 1: 
                   return "AUTO";
                case 2:
                    return "HEAT";
                case 3:
                    return "COOL";
                case 4:
                    return "FAN LOW";
                case 5:
                    return "FAN AUTO";
                case 6:
                    return "ENERGY SAVING MODE";
                case 7:
                    return "COMFORT MODE";
            }
            return ""; 
        }

    }
}
