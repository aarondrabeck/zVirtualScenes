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

        /// <summary>
        /// Called when JavaScript executer is finished.
        /// </summary>
        public event onJavaScriptExecuterEventHandler onJavaScriptExecuterComplete;
        #endregion

        public void ExecuteScript(string Script, zvsContext context)
        {
            Jint.JintEngine engine = new Jint.JintEngine();
            engine.SetDebugMode(true);
            engine.DisableSecurity();
            engine.AllowClr = true;
            engine.SetParameter("zvsContext", context);
            engine.SetFunction("RunScene", new Action<double>(RunScene));
            try
            {
                object result = engine.Run(Script);
                if (result != null)
                {
                    if (onJavaScriptExecuterComplete != null)
                         onJavaScriptExecuterComplete(this, new JavaScriptExecuterEventArgs(false, result.ToString()));
                    return;
                }
            }
            catch (Exception exc)
            {
                if (onJavaScriptExecuterComplete != null)
                    onJavaScriptExecuterComplete(this, new JavaScriptExecuterEventArgs(true, exc.ToString()));
            }

            if (onJavaScriptExecuterComplete != null)
                         onJavaScriptExecuterComplete(this, new JavaScriptExecuterEventArgs(false, "None"));            
        }

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

                    #region LISTEN FOR ENDING
                    SceneRunner.onSceneRunEventHandler handler = null;
                    handler = (se, end_args) =>
                    {
                        if (end_args.SceneRunnerGUID == sr.SceneRunnerGUID)
                        {
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

