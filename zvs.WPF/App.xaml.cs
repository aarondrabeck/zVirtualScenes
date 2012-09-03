using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.IO;
using zvs.Processor;
using System.ComponentModel;
using zvs.Processor.Triggers;
using System.Diagnostics;
using System.Text;
using Microsoft.Shell;
using System.Windows.Shell;
using System.Reflection;
using zvs.Entities;

namespace zvs.WPF
{
    /// <summary>
    /// interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
        public Core zvsCore;
        public ZVSTaskbarIcon taskbarIcon;
        public bool isShuttingDown = false;
        public zvsMainWindow zvsWindow = null;
        public Window firstWindow;
        System.Threading.Mutex zvsMutex = null;

        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance("AdvancedJumpList"))
            {
                var application = new App();
                application.Init();
                application.Run();


                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
        }

        public void Init()
        {
            this.InitializeComponent();
        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            if (args == null || args.Count == 0)
                return true;
            if ((args.Count > 2))
            {
                //the first index always contains the location of the exe so we need to check the second index
                if ((args[1].ToLowerInvariant() == "-startscene"))
                {
                    int SceneId = 0;
                    string SearchQuery = args[2].ToLower();

                    using (zvsContext context = new zvsContext())
                    {
                        Scene scene = null;
                        if (int.TryParse(SearchQuery, out SceneId))
                            scene = context.Scenes.FirstOrDefault(s => s.SceneId == SceneId);
                        else if (SearchQuery != null)
                            scene = context.Scenes.FirstOrDefault(s => s.Name.ToLower().Equals(SearchQuery));

                        if (scene != null)
                        {

                            SceneRunner sr = new SceneRunner();
                            SceneRunner.onSceneRunEventHandler startHandler = null;
                            startHandler = (s, a) =>
                            {
                                if (a.SceneRunnerGUID == sr.SceneRunnerGUID)
                                {
                                    SceneRunner.onSceneRunBegin -= startHandler;

                                    try
                                    {
                                        zvsCore.Dispatcher.Invoke(new Action(() =>
                                        {
                                            zvsCore.Logger.WriteToLog(Urgency.INFO,
                                       string.Format(a.Details),
                                       "Command Line");
                                        }));
                                    }
                                    catch { }

                                    #region LISTEN FOR ENDING
                                    SceneRunner.onSceneRunEventHandler handler = null;
                                    handler = (se, end_args) =>
                                    {
                                        if (end_args.SceneRunnerGUID == sr.SceneRunnerGUID)
                                        {
                                            SceneRunner.onSceneRunComplete -= handler;

                                            try
                                            {
                                                zvsCore.Dispatcher.Invoke(new Action(() =>
                                                {
                                                    zvsCore.Logger.WriteToLog(Urgency.INFO,
                                       string.Format(end_args.Details),
                                       "Command Line");
                                                }));
                                            }
                                            catch { }

                                        }
                                    };
                                    SceneRunner.onSceneRunComplete += handler;
                                    #endregion
                                }
                            };
                            SceneRunner.onSceneRunBegin += startHandler;
                            sr.RunScene(scene.SceneId);
                        }
                        else
                            try
                            {
                                zvsCore.Dispatcher.Invoke(new Action(() =>
                                {
                                    zvsCore.Logger.WriteToLog(Urgency.INFO,
                               string.Format("Cannot find scene '{0}'", SearchQuery),
                               "Command Line");
                                }));
                            }
                            catch { }
                    }
                }
            }
            return true;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                zvsMutex = System.Threading.Mutex.OpenExisting("zVirtualScenesGUIMutex");

                Core.ProgramHasToClosePrompt(Utils.ApplicationName + " can't start because it is already running");
            }
            catch
            {
                //the specified mutex doesn't exist, we should create it
                zvsMutex = new System.Threading.Mutex(true, "zVirtualScenesGUIMutex"); //these names need to match.
            }


            AppDomain.CurrentDomain.SetData("DataDirectory", Utils.AppDataPath);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            string error = Utils.PreReqChecks();

            if (error != null)
            {
                Core.ProgramHasToClosePrompt(error);
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

            zvs.Processor.PluginManager.onPluginInitialized += (sender, args) =>
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    zvsCore.Logger.WriteToLog(Urgency.INFO, args.Details, "Plug-in Manager");
                }));
            };

            zvs.Processor.PluginManager.onProcessingCommandBegin += (sender, args) =>
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    zvsCore.Logger.WriteToLog(args.hasErrors ? Urgency.ERROR : Urgency.INFO, args.Details, "Plug-in Manager");
                }));
            };

            zvs.Processor.PluginManager.onProcessingCommandEnd += (sender, args) =>
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
            if (zvsMutex != null)
                zvsMutex.ReleaseMutex();

            Application.Current.Shutdown();
        }

        private void Application_Exit_1(object sender, ExitEventArgs e)
        {
            taskbarIcon.Dispose();
        }
    }
}
