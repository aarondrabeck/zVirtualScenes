using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using zvs.DataModel;
using System.Data.Entity;

namespace zvs.Processor
{
    internal class SceneRunner : IDisposable
    {
        
        private Scene _scene;
        private int _executionErrors = 0;
        private int _executedCommands = 0;
        private List<SceneCommand> _commandsToExecute = new List<SceneCommand>();

        private ICommandProcessor CommandProcessor { get; set; }

        /// <summary>
        /// Executes a scene asynchronously and reports progress.
        /// </summary>
        public SceneRunner(ICommandProcessor commandProcessor, ZvsContext zvsContext)
        {
            CommandProcessor = commandProcessor;
            context = new ZvsContext();
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

            _executionErrors = 0;
            _executedCommands = 0;

            _commandsToExecute = _scene.Commands.OrderBy(o => o.SortOrder).ToList();

            return await ProcessNextCommandAsync();
        }

        private async Task<SceneResult> ProcessNextCommandAsync()
        {
            if (_executedCommands < _commandsToExecute.Count)
            {
                var sceneCommand = _commandsToExecute[_executedCommands];
                var result = await cp.RunStoredCommandAsync(_scene, sceneCommand.StoredCommand.Id);

                if (result.HasErrors)
                    _executionErrors++;

                _executedCommands++;

                //on to the next one
                return await ProcessNextCommandAsync();
            }
            else
            {
                _scene.isRunning = false;
                await context.SaveChangesAsync();

                return new SceneResult(_scene.Id, _executionErrors > 0, string.Format("Scene '{0}' finished running with {1} errors.", _scene.Name, _executionErrors));
            }
        }

        public void Dispose()
        {
            context.Dispose();
        }

    }
}
