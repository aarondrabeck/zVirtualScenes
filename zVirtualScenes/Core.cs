using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using zVirtualScenes.Triggers;
using System.Windows.Threading;
using zVirtualScenesModel;
using System.Collections.ObjectModel;
using System.Windows;
using System.IO;
using Microsoft.Win32;

namespace zVirtualScenes
{
    public class Core
    {
        public PluginManager pluginManager;
        public TriggerManager triggerManager;
        public ScheduledTaskManager scheduledTaskManager;
        public Logger Logger;
        public Dispatcher Dispatcher;

        public Core(Dispatcher Dispatcher)
        {           

            this.Dispatcher = Dispatcher;

         

            //Create a instace of the logger
            Logger = new Logger();

            Logger.WriteToLog(Urgency.INFO, "Starting Core Processor", Utils.ApplicationName);

            BackgroundWorker PluginBW = new BackgroundWorker();
            PluginBW.DoWork += (sender, args) =>
            {
                pluginManager = new PluginManager(this);
            };

            PluginBW.RunWorkerCompleted += (sender, args) =>
                {
                    if (args.Error != null)
                    {
                        if (args.Error is System.Reflection.ReflectionTypeLoadException)
                        {
                            System.Reflection.ReflectionTypeLoadException reflectionEx = (System.Reflection.ReflectionTypeLoadException)args.Error;
                            string error = "Cannot load one or more plug-ins.";
                            Exception ex = reflectionEx.LoaderExceptions.FirstOrDefault();
                            if (ex != null)
                            {
                                error = ex.ToString() +
                                    Environment.NewLine +
                                    Environment.NewLine +
                                    string.Format("This plug-in might not be compatible with {0}. Try removing the plugin and re-launching the application. ", Utils.ApplicationNameAndVersion);
                            }
                            if (MessageBox.Show(error, "Fatal plug-in load error", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK) == MessageBoxResult.OK)
                            {
                                Environment.Exit(1);
                                return;
                            }
                        }
                        else
                        {
                            if (MessageBox.Show(args.Error.Message, "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK) == MessageBoxResult.OK)
                            {
                                Environment.Exit(1);
                                return;
                            }
                        }
                    }
                };
            PluginBW.RunWorkerAsync();


            triggerManager = new TriggerManager();
            scheduledTaskManager = new ScheduledTaskManager();
        }
    }
}
