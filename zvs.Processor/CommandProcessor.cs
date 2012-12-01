using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zvs.Entities;

namespace zvs.Processor
{
    public class CommandProcessorResult : EventArgs
    {
        public bool Errors { get; private set; }
        public string Details { get; private set; }
        public int QueueCommandId { get; private set; }

        public CommandProcessorResult(bool Errors, string Details, int CommandQueueID)
        {
            this.QueueCommandId = CommandQueueID;
            this.Errors = Errors;
            this.Details = Details;
        }
    }

    public class CommandProcessor
    {
        private Core Core;

        //Constructor
        public CommandProcessor(Core core)
        {
            if (core == null)
                throw new ArgumentException("Core");

            this.Core = core;
        }

        //public Methods 
        public async Task<CommandProcessorResult> RunStoredCommandAsync(int storedCommandId)
        {
            using (zvsContext context = new zvsContext())
            {
                StoredCommand sc = context.StoredCommands.Single(o => o.Id == storedCommandId);

                if (sc.Command == null)
                    return new CommandProcessorResult(true, "Failed to process stored command. StoredCommand command is null.", 0);
                if (sc.Command is DeviceCommand)
                    return await RunDeviceCommandAsync(sc.CommandId, sc.Argument);
                else if (sc.Command is DeviceTypeCommand)
                    return await RunDeviceTypeCommandAsync(sc.Command.Id, sc.Device.Id, sc.Argument);
                else if (sc.Command is JavaScriptCommand)
                    return await RunJavaScriptCommandAsync(sc.Command.Id, sc.Argument);
                else if (sc.Command is BuiltinCommand)
                    return await RunBuiltinCommandAsync(sc.Command.Id, sc.Argument);
            }

            return new CommandProcessorResult(true, "Failed to process stored command. Command type unknown.", 0);
        }

        public async Task<CommandProcessorResult> RunDeviceCommandAsync(int deviceCommandId, string argument = "")
        {
            QueuedDeviceCommand qdc = await Task<QueuedDeviceCommand>.Factory.StartNew(() =>
            {
                using (zvsContext context = new zvsContext())
                {
                    var deviceCmd = context.DeviceCommands.Single(o => o.Id == deviceCommandId);
                    var cmd = new QueuedDeviceCommand
                    {
                        Command = deviceCmd,
                        Argument = argument,
                        Device = deviceCmd.Device
                    };

                    context.QueuedCommands.Add(cmd);
                    context.SaveChanges();
                    return cmd;
                }
            });

            CommandProcessorResult result = await ProcessCommandAsync(qdc);
            if (result.Errors)
                Core.log.Error(result.Details);
            else
                Core.log.Info(result.Details);
            return result;
        }

        public async Task<CommandProcessorResult> RunDeviceTypeCommandAsync(int deviceTypeCommandId, int deviceId, string argument = "")
        {
            QueuedDeviceTypeCommand qdtc = await Task<QueuedDeviceTypeCommand>.Factory.StartNew(() =>
            {
                using (zvsContext context = new zvsContext())
                {
                    var cmd = new QueuedDeviceTypeCommand
                    {
                        Command = context.DeviceTypeCommands.Single(o => o.Id == deviceTypeCommandId),
                        Device = context.Devices.Single(o => o.Id == deviceId),
                        Argument = argument
                    };

                    context.QueuedCommands.Add(cmd);
                    context.SaveChanges();
                    return cmd;
                }
            });
            CommandProcessorResult result = await ProcessCommandAsync(qdtc);
            if (result.Errors)
                Core.log.Error(result.Details);
            else
                Core.log.Info(result.Details);
            return result;
        }

        public async Task<CommandProcessorResult> RunBuiltinCommandAsync(int builtinCommandId, string argument = "")
        {
            QueuedBuiltinCommand qbc = await Task<QueuedBuiltinCommand>.Factory.StartNew(() =>
             {
                 using (zvsContext context = new zvsContext())
                 {
                     var cmd = new QueuedBuiltinCommand
                     {
                         Command = context.BuiltinCommands.Single(o => o.Id == builtinCommandId),
                         Argument = argument
                     };

                     context.QueuedCommands.Add(cmd);
                     context.SaveChanges();
                     return cmd;
                 }
             });

            CommandProcessorResult result = await ProcessCommandAsync(qbc);
            if (result.Errors)
                Core.log.Error(result.Details);
            else
                Core.log.Info(result.Details);
            return result;
        }

