using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.IO;
using zVirtualScenesModel;
using zVirtualScenes;
using System.ComponentModel;
using zVirtualScenes.Triggers;

namespace zVirtualScenes_WPF
{
    /// <summary>
    /// interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public Core zvsCore;
        public ZVSTaskbarIcon taskbarIcon;
        public bool isShuttingDown = false;

        protected override void OnStartup(StartupEventArgs e)
        {
            //Initilize the core
            zvsCore = new Core(this.Dispatcher);

            TriggerManager.onTriggerStart += (sender, args) =>
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    zvsCore.Logger.WriteToLog(args.hasErrors ? Urgency.ERROR : Urgency.INFO, args.Details,"Trigger Manager");
                }));
            };

            ScheduledTaskManager.onScheduledTaskBegin += (sender, args) =>
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    zvsCore.Logger.WriteToLog(args.hasErrors ? Urgency.ERROR : Urgency.INFO, args.Details, "Scheduled Task Manager");
                }));
            };

            ScheduledTaskManager.onScheduledTaskEnd += (sender, args) =>
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    zvsCore.Logger.WriteToLog(args.hasErrors ? Urgency.ERROR : Urgency.INFO, args.Details, "Scheduled Task Manager");
                }));
            };

            zVirtualScenes.PluginManager.onPluginInitialized += (sender, args) =>
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    zvsCore.Logger.WriteToLog(Urgency.INFO, args.Details, "Plug-in Manager");
                }));
            };

            zVirtualScenes.PluginManager.onProcessingCommandBegin += (sender, args) =>
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    zvsCore.Logger.WriteToLog(args.hasErrors ? Urgency.ERROR : Urgency.INFO, args.Details, "Plug-in Manager");
                }));
            };

            zVirtualScenes.PluginManager.onProcessingCommandEnd += (sender, args) =>
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    zvsCore.Logger.WriteToLog(args.hasErrors ? Urgency.ERROR : Urgency.INFO, args.Details, "Plug-in Manager");
                }));
            };

            //Create taskbar Icon
            taskbarIcon = new ZVSTaskbarIcon();

            base.OnStartup(e);
        }

        public void ShowMainWindow()
        {
            MainWindow mainWindown = (MainWindow)Application.Current.MainWindow;
            if (mainWindown != null)
            {
                mainWindown.Show();

                //if we have to change the state, listen for when that is complete to activate the window.
                if (mainWindown.WindowState == WindowState.Minimized)
                {
                    EventHandler handler = null;
                    handler = (sender, args) =>
                    {
                        mainWindown.StateChanged -= handler;

                        //ACTIVATE TO BRING IT TO FRONT
                        mainWindown.Activate();
                    };
                    mainWindown.StateChanged += handler;
                    mainWindown.WindowState = mainWindown.lastOpenedWindowState;
                }
                else
                {
                    mainWindown.Activate();
                }
            }
        }

        public void ShutdownZVS()
        {
             Application.Current.Shutdown(); 
        }
               
        private void Application_Exit_1(object sender, ExitEventArgs e)
        {
            taskbarIcon.Dispose();
        }
    }
}
