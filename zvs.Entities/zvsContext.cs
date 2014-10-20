using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace zvs.DataModel
{
    /// <summary>
    /// TO UPDATE WITH CODE FIRST:
    /// 
    /// UPDATE-DATABASE -ProjectName zvs.Entities -verbose
    /// 
    /// optional: -force  (force's changes if they require dataloss)
    /// optional: -script (only creates SQL, does not apply to DB)
    /// 
    /// Install-Package EntityFramework -Pre -ProjectName zvs.Context
    /// </summary>
    /// 


    public class ZvsContext : NotifyEntityChangeContext
    {
        public ZvsContext() : base("zvsDBEFCF7") { }

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
        public DbSet<SceneCommand> SceneCommands { get; set; }
        public DbSet<SceneSetting> SceneSettings { get; set; }
        public DbSet<SceneSettingOption> SceneSettingOptions { get; set; }
        public DbSet<SceneSettingValue> SceneSettingValues { get; set; }

        public DbSet<ScheduledTask> ScheduledTasks { get; set; }
        public DbSet<StoredCommand> StoredCommands { get; set; }

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


            modelBuilder.Entity<DeviceValueTrigger>()
                    .HasOptional(s => s.StoredCommand)
                    .WithOptionalDependent(a => a.DeviceValueTrigger)
                    .WillCascadeOnDelete(true);

            modelBuilder.Entity<ScheduledTask>()
                   .HasOptional(s => s.StoredCommand)
                   .WithOptionalDependent(a => a.ScheduledTask)
                   .WillCascadeOnDelete(true);

            modelBuilder.Entity<SceneCommand>()
                   .HasOptional(s => s.StoredCommand)
                   .WithOptionalDependent(a => a.SceneCommand)
                   .WillCascadeOnDelete(true);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
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
