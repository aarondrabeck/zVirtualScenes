using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zvs.Entities;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace zvs.Entities
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
    public partial class zvsContext : NotifyEntityChangeContext
    {
        public zvsContext() : base("zvsDBEFCF6") { }

        public DbSet<Adapter> Adapters { get; set; }
        public DbSet<AdapterSetting> AdapterSettings { get; set; }
        public DbSet<AdapterSettingOption> AdapterSettingOptions { get; set; }

        public DbSet<BuiltinCommand> BuiltinCommands { get; set; }
        public DbSet<DbInfo> DbInfo { get; set; }

        public DbSet<QueuedCommand> QueuedCommands { get; set; }
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
        public DbSet<DeviceValueTrigger> DeviceValueTriggers { get; set; }

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

            modelBuilder.Entity<DeviceType>()
                .HasMany(o => o.Settings)
                .WithRequired(o => o.DeviceType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DeviceValueTrigger>()
                    .HasOptional(s => s.StoredCommand)
                    .WithOptionalPrincipal(a => a.DeviceValueTrigger)
                    .WillCascadeOnDelete();

            modelBuilder.Entity<ScheduledTask>()
                   .HasOptional(s => s.StoredCommand)
                   .WithOptionalPrincipal(a => a.ScheduledTask)
                   .WillCascadeOnDelete();

            modelBuilder.Entity<SceneCommand>()
                   .HasOptional(s => s.StoredCommand)
                   .WithOptionalPrincipal(a => a.SceneCommand)
                   .WillCascadeOnDelete();

        }

        public async Task<Result> TrySaveChangesAsync()
        {
            try
            {
                await SaveChangesAsync();
            }
            catch (DbEntityValidationException dbEx)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                        sb.Append(string.Format("{0}:{1}" + Environment.NewLine, validationError.PropertyName, validationError.ErrorMessage));
                }
                return new Result(sb.ToString());
            }
            catch (Exception ex)
            {
                return new Result(ex.GetInnerMostExceptionMessage());
            }

            return new Result();
        }

        public override int SaveChanges()
        {
            return SaveChangesAsync().Result;
        }
    }


}
