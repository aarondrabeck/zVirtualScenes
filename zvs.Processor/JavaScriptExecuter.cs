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
        zvs.Processor.Logging.ILog log = zvs.Processor.Logging.LogManager.GetLogger<JavaScriptExecuter>();
        Jint.JintEngine engine = new Jint.JintEngine();

        #region Events
        public delegate void onJavaScriptExecuterEventHandler(object sender, JavaScriptExecuterEventArgs args);
        public class JavaScriptExecuterEventArgs : EventArgs
        {
            public bool Errors { get; private set; }
            public string Details { get; private set; }

            public JavaScriptExecuterEventArgs(bool errors, string details)
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

        /// <summary>
        /// Called when JavaScript executer is finished.
        /// </summary>
        public event onJavaScriptExecuterEventHandler onExecuteScriptEnd;

        /// <summary>
        /// Called when JavaScript executer begins.
        /// </summary>
        public event onJavaScriptExecuterEventHandler onExecuteScriptBegin;

        private void ReportBegin(JavaScriptExecuterEventArgs args)
        {
            if (args.Errors)
                Core.log.Error(args.Details);
            else
                Core.log.Info(args.Details);

            if (onExecuteScriptBegin != null)
                onExecuteScriptBegin(this, args);
        }

        private void ReportEnd(JavaScriptExecuterEventArgs args)
        {
            if (args.Errors)
                Core.log.Error(args.Details);
            else
                Core.log.Info(args.Details);

            if (onExecuteScriptEnd != null)
                onExecuteScriptEnd(this, args);
        }

        private void ReportProgress(string progress, params object[] args)
        {
            string msg = string.Format(progress, args);
            Core.log.Error(msg);

            if (onReportProgress != null)
                onReportProgress(this, new onReportProgressEventArgs(msg));
        }

        #endregion

        public JavaScriptExecuter(Core core)
        {
            Core = core;
        }

        //shell("wget.exe", "http://10.0.0.55/webcam/latest.jpg");
        private System.Diagnostics.Process Shell(string Path, string Arguments)
        {
            return System.Diagnostics.Process.Start(Path, Arguments);
        }

        private string MapPath(string Path)
        {
            return System.IO.Path.Combine(zvs.Processor.Utils.AppPath, Path);
        }
        public void ExecuteScript(string Script, zvsContext context)
        {
            ReportBegin(new JavaScriptExecuterEventArgs(false, "JavaScript executor started (debug mode = true)"));
            this.Context = context;
            engine.SetDebugMode(true);
            engine.DisableSecurity();
            engine.AllowClr = true;

            engine.SetParameter("zvsContext", context);

            engine.SetFunction("RunScene", new Action<double>(RunSceneJS));
            engine.SetFunction("RunScene", new Action<string>(RunSceneJS));
            engine.SetFunction("RunDeviceCommand", new Action<double, string, string>(RunDeviceCommandJS));
            engine.SetFunction("RunDeviceCommand", new Action<string, string, string>(RunDeviceCommandJS));
            engine.SetFunction("ReportProgress", new Action<string>(ReportProgressJS));
            engine.SetFunction("progress", new Action<string>(ReportProgressJS));
            engine.SetFunction("Delay", new Action<string, double, bool>(Delay));
            engine.SetFunction("error", new Action<object>(Error));
            engine.SetFunction("info", new Action<object>(Info));
            engine.SetFunction("log", new Action<object>(Info));
            engine.SetFunction("warn", new Action<object>(Warning));
            engine.SetFunction("require", new Action<string>(Require));
            engine.SetFunction("shell", new Func<string, string, System.Diagnostics.Process>(Shell));
            engine.SetFunction("mappath", new Func<string, string>(MapPath));

            // if (Trigger != null) engine.SetParameter("Trigger", this.Trigger);
            //if (Scene != null) engine.SetParameter("Scene", this.Scene);
            // engine.SetParameter("HasTrigger", (this.Trigger!=null));
            // engine.SetParameter("HasScene", (this.Scene!=null));

            try
            {
                //pull out import statements
                //import them into the engine by running each script
                //then run the engine as normal
                object result = engine.Run(Script);

                if (result != null)
                {
                    ReportEnd(new JavaScriptExecuterEventArgs(true, string.Format("JavaScript executed without errors. {0}", result.ToString())));
                    return;
                }
            }
            catch (Exception exc)
            {
                ReportEnd(new JavaScriptExecuterEventArgs(true,string.Format("JavaScript executed with errors. {0}", exc.ToString())));
                return;
            }
            ReportEnd(new JavaScriptExecuterEventArgs(false, "JavaScript executed without errors"));
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
                    log.Error("Error running script: " + Script, e);

                }
            }
        }

        //Delay("RunDeviceCommand('Office Light','Set Level', '99');", 3000);
        public void Delay(string script, double time, bool Async)
        {
            ReportProgress("Executing delayed script {0}...", Async ? "synchronously" : "asynchronously");

            AutoResetEvent mutex = new AutoResetEvent(false);
            System.Timers.Timer t = new System.Timers.Timer();
            t.Interval = time;
            t.Elapsed += (sender, e) =>
            {
                t.Stop();
                ExecuteScript(script, Context);
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
            ReportProgress(string.Format("Running JavaScript Command: RunDeviceCommand({0},{1},{2})", DeviceName, CommandName, Value));

            Device d = null;
            using (zvsContext context = new zvsContext())            
                d = context.Devices.FirstOrDefault(o => o.Name == DeviceName);

            if (d == null)
            {
                ReportProgress("Cannot find device {0}", DeviceName);
                return;
            }

            RunDeviceCommand(d.DeviceId, CommandName, Value);//TODO: ReportProgress here
        }

        //RunDeviceCommand(7,'Set Level', '99');
        public void RunDeviceCommandJS(double DeviceId, string CommandName, string Value)
        {
            ReportProgress(string.Format("Running JavaScript Command: RunDeviceCommand({0},{1},{2})", DeviceId, CommandName, Value));
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
                cp.RunDeviceCommand(context, dc, Value);
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
            ReportProgress(string.Format("Running JavaScript Command: RunScene({0})", SceneName));
            Scene s = null;
            using (zvsContext context = new zvsContext())
                s = context.Scenes.FirstOrDefault(o => o.Name == SceneName);
                
            if (s == null)
            {
                ReportProgress("Cannot find scene {0}", SceneName);
                return;
            }
            RunScene(s.SceneId);
        }

        //RunScene(1);
        public void RunSceneJS(double SceneID)
        {
            ReportProgress(string.Format("Running JavaScript Command: RunScene({0})", SceneID));
            RunScene(SceneID);
        }

        public void RunScene(double SceneID)
        {
            AutoResetEvent mutex = new AutoResetEvent(false);
            using (zvsContext context = new zvsContext())
            {
                BuiltinCommand cmd = context.BuiltinCommands.FirstOrDefault(c => c.UniqueIdentifier == "RUN_SCENE");
                if (cmd != null)
                {
                    CommandProcessor cp = new CommandProcessor(Core);
                    cp.onProcessingCommandEnd += (s, a) =>
                    {
                        mutex.Set();
                    };
                    cp.RunBuiltinCommand(context, cmd, SceneID.ToString());
                    mutex.WaitOne();
                }
            }
        }
    }
}

