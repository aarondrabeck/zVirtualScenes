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
using System.Threading.Tasks;
using System.Data.Entity;
using zvs.Context.Migrations;
using System.Data.Entity.Migrations;

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
        private System.Threading.Mutex zvsMutex = null;
        private zvs.Processor.Logging.ILog log;

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

        public async Task<bool> SignalExternalCommandLineArgs(IList<string> args)
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
                            scene = await context.Scenes.FirstOrDefaultAsync(s => s.Id == SceneId);
                        else if (SearchQuery != null)
                            scene = await context.Scenes.FirstOrDefaultAsync(s => s.Name.ToLower().Equals(SearchQuery));

                        if (scene != null)
                        {
                            BuiltinCommand cmd = await context.BuiltinCommands.FirstOrDefaultAsync(c => c.UniqueIdentifier == "RUN_SCENE");
                            if (cmd != null)
                            {
                                CommandProcessor cp = new CommandProcessor(zvsCore);
                                await cp.RunCommandAsync(this, cmd, scene.Id.ToString());
                            }
                        }
                        else
                            log.InfoFormat("Cannot find scene '{0}'", SearchQuery);
                    }
                }
            }
            return true;
        }

        protected async override void OnStartup(StartupEventArgs e)
        {
            SplashScreen splashscreen = new SplashScreen();
            splashscreen.SetLoadingTextFormat("Starting {0}", Utils.ApplicationNameAndVersion);
            splashscreen.Show();
            await Task.Delay(10);

#if DEBUG
            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif

#if (RELEASE)
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
#endif

            #region Create Logger
            zvs.Processor.Logging.LogManager.ConfigureLogging();
            log = zvs.Processor.Logging.LogManager.GetLogger<App>();

            log.InfoFormat("Init Complete ({0})", (Utils.DebugMode ? "Debug Mode" : "Release Mode"));
#if DEBUG
            log.Info("--------------DUMPING ENVIRONMENT--------------");
            log.InfoFormat("AppDataPath:{0}", Utils.AppDataPath);
            log.InfoFormat("AppPath:{0}", Utils.AppPath);
            log.InfoFormat("ApplicationNameAndVersion:{0}", Utils.ApplicationNameAndVersion);
            log.InfoFormat("ApplicationVersionLong:{0}", Utils.ApplicationVersionLong);
            log.InfoFormat("HasDotNet45:{0}", Utils.HasDotNet45());
            log.InfoFormat("HasSQLCE4:{0}", Utils.HasSQLCE4());
            log.InfoFormat("CommandLine:{0}", System.Environment.CommandLine);
            log.InfoFormat("CurrentDirectory:{0}", System.Environment.CurrentDirectory);
            log.InfoFormat("Is64BitOperatingSystem:{0}", System.Environment.Is64BitOperatingSystem);
            log.InfoFormat("Is64BitProcess:{0}", System.Environment.Is64BitProcess);
            log.InfoFormat("MachineName:{0}", System.Environment.MachineName);
            log.InfoFormat("OSVersion:{0}", System.Environment.OSVersion);
            log.InfoFormat("ProcessorCount:{0}", System.Environment.ProcessorCount);
            log.InfoFormat("UserDomainName:{0}", System.Environment.UserDomainName);
            log.InfoFormat("UserInteractive:{0}", System.Environment.UserInteractive);
            log.InfoFormat("UserName:{0}", System.Environment.UserName);
            log.InfoFormat("Version:{0}", System.Environment.Version);
            log.InfoFormat("WorkingSet:{0}", System.Environment.WorkingSet);
            log.Info("--------------/DUMPING ENVIRONMENT--------------");
#endif
            AppDomain.CurrentDomain.SetData("DataDirectory", Utils.AppDataPath);
            #endregion

            #region Checking for other running instances
            await Task.Delay(10);
            splashscreen.SetLoadingTextFormat("Checking for other running instances");
            await Task.Delay(10);

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
            #endregion

            #region Check for .Net Framework 4.5
            await Task.Delay(10);
            splashscreen.SetLoadingTextFormat("Checking for .Net framework 4.5");
            await Task.Delay(10);

            if (!Utils.HasDotNet45())
            {
                Core.ProgramHasToClosePrompt(string.Format("Microsoft .NET Framework 4.5 Full/Extended is required to run {0}. \r\n\r\nPlease install Microsoft .NET Framework 4.5 and re-launch the application.",
                    Utils.ApplicationName));
            }
            #endregion

            #region Checking for Microsoft® SQL Server® Compact 4.0 SP1
            await Task.Delay(10);
            splashscreen.SetLoadingTextFormat("Checking for Microsoft® SQL Server® Compact 4.0 SP1");
            await Task.Delay(10);

            if (!Utils.HasSQLCE4())
            {
                Core.ProgramHasToClosePrompt(string.Format("Microsoft® SQL Server® Compact 4.0 SP1 is required to run {0}. \r\n\r\nPlease install Microsoft® SQL Server® Compact 4.0 SP1 and re-launch the application.",
                    Utils.ApplicationName));
            }
            #endregion

            #region Initializing and upgrading local database
            await Task.Delay(10);
            splashscreen.SetLoadingTextFormat("Initializing and migrating database");
            await Task.Delay(10);

            await Task.Run(() => 
            { 
                using (zvsContext context = new zvsContext())
                {
                    var configuration = new Configuration();
                    var migrator = new DbMigrator(configuration);

                    migrator.Update();
                    context.Database.Initialize(true);
                }
            });
            #endregion

            //TODO: Check for VCRedist

            #region Start Core Services
            await Task.Delay(10);
            splashscreen.SetLoadingTextFormat("Starting core services");
            await Task.Delay(10);

            //Initialize the core
            zvsCore = new Core();
            await Task.Run(() =>  
            { 
                zvsCore.StartAsync();            
            });
            #endregion

            //Create taskbar Icon 
            taskbarIcon = new ZVSTaskbarIcon();
            taskbarIcon.ShowBalloonTip(Utils.ApplicationName, Utils.ApplicationNameAndVersion + " started", 3000, System.Windows.Forms.ToolTipIcon.Info);

            //close Splash Screen
            splashscreen.Close();

#if DEBUG
            sw.Stop();
            Debug.WriteLine("App Startup initialized in {0}", sw.Elapsed.ToString() as object);
#endif

            base.OnStartup(e);
        }

        private async void RefreshTriggerDescripitions()
        {
            using (zvsContext context = new zvsContext())
            {
                var triggers = await context.DeviceValueTriggers
                    .Include(o => o.DeviceValue)
                    .Include(o => o.DeviceValue.Device)
                    .ToListAsync();

                foreach (var trigger in triggers)
                    trigger.SetDescription(context);

                var result = await context.TrySaveChangesAsync();
                if (result.HasError)
                    ((App)App.Current).zvsCore.log.Error(result.Message);
            }
        }

        private async void RefreshCommandDescripitions()
        {
            using (zvsContext context = new zvsContext())
            {
                var storedCommands = await context.StoredCommands
                    .Include(o => o.Command)
                    .ToListAsync();

                foreach (var storedCommand in storedCommands)
                {
                    await storedCommand.SetTargetObjectNameAsync(context);
                    await storedCommand.SetDescriptionAsync(context);
                }

                var result = await context.TrySaveChangesAsync();
                if (result.HasError)
                    ((App)App.Current).zvsCore.log.Error(result.Message);
            }
        }

