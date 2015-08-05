using System;
using System.Threading;
using System.Threading.Tasks;
using zvs.DataModel;

namespace zvs.Processor
{
    public class ZvsEngine
    {
        public IAdapterManager AdapterManager { get; }
        private IEntityContextConnection EntityContextConnection { get; }
        public IPluginManager PluginManager { get; }
        public TriggerRunner TriggerRunner { get; }
        public ScheduledTaskRunner ScheduledTaskRunner { get; }
        private IFeedback<LogEntry> Log { get; }

        public ZvsEngine(IFeedback<LogEntry> feedback, IAdapterManager adapterManager, IPluginManager pluginManager,
            IEntityContextConnection entityContextConnection, TriggerRunner triggerRunner, ScheduledTaskRunner scheduledTaskRunner)
        {
            if (entityContextConnection == null)
                throw new ArgumentNullException(nameof(entityContextConnection));

            if (feedback == null)
                throw new ArgumentNullException(nameof(feedback));

            if (adapterManager == null)
                throw new ArgumentNullException(nameof(adapterManager));

            if (pluginManager == null)
                throw new ArgumentNullException(nameof(pluginManager));

            if (triggerRunner == null)
                throw new ArgumentNullException(nameof(triggerRunner));

            if (scheduledTaskRunner == null)
                throw new ArgumentNullException(nameof(scheduledTaskRunner));

            EntityContextConnection = entityContextConnection;
            Log = feedback;
            AdapterManager = adapterManager;
            PluginManager = pluginManager;
            TriggerRunner = triggerRunner;
            ScheduledTaskRunner = scheduledTaskRunner;
            Log.Source = "Zvs Engine";


            AppDomain.CurrentDomain.SetData("DataDirectory", Utils.AppDataPath);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Log.ReportInfoFormatAsync(cancellationToken, "Starting zvsEngine {0}", Utils.ApplicationName);

            #region Install Base Commands and Properties
            var builtinCommandBuilder = new BuiltinCommandBuilder(EntityContextConnection);

            var repollMeResult = await builtinCommandBuilder.RegisterAsync(new BuiltinCommand
            {
                UniqueIdentifier = "REPOLL_ME",
                Name = "Re-poll Device",
                ArgumentType = DataType.INTEGER,
                Description = "This will force a re-poll on an object."
            }, cancellationToken);
            if (repollMeResult.HasError)
                await Log.ReportResultAsync(repollMeResult, cancellationToken);

            var repollAllResult = await builtinCommandBuilder.RegisterAsync(new BuiltinCommand
            {
                UniqueIdentifier = "REPOLL_ALL",
                Name = "Re-poll all Devices",
                ArgumentType = DataType.NONE,
                Description = "This will force a re-poll on all objects."
            }, cancellationToken);
            if (repollAllResult.HasError)
                await Log.ReportResultAsync(repollAllResult, cancellationToken);

            var groupOnResult = await builtinCommandBuilder.RegisterAsync(new BuiltinCommand
            {
                UniqueIdentifier = "GROUP_ON",
                Name = "Turn Group On",
                ArgumentType = DataType.STRING,
                Description = "Activates a group."
            }, cancellationToken);
            if (groupOnResult.HasError)
                await Log.ReportResultAsync(groupOnResult, cancellationToken);

            var groupOffResult = await builtinCommandBuilder.RegisterAsync(new BuiltinCommand
            {
                UniqueIdentifier = "GROUP_OFF",
                Name = "Turn Group Off",
                ArgumentType = DataType.STRING,
                Description = "Deactivates a group."
            }, cancellationToken);
            if (groupOffResult.HasError)
                await Log.ReportResultAsync(groupOffResult, cancellationToken);

            var timeDelayResult = await builtinCommandBuilder.RegisterAsync(new BuiltinCommand
            {
                UniqueIdentifier = "TIMEDELAY",
                Name = "Time Delay (sec)",
                ArgumentType = DataType.INTEGER,
                Description = "Pauses a execution for x seconds."
            }, cancellationToken);
            if (timeDelayResult.HasError)
                await Log.ReportResultAsync(timeDelayResult, cancellationToken);

            var runSceneResult = await builtinCommandBuilder.RegisterAsync(new BuiltinCommand
             {
                 UniqueIdentifier = "RUN_SCENE",
                 Name = "Run Scene",
                 ArgumentType = DataType.INTEGER,
                 Description = "Argument = SceneId"
             }, cancellationToken);
            if (runSceneResult.HasError)
                await Log.ReportResultAsync(runSceneResult, cancellationToken);
            #endregion

            await AdapterManager.StartAsync(cancellationToken);
            await PluginManager.StartAsync(cancellationToken);
            await ScheduledTaskRunner.StartAsync(cancellationToken);
            await TriggerRunner.StartAsync(cancellationToken);

        }

        public async Task<Result> RunCommandAsync(int? commandId, string argument, string argument2,
            CancellationToken cancellationToken)
        {
            var commandProcessor = new CommandProcessor(AdapterManager, EntityContextConnection, Log);
            return await commandProcessor.RunCommandAsync(commandId, argument, argument2, cancellationToken);
        }
    }
}
