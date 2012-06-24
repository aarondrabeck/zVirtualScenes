using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Data;
using System.Linq;
using System.Threading;
using System;
using System.ComponentModel;
using zVirtualScenesModel;
using System.Windows.Threading;

namespace zVirtualScenes
{
    public class PluginManager
    {
        private const int verbose = 10;
        private const string _FriendlyName = "Plug-in Manager";
        private Core Core;

        #region Events
        public delegate void onProcessingCommandEventHandler(object sender, onProcessingCommandEventArgs args);
        public class onProcessingCommandEventArgs : EventArgs
        {
            public bool hasErrors { get; private set; }
            public string Details { get; private set; }
            public scene_commands.command_types CommandType { get; private set; }
            public int CommandQueueID { get; private set; }

            public onProcessingCommandEventArgs(scene_commands.command_types CommandType, bool Errors, string Details, int CommandQueueID)
            {
                this.CommandQueueID = CommandQueueID;
                this.CommandType = CommandType;
                this.hasErrors = Errors;
                this.Details = Details;
            }
        }
        public static event onProcessingCommandEventHandler onProcessingCommandBegin;
        public static event onProcessingCommandEventHandler onProcessingCommandEnd;

        public delegate void onPluginInitializedEventHandler(object sender, onPluginInitializedEventArgs args);
        public class onPluginInitializedEventArgs : EventArgs
        {
            public string Details = string.Empty;

            public onPluginInitializedEventArgs(string Details)
            {
                this.Details = Details;
            }
        }
        public static event onPluginInitializedEventHandler onPluginInitialized;
        #endregion

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

            using (zvsLocalDBEntities context = new zvsLocalDBEntities())
            {
                builtin_commands.AddOrEdit(new builtin_commands
                {
                    name = "REPOLL_ME",
                    friendly_name = "Re-poll Device",
                    arg_data_type = (int)Data_Types.INTEGER,
                    show_on_dynamic_obj_list = false,
                    description = "This will force a re-poll on an object."
                }, context);

                builtin_commands.AddOrEdit(new builtin_commands
                {
                    name = "REPOLL_ALL",
                    friendly_name = "Re-poll all Devices",
                    arg_data_type = (int)Data_Types.NONE,
                    show_on_dynamic_obj_list = false,
                    description = "This will force a re-poll on all objects."
                }, context);

                builtin_commands.AddOrEdit(new builtin_commands
                {
                    name = "GROUP_ON",
                    friendly_name = "Turn Group On",
                    arg_data_type = (int)Data_Types.STRING,
                    show_on_dynamic_obj_list = false,
                    description = "Activates a group."
                }, context);

                builtin_commands.AddOrEdit(new builtin_commands
                {
                    name = "GROUP_OFF",
                    friendly_name = "Turn Group Off",
                    arg_data_type = (int)Data_Types.STRING,
                    show_on_dynamic_obj_list = false,
                    description = "Deactivates a group."
                }, context);

                builtin_commands.AddOrEdit(new builtin_commands
                {
                    name = "TIMEDELAY",
                    friendly_name = "Scene Time Delay (sec)",
                    arg_data_type = (int)Data_Types.INTEGER,
                    show_on_dynamic_obj_list = false,
                    description = "Pauses a scene execution for x seconds."
                }, context);

                device_propertys.AddOrEdit(new device_propertys
                {
                    name = "ENABLEPOLLING",
                    friendly_name = "Enable polling for this device.",
                    default_value = "false",
                    value_data_type = (int)Data_Types.BOOL
                }, context);

                // Iterate the plug-in
                foreach (Plugin p in _plugins)
                {
                    //keeps this plug-in in scope 
                    var p2 = p;

                    //Plugin need access to the core in order to use the Logger
                    p2.Core = this.Core;

                    //make sure none of them are new...
                    plugin ent_p = context.plugins.FirstOrDefault(pl => pl.name == p2.Name);
                    if (ent_p == null)
                    {
                        //if it is a new plug-in save it to the database.
                        ent_p = new plugin { name = p2.Name, friendly_name = p2.ToString() };
                        context.plugins.Add(ent_p);
                        context.SaveChanges();
                    }

                    //initialize each plug-in async.
                    BackgroundWorker pluginInitializer = new BackgroundWorker();
                    pluginInitializer.DoWork += (object sender, DoWorkEventArgs e) =>
                    {
                        if (onPluginInitialized != null)
                            onPluginInitialized(this, new onPluginInitializedEventArgs(string.Format("Initializing '{0}'", p2.FriendlyName)));

                        p2.Initialize();
                        p2.Start();
                    };
                    pluginInitializer.RunWorkerAsync();
                }
            }

