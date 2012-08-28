using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Data;
using System.Linq;
using System.Threading;
using System;
using System.ComponentModel;

using System.Windows.Threading;
using zvs.Entities;

namespace zvs.Processor
{
    public class PluginManager
    {
        private const int verbose = 10;
        private const string _Name = "Plug-in Manager";
        private Core Core;

        #region Events
        public delegate void onProcessingCommandEventHandler(object sender, onProcessingCommandEventArgs args);
        public class onProcessingCommandEventArgs : EventArgs
        {
            public bool hasErrors { get; private set; }
            public string Details { get; private set; }
            public int CommandQueueID { get; private set; }

            public onProcessingCommandEventArgs(bool Errors, string Details, int CommandQueueID)
            {
                this.CommandQueueID = CommandQueueID;
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
        private IEnumerable<zvsPlugin> _plugins;
#pragma warning restore 649

        public PluginManager(Core Core)
        {
            this.Core = Core;
            DirectoryCatalog catalog = new DirectoryCatalog("plugins");
            CompositionContainer compositionContainer = new CompositionContainer(catalog);
            compositionContainer.ComposeParts(this);

            using (zvsContext context = new zvsContext())
            {
                BuiltinCommand.AddOrEdit(new BuiltinCommand
                {
                    UniqueIdentifier = "REPOLL_ME",
                    Name = "Re-poll Device",
                    ArgumentType = DataType.INTEGER,
                    Description = "This will force a re-poll on an object."
                }, context);

                BuiltinCommand.AddOrEdit(new BuiltinCommand
                {
                    UniqueIdentifier = "REPOLL_ALL",
                    Name = "Re-poll all Devices",
                    ArgumentType = DataType.NONE,
                    Description = "This will force a re-poll on all objects."
                }, context);

                BuiltinCommand.AddOrEdit(new BuiltinCommand
                {
                    UniqueIdentifier = "GROUP_ON",
                    Name = "Turn Group On",
                    ArgumentType = DataType.STRING,
                    Description = "Activates a group."
                }, context);

                BuiltinCommand.AddOrEdit(new BuiltinCommand
                {
                    UniqueIdentifier = "GROUP_OFF",
                    Name = "Turn Group Off",
                    ArgumentType = DataType.STRING,
                    Description = "Deactivates a group."
                }, context);

                BuiltinCommand.AddOrEdit(new BuiltinCommand
                {
                    UniqueIdentifier = "TIMEDELAY",
                    Name = "Scene Time Delay (sec)",
                    ArgumentType = DataType.INTEGER,
                    Description = "Pauses a scene execution for x seconds."
                }, context);

                DeviceProperty.AddOrEdit(new DeviceProperty
                {
                    UniqueIdentifier = "ENABLEPOLLING",
                    Name = "Enable polling for this device.",
                    Value = "false", //default value
                    ValueType = DataType.BOOL,
                    Description = "Toggles automatic polling for a device."
                }, context);

                // Iterate the plug-in
                foreach (zvsPlugin p in _plugins)
                {
                    //keeps this plug-in in scope 
                    var p2 = p;

                    //Plug-in need access to the core in order to use the Logger
                    p2.Core = this.Core;

                    //initialize each plug-in async.
                    BackgroundWorker pluginInitializer = new BackgroundWorker();
                    pluginInitializer.DoWork += (object sender, DoWorkEventArgs e) =>
                    {
                        if (onPluginInitialized != null)
                            onPluginInitialized(this, new onPluginInitializedEventArgs(string.Format("Initializing '{0}'", p2.Name)));

                        p2.Initialize();
                        p2.Start();
                    };
                    pluginInitializer.RunWorkerAsync();
                }
            }
            QueuedCommand.NewCommandCommand += QueuedCommand_NewCommandCommand;
        }

        void QueuedCommand_NewCommandCommand(object sender, QueuedCommand.NewCommandArgs args)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (s, a) =>
            {
                zvsContext context = new zvsContext();
                if (args.Command is QueuedDeviceCommand)
                {
                    QueuedDeviceCommand cmd = context.QueuedCommands.OfType<QueuedDeviceCommand>().FirstOrDefault(o => o.QueuedCommandId == args.Command.QueuedCommandId);
                    ProcessDeviceCmd(context, cmd);
                }
                else if (args.Command is QueuedDeviceTypeCommand)
                {
                    QueuedDeviceTypeCommand cmd = context.QueuedCommands.OfType<QueuedDeviceTypeCommand>().FirstOrDefault(o => o.QueuedCommandId == args.Command.QueuedCommandId);
                    ProcessDeviceTypeCmd(context, cmd);
                }
                else if (args.Command is QueuedBuiltinCommand)
                {
                    QueuedBuiltinCommand cmd = context.QueuedCommands.OfType<QueuedBuiltinCommand>().FirstOrDefault(o => o.QueuedCommandId == args.Command.QueuedCommandId);
                    ProcessBuiltinCmd(context, cmd);
                }
            };
            bw.RunWorkerAsync();
        }

