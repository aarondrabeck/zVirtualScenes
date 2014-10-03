using System;
using System.Linq;
using System.Threading.Tasks;
using zvs.Entities;
using System.Data.Entity;

namespace zvs.Processor
{
    public class CommandProcessorResult : EventArgs
    {
        public bool HasErrors { get; private set; }
        public string Message { get; private set; }

        public CommandProcessorResult(bool hasErrors, string message)
        {
            this.HasErrors = hasErrors;
            this.Message = message;
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

            return await RunCommandAsync(sender, sCmd.Command, sCmd.Argument, sCmd.Argument2);
        }

        public async Task<CommandProcessorResult> RunCommandAsync(object sender, Command command, string argument = "", string argument2 = "")
        {
            var result = await ProcessCommandAsync(sender, command, argument, argument2);

            if (result.HasErrors)
                Core.log.Error(result.Message);
            else
                Core.log.Info(result.Message);
            return result;
        }

        //private Methods
        private async Task<CommandProcessorResult> ProcessCommandAsync(object sender, Command command, string argument = "", string argument2 = "")
        {
            CommandProcessorResult result = new CommandProcessorResult(true, "Failed to process command. Command type unknown.");

            using (zvsContext context = new zvsContext())
            {
                #region DeviceCommand
                if (command is DeviceCommand)
                {
                    var deviceCommand = await context.DeviceCommands
                        .Include(o => o.Device)
                        .Include(o => o.Device.Type)
                        .Include(o => o.Device.Type.Adapter)
                        .FirstOrDefaultAsync(o => o.Id == command.Id);

                    var commandAction = string.Format("{0}{1} ({3}) on {2} ({4})",
                                                           deviceCommand.Name,
                                                           string.IsNullOrEmpty(argument) ? "" : " " + argument,
                                                           deviceCommand.Device.Name, deviceCommand.Id, deviceCommand.Device.Id);

                    var aGuid = deviceCommand.Device.Type.Adapter.AdapterGuid;
                    if (!Core.AdapterManager.AdapterGuidToAdapterDictionary.ContainsKey(aGuid))
                    {
                        return new CommandProcessorResult(true, string.Format("{0} failed, device adapter is not loaded!",
                            commandAction));
                    }

                    var adapter = Core.AdapterManager.AdapterGuidToAdapterDictionary[aGuid];
                    if (adapter.IsEnabled)
                    {
                        string details = string.Format("{0} complete", commandAction);

                        await adapter.ProcessDeviceCommandAsync(deviceCommand.Device, deviceCommand, argument, argument2);
                        return new CommandProcessorResult(false, details);
                    }
                    else
                    {
                        string err_str = string.Format("{0} failed because the '{1}' adapter is {2}",
                         commandAction,
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
                        return new CommandProcessorResult(true, string.Format("{0}{1} failed. Invalid device id.",
                            command.Name,
                            string.IsNullOrEmpty(argument) ? "" : " " + argument));

                    var commandAction = string.Format("{0}{1} {2}",
                                                           command.Name,
                                                           string.IsNullOrEmpty(argument) ? "" : " " + argument,
                                                           device.Name);

                    var aGuid = device.Type.Adapter.AdapterGuid;
                    if (!Core.AdapterManager.AdapterGuidToAdapterDictionary.ContainsKey(aGuid))
                        return new CommandProcessorResult(true, string.Format("{0} failed.  Could not locate the associated adapter.", commandAction));

                    var adapter = Core.AdapterManager.AdapterGuidToAdapterDictionary[aGuid];

                    if (adapter.IsEnabled)
                    {
                        string details = string.Format("{0} complete", commandAction);

                        await adapter.ProcessDeviceTypeCommandAsync(device.Type, device, command as DeviceTypeCommand, argument);
                        return new CommandProcessorResult(false, details);
                    }
                    else
                    {
                        string err_str = string.Format("{0} failed because the {1} adapter is {2}",
                        commandAction,
                        device.Type.Adapter.Name,
                        adapter.IsEnabled ? "not ready" : "disabled");

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

                                string details = string.Format("{0} second time delay complete.", delay);

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
                                    return new CommandProcessorResult(true, string.Format("Re-poll of {0} failed, could not locate the associated adapter.", device.Name));
                                }
                                var adapter = Core.AdapterManager.AdapterGuidToAdapterDictionary[device.Type.Adapter.AdapterGuid];

                                if (!adapter.IsEnabled)
                                    return new CommandProcessorResult(true, string.Format("Re-poll of {0} failed, adapter not enabled.", device.Name));

                                await adapter.RepollAsync(device, context);

                                string details = string.Format("Re-poll of {0} ({1}) complete", device.Name, device.Id);
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

                                string details = "Built-in cmd re-poll all devices complete";
                                return new CommandProcessorResult(false, details);

                            }
                        case "GROUP_ON":
                            {
                                int g_id = int.TryParse(argument, out g_id) ? g_id : 0;
                                Group group = await context.Groups.FirstOrDefaultAsync(o => o.Id == g_id);

                                if (group == null)
                                    return new CommandProcessorResult(true, string.Format("Device type command Group On failed.  Invalid group id."));

                                //EXECUTE ON ALL API's
                                foreach (var guid in Core.AdapterManager.AdapterGuidToAdapterDictionary.Keys)
                                {
                                    var adapter = Core.AdapterManager.AdapterGuidToAdapterDictionary[guid];

                                    if (!adapter.IsEnabled)
                                        continue;

                                    await adapter.ActivateGroupAsync(group, context);
                                }

                                string details = string.Format("{0} ({2}) '{1}' complete",
                                                                 command.Name,
                                                                 group.Name, command.Id);

                                return new CommandProcessorResult(false, details);

                            }
                        case "GROUP_OFF":
                            {
                                int g_id = int.TryParse(argument, out g_id) ? g_id : 0;
                                Group group = await context.Groups.FirstOrDefaultAsync(o => o.Id == g_id);

                                if (group == null)
                                    return new CommandProcessorResult(true, string.Format("Device type command Group Off failed. Invalid group id."));

                                //EXECUTE ON ALL API's
                                foreach (var guid in Core.AdapterManager.AdapterGuidToAdapterDictionary.Keys)
                                {
                                    var adapter = Core.AdapterManager.AdapterGuidToAdapterDictionary[guid];

                                    if (!adapter.IsEnabled)
                                        continue;

                                    await adapter.DeactivateGroupAsync(group, context);
                                }

                                string details = string.Format("{0} ({2}) '{1}' complete",
                                                                 command.Name,
                                                                 group.Name, command.Id);

                                return new CommandProcessorResult(false, details);

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

                                string details = string.Format("{0} Built-in cmd '{1}' ({2}) complete",
                                    sceneResult.Details,
                                    command.Name, command.Id);

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
