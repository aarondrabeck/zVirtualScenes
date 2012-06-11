using System.Linq;

namespace zVirtualScenesModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    public partial class zvsLocalDBEntities : DbContext
    {
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

        public static Dictionary<int, Action> EventsDictionary = new Dictionary<int, Action> {
         {0, () => { if(onDevicesChanged != null) { onDevicesChanged(null, new onEntityChangedEventArgs(System.Data.EntityState.Added)); }}},
         {1, () => { if(onDevicesChanged != null) { onDevicesChanged(null, new onEntityChangedEventArgs(System.Data.EntityState.Deleted)); }}},
         {2, () => { if(onDevicesChanged != null) { onDevicesChanged(null, new onEntityChangedEventArgs(System.Data.EntityState.Modified)); }}},
         {3, () => { if(onGroupsChanged != null) { onGroupsChanged(null, new onEntityChangedEventArgs(System.Data.EntityState.Added)); }}},
         {4, () => { if(onGroupsChanged != null) { onGroupsChanged(null, new onEntityChangedEventArgs(System.Data.EntityState.Deleted)); }}},
         {5, () => { if(onGroupsChanged != null) { onGroupsChanged(null, new onEntityChangedEventArgs(System.Data.EntityState.Modified)); }}},
         {6, () => { if(onGroup_DevicesChanged != null) { onGroup_DevicesChanged(null, new onEntityChangedEventArgs(System.Data.EntityState.Added)); }}},
         {7, () => { if(onGroup_DevicesChanged != null) { onGroup_DevicesChanged(null, new onEntityChangedEventArgs(System.Data.EntityState.Deleted)); }}},
         {8, () => { if(onGroup_DevicesChanged != null) { onGroup_DevicesChanged(null, new onEntityChangedEventArgs(System.Data.EntityState.Modified)); }}},
         {9, () => { if(onPluginsChanged != null) { onPluginsChanged(null, new onEntityChangedEventArgs(System.Data.EntityState.Added)); }}},
         {10, () => { if(onPluginsChanged != null) { onPluginsChanged(null, new onEntityChangedEventArgs(System.Data.EntityState.Deleted)); }}},
         {11, () => { if(onPluginsChanged != null) { onPluginsChanged(null, new onEntityChangedEventArgs(System.Data.EntityState.Modified)); }}},
         {12, () => { if(onDeviceValueChanged != null) { onDeviceValueChanged(null, new onEntityChangedEventArgs(System.Data.EntityState.Added)); }}},
         {13, () => { if(onDeviceValueChanged != null) { onDeviceValueChanged(null, new onEntityChangedEventArgs(System.Data.EntityState.Deleted)); }}},
         {14, () => { if(onDeviceValueChanged != null) { onDeviceValueChanged(null, new onEntityChangedEventArgs(System.Data.EntityState.Modified)); }}},
         {15, () => { if(onScenesChanged != null) { onScenesChanged(null, new onEntityChangedEventArgs(System.Data.EntityState.Added)); }}},
         {16, () => { if(onScenesChanged != null) { onScenesChanged(null, new onEntityChangedEventArgs(System.Data.EntityState.Deleted)); }}},
         {17, () => { if(onScenesChanged != null) { onScenesChanged(null, new onEntityChangedEventArgs(System.Data.EntityState.Modified)); }}},
         {18, () => { if(onSceneCommandsChanged != null) { onSceneCommandsChanged(null, new onEntityChangedEventArgs(System.Data.EntityState.Added)); }}},
         {19, () => { if(onSceneCommandsChanged != null) { onSceneCommandsChanged(null, new onEntityChangedEventArgs(System.Data.EntityState.Deleted)); }}},
         {20, () => { if(onSceneCommandsChanged != null) { onSceneCommandsChanged(null, new onEntityChangedEventArgs(System.Data.EntityState.Modified)); }}},
         {21, () => { if(onDeviceValueTriggersChanged != null) { onDeviceValueTriggersChanged(null, new onEntityChangedEventArgs(System.Data.EntityState.Added)); }}},
         {22, () => { if(onDeviceValueTriggersChanged != null) { onDeviceValueTriggersChanged(null, new onEntityChangedEventArgs(System.Data.EntityState.Deleted)); }}},
         {23, () => { if(onDeviceValueTriggersChanged != null) { onDeviceValueTriggersChanged(null, new onEntityChangedEventArgs(System.Data.EntityState.Modified)); }}}
        };

        public override int SaveChanges()
        {
            List<int> EventsToTrigger = new List<int>();

            //Determine the changes that are about to be made
            //Devices
            if (this.ChangeTracker.Entries<device>().Where(p => p.State == System.Data.EntityState.Added).Count() > 0)
                if (!EventsToTrigger.Contains(0)) { EventsToTrigger.Add(0); }

            if (this.ChangeTracker.Entries<device>().Where(p => p.State == System.Data.EntityState.Deleted).Count() > 0)
                if (!EventsToTrigger.Contains(0)) { EventsToTrigger.Add(1); }

            if (this.ChangeTracker.Entries<device>().Where(p => p.State == System.Data.EntityState.Modified).Count() > 0)
                if (!EventsToTrigger.Contains(0)) { EventsToTrigger.Add(2); }

            //Groups
            if (this.ChangeTracker.Entries<group>().Where(p => p.State == System.Data.EntityState.Added).Count() > 0)
                if (!EventsToTrigger.Contains(3)) { EventsToTrigger.Add(3); }

            if (this.ChangeTracker.Entries<group>().Where(p => p.State == System.Data.EntityState.Deleted).Count() > 0)
                if (!EventsToTrigger.Contains(4)) { EventsToTrigger.Add(4); }

            if (this.ChangeTracker.Entries<group>().Where(p => p.State == System.Data.EntityState.Modified).Count() > 0)
                if (!EventsToTrigger.Contains(5)) { EventsToTrigger.Add(5); }

            //Group Devices
            if (this.ChangeTracker.Entries<group_devices>().Where(p => p.State == System.Data.EntityState.Added).Count() > 0)
                if (!EventsToTrigger.Contains(6)) { EventsToTrigger.Add(6); }

            if (this.ChangeTracker.Entries<group_devices>().Where(p => p.State == System.Data.EntityState.Deleted).Count() > 0)
                if (!EventsToTrigger.Contains(7)) { EventsToTrigger.Add(7); }

            if (this.ChangeTracker.Entries<group_devices>().Where(p => p.State == System.Data.EntityState.Modified).Count() > 0)
                if (!EventsToTrigger.Contains(8)) { EventsToTrigger.Add(8); }

            //plugins
            if (this.ChangeTracker.Entries<plugin>().Where(p => p.State == System.Data.EntityState.Added).Count() > 0)
                if (!EventsToTrigger.Contains(9)) { EventsToTrigger.Add(9); }

            if (this.ChangeTracker.Entries<plugin>().Where(p => p.State == System.Data.EntityState.Deleted).Count() > 0)
                if (!EventsToTrigger.Contains(10)) { EventsToTrigger.Add(10); }

            if (this.ChangeTracker.Entries<plugin>().Where(p => p.State == System.Data.EntityState.Modified).Count() > 0)
                if (!EventsToTrigger.Contains(11)) { EventsToTrigger.Add(11); }

            //device values
            if (this.ChangeTracker.Entries<device_values>().Where(p => p.State == System.Data.EntityState.Added).Count() > 0)
                if (!EventsToTrigger.Contains(12)) { EventsToTrigger.Add(12); }

            if (this.ChangeTracker.Entries<device_values>().Where(p => p.State == System.Data.EntityState.Deleted).Count() > 0)
                if (!EventsToTrigger.Contains(13)) { EventsToTrigger.Add(13); }

            if (this.ChangeTracker.Entries<device_values>().Where(p => p.State == System.Data.EntityState.Modified).Count() > 0)
                if (!EventsToTrigger.Contains(14)) { EventsToTrigger.Add(14); }

            //scenes
            if (this.ChangeTracker.Entries<scene>().Where(p => p.State == System.Data.EntityState.Added).Count() > 0)
                if (!EventsToTrigger.Contains(15)) { EventsToTrigger.Add(15); }

            if (this.ChangeTracker.Entries<scene>().Where(p => p.State == System.Data.EntityState.Deleted).Count() > 0)
                if (!EventsToTrigger.Contains(16)) { EventsToTrigger.Add(16); }

            if (this.ChangeTracker.Entries<scene>().Where(p => p.State == System.Data.EntityState.Modified).Count() > 0)
                if (!EventsToTrigger.Contains(17)) { EventsToTrigger.Add(17); }

            //scene commands
            if (this.ChangeTracker.Entries<scene_commands>().Where(p => p.State == System.Data.EntityState.Added).Count() > 0)
                if (!EventsToTrigger.Contains(18)) { EventsToTrigger.Add(18); }

            if (this.ChangeTracker.Entries<scene_commands>().Where(p => p.State == System.Data.EntityState.Deleted).Count() > 0)
                if (!EventsToTrigger.Contains(19)) { EventsToTrigger.Add(19); }

            if (this.ChangeTracker.Entries<scene_commands>().Where(p => p.State == System.Data.EntityState.Modified).Count() > 0)
                if (!EventsToTrigger.Contains(20)) { EventsToTrigger.Add(20); }

            //device value triggers
            if (this.ChangeTracker.Entries<device_value_triggers>().Where(p => p.State == System.Data.EntityState.Added).Count() > 0)
                if (!EventsToTrigger.Contains(21)) { EventsToTrigger.Add(21); }

            if (this.ChangeTracker.Entries<device_value_triggers>().Where(p => p.State == System.Data.EntityState.Deleted).Count() > 0)
                if (!EventsToTrigger.Contains(22)) { EventsToTrigger.Add(22); }

            if (this.ChangeTracker.Entries<device_value_triggers>().Where(p => p.State == System.Data.EntityState.Modified).Count() > 0)
                if (!EventsToTrigger.Contains(23)) { EventsToTrigger.Add(23); }


            //Save the changes to the Database
            int result = base.SaveChanges();

            //Call events to notify other contexts
            foreach (int key in EventsToTrigger)
            {
                if (EventsDictionary.ContainsKey(key))
                    ((Action)EventsDictionary[key]).DynamicInvoke();
            }

            return result;
        }

        public class onEntityChangedEventArgs : EventArgs
        {
            public System.Data.EntityState ChangeType { get; private set; }  

            public onEntityChangedEventArgs(System.Data.EntityState ChangeType)
            {
                this.ChangeType = ChangeType;
            }
        }
    }
}
