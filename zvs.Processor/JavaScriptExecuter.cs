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
        private zvsContext Context;
        private string Source;

        public DeviceValueTrigger Trigger { get; set; }
        public Scene Scene { get; set; }
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
        public event onJavaScriptExecuterEventHandler onComplete;
        #endregion


        //shell("wget.exe", "http://10.0.0.55/webcam/latest.jpg");
        private System.Diagnostics.Process Shell(string Path, string Arguments)
        {
            return System.Diagnostics.Process.Start(Path, Arguments);
        }

        private string MapPath(string Path)
        {
            return System.IO.Path.Combine(zvs.Processor.Utils.AppPath, Path);
        }
        public void ExecuteScript(string Script, zvsContext context, string source)
        {
            this.Source = source;
            this.Context = context;
            engine.SetDebugMode(true);
            engine.DisableSecurity();
            engine.AllowClr = true;                     

            engine.SetParameter("zvsContext", context);
            
            engine.SetFunction("RunScene", new Action<double>(RunScene));
            engine.SetFunction("RunScene", new Action<string>(RunScene));
            engine.SetFunction("RunDeviceCommand", new Action<double, string, string>(RunDeviceCommand));
            engine.SetFunction("RunDeviceCommand", new Action<string, string, string>(RunDeviceCommand));
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

            if (Trigger != null) engine.SetParameter("Trigger", this.Trigger);
            if (Scene != null) engine.SetParameter("Scene", this.Scene);
            engine.SetParameter("HasTrigger", (this.Trigger!=null));
            engine.SetParameter("HasScene", (this.Scene!=null));


            try
            {
                //pull out import statements
                //import them into the engine by running each script
                //then run the engine as normal
                object result = engine.Run(Script);
                string entry = string.Format("Script Result:{0}, Trigger Name:{1}, Scene Name:{2}", (result == null) ? "" : result.ToString(), (Trigger == null) ? "None" : Trigger.Name, (Scene == null) ? "None" : Scene.Name);

                log.Info(entry);


                if (result != null)
                {
                    if (onComplete != null)
                        onComplete(this, new JavaScriptExecuterEventArgs(false, result.ToString()));
                    return;
                }
            }
            catch (Exception exc)
            {
                if (onComplete != null)
                    onComplete(this, new JavaScriptExecuterEventArgs(true, exc.ToString()));
                return;
            }
            if (onComplete != null)
                onComplete(this, new JavaScriptExecuterEventArgs(false, "None"));
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
                ExecuteScript(script, Context, "Test Button");
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

            if (onReportProgress != null)
                onReportProgress(this, new onReportProgressEventArgs(progress));
        }

        protected void ReportProgress(string progress, params string[] args)
        {
            if (onReportProgress != null)
                onReportProgress(this, new onReportProgressEventArgs(string.Format(progress, args)));
        }

        //RunDeviceCommand('Office Light','Set Level', '99');
        public void RunDeviceCommand(string DeviceName, string CommandName, string Value)
        {
            using (zvsContext context = new zvsContext())
            {
                Device d = context.Devices.FirstOrDefault(o => o.Name == DeviceName);
                if (d != null)
                    RunDeviceCommand(d.DeviceId, CommandName, Value);//TODO: ReportProgress here
            }
        }

        //RunDeviceCommand(7,'Set Level', '99');
        public void RunDeviceCommand(double DeviceId, string CommandName, string Value)
        {
            int dId = Convert.ToInt32(DeviceId);
            using (zvsContext context = new zvsContext())
            {
                Device device = context.Devices.Find(dId);
                if (device == null)
                    return; //TODO: ReportProgress here

                DeviceCommand dc = device.Commands.FirstOrDefault(o => o.Name == CommandName);
                if (dc == null)
                    return; //TODO: ReportProgress here


                //TODO: ReportProgress here
                dc.Run(context, Value);
            }
        }

        public void RunScene(string SceneName)
        {
            using (zvsContext context = new zvsContext())
            {
                int s = (from S in context.Scenes
                         where S.Name == SceneName
                         select S.SceneId).FirstOrDefault();

                if (s != null && s > 0)
                {
                    RunScene(s);
                }
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
        //RunScene(1);
        public void RunScene(double SceneID)
        {
            ReportProgress(string.Format("Running JavaScript Command: RunScene({0})", SceneID));
            int sid = Convert.ToInt32(SceneID);
            AutoResetEvent mutex = new AutoResetEvent(false);

            SceneRunner sr = new SceneRunner(sid, "JavaScript");
            sr.onRunComplete += (s, a) =>
            {
                mutex.Set();
            };
            sr.RunScene();
            mutex.WaitOne();
        }
    }
}

