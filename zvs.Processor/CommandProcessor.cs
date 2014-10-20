using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using zvs.DataModel;
using System.Data.Entity;

namespace zvs.Processor
{
    public class CommandProcessor : ICommandProcessor
    {
        public IAdapterManager AdapterManager { get; private set; }

        public IFeedback<LogEntry> Log { get; private set; }
        //Constructor
        public CommandProcessor(IAdapterManager adapterManager, IFeedback<LogEntry> log)
        {
            if (adapterManager == null)
                throw new ArgumentNullException("adapterManager");

            if (log == null)
                throw new ArgumentNullException("log");

            AdapterManager = adapterManager;
            Log = log;
        }

        //public Methods 
        public async Task<Result> RunStoredCommandAsync(object sender, int storedCommandId, CancellationToken cancellationToken)
        {
            StoredCommand sCmd;
            using (var context = new ZvsContext())
            {
                sCmd = await context.StoredCommands
                    .Include(o => o.Command)
                    .SingleAsync(o => o.Id == storedCommandId, cancellationToken);
            }

            if (sCmd.Command == null)
                return Result.ReportError("Failed to process stored command. StoredCommand command is null.");

            return await RunCommandAsync(sender, sCmd.Command, sCmd.Argument, sCmd.Argument2, cancellationToken);
        }

        public async Task<Result> RunCommandAsync(object sender, Command command, string argument, string argument2, CancellationToken cancellationToken)
        {
            var result = await ProcessCommandAsync(sender, command, argument, argument2, cancellationToken);

            if (result.HasError)
                await Log.ReportErrorAsync(result.Message, cancellationToken);
            else
                await Log.ReportInfoAsync(result.Message, cancellationToken);
            return result;
        }