            builtin_command_que.BuiltinCommandAddedToQueEvent += new builtin_command_que.BuiltinCommandAddedEventHandler(builtin_command_que_BuiltinCommandAddedToQueEvent);
            device_type_command_que.DeviceTypeCommandAddedToQueEvent += new device_type_command_que.DeviceTypeCommandAddedEventHandler(device_type_command_que_DeviceTypeCommandAddedToQueEvent);
            device_command_que.DeviceCommandAddedToQueEvent += new device_command_que.DeviceCommandAddedEventHandler(device_command_que_DeviceCommandAddedToQueEvent);
        }

        void device_type_command_que_DeviceTypeCommandAddedToQueEvent(int device_type_command_que_id)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (s, a) =>
            {
                using (zvsLocalDBEntities context = new zvsLocalDBEntities())
                {
                    device_type_command_que cmd = context.device_type_command_que.FirstOrDefault(c => c.id == device_type_command_que_id);

                    if (cmd == null || cmd.device == null)
                    {
                        if (onProcessingCommandBegin != null)
                            onProcessingCommandBegin(this, new onProcessingCommandEventArgs(scene_commands.command_types.device_type_command, false, string.Format("Processing device type command id {0}", cmd.id), cmd.id));

                        if (onProcessingCommandEnd != null)
                            onProcessingCommandEnd(this, new onProcessingCommandEventArgs(scene_commands.command_types.device_type_command, true, string.Format("Failed to process device type command id {0}. Could not locate queued command in database.", cmd.id), cmd.id));

                        return;
                    }

                    //We found the command so continue
                    if (onProcessingCommandBegin != null)
                        onProcessingCommandBegin(this, new onProcessingCommandEventArgs(scene_commands.command_types.device_type_command, false, string.Format("Processing device type command '{0}':'{1}'", cmd.device_type_commands.friendly_name, cmd.arg), cmd.id));

                    Plugin p = GetPlugins().FirstOrDefault(o => o.Name == cmd.device.device_types.plugin.name);
                    if (p == null)
                    {
                        if (onProcessingCommandEnd != null)
                            onProcessingCommandEnd(this, new onProcessingCommandEventArgs(scene_commands.command_types.device_type_command, true, string.Format("Failed to process built-in command id {0}. Could not locate queued commands plug-in.", cmd.id), cmd.id));

                        context.device_type_command_que.Remove(cmd);
                        context.SaveChanges();

                        return;
                    }


                    if (p.Enabled && p.IsReady)
                    {
                        string details = string.Format("Processing command '{0}':'{1}' on {2} to plug-in '{3}'.",
                                                              cmd.device_type_commands.friendly_name,
                                                              cmd.arg,
                                                              cmd.device.friendly_name,
                                                              p.FriendlyName);

                        if (onProcessingCommandEnd != null)
                            onProcessingCommandEnd(this, new onProcessingCommandEventArgs(scene_commands.command_types.device_type_command, false, details, cmd.id));

                        p.ProcessDeviceTypeCommand(cmd);
                        context.device_type_command_que.Remove(cmd);
                        context.SaveChanges();
                        return;

                    }
                    else
                    {
                        string err_str = string.Format("Failed to process command '{0}' on '{1}' because the '{2}' plug-in is {3}. Removing command from queue...",
                        cmd.device_type_commands.friendly_name,
                        cmd.device.friendly_name,
                        cmd.device.device_types.plugin.name,
                        p.Enabled ? "not ready" : "disabled"
                        );

                        if (onProcessingCommandEnd != null)
                            onProcessingCommandEnd(this, new onProcessingCommandEventArgs(scene_commands.command_types.device_type_command, true, err_str, cmd.id));

                        context.device_type_command_que.Remove(cmd);
                        context.SaveChanges();
                        return;
                    }
                }
            };
            bw.RunWorkerAsync();
        }

        void device_command_que_DeviceCommandAddedToQueEvent(int device_command_que_id)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (s, a) =>
            {
                using (zvsLocalDBEntities context = new zvsLocalDBEntities())
                {
                    device_command_que cmd = context.device_command_que.FirstOrDefault(c => c.id == device_command_que_id);

                    if (cmd == null || cmd.device == null)
                    {
                        if (onProcessingCommandBegin != null)
                            onProcessingCommandBegin(this, new onProcessingCommandEventArgs(scene_commands.command_types.device_command, false, string.Format("Processing device command id {0}", cmd.id), cmd.id));

                        if (onProcessingCommandEnd != null)
                            onProcessingCommandEnd(this, new onProcessingCommandEventArgs(scene_commands.command_types.device_command, true, string.Format("Failed to process device command id {0}. Could not locate queued command in database.", cmd.id), cmd.id));

                        return;
                    }

                    //We found the command so continue
                    if (onProcessingCommandBegin != null)
                        onProcessingCommandBegin(this, new onProcessingCommandEventArgs(scene_commands.command_types.device_command, false, string.Format("Processing device command '{0}':'{1}'", cmd.device_commands.friendly_name, cmd.arg), cmd.id));



                    Plugin p = GetPlugins().FirstOrDefault(o => o.Name == cmd.device.device_types.plugin.name);
                    if (p == null)
                    {
                        if (onProcessingCommandEnd != null)
                            onProcessingCommandEnd(this, new onProcessingCommandEventArgs(scene_commands.command_types.device_command, true, string.Format("Failed to process device command id {0}. Could not locate queued commands plug-in.", cmd.id), cmd.id));

                        context.device_command_que.Remove(cmd);
                        context.SaveChanges();

                        return;
                    }

                    if (p.Enabled && p.IsReady)
                    {
                        string details = string.Format("Processed command '{0}':'{1}' on {2} to plug-in '{3}'.",
                                                              cmd.device_commands.friendly_name,                                                             
                                                              cmd.arg,
                                                              cmd.device.friendly_name,
                                                              p.FriendlyName);

                        if (onProcessingCommandEnd != null)
                            onProcessingCommandEnd(this, new onProcessingCommandEventArgs(scene_commands.command_types.device_command, false, details, cmd.id));

                        p.ProcessDeviceCommand(cmd);
                        context.device_command_que.Remove(cmd);
                        context.SaveChanges();
                        return;

                    }
                    else
                    {
                        string err_str = string.Format("Failed to process command '{0}' on '{1}' because the '{2}' plug-in is {3}. Removing command from queue...",
                        cmd.device_commands.friendly_name,
                        cmd.device.friendly_name,
                        cmd.device.device_types.plugin.name,
                        p.Enabled ? "not ready" : "disabled"
                        );

                        if (onProcessingCommandEnd != null)
                            onProcessingCommandEnd(this, new onProcessingCommandEventArgs(scene_commands.command_types.device_command, true, err_str, cmd.id));

                        context.device_command_que.Remove(cmd);
                        context.SaveChanges();
                        return;
                    }
                }
            };
            bw.RunWorkerAsync();
        }

        void builtin_command_que_BuiltinCommandAddedToQueEvent(int builtin_command_que_id)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (s, a) =>
            {
                zvsLocalDBEntities context = new zvsLocalDBEntities();
                builtin_command_que cmd = context.builtin_command_que.FirstOrDefault(c => c.id == builtin_command_que_id);

                if (cmd == null)
                {
                    if (onProcessingCommandBegin != null)
                        onProcessingCommandBegin(this, new onProcessingCommandEventArgs(scene_commands.command_types.builtin, false, string.Format("Processing built-in command id {0}", cmd.id), cmd.id));

                    if (onProcessingCommandEnd != null)
                        onProcessingCommandEnd(this, new onProcessingCommandEventArgs(scene_commands.command_types.builtin, true, string.Format("Failed to process built-in command id {0}. Could not locate queued command in database.", cmd.id), cmd.id));
                    return;
                }

                //We found the command so continue
                if (onProcessingCommandBegin != null)
                    onProcessingCommandBegin(this, new onProcessingCommandEventArgs(scene_commands.command_types.builtin, false, string.Format("Processing built-in command '{0}':'{1}'", cmd.builtin_commands.friendly_name, cmd.arg), cmd.id));

                switch (cmd.builtin_commands.name)
                {
                    case "TIMEDELAY":
                        {
                            int delay = 0;
                            if (int.TryParse(cmd.arg, out delay) && delay > 0)
                            {
                                System.Timers.Timer timer = new System.Timers.Timer();
                                timer.Elapsed += (sender, args) =>
                                {
                                    timer.Stop();

                                    string details = string.Format("Processed command '{0}':'{1}'.",
                                                             cmd.builtin_commands.friendly_name,
                                                             cmd.arg);

                                    if (onProcessingCommandEnd != null)
                                        onProcessingCommandEnd(this, new onProcessingCommandEventArgs(scene_commands.command_types.builtin, false, details, cmd.id));

                                    //Remove processed command from que
                                    context.builtin_command_que.Remove(cmd);
                                    context.SaveChanges();
                                    context.Dispose();

                                };
                                timer.Interval = delay * 1000;
                                timer.Start();
                            }
                            break;
                        }
                    case "REPOLL_ME":
                        {
                            int d_id = 0;
                            int.TryParse(cmd.arg, out d_id);
                            device d = device.GetAllDevices(context, false).FirstOrDefault(o => o.id == d_id);

                            if (d.device_types.plugin.enabled)
                                GetPlugin(d.device_types.plugin.name).Repoll(d);

                            string details = string.Format("Processed command '{0}':'{1}'.",
                                                             cmd.builtin_commands.friendly_name,
                                                             cmd.arg);

                            if (onProcessingCommandEnd != null)
                                onProcessingCommandEnd(this, new onProcessingCommandEventArgs(scene_commands.command_types.builtin, false, details, cmd.id));

                            //Remove processed command from que
                            context.builtin_command_que.Remove(cmd);
                            context.SaveChanges();
                            context.Dispose();

                            break;
                        }
                    case "REPOLL_ALL":
                        {
                            foreach (device d in device.GetAllDevices(context, false))
                            {
                                if (d.device_types.plugin.enabled)
                                {
                                    GetPlugin(d.device_types.plugin.name).Repoll(d);
                                }
                            }

                            string details = string.Format("Processed command '{0}':'{1}'.",
                                                             cmd.builtin_commands.friendly_name,
                                                             cmd.arg);

                            if (onProcessingCommandEnd != null)
                                onProcessingCommandEnd(this, new onProcessingCommandEventArgs(scene_commands.command_types.builtin, false, details, cmd.id));

                            //Remove processed command from que
                            context.builtin_command_que.Remove(cmd);
                            context.SaveChanges();
                            context.Dispose();

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

                            string details = string.Format("Processed command '{0}':'{1}'.",
                                                             cmd.builtin_commands.friendly_name,
                                                             cmd.arg);

                            if (onProcessingCommandEnd != null)
                                onProcessingCommandEnd(this, new onProcessingCommandEventArgs(scene_commands.command_types.builtin, false, details, cmd.id));

                            //Remove processed command from que
                            context.builtin_command_que.Remove(cmd);
                            context.SaveChanges();
                            context.Dispose();

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

                            string details = string.Format("Processed command '{0}':'{1}'.",
                                                             cmd.builtin_commands.friendly_name,
                                                             cmd.arg);

                            if (onProcessingCommandEnd != null)
                                onProcessingCommandEnd(this, new onProcessingCommandEventArgs(scene_commands.command_types.builtin, false, details, cmd.id));

                            //Remove processed command from que
                            context.builtin_command_que.Remove(cmd);
                            context.SaveChanges();
                            context.Dispose();
                            break;
                        }
                    default:
                        {
                            string details = string.Format("Error processing command '{0}':'{1}'. Commnad {2} not recognized.",
                                                             cmd.builtin_commands.friendly_name,
                                                             cmd.arg,
                                                             cmd.builtin_commands.name);

                            if (onProcessingCommandEnd != null)
                                onProcessingCommandEnd(this, new onProcessingCommandEventArgs(scene_commands.command_types.builtin, true, details, cmd.id));

                            //Remove processed command from que
                            context.builtin_command_que.Remove(cmd);
                            context.SaveChanges();
                            context.Dispose();
                            break;
                        }
                }
            };
            bw.RunWorkerAsync();
        }

        public IEnumerable<Plugin> GetPlugins()
        {
            return _plugins;
        }

        public Plugin GetPlugin(string pluginName)
        {
            return _plugins.FirstOrDefault(p => p.Name == pluginName);
        }
        
        public void NotifyPluginSettingsChanged(plugin_settings ps)
        {
            Plugin p = GetPlugin(ps.plugin.name);
            if (p != null)            
                p.SettingsChange(ps);            
        }
    }
}

