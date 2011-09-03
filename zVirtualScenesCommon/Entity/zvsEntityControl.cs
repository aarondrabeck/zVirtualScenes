using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using zVirtualScenesCommon.Util;

namespace zVirtualScenesCommon.Entity
{
    public static class zvsEntityControl
    {
        public static zvsEntities2 zvsContext = new zvsEntities2();

        public static string GetProgramNameAndVersion
        {
            get { return "zVirtualScenes -  v2.4"; }// +ProgramVersion + " | db" + DatabaseVersion; }
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
        public delegate void SceneRunCompleteEventHandler(scene s, int ErrorCount);

        public static void SceneRunComplete(scene s, int ErrorCount)
        {
            if (SceneRunCompleteEvent != null)
                SceneRunCompleteEvent(s, ErrorCount);
        }

    }    
}
