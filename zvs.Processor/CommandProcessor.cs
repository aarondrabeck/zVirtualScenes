using System;
using System.Globalization;
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
        private IEntityContextConnection EntityContextConnection { get; set; }
        private IFeedback<LogEntry> Log { get; set; }

        //Constructor
        public CommandProcessor(IAdapterManager adapterManager, IEntityContextConnection entityContextConnection, IFeedback<LogEntry> log)
        {
            if (entityContextConnection == null)
                throw new ArgumentNullException("entityContextConnection");

            if (adapterManager == null)
                throw new ArgumentNullException("adapterManager");

            if (log == null)
                throw new ArgumentNullException("log");

            AdapterManager = adapterManager;
            EntityContextConnection = entityContextConnection;
            Log = log;
        }

        internal async Task<Result> ExecuteDeviceCommandAsync(DeviceCommand command, string argument, string argument2, CancellationToken cancellationToken)
        {
            using (var context = new ZvsContext(EntityContextConnection))
            {
                var deviceCommand = await context.DeviceCommands
                    .Include(o => o.Device)
                    .Include(o => o.Device.Type)
                    .Include(o => o.Device.Type.Adapter)
                    .FirstOrDefaultAsync(o => o.Id == command.Id, cancellationToken);

                if (deviceCommand == null)
                    return Result.ReportErrorFormat("Cannot locate device command with id of {0}", command.Id);

                var commandAction = string.Format("{0}{1} ({3}) on {2} ({4})",
                    deviceCommand.Name,
                    string.IsNullOrEmpty(argument) ? "" : " " + argument,
                    deviceCommand.Device.Name, deviceCommand.Id, deviceCommand.Device.Id);

                var aGuid = deviceCommand.Device.Type.Adapter.AdapterGuid;
                var adapter = AdapterManager.FindZvsAdapter(aGuid);
                if (adapter == null)
                {
                    return Result.ReportErrorFormat("{0} failed, device adapter is not loaded!",
                        commandAction);
                }

                if (!adapter.IsEnabled)
                    return Result.ReportErrorFormat("{0} failed because the '{1}' adapter is disabled",
                        commandAction,
                        deviceCommand.Device.Type.Adapter.Name);

                await adapter.ProcessDeviceCommandAsync(deviceCommand.Device, deviceCommand, argument, argument2);
                return Result.ReportSuccessFormat("{0} complete", commandAction);
            }
        }

        internal async Task<Result> ExecuteDeviceTypeCommandAsync(DeviceTypeCommand command, string argument, string argument2, CancellationToken cancellationToken)
        {
            using (var context = new ZvsContext(EntityContextConnection))
            {
                int dId = int.TryParse(argument2, out dId) ? dId : 0;

                var device = await context.Devices
                    .Include(o => o.Type)
                    .Include(o => o.Type.Adapter)
                    .FirstOrDefaultAsync(o => o.Id == dId, cancellationToken);

                if (device == null)
                    return Result.ReportErrorFormat("Cannot find device with id of {0}", dId);

                var commandAction = string.Format("{0}{1} {2}",
                                                       command.Name,
                                                       string.IsNullOrEmpty(argument) ? "" : " " + argument,
                                                       device.Name);

                var aGuid = device.Type.Adapter.AdapterGuid;
                var adapter = AdapterManager.FindZvsAdapter(aGuid);
                if (adapter == null)
                {
                    return Result.ReportErrorFormat("{0} failed, device adapter is not loaded!",
                        commandAction);
                }

                if (!adapter.IsEnabled)
                    return Result.ReportErrorFormat("{0} failed because the {1} adapter is {2}",
                        commandAction,
                        device.Type.Adapter.Name,
                        adapter.IsEnabled ? "not ready" : "disabled");

                await adapter.ProcessDeviceTypeCommandAsync(device.Type, device, command, argument);
                return Result.ReportSuccessFormat("{0} complete", commandAction);
            }
        }

        internal async Task<Result> ExecuteBuiltinCommandAsync(BuiltinCommand command, string argument, string argument2, CancellationToken cancellationToken)
        {
            using (var context = new ZvsContext(EntityContextConnection))
            {
                switch (command.UniqueIdentifier)
                {
                    case "TIMEDELAY":
                        {
                            int delay;
                            int.TryParse(argument, out delay);
                            await Task.Delay(delay * 1000, cancellationToken);
                            return Result.ReportSuccessFormat("{0} second time delay complete", delay);
                        }
                    case "REPOLL_ME":
                        {
                            int dId;
                            int.TryParse(argument, out dId);

                            var device = await context.Devices
                                .Include(o => o.Type.Adapter)
                                .FirstOrDefaultAsync(o => o.Type.UniqueIdentifier != "BUILTIN" && o.Id == dId,
                                    cancellationToken);

                            if (device == null)
                                return Result.ReportErrorFormat("Cannot find device with id of {0}", dId);

                            var adapter = AdapterManager.FindZvsAdapter(device.Type.Adapter.AdapterGuid);
                            if (adapter == null)
                                return
                                    Result.ReportErrorFormat(
                                        "Re-poll of {0} failed, the associated adapter is not loaded", device.Name);

                            if (!adapter.IsEnabled)
                                return Result.ReportErrorFormat("Re-poll of {0} failed, adapter is disabled", device.Name);

                            await adapter.RepollAsync(device);
                            return Result.ReportSuccessFormat("Re-poll of {0} ({1}) complete", device.Name, device.Id);
                        }
                    case "REPOLL_ALL":
                        {
                            var devices = await context.Devices
                                .Include(o => o.Type.Adapter)
                                .Where(o => o.Type.UniqueIdentifier != "BUILTIN")
                                .ToListAsync(cancellationToken);

                            foreach (var device in devices)
                            {
                                await
                                    ExecuteBuiltinCommandAsync(new BuiltinCommand { UniqueIdentifier = "REPOLL_ME" },
                                        device.Id.ToString(CultureInfo.InvariantCulture), "", cancellationToken);
                            }

                            return Result.ReportSuccessFormat("Built-in cmd re-poll {0} devices complete", devices.Count);

                        }
                    case "GROUP_ON":
                    case "GROUP_OFF":
                        {
                            int gId = int.TryParse(argument, out gId) ? gId : 0;
                            var group = await context.Groups
                                .Include(o=> o.Devices)
                                .FirstOrDefaultAsync(o => o.Id == gId, cancellationToken);

                            if (group == null)
                                return Result.ReportErrorFormat("Command {0} failed. Invalid group id", command.Name);

                            if (group.Devices.Count < 1)
                                return Result.ReportErrorFormat("No devices found in the {0} group", command.Name);

                            var adapterGuids = await context.Devices
                                .Where(o => o.Groups.Any(g => g.Id == gId))
                                .Select(o => o.Type.Adapter.AdapterGuid)
                                .Distinct()
                                .ToListAsync(cancellationToken);

                            //EXECUTE ON ALL Adapters
                            foreach (var adapter in adapterGuids.Select(adapterGuid => AdapterManager.FindZvsAdapter(adapterGuid)).Where(adapter => adapter != null && adapter.IsEnabled))
                            {
                                if (command.UniqueIdentifier == "GROUP_ON")
                                    await adapter.ActivateGroupAsync(@group);
                                else
                                    await adapter.DeactivateGroupAsync(@group);
                            }

                            return Result.ReportSuccessFormat("{0} {2}, {1} complete",
                                command.Name,
                                group.Name, command.Id);

                        }
                    case "RUN_SCENE":
                        {
                            int id;
                            int.TryParse(argument, out id);

                            var sceneRunner = new SceneRunner(Log, this, EntityContextConnection);
                            var sceneResult = await sceneRunner.RunSceneAsync(id, cancellationToken);

                            var details = string.Format("{0} Built-in cmd '{1}' ({2}) complete",
                                sceneResult.Message,
                                command.Name, command.Id);

                            return sceneResult.HasError ? Result.ReportError(details) : Result.ReportSuccess(details);
                        }
                    default:
                        {
                            return
                                Result.ReportErrorFormat(
                                    "Built-in cmd {0} failed. No logic defined for this built-in command.", command.Id);
                        }
                }

            }
        }

        internal async Task<Result> ExecuteJavaScriptCommandAsync(JavaScriptCommand command,
            string argument, string argument2, CancellationToken cancellationToken)
        {
            //var javaScriptCommand = (JavaScriptCommand)command;

            //var je = new JavaScriptExecuter(sender, ZvsEngine);
            //je.onReportProgress += (s, args) =>
            //{
            //    ZvsEngine.log.Info(args.Progress);
            //};

            //var jsResult = await je.ExecuteScriptAsync(javaScriptCommand.Script, context);

            //var details = string.Format("{0}. JavaScript cmd '{1}' processed.",
            //                jsResult.Details,
            //                command.Name);

            //return new CommandProcessorResult(false, details);
            return Result.ReportError("Not Implemented");
        }

        //private Methods
        public async Task<Result> RunCommandAsync(int? commandId, string argument, string argument2, CancellationToken cancellationToken)
        {
            using (var context = new ZvsContext(EntityContextConnection))
            {
                if (!commandId.HasValue)
                    return Result.ReportError("No command to run");

                var command = await context.Commands.FirstOrDefaultAsync(o => o.Id == commandId.Value, cancellationToken);

                if (command == null)
                    return Result.ReportErrorFormat("Command with id of {0} not found", commandId);

                var result = Result.ReportError("Unknown Command Type");
                if (command is DeviceCommand)
                {
                    result = await ExecuteDeviceCommandAsync(command as DeviceCommand, argument, argument2, cancellationToken);
                }
                else if (command is DeviceTypeCommand)
                {
                    result = await ExecuteDeviceTypeCommandAsync(command as DeviceTypeCommand, argument, argument2, cancellationToken);
                }
                else if (command is BuiltinCommand)
                {
                    result = await ExecuteBuiltinCommandAsync(command as BuiltinCommand, argument, argument2, cancellationToken);
                }
                else if (command is JavaScriptCommand)
                {
                    result = await ExecuteJavaScriptCommandAsync(command as JavaScriptCommand, argument, argument2, cancellationToken);
                }

                return result;
            }
        }
    }
}
