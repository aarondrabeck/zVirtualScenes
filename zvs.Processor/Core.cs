using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using zvs.Processor.Triggers;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Windows;
using System.IO;
using Microsoft.Win32;
using zvs.Context;
using zvs.Entities;
using System.Threading.Tasks;
using System.Data.Entity;

namespace zvs.Processor
{
    public class Core
    {
        public AdapterManager AdapterManager;
        public PluginManager PluginManager;
        private TriggerManager TriggerManager;
        private ScheduledTaskManager ScheduledTaskManager;
        public Logging.ILog log;

        public Core()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", Utils.AppDataPath);
            AdapterManager = new Processor.AdapterManager();
            PluginManager = new Processor.PluginManager();

            //Create a instance of the logger
            log = Logging.LogManager.GetLogger<Core>();
        }

        public async Task StartAsync()
        {
            log.InfoFormat("Starting Core Processor:{0}", Utils.ApplicationName);

            #region Install Base Commands and Properties
            using (zvsContext context = new zvsContext())
            {
                var builtinCommandBuilder = new BuiltinCommandBuilder(null, this, context);

                await builtinCommandBuilder.RegisterAsync(new BuiltinCommand
                {
                    UniqueIdentifier = "REPOLL_ME",
                    Name = "Re-poll Device",
                    ArgumentType = DataType.INTEGER,
                    Description = "This will force a re-poll on an object."
                });

                await builtinCommandBuilder.RegisterAsync(new BuiltinCommand
                {
                    UniqueIdentifier = "REPOLL_ALL",
                    Name = "Re-poll all Devices",
                    ArgumentType = DataType.NONE,
                    Description = "This will force a re-poll on all objects."
                });

                await builtinCommandBuilder.RegisterAsync(new BuiltinCommand
                {
                    UniqueIdentifier = "GROUP_ON",
                    Name = "Turn Group On",
                    ArgumentType = DataType.STRING,
                    Description = "Activates a group."
                });

                await builtinCommandBuilder.RegisterAsync(new BuiltinCommand
                {
                    UniqueIdentifier = "GROUP_OFF",
                    Name = "Turn Group Off",
                    ArgumentType = DataType.STRING,
                    Description = "Deactivates a group."
                });

                await builtinCommandBuilder.RegisterAsync(new BuiltinCommand
                {
                    UniqueIdentifier = "TIMEDELAY",
                    Name = "Time Delay (sec)",
                    ArgumentType = DataType.INTEGER,
                    Description = "Pauses a execution for x seconds."
                });

                await builtinCommandBuilder.RegisterAsync(new BuiltinCommand
                {
                    UniqueIdentifier = "RUN_SCENE",
                    Name = "Run Scene",
                    ArgumentType = DataType.INTEGER,
                    Description = "Argument = SceneId"
                });
            }
            #endregion
#if (release)
            try
            {
#endif

            await AdapterManager.LoadAdaptersAsync(this);

            await PluginManager.LoadPluginsAsync(this);
#if (release)
            }
            catch (Exception ex)
            {
                Core.ProgramHasToClosePrompt(ex.Message);
            }
#endif
            TriggerManager = new TriggerManager(this);
            ScheduledTaskManager = new ScheduledTaskManager(this);
            ScheduledTaskManager.StartAsync();

            //TODO: MAKE A NICE INTERFACE FOR THIS
            //Install Program Options
            using (zvsContext context = new zvsContext())
            {
                var option = await context.ProgramOptions.FirstOrDefaultAsync(o => o.UniqueIdentifier == "LOGDIRECTION");
                if (option == null)
                {
                    var result = await ProgramOption.TryAddOrEditAsync(context, new ProgramOption()
                    {
                        UniqueIdentifier = "LOGDIRECTION",
                        Value = "Descending"
                    });

                    if (result.HasError)
                        log.Error(result.Message);
                }
            }
        }

        public static void ProgramHasToClosePrompt(string reason)
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
            if (MessageBox.Show(reason, Utils.ApplicationName, MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
            {
                WpfBugWindow.Close();
                Environment.Exit(1);
            }
        }
    }
}
