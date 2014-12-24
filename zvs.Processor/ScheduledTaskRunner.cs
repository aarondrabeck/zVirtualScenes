using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using zvs.DataModel;
using zvs.DataModel.Tasks;
using zvs.Processor.ScheduledTask;

namespace zvs.Processor
{
    public class ScheduledTaskRunner : IRunner
    {
        private IEntityContextConnection EntityContextConnection { get; set; }
        private IFeedback<LogEntry> Log { get; set; }
        private CancellationTokenSource Cts { get; set; }
        private ICommandProcessor CommandProcessor { get; set; }
        private ITimeProvider TimeProvider { get; set; }
        private IList<DataModel.ScheduledTask> CommandScheduledTasksCache { get; set; }
        private bool UpdateCache { get; set; }

        public bool IsRunning { get; private set; }
        public ScheduledTaskRunner(IFeedback<LogEntry> log, ICommandProcessor commandProcessor, IEntityContextConnection entityContextConnection, ITimeProvider timeProvider)
        {
            if (log == null)
                throw new ArgumentNullException("log");

            if (commandProcessor == null)
                throw new ArgumentNullException("commandProcessor");

            if (entityContextConnection == null)
                throw new ArgumentNullException("entityContextConnection");

            if (timeProvider == null)
                throw new ArgumentNullException("timeProvider");

            EntityContextConnection = entityContextConnection;
            CommandProcessor = commandProcessor;
            Log = log;
            TimeProvider = timeProvider;

            Log.Source = "Scheduled Task Runner";
        }

        public async Task StartAsync(CancellationToken ct)
        {
            if (IsRunning)
            {
                await Log.ReportWarningAsync("ScheduledTask runner is already started", ct);
                return;
            }

            await Log.ReportInfoAsync("ScheduledTask runner started", ct);
            Cts = new CancellationTokenSource();

            NotifyEntityChangeContext.ChangeNotifications<DataModel.ScheduledTask>.OnEntityUpdated += OnEntityChanged;
            NotifyEntityChangeContext.ChangeNotifications<DataModel.ScheduledTask>.OnEntityDeleted += OnEntityChanged;
            NotifyEntityChangeContext.ChangeNotifications<DataModel.ScheduledTask>.OnEntityAdded += OnEntityChanged;

            CommandScheduledTasksCache = await GetScheduledTasksAsync(Cts.Token);
            Run(Cts.Token);
        }

        public async Task StopAsync(CancellationToken ct)
        {
            if (!IsRunning)
            {
                await Log.ReportWarningAsync("ScheduledTask runner is not started", ct);
                return;
            }
            NotifyEntityChangeContext.ChangeNotifications<DataModel.ScheduledTask>.OnEntityUpdated -= OnEntityChanged;
            NotifyEntityChangeContext.ChangeNotifications<DataModel.ScheduledTask>.OnEntityDeleted -= OnEntityChanged;
            NotifyEntityChangeContext.ChangeNotifications<DataModel.ScheduledTask>.OnEntityAdded -= OnEntityChanged;

            Cts.Cancel();
        }

        private async void Run(CancellationToken cancellationToken)
        {
            IsRunning = true;
            while (true)
            {
                if (UpdateCache)
                {
                    CommandScheduledTasksCache = await GetScheduledTasksAsync(Cts.Token);
                    UpdateCache = false;
                }

                foreach (var task in CommandScheduledTasksCache)
                {
                    var needsToRun = false;

                    switch (task.TaskType)
                    {
                        case ScheduledTaskType.OneTime:
                        {
                            needsToRun = task.EvalOneTimeTrigger(TimeProvider);
                            break;
                        }
                        case ScheduledTaskType.Daily:
                        {
                            needsToRun = task.EvalDailyTrigger(TimeProvider);
                            break;
                        }
                        case ScheduledTaskType.Interval:
                        {
                            needsToRun = task.EvalIntervalTrigger(TimeProvider);
                            break;
                        }
                        case ScheduledTaskType.Weekly:
                        {
                            needsToRun = task.EvalWeeklyTrigger(TimeProvider);
                            break;
                        }
                        case ScheduledTaskType.Monthly:
                        {
                            needsToRun = task.EvalMonthlyTrigger(TimeProvider);
                            break;
                        }

                    }
                    if (!needsToRun) continue;

                    await Log.ReportInfoFormatAsync(cancellationToken, "Scheduled task '{0}' executed", task.Name);

                    var task1 = task;
                    await Task.Run(async () => await CommandProcessor.RunCommandAsync(task1.CommandId, task1.Argument, task1.Argument2,  cancellationToken), cancellationToken);
                }

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }

            IsRunning = false;
            await Log.ReportInfoAsync("ScheduledTask runner stopped", CancellationToken.None);
        }

        internal async Task<IList<DataModel.ScheduledTask>> GetScheduledTasksAsync(CancellationToken cancellationToken)
        {
            using (var context = new ZvsContext(EntityContextConnection))
            {
                return await context.ScheduledTasks
                    .Include(o=> o.Command)
                    .Where(o => o.IsEnabled)
                    .ToListAsync(cancellationToken);
            }
        }

        void OnEntityChanged(object sender, object e)
        {
            UpdateCache = true;
        }
    }
}
