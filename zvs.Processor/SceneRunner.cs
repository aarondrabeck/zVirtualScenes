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
    internal class SceneRunner : IDisposable
    {
        private zvsContext context;
        private Scene _scene;
        private int ExecutionErrors = 0;
        private int ExecutedCommands = 0;
        private List<SceneCommand> CommandsToExecute = new List<SceneCommand>();
        private BackgroundWorker worker = new BackgroundWorker();
        private CommandProcessor cp;
        private Core Core;

        /// <summary>
        /// Executes a scene asynchronously and reports progress.
        /// </summary>
        /// <param name="InvokersName"> Name of the person executing the scene.</param>
        public SceneRunner( Core core)
        {
            Core = core;
            cp = new CommandProcessor(core);
            cp.onProcessingCommandEnd += cp_onProcessingCommandEnd;
            context = new zvsContext();            
        }

        #region Events
        public delegate void onSceneRunEventHandler(object sender, onSceneRunEventArgs args);
        public class onSceneRunEventArgs : EventArgs
        {
            public bool Errors { get; private set; }
            public string Details { get; private set; }
            public int SceneID { get; private set; }

            public onSceneRunEventArgs(int SceneID, bool Errors, string Details)
            {
                this.SceneID = SceneID;
                this.Errors = Errors;
                this.Details = Details;
            }
        }

        public delegate void onReportProgressEventHandler(object sender, onReportProgressEventArgs args);
        public class onReportProgressEventArgs : EventArgs
        {
            public int SceneID { get; private set; }
            public string Progress { get; private set; }
            public Guid SceneRunnerGUID { get; private set; }

            public onReportProgressEventArgs(string progress, int sceneID, Guid sceneRunnerGUID)
            {
                this.Progress = progress;
                this.SceneID = sceneID;
                this.SceneRunnerGUID = sceneRunnerGUID;
            }
        }
        /// <summary>
        /// Called when a scene has been called to be executed.
        /// </summary>
        public event onSceneRunEventHandler onRunBegin;

        /// <summary>
        /// Called when a scene is finished.
        /// </summary>
        public event onSceneRunEventHandler onRunEnd;
        #endregion

        #region Event Helper Methods
        private void ReportBegin(onSceneRunEventArgs args)
        {
            string msg = string.Format("{0}, SceneID:{1}", args.Details, args.SceneID);
            if (args.Errors)
                Core.log.Error(msg);
            else
                Core.log.Info(msg);

            if (onRunBegin != null)
                onRunBegin(this, args);
        }

        private void ReportEnd(onSceneRunEventArgs args)
        {
            string msg = string.Format("{0}, SceneID:{1}", args.Details, args.SceneID);
            if (args.Errors)
                Core.log.Error(msg);
            else
                Core.log.Info(msg);

            if (onRunEnd != null)
                onRunEnd(this, args);
        }
        #endregion

        //Methods
        public void RunScene(int sceneId)
        {
            worker.DoWork += (sender, args) =>
            {
                _scene = context.Scenes.FirstOrDefault(o => o.SceneId == sceneId);

                if (_scene == null)
                {
                    ReportBegin(new onSceneRunEventArgs(sceneId, false, "Scene '" + sceneId + "' started."));
                    ReportEnd(new onSceneRunEventArgs(sceneId, true, "Failed to run scene '" + sceneId + "' because it was not found in the database!"));
                }


                ReportBegin(new onSceneRunEventArgs(_scene.SceneId, false, "Scene '" + _scene.Name + "' started."));


                if (_scene.isRunning)
                {
                    ReportEnd(new onSceneRunEventArgs(_scene.SceneId, true, "Failed to run scene '" + _scene.Name + "' because it is already running!"));
                }
                else
                {
                    if (_scene.Commands.Count < 1)
                    {
                        ReportEnd(new onSceneRunEventArgs(_scene.SceneId, true, "Failed to run scene '" + _scene.Name + "' because it has no commands!"));

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

        public void Dispose()
        {
            context.Dispose();
        }

        private void cp_onProcessingCommandEnd(object sender, CommandProcessor.onProcessingCommandEventArgs args)
        {
            if (args.Errors)
                ExecutionErrors++;

            ExecutedCommands++;

            //on to the next one
            ProcessNextCommand();
        }

        private void ProcessNextCommand()
        {
            if (ExecutedCommands < CommandsToExecute.Count)
            {
                SceneCommand sceneCommand = CommandsToExecute[ExecutedCommands];
                cp.RunStoredCommand(context, sceneCommand.StoredCommand);
            }
            else
            {
                _scene.isRunning = false;
                context.SaveChanges();

                ReportEnd(new onSceneRunEventArgs(_scene.SceneId, ExecutionErrors > 0, string.Format("Scene '{0}' finished running with {1} errors.", _scene.Name, ExecutionErrors)));
            }
        }
        
    }
}
