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
        public static zvsEntities2 zvsContext = new zvsEntities2(GetzvsConnectionString);

        public static string GetzvsConnectionString
        {
            get
            {
                string dbpath = string.Empty;

                #if !DEBUG   
                dbpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"database\zvs-debug.db");
                #else
                dbpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"database\zvs.db");
                #endif

                string sqlLiteConnectionString = string.Format("data source=\"{0}\"", dbpath);

                EntityConnectionStringBuilder ee = new EntityConnectionStringBuilder
                {
                    Metadata = @"res://*/Entity.zvsModel.csdl|res://*/Entity.zvsModel.ssdl|res://*/Entity.zvsModel.msl",
                    Provider = @"System.Data.SQLite",
                    ProviderConnectionString = sqlLiteConnectionString,
                };
                return ee.ToString();
            }
        }

        public static string zvsNameAndVersion
        {
            get { 
                
                string version = "zVirtualScenes v2.5 Beta 8";

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
        public delegate void SceneRunCompleteEventHandler(long scene_id, int ErrorCount);

        public static void SceneRunComplete(long scene_id, int ErrorCount)
        {
            if (SceneRunCompleteEvent != null)
                SceneRunCompleteEvent(scene_id, ErrorCount);
        }

        /// <summary>
        /// Called after new device has been added to the database.  
        /// This is needed because Entites does not not handle adding devices well while binding to controls. 
        /// </summary>
        public static event DeviceAddedEventHandler DeviceAddedEvent;
        public delegate void DeviceAddedEventHandler(object sender, EventArgs e);

        public static void DeviceAdded(object sender, EventArgs e)
        {
            if (DeviceAddedEvent != null)
                DeviceAddedEvent(sender,  e);
        } 

    }    
}
