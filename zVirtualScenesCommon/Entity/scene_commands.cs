using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;
using zVirtualScenesCommon;
using System.Runtime.Serialization;
using System.Data.Objects;
using System.ComponentModel;

namespace zVirtualScenesCommon.Entity
{
    public partial class scene_commands : EntityObject
    { 
        public string Icon
        {
            get
            {
                using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                {
                    device d = db.devices.Single(o => o.id == this.device_id);

                    if (d.device_types.name != null)
                    {
                        if (d.device_types.name.Equals("THERMOSTAT"))
                            return "20zwave-thermostat.png";
                        else if (d.device_types.name.Equals("DIMMER"))
                            return "20bulb.png";
                        else if (d.device_types.name.Equals("SWITCH"))
                            return "20switch.png";
                        else if (d.device_types.name.Equals("CONTROLLER"))
                            return "controler320.png";
                        else
                            return "20radio2.png";
                    }

                    return string.Empty;
                }
            }
        }

        public string Actionable_Object
        {
            get {
                using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                {
                    switch ((command_types)this.command_type_id)
                    {
                        case command_types.builtin:
                            {
                                builtin_commands bc = db.builtin_commands.FirstOrDefault(c => c.id == this.command_id);
                                if (bc != null)
                                    return bc.friendly_name;
                                break;
                            }
                        case command_types.device_command:
                        case command_types.device_type_command:
                            {
                                device d = db.devices.FirstOrDefault(o => o.id == this.device_id);
                                if (d != null)
                                    return d.friendly_name;
                                break;
                            }
                    }
                    return string.Empty;
                }
            }
        }

        public string Action_Description
        {
            get
            {
                using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                {
                    switch ((command_types)this.command_type_id)
                    {
                        case command_types.builtin:
                            {
                                builtin_commands bc = db.builtin_commands.FirstOrDefault(c => c.id == this.command_id);
                                if (bc != null)
                                {
                                    switch (bc.name)
                                    {
                                        case "REPOLL_ME":
                                            {
                                                int d_id = 0;
                                                int.TryParse(this.arg, out d_id);

                                                device device_to_repoll = db.devices.FirstOrDefault(d => d.id == d_id);
                                                if (device_to_repoll != null && d_id > 0)
                                                {
                                                    return device_to_repoll.friendly_name;
                                                }
                                                else
                                                {
                                                    return this.arg;
                                                }
                                            }
                                        case "GROUP_ON":
                                        case "GROUP_OFF":
                                            {
                                                int g_id = 0;
                                                int.TryParse(this.arg, out g_id);
                                                group g = db.groups.FirstOrDefault(gr => gr.id == g_id);

                                                if (g != null)
                                                    return g.name;
                                                break;
                                            }
                                        default:
                                            return this.arg;
                                    }
                                }
                                break;
                            }
                        case command_types.device_command:
                            {
                                device_commands bc = db.device_commands.FirstOrDefault(c => c.id == this.command_id);
                                if (bc != null)
                                {
                                    switch ((Data_Types)bc.arg_data_type)
                                    {
                                        case Data_Types.NONE:
                                            return bc.friendly_name + ".";
                                        case Data_Types.SHORT:
                                        case Data_Types.STRING:
                                        case Data_Types.LIST:
                                        case Data_Types.INTEGER:
                                        case Data_Types.DECIMAL:
                                        case Data_Types.BYTE:
                                        case Data_Types.BOOL:
                                            return string.Format("{0} to '{1}'.", bc.friendly_name, this.arg);
                                    }
                                }
                                break;
                            }
                        case command_types.device_type_command:
                            {
                                device_type_commands bc = db.device_type_commands.FirstOrDefault(c => c.id == this.command_id);
                                if (bc != null)
                                {
                                    switch ((Data_Types)bc.arg_data_type)
                                    {
                                        case Data_Types.NONE:
                                            return bc.friendly_name + ".";
                                        case Data_Types.SHORT:
                                        case Data_Types.STRING:
                                        case Data_Types.LIST:
                                        case Data_Types.INTEGER:
                                        case Data_Types.DECIMAL:
                                        case Data_Types.BYTE:
                                        case Data_Types.BOOL:
                                            return string.Format("{0} to '{1}'.", bc.friendly_name, this.arg);
                                    }
                                }
                                break;
                            }
                    }

                    return string.Empty;
                }
            }
        }
    }
}
