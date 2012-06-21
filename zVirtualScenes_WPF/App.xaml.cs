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
        public zvsMainWindow zvsWindow = null;
        public Window firstWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            //Initilize the core
            zvsCore = new Core(this.Dispatcher);

            //This is a placeholder for a main window. Application.Current.MainWindow
            firstWindow = new Window();

            //Create taskbar Icon 
            taskbarIcon = new ZVSTaskbarIcon();

            //for (int i = 0; i < 50; i++)
            //{
            //    Test main = new Test();
            //    main.Show();
            //    main.Close();
            //    GC.Collect(3);

            //    GC.WaitForPendingFinalizers();

            //    GC.Collect(3);
            //}


            taskbarIcon.ShowBalloonTip(Utils.ApplicationName, Utils.ApplicationNameAndVersion + " started", 3000, System.Windows.Forms.ToolTipIcon.Info);

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

            base.OnStartup(e);
        }

        public void ShowzvsWindow()
        {
            if (zvsWindow == null)
            {
                zvsWindow = new zvsMainWindow();
                zvsWindow.Closed += (a, s) =>
                {
                    zvsWindow = null;
                };
                zvsWindow.Show();
            }
            else
            {
                zvsWindow.Show();

                //if we have to change the state, listen for when that is complete to activate the window.
                if (zvsWindow.WindowState == WindowState.Minimized)
                {
                    EventHandler handler = null;
                    handler = (sender, args) =>
                    {
                        zvsWindow.StateChanged -= handler;

                        //ACTIVATE TO BRING IT TO FRONT
                        zvsWindow.Activate();
                    };
                    zvsWindow.StateChanged += handler;
                    zvsWindow.WindowState = zvsWindow.lastOpenedWindowState;
                }
                else
                {
                    zvsWindow.Activate();
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
