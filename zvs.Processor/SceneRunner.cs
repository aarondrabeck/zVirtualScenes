using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using zvs.Entities;


namespace zvs.Processor
{
    public class SceneRunner : IDisposable
    {
        private string Source = string.Empty;
        private zvsContext context;
        private Scene _scene;
        private int ExecutionErrors = 0;
        private int ExecutedCommands = 0;
        private List<SceneCommand> CommandsToExecute = new List<SceneCommand>();
        private BackgroundWorker worker = new BackgroundWorker();
        private bool hasExecuted = false;
        private int SceneId = 0;
        public Guid SceneRunnerGUID { get; private set; }

        /// <summary>
        /// Executes a scene asynchronously and reports progress.
        /// </summary>
        /// <param name="InvokersName"> Name of the person executing the scene.</param>
        public SceneRunner(int sceneID, string source)
        {
            this.SceneId = sceneID;
            this.Source = source;
            SceneRunnerGUID = Guid.NewGuid();
            context = new zvsContext();
        }

        #region Events
        public delegate void onSceneRunEventHandler(object sender, onSceneRunEventArgs args);
        public class onSceneRunEventArgs : EventArgs
        {
            public bool Errors { get; private set; }
            public string Details { get; private set; }
            public int SceneID { get; private set; }
            public Guid SceneRunnerGUID { get; private set; }
            public string Source { get; private set; }

            public onSceneRunEventArgs(int SceneID, bool Errors, string Details, Guid SceneRunnerGUID, string source)
            {
                this.SceneID = SceneID;
                this.Errors = Errors;
                this.Details = Details;
                this.SceneRunnerGUID = SceneRunnerGUID;
                this.Source = source;
            }
        }

        public delegate void onReportProgressEventHandler(object sender, onReportProgressEventArgs args);
        public class onReportProgressEventArgs : EventArgs
        {
            public int SceneID { get; private set; }
            public string Progress { get; private set; }
            public Guid SceneRunnerGUID { get; private set; }
            public string Source { get; private set; }

            public onReportProgressEventArgs(string progress, int sceneID, Guid sceneRunnerGUID, string source)
            {
                this.Progress = progress;
                this.SceneID = sceneID;
                this.SceneRunnerGUID = sceneRunnerGUID;
                this.Source = source;
            }
        }
        /// <summary>
        /// Called when a scene has been called to be executed.
        /// </summary>
        public static event onSceneRunEventHandler onSceneRunBegin;
        public event onSceneRunEventHandler onRunBegin;

        /// <summary>
        /// Called as a scene runner has progress to report.
        /// </summary>
        public static event onReportProgressEventHandler onSceneReportProgress;
        public event onReportProgressEventHandler onReportProgress;

        /// <summary>
        /// Called when a scene is finished.
        /// </summary>
        public static event onSceneRunEventHandler onSceneRunComplete;
        public event onSceneRunEventHandler onRunComplete;

        #endregion

        #region Event Helper Methods
        private void ReportBegin(onSceneRunEventArgs arg)
        {
            if (onSceneRunBegin != null)
                onSceneRunBegin(this, arg);

            if (onRunBegin != null)
                onRunBegin(this, arg);
        }

        private void ReportComplete(onSceneRunEventArgs arg)
        {
            if (onSceneRunComplete != null)
                onSceneRunComplete(this, arg);

            if (onRunComplete != null)
                onRunComplete(this, arg);
        }

        private void ReportProgress(string progress)
        {
            if (onReportProgress != null)
                onReportProgress(this, new onReportProgressEventArgs(progress, SceneId, SceneRunnerGUID, Source));

            if (onSceneReportProgress != null)
                onSceneReportProgress(this, new onReportProgressEventArgs(progress, SceneId, SceneRunnerGUID, Source));
        }
        #endregion

        //Methods
        public void RunScene()
        {
            if (hasExecuted)
                throw new Exception("Scene runners cannot be reused.");

            hasExecuted = true;
            worker.DoWork += (sender, args) =>
            {
                _scene = context.Scenes.FirstOrDefault(o => o.SceneId == SceneId);

                if (_scene == null)
                {
                    ReportBegin(new onSceneRunEventArgs(SceneId, false, "Scene '" + SceneId + "' started.", SceneRunnerGUID, Source));
                    ReportComplete(new onSceneRunEventArgs(SceneId, true, "Failed to run scene '" + SceneId + "' because it was not found in the database!", SceneRunnerGUID, Source));
                }


                ReportBegin(new onSceneRunEventArgs(_scene.SceneId, false, "Scene '" + _scene.Name + "' started.", SceneRunnerGUID, Source));


                if (_scene.isRunning)
                {
                    ReportComplete(new onSceneRunEventArgs(_scene.SceneId, true, "Failed to run scene '" + _scene.Name + "' because it is already running!", SceneRunnerGUID, Source));
                }
                else
                {
                    if (_scene.Commands.Count < 1)
                    {
                        ReportComplete(new onSceneRunEventArgs(_scene.SceneId, true, "Failed to run scene '" + _scene.Name + "' because it has no commands!", SceneRunnerGUID, Source));

                        return;
                    }

                    _scene.isRunning = true;
                    context.SaveChanges();

                    ExecutionErrors = 0;
                    ExecutedCommands = 0;
                    CommandsToExecute = _scene.Commands.OrderBy(o => o.SortOrder).ToList();

                    ProcessNextCommand();
                }
            };
            worker.RunWorkerAsync();


        }