        private void ProcessDeviceCmd(zvsContext context, QueuedDeviceCommand cmd)
        {
            if (cmd == null && cmd.Device == null)
            {
                if (onProcessingCommandBegin != null)
                    onProcessingCommandBegin(this, new onProcessingCommandEventArgs(false, string.Format("Processing device command id {0}", cmd.QueuedCommandId), cmd.QueuedCommandId));

                if (onProcessingCommandEnd != null)
                    onProcessingCommandEnd(this, new onProcessingCommandEventArgs(true, string.Format("Failed to process device command id {0}. Could not locate device in the database.", cmd.QueuedCommandId), cmd.QueuedCommandId));

                context.Dispose();
                return;
            }

            string StartDetails = string.Format("Processing queued device command #{0} ({1} with arg '{2}' on {3})",
                                                     cmd.QueuedCommandId,
                                                     cmd.Command.Name,
                                                     cmd.Argument,
                                                     cmd.Device.Name);

            //We found the command so continue
            if (onProcessingCommandBegin != null)
                onProcessingCommandBegin(this, new onProcessingCommandEventArgs(false, StartDetails, cmd.QueuedCommandId));

            zvsPlugin p = GetPlugins().FirstOrDefault(o => o.UniqueIdentifier == cmd.Device.Type.Plugin.UniqueIdentifier);
            if (p == null)
            {
                if (onProcessingCommandEnd != null)
                    onProcessingCommandEnd(this, new onProcessingCommandEventArgs(true, string.Format("Failed to process device command id {0}. Could not locate queued commands plug-in.", cmd.QueuedCommandId), cmd.QueuedCommandId));

                context.QueuedCommands.Remove(cmd);
                context.SaveChanges();
                context.Dispose();

                return;
            }

            if (p.Enabled && p.IsReady)
            {
                string details = string.Format("Finished processing command #{0} ({1} with arg '{2}' on {3} to plug-in '{4}')",
                                                      cmd.QueuedCommandId,
                                                      cmd.Command.Name,
                                                      cmd.Argument,
                                                      cmd.Device.Name,
                                                      p.Name);

                if (onProcessingCommandEnd != null)
                    onProcessingCommandEnd(this, new onProcessingCommandEventArgs(false, details, cmd.QueuedCommandId));

                p.ProcessDeviceCommand(cmd);
                context.QueuedCommands.Remove(cmd);
                context.SaveChanges();
                context.Dispose();
                return;

            }
            else
            {
                string err_str = string.Format("Failed to process command #{0} '{1}' on '{2}' because the '{3}' plug-in is {4}. Removing command from queue...",
                    cmd.QueuedCommandId,
                    cmd.Command.Name,
                    cmd.Device.Name,
                    cmd.Device.Type.Plugin.Name,
                    p.Enabled ? "not ready" : "disabled");

                if (onProcessingCommandEnd != null)
                    onProcessingCommandEnd(this, new onProcessingCommandEventArgs(true, err_str, cmd.QueuedCommandId));

                context.QueuedCommands.Remove(cmd);
                context.SaveChanges();
                context.Dispose();
                return;
            }
        }

