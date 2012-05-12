using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using zVirtualScenesCommon;
using System.ComponentModel;
using zvsProcessor.Triggers;

namespace zvsProcessor
{
    public class zvsManager
    {
        public PluginManager pluginManager;
        public TriggerManager triggerManager;
        public ScheduledTaskManager scheduledTaskManager;

        public zvsManager()
        {
            //Start the plug-in manager in its own thread.
            BackgroundWorker PluginManagerWorker = new BackgroundWorker();
            PluginManagerWorker.DoWork += (object sender, DoWorkEventArgs e) =>
            {
                pluginManager = new PluginManager();
            };
            PluginManagerWorker.RunWorkerAsync();


            //Start the trigger manager in its own thread.
            BackgroundWorker triggerManagerWorker = new BackgroundWorker();
            triggerManagerWorker.DoWork += (object sender, DoWorkEventArgs e) =>
            {
                triggerManager = new TriggerManager();
            };
            triggerManagerWorker.RunWorkerAsync();

            //Start the scheduled task manager in its own thread.
            BackgroundWorker scheduledTaskWorker = new BackgroundWorker();
            scheduledTaskWorker.DoWork += (object sender, DoWorkEventArgs e) =>
            {
                scheduledTaskManager = new ScheduledTaskManager();
            };
            scheduledTaskWorker.RunWorkerAsync();
        }
    }
}
