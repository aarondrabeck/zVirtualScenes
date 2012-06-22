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
using System.Diagnostics;
using System.Text;

namespace zVirtualScenesGUI
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
            AppDomain.CurrentDomain.SetData("DataDirectory", Utils.AppDataPath);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            
            string error = Utils.PreReqChecks();

            if (error != null)
            {
                Window WpfBugWindow = new Window()
                {
                    AllowsTransparency = true,
                    Background = System.Windows.Media.Brushes.Transparent,
                    WindowStyle = WindowStyle.None,
                    Top = 0,
                    Left = 0,
                    Width = 1,
                    Height = 1,
                    ShowInTaskbar = false
                };
                WpfBugWindow.Show();
                if (MessageBox.Show(error, Utils.ApplicationName, MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    WpfBugWindow.Close();
                    Environment.Exit(1);
                    return;
                }
            }

            //throw new Exception("Exception Test!");
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
                    zvsCore.Logger.WriteToLog(args.hasErrors ? Urgency.ERROR : Urgency.INFO, args.Details, "Trigger Manager");
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

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            App app = (App)Application.Current;

            string exception = GetHostDetails + Environment.NewLine + Environment.NewLine + e.ExceptionObject.ToString();
            //if (exception.Length > 4000)
            //{
            //    exception = exception.Substring(0, 4000);
            //}

            FatalErrorWindow fWindow = new FatalErrorWindow(exception);
            fWindow.ShowDialog();
        }

        

        public static string GetHostDetails
        {
            get
            {
                StringBuilder Data = new StringBuilder();
                Data.AppendLine(string.Format("OSVersion: {0}", System.Environment.OSVersion));
                Data.AppendLine(string.Format("Is64BitOperatingSystem: {0}", System.Environment.Is64BitOperatingSystem));
                Data.AppendLine(string.Format("MachineName: {0}", System.Environment.MachineName));
                Data.AppendLine(string.Format("UserDomainName: {0}", System.Environment.UserDomainName));
                Data.AppendLine(string.Format("UserName: {0}", System.Environment.UserName));
                Data.AppendLine(string.Format("Version: {0}", System.Environment.Version));
                return Data.ToString();
            }
        }

        bool isLoading = false;
        public void ShowzvsWindow()
        {
            if (zvsWindow == null || !isLoading)
            {
                isLoading = true;
                zvsWindow = new zvsMainWindow();
                zvsWindow.Closed += (a, s) =>
                {
                    zvsWindow = null;
                    zvsCore.Logger.WriteToLog(Urgency.INFO, string.Format("{0} User Interface Unloaded", Utils.ApplicationName), Utils.ApplicationName + " GUI");
                    isLoading = false;
                };
                zvsWindow.Show();
            }
            else
            {
                zvsWindow.Activate();
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
