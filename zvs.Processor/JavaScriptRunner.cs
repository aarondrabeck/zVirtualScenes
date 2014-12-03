using System;
using System.Threading;
using System.Threading.Tasks;
using zvs.DataModel;
using System.Data.Entity;

namespace zvs.Processor
{
    public class JavaScriptRunner
    {
        private IEntityContextConnection EntityContextConnection { get; set; }
        private IFeedback<LogEntry> Log { get; set; }
        private Jint.Engine JintEngine { get; set; }
        private ICommandProcessor CommandProcessor { get; set; }

        public static bool JavascriptDebugEnabled { get; set; }

        public JavaScriptRunner(IFeedback<LogEntry> log, ICommandProcessor commandProcessor, IEntityContextConnection entityContextConnection)
        {
            if (log == null)
                throw new ArgumentNullException("log");

            if (commandProcessor == null)
                throw new ArgumentNullException("commandProcessor");

            if (entityContextConnection == null)
                throw new ArgumentNullException("entityContextConnection");

            Log = log;
            CommandProcessor = commandProcessor;
            EntityContextConnection = entityContextConnection;
            log.Source = "JavaScript Runner";


            JintEngine = new Jint.Engine(cfg =>
            {
                cfg.AllowClr();
                cfg.AllowDebuggerStatement(JavascriptDebugEnabled);
            });
            //JintEngine.Step += async (s, info) =>
            //{
            //    await Log.ReportInfoFormatAsync(CancellationToken.None, "{1} {2}",
            //         info.CurrentStatement.Source.Start.Line,
            //         info.CurrentStatement.Source.Code.Replace(Environment.NewLine, ""));
            //};

        }

        public async Task<Result> ExecuteScriptAsync(string script, CancellationToken cancellationToken)
        {
            using (var context = new ZvsContext(EntityContextConnection))
            {
                JintEngine.SetValue("zvsContext", context);

                JintEngine.SetValue("logInfo", new Action<object>(LogInfo));
                JintEngine.SetValue("logWarn", new Action<object>(LogWarn));
                JintEngine.SetValue("logError", new Action<object>(LogError));
                JintEngine.SetValue("setTimeout", new Action<string, double>(SetTimeout));
                JintEngine.SetValue("shell", new Func<string, string, System.Diagnostics.Process>(Shell));

                //JintEngine.SetFunction("runScene", new Action<double>(RunSceneJS));
                //JintEngine.SetFunction("runScene", new Action<string>(RunSceneJS));
                JintEngine.SetValue("runDeviceNameCommandName", new Func<string, string, string, Result>(RunDeviceNameCommandName));
                // JintEngine.SetFunction("runDeviceNameCommandId", new Action<string, double, string>(RunDeviceNameCommandId));
                //  JintEngine.SetFunction("runDeviceIdCommandId", new Action<double, double, string>(RunDeviceIdCommandId));

                //  JintEngine.SetFunction("error", new Action<object>(Error));

                JintEngine.SetValue("require", new Action<string>(Require));

                JintEngine.SetValue("mappath", new Func<string, string>(MapPath));
                try
                {
                    //pull out import statements
                    //import them into the JintEngine by running each script
                    //then run the JintEngine as normal

                    var result = await Task.Run(() => JintEngine.Execute(script), cancellationToken);
                    return Result.ReportSuccessFormat("JavaScript execution complete. {0}", result);
                }
                catch (Exception ex)
                {
                    return Result.ReportErrorFormat("JavaScript execution error. {0}", ex.Message);
                }
            }
        }

        public void LogInfo(object message)
        {
            var msg = "null";
            if (message != null)
                msg = message.ToString();

            Log.ReportInfoAsync(msg, CancellationToken.None).Wait();
        }

        public void LogWarn(object message)
        {
            var msg = "null";
            if (message != null)
                msg = message.ToString();

            Log.ReportWarningAsync(msg, CancellationToken.None).Wait();
        }

        public void LogError(object message)
        {
            var msg = "null";
            if (message != null)
                msg = message.ToString();

            Log.ReportErrorAsync(msg, CancellationToken.None).Wait();
        }

        public void SetTimeout(string function, double millisecondsDelay)
        {
            Task.Delay((int)millisecondsDelay).Wait();
            JintEngine.Execute(function);
        }

        //shell("wget.exe", "http://10.0.0.55/webcam/latest.jpg");
        private static System.Diagnostics.Process Shell(string path, string arguments)
        {
            return System.Diagnostics.Process.Start(path, arguments);
        }

        public string MapPath(string path)
        {
            return System.IO.Path.Combine(Utils.AppPath, path);
        }

