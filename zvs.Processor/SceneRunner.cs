using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using zvs.DataModel;

namespace zvs.Processor
{
    internal class SceneRunner
    {
        private IEntityContextConnection EntityContextConnection { get; }
        private IFeedback<LogEntry> Log { get; }
        private ICommandProcessor CommandProcessor { get; }

        /// <summary>
        /// Executes a scene asynchronously and reports progress.
        /// </summary>
        public SceneRunner(IFeedback<LogEntry> log, ICommandProcessor commandProcessor, IEntityContextConnection entityContextConnection)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            if (commandProcessor == null)
                throw new ArgumentNullException(nameof(commandProcessor));

            if (entityContextConnection == null)
                throw new ArgumentNullException(nameof(entityContextConnection));

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
                            $"Failed to run scene with Id of {sceneId} because it was not found in the database";
                        await Log.ReportWarningAsync(msg, CancellationToken.None);
                        return Result.ReportErrorFormat(msg);
                    }
                    if (scene.IsRunning)
                    {
                        var msg = $"Failed to run scene '{scene.Name}' because it is already running";
                        await Log.ReportWarningAsync(msg, CancellationToken.None);
                        return Result.ReportErrorFormat(msg);
                    }

                    if (scene.Commands.Count < 1)
                    {
                        var msg = $"Failed to run scene '{scene.Name}' because it has no commands";

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

                        var result = await CommandProcessor.RunCommandAsync(command.CommandId, command.Argument, command.Argument2, cancellationToken);
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
                    var summaryMsg =
                        $"Scene '{scene.Name}' completed execution with {commandsRunSuccessfully} of {scene.Commands.Count()} commands ran successfully"
                        ;
                    await Log.ReportInfoAsync(summaryMsg, CancellationToken.None);
                    return Result.ReportSuccess(summaryMsg);
                }
            }
            catch (OperationCanceledException)
            {
                var msg = $"Scene runner running scene Id {sceneId} was cancelled";
                Log.ReportInfoAsync(msg, CancellationToken.None).Wait(CancellationToken.None);
                return Result.ReportSuccessFormat(msg);
            }
        }
    }
}
