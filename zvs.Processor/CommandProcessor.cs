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

        public CommandProcessorResult(bool Errors, string Details)
        {
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
                return new CommandProcessorResult(true, "Failed to process stored command. StoredCommand command is null.");

            return await RunCommandAsync(sender, sCmd.CommandId, sCmd.Argument, sCmd.Argument2);
        }

        public async Task<CommandProcessorResult> RunCommandAsync(object sender, int commandId, string argument = "", string argument2 = "")
        {
            var result = await ProcessCommandAsync(sender, commandId, argument, argument2);

            if (result.HasErrors)
                Core.log.Error(result.Details);
            else
                Core.log.Info(result.Details);
            return result;
        }

        //private Methods
        private async Task<CommandProcessorResult> ProcessCommandAsync(object sender, int commandId, string argument = "", string argument2 = "")
        {
            CommandProcessorResult result = new CommandProcessorResult(true, "Failed to process command. Command type unknown.");

            using (zvsContext context = new zvsContext())
            {
                var command = await context.Commands.FirstOrDefaultAsync(o => o.Id == commandId);

                if (command == null)
                    return new CommandProcessorResult(true, string.Format("Command {0} not found in database.", commandId));

                #region DeviceCommand
                if (command is DeviceCommand)
                {
                    var deviceCommand = await context.DeviceCommands
                        .Include(o => o.Device)
                        .Include(o => o.Device.Type)
                        .Include(o => o.Device.Type.Adapter)
                        .FirstOrDefaultAsync(o => o.Id == commandId);

                    var aGuid = deviceCommand.Device.Type.Adapter.AdapterGuid;
                    if (!Core.AdapterManager.AdapterGuidToAdapterDictionary.ContainsKey(aGuid))
                    {
                        return new CommandProcessorResult(true, string.Format("Device cmd {0} failed.  Could not locate the associated plug-in.", commandId));
                    }

                    var adapter = Core.AdapterManager.AdapterGuidToAdapterDictionary[aGuid];
                    if (adapter.IsEnabled)
                    {
                        string details = string.Format("Device cmd {0}{1} on {2} processed.",
                                                           deviceCommand.Name,
                                                           string.IsNullOrEmpty(argument) ? "" : string.Format(" with arg '{0}'", argument),
                                                           deviceCommand.Device.Name
                                                           );

                        await adapter.ProcessDeviceCommandAsync(deviceCommand.Device, deviceCommand, argument, argument2);
                        return new CommandProcessorResult(false, details);
                    }
                    else
                    {
                        string err_str = string.Format("Device cmd {0}{1} on {2} failed because the '{3}' plug-in is {4}. Removing command from queue...",
                         deviceCommand.Name,
                         string.IsNullOrEmpty(argument) ? "" : string.Format(" with arg '{0}'", argument),
                         deviceCommand.Device.Name,
                         deviceCommand.Device.Type.Adapter.Name,
                         adapter.IsEnabled ? "not ready" : "disabled"
                         );

                        return new CommandProcessorResult(true, err_str);
                    }

                }
                #endregion

                #region DeviceTypeCommand
                else if (command is DeviceTypeCommand)
                {
                    int dId = int.TryParse(argument2, out dId) ? dId : 0;

                    var device = await context.Devices
                        .Include(o => o.Type)
                        .Include(o => o.Type.Adapter)
                        .FirstOrDefaultAsync(o => o.Id == dId);

                    if (device == null)
                        return new CommandProcessorResult(true, string.Format("Ddevice type cmd {0} failed. Invalid device id.", command.Id));

                    var aGuid = device.Type.Adapter.AdapterGuid;
                    if (!Core.AdapterManager.AdapterGuidToAdapterDictionary.ContainsKey(aGuid))
                        return new CommandProcessorResult(true, string.Format("Device type cmd {0} failed.  Could not locate the associated plug-in.", command.Id));

                    var adapter = Core.AdapterManager.AdapterGuidToAdapterDictionary[aGuid];

                    if (adapter.IsEnabled)
                    {
                        string details = string.Format("Device type cmd {0}{1} on {2} processed.",
                                                            command.Name,
                                                            string.IsNullOrEmpty(argument) ? "" : string.Format(" with arg '{0}'", argument),
                                                            device.Name
                                                            );

                        await adapter.ProcessDeviceTypeCommandAsync(device.Type, device, command as DeviceTypeCommand, argument);
                        return new CommandProcessorResult(false, details);
                    }
                    else
                    {
                        string err_str = string.Format("Device type cmd {0}{1} on {2} failed because the '{3}' plug-in is {4}. Removing command from queue...",
                        command.Name,
                        string.IsNullOrEmpty(argument) ? "" : string.Format(" with arg '{0}'", argument),
                        device.Name,
                        device.Type.Adapter.Name,
                        adapter.IsEnabled ? "not ready" : "disabled"
                        );

                        return new CommandProcessorResult(true, err_str);
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
                                int.TryParse(argument, out delay);

                                await Task.Delay(delay * 1000);

                                string details = string.Format("Built-in cmd {0} second time delay processed.", delay);

                                return new CommandProcessorResult(false, details);
                            }
                        case "REPOLL_ME":
                            {
                                int d_id = 0;
                                int.TryParse(argument, out d_id);

                                var device = await context.Devices
                                    .Include(o => o.Type.Adapter)
                                    .FirstOrDefaultAsync(o => o.Type.UniqueIdentifier != "BUILTIN" && o.Id == d_id);

                                if (!Core.AdapterManager.AdapterGuidToAdapterDictionary.ContainsKey(device.Type.Adapter.AdapterGuid))
                                {
                                    return new CommandProcessorResult(true, string.Format("Device type cmd {0} failed.  Could not locate the associated adapter.", commandId));
                                }
                                var adapter = Core.AdapterManager.AdapterGuidToAdapterDictionary[device.Type.Adapter.AdapterGuid];

                                if (!adapter.IsEnabled)
                                    return new CommandProcessorResult(true, string.Format("Device type cmd {0} failed.  Adapter not enabled.", commandId));

                                await adapter.RepollAsync(device, context);

                                string details = string.Format("Built-in cmd re-poll '{0}' processed.", device.Name);
                                return new CommandProcessorResult(false, details);
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

                                string details = "Built-in cmd re-poll all devices processed.";
                                return new CommandProcessorResult(false, details);

                            }
                        case "GROUP_ON":
                            {
                                int g_id = int.TryParse(argument, out g_id) ? g_id : 0;
                                Group group = await context.Groups.FirstOrDefaultAsync(o => o.Id == g_id);

                                if (group == null)
                                    return new CommandProcessorResult(true, string.Format("Device type cmd {0} failed.  Invalid group id.", command.Id));

                                //EXECUTE ON ALL API's
                                foreach (var guid in Core.AdapterManager.AdapterGuidToAdapterDictionary.Keys)
                                {
                                    var adapter = Core.AdapterManager.AdapterGuidToAdapterDictionary[guid];

                                    if (!adapter.IsEnabled)
                                        continue;

                                    await adapter.ActivateGroupAsync(group, context);
                                }

                                string details = string.Format("Built-in cmd {0} '{1}' processed.",
                                                                 command.Name,
                                                                 group.Name);

                                return new CommandProcessorResult(false, details);

                            }
                        case "GROUP_OFF":
                            {
                                int g_id = int.TryParse(argument, out g_id) ? g_id : 0;
                                Group group = await context.Groups.FirstOrDefaultAsync(o => o.Id == g_id);

                                if (group == null)
                                    return new CommandProcessorResult(true, string.Format("Device type cmd {0} failed.  Invalid group id.", command.Id));

                                //EXECUTE ON ALL API's
                                foreach (var guid in Core.AdapterManager.AdapterGuidToAdapterDictionary.Keys)
                                {
                                    var adapter = Core.AdapterManager.AdapterGuidToAdapterDictionary[guid];

                                    if (!adapter.IsEnabled)
                                        continue;

                                    await adapter.DeactivateGroupAsync(group, context);
                                }

                                string details = string.Format("Built-in cmd {0} '{1}' processed.",
                                                                 command.Name,
                                                                 group.Name);

                                return new CommandProcessorResult(false, details );

                            }
                        case "RUN_SCENE":
                            {
                                int id = 0;
                                int.TryParse(argument, out id);

                                SceneRunner sr = new SceneRunner(Core);
                                sr.onReportProgress += (s, a) =>
                                {
                                    Core.log.Info(a.Progress);
                                };
                                SceneRunner.SceneResult sceneResult = await sr.RunSceneAsync(id);

                                string details = string.Format("{0} Built-in cmd '{1}' processed.",
                                    sceneResult.Details,
                                    command.Name);

                                return new CommandProcessorResult(sceneResult.Errors, details);

                            }
                        default:
                            {
                                return new CommandProcessorResult(true, string.Format("Built-in cmd {0} failed. No logic defined for this built-in command.", command.Id));
                            }
                    }
                }
                #endregion

                #region JavaScriptCommand
                else if (command is JavaScriptCommand)
                {
                    JavaScriptCommand javaScriptCommand = (JavaScriptCommand)command;

                    JavaScriptExecuter je = new JavaScriptExecuter(sender, Core);
                    je.onReportProgress += (s, args) =>
                    {
                        Core.log.Info(args.Progress);
                    };

                    JavaScriptExecuter.JavaScriptResult jsResult = await je.ExecuteScriptAsync(javaScriptCommand.Script, context);

                    string details = string.Format("{0}. JavaScript cmd '{1}' processed.",
                                    jsResult.Details,
                                    command.Name);

                    return new CommandProcessorResult(false, details);
                }
                #endregion

                return result;
            }
        }
    }
}