        private void ProcessNextCommand()
        {
            if (ExecutedCommands < CommandsToExecute.Count)
            {
                SceneCommand sceneCommand = CommandsToExecute[ExecutedCommands];

                if (sceneCommand.Command is DeviceCommand)
                {
                    //Add the command to the queue    
                    QueuedDeviceCommand d_cmd = QueuedDeviceCommand.Create((DeviceCommand)sceneCommand.Command, sceneCommand.Argument);
                    context.QueuedCommands.Add(d_cmd);
                    context.SaveChanges();

                    PluginManager.onProcessingCommandEventHandler handler = null;
                    handler = (sender, args) =>
                    {
                        if (args.CommandQueueID == d_cmd.QueuedCommandId)
                        {
                            PluginManager.onProcessingCommandEnd -= handler;

                            if (args.hasErrors)
                                ExecutionErrors++;

                            ExecutedCommands++;

                            //on to the next one
                            ProcessNextCommand();
                        }
                    };
                    PluginManager.onProcessingCommandEnd += handler;
                    QueuedCommand.AddNewCommandCommand(new QueuedCommand.NewCommandArgs(d_cmd));
                }

                if (sceneCommand.Command is DeviceTypeCommand)
                {
                    //Add the command to the queue    
                    QueuedDeviceTypeCommand dt_cmd = QueuedDeviceTypeCommand.Create((DeviceTypeCommand)sceneCommand.Command, sceneCommand.Device, sceneCommand.Argument);
                    context.QueuedCommands.Add(dt_cmd);
                    context.SaveChanges();

                    PluginManager.onProcessingCommandEventHandler handler = null;
                    handler = (sender, args) =>
                    {
                        if (args.CommandQueueID == dt_cmd.QueuedCommandId)
                        {
                            PluginManager.onProcessingCommandEnd -= handler;

                            if (args.hasErrors)
                                ExecutionErrors++;

                            ExecutedCommands++;

                            //on to the next one
                            ProcessNextCommand();
                        }
                    };

                    PluginManager.onProcessingCommandEnd += handler;
                    QueuedCommand.AddNewCommandCommand(new QueuedCommand.NewCommandArgs(dt_cmd));
                }

                if (sceneCommand.Command is BuiltinCommand)
                {
                    //Add the command to the queue   
                    QueuedBuiltinCommand b_cmd = QueuedBuiltinCommand.Create((BuiltinCommand)sceneCommand.Command, sceneCommand.Argument);

                    context.QueuedCommands.Add(b_cmd);
                    context.SaveChanges();

                    PluginManager.onProcessingCommandEventHandler handler = null;
                    handler = (sender, args) =>
                    {
                        if (args.CommandQueueID == b_cmd.QueuedCommandId)
                        {
                            PluginManager.onProcessingCommandEnd -= handler;

                            if (args.hasErrors)
                                ExecutionErrors++;

                            ExecutedCommands++;

                            //on to the next one
                            ProcessNextCommand();
                        }
                    };
                    PluginManager.onProcessingCommandEnd += handler;
                    QueuedCommand.AddNewCommandCommand(new QueuedCommand.NewCommandArgs(b_cmd));
                }
                if (sceneCommand.Command is JavaScriptCommand)
                {
                    JavaScriptCommand js = (JavaScriptCommand)sceneCommand.Command;
                    JavaScriptExecuter je = new JavaScriptExecuter();
                    je.onReportProgress += (s, a) =>
                        {
                            ReportProgress(a.Progress);
                        };
                    je.onComplete += (s, a) =>
                    {
                        if (a.Errors)
                            ExecutionErrors++;

                        ExecutedCommands++;
                        ProcessNextCommand();
                    };
                    je.ExecuteScript(js.Script, context, Source);

                }

            }
            else
            {
                _scene.isRunning = false;
                context.SaveChanges();

                ReportComplete(new onSceneRunEventArgs(_scene.SceneId, ExecutionErrors > 0, string.Format("Scene '{0}' finished running with {1} errors.", _scene.Name, ExecutionErrors), SceneRunnerGUID, Source));

            }
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