#if (RELEASE)
        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            log.Fatal(sender.ToString(), (System.Exception)e.ExceptionObject);

            App app = (App)Application.Current;
            string exception = GetHostDetails + Environment.NewLine + Environment.NewLine + e.ExceptionObject.ToString();
            FatalErrorWindow fWindow = new FatalErrorWindow(exception);
            fWindow.ShowDialog();
        }
#endif

        bool MainWindowCreated = false;

        public async void ShowzvsWindow()
        {
            if (zvsWindow == null || !MainWindowCreated)
            {
#if DEBUG
                Stopwatch sw = new Stopwatch();
                sw.Start();
#endif

                MainWindowCreated = true;

                SplashScreen splashscreen = new SplashScreen();
                splashscreen.SetLoadingText("Loading user interface");
                splashscreen.Show();

                //TODO: REMOVE THE NEED FOR STATIC DESCIPTIONS  DB DESIGN? - CHANGE UI??
                //RefreshCommandDescripitions();
                //RefreshTriggerDescripitions();

                await Task.Delay(10);
                splashscreen.SetLoadingTextFormat("Loading user interface settings");
                await Task.Delay(10);

                zvsWindow = new zvsMainWindow();
                zvsWindow.Loaded += (a, s) =>
                {
                    splashscreen.Close();
                };
                zvsWindow.Closed += (a, s) =>
                {
                    zvsWindow = null;
                    log.InfoFormat("{0} User Interface Unloaded", Utils.ApplicationName); //, Utils.ApplicationName + " GUI");
                    MainWindowCreated = false;

                };
                zvsWindow.Show();
#if DEBUG
                sw.Stop();
                Debug.WriteLine("ZVS window created in {0}", sw.Elapsed.ToString() as object);
#endif
            }

            zvsWindow.Activate();
        }

        public void ShutdownZVS()
        {
            if (zvsMutex != null)
                zvsMutex.ReleaseMutex();

            log.Info("Shutting down");

            zvs.Processor.Logging.EventedLog.Enabled = false;

            Application.Current.Shutdown();
        }

        private void Application_Exit_1(object sender, ExitEventArgs e)
        {
            taskbarIcon.Dispose();
        }
    }
}
