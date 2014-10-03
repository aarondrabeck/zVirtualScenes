using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using zvs.Entities;
using System.Data.Entity;

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
        public event onReportProgressEventHandler onReportProgress = delegate { };
        #endregion

        #region Event Helper Methods
        private void ReportProgress(onReportProgressEventArgs args)
        {
            onReportProgress(this, args);
        }
        #endregion

        //Methods
        public async Task<SceneResult> RunSceneAsync(int sceneId)
        {
            _scene = await context.Scenes
                .Include(o => o.Commands)
                .FirstOrDefaultAsync(o => o.Id == sceneId);

            if (_scene == null)
                return new SceneResult(sceneId, true, "Failed to run scene '" + sceneId + "' because it was not found in the database!");

            if (_scene.isRunning)
                return new SceneResult(_scene.Id, true, "Failed to run scene '" + _scene.Name + "' because it is already running!");

            if (_scene.Commands.Count < 1)
                return new SceneResult(_scene.Id, true, "Failed to run scene '" + _scene.Name + "' because it has no commands!");

            ReportProgress(new onReportProgressEventArgs(_scene.Id, "Scene '" + _scene.Name + "' started."));

            _scene.isRunning = true;
            await context.SaveChangesAsync();

            ExecutionErrors = 0;
            ExecutedCommands = 0;

            CommandsToExecute = _scene.Commands.OrderBy(o => o.SortOrder).ToList();

            return await ProcessNextCommandAsync();
        }

        private async Task<SceneResult> ProcessNextCommandAsync()
        {
            if (ExecutedCommands < CommandsToExecute.Count)
            {
                SceneCommand sceneCommand = CommandsToExecute[ExecutedCommands];
                CommandProcessorResult result = await cp.RunStoredCommandAsync(_scene, sceneCommand.StoredCommand.Id);

                if (result.HasErrors)
                    ExecutionErrors++;

                ExecutedCommands++;

                //on to the next one
                return await ProcessNextCommandAsync();
            }
            else
            {
                _scene.isRunning = false;
                await context.SaveChangesAsync();

                return new SceneResult(_scene.Id, ExecutionErrors > 0, string.Format("Scene '{0}' finished running with {1} errors.", _scene.Name, ExecutionErrors));
            }
        }

        public void Dispose()
        {
            context.Dispose();
        }

    }
}
