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
        public bool HasErrors { get; private set; }
        public string Details { get; private set; }
        public int QueueCommandId { get; private set; }

        public CommandProcessorResult(bool Errors, string Details, int CommandQueueID)
        {
            this.QueueCommandId = CommandQueueID;
            this.HasErrors = Errors;
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
            StoredCommand sc = await Task<StoredCommand>.Factory.StartNew(() =>
            {
                using (zvsContext context = new zvsContext())
                {
                    var sCmd = context.StoredCommands.Single(o => o.Id == storedCommandId);
                    var a = sCmd.Command; //TODO: Query for cmd while context is open...replace this with .include in EF 6
                    return sCmd;
                }
            });

            if (sc.Command == null)
                return new CommandProcessorResult(true, "Failed to process stored command. StoredCommand command is null.", 0);

            return await RunCommandAsync(sc.CommandId, sc.Argument, sc.Argument2);
        }

        public async Task<CommandProcessorResult> RunCommandAsync(int commandId, string argument = "", string argument2 = "")
        {
            QueuedCommand queuedCommand = await Task<QueuedCommand>.Factory.StartNew(() =>
            {
                using (zvsContext context = new zvsContext())
                {
                    var cmd = new QueuedCommand
                    {
                        Command = context.Commands.Single(o => o.Id == commandId),
                        Argument = argument,
                        Argument2 = argument2
                    };

                    context.QueuedCommands.Add(cmd);
                    context.SaveChanges();
                    return cmd;
                }
            });

            CommandProcessorResult result = await ProcessCommandAsync(queuedCommand.Id);
            if (result.HasErrors)
                Core.log.Error(result.Details);
            else
                Core.log.Info(result.Details);
            return result;
        }

        //private Methods
        private async Task<CommandProcessorResult> ProcessCommandAsync(int queuedCommandId)
        {
            CommandProcessorResult result = new CommandProcessorResult(true, "Failed to process queued command. Command type unknown.", 0);

            using (zvsContext context = new zvsContext())
            {
                QueuedCommand queuedCommand = await Task.Factory.StartNew(() =>
                {
                    return context.QueuedCommands.FirstOrDefault(o => o.Id == queuedCommandId);
                });

                if (queuedCommand == null)
                    return new CommandProcessorResult(true, string.Format("Queued command {0} not found in database.", queuedCommandId), queuedCommandId);

                Command command = await Task.Factory.StartNew(() =>
                {
                    return queuedCommand.Command;
                });

                #region DeviceCommand
                if (command is DeviceCommand)
                {
                    DeviceCommand deviceCommand = (DeviceCommand)command;
                    return await Task<CommandProcessorResult>.Factory.StartNew(() =>
                    {
                        zvsPlugin p = Core.pluginManager.GetPlugins().FirstOrDefault(o => o.UniqueIdentifier == deviceCommand.Device.Type.Plugin.UniqueIdentifier);
                        if (p == null)
                        {
                            context.QueuedCommands.Remove(queuedCommand);
                            context.SaveChanges();
                            return new CommandProcessorResult(true, string.Format("Queued device cmd {0} failed.  Could not locate the associated plug-in.", queuedCommand.Id), queuedCommand.Id);
                        }

                        if (p.Enabled && p.IsReady)
                        {
                            string details = string.Format("Queued device cmd {0}{1} on {2} processed.",
                                                               deviceCommand.Name,
                                                               string.IsNullOrEmpty(queuedCommand.Argument) ? "" : string.Format(" with arg '{0}'", queuedCommand.Argument),
                                                               deviceCommand.Device.Name
                                                               );

                            p.ProcessCommand(queuedCommand.Id);
                            context.QueuedCommands.Remove(queuedCommand);
                            context.SaveChanges();
                            return new CommandProcessorResult(false, details, queuedCommand.Id);

                        }
                        else
                        {
                            string err_str = string.Format("Queued device cmd {0}{1} on {2} failed because the '{3}' plug-in is {4}. Removing command from queue...",
                             deviceCommand.Name,
                             string.IsNullOrEmpty(queuedCommand.Argument) ? "" : string.Format(" with arg '{0}'", queuedCommand.Argument),
                             deviceCommand.Device.Name,
                             deviceCommand.Device.Type.Plugin.Name,
                             p.Enabled ? "not ready" : "disabled"
                             );

                            context.QueuedCommands.Remove(queuedCommand);
                            context.SaveChanges();
                            return new CommandProcessorResult(true, err_str, queuedCommand.Id);
                        }
                    });
                }
                #endregion

                #region DeviceTypeCommand
                else if (command is DeviceTypeCommand)
                {
                    return await Task<CommandProcessorResult>.Factory.StartNew(() =>
                    {
                        Device device = null;
                        if (!Device.TryGetDevice(context, queuedCommand.Argument2, out device))
                        {
                            context.QueuedCommands.Remove(queuedCommand);
                            context.SaveChanges();
                            return new CommandProcessorResult(true, string.Format("Queued device type cmd {0} failed. Invalid device id.", queuedCommand.Id), queuedCommand.Id);
                        }

                        zvsPlugin p = Core.pluginManager.GetPlugins().FirstOrDefault(o => o.UniqueIdentifier == device.Type.Plugin.UniqueIdentifier);
                        if (p == null)
                        {
                            context.QueuedCommands.Remove(queuedCommand);
                            context.SaveChanges();
                            return new CommandProcessorResult(true, string.Format("Queued device type cmd {0} failed. Could not locate the associated plug-in.", queuedCommand.Id), queuedCommand.Id);
                        }

                        if (p.Enabled && p.IsReady)
                        {
                            string details = string.Format("Queued device type cmd {0}{1} on {2} processed.",
                                                                command.Name,
                                                                string.IsNullOrEmpty(queuedCommand.Argument) ? "" : string.Format(" with arg '{0}'", queuedCommand.Argument),
                                                                device.Name
                                                                );

                            p.ProcessCommand(queuedCommand.Id);
                            context.QueuedCommands.Remove(queuedCommand);
                            context.SaveChanges();
                            return new CommandProcessorResult(false, details, queuedCommand.Id);

                        }
                        else
                        {
                            string err_str = string.Format("Queued device type cmd {0}{1} on {2} failed because the '{3}' plug-in is {4}. Removing command from queue...",
                            command.Name,
                            string.IsNullOrEmpty(queuedCommand.Argument) ? "" : string.Format(" with arg '{0}'", queuedCommand.Argument),
                            device.Name,
                            device.Type.Plugin.Name,
                            p.Enabled ? "not ready" : "disabled"
                            );

                            context.QueuedCommands.Remove(queuedCommand);
                            context.SaveChanges();
                            return new CommandProcessorResult(true, err_str, queuedCommand.Id);
                        }
                    });
                }
                #endregion

                #region BuiltinCommand
                else if (command is BuiltinCommand)
                {
                    switch (command.UniqueIdentifier)
                    {
                        case "TIMEDELAY":
                            {
                                int delay = 0;
                                int.TryParse(queuedCommand.Argument, out delay);

                                await Task.Delay(delay * 1000);

                                string details = string.Format("Queued built-in cmd {0} second time delay processed.", delay);

                                await Task.Factory.StartNew(() =>
                                {
                                    //Remove processed command from queue
                                    context.QueuedCommands.Remove(queuedCommand);
                                    context.SaveChanges();
                                });

                                return new CommandProcessorResult(false, details, queuedCommand.Id);
                            }
                        case "REPOLL_ME":
                            {
                                int d_id = 0;
                                int.TryParse(queuedCommand.Argument, out d_id);

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
                                    context.QueuedCommands.Remove(queuedCommand);
                                    context.SaveChanges();
                                });

                                return new CommandProcessorResult(false, details, queuedCommand.Id);
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
                                    context.QueuedCommands.Remove(queuedCommand);
                                    context.SaveChanges();
                                });

                                return new CommandProcessorResult(false, details, queuedCommand.Id);

                            }
                        case "GROUP_ON":
                            {
                                int g_id = 0;
                                int.TryParse(queuedCommand.Argument, out g_id);

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

                                string GroupName = queuedCommand.Argument;
                                int GroupId = 0;
                                int.TryParse(queuedCommand.Argument, out GroupId);
                                Group g = context.Groups.FirstOrDefault(o => o.Id == GroupId);
                                if (g != null)
                                    GroupName = g.Name;

                                string details = string.Format("Queued built-in cmd {0} '{1}' processed.",
                                                                 command.Name,
                                                                 GroupName);

                                await Task.Factory.StartNew(() =>
                                {
                                    //Remove processed command from queue
                                    context.QueuedCommands.Remove(queuedCommand);
                                    context.SaveChanges();
                                });

                                return new CommandProcessorResult(false, details, queuedCommand.Id);

                            }
                        case "GROUP_OFF":
                            {
                                int g_id = 0;
                                int.TryParse(queuedCommand.Argument, out g_id);
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

                                string GroupName = queuedCommand.Argument;
                                int GroupId = 0;
                                int.TryParse(queuedCommand.Argument, out GroupId);
                                Group g = context.Groups.FirstOrDefault(o => o.Id == GroupId);
                                if (g != null)
                                    GroupName = g.Name;

                                string details = string.Format("Queued built-in cmd {0} '{1}' processed.",
                                                                 command.Name,
                                                                 GroupName);

                                await Task.Factory.StartNew(() =>
                                {
                                    //Remove processed command from queue
                                    context.QueuedCommands.Remove(queuedCommand);
                                    context.SaveChanges();
                                });
                                return new CommandProcessorResult(false, details, queuedCommand.Id);

                            }
                        case "RUN_SCENE":
                            {
                                int id = 0;
                                int.TryParse(queuedCommand.Argument, out id);

                                SceneRunner sr = new SceneRunner(Core);
                                sr.onReportProgress += (s, a) =>
                                {
                                    Core.log.Info(a.Progress);
                                };
                                SceneRunner.SceneResult sceneResult = await sr.RunSceneAsync(id);

                                string details = string.Format("{0} Queued built-in cmd '{1}' processed.",
                                    sceneResult.Details,
                                    command.Name);

                                await Task.Factory.StartNew(() =>
                                {
                                    //Remove processed command from queue
                                    context.QueuedCommands.Remove(queuedCommand);
                                    context.SaveChanges();
                                });

                                return new CommandProcessorResult(sceneResult.Errors, details, queuedCommand.Id);

                            }
                        default:
                            {
                                var err = new CommandProcessorResult(true, string.Format("Queued built-in cmd {0} failed. No logic defined for this built-in command.", queuedCommand.Id), queuedCommand.Id);

                                await Task.Factory.StartNew(() =>
                                {
                                    //Remove processed command from queue
                                    context.QueuedCommands.Remove(queuedCommand);
                                    context.SaveChanges();
                                });

                                return err;
                            }
                    }
                }
                #endregion

                #region QueuedJavaScriptCommand
                else if (command is JavaScriptCommand)
                {
                    JavaScriptCommand javaScriptCommand = (JavaScriptCommand)command;

                    JavaScriptExecuter je = new JavaScriptExecuter(Core, queuedCommand.Argument, queuedCommand.Argument2);
                    je.onReportProgress += (sender, args) =>
                    {
                        Core.log.Info(args.Progress);
                    };

                    JavaScriptExecuter.JavaScriptResult jsResult = await je.ExecuteScriptAsync(javaScriptCommand.Script, context);

                    string details = string.Format("{0}. Queued JavaScript cmd '{1}' processed.",
                                    jsResult.Details,
                                    command.Name);

                    await Task.Factory.StartNew(() =>
                    {
                        //Remove processed command from queue
                        context.QueuedCommands.Remove(queuedCommand);
                        context.SaveChanges();
                    });

                    return new CommandProcessorResult(false, details, queuedCommand.Id);
                }
                #endregion

                await Task.Factory.StartNew(() =>
                {
                    context.QueuedCommands.Remove(queuedCommand);
                    context.SaveChanges();
                });

                return result;
            }
        }
    }
}
