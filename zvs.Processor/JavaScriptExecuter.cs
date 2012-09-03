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

        /// <summary>
        /// Called when JavaScript executer is finished.
        /// </summary>
        public event onJavaScriptExecuterEventHandler onComplete;

        public event onReportProgressEventHandler onReportProgress;
        #endregion

        public void ExecuteScript(string Script, zvsContext context)
        {
            this.Context = context;
            Jint.JintEngine engine = new Jint.JintEngine();
            engine.SetDebugMode(true);
            engine.DisableSecurity();
            engine.AllowClr = true;
            engine.SetParameter("zvsContext", context);
            engine.SetFunction("RunScene", new Action<double>(RunScene));
            engine.SetFunction("RunDeviceCommand", new Action<double, string, string>(RunDeviceCommand));
            engine.SetFunction("RunDeviceCommand", new Action<string, string, string>(RunDeviceCommand));
            engine.SetFunction("Delay", new Action<string, double, bool>(Delay));
            try
            {
                object result = engine.Run(Script);
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

            if(!Async)
                 mutex.WaitOne();
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

        //RunScene(1);
        public void RunScene(double SceneID)
        {
            int sid = Convert.ToInt32(SceneID);
            AutoResetEvent mutex = new AutoResetEvent(false);
            SceneRunner sr = new SceneRunner();
            SceneRunner.onSceneRunEventHandler startHandler = null;
            startHandler = (s, args) =>
            {
                if (args.SceneRunnerGUID == sr.SceneRunnerGUID)
                {
                    SceneRunner.onSceneRunBegin -= startHandler;
                    ReportProgress(args.Details);

                    #region LISTEN FOR ENDING
                    SceneRunner.onSceneRunEventHandler handler = null;
                    handler = (se, end_args) =>
                    {
                        if (end_args.SceneRunnerGUID == sr.SceneRunnerGUID)
                        {
                            ReportProgress(end_args.Details);
                            SceneRunner.onSceneRunComplete -= handler;
                            mutex.Set();
                        }
                    };
                    SceneRunner.onSceneRunComplete += handler;
                    #endregion
                }
            };
            SceneRunner.onSceneRunBegin += startHandler;

            //TODO: This should be sync not async
            sr.RunScene(sid);
            mutex.WaitOne();
        }
    }
}

