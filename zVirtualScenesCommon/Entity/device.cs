using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;
using zVirtualScenesCommon;
using System.Runtime.Serialization;
using System.Data.Objects;
using zVirtualScenesCommon.Util;
using System.Data;

namespace zVirtualScenesCommon.Entity
{
    public partial class device : EntityObject
    {
        /// <summary>
        /// This is called when a device or device value 
        /// </summary>        
        public delegate void DeviceAddRemoveEventHandler(int DeviceID);
        public static event DeviceAddRemoveEventHandler DeviceAdded;
        public static event DeviceAddRemoveEventHandler DeviceRemoved;

        public void CallAdded(int DeviceID)
        {
            if (DeviceAdded != null)
                DeviceAdded(DeviceID);
        }

        public void CallRemoved(int DeviceID)
        {
            if (DeviceRemoved != null)
                DeviceRemoved(DeviceID);
        }

        public static IQueryable<device> GetAllDevices(zvsEntities2 db, bool forList)
        {
            var query = from o in db.devices
                        where o.device_types.plugin.name != "BUILTIN"
                        select o;

            if (forList)
                return query.Where(o => o.device_types.show_in_list == true).AsQueryable();
            else
                return query.AsQueryable();

        }

        public string GroupNames
        {    
            get
            {
                using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                {
                    db.devices.Attach(this);
                    StringBuilder sb = new StringBuilder();
                foreach (group_devices gd in this.group_devices)
                {
                    if (gd.group != null)
                        sb.Append(gd.group.name + (this.group_devices.Last() == gd ? "" : ", "));
                }
                return sb.ToString();
                }
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

        public string DeviceTypeName
        {
            get
            {
                using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                {                                           
                    db.devices.Attach(this);
                  // db.AttachToOrGet<device>("device", ref this);
                    return this.device_types.name;
                }
            }
        }

       

        public string DeviceTypeFriendlyName
        {
            get
            {
                using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                {
                    db.devices.Attach(this);
                    return this.device_types.friendly_name;
                }
            }
        }


        public int LevelMeter
        {
            get
            {
                using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                {
                    db.devices.Attach(this);

                    if (this.device_types.name.Equals("THERMOSTAT"))
                    {
                        float temp = 0;
                        device_values dv = this.device_values.LastOrDefault(v => v.label_name == "Temperature");

                        if (dv != null)
                            float.TryParse(dv.value2, out temp);

                        return (int)temp;
                    }
                    else if (this.device_types.name.Equals("SWITCH"))
                    {
                        int level = 0;
                        device_values dv = this.device_values.LastOrDefault(v => v.label_name == "Basic");

                        if (dv != null)
                            int.TryParse(dv.value2, out level);

                        return (level > 0 ? 100 : 0);
                    }
                    else
                    {
                        int level = 0;
                        device_values dv = this.device_values.LastOrDefault(v => v.label_name == "Basic");

                        if (dv != null)
                            int.TryParse(dv.value2, out level);

                        return level;
                    }
                }
            }
        }

        public string LevelText
        {
            get
            {
                using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                {
                    db.devices.Attach(this);
                    if (this.device_types.name.Equals("THERMOSTAT"))
                    {
                        float temp = 0;
                        device_values dv = this.device_values.LastOrDefault(v => v.label_name == "Temperature");

                        if (dv != null)
                            float.TryParse(dv.value2, out temp);

                        return string.Format("{0} {1}", temp, program_options.GetProgramOption("TempAbbreviation"));
                    }
                    else if (this.device_types.name.Equals("SWITCH"))
                    {
                        int level = 0;
                        device_values dv = this.device_values.LastOrDefault(v => v.label_name == "Basic");

                        if (dv != null)
                            int.TryParse(dv.value2, out level);

                        return (level > 0 ? "ON" : "OFF");
                    }
                    else
                    {
                        int level = 0;
                        device_values dv = this.device_values.LastOrDefault(v => v.label_name == "Basic");

                        if (dv != null)
                            int.TryParse(dv.value2, out level);

                        return level + "%";
                    }
                }
            }
        }
    }
}