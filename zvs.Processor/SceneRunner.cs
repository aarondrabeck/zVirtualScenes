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
        private CommandProcessor cp;
        private Core Core;

        /// <summary>
        /// Executes a scene asynchronously and reports progress.
        /// </summary>
        /// <param name="InvokersName"> Name of the person executing the scene.</param>
        public SceneRunner(Core core)
        {
            Core = core;
            cp = new CommandProcessor(core);
            context = new zvsContext();
        }

        #region Events
        public class SceneResult : EventArgs
        {
            public bool Errors { get; private set; }
            public string Details { get; private set; }
            public int SceneID { get; private set; }

            public SceneResult(int SceneID, bool Errors, string Details)
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

            public onReportProgressEventArgs(int sceneID, string progress)
            {
                this.Progress = progress;
                this.SceneID = sceneID;
            }
        }
        /// <summary>
        /// Called when a scene has been called to be executed.
        /// </summary>
        public event onReportProgressEventHandler onReportProgress;
        #endregion

        #region Event Helper Methods
        private void ReportProgress(onReportProgressEventArgs args)
        {
            if (onReportProgress != null)
                onReportProgress(this, args);
        }
        #endregion

        //Methods
        public async Task<SceneResult> RunSceneAsync(int sceneId)
        {
            await Task.Factory.StartNew(() =>
             {
                 _scene = context.Scenes.FirstOrDefault(o => o.Id == sceneId);
             });

            if (_scene == null)
                return new SceneResult(sceneId, true, "Failed to run scene '" + sceneId + "' because it was not found in the database!");

            if (_scene.isRunning)
                return new SceneResult(_scene.Id, true, "Failed to run scene '" + _scene.Name + "' because it is already running!");

            if (_scene.Commands.Count < 1)
                return new SceneResult(_scene.Id, true, "Failed to run scene '" + _scene.Name + "' because it has no commands!");

            ReportProgress(new onReportProgressEventArgs(_scene.Id, "Scene '" + _scene.Name + "' started."));
            await Task.Factory.StartNew(() =>
             {
                 _scene.isRunning = true;
                 context.SaveChanges();

                 ExecutionErrors = 0;
                 ExecutedCommands = 0;
                 CommandsToExecute = _scene.Commands.OrderBy(o => o.SortOrder).ToList();
             });

            return await ProcessNextCommandAsync();
        }

        private async Task<SceneResult> ProcessNextCommandAsync()
        {
            if (ExecutedCommands < CommandsToExecute.Count)
            {
                SceneCommand sceneCommand = CommandsToExecute[ExecutedCommands];
                CommandProcessorResult result = await cp.RunStoredCommandAsync(sceneCommand.StoredCommand.Id);

                if (result.Errors)
                    ExecutionErrors++;

                ExecutedCommands++;

                //on to the next one
                return await ProcessNextCommandAsync();
            }
            else
            {
                await Task.Factory.StartNew(() =>
             {
                _scene.isRunning = false;
                context.SaveChanges();
             });

                return new SceneResult(_scene.Id, ExecutionErrors > 0, string.Format("Scene '{0}' finished running with {1} errors.", _scene.Name, ExecutionErrors));
            }
        }

        public void Dispose()
        {
            context.Dispose();
        }

    }
}
