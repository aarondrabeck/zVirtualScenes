using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using zvs.Entities;
using System.Data.Entity;

namespace zvs.Processor
{
    public class JavaScriptExecuter
    {
        private Core Core;
        private zvsContext Context;
        private int id;
        zvs.Processor.Logging.ILog log = zvs.Processor.Logging.LogManager.GetLogger<JavaScriptExecuter>();
        Jint.JintEngine engine = new Jint.JintEngine();
        public object Sender { get; private set; }

        #region Events
        public class JavaScriptResult : EventArgs
        {
            public bool Errors { get; private set; }
            public string Details { get; private set; }

            public JavaScriptResult(bool errors, string details)
            {
                this.Errors = errors;
                this.Details = details;
            }
        }

        public delegate void onReportProgressEventHandler(object sender, onReportProgressEventArgs e);
        public class onReportProgressEventArgs : EventArgs
        {
            public string Progress { get; private set; }

            public onReportProgressEventArgs(string progress)
            {
                this.Progress = progress;
            }
        }

        public event onReportProgressEventHandler onReportProgress = delegate { };

        private void ReportProgress(string progress, params object[] args)
        {
            onReportProgress(this, new onReportProgressEventArgs(string.Format(progress, args)));
        }

        #endregion

        public JavaScriptExecuter(object sender, Core core)
        {
            Core = core;
            Sender = sender;

            engine.Step += (s, info) =>
            {
                ReportProgress("JSE{0}:{1} '{2}'",
                    id,
                    info.CurrentStatement.Source.Start.Line,
                    info.CurrentStatement.Source.Code.Replace(Environment.NewLine, ""));
            };
            var random = new Random();
            id = random.Next(1, 100);

        }
        public static bool JavascriptDebugEnabled { get; set; }

        //shell("wget.exe", "http://10.0.0.55/webcam/latest.jpg");
        private System.Diagnostics.Process Shell(string Path, string Arguments)
        {
            return System.Diagnostics.Process.Start(Path, Arguments);
        }

        //private System.Diagnostics.Process Shell(string Path)
        //{
        //   return System.Diagnostics.Process.Start(Path);
        //}

        private string MapPath(string Path)
        {
            return System.IO.Path.Combine(zvs.Processor.Utils.AppPath, Path);
        }
        public async Task<JavaScriptResult> ExecuteScriptAsync(string Script, zvsContext context)
        {
            this.Context = context;
            engine.SetDebugMode(JavascriptDebugEnabled);
            engine.DisableSecurity();
            engine.AllowClr = true;

            engine.SetParameter("zvsContext", context);

            engine.SetFunction("runScene", new Action<double>(RunSceneJS));
            engine.SetFunction("runScene", new Action<string>(RunSceneJS));
            //engine.SetFunction("runDeviceCommand", new Action<double, string, string>(RunDeviceCommandJS));
            //engine.SetFunction("runDeviceCommand", new Action<double, int, string>(RunDeviceCommandJS));
            //engine.SetFunction("runDeviceCommand", new Action<double, double, string>(RunDeviceCommandJS));
            //engine.SetFunction("runDeviceCommand", new Action<int, double, string>(RunDeviceCommandJS));
            //engine.SetFunction("runDeviceCommand", new Action<string, double, string>(RunDeviceCommandJS));

            engine.SetFunction("runDeviceNameCommandName", new Action<string, string, string>(RunDeviceNameCommandName));
            engine.SetFunction("runDeviceNameCommandId", new Action<string, double, string>(RunDeviceNameCommandId));
            engine.SetFunction("runDeviceIdCommandId", new Action<double, double, string>(RunDeviceIdCommandId));

            engine.SetFunction("reportProgress", new Action<string>(ReportProgressJS));
            engine.SetFunction("progress", new Action<string>(ReportProgressJS));
            engine.SetFunction("delay", new Action<string, double, bool>(Delay));
            engine.SetFunction("error", new Action<object>(Error));
            engine.SetFunction("info", new Action<object>(Info));
            engine.SetFunction("log", new Action<object>(Info));
            engine.SetFunction("warn", new Action<object>(Warning));
            engine.SetFunction("require", new Action<string>(Require));
            engine.SetFunction("shell", new Func<string, string, System.Diagnostics.Process>(Shell));
            //engine.SetFunction("shell", new Func<string, System.Diagnostics.Process>(Shell));
            engine.SetFunction("mappath", new Func<string, string>(MapPath));

            //include a default value so script can be tested
            engine.SetParameter("senderType", null);
            if (Sender != null)
                engine.SetParameter("senderType", Sender.GetType().Name);

            engine.SetParameter("senderObject", Sender);


            try
            {
                //pull out import statements
                //import them into the engine by running each script
                //then run the engine as normal

                var result = await Task<object>.Factory.StartNew(() =>
                {
                    return engine.Run(Script);
                });

                if (result != null)
                    return new JavaScriptResult(true, string.Format("JSE{0} Finished without errors. {1}", id, result.ToString()));
            }
            catch (Exception exc)
            {
                return new JavaScriptResult(true, string.Format("JSE{0} Finished with errors. {1}", id, exc.Message));
            }
            return new JavaScriptResult(false, string.Format("JSE{0} Finished without errors.", id));
        }

