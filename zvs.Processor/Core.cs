using System;
using System.Collections.Generic;
using System.Linq;
using zvs.Processor.Triggers;
using zvs.Entities;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Diagnostics;

namespace zvs.Processor
{
    public class Core
    {
        public AdapterManager AdapterManager;
        public PluginManager PluginManager;
        private TriggerManager TriggerManager;
        private ScheduledTaskManager ScheduledTaskManager;
        public Logging.ILog log;

        public Core()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", Utils.AppDataPath);
            AdapterManager = new Processor.AdapterManager();
            PluginManager = new Processor.PluginManager();

            //Create a instance of the logger
            log = Logging.LogManager.GetLogger<Core>();

            NotifyEntityChangeContext.ChangeNotifications<Device>.OnEntityUpdated += Core_onEntityUpdated;
            NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.OnEntityUpdated += Core_onEntityUpdated;
            NotifyEntityChangeContext.ChangeNotifications<Group>.OnEntityUpdated += Core_onEntityUpdated;
            NotifyEntityChangeContext.ChangeNotifications<Scene>.OnEntityUpdated += Core_onEntityUpdated;
            NotifyEntityChangeContext.ChangeNotifications<JavaScriptCommand>.OnEntityUpdated += Core_onEntityUpdated;
        }

        void Core_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<JavaScriptCommand>.EntityUpdatedArgs e)
        {
            if (e.NewEntity.Name != e.OldEntity.Name)
            {
                Task.Run(async () =>
                {
                    using (var context = new zvsContext())
                    {
                        var storedCommands = await context.StoredCommands
                               .Include(o => o.Command)
                               .Where(o => o.CommandId == e.NewEntity.Id)
                               .ToListAsync();

                        foreach (var storedCommand in storedCommands)
                            await storedCommand.SetTargetObjectNameAsync(context);

                        await context.TrySaveChangesAsync();

                    }
                });
            }
        }

        #region Keep trigger descriptions and stored command actionableObjects in sync.

        void Core_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<Scene>.EntityUpdatedArgs e)
        {
            if (e.NewEntity.Name != e.OldEntity.Name)
            {
                Task.Run(async () =>
                {
                    using (var context = new zvsContext())
                    {
                        var ChangedObjId = e.NewEntity.Id.ToString();
                        var storedCommands = new List<StoredCommand>();

                        //Scene Commands
                        var Id = await context.BuiltinCommands
                        .Where(o => o.UniqueIdentifier == "RUN_SCENE")
                        .Select(o => o.Id)
                        .FirstOrDefaultAsync();

                        storedCommands.AddRange(await context.StoredCommands
                               .Include(o => o.Command)
                               .Where(o => Id == o.CommandId && o.Argument == ChangedObjId)
                               .ToListAsync());

                        foreach (var storedCommand in storedCommands)
                            await storedCommand.SetTargetObjectNameAsync(context);

                        await context.TrySaveChangesAsync();

                    }
                });
            }
        }
        void Core_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<Group>.EntityUpdatedArgs e)
        {
            if (e.NewEntity.Name != e.OldEntity.Name)
            {
                Task.Run(async () =>
                {
                    using (var context = new zvsContext())
                    {
                        var changeingGroupIDstr = e.NewEntity.Id.ToString();
                        var storedCommands = new List<StoredCommand>();

                        //Group Commands
                        var GroupComandIds = await context.BuiltinCommands
                        .Where(o => o.UniqueIdentifier == "GROUP_ON" || o.UniqueIdentifier == "GROUP_OFF")
                        .Select(o => o.Id)
                        .ToListAsync();

                        storedCommands.AddRange(await context.StoredCommands
                               .Include(o => o.Command)
                               .Where(o => GroupComandIds.Contains(o.CommandId) && o.Argument == changeingGroupIDstr)
                               .ToListAsync());

                        foreach (var storedCommand in storedCommands)
                            await storedCommand.SetTargetObjectNameAsync(context);

                        await context.TrySaveChangesAsync();

                    }
                });
            }
        }
        async void Core_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.EntityUpdatedArgs e)
        {
            if (e.NewEntity.Name != e.OldEntity.Name)
            {
                await Task.Run(async () =>
                {
                    using (var context = new zvsContext())
                    {
                        var triggers = await context.DeviceValueTriggers
                            .Include(o => o.DeviceValue)
                            .Include(o => o.DeviceValue.Device)
                            .Include(o => o.StoredCommand)
                            .Where(o => o.DeviceValue.DeviceId == e.NewEntity.Id)
                            .ToListAsync();

                        foreach (var trigger in triggers)
                            trigger.SetDescription();

                        await context.TrySaveChangesAsync();
                    }
                });
            }
        }

        void Core_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<Device>.EntityUpdatedArgs e)
        {
            if (e.NewEntity.Name != e.OldEntity.Name)
            {
                Task.Run(async () =>
                {
                    var sw = new Stopwatch();
                    sw.Start();

                    using (var context = new zvsContext())
                    {
                        var triggers = await context.DeviceValueTriggers
                            .Include(o => o.DeviceValue)
                            .Include(o => o.DeviceValue.Device)
                            .Include(o => o.StoredCommand)
                            .Where(o => o.DeviceValue.DeviceId == e.NewEntity.Id)
                            .ToListAsync();

                        foreach (var trigger in triggers)
                            trigger.SetDescription();

                        var deviceIdStr = e.NewEntity.Id.ToString();
                        var storedCommands = new List<StoredCommand>();

                        //Repoll Device Commands
                        var RepollCommandId = await context.BuiltinCommands
                        .Where(o => o.UniqueIdentifier == "REPOLL_ME")
                        .Select(o => o.Id)
                        .FirstOrDefaultAsync();

                            storedCommands.AddRange(await context.StoredCommands
                                  .Include(o => o.Command)
                                  .Where(o => RepollCommandId == o.CommandId && o.Argument == deviceIdStr)
                                  .ToListAsync());

                        //Device Type Commands                    
                        storedCommands.AddRange(await context.StoredCommands
                            .Where(o => o.Argument2 == deviceIdStr)
                            .Where(o => o.Command is DeviceTypeCommand)
                            .ToListAsync());

                        //Device Commands
                        var DeviceCommandIds = await context.DeviceCommands
                            .Where(o => o.DeviceId == e.NewEntity.Id)
                            .Select(o => o.Id)
                            .ToListAsync();

                        storedCommands.AddRange(await context.StoredCommands
                                .Include(o => o.Command)
                                .Where(o => DeviceCommandIds.Contains(o.CommandId))
                                .ToListAsync());

                        foreach (var storedCommand in storedCommands)
                            await storedCommand.SetTargetObjectNameAsync(context);

                        Debug.WriteLine("presavechanges " + sw.Elapsed.ToString());

                        await context.TrySaveChangesAsync();

                        sw.Stop();
                        Debug.WriteLine("Updating device names in storedcommands and triggers took " + sw.Elapsed.ToString());

                    }
                });
            }
        }
        #endregion

        public async void StartAsync()
        {
            log.InfoFormat("Starting Core Processor:{0}", Utils.ApplicationName);

            #region Install Base Commands and Properties
            using (var context = new zvsContext())
            {
                var builtinCommandBuilder = new BuiltinCommandBuilder(null, this, context);

                await builtinCommandBuilder.RegisterAsync(new BuiltinCommand
                {
                    UniqueIdentifier = "REPOLL_ME",
                    Name = "Re-poll Device",
                    ArgumentType = DataType.INTEGER,
                    Description = "This will force a re-poll on an object."
                });

                await builtinCommandBuilder.RegisterAsync(new BuiltinCommand
                {
                    UniqueIdentifier = "REPOLL_ALL",
                    Name = "Re-poll all Devices",
                    ArgumentType = DataType.NONE,
                    Description = "This will force a re-poll on all objects."
                });

                await builtinCommandBuilder.RegisterAsync(new BuiltinCommand
                {
                    UniqueIdentifier = "GROUP_ON",
                    Name = "Turn Group On",
                    ArgumentType = DataType.STRING,
                    Description = "Activates a group."
                });

                await builtinCommandBuilder.RegisterAsync(new BuiltinCommand
                {
                    UniqueIdentifier = "GROUP_OFF",
                    Name = "Turn Group Off",
                    ArgumentType = DataType.STRING,
                    Description = "Deactivates a group."
                });

                await builtinCommandBuilder.RegisterAsync(new BuiltinCommand
                {
                    UniqueIdentifier = "TIMEDELAY",
                    Name = "Time Delay (sec)",
                    ArgumentType = DataType.INTEGER,
                    Description = "Pauses a execution for x seconds."
                });

                await builtinCommandBuilder.RegisterAsync(new BuiltinCommand
                {
                    UniqueIdentifier = "RUN_SCENE",
                    Name = "Run Scene",
                    ArgumentType = DataType.INTEGER,
                    Description = "Argument = SceneId"
                });
            }
            #endregion

            AdapterManager.LoadAdaptersAsync(this);

            await PluginManager.LoadPluginsAsync(this);

            TriggerManager = new TriggerManager(this);
            ScheduledTaskManager = new ScheduledTaskManager(this);
            ScheduledTaskManager.StartAsync();

            //TODO: MAKE A NICE INTERFACE FOR THIS
            //Install Program Options
            using (var context = new zvsContext())
            {
                var option = await context.ProgramOptions.FirstOrDefaultAsync(o => o.UniqueIdentifier == "LOGDIRECTION");
                if (option == null)
                {
                    var result = await ProgramOption.TryAddOrEditAsync(context, new ProgramOption()
                    {
                        UniqueIdentifier = "LOGDIRECTION",
                        Value = "Descending"
                    });

                    if (result.HasError)
                        log.Error(result.Message);
                }
            }
        }

       
       
    }
}