        //private Methods
        private async Task<Result> ProcessCommandAsync(object sender, Command command, string argument, string argument2, CancellationToken cancellationToken)
        {
            //var result = new Res(true, "Failed to process command. Command type unknown.");

            using (var context = new ZvsContext())
            {
                #region DeviceCommand
                if (command is DeviceCommand)
                {
                    var deviceCommand = await context.DeviceCommands
                        .Include(o => o.Device)
                        .Include(o => o.Device.Type)
                        .Include(o => o.Device.Type.Adapter)
                        .FirstOrDefaultAsync(o => o.Id == command.Id, cancellationToken);

                    var commandAction = string.Format("{0}{1} ({3}) on {2} ({4})",
                                                           deviceCommand.Name,
                                                           string.IsNullOrEmpty(argument) ? "" : " " + argument,
                                                           deviceCommand.Device.Name, deviceCommand.Id, deviceCommand.Device.Id);

                    var aGuid = deviceCommand.Device.Type.Adapter.AdapterGuid;
                    var adapter = AdapterManager.GetZvsAdapterByGuid(aGuid);
                    if (adapter== null)
                    {
                        return Result.ReportError(string.Format("{0} failed, device adapter is not loaded!",
                            commandAction));
                    }

                    var adapter = ZvsEngine.AdapterManager.AdapterGuidToAdapterDictionary[aGuid];
                    if (adapter.IsEnabled)
                    {
                        var details = string.Format("{0} complete", commandAction);

                        await adapter.ProcessDeviceCommandAsync(deviceCommand.Device, deviceCommand, argument, argument2);
                        return new CommandProcessorResult(false, details);
                    }
                    else
                    {
                        var err_str = string.Format("{0} failed because the '{1}' adapter is {2}",
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
                    if (!ZvsEngine.AdapterManager.AdapterGuidToAdapterDictionary.ContainsKey(aGuid))
                        return new CommandProcessorResult(true, string.Format("{0} failed.  Could not locate the associated adapter.", commandAction));

                    var adapter = ZvsEngine.AdapterManager.AdapterGuidToAdapterDictionary[aGuid];

                    if (adapter.IsEnabled)
                    {
                        var details = string.Format("{0} complete", commandAction);

                        await adapter.ProcessDeviceTypeCommandAsync(device.Type, device, command as DeviceTypeCommand, argument);
                        return new CommandProcessorResult(false, details);
                    }
                    else
                    {
                        var err_str = string.Format("{0} failed because the {1} adapter is {2}",
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
                                var delay = 0;
                                int.TryParse(argument, out delay);

                                await Task.Delay(delay * 1000);

                                var details = string.Format("{0} second time delay complete.", delay);

                                return new CommandProcessorResult(false, details);
                            }
                        case "REPOLL_ME":
                            {
                                var d_id = 0;
                                int.TryParse(argument, out d_id);

                                var device = await context.Devices
                                    .Include(o => o.Type.Adapter)
                                    .FirstOrDefaultAsync(o => o.Type.UniqueIdentifier != "BUILTIN" && o.Id == d_id);

                                if (!ZvsEngine.AdapterManager.AdapterGuidToAdapterDictionary.ContainsKey(device.Type.Adapter.AdapterGuid))
                                {
                                    return new CommandProcessorResult(true, string.Format("Re-poll of {0} failed, could not locate the associated adapter.", device.Name));
                                }
                                var adapter = ZvsEngine.AdapterManager.AdapterGuidToAdapterDictionary[device.Type.Adapter.AdapterGuid];

                                if (!adapter.IsEnabled)
                                    return new CommandProcessorResult(true, string.Format("Re-poll of {0} failed, adapter not enabled.", device.Name));

                                await adapter.RepollAsync(device, context);

                                var details = string.Format("Re-poll of {0} ({1}) complete", device.Name, device.Id);
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
                                    if (!ZvsEngine.AdapterManager.AdapterGuidToAdapterDictionary.ContainsKey(d.Type.Adapter.AdapterGuid))
                                    {
                                        ZvsEngine.log.WarnFormat("Re-poll all, could not locate the associated adapter for device {0}", d.Name);
                                        continue;
                                    }

                                    var adapter = ZvsEngine.AdapterManager.AdapterGuidToAdapterDictionary[d.Type.Adapter.AdapterGuid];

                                    if (!adapter.IsEnabled)
                                    {
                                        ZvsEngine.log.WarnFormat("Re-poll all, adapter for device {0} is disabled", d.Name);
                                        continue;
                                    }

                                    await adapter.RepollAsync(d, context);
                                }

                                var details = "Built-in cmd re-poll all devices complete";
                                return new CommandProcessorResult(false, details);

                            }
                        case "GROUP_ON":
                            {
                                int g_id = int.TryParse(argument, out g_id) ? g_id : 0;
                                var group = await context.Groups.FirstOrDefaultAsync(o => o.Id == g_id);

                                if (group == null)
                                    return new CommandProcessorResult(true, string.Format("Device type command Group On failed.  Invalid group id."));

                                //EXECUTE ON ALL API's
                                foreach (var guid in ZvsEngine.AdapterManager.AdapterGuidToAdapterDictionary.Keys)
                                {
                                    var adapter = ZvsEngine.AdapterManager.AdapterGuidToAdapterDictionary[guid];

                                    if (!adapter.IsEnabled)
                                        continue;

                                    await adapter.ActivateGroupAsync(group, context);
                                }

                                var details = string.Format("{0} ({2}) '{1}' complete",
                                                                 command.Name,
                                                                 group.Name, command.Id);

                                return new CommandProcessorResult(false, details);

                            }
                        case "GROUP_OFF":
                            {
                                int g_id = int.TryParse(argument, out g_id) ? g_id : 0;
                                var group = await context.Groups.FirstOrDefaultAsync(o => o.Id == g_id);

                                if (group == null)
                                    return new CommandProcessorResult(true, string.Format("Device type command Group Off failed. Invalid group id."));

                                //EXECUTE ON ALL API's
                                foreach (var guid in ZvsEngine.AdapterManager.AdapterGuidToAdapterDictionary.Keys)
                                {
                                    var adapter = ZvsEngine.AdapterManager.AdapterGuidToAdapterDictionary[guid];

                                    if (!adapter.IsEnabled)
                                        continue;

                                    await adapter.DeactivateGroupAsync(group, context);
                                }

                                var details = string.Format("{0} ({2}) '{1}' complete",
                                                                 command.Name,
                                                                 group.Name, command.Id);

                                return new CommandProcessorResult(false, details);

                            }
                        case "RUN_SCENE":
                            {
                                var id = 0;
                                int.TryParse(argument, out id);

                                var sr = new SceneRunner(ZvsEngine);
                                sr.onReportProgress += (s, a) =>
                                {
                                    ZvsEngine.log.Info(a.Progress);
                                };
                                var sceneResult = await sr.RunSceneAsync(id);

                                var details = string.Format("{0} Built-in cmd '{1}' ({2}) complete",
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
                    var javaScriptCommand = (JavaScriptCommand)command;

                    var je = new JavaScriptExecuter(sender, ZvsEngine);
                    je.onReportProgress += (s, args) =>
                    {
                        ZvsEngine.log.Info(args.Progress);
                    };

                    var jsResult = await je.ExecuteScriptAsync(javaScriptCommand.Script, context);

                    var details = string.Format("{0}. JavaScript cmd '{1}' processed.",
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
