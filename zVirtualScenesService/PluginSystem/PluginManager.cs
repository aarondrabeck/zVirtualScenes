using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Data;
using System.Linq;
using System.Threading;
using zVirtualScenesAPI;
using zVirtualScenesCommon.Util;
using System;

using zVirtualScenesCommon;
using zVirtualScenesCommon.Entity;

namespace zVirtualScenesService.PluginSystem
{
    public class PluginManager
    {
        private const string _FriendlyName = "PLUGIN MANAGER";
        [ImportMany]
#pragma warning disable 649
        private IEnumerable<Plugin> _plugins;
#pragma warning restore 649
        //private bool _runCommands;

        public PluginManager()
        {
            DirectoryCatalog catalog = new DirectoryCatalog("plugins");
            CompositionContainer compositionContainer = new CompositionContainer(catalog);
            compositionContainer.ComposeParts(this);

            builtin_commands.InstallBuiltInCommand(new builtin_commands
            {
                name = "REPOLL_ME",
                friendly_name = "Repoll Device",
                arg_data_type = (int)Data_Types.INTEGER,
                show_on_dynamic_obj_list = false,
                description = "This will force a repoll on an object."
            });

            builtin_commands.InstallBuiltInCommand(new builtin_commands
            {
                name = "REPOLL_ALL",
                friendly_name = "Repoll all Devices",
                arg_data_type = (int)Data_Types.NONE,
                show_on_dynamic_obj_list = false,
                description = "This will force a repoll on all objects."
            });

            builtin_commands.InstallBuiltInCommand(new builtin_commands
            {
                name = "GROUP_ON",
                friendly_name = "Turn Group On",
                arg_data_type = (int)Data_Types.STRING,
                show_on_dynamic_obj_list = false,
                description = "Activates a group."
            });

            builtin_commands.InstallBuiltInCommand(new builtin_commands
            {
                name = "GROUP_OFF",
                friendly_name = "Turn Group Off",
                arg_data_type = (int)Data_Types.STRING,
                show_on_dynamic_obj_list = false,
                description = "Deactivates a group."
            });

            builtin_commands.InstallBuiltInCommand(new builtin_commands
            {
                name = "TIMEDELAY",
                friendly_name = "Scene Time Delay (sec)",
                arg_data_type = (int)Data_Types.INTEGER,
                show_on_dynamic_obj_list = false,
                description = "Pauses a scene execution for x seconds."
            });

            device_propertys.DefineOrUpdateDeviceProperty(new device_propertys
            {
                name = "ENABLEPOLLING",
                friendly_name = "Enable polling for this device.",
                default_value = "false",
                value_data_type = (int)Data_Types.BOOL
            });
            
            // Iterate the plugins to make sure none of them are new...
            foreach (Plugin p in _plugins)
            {
                using (zvsEntities2 context = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                {
                    plugin ent_p = context.plugins.FirstOrDefault(pl => pl.name == p.Name);
                    if (ent_p == null)
                    {
                        ent_p = new plugin { name = p.Name, friendly_name = p.ToString() };
                        context.plugins.AddObject(ent_p);
                        context.SaveChanges();
                    }
                }

                p.Initialize();
                p.Start();
            }

            builtin_command_que.BuiltinCommandAddedToQueEvent += new builtin_command_que.BuiltinCommandAddedEventHandler(builtin_command_que_BuiltinCommandAddedToQueEvent);
            device_type_command_que.DeviceTypeCommandAddedToQueEvent += new device_type_command_que.DeviceTypeCommandAddedEventHandler(device_type_command_que_DeviceTypeCommandAddedToQueEvent);
            device_command_que.DeviceCommandAddedToQueEvent += new device_command_que.DeviceCommandAddedEventHandler(device_command_que_DeviceCommandAddedToQueEvent);            
        }

        void device_type_command_que_DeviceTypeCommandAddedToQueEvent(int device_type_command_que_id)
        {
            using (zvsEntities2 context = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                device_type_command_que cmd = context.device_type_command_que.FirstOrDefault(c => c.id == device_type_command_que_id);

                if (cmd != null && cmd.device != null)
                {
                    Console.WriteLine("[Processing Device Type CMD] API:" + cmd.device.device_types.plugin.name +
                                                            ", CMD_NAME:" + cmd.device_type_commands.friendly_name +
                                                            ", ARG:" + cmd.arg);

                    foreach (Plugin p in GetPlugins().Where(p => p.Name == cmd.device.device_types.plugin.name))
                    {
                        if (p.Enabled)
                        {
                            bool err = false;
                            int iterations = 0;
                            while (!p.IsReady)
                            {
                                if (iterations == 5)
                                {
                                    err = true;
                                    string err_str = "Timed-out while trying to process " + cmd.device_type_commands.friendly_name + " on '" + cmd.device.friendly_name + "'. Plugin Not Ready.";
                                    Logger.WriteToLog(Urgency.ERROR, err_str, p.Name);

                                    device_type_command_que.DeviceTypeCommandRunComplete(cmd, true, err_str);
                                    context.device_type_command_que.DeleteObject(cmd);
                                    context.SaveChanges();
                                    return;
                                }
                                iterations++;
                                Thread.Sleep(1000);
                            }

                            if (!err)
                            {
                                p.ProcessDeviceTypeCommand(cmd);
                                device_type_command_que.DeviceTypeCommandRunComplete(cmd, false, string.Empty);
                                context.device_type_command_que.DeleteObject(cmd);
                                context.SaveChanges();
                                return;
                            }
                        }
                        else
                        {
                            string err_str = "Attemped command " + cmd.device_type_commands.friendly_name + " on '" + cmd.device.friendly_name + "' on a disabled plugin. Removing command from que...";
                            Logger.WriteToLog(Urgency.WARNING, err_str, p.Name);

                            device_type_command_que.DeviceTypeCommandRunComplete(cmd, true, err_str);
                            context.device_type_command_que.DeleteObject(cmd);
                            context.SaveChanges();
                            return;
                        }
                    }
                }
                else
                {
                    string err_str = "Could not locate qued device command.";
                    device_type_command_que.DeviceTypeCommandRunComplete(cmd, true, err_str);
                    Logger.WriteToLog(Urgency.ERROR, err_str, _FriendlyName);
                }
            }
                    
        }

        void device_command_que_DeviceCommandAddedToQueEvent(int device_command_que_id)
        {
            using (zvsEntities2 context = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                device_command_que cmd = context.device_command_que.FirstOrDefault(c => c.id == device_command_que_id);

                if (cmd != null && cmd.device != null)
                {
                    Console.WriteLine("[Processing Device CMD] API:" + cmd.device.device_types.plugin.name +
                                                            ", CMD_NAME:" + cmd.device_commands.friendly_name +
                                                            ", ARG:" + cmd.arg);

                    foreach (Plugin p in GetPlugins().Where(p => p.Name == cmd.device.device_types.plugin.name))
                    {
                        if (p.Enabled)
                        {
                            bool err = false;
                            int iterations = 0;
                            while (!p.IsReady)
                            {
                                if (iterations == 5)
                                {
                                    err = true;
                                    string err_str = "Timed-out while trying to process " + cmd.device_commands.friendly_name + " on '" + cmd.device.friendly_name + "'. Plugin Not Ready.";
                                    Logger.WriteToLog(Urgency.ERROR, err_str, p.Name);

                                    device_command_que.DeviceCommandRunComplete(cmd, true, err_str);
                                    context.device_command_que.DeleteObject(cmd);
                                    context.SaveChanges();
                                    return;
                                }
                                iterations++;
                                Thread.Sleep(1000);
                            }

                            if (!err)
                            {
                                p.ProcessDeviceCommand(cmd);
                                device_command_que.DeviceCommandRunComplete(cmd, false, string.Empty);
                                context.device_command_que.DeleteObject(cmd);
                                context.SaveChanges();
                                return;
                            }
                        }
                        else
                        {
                            string err_str = "Attemped command " + cmd.device_commands.friendly_name + " on '" + cmd.device.friendly_name + "' on a disabled plugin. Removing command from que...";
                            Logger.WriteToLog(Urgency.WARNING, err_str, p.Name);

                            device_command_que.DeviceCommandRunComplete(cmd, true, err_str);
                            context.device_command_que.DeleteObject(cmd);
                            context.SaveChanges();
                            return;
                        }
                    }
                }
                else
                    Logger.WriteToLog(Urgency.ERROR, "Could not locate qued device command.", _FriendlyName);
            }
                   
        }

        void builtin_command_que_BuiltinCommandAddedToQueEvent(int builtin_command_que_id)
        {
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                builtin_command_que cmd = db.builtin_command_que.FirstOrDefault(c => c.id == builtin_command_que_id);

                if (cmd != null)
                {
                    Console.WriteLine("[PROCESSING BUILTIN CMD] CMD_NAME:" + cmd.builtin_commands.friendly_name + ", ARG:" + cmd.arg);

                    switch (cmd.builtin_commands.name)
                    {
                        case "REPOLL_ME":
                            {
                                int d_id = 0;
                                int.TryParse(cmd.arg, out d_id);
                                device d = device.GetAllDevices(db,false).FirstOrDefault(o => o.id == d_id);

                                if (d.device_types.plugin.enabled)
                                {
                                    GetPlugin(d.device_types.plugin.name).Repoll(d);
                                }
                                break;
                            }
                        case "REPOLL_ALL":
                            {
                                foreach (device d in device.GetAllDevices(db, false))
                                {
                                    if (d.device_types.plugin.enabled)
                                    {
                                        GetPlugin(d.device_types.plugin.name).Repoll(d);
                                    }
                                }
                                break;
                            }
                        case "GROUP_ON":
                            {
                                int g_id = 0;
                                int.TryParse(cmd.arg, out g_id);
                                //EXECUTE ON ALL API's
                                if (g_id > 0)
                                {
                                    foreach (Plugin p in GetPlugins())
                                    {
                                        if (p.Enabled)
                                            p.ActivateGroup(g_id);
                                    }
                                }
                                break;
                            }
                        case "GROUP_OFF":
                            {
                                int g_id = 0;
                                int.TryParse(cmd.arg, out g_id);
                                //EXECUTE ON ALL API's
                                if (g_id > 0)
                                {
                                    foreach (Plugin p in GetPlugins())
                                    {
                                        if (p.Enabled)
                                            p.DeactivateGroup(g_id);
                                    }
                                }
                                break;
                            }
                    }
                    //TODO: Error Checking for CMD
                    builtin_command_que.BuiltinCommandRunComplete(cmd, false, "");

                    //Remove processed command from que
                    db.builtin_command_que.DeleteObject(cmd);
                    db.SaveChanges();
                }
                else
                    Logger.WriteToLog(Urgency.ERROR, "Could not locate qued builit-in command.", _FriendlyName);
            }
            
        }

        public IEnumerable<Plugin> GetPlugins()
        {
            return _plugins;
        }

        public Plugin GetPlugin(string pluginName)
        {
            return _plugins.FirstOrDefault(p => p.Name == pluginName);
        }
    }
}
         