        private void ProcessDeviceTypeCmd(zvsContext context, QueuedDeviceTypeCommand cmd)
        {
            if (cmd == null || cmd.Device == null)
            {
                if (onProcessingCommandBegin != null)
                    onProcessingCommandBegin(this, new onProcessingCommandEventArgs(false, string.Format("Processing device type command id {0}", cmd.QueuedCommandId), cmd.QueuedCommandId));

                if (onProcessingCommandEnd != null)
                    onProcessingCommandEnd(this, new onProcessingCommandEventArgs(true, string.Format("Failed to process device type command id {0}. Could not locate queued command in database.", cmd.QueuedCommandId), cmd.QueuedCommandId));

                context.Dispose();

                return;
            }

            string StartDetails = string.Format("Processing queued device type command #{0} ({1} with arg '{2}' on {3})",
                                                     cmd.QueuedCommandId,
                                                     cmd.Command.Name,
                                                     cmd.Argument,
                                                     cmd.Device.Name);

            //We found the command so continue
            if (onProcessingCommandBegin != null)
                onProcessingCommandBegin(this, new onProcessingCommandEventArgs(false, StartDetails, cmd.QueuedCommandId));

            zvsPlugin p = GetPlugins().FirstOrDefault(o => o.UniqueIdentifier == cmd.Device.Type.Plugin.UniqueIdentifier);
            if (p == null)
            {
                if (onProcessingCommandEnd != null)
                    onProcessingCommandEnd(this, new onProcessingCommandEventArgs(true, string.Format("Failed to process device type command id {0}. Could not locate queued commands plug-in.", cmd.QueuedCommandId), cmd.QueuedCommandId));

                context.QueuedCommands.Remove(cmd);
                context.SaveChanges();
                context.Dispose();

                return;
            }


            if (p.Enabled && p.IsReady)
            {
                string details = string.Format("Finished processing command #{0} ({1} with arg '{2}' on {3} to plug-in '{4}')",
                                                    cmd.QueuedCommandId,
                                                    cmd.Command.Name,
                                                     cmd.Argument,
                                                    cmd.Device.Name,
                                                    p.Name);

                if (onProcessingCommandEnd != null)
                    onProcessingCommandEnd(this, new onProcessingCommandEventArgs(false, details, cmd.QueuedCommandId));

                p.ProcessDeviceTypeCommand(cmd);
                context.QueuedCommands.Remove(cmd);
                context.SaveChanges();
                context.Dispose();
                return;

            }
            else
            {
                string err_str = string.Format("Failed to process command #{0} '{1}' on '{2}' because the '{3}' plug-in is {4}. Removing command from queue...",
                cmd.QueuedCommandId,
                cmd.Command.Name,
                cmd.Device.Name,
                cmd.Device.Type.Plugin.Name,
                p.Enabled ? "not ready" : "disabled"
                );

                if (onProcessingCommandEnd != null)
                    onProcessingCommandEnd(this, new onProcessingCommandEventArgs(true, err_str, cmd.QueuedCommandId));

                context.QueuedCommands.Remove(cmd);
                context.SaveChanges();
                context.Dispose();
                return;
            }
            
        }