        public void Require(string Script)
        {
            var path = Script;
            if (!System.IO.File.Exists(path))
            {
                path = string.Format("..\\scripts\\{0}", Script);
                if (System.IO.File.Exists(path))
                {
                    Script = path;
                }
                else
                {
                    path = string.Format("scripts\\{0}", Script);
                    if (System.IO.File.Exists(path)) Script = path;
                }
            }
            if (System.IO.File.Exists(Script))
            {
                var s = System.IO.File.ReadAllText(Script);
                try
                {
                    engine.Run(s);
                }
                catch (Exception e)
                {
                    log.Error(string.Format("JSE{0} Error running script: {1}", id, Script), e);

                }
            }
        }

        //delay("RunDeviceCommand('Office Light','Set Level', '99');", 3000);
        public async void Delay(string script, double time, bool Async)
        {
            var mutex = new AutoResetEvent(false);
            await Task.Delay((int)time);

            var je = new JavaScriptExecuter(Sender, Core);
            je.onReportProgress += (s, a) =>
            {
                Core.log.Info(a.Progress);
            };

            // invoked on the ThreadPool, where there won’t be a SynchronizationContext
            var result = await je.ExecuteScriptAsync(script, Context);
            Core.log.Info(result.Details);

            mutex.Set();

            if (!Async)
                mutex.WaitOne();
        }

        //ReportProgress("Hello World!")
        public void ReportProgressJS(string progress)
        {
            ReportProgress(progress);
        }

        //RunDeviceCommand('Office Light','Set Level', '99');
        public async void RunDeviceNameCommandName(string DeviceName, string CommandName, string Value)
        {
            Device d = null;
            using (var context = new zvsContext())
                d = await context.Devices.FirstOrDefaultAsync(o => o.Name == DeviceName);

            if (d == null)
            {
                ReportProgress("JSE{0} Warning cannot find device {1}", id, DeviceName);
                return;
            }

            RunDeviceCommand(d.Id, CommandName, Value);//TODO: ReportProgress here
        }
        public async void RunDeviceNameCommandId(string DeviceName, double CommandId, string Value)
        {
            Device d = null;
            using (var context = new zvsContext())
                d = await context.Devices.FirstOrDefaultAsync(o => o.Name == DeviceName);

            if (d == null)
            {
                ReportProgress("JSE{0} Warning cannot find device {1}", id, DeviceName);
                return;
            }

            RunDeviceCommand(d.Id, CommandId, Value);//TODO: ReportProgress here
        }


        private async void RunDeviceIdCommandId(double DeviceId, double CommandID, string Value)
        {
            RunDeviceCommand(DeviceId, CommandID, Value);
        }

        private async void RunDeviceCommand(double DeviceId, double CommandID, string Value)
        {
            var did = Convert.ToInt32(DeviceId);
            var cid = Convert.ToInt32(CommandID);
            using (var context = new zvsContext())
            {
                var dc = await context.DeviceCommands.FirstOrDefaultAsync(o => o.Id == cid && o.DeviceId == did);
                if (dc == null)
                {
                    ReportProgress("Cannot find device command '{0}'", CommandID);
                    return;
                }

                var cp = new CommandProcessor(Core);
                // invoked on the ThreadPool, where there won’t be a SynchronizationContext
                var result = await cp.RunCommandAsync(this, dc, Value);
            }
        }

        private async void RunDeviceCommand(double DeviceId, string CommandName, string Value)
        {
            var dId = Convert.ToInt32(DeviceId);
            using (var context = new zvsContext())
            {
                var dc = await context.DeviceCommands.FirstOrDefaultAsync(o => o.Name == CommandName && o.DeviceId == dId);
                if (dc == null)
                {
                    ReportProgress("Cannot find device command '{0}'", CommandName);
                    return;
                }

                var cp = new CommandProcessor(Core);
                // invoked on the ThreadPool, where there won’t be a SynchronizationContext
                var result = await cp.RunCommandAsync(this, dc, Value);
            }
        }

        public void Error(object Message)
        {
            log.Error(Message);
        }

        public void Info(object Message)
        {
            log.Info(Message);
        }

        public void Warning(object Message)
        {
            log.Warn(Message);
        }

        //RunScene("Energy Save");
        public async void RunSceneJS(string SceneName)
        {
            Scene s = null;
            using (var context = new zvsContext())
                s = await context.Scenes.FirstOrDefaultAsync(o => o.Name == SceneName);

            if (s == null)
            {
                ReportProgress("JSE{0} Warning cannot find scene {1}", id, SceneName);
                return;
            }

            // invoked on the ThreadPool, where there won’t be a SynchronizationContext
            var result = await RunSceneAsync(s.Id);
        }

        //RunScene(1);
        public async void RunSceneJS(double SceneID)
        {
            // invoked on the ThreadPool, where there won’t be a SynchronizationContext
            var result = await RunSceneAsync(SceneID);
        }

        public async Task<CommandProcessorResult> RunSceneAsync(double SceneID)
        {
            using (var context = new zvsContext())
            {
                var cmd = context.BuiltinCommands.Single(c => c.UniqueIdentifier == "RUN_SCENE");
                var cp = new CommandProcessor(Core);
                return await cp.RunCommandAsync(this, cmd, SceneID.ToString());
            }
        }
    }
}

