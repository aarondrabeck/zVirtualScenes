using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using zVirtualScenesCommon.Util;
using System.Data.EntityClient;
using System.IO;

namespace zVirtualScenesCommon.Entity
{
    public static class zvsEntityControl
    {
        //public static zvsEntities2 zvsContext = new zvsEntities2(GetzvsConnectionString);

        public static string GetDBPath
        {
            get
            {
                #if !DEBUG
                    return Path.Combine(Paths.AppDataPath, @"\zvs-debug.db");
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
            get { 
                
                string version = "zVirtualScenes v2.6";

                #if (!DEBUG)
                return version + " DEBUG MODE";
                #else
                return version;
                #endif
            }
        }

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

        public delegate void DeviceModifiedEventHandler(int device_id, string PropertyModified);
        public static event DeviceModifiedEventHandler DeviceModified;
        public static void CallDeviceModified(int device_id, string PropertyModified)
        {
            if (DeviceModified != null)
                DeviceModified(device_id, PropertyModified);
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
