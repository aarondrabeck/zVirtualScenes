using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace zvs.DataModel
{
    /// <summary>
    /// TO UPDATE WITH CODE FIRST:
    /// 
    /// UPDATE-DATABASE -ProjectName zvs.DataModel -verbose
    /// 
    /// optional: -force  (force's changes if they require dataloss)
    /// optional: -script (only creates SQL, does not apply to DB)
    /// 
    /// Install-Package EntityFramework -Pre -ProjectName zvs.Context
    /// </summary>
    /// 


    public class ZvsContext : NotifyEntityChangeContext
    {
        private IEntityContextConnection EntityContextConnection { get; set; }
        [Obsolete("Use IEntityContextConnection to create a context.")]
        public ZvsContext() : base("zvsDBEFCF8") { }

        public ZvsContext(IEntityContextConnection entityContextConnection)
            : base(entityContextConnection.NameOrConnectionString)
        {
            EntityContextConnection = entityContextConnection;
        }

        public DbSet<Adapter> Adapters { get; set; }
        public DbSet<AdapterSetting> AdapterSettings { get; set; }
        public DbSet<AdapterSettingOption> AdapterSettingOptions { get; set; }
        public DbSet<BuiltinCommand> BuiltinCommands { get; set; }
        public DbSet<DbInfo> DbInfo { get; set; }
        public DbSet<Command> Commands { get; set; }
        public DbSet<CommandOption> CommandOptions { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<DeviceCommand> DeviceCommands { get; set; }
        public DbSet<DeviceSetting> DeviceSettings { get; set; }
        public DbSet<DeviceSettingOption> DeviceSettingOptions { get; set; }
        public DbSet<DeviceSettingValue> DeviceSettingValues { get; set; }
        public DbSet<DeviceType> DeviceTypes { get; set; }
        public DbSet<DeviceTypeCommand> DeviceTypeCommands { get; set; }
        public DbSet<DeviceTypeSetting> DeviceTypeSettings { get; set; }
        public DbSet<DeviceTypeSettingValue> DeviceTypeSettingValues { get; set; }
        public DbSet<DeviceTypeSettingOption> DeviceTypeSettingOptions { get; set; }
        public DbSet<DeviceValue> DeviceValues { get; set; }
        public DbSet<DeviceValueHistory> DeviceValueHistories { get; set; }
        public DbSet<DeviceValueTrigger> DeviceValueTriggers { get; set; }
        public DbSet<LogEntry> LogEntries { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<JavaScriptCommand> JavaScriptCommands { get; set; }
        public DbSet<Plugin> Plugins { get; set; }
        public DbSet<PluginSetting> PluginSettings { get; set; }
        public DbSet<PluginSettingOption> PluginSettingOptions { get; set; }
        public DbSet<ProgramOption> ProgramOptions { get; set; }
        public DbSet<Scene> Scenes { get; set; }
        public DbSet<SceneSetting> SceneSettings { get; set; }
        public DbSet<SceneSettingOption> SceneSettingOptions { get; set; }
        public DbSet<SceneSettingValue> SceneSettingValues { get; set; }
        public DbSet<SceneStoredCommand> SceneStoredCommands { get; set; }
        public DbSet<ScheduledTask> ScheduledTasks { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Device>()
                .HasMany(c => c.Groups)
                .WithMany(a => a.Devices)
                .Map(m => m.ToTable("DeviceToGroups", schemaName: "ZVS"));

            modelBuilder.Entity<DeviceValueHistory>()
                .HasRequired(s => s.DeviceValue)
                .WithMany(o => o.History)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<DeviceType>()
                .HasMany(o => o.Settings)
                .WithRequired(o => o.DeviceType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Device>()
                .HasRequired(o => o.Type)
                .WithMany(o => o.Devices)
                .WillCascadeOnDelete(false);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            //Limit Log Table Size
            const int maxLogSize = 2000;
            var addedLogEntryCount = ChangeTracker.Entries<LogEntry>().Count(p => p.State == EntityState.Added);

            if (addedLogEntryCount > 0)
            {
                if (addedLogEntryCount > maxLogSize)
                {
                    var numberToRemove = addedLogEntryCount - maxLogSize;
                    var doNotAdd = ChangeTracker.Entries<LogEntry>()
                        .Where(p => p.State == EntityState.Added)
                        .OrderBy(o => o.Entity.Datetime).Take(numberToRemove);

                    foreach (var entry in doNotAdd)
                        entry.State = EntityState.Detached;

                }

                var currentLogEntryCount = await LogEntries.CountAsync(cancellationToken);
                var toRemove = (currentLogEntryCount + addedLogEntryCount) - maxLogSize;
                if (toRemove > 0)
                {
                    var toBeRemoved =
                        await LogEntries.OrderBy(o => o.Datetime).Take(toRemove).ToListAsync(cancellationToken);
                    LogEntries.RemoveRange(toBeRemoved);
                }
            }

            //Update Run Scene Command Scene Name upon Scene Name update
            var sceneIdsOfUpdatedNames = ChangeTracker.Entries<Scene>().Where(p => p.State == EntityState.Modified && p.Property("Name").IsModified).Select(o => o.Entity.Id.ToString(CultureInfo.InvariantCulture)).ToList();
            Expression<Func<IStoredCommand, bool>> sceneCmdPredicate = o => o.Command.UniqueIdentifier == "RUN_SCENE" && sceneIdsOfUpdatedNames.Contains(o.Argument);
            foreach (var cmd in await SceneStoredCommands.Where(sceneCmdPredicate).ToListAsync(cancellationToken))
                await cmd.SetTargetObjectNameAsync(this);

            foreach (var cmd in await DeviceValueTriggers.Where(sceneCmdPredicate).ToListAsync(cancellationToken))
                await cmd.SetTargetObjectNameAsync(this);

            foreach (var cmd in await ScheduledTasks.Where(sceneCmdPredicate).ToListAsync(cancellationToken))
                await cmd.SetTargetObjectNameAsync(this);

            //Update Activate Group Command Scene Name upon Group Name update
            var groupIdsOfUpdatedNames = ChangeTracker.Entries<Group>().Where(p => p.State == EntityState.Modified && p.Property("Name").IsModified).Select(o => o.Entity.Id.ToString(CultureInfo.InvariantCulture)).ToList();
            Expression<Func<IStoredCommand, bool>> groupPredicate = o => (o.Command.UniqueIdentifier == "GROUP_ON" || o.Command.UniqueIdentifier == "GROUP_OFF") && groupIdsOfUpdatedNames.Contains(o.Argument);
            foreach (var cmd in await SceneStoredCommands.Where(groupPredicate).ToListAsync(cancellationToken))
                await cmd.SetTargetObjectNameAsync(this);

            foreach (var cmd in await DeviceValueTriggers.Where(groupPredicate).ToListAsync(cancellationToken))
                await cmd.SetTargetObjectNameAsync(this);

            foreach (var cmd in await ScheduledTasks.Where(groupPredicate).ToListAsync(cancellationToken))
                await cmd.SetTargetObjectNameAsync(this);

            //Update cmd descriptions on JavaScriptCommand name changes
            var javescriptIdsOfUpdatedNameIds = ChangeTracker.Entries<JavaScriptCommand>().Where(p => p.State == EntityState.Modified && p.Property("Name").IsModified).Select(o => o.Entity.Id).ToList();
            Expression<Func<IStoredCommand, bool>> jsCmdPredicate = o => o.Command is JavaScriptCommand && javescriptIdsOfUpdatedNameIds.Contains(o.Command.Id);
            foreach (var cmd in await SceneStoredCommands.Where(jsCmdPredicate).ToListAsync(cancellationToken))
                await cmd.SetTargetObjectNameAsync(this);

            foreach (var cmd in await DeviceValueTriggers.Where(jsCmdPredicate).ToListAsync(cancellationToken))
                await cmd.SetTargetObjectNameAsync(this);

            foreach (var cmd in await ScheduledTasks.Where(jsCmdPredicate).ToListAsync(cancellationToken))
                await cmd.SetTargetObjectNameAsync(this);

            //Update trigger descriptions on device value name changes
            var deviceValueIdsOfUpdatedNames = ChangeTracker.Entries<DeviceValue>().Where(p => p.State == EntityState.Modified && p.Property("Name").IsModified).Select(o => o.Entity.Id).ToList();
            var triggers = await DeviceValueTriggers.Include(o => o.DeviceValue).Include(o => o.DeviceValue.Device).Where(o => deviceValueIdsOfUpdatedNames.Contains(o.DeviceValue.Id)).ToListAsync(cancellationToken);

            foreach (var trigger in triggers)
                trigger.SetDescription();

            //Update commands on device name changes
            var deviceIdsOfUpdatedNames = ChangeTracker.Entries<Device>().Where(p => p.State == EntityState.Modified && (p.Property("Name").IsModified || p.Property("Location").IsModified)).Select(o => o.Entity.Id).ToList();
            var deviceIdsStrOfUpdatedNames = deviceIdsOfUpdatedNames.Select(o => o.ToString());
            var deviceTriggers = await DeviceValueTriggers.Include(o => o.DeviceValue).Include(o => o.DeviceValue.Device).Where(o => deviceIdsOfUpdatedNames.Contains(o.DeviceValue.DeviceId)).ToListAsync(cancellationToken);

            foreach (var trigger in deviceTriggers)
                trigger.SetDescription();

            //Update repoll commands
            Expression<Func<IStoredCommand, bool>> repollCmdPredicate = o => o.Command.UniqueIdentifier == "REPOLL_ME" && deviceIdsStrOfUpdatedNames.Contains(o.Argument);
            foreach (var cmd in await SceneStoredCommands.Where(repollCmdPredicate).ToListAsync(cancellationToken))
                await cmd.SetTargetObjectNameAsync(this);

            foreach (var cmd in await DeviceValueTriggers.Where(repollCmdPredicate).ToListAsync(cancellationToken))
                await cmd.SetTargetObjectNameAsync(this);

            foreach (var cmd in await ScheduledTasks.Where(repollCmdPredicate).ToListAsync(cancellationToken))
                await cmd.SetTargetObjectNameAsync(this);

            Expression<Func<IStoredCommand, bool>> deviceTypeCmdPredicate = o => o.Command is DeviceTypeCommand && deviceIdsStrOfUpdatedNames.Contains(o.Argument2);
            foreach (var cmd in await SceneStoredCommands.Where(deviceTypeCmdPredicate).ToListAsync(cancellationToken))
                await cmd.SetTargetObjectNameAsync(this);

            foreach (var cmd in await DeviceValueTriggers.Where(deviceTypeCmdPredicate).ToListAsync(cancellationToken))
                await cmd.SetTargetObjectNameAsync(this);

            foreach (var cmd in await ScheduledTasks.Where(deviceTypeCmdPredicate).ToListAsync(cancellationToken))
                await cmd.SetTargetObjectNameAsync(this);

            var deviceCommandIds = await DeviceCommands.Where(o => deviceIdsOfUpdatedNames.Contains(o.DeviceId)).Select(o => o.Id).ToListAsync(cancellationToken);
            Expression<Func<IStoredCommand, bool>> deviceCmdPredicate = o => o.Command is DeviceCommand && deviceCommandIds.Contains(o.Command.Id);
            foreach (var cmd in await SceneStoredCommands.Where(deviceCmdPredicate).ToListAsync(cancellationToken))
                await cmd.SetTargetObjectNameAsync(this);

            foreach (var cmd in await DeviceValueTriggers.Where(deviceCmdPredicate).ToListAsync(cancellationToken))
                await cmd.SetTargetObjectNameAsync(this);

            foreach (var cmd in await ScheduledTasks.Where(deviceCmdPredicate).ToListAsync(cancellationToken))
                await cmd.SetTargetObjectNameAsync(this);

            //Automatically store history when a device value is changed. 
            var history =
                ChangeTracker.Entries<DeviceValue>()
                    .Where(p => p.State == EntityState.Modified && p.Property("Value").IsModified)
                    .Select(
                        o => new DeviceValueHistory
                        {
                            DeviceValueId = o.Entity.Id,
                            Value = o.Entity.Value
                        }).ToList();

            DeviceValueHistories.AddRange(history);
            return await base.SaveChangesAsync(cancellationToken);
        }

        public async Task<Result> TrySaveChangesAsync(CancellationToken cancellationToken)
        {
            try
            {
                await SaveChangesAsync(cancellationToken);
            }
            catch (DbEntityValidationException dbEx)
            {
                var sb = new StringBuilder();
                if (dbEx.EntityValidationErrors == null) return Result.ReportError(sb.ToString());

                foreach (var dbEntityValidationResult in dbEx.EntityValidationErrors)
                {
                    var type = dbEntityValidationResult.Entry.Entity.GetTypeEntityWrapperDetection().Name;
                    var entity = dbEntityValidationResult.Entry.Entity as IIdentity;
                    sb.Append(string.Format("{0} (Id:{1}){2}", type,
                        entity == null ? "N/A" : entity.Id.ToString(CultureInfo.InvariantCulture), Environment.NewLine));

                    foreach (var error in dbEntityValidationResult.ValidationErrors)
                    {
                        sb.Append(string.Format("  Property Name: {0} {2}  Error: {1}{2}", error.PropertyName,
                            error.ErrorMessage, Environment.NewLine));
                    }
                }
                return Result.ReportError(sb.ToString());
            }
            catch (Exception ex)
            {
                return Result.ReportError(ex.GetInnerMostExceptionMessage());
            }

            return Result.ReportSuccess();
        }

        public override int SaveChanges()
        {
            return SaveChangesAsync().Result;
        }
    }
}
