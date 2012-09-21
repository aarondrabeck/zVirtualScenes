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
            zvs.Processor.Logging.LogManager.ConfigureLogging();
            log = zvs.Processor.Logging.LogManager.GetLogger<App>();

            log.Info("Init Complete");
        }
        zvs.Processor.Logging.ILog log;
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
                            BuiltinCommand cmd = context.BuiltinCommands.FirstOrDefault(c => c.UniqueIdentifier == "RUN_SCENE");
                            if (cmd != null)
                            {
                                CommandProcessor cp = new CommandProcessor(zvsCore);
                                cp.RunBuiltinCommand(context, cmd, scene.SceneId.ToString());
                            }
                        }
                        else
                            try
                            {
                                zvsCore.Dispatcher.Invoke(new Action(() =>
                                {
                                    log.InfoFormat("Cannot find scene '{0}'", SearchQuery);
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

            //SceneRunner.onSceneRunBegin += (sender, args) =>
            //{
            //    this.Dispatcher.Invoke(new Action(() =>
            //    {
            //        if (args.Errors)
            //        {
            //            log.ErrorFormat("{0}, SceneID:{1}, SceneRunnerGUID:{2}, Source:{3}, ", args.Details, args.SceneID, args.SceneRunnerGUID, args.Source);
            //        }
            //        else
            //        {
            //            log.InfoFormat("{0}, SceneID:{1}, SceneRunnerGUID:{2}, Source:{3}, ", args.Details, args.SceneID, args.SceneRunnerGUID, args.Source);
            //        }

            //    }));
            //};

            //SceneRunner.onSceneRunComplete += (sender, args) =>
            //{
            //    this.Dispatcher.Invoke(new Action(() =>
            //    {
            //        if (args.Errors)
            //        {
            //            log.ErrorFormat("{0}, SceneID:{1}, SceneRunnerGUID:{2}, Source:{3}, ", args.Details, args.SceneID, args.SceneRunnerGUID, args.Source);
            //        }
            //        else
            //        {
            //            log.InfoFormat("{0}, SceneID:{1}, SceneRunnerGUID:{2}, Source:{3}, ", args.Details, args.SceneID, args.SceneRunnerGUID, args.Source);
            //        }

            //    }));
            //};

            //SceneRunner.onSceneReportProgress += (sender, args) =>
            //{
            //    this.Dispatcher.Invoke(new Action(() =>
            //    {
            //        log.InfoFormat("Progress:{0}, SceneID:{1}, SceneRunnerGUID:{2}, Source:{3}, ", args.Progress, args.SceneID, args.SceneRunnerGUID, args.Source);
            //    }));
            //};

            //TriggerManager.onTriggerStart += (sender, args) =>
            //{
            //    this.Dispatcher.Invoke(new Action(() =>
            //    {
            //        if (args.hasErrors)
            //        {
            //            log.ErrorFormat("{0}, TriggerID:{1}", args.Details, args.TriggerID);
            //        }
            //        else
            //        {
            //            log.InfoFormat("{0}, TriggerID:{1}", args.Details, args.TriggerID);
            //        }
            //    }));
            //};

            //ScheduledTaskManager.onScheduledTaskBegin += (sender, args) =>
            //{
            //    this.Dispatcher.Invoke(new Action(() =>
            //    {
            //        if (args.hasErrors)
            //        {
            //            log.ErrorFormat("{0}, TaskID:{1}", args.Details, args.TaskID);
            //        }
            //        else
            //        {
            //            log.InfoFormat("{0}, TaskID:{1}", args.Details, args.TaskID);
            //        }
            //    }));
            //};

            //ScheduledTaskManager.onScheduledTaskEnd += (sender, args) =>
            //{
            //    this.Dispatcher.Invoke(new Action(() =>
            //    {
            //        if (args.hasErrors)
            //        {
            //            log.ErrorFormat("{0}, TaskID:{1}", args.Details, args.TaskID);
            //        }
            //        else
            //        {
            //            log.InfoFormat("{0}, TaskID:{1}", args.Details, args.TaskID);
            //        }
            //    }));
            //};

            zvs.Processor.PluginManager.onPluginInitialized += (sender, args) =>
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    log.InfoFormat("{0}, sender:{1}", args.Details, sender);
                }));
            };

            //zvs.Processor.PluginManager.onProcessingCommandBegin += (sender, args) =>
            //{
            //    this.Dispatcher.Invoke(new Action(() =>
            //    {
            //        if (args.hasErrors)
            //        {
            //            log.ErrorFormat("{0}, CommandQueueID:{1}", args.Details, args.CommandQueueID);
            //        }
            //        else
            //        {
            //            log.InfoFormat("{0}, CommandQueueID:{1}", args.Details, args.CommandQueueID);
            //        }
            //    }));
            //};

            //zvs.Processor.PluginManager.onProcessingCommandEnd += (sender, args) =>
            //{
            //    this.Dispatcher.Invoke(new Action(() =>
            //    {
            //        if (args.hasErrors)
            //        {
            //            log.ErrorFormat("{0}, CommandQueueID:{1}", args.Details, args.CommandQueueID);
            //        }
            //        else
            //        {
            //            log.InfoFormat("{0}, CommandQueueID:{1}", args.Details, args.CommandQueueID);
            //        }
            //    }));
            //};

            base.OnStartup(e);
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            log.Fatal(sender.ToString(), (System.Exception)e.ExceptionObject);

            App app = (App)Application.Current;
            string exception = GetHostDetails + Environment.NewLine + Environment.NewLine + e.ExceptionObject.ToString();
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
                    log.InfoFormat("{0} User Interface Unloaded", Utils.ApplicationName);//, Utils.ApplicationName + " GUI");
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

            log.Info("Shutting down");

            Application.Current.Shutdown();
        }

        private void Application_Exit_1(object sender, ExitEventArgs e)
        {
            taskbarIcon.Dispose();
        }
    }
}
