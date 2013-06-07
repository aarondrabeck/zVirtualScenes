using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using zvs.Entities;

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
        private string callerName;


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

        public delegate void onReportProgressEventHandler(object sender, onReportProgressEventArgs args);
        public class onReportProgressEventArgs : EventArgs
        {
            public string Progress { get; private set; }

            public onReportProgressEventArgs(string progress)
            {
                this.Progress = progress;
            }
        }

        public event onReportProgressEventHandler onReportProgress;

        private void ReportProgress(string progress, params object[] args)
        {
            if (onReportProgress != null)
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
            Random random = new Random();
            id = random.Next(1, 100);

        }

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
            engine.SetDebugMode(true);
            engine.DisableSecurity();
            engine.AllowClr = true;

            engine.SetParameter("zvsContext", context);

            engine.SetFunction("runScene", new Action<double>(RunSceneJS));
            engine.SetFunction("runScene", new Action<string>(RunSceneJS));
            engine.SetFunction("runDeviceCommand", new Action<double, string, string>(RunDeviceCommandJS));
            engine.SetFunction("runDeviceCommand", new Action<string, string, string>(RunDeviceCommandJS));
            engine.SetFunction("reportProgress", new Action<string>(ReportProgressJS));
            engine.SetFunction("progress", new Action<string>(ReportProgressJS));
            engine.SetFunction("Delay", new Action<string, double, bool>(Delay));
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

                object result = await Task<object>.Factory.StartNew(() =>
                {
                    return engine.Run(Script);
                });

                if (result != null)
                    return new JavaScriptResult(true, string.Format("JSE{0} Finished without errors. {1}", id, result.ToString()));
            }
            catch (Exception exc)
            {
                return new JavaScriptResult(true, string.Format("JSE{0} Finished with errors. {1}", id, exc.ToString()));
            }
            return new JavaScriptResult(false, string.Format("JSE{0} Finished without errors.", id));
        }

        public void Require(string Script)
        {
            string path = Script;
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
                string s = System.IO.File.ReadAllText(Script);
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

        //Delay("RunDeviceCommand('Office Light','Set Level', '99');", 3000);
        public void Delay(string script, double time, bool Async)
        {
            AutoResetEvent mutex = new AutoResetEvent(false);
            System.Timers.Timer t = new System.Timers.Timer();
            t.Interval = time;
            t.Elapsed += (sender, e) =>
            {
                t.Stop();
                JavaScriptExecuter je = new JavaScriptExecuter(Sender, Core);
                je.onReportProgress += (s, a) =>
                {
                    Core.log.Info(a.Progress);
                };

                // invoked on the ThreadPool, where there won’t be a SynchronizationContext
                JavaScriptResult result = Task.Run(() => je.ExecuteScriptAsync(script, Context)).Result;
                Core.log.Info(result.Details);

                mutex.Set();
                t.Dispose();
            };
            t.Start();

            if (!Async)
                mutex.WaitOne();
        }

        //ReportProgress("Hello World!")
        public void ReportProgressJS(string progress)
        {
            ReportProgress(progress);
        }

        //RunDeviceCommand('Office Light','Set Level', '99');
        public void RunDeviceCommandJS(string DeviceName, string CommandName, string Value)
        {
            Device d = null;
            using (zvsContext context = new zvsContext())
                d = context.Devices.FirstOrDefault(o => o.Name == DeviceName);

            if (d == null)
            {
                ReportProgress("JSE{0} Warning cannot find device {1}", id, DeviceName);
                return;
            }

            RunDeviceCommand(d.Id, CommandName, Value);//TODO: ReportProgress here
        }

        //RunDeviceCommand(7,'Set Level', '99');
        public void RunDeviceCommandJS(double DeviceId, string CommandName, string Value)
        {
            RunDeviceCommand(DeviceId, CommandName, Value);
        }

        private void RunDeviceCommand(double DeviceId, string CommandName, string Value)
        {
            int dId = Convert.ToInt32(DeviceId);
            using (zvsContext context = new zvsContext())
            {
                Device device = context.Devices.Find(dId);
                if (device == null)
                {
                    ReportProgress("Cannot find device with DeviceId of {0}", dId);
                    return;
                }

                DeviceCommand dc = device.Commands.FirstOrDefault(o => o.Name == CommandName);
                if (dc == null)
                {
                    ReportProgress("Cannot find device command '{0}'", CommandName);
                    return;
                }

                CommandProcessor cp = new CommandProcessor(Core);
                // invoked on the ThreadPool, where there won’t be a SynchronizationContext
                CommandProcessorResult result = Task.Run(() => cp.RunCommandAsync(this, dc.Id, Value)).Result;
            }
        }

        public void Error(object Message)
        {
            log.Error(Message);
        }

        public void Info(object Message)
        {
            if (Message != null)
            {
                log.Info(Message);
                ReportProgressJS(Message.ToString());
            }
        }

        public void Warning(object Message)
        {
            log.Warn(Message);
        }

        //RunScene("Energy Save");
        public void RunSceneJS(string SceneName)
        {
            Scene s = null;
            using (zvsContext context = new zvsContext())
                s = context.Scenes.FirstOrDefault(o => o.Name == SceneName);

            if (s == null)
            {
                ReportProgress("JSE{0} Warning cannot find scene {1}",id, SceneName);
                return;
            }

            // invoked on the ThreadPool, where there won’t be a SynchronizationContext
            CommandProcessorResult result =  Task.Run(() => RunSceneAsync(s.Id)).Result;
        }

        //RunScene(1);
        public void RunSceneJS(double SceneID)
        {
            // invoked on the ThreadPool, where there won’t be a SynchronizationContext
            CommandProcessorResult result = Task.Run(() => RunSceneAsync(SceneID)).Result;
        }

        public async Task<CommandProcessorResult> RunSceneAsync(double SceneID)
        {
            using (zvsContext context = new zvsContext())
            {
                BuiltinCommand cmd = context.BuiltinCommands.Single(c => c.UniqueIdentifier == "RUN_SCENE");
                CommandProcessor cp = new CommandProcessor(Core);
                return await cp.RunCommandAsync(this, cmd.Id, SceneID.ToString());
            }
        }
    }
}

