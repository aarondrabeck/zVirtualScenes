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
         {11, () => { if(onPluginsChanged != null) { onPluginsChanged(null, new onEntityChangedEventArgs(System.Data.EntityState.Modified)); }}}
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
                if (!EventsToTrigger.Contains(9)) { EventsToTrigger.Add(6); }

            if (this.ChangeTracker.Entries<plugin>().Where(p => p.State == System.Data.EntityState.Deleted).Count() > 0)
                if (!EventsToTrigger.Contains(10)) { EventsToTrigger.Add(7); }

            if (this.ChangeTracker.Entries<plugin>().Where(p => p.State == System.Data.EntityState.Modified).Count() > 0)
                if (!EventsToTrigger.Contains(11)) { EventsToTrigger.Add(8); }

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
            public System.Data.EntityState ChangeType;

            public onEntityChangedEventArgs(System.Data.EntityState ChangeType)
            {
                this.ChangeType = ChangeType;
            }
        }
    }
}
