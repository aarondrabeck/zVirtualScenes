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
    public class ScheduledTaskRunner
    {
        private IEntityContextConnection EntityContextConnection { get; set; }
        private IFeedback<LogEntry> Log { get; set; }
        private CancellationTokenSource Cts { get; set; }
        private ICommandProcessor CommandProcessor { get; set; }
        private ITimeProvider TimeProvider { get; set; }
        private IList<ZvsScheduledTask> CommandScheduledTasksCache { get; set; }
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

            NotifyEntityChangeContext.ChangeNotifications<ZvsScheduledTask>.OnEntityUpdated += OnEntityChanged;
            NotifyEntityChangeContext.ChangeNotifications<ZvsScheduledTask>.OnEntityDeleted += OnEntityChanged;
            NotifyEntityChangeContext.ChangeNotifications<ZvsScheduledTask>.OnEntityAdded += OnEntityChanged;

            NotifyEntityChangeContext.ChangeNotifications<IntervalScheduledTask>.OnEntityAdded += OnEntityChanged;
            NotifyEntityChangeContext.ChangeNotifications<IntervalScheduledTask>.OnEntityDeleted += OnEntityChanged;
            NotifyEntityChangeContext.ChangeNotifications<IntervalScheduledTask>.OnEntityUpdated += OnEntityChanged;

            NotifyEntityChangeContext.ChangeNotifications<OneTimeScheduledTask>.OnEntityAdded += OnEntityChanged;
            NotifyEntityChangeContext.ChangeNotifications<OneTimeScheduledTask>.OnEntityDeleted += OnEntityChanged;
            NotifyEntityChangeContext.ChangeNotifications<OneTimeScheduledTask>.OnEntityUpdated += OnEntityChanged;

            NotifyEntityChangeContext.ChangeNotifications<MonthlyScheduledTask>.OnEntityAdded += OnEntityChanged;
            NotifyEntityChangeContext.ChangeNotifications<MonthlyScheduledTask>.OnEntityDeleted += OnEntityChanged;
            NotifyEntityChangeContext.ChangeNotifications<MonthlyScheduledTask>.OnEntityUpdated += OnEntityChanged;

            NotifyEntityChangeContext.ChangeNotifications<DailyScheduledTask>.OnEntityAdded += OnEntityChanged;
            NotifyEntityChangeContext.ChangeNotifications<DailyScheduledTask>.OnEntityDeleted += OnEntityChanged;
            NotifyEntityChangeContext.ChangeNotifications<DailyScheduledTask>.OnEntityUpdated += OnEntityChanged;

            NotifyEntityChangeContext.ChangeNotifications<WeeklyScheduledTask>.OnEntityAdded += OnEntityChanged;
            NotifyEntityChangeContext.ChangeNotifications<WeeklyScheduledTask>.OnEntityDeleted += OnEntityChanged;
            NotifyEntityChangeContext.ChangeNotifications<WeeklyScheduledTask>.OnEntityUpdated += OnEntityChanged;

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
            NotifyEntityChangeContext.ChangeNotifications<ZvsScheduledTask>.OnEntityUpdated -= OnEntityChanged;
            NotifyEntityChangeContext.ChangeNotifications<ZvsScheduledTask>.OnEntityDeleted -= OnEntityChanged;
            NotifyEntityChangeContext.ChangeNotifications<ZvsScheduledTask>.OnEntityAdded -= OnEntityChanged;

            NotifyEntityChangeContext.ChangeNotifications<IntervalScheduledTask>.OnEntityAdded -= OnEntityChanged;
            NotifyEntityChangeContext.ChangeNotifications<IntervalScheduledTask>.OnEntityDeleted -= OnEntityChanged;
            NotifyEntityChangeContext.ChangeNotifications<IntervalScheduledTask>.OnEntityUpdated -= OnEntityChanged;

            NotifyEntityChangeContext.ChangeNotifications<OneTimeScheduledTask>.OnEntityAdded -= OnEntityChanged;
            NotifyEntityChangeContext.ChangeNotifications<OneTimeScheduledTask>.OnEntityDeleted -= OnEntityChanged;
            NotifyEntityChangeContext.ChangeNotifications<OneTimeScheduledTask>.OnEntityUpdated -= OnEntityChanged;

            NotifyEntityChangeContext.ChangeNotifications<MonthlyScheduledTask>.OnEntityAdded -= OnEntityChanged;
            NotifyEntityChangeContext.ChangeNotifications<MonthlyScheduledTask>.OnEntityDeleted -= OnEntityChanged;
            NotifyEntityChangeContext.ChangeNotifications<MonthlyScheduledTask>.OnEntityUpdated -= OnEntityChanged;

            NotifyEntityChangeContext.ChangeNotifications<DailyScheduledTask>.OnEntityAdded -= OnEntityChanged;
            NotifyEntityChangeContext.ChangeNotifications<DailyScheduledTask>.OnEntityDeleted -= OnEntityChanged;
            NotifyEntityChangeContext.ChangeNotifications<DailyScheduledTask>.OnEntityUpdated -= OnEntityChanged;

            NotifyEntityChangeContext.ChangeNotifications<WeeklyScheduledTask>.OnEntityAdded -= OnEntityChanged;
            NotifyEntityChangeContext.ChangeNotifications<WeeklyScheduledTask>.OnEntityDeleted -= OnEntityChanged;
            NotifyEntityChangeContext.ChangeNotifications<WeeklyScheduledTask>.OnEntityUpdated -= OnEntityChanged;
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

                    var scheduledTask = task.ScheduledTask as OneTimeScheduledTask;
                    if (scheduledTask != null)
                        needsToRun = scheduledTask.EvalTrigger(TimeProvider);

                    var dailyScheduledTask = task.ScheduledTask as DailyScheduledTask;
                    if (dailyScheduledTask != null)
                        needsToRun = dailyScheduledTask.EvalTrigger(TimeProvider);

                    var intervalScheduledTask = task.ScheduledTask as IntervalScheduledTask;
                    if (intervalScheduledTask != null)
                        needsToRun = intervalScheduledTask.EvalTrigger(TimeProvider);

                    var weeklyScheduledTask = task.ScheduledTask as WeeklyScheduledTask;
                    if (weeklyScheduledTask != null)
                        needsToRun = weeklyScheduledTask.EvalTrigger(TimeProvider);

                    var monthlyScheduledTask = task.ScheduledTask as MonthlyScheduledTask;
                    if (monthlyScheduledTask != null)
                        needsToRun = monthlyScheduledTask.EvalTrigger(TimeProvider);

                    if (!needsToRun) continue;

                    await Log.ReportInfoFormatAsync(cancellationToken, "Scheduled task '{0}' executed.", task.Name);

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

        internal async Task<IList<ZvsScheduledTask>> GetScheduledTasksAsync(CancellationToken cancellationToken)
        {
            using (var context = new ZvsContext(EntityContextConnection))
            {
                return await context.ZvsScheduledTasks
                    .Include(o=> o.Command)
                    .Include(o => o.ScheduledTask)
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