        private void ProcessBuiltinCmd(zvsContext context, QueuedBuiltinCommand cmd)
        {
            if (cmd == null)
            {
                if (onProcessingCommandBegin != null)
                    onProcessingCommandBegin(this, new onProcessingCommandEventArgs(false, string.Format("Processing built-in command id {0}", cmd.QueuedCommandId), cmd.QueuedCommandId));

                if (onProcessingCommandEnd != null)
                    onProcessingCommandEnd(this, new onProcessingCommandEventArgs(true, string.Format("Failed to process built-in command id {0}. Could not locate queued command in database.", cmd.QueuedCommandId), cmd.QueuedCommandId));
               
                context.Dispose();
                return;

            }

            string StartDetails = string.Format("Processing queued built-in command #{0} ({1} with arg '{2}')",
                                                         cmd.QueuedCommandId,
                                                         cmd.Command.Name,
                                                         cmd.Argument);

            //We found the command so continue
            if (onProcessingCommandBegin != null)
                onProcessingCommandBegin(this, new onProcessingCommandEventArgs(false, StartDetails, cmd.QueuedCommandId));

            switch (cmd.Command.UniqueIdentifier)
            {
                case "TIMEDELAY":
                    {
                        int delay = 0;
                        if (int.TryParse(cmd.Argument, out delay) && delay > 0)
                        {
                            System.Timers.Timer timer = new System.Timers.Timer();
                            timer.Elapsed += (sender, args) =>
                            {
                                timer.Stop();

                                string details = string.Format("Finished processing queued built-in command #{0} ({1} with arg '{2}')",
                                                         cmd.QueuedCommandId,
                                                         cmd.Command.Name,
                                                         cmd.Argument);

                                if (onProcessingCommandEnd != null)
                                    onProcessingCommandEnd(this, new onProcessingCommandEventArgs(false, details, cmd.QueuedCommandId));

                                //Remove processed command from queue
                                context.QueuedCommands.Remove(cmd);
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
                        int.TryParse(cmd.Argument, out d_id);
                        Device d = Device.GetAllDevices(context, false).FirstOrDefault(o => o.DeviceId == d_id);

                        if (d.Type.Plugin.isEnabled)
                            GetPlugin(d.Type.Plugin.UniqueIdentifier).Repoll(d);

                        string details = string.Format("Finished processing queued built-in command #{0} ({1} with arg '{2}')",
                                                         cmd.QueuedCommandId,
                                                         cmd.Command.Name,
                                                         cmd.Argument);

                        if (onProcessingCommandEnd != null)
                            onProcessingCommandEnd(this, new onProcessingCommandEventArgs(false, details, cmd.QueuedCommandId));

                        //Remove processed command from queue
                        context.QueuedCommands.Remove(cmd);
                        context.SaveChanges();
                        context.Dispose();

                        break;
                    }
                case "REPOLL_ALL":
                    {
                        foreach (Device d in Device.GetAllDevices(context, false))
                        {
                            if (d.Type.Plugin.isEnabled)
                            {
                                GetPlugin(d.Type.Plugin.UniqueIdentifier).Repoll(d);
                            }
                        }

                        string details = string.Format("Finished processing queued built-in command #{0} ({1}))",
                                                          cmd.QueuedCommandId,
                                                         cmd.Command.Name);

                        if (onProcessingCommandEnd != null)
                            onProcessingCommandEnd(this, new onProcessingCommandEventArgs(false, details, cmd.QueuedCommandId));

                        //Remove processed command from queue
                        context.QueuedCommands.Remove(cmd);
                        context.SaveChanges();
                        context.Dispose();

                        break;
                    }
                case "GROUP_ON":
                    {
                        int g_id = 0;
                        int.TryParse(cmd.Argument, out g_id);
                        //EXECUTE ON ALL API's
                        if (g_id > 0)
                        {
                            foreach (zvsPlugin p in GetPlugins())
                            {
                                if (p.Enabled)
                                    p.ActivateGroup(g_id);
                            }
                        }

                        string details = string.Format("Finished processing queued built-in command #{0} ({1} with arg '{2}')",
                                                         cmd.QueuedCommandId,
                                                         cmd.Command.Name,
                                                         cmd.Argument);

                        if (onProcessingCommandEnd != null)
                            onProcessingCommandEnd(this, new onProcessingCommandEventArgs(false, details, cmd.QueuedCommandId));

                        //Remove processed command from queue
                        context.QueuedCommands.Remove(cmd);
                        context.SaveChanges();
                        context.Dispose();

                        break;
                    }
                case "GROUP_OFF":
                    {
                        int g_id = 0;
                        int.TryParse(cmd.Argument, out g_id);
                        //EXECUTE ON ALL API's
                        if (g_id > 0)
                        {
                            foreach (zvsPlugin p in GetPlugins())
                            {
                                if (p.Enabled)
                                    p.DeactivateGroup(g_id);
                            }
                        }

                        string details = string.Format("Finished processing queued built-in command #{0} ({1} with arg '{2}')",
                                                       cmd.QueuedCommandId,
                                                         cmd.Command.Name,
                                                         cmd.Argument);

                        if (onProcessingCommandEnd != null)
                            onProcessingCommandEnd(this, new onProcessingCommandEventArgs(false, details, cmd.QueuedCommandId));

                        //Remove processed command from queue
                        context.QueuedCommands.Remove(cmd);
                        context.SaveChanges();
                        context.Dispose();
                        break;
                    }
                default:
                    {
                        string details = string.Format("Error processing command '{0}':'{1}'. Command {2} not recognized.",
                                                         cmd.Command.Name,
                                                         cmd.Argument,
                                                         cmd.Command.UniqueIdentifier);

                        if (onProcessingCommandEnd != null)
                            onProcessingCommandEnd(this, new onProcessingCommandEventArgs(true, details, cmd.QueuedCommandId));

                        //Remove processed command from queue
                        context.QueuedCommands.Remove(cmd);
                        context.SaveChanges();
                        context.Dispose();
                        break;
                    }
            }
        }

        public IEnumerable<zvsPlugin> GetPlugins()
        {
            return _plugins;
        }

        public zvsPlugin GetPlugin(string uniqueIdentifier)
        {
            return _plugins.FirstOrDefault(p => p.UniqueIdentifier == uniqueIdentifier);
        }

        public void NotifyPluginSettingsChanged(PluginSetting ps)
        {
            zvsPlugin p = GetPlugin(ps.Plugin.UniqueIdentifier);
            if (p != null)
                p.SettingsChange(ps);
        }
    }
}