        public void Require(string script)
        {
            var path = script;
            if (!System.IO.File.Exists(path))
            {
                path = string.Format("scripts\\{0}", script);
                if (!System.IO.File.Exists(path))
                    return;

                script = path;
            }

            var s = System.IO.File.ReadAllText(script);
            try
            {
                JintEngine.Execute(s);
            }
            catch (Exception e)
            {
                Log.ReportErrorFormatAsync(CancellationToken.None, "Error in required file {0}. {1}", script, e.Message).Wait();
            }
        }

        //RunDeviceCommand('Office Light','Set Level', '99');
        public Result RunDeviceNameCommandName(string deviceName, string commandName, string value)
        {
            using (var context = new ZvsContext(EntityContextConnection))
            {
                var command = context.DeviceCommands.FirstOrDefaultAsync(o => o.Name == commandName && o.Device.Name == deviceName).Result;
                return command == null ? 
                    Result.ReportErrorFormat("Cannot find a device and/or command with name {0}-{1}", deviceName, commandName) : 
                    CommandProcessor.RunCommandAsync(command.Id, value, string.Empty, CancellationToken.None).Result;
            }
        }
        //public async void RunDeviceNameCommandId(string DeviceName, double CommandId, string Value)
        //{
        //    Device d = null;
        //    using (var context = new ZvsContext())
        //        d = await context.Devices.FirstOrDefaultAsync(o => o.Name == DeviceName);

        //    if (d == null)
        //    {
        //        ReportProgress("JSE{0} Warning cannot find device {1}", id, DeviceName);
        //        return;
        //    }

        //    RunDeviceCommand(d.Id, CommandId, Value);//TODO: ReportProgress here
        //}


        //private async void RunDeviceIdCommandId(double DeviceId, double CommandID, string Value)
        //{
        //    RunDeviceCommand(DeviceId, CommandID, Value);
        //}

        //private async void RunDeviceCommand(double DeviceId, double CommandID, string Value)
        //{
        //    var did = Convert.ToInt32(DeviceId);
        //    var cid = Convert.ToInt32(CommandID);
        //    using (var context = new ZvsContext())
        //    {
        //        var dc = await context.DeviceCommands.FirstOrDefaultAsync(o => o.Id == cid && o.DeviceId == did);
        //        if (dc == null)
        //        {
        //            ReportProgress("Cannot find device command '{0}'", CommandID);
        //            return;
        //        }

        //        var cp = new CommandProcessor(ZvsEngine);
        //        // invoked on the ThreadPool, where there won’t be a SynchronizationContext
        //        var result = await cp.RunCommandAsync(this, dc, Value);
        //    }
        //}

        //private async void RunDeviceCommand(double DeviceId, string CommandName, string Value)
        //{
        //    var dId = Convert.ToInt32(DeviceId);
        //    using (var context = new ZvsContext())
        //    {
        //        var dc = await context.DeviceCommands.FirstOrDefaultAsync(o => o.Name == CommandName && o.DeviceId == dId);
        //        if (dc == null)
        //        {
        //            ReportProgress("Cannot find device command '{0}'", CommandName);
        //            return;
        //        }

        //        var cp = new CommandProcessor(ZvsEngine);
        //        // invoked on the ThreadPool, where there won’t be a SynchronizationContext
        //        var result = await cp.RunCommandAsync(this, dc, Value);
        //    }
        //}


        ////RunScene("Energy Save");
        //public async void RunSceneJS(string SceneName)
        //{
        //    Scene s = null;
        //    using (var context = new ZvsContext())
        //        s = await context.Scenes.FirstOrDefaultAsync(o => o.Name == SceneName);

        //    if (s == null)
        //    {
        //        ReportProgress("JSE{0} Warning cannot find scene {1}", id, SceneName);
        //        return;
        //    }

        //    // invoked on the ThreadPool, where there won’t be a SynchronizationContext
        //    var result = await RunSceneAsync(s.Id);
        //}

        ////RunScene(1);
        //public async void RunSceneJS(double SceneID)
        //{
        //    // invoked on the ThreadPool, where there won’t be a SynchronizationContext
        //    var result = await RunSceneAsync(SceneID);
        //}

        //public async Task<Result> RunSceneAsync(double SceneID)
        //{
        //    using (var context = new ZvsContext())
        //    {
        //        var cmd = context.BuiltinCommands.Single(c => c.UniqueIdentifier == "RUN_SCENE");
        //        var cp = new CommandProcessor(ZvsEngine);
        //        return await cp.RunCommandAsync(this, cmd, SceneID.ToString());
        //    }
        //}
    }
}

