using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using zVirtualScenesCommon.Util;
using System.Data.EntityClient;
using System.IO;
using System.ComponentModel;

namespace zVirtualScenesCommon.Entity
{
    public static class zvsEntityControl
    {
        public static class Objects
        {
            //Until EF5 is released this is the only decent way to use the built-in observables and inotify interfaces
            //without doing your own context syncing
            public static zvsEntities2 SharedContext = new zvsEntities2(zvsEntityControl.GetzvsConnectionString);
            public static MTIBindingList DeviceList = ((IListSource)SharedContext.devices).GetList() as MTIBindingList;
        }

        public static string GetDBPath
        {
            get
            {
#if !DEBUG
                return Path.Combine(Paths.AppDataPath, @"zvsDatabase-debug.sdf");
#else
                return Path.Combine(Paths.AppDataPath, @"zvsDatabase.sdf");
#endif
            }
        }

        public static string GetBlankDBPath
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"zvsDatabase.sdf");
            }
        }


        public static string GetzvsConnectionString
        {
            get
            {
                string ConnectionString = string.Format("data source=\"{0}\"", GetDBPath);
                EntityConnectionStringBuilder ee = new EntityConnectionStringBuilder
                {
                    Metadata = @"res://*/Entity.zvsModel.csdl|res://*/Entity.zvsModel.ssdl|res://*/Entity.zvsModel.msl",
                    Provider = @"System.Data.SqlServerCe.4.0",
                    ProviderConnectionString = ConnectionString,
                };
                return ee.ToString();
            }
        }

        public static string zvsNameAndVersion
        {
            get
            {

                string version = "zVirtualScenes v3.0";

#if (!DEBUG)
                return version + " DEBUG MODE";
#else
                return version;
#endif
            }
        }

        #region Table SaveChanges Event

        

        public delegate void onSaveChangesEventHandler(onSaveChangesEventArgs args);
        public static event onSaveChangesEventHandler onSaveChanges;
        public class onSaveChangesEventArgs : EventArgs
        {
            public enum Tables
            {
                builtin_command_que,
                builtin_commands,
                device,
                group_device,
                device_command_que,
                device_commands,
                device_property_values,
                device_propertys,
                device_type_command_que,
                device_type_commands,
                device_value_triggers,
                device_values,
                group,
                program_options,
                scene,
                scene_commands,
                scene_property,
                scene_property_value,
                scheduled_tasks
            }

            public enum ChangeType
            {
                None,
                AddRemove,
                Modified
            }

            public List<Tables> tablesChanged = new List<Tables>();
            public ChangeType changeType = ChangeType.None;
        }

        public static void CallonSaveChanges(object sender, List<zVirtualScenesCommon.Entity.zvsEntityControl.onSaveChangesEventArgs.Tables> TablesChanged, zVirtualScenesCommon.Entity.zvsEntityControl.onSaveChangesEventArgs.ChangeType changeType)
        {
            if (onSaveChanges != null)
                onSaveChanges(new onSaveChangesEventArgs() { tablesChanged = TablesChanged, changeType = changeType });
        }
        #endregion

        /// <summary>
        /// Called when a scene has been called to be executed.
        /// </summary>
        public static event SceneRunStartedEventHandler SceneRunStartedEvent;
        public delegate void SceneRunStartedEventHandler(scene s, string result);

        public static void SceneRunStarted(scene s, string result)
        {
            if (SceneRunStartedEvent != null)
                SceneRunStartedEvent(s, result);
        }

        /// <summary>
        /// Called when a scene is finished.
        /// </summary>
        public static event SceneRunCompleteEventHandler SceneRunCompleteEvent;
        public delegate void SceneRunCompleteEventHandler(int scene_id, int ErrorCount);

        public static void SceneRunComplete(int scene_id, int ErrorCount)
        {
            if (SceneRunCompleteEvent != null)
                SceneRunCompleteEvent(scene_id, ErrorCount);
        }

        public delegate void SceneModifiedEventHandler(object sender, int? SceneID);
        public static event SceneModifiedEventHandler SceneModified;
        public static void CallSceneModified(object sender, int? SceneID)
        {
            if (SceneModified != null)
                SceneModified(sender, SceneID);
        }

        public delegate void ScheduledTaskModifiedEventHandler(object sender, string PropertyModified);
        public static event ScheduledTaskModifiedEventHandler ScheduledTaskModified;
        public static void CallScheduledTaskModified(object sender, string PropertyModified)
        {
            if (ScheduledTaskModified != null)
                ScheduledTaskModified(sender, PropertyModified);
        }

        public delegate void TriggerModifiedEventHandler(object sender, string PropertyModified);
        public static event TriggerModifiedEventHandler TriggerModified;
        public static void CallTriggerModified(object sender, string PropertyModified)
        {
            if (TriggerModified != null)
                TriggerModified(sender, PropertyModified);
        }

    }
}
