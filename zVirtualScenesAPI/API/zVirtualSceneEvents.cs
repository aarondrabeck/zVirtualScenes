using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using zVirtualScenesAPI.Structs;

namespace zVirtualScenesAPI.Events
{
    public static class zVirtualSceneEvents
    {
        /// <summary>
        /// Called when a command is added to the que
        /// </summary>
        public static event CommandAddedEventHandler CommandAddedEvent;
        public delegate void CommandAddedEventHandler(int QueCmdID);

        public static void QueCommandAdded(int QueCmdID)
        {
            if (CommandAddedEvent != null)
                CommandAddedEvent(QueCmdID);
        }

        /// <summary>
        /// Called after a command is executed
        /// </summary>
        public static event CommandRunCompleteEventHandler CommandRunCompleteEvent;
        public delegate void CommandRunCompleteEventHandler(int QueID, bool withErrors, string txtError);

        public static void CommandRunComplete(int QueID, bool withErrors, string txtError)
        {
            if (CommandRunCompleteEvent != null)
                CommandRunCompleteEvent(QueID, withErrors, txtError);
        }

        /// <summary>
        /// Called after the Value has been changed in the database
        /// </summary>
        public static event ValueDataChangedEventHandler ValueDataChangedEvent;
        public delegate void ValueDataChangedEventHandler(int ObjectId, string ValueID, string label, string Value, string PreviousValue);        
        
        public static void ValueDataChanged(int ObjectId, string ValueID, string label, string Value, string prevVal)
        {
            if (ValueDataChangedEvent != null)
                ValueDataChangedEvent(ObjectId, ValueID, label, Value, prevVal);
        }

        /// <summary>
        /// Called before the value is changed in the database
        /// </summary>
        public static event ValueChangingEventHandler ValueChangingEvent;
        public delegate void ValueChangingEventHandler(int ObjectId, string ValueID, string label, string Value);
        
        public static void ValueChanging(int ObjectId, string ValueID, string label, string Value)
        {
            if (ValueChangingEvent != null)
                ValueChangingEvent(ObjectId, ValueID, label, Value);
        }

        /// <summary>
        /// Called after the value is changed in the database
        /// </summary>
        public static event SceneChangedEventHandler SceneChangedEvent;
        public delegate void SceneChangedEventHandler(int SceneID);

        public static void SceneChanged(int SceneID)
        {
            if (SceneChangedEvent != null)
                SceneChangedEvent(SceneID);
        }

        /// <summary>
        /// Called after the value is changed in the database
        /// </summary>
        public static event SceneCMDChangedEventHandler SceneCMDChangedEvent;
        public delegate void SceneCMDChangedEventHandler(int SceneID);

        public static void SceneCMDChanged(int SceneID)
        {
            if (SceneCMDChangedEvent != null)
                SceneCMDChangedEvent(SceneID);
        }

        /// <summary>
        /// Called after the value is changed in the database
        /// </summary>
        public static event SceneRunCompleteEventHandler SceneRunCompleteEvent;
        public delegate void SceneRunCompleteEventHandler(int SceneID, int ErrorCount);

        public static void SceneRunComplete(int SceneID, int ErrorCount)
        {
            if (SceneRunCompleteEvent != null)
                SceneRunCompleteEvent(SceneID, ErrorCount);
        }

        
    }
}
