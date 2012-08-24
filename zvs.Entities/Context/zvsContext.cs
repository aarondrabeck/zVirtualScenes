using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zvs.Model;

namespace zvs.Model
{
    /// <summary>
    /// TO UPDATE WITH CODE FIRST:
    /// 
    /// UPDATE-DATABASE -ProjectName zvs.Context -verbose
    /// 
    /// optional: -force  (force's changes if they require dataloss)
    /// optional: -script (only creates SQL, does not apply to DB)
    /// 
    /// Install-Package EntityFramework -Pre -ProjectName zvs.Context
    /// </summary>
    public partial class zvsContext : DbContext
    {
        public zvsContext()
            : base("zvsDBEFCF")
        {

        }

        public DbSet<BuiltinCommand> BuiltinCommands { get; set; }
        public DbSet<DbInfo> DbInfo { get; set; }

        public DbSet<QueuedCommand> QueuedCommands { get; set; }
        public DbSet<Command> Commands { get; set; }
        public DbSet<CommandOption> CommandOptions { get; set; }

        public DbSet<Device> Devices { get; set; }
        public DbSet<DeviceCommand> DeviceCommands { get; set; }
        public DbSet<DeviceProperty> DeviceProperties { get; set; }
        public DbSet<DevicePropertyOption> DevicePropertyOptions { get; set; }
        public DbSet<DevicePropertyValue> DevicePropertyValues { get; set; }
        public DbSet<DeviceType> DeviceTypes { get; set; }
        public DbSet<DeviceTypeCommand> DeviceTypeCommands { get; set; }
        public DbSet<DeviceValue> DeviceValues { get; set; }
        public DbSet<DeviceValueTrigger> DeviceValueTriggers { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<Plugin> Plugins { get; set; }
        public DbSet<PluginSetting> PluginSettings { get; set; }
        public DbSet<PluginSettingOption> PluginSettingOptions { get; set; }

        public DbSet<ProgramOption> ProgramOptions { get; set; }

        public DbSet<Scene> Scenes { get; set; }
        public DbSet<SceneCommand> SceneCommands { get; set; }
        public DbSet<SceneProperty> SceneProperties { get; set; }
        public DbSet<ScenePropertyOption> ScenePropertyOptions { get; set; }
        public DbSet<ScenePropertyValue> ScenePropertyValues { get; set; }

        public DbSet<ScheduledTask> ScheduledTasks { get; set; }

        

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Device>()
            .HasMany(c => c.Groups)
            .WithMany(a => a.Devices)
            .Map(m => m.ToTable("DeviceToGroups", schemaName: "ZVS"));
        }

    }


}
