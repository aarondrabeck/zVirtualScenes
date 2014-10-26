using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using zvs.DataModel.Migrations;
using zvs.Processor;
using Microsoft.Shell;
using zvs.DataModel;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace zvs.WPF
{
    /// <summary>
    /// interaction logic for App.xaml
    /// </summary>
    public partial class App : ISingleInstanceApp
    {
        public CancellationTokenSource Cts { get; private set; }

        bool _isMainWindowCreated;
        public ZvsEngine ZvsEngine;
        public ZVSTaskbarIcon TaskbarIcon;
        public zvsMainWindow ZvsWindow;
        private Mutex _zvsMutex;

        [STAThread]
        public static void Main()
        {
            if (!SingleInstance<App>.InitializeAsFirstInstance("AdvancedJumpList")) return;
            
            var application = new App();
            application.Init();
            application.Run();
            // Allow single instance code to perform cleanup operations
            SingleInstance<App>.Cleanup();
        }

        public void Init()
        {
            InitializeComponent();
            Cts = new CancellationTokenSource();
        }

        public async Task<bool> SignalExternalCommandLineArgs(IList<string> args)
        {
            if (args == null || args.Count == 0)
                return true;
            if ((args.Count <= 2)) return true;
            //the first index always contains the location of the exe so we need to check the second index
            if ((args[1].ToLowerInvariant() != "-startscene")) return true;
            var searchQuery = args[2].ToLower();

            using (var context = new ZvsContext())
            {
                Scene scene;
                int sceneId;
                if (int.TryParse(searchQuery, out sceneId))
                    scene = await context.Scenes.FirstOrDefaultAsync(s => s.Id == sceneId);
                else scene = await context.Scenes.FirstOrDefaultAsync(s => s.Name.ToLower().Equals(searchQuery));

                if (scene != null)
                {
                    var cmd = await context.BuiltinCommands.FirstOrDefaultAsync(c => c.UniqueIdentifier == "RUN_SCENE");
                    if (cmd == null) return true;
                    var cp = new CommandProcessor(ZvsEngine);
                    await cp.RunCommandAsync(this, cmd, scene.Id.ToString());
                }
                else
                    await ZvsEngine.Log.ReportInfoFormatAsync(Cts.Token, "Cannot find scene '{0}'", searchQuery);
            }
            return true;
        }

        protected async override void OnStartup(StartupEventArgs e)
        {
            var databaseFeedback = new DatabaseFeedback();

            ZvsEngine = new ZvsEngine(databaseFeedback, new Processor.AdapterManager(databaseFeedback), new PluginManager(databaseFeedback), new TriggerRunner(databaseFeedback), new ScheduledTaskManager(databaseFeedback));

            var splashscreen = new SplashScreen();
            splashscreen.SetLoadingTextFormat("Starting {0}", Utils.ApplicationNameAndVersion);
            // splashscreen.Show();
            await Task.Delay(10);

#if DEBUG
            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif

#if (RELEASE)
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
#endif

            #region Create Logger

            await ZvsEngine.Log.ReportInfoFormatAsync(Cts.Token, "Init Complete ({0})", (Utils.DebugMode ? "Debug Mode" : "Release Mode"));
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
                _zvsMutex = Mutex.OpenExisting("zVirtualScenesGUIMutex");
                ProgramHasToClosePrompt(Utils.ApplicationName + " can't start because it is already running");
            }
            catch
            {
                //the specified mutex doesn't exist, we should create it
                _zvsMutex = new Mutex(true, "zVirtualScenesGUIMutex"); //these names need to match.
            }
            #endregion

            #region Check for .Net Framework 4.5
            await Task.Delay(10);
            splashscreen.SetLoadingTextFormat("Checking for .Net framework 4.5");
            await Task.Delay(10);

            if (!Utils.HasDotNet45())
            {
                ProgramHasToClosePrompt(string.Format("Microsoft .NET Framework 4.5 Full/Extended is required to run {0}. \r\n\r\nPlease install Microsoft .NET Framework 4.5 and re-launch the application.",
                    Utils.ApplicationName));
            }
            #endregion

            #region Checking for Microsoft® SQL Server® Compact 4.0 SP1
            await Task.Delay(10);
            splashscreen.SetLoadingTextFormat("Checking for Microsoft® SQL Server® Compact 4.0 SP1");
            await Task.Delay(10);

            if (!Utils.HasSQLCE4())
            {
                ProgramHasToClosePrompt(string.Format("Microsoft® SQL Server® Compact 4.0 SP1 is required to run {0}. \r\n\r\nPlease install Microsoft® SQL Server® Compact 4.0 SP1 and re-launch the application.",
                    Utils.ApplicationName));
            }
            #endregion

            #region Initializing and upgrading local database
            await Task.Delay(10);
            splashscreen.SetLoadingTextFormat("Initializing and migrating database");
            await Task.Delay(10);

            await Task.Run(() =>
            {
                using (var context = new ZvsContext())
                {
                    var configuration = new Configuration();
                    var migrator = new DbMigrator(configuration);

                    migrator.Update();
                    context.Database.Initialize(true);
                }
            });
            #endregion

            //TODO: Check for VCRedist

            #region Start zvsEngine Services
            await Task.Delay(10);
            splashscreen.SetLoadingTextFormat("Starting zvsEngine services");
            await Task.Delay(10);

            //Initialize the zvsEngine

            try
            {
                await Task.Run(() => ZvsEngine.StartAsync());
            }
            catch (Exception ex)
            {
                ProgramHasToClosePrompt(ex.Message);
            }

            #endregion

            //Create taskbar Icon 
            TaskbarIcon = new ZVSTaskbarIcon();
            TaskbarIcon.ShowBalloonTip(Utils.ApplicationName, Utils.ApplicationNameAndVersion + " started", 3000, System.Windows.Forms.ToolTipIcon.Info);

            //close Splash Screen
            //  splashscreen.Close();

#if DEBUG
            sw.Stop();
            Debug.WriteLine("App Startup initialized in {0}", sw.Elapsed.ToString() as object);
#endif

            base.OnStartup(e);
        }

        public async Task RefreshTriggerDescripitions()
        {
            using (var context = new ZvsContext())
            {
                var triggers = await context.DeviceValueTriggers
                    .Include(o => o.DeviceValue)
                    .Include(o => o.DeviceValue.Device)
                    .ToListAsync();

                foreach (var trigger in triggers)
                    trigger.SetDescription();

                var result = await context.TrySaveChangesAsync();
                if (result.HasError)
                    await ZvsEngine.Log.ReportErrorAsync(result.Message, Cts.Token);
            }
        }

        public async Task RefreshCommandDescripitions()
        {
            using (var context = new ZvsContext())
            {
                var storedCommands = await context.StoredCommands
                    .Include(o => o.Command)
                    .ToListAsync();

                foreach (var storedCommand in storedCommands)
                {
                    await storedCommand.SetTargetObjectNameAsync(context);
                    storedCommand.SetDescription(context);
                }

                var result = await context.TrySaveChangesAsync();
                if (result.HasError)
                    await ZvsEngine.Log.ReportErrorAsync(result.Message, Cts.Token);
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



        public async void ShowzvsWindow()
        {
            if (ZvsWindow == null || !_isMainWindowCreated)
            {
#if DEBUG
                Stopwatch sw = new Stopwatch();
                sw.Start();
#endif
                _isMainWindowCreated = true;

                var splashscreen = new SplashScreen();
                splashscreen.SetLoadingText("Loading user interface");
                splashscreen.Show();

                await Task.Delay(10);
                splashscreen.SetLoadingTextFormat("Loading user interface settings");
                await Task.Delay(10);

                ZvsWindow = new zvsMainWindow();
                ZvsWindow.Loaded += (a, s) => splashscreen.Close();
                ZvsWindow.Closed += async (a, s) =>
                {
                    ZvsWindow = null;
                    await ZvsEngine.Log.ReportInfoFormatAsync(Cts.Token, "{0} User Interface Unloaded", Utils.ApplicationName);
                    _isMainWindowCreated = false;

                };
                ZvsWindow.Show();
#if DEBUG
                sw.Stop();
                Debug.WriteLine("ZVS window created in {0}", sw.Elapsed.ToString() as object);
#endif
            }

            ZvsWindow.Activate();
        }

        public async Task ShutdownZvs()
        {
            if (_zvsMutex != null)
                _zvsMutex.ReleaseMutex();

            await ZvsEngine.Log.ReportInfoAsync("Shutting down", Cts.Token);
            Cts.Cancel();
            Current.Shutdown();
        }

        private void Application_Exit_1(object sender, ExitEventArgs e)
        {
            TaskbarIcon.Dispose();
        }

        private static void ProgramHasToClosePrompt(string reason)
        {

            var wpfBugWindow = new Window
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
            wpfBugWindow.Show();
            if (MessageBox.Show(reason, Utils.ApplicationName, MessageBoxButton.OK, MessageBoxImage.Error) !=
                MessageBoxResult.OK) return;

            wpfBugWindow.Close();
            Environment.Exit(1);
        }
    }
}
