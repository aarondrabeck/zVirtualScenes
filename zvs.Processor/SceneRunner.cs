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
        private zvsContext context;
        private Scene _scene;
        private int ExecutionErrors = 0;
        private int ExecutedCommands = 0;
        private List<SceneCommand> CommandsToExecute = new List<SceneCommand>();
        private BackgroundWorker worker = new BackgroundWorker();
        private bool hasExecuted = false;

        private Guid _SceneRunnerGUID = Guid.NewGuid();
        public Guid SceneRunnerGUID
        {
            get
            {
                return _SceneRunnerGUID;
            }
        }

        #region Events
        public delegate void onSceneRunEventHandler(object sender, onSceneRunEventArgs args);
        public class onSceneRunEventArgs : EventArgs
        {
            public bool Errors { get; private set; }
            public string Details { get; private set; }
            public int SceneID { get; private set; }
            public Guid SceneRunnerGUID { get; private set; }

            public onSceneRunEventArgs(int SceneID, bool Errors, string Details, Guid SceneRunnerGUID)
            {
                this.SceneID = SceneID;
                this.Errors = Errors;
                this.Details = Details;
                this.SceneRunnerGUID = SceneRunnerGUID;
            }
        }
        /// <summary>
        /// Called when a scene has been called to be executed.
        /// </summary>
        public static event onSceneRunEventHandler onSceneRunBegin;

        /// <summary>
        /// Called when a scene is finished.
        /// </summary>
        public static event onSceneRunEventHandler onSceneRunComplete;
        #endregion

        public SceneRunner()
        {
            context = new zvsContext();
        }

        //Methods
        public void RunScene(int SceneID)
        {
            if (hasExecuted)
                throw new Exception("Scene runners cannot be reused.");

            hasExecuted = true;
            worker.DoWork += (sender, args) =>
            {
                _scene = context.Scenes.FirstOrDefault(o => o.SceneId == SceneID);

                if (_scene == null)
                {
                    if (onSceneRunBegin != null)
                        onSceneRunBegin(this, new onSceneRunEventArgs(SceneID, false, "Scene '" + SceneID + "' started.", SceneRunnerGUID));

                    if (onSceneRunComplete != null)
                        onSceneRunComplete(this, new onSceneRunEventArgs(SceneID, true, "Failed to run scene '" + SceneID + "' because it was not found in the database!", SceneRunnerGUID));
                }


                if (onSceneRunBegin != null)
                    onSceneRunBegin(this, new onSceneRunEventArgs(_scene.SceneId, false, "Scene '" + _scene.Name + "' started.", SceneRunnerGUID));


                if (_scene.isRunning)
                {
                    if (onSceneRunComplete != null)
                        onSceneRunComplete(this, new onSceneRunEventArgs(_scene.SceneId, true, "Failed to run scene '" + _scene.Name + "' because it is already running!", SceneRunnerGUID));
                }
                else
                {
                    if (_scene.Commands.Count < 1)
                    {
                        if (onSceneRunComplete != null)
                            onSceneRunComplete(this, new onSceneRunEventArgs(_scene.SceneId, true, "Failed to run scene '" + _scene.Name + "' because it has no commands!", SceneRunnerGUID));

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
                    je.onComplete += (s, a) =>
                    {
                        if (a.Errors)
                            ExecutionErrors++;

                        ExecutedCommands++;
                        ProcessNextCommand();
                    };
                    je.ExecuteScript(js.Script, context);
                    
                }

            }
            else
            {
                _scene.isRunning = false;
                context.SaveChanges();

                if (onSceneRunComplete != null)
                    onSceneRunComplete(this, new onSceneRunEventArgs(_scene.SceneId, ExecutionErrors > 0, string.Format("Scene '{0}' finished running with {1} errors.", _scene.Name, ExecutionErrors), SceneRunnerGUID));

            }
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
