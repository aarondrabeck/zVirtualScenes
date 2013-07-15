using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zvs.Entities;
using System.Data.Entity;

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
                throw new ArgumentNullException("Core");

            this.Core = core;
        }

        //public Methods 
        public async Task<CommandProcessorResult> RunStoredCommandAsync(object sender, int storedCommandId)
        {
            zvsContext context = new zvsContext();

            var sCmd = await context.StoredCommands
                .Include(o => o.Command)
                .SingleAsync(o => o.Id == storedCommandId);
            context.Dispose();

            if (sCmd.Command == null)
                return new CommandProcessorResult(true, "Failed to process stored command. StoredCommand command is null.", 0);

            return await RunCommandAsync(sender, sCmd.CommandId, sCmd.Argument, sCmd.Argument2);
        }

        public async Task<CommandProcessorResult> RunCommandAsync(object sender, int commandId, string argument = "", string argument2 = "")
        {
            zvsContext context = new zvsContext();

            var queuedCommand = new QueuedCommand
            {
                Command = await context.Commands.SingleAsync(o => o.Id == commandId),
                Argument = argument,
                Argument2 = argument2
            };

            context.QueuedCommands.Add(queuedCommand);
            await context.TrySaveChangesAsync();
            context.Dispose();

            CommandProcessorResult result = await ProcessCommandAsync(sender, queuedCommand.Id);
            if (result.HasErrors)
                Core.log.Error(result.Details);
            else
                Core.log.Info(result.Details);
            return result;
        }

        //private Methods
        private async Task<CommandProcessorResult> ProcessCommandAsync(object sender, int queuedCommandId)
        {
            CommandProcessorResult result = new CommandProcessorResult(true, "Failed to process queued command. Command type unknown.", 0);

            using (zvsContext context = new zvsContext())
            {
                QueuedCommand queuedCommand = await context.QueuedCommands
                    .Include(o => o.Command)
                    .FirstOrDefaultAsync(o => o.Id == queuedCommandId);

                if (queuedCommand == null)
                    return new CommandProcessorResult(true, string.Format("Queued command {0} not found in database.", queuedCommandId), queuedCommandId);

                Command command = queuedCommand.Command;

                #region DeviceCommand
                if (command is DeviceCommand)
                {
                    DeviceCommand deviceCommand = (DeviceCommand)command;

                    var aGuid = deviceCommand.Device.Type.Adapter.AdapterGuid;
                    if (!Core.AdapterManager.AdapterGuidToAdapterDictionary.ContainsKey(aGuid))
                    {
                        context.QueuedCommands.Remove(queuedCommand);
                        await context.TrySaveChangesAsync();

                        return new CommandProcessorResult(true, string.Format("Queued device cmd {0} failed.  Could not locate the associated plug-in.", queuedCommand.Id),
                            queuedCommand.Id);
                    }

                    var adapter = Core.AdapterManager.AdapterGuidToAdapterDictionary[aGuid];
                    if (adapter.IsEnabled)
                    {
                        string details = string.Format("Queued device cmd {0}{1} on {2} processed.",
                                                           deviceCommand.Name,
                                                           string.IsNullOrEmpty(queuedCommand.Argument) ? "" : string.Format(" with arg '{0}'", queuedCommand.Argument),
                                                           deviceCommand.Device.Name
                                                           );

                        await adapter.ProcessCommandAsync(queuedCommand.Id);
                        context.QueuedCommands.Remove(queuedCommand);
                        await context.TrySaveChangesAsync();
                        return new CommandProcessorResult(false, details, queuedCommand.Id);

                    }
                    else
                    {
                        string err_str = string.Format("Queued device cmd {0}{1} on {2} failed because the '{3}' plug-in is {4}. Removing command from queue...",
                         deviceCommand.Name,
                         string.IsNullOrEmpty(queuedCommand.Argument) ? "" : string.Format(" with arg '{0}'", queuedCommand.Argument),
                         deviceCommand.Device.Name,
                         deviceCommand.Device.Type.Adapter.Name,
                         adapter.IsEnabled ? "not ready" : "disabled"
                         );

                        context.QueuedCommands.Remove(queuedCommand);
                        await context.TrySaveChangesAsync();
                        return new CommandProcessorResult(true, err_str, queuedCommand.Id);
                    }

                }
                #endregion

                #region DeviceTypeCommand
                else if (command is DeviceTypeCommand)
                {
                    int dId = int.TryParse(queuedCommand.Argument2, out dId) ? dId : 0;
                    var device = await context.Devices.FirstOrDefaultAsync(o => o.Id == dId);

                    if (device == null)
                    {
                        context.QueuedCommands.Remove(queuedCommand);
                        await context.TrySaveChangesAsync();
                        return new CommandProcessorResult(true, string.Format("Queued device type cmd {0} failed. Invalid device id.", queuedCommand.Id), queuedCommand.Id);
                    }

                    var aGuid = device.Type.Adapter.AdapterGuid;
                    if (!Core.AdapterManager.AdapterGuidToAdapterDictionary.ContainsKey(aGuid))
                    {
                        context.QueuedCommands.Remove(queuedCommand);
                        await context.TrySaveChangesAsync();
                        return new CommandProcessorResult(true, string.Format("Queued device type cmd {0} failed.  Could not locate the associated plug-in.", queuedCommand.Id),
                            queuedCommand.Id);
                    }

                    var adapter = Core.AdapterManager.AdapterGuidToAdapterDictionary[aGuid];

                    if (adapter.IsEnabled)
                    {
                        string details = string.Format("Queued device type cmd {0}{1} on {2} processed.",
                                                            command.Name,
                                                            string.IsNullOrEmpty(queuedCommand.Argument) ? "" : string.Format(" with arg '{0}'", queuedCommand.Argument),
                                                            device.Name
                                                            );

                        await adapter.ProcessCommandAsync(queuedCommand.Id);
                        context.QueuedCommands.Remove(queuedCommand);
                        await context.TrySaveChangesAsync();
                        return new CommandProcessorResult(false, details, queuedCommand.Id);

                    }
                    else
                    {
                        string err_str = string.Format("Queued device type cmd {0}{1} on {2} failed because the '{3}' plug-in is {4}. Removing command from queue...",
                        command.Name,
                        string.IsNullOrEmpty(queuedCommand.Argument) ? "" : string.Format(" with arg '{0}'", queuedCommand.Argument),
                        device.Name,
                        device.Type.Adapter.Name,
                        adapter.IsEnabled ? "not ready" : "disabled"
                        );

                        context.QueuedCommands.Remove(queuedCommand);
                        await context.TrySaveChangesAsync();
                        return new CommandProcessorResult(true, err_str, queuedCommand.Id);
                    }

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


                                //Remove processed command from queue
                                context.QueuedCommands.Remove(queuedCommand);
                                await context.TrySaveChangesAsync();

                                return new CommandProcessorResult(false, details, queuedCommand.Id);
                            }
                        case "REPOLL_ME":
                            {
                                int d_id = 0;
                                int.TryParse(queuedCommand.Argument, out d_id);

                                var device = await context.Devices
                                    .Include(o => o.Type.Adapter)
                                    .FirstOrDefaultAsync(o => o.Type.UniqueIdentifier != "BUILTIN" && o.Id == d_id);

                                if (!Core.AdapterManager.AdapterGuidToAdapterDictionary.ContainsKey(device.Type.Adapter.AdapterGuid))
                                {
                                    context.QueuedCommands.Remove(queuedCommand);
                                    await context.TrySaveChangesAsync();
                                    return new CommandProcessorResult(true, string.Format("Queued device type cmd {0} failed.  Could not locate the associated adapter.", queuedCommand.Id),
                                        queuedCommand.Id);
                                }
                                var adapter = Core.AdapterManager.AdapterGuidToAdapterDictionary[device.Type.Adapter.AdapterGuid];

                                if (!adapter.IsEnabled)
                                {
                                    context.QueuedCommands.Remove(queuedCommand);
                                    await context.TrySaveChangesAsync();

                                    return new CommandProcessorResult(true, string.Format("Queued device type cmd {0} failed.  Adapter not enabled.", queuedCommand.Id),
                                        queuedCommand.Id);
                                }
                                await adapter.RepollAsync(device, context);

                                string details = string.Format("Queued built-in cmd re-poll '{0}' processed.", device.Name);

                                //Remove processed command from queue
                                context.QueuedCommands.Remove(queuedCommand);
                                await context.TrySaveChangesAsync();

                                return new CommandProcessorResult(false, details, queuedCommand.Id);
                            }
                        case "REPOLL_ALL":
                            {
                                var devices = await context.Devices
                                    .Include(o => o.Type.Adapter)
                                    .Where(o => o.Type.UniqueIdentifier != "BUILTIN")
                                    .ToListAsync();

                                foreach (var device in devices)
                                {
                                    var d = device;
                                    if (!Core.AdapterManager.AdapterGuidToAdapterDictionary.ContainsKey(d.Type.Adapter.AdapterGuid))
                                    {
                                        Core.log.WarnFormat("Re-poll all, could not locate the associated adapter for device {0}", d.Name);
                                        continue;
                                    }

                                    var adapter = Core.AdapterManager.AdapterGuidToAdapterDictionary[d.Type.Adapter.AdapterGuid];

                                    if (!adapter.IsEnabled)
                                    {
                                        Core.log.WarnFormat("Re-poll all, adapter for device {0} is disabled", d.Name);
                                        continue;
                                    }

                                    await adapter.RepollAsync(d, context);
                                }

                                string details = "Queued built-in cmd re-poll all devices processed.";

                                //Remove processed command from queue
                                context.QueuedCommands.Remove(queuedCommand);
                                await context.TrySaveChangesAsync();

                                return new CommandProcessorResult(false, details, queuedCommand.Id);

                            }
                        case "GROUP_ON":
                            {
                                int g_id = int.TryParse(queuedCommand.Argument, out g_id) ? g_id : 0;
                                Group group = await context.Groups.FirstOrDefaultAsync(o => o.Id == g_id);

                                if (group == null)
                                {
                                    context.QueuedCommands.Remove(queuedCommand);
                                    await context.TrySaveChangesAsync();

                                    return new CommandProcessorResult(true, string.Format("Queued device type cmd {0} failed.  Invalid group id.", queuedCommand.Id),
                                        queuedCommand.Id);
                                }

                                //EXECUTE ON ALL API's
                                foreach (var guid in Core.AdapterManager.AdapterGuidToAdapterDictionary.Keys)
                                {
                                    var adapter = Core.AdapterManager.AdapterGuidToAdapterDictionary[guid];

                                    if (!adapter.IsEnabled)
                                        continue;

                                    await adapter.ActivateGroupAsync(group, context);
                                }

                                string details = string.Format("Queued built-in cmd {0} '{1}' processed.",
                                                                 command.Name,
                                                                 group.Name);

                                //Remove processed command from queue
                                context.QueuedCommands.Remove(queuedCommand);
                                await context.TrySaveChangesAsync();

                                return new CommandProcessorResult(false, details, queuedCommand.Id);

                            }
                        case "GROUP_OFF":
                            {
                                int g_id = int.TryParse(queuedCommand.Argument, out g_id) ? g_id : 0;
                                Group group = await context.Groups.FirstOrDefaultAsync(o => o.Id == g_id);

                                if (group == null)
                                {
                                    context.QueuedCommands.Remove(queuedCommand);
                                    await context.TrySaveChangesAsync();

                                    return new CommandProcessorResult(true, string.Format("Queued device type cmd {0} failed.  Invalid group id.", queuedCommand.Id),
                                        queuedCommand.Id);
                                }

                                //EXECUTE ON ALL API's
                                foreach (var guid in Core.AdapterManager.AdapterGuidToAdapterDictionary.Keys)
                                {
                                    var adapter = Core.AdapterManager.AdapterGuidToAdapterDictionary[guid];

                                    if (!adapter.IsEnabled)
                                        continue;

                                    await adapter.DeactivateGroupAsync(group, context);
                                }

                                string details = string.Format("Queued built-in cmd {0} '{1}' processed.",
                                                                 command.Name,
                                                                 group.Name);

                                //Remove processed command from queue
                                context.QueuedCommands.Remove(queuedCommand);
                                await context.TrySaveChangesAsync();

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

                                //Remove processed command from queue
                                context.QueuedCommands.Remove(queuedCommand);
                                await context.TrySaveChangesAsync();

                                return new CommandProcessorResult(sceneResult.Errors, details, queuedCommand.Id);

                            }
                        default:
                            {
                                var err = new CommandProcessorResult(true, string.Format("Queued built-in cmd {0} failed. No logic defined for this built-in command.", queuedCommand.Id), queuedCommand.Id);

                                //Remove processed command from queue
                                context.QueuedCommands.Remove(queuedCommand);
                                await context.TrySaveChangesAsync();

                                return err;
                            }
                    }
                }
                #endregion

                #region QueuedJavaScriptCommand
                else if (command is JavaScriptCommand)
                {
                    JavaScriptCommand javaScriptCommand = (JavaScriptCommand)command;

                    JavaScriptExecuter je = new JavaScriptExecuter(sender, Core);
                    je.onReportProgress += (s, args) =>
                    {
                        Core.log.Info(args.Progress);
                    };

                    JavaScriptExecuter.JavaScriptResult jsResult = await je.ExecuteScriptAsync(javaScriptCommand.Script, context);

                    string details = string.Format("{0}. Queued JavaScript cmd '{1}' processed.",
                                    jsResult.Details,
                                    command.Name);

                    //Remove processed command from queue
                    context.QueuedCommands.Remove(queuedCommand);
                    await context.TrySaveChangesAsync();

                    return new CommandProcessorResult(false, details, queuedCommand.Id);
                }
                #endregion


                context.QueuedCommands.Remove(queuedCommand);
                await context.TrySaveChangesAsync();


                return result;
            }
        }
    }
}
