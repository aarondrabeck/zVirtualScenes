using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using zvs.DataModel;
using System.Data.Entity;

namespace zvs.Processor
{
    internal class SceneRunner
    {
        private IEntityContextConnection EntityContextConnection { get; set; }
        private IFeedback<LogEntry> Log { get; set; }
        private ICommandProcessor CommandProcessor { get; set; }

        /// <summary>
        /// Executes a scene asynchronously and reports progress.
        /// </summary>
        public SceneRunner(IFeedback<LogEntry> log, ICommandProcessor commandProcessor, IEntityContextConnection entityContextConnection)
        {
            if (log == null)
                throw new ArgumentNullException("log");

            if (commandProcessor == null)
                throw new ArgumentNullException("commandProcessor");

            if (entityContextConnection == null)
                throw new ArgumentNullException("entityContextConnection");

            CommandProcessor = commandProcessor;
            Log = log;
            EntityContextConnection = entityContextConnection;
        }

        //Methods
        public async Task<Result> RunSceneAsync(int sceneId, CancellationToken cancellationToken)
        {
            try
            {
                using (var context = new ZvsContext(EntityContextConnection))
                {
                    var scene = await context.Scenes
                        .Include(o => o.Commands)
                        .FirstOrDefaultAsync(o => o.Id == sceneId, cancellationToken);

                    if (scene == null)
                    {
                        var msg =
                            string.Format("Failed to run scene with Id of {0} because it was not found in the database",
                                sceneId);
                        await Log.ReportWarningAsync(msg, CancellationToken.None);
                        return Result.ReportErrorFormat(msg);
                    }
                    if (scene.IsRunning)
                    {
                        var msg = string.Format("Failed to run scene '{0}' because it is already running", scene.Name);
                        await Log.ReportWarningAsync(msg, CancellationToken.None);
                        return Result.ReportErrorFormat(msg);
                    }

                    if (scene.Commands.Count < 1)
                    {
                        var msg = string.Format("Failed to run scene '{0}' because it has no commands", scene.Name);

                        await Log.ReportWarningAsync(msg, CancellationToken.None);
                        return Result.ReportErrorFormat(msg);
                    }

                    scene.IsRunning = true;
                    await context.SaveChangesAsync(cancellationToken);
                    await Log.ReportInfoFormatAsync(CancellationToken.None, "Scene '{0}' started execution", scene.Name);

                    var commandsRunSuccessfully = 0;
                    foreach (var command in scene.Commands.OrderBy(o => o.SortOrder))
                    {
                        if (cancellationToken.IsCancellationRequested)
                            throw new OperationCanceledException();

                        var result = await CommandProcessor.RunStoredCommandAsync(this, command, cancellationToken);
                        if (result.HasError)
                        {
                            await Log.ReportWarningAsync(result.Message, CancellationToken.None);
                        }
                        else
                        {
                            await Log.ReportInfoAsync(result.Message, CancellationToken.None);
                            commandsRunSuccessfully++;
                        }

                    }

                    scene.IsRunning = false;
                    await context.SaveChangesAsync(cancellationToken);
                    var summaryMsg = string.Format("Scene '{0}' completed execution with {1} of {2} commands ran successfully",
                        scene.Name,
                        commandsRunSuccessfully,
                        scene.Commands.Count())
                    ;
                    await Log.ReportInfoAsync(summaryMsg, CancellationToken.None);
                    return Result.ReportSuccess(summaryMsg);
                }
            }
            catch (OperationCanceledException)
            {
                var msg = string.Format("Scene runner running scene Id {0} was cancelled", sceneId);
                Log.ReportInfoAsync(msg, CancellationToken.None).Wait(CancellationToken.None);
                return Result.ReportSuccessFormat(msg);
            }
        }
    }
}
