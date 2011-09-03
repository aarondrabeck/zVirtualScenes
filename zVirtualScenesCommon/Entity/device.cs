using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;
using zVirtualScenesCommon;
using System.Runtime.Serialization;
using System.Data.Objects;


namespace zVirtualScenesCommon.Entity
{
    
    
    public partial class device : EntityObject
    {
        partial void Onlast_heard_fromChanging(DateTime? value)
        {
            
        }

        public static IQueryable<device> GetAllDevices(bool forList)
        {
            var query = from o in zvsEntityControl.zvsContext.devices
                        where o.device_types.plugin.name != "BUILTIN"
                        select o;

            if (forList)
                return query.Where(o => o.device_types.show_in_list == true).AsQueryable();
            else
                return query.AsQueryable();

        }

        public string GetGroups
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach(group_devices gd in this.group_devices)
                {
                    if(gd.group != null)
                        sb.Append(gd.group.name + (this.group_devices.Last() == gd ? "" : ", "));
                }
                return sb.ToString();
            }
        }

        public string DeviceIcon()
        {
            if (this.device_types.name.Equals("THERMOSTAT"))
                return "20zwave-thermostat.png";
            else if (this.device_types.name.Equals("DIMMER"))
                return "20bulb.png";
            else if (this.device_types.name.Equals("SWITCH"))
                return "20switch.png";
            else if (this.device_types.name.Equals("CONTROLLER"))
                return "controler320.png";
            else if (this.device_types.name.Equals("DOORLOCK"))
                return "doorlock20";
            else
                return "20radio2.png";
        }   

        public override string ToString()
        {
            return this.friendly_name;
        }

        public int GetLevelMeter()
        {
            if (this.device_types.name.Equals("THERMOSTAT"))
            {
                int temp = 0;
                int.TryParse(this.device_values.SingleOrDefault(v => v.label_name == "Temperature").value, out temp);
                return temp;
            }
            else if (this.device_types.name.Equals("SWITCH"))
            {
                int level = 0;
                int.TryParse(this.device_values.SingleOrDefault(v => v.label_name == "Basic").value, out level);
                return (level > 0 ? 100 : 0);
            }
            else
            {
                device_values value = this.device_values.SingleOrDefault(v => v.label_name == "Basic");
                if (value == null || value.value == null)
                    return 0;

                int level = 0;
                    int.TryParse(value.value, out level);
                    return level;
            }
        }

        public string GetLevelText
        {
            get
            {
                if (this.device_types.name.Equals("THERMOSTAT"))
                {
                    int temp = 0;
                    int.TryParse(this.device_values.SingleOrDefault(v => v.label_name == "Temperature").value, out temp);
                    return temp + " F";
                }
                else if (this.device_types.name.Equals("SWITCH"))
                {
                    int level = 0;
                    int.TryParse(this.device_values.SingleOrDefault(v => v.label_name == "Basic").value, out level);
                    return (level > 0 ? "ON" : "OFF");
                }
                else
                {
                    device_values value = this.device_values.SingleOrDefault(v => v.label_name == "Basic");
                    if (value == null || value.value == null)
                        return "0";

                    int level = 0;
                    int.TryParse(value.value, out level);
                    return level + "%";
                }
            }
        }
    }
}