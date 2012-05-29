using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Data;
using System.Linq;
using System.Threading;
using System;
using System.ComponentModel;
using zVirtualScenesCommon.Util;
using zVirtualScenesAPI;
using zVirtualScenesCommon;
using zvsModel;


namespace zVirtualScenes
{
    public class PluginManager
    {
        private const int verbose = 10;
        private const string _FriendlyName = "Plug-in Manager";
        private Core Core;

        [ImportMany]
        #pragma warning disable 649
        private IEnumerable<Plugin> _plugins;
        #pragma warning restore 649

        public PluginManager(Core Core)
        {
            this.Core = Core;
            DirectoryCatalog catalog = new DirectoryCatalog("plugins");
            CompositionContainer compositionContainer = new CompositionContainer(catalog);
            compositionContainer.ComposeParts(this);

            builtin_commands.AddOrEdit(new builtin_commands
            {
                name = "REPOLL_ME",
                friendly_name = "Re-poll Device",
                arg_data_type = (int)Data_Types.INTEGER,
                show_on_dynamic_obj_list = false,
                description = "This will force a re-poll on an object."
            });

            builtin_commands.AddOrEdit(new builtin_commands
            {
                name = "REPOLL_ALL",
                friendly_name = "Re-poll all Devices",
                arg_data_type = (int)Data_Types.NONE,
                show_on_dynamic_obj_list = false,
                description = "This will force a re-poll on all objects."
            });

            builtin_commands.AddOrEdit(new builtin_commands
            {
                name = "GROUP_ON",
                friendly_name = "Turn Group On",
                arg_data_type = (int)Data_Types.STRING,
                show_on_dynamic_obj_list = false,
                description = "Activates a group."
            });

            builtin_commands.AddOrEdit(new builtin_commands
            {
                name = "GROUP_OFF",
                friendly_name = "Turn Group Off",
                arg_data_type = (int)Data_Types.STRING,
                show_on_dynamic_obj_list = false,
                description = "Deactivates a group."
            });

            builtin_commands.AddOrEdit(new builtin_commands
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

            // Iterate the plug-in
            foreach (Plugin p in _plugins)
            {
                //keeps this plug-in in scope 
                var p2 = p;

                using (zvsEntities2 context = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                {
                    //make sure none of them are new...
                    plugin ent_p = context.plugins.FirstOrDefault(pl => pl.name == p2.Name);
                    if (ent_p == null)
                    {
                        //if it is a new plug-in save it to the database.
                        ent_p = new plugin { name = p2.Name, friendly_name = p2.ToString() };
                        context.plugins.AddObject(ent_p);
                        context.SaveChanges();
                    }
                }

                Core.Logger.WriteToLog(Urgency.INFO, string.Format("Loading '{0}'", p2.FriendlyName), _FriendlyName);
                p2.Initialize();
                p2.Start();

                ////initialize each plug-in on a separate thread.
                //BackgroundWorker pluginInitializer = new BackgroundWorker();
                //pluginInitializer.DoWork += (object sender, DoWorkEventArgs e) =>
                //{
                    
                //};
                //pluginInitializer.RunWorkerAsync();
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
                    if (verbose > 1)
                    {
                        Core.Logger.WriteToLog(Urgency.INFO, "[Processing Device Type CMD] API:" + cmd.device.device_types.plugin.name +
                                                               ", CMD_NAME:" + cmd.device_type_commands.friendly_name +
                                                               ", ARG:" + cmd.arg, _FriendlyName);
                    }

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
                                    string err_str = "Timed-out while trying to process " + cmd.device_type_commands.friendly_name + " on '" + cmd.device.friendly_name + "'. Plug-in Not Ready.";
                                    Core.Logger.WriteToLog(Urgency.ERROR, err_str, p.Name);

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
                            string err_str = "Attempted command " + cmd.device_type_commands.friendly_name + " on '" + cmd.device.friendly_name + "' on a disabled plug-in. Removing command from queue...";
                            Core.Logger.WriteToLog(Urgency.WARNING, err_str, p.Name);

                            device_type_command_que.DeviceTypeCommandRunComplete(cmd, true, err_str);
                            context.device_type_command_que.DeleteObject(cmd);
                            context.SaveChanges();
                            return;
                        }
                    }
                }
                else
                {
                    string err_str = "Could not locate queued device command.";
                    device_type_command_que.DeviceTypeCommandRunComplete(cmd, true, err_str);
                    Core.Logger.WriteToLog(Urgency.ERROR, err_str, _FriendlyName);
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
                    if (verbose > 1)
                    {
                        Core.Logger.WriteToLog(Urgency.INFO, "[Processing Device CMD] API:" + cmd.device.device_types.plugin.name +
                                                                ", CMD_NAME:" + cmd.device_commands.friendly_name +
                                                                ", ARG:" + cmd.arg, _FriendlyName);
                    }

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
                                    string err_str = "Timed-out while trying to process " + cmd.device_commands.friendly_name + " on '" + cmd.device.friendly_name + "'. Plug-in Not Ready.";
                                    Core.Logger.WriteToLog(Urgency.ERROR, err_str, p.Name);

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
                            string err_str = "Attempted command " + cmd.device_commands.friendly_name + " on '" + cmd.device.friendly_name + "' on a disabled plug-in. Removing command from queue...";
                            Core.Logger.WriteToLog(Urgency.WARNING, err_str, p.Name);

                            device_command_que.DeviceCommandRunComplete(cmd, true, err_str);
                            context.device_command_que.DeleteObject(cmd);
                            context.SaveChanges();
                            return;
                        }
                    }
                }
                else
                    Core.Logger.WriteToLog(Urgency.ERROR, "Could not locate queued device command.", _FriendlyName);
            }

        }

        void builtin_command_que_BuiltinCommandAddedToQueEvent(int builtin_command_que_id)
        {
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                builtin_command_que cmd = db.builtin_command_que.FirstOrDefault(c => c.id == builtin_command_que_id);

                if (cmd != null)
                {
                    if (verbose > 1)
                    {
                        Core.Logger.WriteToLog(Urgency.INFO, "[PROCESSING BUILTIN CMD] CMD_NAME:" + cmd.builtin_commands.friendly_name + ", ARG:" + cmd.arg, "MainForm");
                    }

                    switch (cmd.builtin_commands.name)
                    {
                        case "REPOLL_ME":
                            {
                                int d_id = 0;
                                int.TryParse(cmd.arg, out d_id);
                                device d = device.GetAllDevices(db, false).FirstOrDefault(o => o.id == d_id);

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
                    Core.Logger.WriteToLog(Urgency.ERROR, "Could not locate queued built-in command.", _FriendlyName);
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

