using System;
using System.Threading;
using System.Threading.Tasks;
using zvs.DataModel;
using System.Data.Entity;

namespace zvs.Processor
{
    public class ZvsEngine
    {
        public IAdapterManager AdapterManager { get; private set; }
        private IEntityContextConnection EntityContextConnection { get; set; }
        //   public PluginManager PluginManager { get; private set; }
        public TriggerRunner TriggerRunner { get; private set; }
        public ScheduledTaskRunner ScheduledTaskRunner { get; private set; }
        private IFeedback<LogEntry> Log { get; set; }

        public ZvsEngine(IFeedback<LogEntry> feedback, IAdapterManager adapterManager, IEntityContextConnection entityContextConnection, TriggerRunner triggerRunner, ScheduledTaskRunner scheduledTaskRunner)//, PluginManager pluginManager, TriggerManager triggerManager, ScheduledTaskManager scheduledTaskManager)
        {
            EntityContextConnection = entityContextConnection;
            Log = feedback;
            AdapterManager = adapterManager;
            TriggerRunner = triggerRunner;
            ScheduledTaskRunner = scheduledTaskRunner;

            Log.Source = "Zvs Engine";
            //    PluginManager = pluginManager;

            AppDomain.CurrentDomain.SetData("DataDirectory", Utils.AppDataPath);
        }

        public async void StartAsync(CancellationToken cancellationToken)
        {
            await Log.ReportInfoFormatAsync(cancellationToken, "Starting zvsEngine {0}", Utils.ApplicationName);

            using (var context = new ZvsContext(EntityContextConnection))
            {
                #region Install Base Commands and Properties
                var builtinCommandBuilder = new BuiltinCommandBuilder(EntityContextConnection);

                await builtinCommandBuilder.RegisterAsync(new BuiltinCommand
                {
                    UniqueIdentifier = "REPOLL_ME",
                    Name = "Re-poll Device",
                    ArgumentType = DataType.INTEGER,
                    Description = "This will force a re-poll on an object."
                }, cancellationToken);

                await builtinCommandBuilder.RegisterAsync(new BuiltinCommand
                {
                    UniqueIdentifier = "REPOLL_ALL",
                    Name = "Re-poll all Devices",
                    ArgumentType = DataType.NONE,
                    Description = "This will force a re-poll on all objects."
                }, cancellationToken);

                await builtinCommandBuilder.RegisterAsync(new BuiltinCommand
                {
                    UniqueIdentifier = "GROUP_ON",
                    Name = "Turn Group On",
                    ArgumentType = DataType.STRING,
                    Description = "Activates a group."
                }, cancellationToken);

                await builtinCommandBuilder.RegisterAsync(new BuiltinCommand
                {
                    UniqueIdentifier = "GROUP_OFF",
                    Name = "Turn Group Off",
                    ArgumentType = DataType.STRING,
                    Description = "Deactivates a group."
                }, cancellationToken);

                await builtinCommandBuilder.RegisterAsync(new BuiltinCommand
                {
                    UniqueIdentifier = "TIMEDELAY",
                    Name = "Time Delay (sec)",
                    ArgumentType = DataType.INTEGER,
                    Description = "Pauses a execution for x seconds."
                }, cancellationToken);

                await builtinCommandBuilder.RegisterAsync(new BuiltinCommand
                {
                    UniqueIdentifier = "RUN_SCENE",
                    Name = "Run Scene",
                    ArgumentType = DataType.INTEGER,
                    Description = "Argument = SceneId"
                }, cancellationToken);
                #endregion

                await AdapterManager.InitializeAdaptersAsync(cancellationToken);
                //await PluginManager.LoadPluginsAsync(cancellationToken);
                await ScheduledTaskRunner.StartAsync(cancellationToken);
                await TriggerRunner.StartAsync(cancellationToken);

                //Install Program Options
                var option =
                    await
                        context.ProgramOptions.FirstOrDefaultAsync(o => o.UniqueIdentifier == "LOGDIRECTION",
                            cancellationToken);
                if (option != null) return;

                var result = await ProgramOption.TryAddOrEditAsync(context, new ProgramOption
                {
                    UniqueIdentifier = "LOGDIRECTION",
                    Value = "Descending"
                }, cancellationToken);

                if (result.HasError)
                    await Log.ReportErrorAsync(result.Message, cancellationToken);
            }

        }

        public async Task<Result> RunCommandAsync(int? commandId, string argument, string argument2,
            CancellationToken cancellationToken)
        {
            var commandProcessor = new CommandProcessor(AdapterManager, EntityContextConnection, Log);
            return await commandProcessor.RunCommandAsync(commandId, argument, argument, cancellationToken);
        }
    }
}
