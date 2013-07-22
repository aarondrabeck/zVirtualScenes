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
    public partial class zvsContext : DbContext
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
        public delegate void onEntityChangedventHandler(object sender, onEntityChangedEventArgs args);

        //Events
        public static event onEntityChangedventHandler onDevicesChanged;
        public static event onEntityChangedventHandler onGroupsChanged;
        public static event onEntityChangedventHandler onGroup_DevicesChanged;
        public static event onEntityChangedventHandler onPluginsChanged;
        public static event onEntityChangedventHandler onDeviceValueChanged;
        public static event onEntityChangedventHandler onScenesChanged;
        public static event onEntityChangedventHandler onSceneCommandsChanged;
        public static event onEntityChangedventHandler onDeviceValueTriggersChanged;
        public static event onEntityChangedventHandler onScheduledTasksChanged;
        public static event onEntityChangedventHandler onJavaScriptCommandsChanged;


        public static Dictionary<int, Action> EventsDictionary = new Dictionary<int, Action> {
         {0, () => { if(onDevicesChanged != null) { onDevicesChanged(null, new onEntityChangedEventArgs(EntityState.Added)); }}},
         {1, () => { if(onDevicesChanged != null) { onDevicesChanged(null, new onEntityChangedEventArgs(EntityState.Deleted)); }}},
         {2, () => { if(onDevicesChanged != null) { onDevicesChanged(null, new onEntityChangedEventArgs(EntityState.Modified)); }}},
         {3, () => { if(onGroupsChanged != null) { onGroupsChanged(null, new onEntityChangedEventArgs(EntityState.Added)); }}},
         {4, () => { if(onGroupsChanged != null) { onGroupsChanged(null, new onEntityChangedEventArgs(EntityState.Deleted)); }}},
         {5, () => { if(onGroupsChanged != null) { onGroupsChanged(null, new onEntityChangedEventArgs(EntityState.Modified)); }}},
         {6, () => { if(onGroup_DevicesChanged != null) { onGroup_DevicesChanged(null, new onEntityChangedEventArgs(EntityState.Added)); }}},
         {7, () => { if(onGroup_DevicesChanged != null) { onGroup_DevicesChanged(null, new onEntityChangedEventArgs(EntityState.Deleted)); }}},
         {8, () => { if(onGroup_DevicesChanged != null) { onGroup_DevicesChanged(null, new onEntityChangedEventArgs(EntityState.Modified)); }}},
         {9, () => { if(onPluginsChanged != null) { onPluginsChanged(null, new onEntityChangedEventArgs(EntityState.Added)); }}},
         {10, () => { if(onPluginsChanged != null) { onPluginsChanged(null, new onEntityChangedEventArgs(EntityState.Deleted)); }}},
         {11, () => { if(onPluginsChanged != null) { onPluginsChanged(null, new onEntityChangedEventArgs(EntityState.Modified)); }}},
         {12, () => { if(onDeviceValueChanged != null) { onDeviceValueChanged(null, new onEntityChangedEventArgs(EntityState.Added)); }}},
         {13, () => { if(onDeviceValueChanged != null) { onDeviceValueChanged(null, new onEntityChangedEventArgs(EntityState.Deleted)); }}},
         {14, () => { if(onDeviceValueChanged != null) { onDeviceValueChanged(null, new onEntityChangedEventArgs(EntityState.Modified)); }}},
         {15, () => { if(onScenesChanged != null) { onScenesChanged(null, new onEntityChangedEventArgs(EntityState.Added)); }}},
         {16, () => { if(onScenesChanged != null) { onScenesChanged(null, new onEntityChangedEventArgs(EntityState.Deleted)); }}},
         {17, () => { if(onScenesChanged != null) { onScenesChanged(null, new onEntityChangedEventArgs(EntityState.Modified)); }}},
         {18, () => { if(onSceneCommandsChanged != null) { onSceneCommandsChanged(null, new onEntityChangedEventArgs(EntityState.Added)); }}},
         {19, () => { if(onSceneCommandsChanged != null) { onSceneCommandsChanged(null, new onEntityChangedEventArgs(EntityState.Deleted)); }}},
         {20, () => { if(onSceneCommandsChanged != null) { onSceneCommandsChanged(null, new onEntityChangedEventArgs(EntityState.Modified)); }}},
         {21, () => { if(onDeviceValueTriggersChanged != null) { onDeviceValueTriggersChanged(null, new onEntityChangedEventArgs(EntityState.Added)); }}},
         {22, () => { if(onDeviceValueTriggersChanged != null) { onDeviceValueTriggersChanged(null, new onEntityChangedEventArgs(EntityState.Deleted)); }}},
         {23, () => { if(onDeviceValueTriggersChanged != null) { onDeviceValueTriggersChanged(null, new onEntityChangedEventArgs(EntityState.Modified)); }}},
         {24, () => { if(onScheduledTasksChanged != null) { onScheduledTasksChanged(null, new onEntityChangedEventArgs(EntityState.Added)); }}},
         {25, () => { if(onScheduledTasksChanged != null) { onScheduledTasksChanged(null, new onEntityChangedEventArgs(EntityState.Deleted)); }}},
         {26, () => { if(onScheduledTasksChanged != null) { onScheduledTasksChanged(null, new onEntityChangedEventArgs(EntityState.Modified)); }}},
         {27, () => { if(onJavaScriptCommandsChanged != null) { onJavaScriptCommandsChanged(null, new onEntityChangedEventArgs(EntityState.Added)); }}},
         {28, () => { if(onJavaScriptCommandsChanged != null) { onJavaScriptCommandsChanged(null, new onEntityChangedEventArgs(EntityState.Deleted)); }}},
         {29, () => { if(onJavaScriptCommandsChanged != null) { onJavaScriptCommandsChanged(null, new onEntityChangedEventArgs(EntityState.Modified)); }}}
        };


        public override async Task<int> SaveChangesAsync(System.Threading.CancellationToken cancellationToken)
        {
            List<int> EventsToTrigger = new List<int>();

            //Determine the changes that are about to be made
            //Devices
            if (this.ChangeTracker.Entries<Device>().Where(p => p.State == EntityState.Added).Count() > 0)
                if (!EventsToTrigger.Contains(0)) { EventsToTrigger.Add(0); }

            if (this.ChangeTracker.Entries<Device>().Where(p => p.State == EntityState.Deleted).Count() > 0)
                if (!EventsToTrigger.Contains(0)) { EventsToTrigger.Add(1); }

            if (this.ChangeTracker.Entries<Device>().Where(p => p.State == EntityState.Modified).Count() > 0)
                if (!EventsToTrigger.Contains(0)) { EventsToTrigger.Add(2); }

            //Groups
            if (this.ChangeTracker.Entries<Group>().Where(p => p.State == EntityState.Added).Count() > 0)
                if (!EventsToTrigger.Contains(3)) { EventsToTrigger.Add(3); }

            if (this.ChangeTracker.Entries<Group>().Where(p => p.State == EntityState.Deleted).Count() > 0)
                if (!EventsToTrigger.Contains(4)) { EventsToTrigger.Add(4); }

            if (this.ChangeTracker.Entries<Group>().Where(p => p.State == EntityState.Modified).Count() > 0)
                if (!EventsToTrigger.Contains(5)) { EventsToTrigger.Add(5); }

            //Group Devices
            //TODO: ADDRESS THIS
            if (this.ChangeTracker.Entries<Group>().Where(p => p.State == EntityState.Added).Count() > 0)
                if (!EventsToTrigger.Contains(6)) { EventsToTrigger.Add(6); }

            if (this.ChangeTracker.Entries<Group>().Where(p => p.State == EntityState.Deleted).Count() > 0)
                if (!EventsToTrigger.Contains(7)) { EventsToTrigger.Add(7); }

            if (this.ChangeTracker.Entries<Group>().Where(p => p.State == EntityState.Modified).Count() > 0)
                if (!EventsToTrigger.Contains(8)) { EventsToTrigger.Add(8); }

            //plugins
            if (this.ChangeTracker.Entries<Adapter>().Where(p => p.State == EntityState.Added).Count() > 0)
                if (!EventsToTrigger.Contains(9)) { EventsToTrigger.Add(9); }

            if (this.ChangeTracker.Entries<Adapter>().Where(p => p.State == EntityState.Deleted).Count() > 0)
                if (!EventsToTrigger.Contains(10)) { EventsToTrigger.Add(10); }

            if (this.ChangeTracker.Entries<Adapter>().Where(p => p.State == EntityState.Modified).Count() > 0)
                if (!EventsToTrigger.Contains(11)) { EventsToTrigger.Add(11); }

            //device values
            if (this.ChangeTracker.Entries<DeviceValue>().Where(p => p.State == EntityState.Added).Count() > 0)
                if (!EventsToTrigger.Contains(12)) { EventsToTrigger.Add(12); }

            if (this.ChangeTracker.Entries<DeviceValue>().Where(p => p.State == EntityState.Deleted).Count() > 0)
                if (!EventsToTrigger.Contains(13)) { EventsToTrigger.Add(13); }

            if (this.ChangeTracker.Entries<DeviceValue>().Where(p => p.State == EntityState.Modified).Count() > 0)
                if (!EventsToTrigger.Contains(14)) { EventsToTrigger.Add(14); }

            //scenes
            if (this.ChangeTracker.Entries<Scene>().Where(p => p.State == EntityState.Added).Count() > 0)
                if (!EventsToTrigger.Contains(15)) { EventsToTrigger.Add(15); }

            if (this.ChangeTracker.Entries<Scene>().Where(p => p.State == EntityState.Deleted).Count() > 0)
                if (!EventsToTrigger.Contains(16)) { EventsToTrigger.Add(16); }

            if (this.ChangeTracker.Entries<Scene>().Where(p => p.State == EntityState.Modified).Count() > 0)
                if (!EventsToTrigger.Contains(17)) { EventsToTrigger.Add(17); }

            //scene commands
            if (this.ChangeTracker.Entries<SceneCommand>().Where(p => p.State == EntityState.Added).Count() > 0)
                if (!EventsToTrigger.Contains(18)) { EventsToTrigger.Add(18); }

            if (this.ChangeTracker.Entries<SceneCommand>().Where(p => p.State == EntityState.Deleted).Count() > 0)
                if (!EventsToTrigger.Contains(19)) { EventsToTrigger.Add(19); }

            if (this.ChangeTracker.Entries<SceneCommand>().Where(p => p.State == EntityState.Modified).Count() > 0)
                if (!EventsToTrigger.Contains(20)) { EventsToTrigger.Add(20); }

            //device value triggers
            if (this.ChangeTracker.Entries<DeviceValueTrigger>().Where(p => p.State == EntityState.Added).Count() > 0)
                if (!EventsToTrigger.Contains(21)) { EventsToTrigger.Add(21); }

            if (this.ChangeTracker.Entries<DeviceValueTrigger>().Where(p => p.State == EntityState.Deleted).Count() > 0)
                if (!EventsToTrigger.Contains(22)) { EventsToTrigger.Add(22); }

            if (this.ChangeTracker.Entries<DeviceValueTrigger>().Where(p => p.State == EntityState.Modified).Count() > 0)
                if (!EventsToTrigger.Contains(23)) { EventsToTrigger.Add(23); }

            //scheduled task
            if (this.ChangeTracker.Entries<ScheduledTask>().Where(p => p.State == EntityState.Added).Count() > 0)
                if (!EventsToTrigger.Contains(24)) { EventsToTrigger.Add(24); }

            if (this.ChangeTracker.Entries<ScheduledTask>().Where(p => p.State == EntityState.Deleted).Count() > 0)
                if (!EventsToTrigger.Contains(25)) { EventsToTrigger.Add(25); }

            if (this.ChangeTracker.Entries<ScheduledTask>().Where(p => p.State == EntityState.Modified).Count() > 0)
                if (!EventsToTrigger.Contains(26)) { EventsToTrigger.Add(26); }

            //JavaScript Commands
            if (this.ChangeTracker.Entries<JavaScriptCommand>().Where(p => p.State == EntityState.Added).Count() > 0)
                if (!EventsToTrigger.Contains(27)) { EventsToTrigger.Add(27); }

            if (this.ChangeTracker.Entries<JavaScriptCommand>().Where(p => p.State == EntityState.Deleted).Count() > 0)
                if (!EventsToTrigger.Contains(28)) { EventsToTrigger.Add(28); }

            if (this.ChangeTracker.Entries<JavaScriptCommand>().Where(p => p.State == EntityState.Modified).Count() > 0)
                if (!EventsToTrigger.Contains(29)) { EventsToTrigger.Add(29); }


            //Save the changes to the Database
            int result = await base.SaveChangesAsync(cancellationToken);

            //Call events to notify other contexts
            foreach (int key in EventsToTrigger)
            {
                if (EventsDictionary.ContainsKey(key))
                    ((Action)EventsDictionary[key]).DynamicInvoke();
            }

            return result;
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

        public class onEntityChangedEventArgs : EventArgs
        {
            public EntityState ChangeType { get; private set; }

            public onEntityChangedEventArgs(EntityState ChangeType)
            {
                this.ChangeType = ChangeType;
            }
        }

    }


}
