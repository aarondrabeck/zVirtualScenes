using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;

namespace zVirtualScenesApplication
{
    public class Device 
    {
        //DEVICE~Fireplace~2~0~MultilevelPowerSwitch 
        //DEVICE~South Termostat~7~65~GeneralThermostatV2
        public Device()
        {
            this.Name = "Default Device";
            this.ID = 0;
            this.Level = 0;
            this.Type = "Unknown";
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
                return "Temp: " + Level + "°";
            return "Unknown Device"; 
        }

        public string Name { get; set; }
        public int ID { get; set; }
        public int Level { get; set; }
        public string Type { get; set; }

    }
}
