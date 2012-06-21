using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace zVirtualScenesModel
{
    public partial class scene_commands
    {
        public enum command_types
        {
            unknown = 0,
            builtin = 1,
            device_command = 2,
            device_type_command = 3
        }

        partial void onAfterPropertyChanged(string name)
        {
            if (name == "device_id" || name == "command_type_id" || name == "command_id")
            {
                NotifyPropertyChanged("Action_Description");
                NotifyPropertyChanged("Actionable_Object");
            }
            else if (name == "arg")
                NotifyPropertyChanged("Action_Description");
        }

        public string Actionable_Object
        {
            get {
                using (zvsLocalDBEntities context = new zvsLocalDBEntities())
                {
                    switch ((Command_Types)this.command_type_id)
                    {
                        case Command_Types.builtin:
                            {
                                builtin_commands bc = context.builtin_commands.FirstOrDefault(c => c.id == this.command_id);
                                if (bc != null)
                                    return bc.friendly_name;
                                break;
                            }
                        case Command_Types.device_command:
                        case Command_Types.device_type_command:
                            {
                                device d = context.devices.FirstOrDefault(o => o.id == this.device_id);
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
                using (zvsLocalDBEntities context = new zvsLocalDBEntities())
                {
                    switch ((Command_Types)this.command_type_id)
                    {
                        case Command_Types.builtin:
                            {
                                builtin_commands bc = context.builtin_commands.FirstOrDefault(c => c.id == this.command_id);
                                if (bc != null)
                                {
                                    switch (bc.name)
                                    {
                                        case "REPOLL_ME":
                                            {
                                                int d_id = 0;
                                                int.TryParse(this.arg, out d_id);

                                                device device_to_repoll = context.devices.FirstOrDefault(d => d.id == d_id);
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
                                                group g = context.groups.FirstOrDefault(gr => gr.id == g_id);

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
                        case Command_Types.device_command:
                            {
                                device_commands bc = context.device_commands.FirstOrDefault(c => c.id == this.command_id);
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
                        case Command_Types.device_type_command:
                            {
                                device_type_commands bc = context.device_type_commands.FirstOrDefault(c => c.id == this.command_id);
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