        public async Task<CommandProcessorResult> RunJavaScriptCommandAsync(int javaScriptCommandId, string argument = "")
        {
            QueuedJavaScriptCommand qjsc = await Task<QueuedJavaScriptCommand>.Factory.StartNew(() =>
             {
                 using (zvsContext context = new zvsContext())
                 {
                     var cmd = new QueuedJavaScriptCommand
                     {
                         Command = context.JavaScriptCommands.Single(o => o.Id == javaScriptCommandId),
                         Argument = argument
                     };

                     context.QueuedCommands.Add(cmd);
                     context.SaveChanges();
                     return (cmd);
                 }
             });

            CommandProcessorResult result = await ProcessCommandAsync(qjsc);
            if (result.Errors)
                Core.log.Error(result.Details);
            else
                Core.log.Info(result.Details);
            return result;
        }

        //private Methods
        private async Task<CommandProcessorResult> ProcessCommandAsync(QueuedCommand queuedCommand)
        {
            if (queuedCommand == null)
                throw new ArgumentException("queuedCommand");

            zvsContext context = new zvsContext();

            #region QueuedDeviceCommand
            if (queuedCommand is QueuedDeviceCommand)
            {
                QueuedDeviceCommand cmd = null;
                await Task.Factory.StartNew(() =>
                {
                    cmd = context.QueuedCommands.OfType<QueuedDeviceCommand>().FirstOrDefault(o => o.Id == queuedCommand.Id);
                });
                if (cmd == null && cmd.Device == null)
                {
                    context.Dispose();
                    return new CommandProcessorResult(true, string.Format("Queued device cmd {0} failed. Queued command not found in database.", cmd.Id), cmd.Id);
                }

                return await Task<CommandProcessorResult>.Factory.StartNew(() =>
                {
                    zvsPlugin p = Core.pluginManager.GetPlugins().FirstOrDefault(o => o.UniqueIdentifier == cmd.Device.Type.Plugin.UniqueIdentifier);
                    if (p == null)
                    {
                        context.QueuedCommands.Remove(cmd);
                        context.SaveChanges();
                        context.Dispose();
                        return new CommandProcessorResult(true, string.Format("Queued device cmd {0} failed.  Could not locate the associated plug-in.", cmd.Id), cmd.Id);
                    }

                    if (p.Enabled && p.IsReady)
                    {
                        string details = string.Format("Queued device cmd {0}{1} on {2} processed.",
                                                           cmd.Command.Name,
                                                           string.IsNullOrEmpty(cmd.Argument) ? "" : string.Format(" with arg '{0}'", cmd.Argument),
                                                           cmd.Device.Name
                                                           );

                        p.ProcessDeviceCommand(cmd);
                        context.QueuedCommands.Remove(cmd);
                        context.SaveChanges();
                        context.Dispose();
                        return new CommandProcessorResult(false, details, cmd.Id);

                    }
                    else
                    {
                        string err_str = string.Format("Queued device cmd {0}{1} on {2} failed because the '{3}' plug-in is {4}. Removing command from queue...",
                         cmd.Command.Name,
                         string.IsNullOrEmpty(cmd.Argument) ? "" : string.Format(" with arg '{0}'", cmd.Argument),
                         cmd.Device.Name,
                         cmd.Device.Type.Plugin.Name,
                         p.Enabled ? "not ready" : "disabled"
                         );

                        context.QueuedCommands.Remove(cmd);
                        context.SaveChanges();
                        context.Dispose();
                        return new CommandProcessorResult(true, err_str, cmd.Id);
                    }
                });
            }
            #endregion

            #region QueuedDeviceTypeCommand
            else if (queuedCommand is QueuedDeviceTypeCommand)
            {
                QueuedDeviceTypeCommand cmd = null;
                await Task.Factory.StartNew(() =>
                {
                    cmd = context.QueuedCommands.OfType<QueuedDeviceTypeCommand>().FirstOrDefault(o => o.Id == queuedCommand.Id);
                });

                if (cmd == null || cmd.Device == null)
                {
                    context.Dispose();
                    return new CommandProcessorResult(true, string.Format("Queued device type cmd {0} failed. Queued command not found in database.", cmd.Id), cmd.Id);
                }

                return await Task<CommandProcessorResult>.Factory.StartNew(() =>
                {
                    zvsPlugin p = Core.pluginManager.GetPlugins().FirstOrDefault(o => o.UniqueIdentifier == cmd.Device.Type.Plugin.UniqueIdentifier);
                    if (p == null)
                    {
                        context.QueuedCommands.Remove(cmd);
                        context.SaveChanges();
                        context.Dispose();
                        return new CommandProcessorResult(true, string.Format("Queued device type cmd {0} failed. Could not locate the associated plug-in.", cmd.Id), cmd.Id);
                    }


                    if (p.Enabled && p.IsReady)
                    {
                        string details = string.Format("Queued device type cmd {0}{1} on {2} processed.",
                                                            cmd.Command.Name,
                                                            string.IsNullOrEmpty(cmd.Argument) ? "" : string.Format(" with arg '{0}'", cmd.Argument),
                                                            cmd.Device.Name
                                                            );

                        p.ProcessDeviceTypeCommand(cmd);
                        context.QueuedCommands.Remove(cmd);
                        context.SaveChanges();
                        context.Dispose();
                        return new CommandProcessorResult(false, details, cmd.Id);

                    }
                    else
                    {
                        string err_str = string.Format("Queued device type cmd {0}{1} on {2} failed because the '{3}' plug-in is {4}. Removing command from queue...",
                        cmd.Command.Name,
                        string.IsNullOrEmpty(cmd.Argument) ? "" : string.Format(" with arg '{0}'", cmd.Argument),
                        cmd.Device.Name,
                        cmd.Device.Type.Plugin.Name,
                        p.Enabled ? "not ready" : "disabled"
                        );

                        context.QueuedCommands.Remove(cmd);
                        context.SaveChanges();
                        context.Dispose();
                        return new CommandProcessorResult(true, err_str, cmd.Id);
                    }
                });
            }
            #endregion

            #region QueuedBuiltinCommand
            else if (queuedCommand is QueuedBuiltinCommand)
            {
                QueuedBuiltinCommand cmd = null;
                await Task.Factory.StartNew(() =>
                {
                    cmd = context.QueuedCommands.OfType<QueuedBuiltinCommand>().FirstOrDefault(o => o.Id == queuedCommand.Id);
                });
                if (cmd == null)
                {
                    context.Dispose();
                    return new CommandProcessorResult(true, string.Format("Queued built-in cmd {0} failed. Queued command not found in database.", cmd.Id), cmd.Id);
                }

                switch (cmd.Command.UniqueIdentifier)
                {
                    case "TIMEDELAY":
                        {
                            int delay = 0;
                            int.TryParse(cmd.Argument, out delay);

                            await Task.Delay(delay * 1000);

                            string details = string.Format("Queued built-in cmd {0} second time delay processed.", delay);

                            await Task.Factory.StartNew(() =>
                            {
                                //Remove processed command from queue
                                context.QueuedCommands.Remove(cmd);
                                context.SaveChanges();
                                context.Dispose();
                            });

                            return new CommandProcessorResult(false, details, cmd.Id);
                        }
                    case "REPOLL_ME":
                        {
                            int d_id = 0;
                            int.TryParse(cmd.Argument, out d_id);

                            Device device = await Task<Device>.Factory.StartNew(() =>
                            {
                                Device d = Device.GetAllDevices(context, false).FirstOrDefault(o => o.Id == d_id);

                                if (d.Type.Plugin.isEnabled)
                                    Core.pluginManager.GetPlugin(d.Type.Plugin.UniqueIdentifier).Repoll(d);
                                return d;
                            });

                            string details = string.Format("Queued built-in cmd re-poll '{0}' processed.", device.Name);

                            await Task.Factory.StartNew(() =>
                            {
                                //Remove processed command from queue
                                context.QueuedCommands.Remove(cmd);
                                context.SaveChanges();
                                context.Dispose();
                            });

                            return new CommandProcessorResult(false, details, cmd.Id);
                        }
                    case "REPOLL_ALL":
                        {
                            await Task.Factory.StartNew(() =>
                            {
                                foreach (Device d in Device.GetAllDevices(context, false))
                                    if (d.Type.Plugin.isEnabled)
                                        Core.pluginManager.GetPlugin(d.Type.Plugin.UniqueIdentifier).Repoll(d);
                            });

                            string details = "Queued built-in cmd re-poll all devices processed.";

                            await Task.Factory.StartNew(() =>
                            {
                                //Remove processed command from queue
                                context.QueuedCommands.Remove(cmd);
                                context.SaveChanges();
                                context.Dispose();
                            });

                            return new CommandProcessorResult(false, details, cmd.Id);

                        }
                    case "GROUP_ON":
                        {
                            int g_id = 0;
                            int.TryParse(cmd.Argument, out g_id);

                            //EXECUTE ON ALL API's
                            if (g_id > 0)
                            {
                                await Task.Factory.StartNew(() =>
                                {
                                    foreach (zvsPlugin p in Core.pluginManager.GetPlugins())
                                    {
                                        if (p.Enabled)
                                            p.ActivateGroup(g_id);
                                    }
                                });
                            }

                            string GroupName = cmd.Argument;
                            int GroupId = 0;
                            int.TryParse(cmd.Argument, out GroupId);
                            Group g = context.Groups.FirstOrDefault(o => o.Id == GroupId);
                            if (g != null)
                                GroupName = g.Name;

                            string details = string.Format("Queued built-in cmd {0} '{1}' processed.",
                                                             cmd.Command.Name,
                                                             GroupName);

                            await Task.Factory.StartNew(() =>
                            {
                                //Remove processed command from queue
                                context.QueuedCommands.Remove(cmd);
                                context.SaveChanges();
                                context.Dispose();
                            });

                            return new CommandProcessorResult(false, details, cmd.Id);

                        }
                    case "GROUP_OFF":
                        {
                            int g_id = 0;
                            int.TryParse(cmd.Argument, out g_id);
                            //EXECUTE ON ALL API's
                            if (g_id > 0)
                            {
                                await Task.Factory.StartNew(() =>
                                {
                                    foreach (zvsPlugin p in Core.pluginManager.GetPlugins())
                                    {
                                        if (p.Enabled)
                                            p.DeactivateGroup(g_id);
                                    }
                                });
                            }

                            string GroupName = cmd.Argument;
                            int GroupId = 0;
                            int.TryParse(cmd.Argument, out GroupId);
                            Group g = context.Groups.FirstOrDefault(o => o.Id == GroupId);
                            if (g != null)
                                GroupName = g.Name;

                            string details = string.Format("Queued built-in cmd {0} '{1}' processed.",
                                                             cmd.Command.Name,
                                                             GroupName);

                            await Task.Factory.StartNew(() =>
                            {
                                //Remove processed command from queue
                                context.QueuedCommands.Remove(cmd);
                                context.SaveChanges();
                                context.Dispose();
                            });
                            return new CommandProcessorResult(false, details, cmd.Id);

                        }
                    case "RUN_SCENE":
                        {
                            int id = 0;
                            int.TryParse(cmd.Argument, out id);

                            SceneRunner sr = new SceneRunner(Core);
                            sr.onReportProgress += (s, a) =>
                            {
                                Core.log.Info(a.Progress);
                            };
                            SceneRunner.SceneResult result = await sr.RunSceneAsync(id);

                            string details = string.Format("{0} Queued built-in cmd '{1}' processed.",
                                result.Details,
                                cmd.Command.Name);

                            await Task.Factory.StartNew(() =>
                            {
                                //Remove processed command from queue
                                context.QueuedCommands.Remove(cmd);
                                context.SaveChanges();
                                context.Dispose();
                            });

                            return new CommandProcessorResult(result.Errors, details, cmd.Id);

                        }
                    default:
                        {
                            var err = new CommandProcessorResult(true, string.Format("Queued built-in cmd {0} failed. No logic defined for this built-in command.", cmd.Id), cmd.Id);

                            await Task.Factory.StartNew(() =>
                            {
                                //Remove processed command from queue
                                context.QueuedCommands.Remove(cmd);
                                context.SaveChanges();
                                context.Dispose();
                            });

                            return err;
                        }
                }
            }
            #endregion

            #region QueuedJavaScriptCommand
            else if (queuedCommand is QueuedJavaScriptCommand)
            {
                QueuedJavaScriptCommand cmd = null;
                await Task.Factory.StartNew(() =>
                {
                    cmd = context.QueuedCommands.OfType<QueuedJavaScriptCommand>().FirstOrDefault(o => o.Id == queuedCommand.Id);
                });
                if (cmd == null)
                {
                    context.Dispose();
                    return new CommandProcessorResult(true, string.Format("Queued JavaScipt cmd {0} failed. Queued command not found in database.", cmd.Id), cmd.Id);
                }

                // We found the command so continue
                JavaScriptCommand js = (JavaScriptCommand)cmd.Command;
                JavaScriptExecuter je = new JavaScriptExecuter(Core);
                je.onReportProgress += (sender, args) =>
                {
                    Core.log.Info(args.Progress);
                };

                JavaScriptExecuter.JavaScriptResult jsResult = await je.ExecuteScriptAsync(js.Script, context);

                string details = string.Format("{0}. Queued JavaScript cmd '{1}' processed.",
                                jsResult.Details,
                                cmd.Command.Name);

                await Task.Factory.StartNew(() =>
                {
                    //Remove processed command from queue
                    context.QueuedCommands.Remove(cmd);
                    context.SaveChanges();
                    context.Dispose();
                });

                return new CommandProcessorResult(false, details, cmd.Id);
            }
            #endregion

            context.Dispose();
            return new CommandProcessorResult(true, "Failed to process queued command. Command type unknown.", 0);
        }
    }
}
