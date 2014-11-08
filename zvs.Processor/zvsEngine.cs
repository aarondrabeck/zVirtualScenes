using System;
using System.Threading;
using System.Threading.Tasks;
using zvs.DataModel;
using System.Data.Entity;

namespace zvs.Processor
{
    public class ZvsEngine
    {
        private IAdapterManager AdapterManager { get; set; }
        private IEntityContextConnection EntityContextConnection { get; set; }
        //   public PluginManager PluginManager { get; private set; }
        private TriggerRunner TriggerRunner { get; set; }
        private ScheduledTaskRunner ScheduledTaskRunner { get; set; }
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
            //NotifyEntityChangeContext.ChangeNotifications<Device>.OnEntityUpdated += Core_onEntityUpdated;
            //NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.OnEntityUpdated += Core_onEntityUpdated;
            //NotifyEntityChangeContext.ChangeNotifications<Group>.OnEntityUpdated += Core_onEntityUpdated;
            //NotifyEntityChangeContext.ChangeNotifications<Scene>.OnEntityUpdated += Core_onEntityUpdated;
            //NotifyEntityChangeContext.ChangeNotifications<JavaScriptCommand>.OnEntityUpdated += Core_onEntityUpdated;
        }

        //static void Core_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<JavaScriptCommand>.EntityUpdatedArgs e)
        //{
        //    if (e.NewEntity.Name != e.OldEntity.Name)
        //    {
        //        Task.Run(async () =>
        //        {
        //            using (var context = new ZvsContext())
        //            {
        //                var storedCommands = await context.StoredCommands
        //                       .Include(o => o.Command)
        //                       .Where(o => o.CommandId == e.NewEntity.Id)
        //                       .ToListAsync();

        //                foreach (var storedCommand in storedCommands)
        //                    await storedCommand.SetTargetObjectNameAsync(context);

        //                await context.TrySaveChangesAsync();
        //            }
        //        });
        //    }
        //}

        #region Keep trigger descriptions and stored command actionableObjects in sync.

        //TODO: Implement

        //static void Core_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<Scene>.EntityUpdatedArgs e)
        //{
        //    if (e.NewEntity.Name != e.OldEntity.Name)
        //    {
        //        Task.Run(async () =>
        //        {
        //            using (var context = new ZvsContext())
        //            {
        //                var changedObjId = e.NewEntity.Id.ToString();
        //                var storedCommands = new List<StoredCommand>();

        //                //Scene Commands
        //                var id = await context.BuiltinCommands
        //                .Where(o => o.UniqueIdentifier == "RUN_SCENE")
        //                .Select(o => o.Id)
        //                .FirstOrDefaultAsync();

        //                storedCommands.AddRange(await context.StoredCommands
        //                       .Include(o => o.Command)
        //                       .Where(o => id == o.CommandId && o.Argument == changedObjId)
        //                       .ToListAsync());

        //                foreach (var storedCommand in storedCommands)
        //                    await storedCommand.SetTargetObjectNameAsync(context);

        //                await context.TrySaveChangesAsync();

        //            }
        //        });
        //    }
        //}

        //static void Core_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<Group>.EntityUpdatedArgs e)
        //{
        //    if (e.NewEntity.Name != e.OldEntity.Name)
        //    {
        //        Task.Run(async () =>
        //        {
        //            using (var context = new ZvsContext())
        //            {
        //                var changeingGroupIDstr = e.NewEntity.Id.ToString();
        //                var storedCommands = new List<StoredCommand>();

        //                //Group Commands
        //                var groupComandIds = await context.BuiltinCommands
        //                .Where(o => o.UniqueIdentifier == "GROUP_ON" || o.UniqueIdentifier == "GROUP_OFF")
        //                .Select(o => o.Id)
        //                .ToListAsync();

        //                storedCommands.AddRange(await context.StoredCommands
        //                       .Include(o => o.Command)
        //                       .Where(o => groupComandIds.Contains(o.CommandId) && o.Argument == changeingGroupIDstr)
        //                       .ToListAsync());

        //                foreach (var storedCommand in storedCommands)
        //                    await storedCommand.SetTargetObjectNameAsync(context);

        //                await context.TrySaveChangesAsync();

        //            }
        //        });
        //    }
        //}

        //static async void Core_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.EntityUpdatedArgs e)
        //{
        //    if (e.NewEntity.Name != e.OldEntity.Name)
        //    {
        //        await Task.Run(async () =>
        //        {
        //            using (var context = new ZvsContext())
        //            {
        //                var triggers = await context.DeviceValueTriggers
        //                    .Include(o => o.DeviceValue)
        //                    .Include(o => o.DeviceValue.Device)
        //                    .Include(o => o.StoredCommand)
        //                    .Where(o => o.DeviceValue.DeviceId == e.NewEntity.Id)
        //                    .ToListAsync();

        //                foreach (var trigger in triggers)
        //                    trigger.SetDescription();

        //                await context.TrySaveChangesAsync();
        //            }
        //        });
        //    }
        //}

        //static void Core_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<Device>.EntityUpdatedArgs e)
        //{
        //    if (e.NewEntity.Name != e.OldEntity.Name)
        //    {
        //        Task.Run(async () =>
        //        {
        //            var sw = new Stopwatch();
        //            sw.Start();

        //            using (var context = new ZvsContext())
        //            {
        //                var triggers = await context.DeviceValueTriggers
        //                    .Include(o => o.DeviceValue)
        //                    .Include(o => o.DeviceValue.Device)
        //                    .Include(o => o.StoredCommand)
        //                    .Where(o => o.DeviceValue.DeviceId == e.NewEntity.Id)
        //                    .ToListAsync();

        //                foreach (var trigger in triggers)
        //                    trigger.SetDescription();

        //                var deviceIdStr = e.NewEntity.Id.ToString(CultureInfo.InvariantCulture);
        //                var storedCommands = new List<StoredCommand>();

        //                //re-poll Device Commands
        //                var repollCommandId = await context.BuiltinCommands
        //                .Where(o => o.UniqueIdentifier == "REPOLL_ME")
        //                .Select(o => o.Id)
        //                .FirstOrDefaultAsync();

        //                storedCommands.AddRange(await context.StoredCommands
        //                      .Include(o => o.Command)
        //                      .Where(o => repollCommandId == o.CommandId && o.Argument == deviceIdStr)
        //                      .ToListAsync());

        //                //Device Type Commands                    
        //                storedCommands.AddRange(await context.StoredCommands
        //                    .Where(o => o.Argument2 == deviceIdStr)
        //                    .Where(o => o.Command is DeviceTypeCommand)
        //                    .ToListAsync());

        //                //Device Commands
        //                var deviceCommandIds = await context.DeviceCommands
        //                    .Where(o => o.DeviceId == e.NewEntity.Id)
        //                    .Select(o => o.Id)
        //                    .ToListAsync();

        //                storedCommands.AddRange(await context.StoredCommands
        //                        .Include(o => o.Command)
        //                        .Where(o => deviceCommandIds.Contains(o.CommandId))
        //                        .ToListAsync());

        //                foreach (var storedCommand in storedCommands)
        //                    await storedCommand.SetTargetObjectNameAsync(context);

        //                Debug.WriteLine("presavechanges " + sw.Elapsed.ToString());

        //                await context.TrySaveChangesAsync();

        //                sw.Stop();
        //                Debug.WriteLine("Updating device names in storedcommands and triggers took " + sw.Elapsed.ToString());

        //            }
        //        });
        //    }
        //}
        #endregion

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
