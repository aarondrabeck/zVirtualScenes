using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using zvs.DataModel;

namespace zvs.Processor
{
    public class TriggerRunner
    {
        private IEntityContextConnection EntityContextConnection { get; set; }
        private IFeedback<LogEntry> Log { get; set; }
        private CancellationToken Ct { get; set; }
        private ICommandProcessor CommandProcessor { get; set; }

        bool IsRunning { get; set; }
        public TriggerRunner(IFeedback<LogEntry> log, ICommandProcessor commandProcessor, IEntityContextConnection entityContextConnection)
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

            Log.Source = "Trigger Runner";
        }

        public async Task StartAsync(CancellationToken ct)
        {
            if (IsRunning)
            {
                await Log.ReportWarningAsync("Trigger runner is already started", ct);
                return;
            }

            Ct = ct;
            NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.OnEntityUpdated += TriggerManager_OnEntityUpdated;
            IsRunning = true;

            await Log.ReportInfoAsync("Trigger runner started, now listening for device values changes", ct);
        }

        public async Task StopAsync(CancellationToken ct)
        {
            if (!IsRunning)
            {
                await Log.ReportWarningAsync("Trigger runner is not started", ct);
                return;
            }

            NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.OnEntityUpdated -= TriggerManager_OnEntityUpdated;
            IsRunning = false;

            await Log.ReportInfoAsync("Trigger runner stopped, no longer listening for device values changes", ct);
        }

        async void TriggerManager_OnEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.EntityUpdatedArgs e)
        {
            if (e.NewEntity.Value == e.OldEntity.Value)
                return;

            using (var context = new ZvsContext(EntityContextConnection))
            {
                var sendingContext = sender as ZvsContext;
                if (sendingContext != null && sendingContext.Database.Connection.ConnectionString != context.Database.Connection.ConnectionString)
                    return;

                //Get triggers for this value
                var triggers = await context.DeviceValueTriggers
                    .Include(o => o.DeviceValue)
                    .Where(o => o.DeviceValueId == e.NewEntity.Id && o.IsEnabled)
                    .ToListAsync(Ct);

                foreach (var trigger in triggers)
                {
                    var changedToValue = e.NewEntity.Value;
                    switch (trigger.Operator)
                    {
                        case TriggerOperator.EqualTo:
                            {
                                Console.WriteLine("--");
                                Console.WriteLine("Device Value: " + changedToValue);
                                Console.WriteLine("Trigger Value: " + trigger.Value);

                                if (changedToValue.Equals(trigger.Value))
                                {
                                    await ActivateTriggerAsync(trigger.Id, Ct);
                                }
                                break;
                            }
                        case TriggerOperator.GreaterThan:
                            {
                                double deviceValue;
                                double triggerValue;

                                if (double.TryParse(changedToValue, out deviceValue) &&
                                    double.TryParse(trigger.Value, out triggerValue))
                                {
                                    if (deviceValue > triggerValue)
                                    {
                                        await ActivateTriggerAsync(trigger.Id, Ct);
                                    }
                                }
                                else
                                {
                                    await
                                        Log.ReportWarningFormatAsync(Ct,
                                            "Trigger '{0}' failed to evaluate. Make sure the trigger value and device value is numeric.",
                                            trigger.Name);
                                }

                                break;
                            }
                        case TriggerOperator.LessThan:
                            {
                                double deviceValue;
                                double triggerValue;

                                if (double.TryParse(changedToValue, out deviceValue) &&
                                    double.TryParse(trigger.Value, out triggerValue))
                                {
                                    if (deviceValue < triggerValue)
                                        await ActivateTriggerAsync(trigger.Id, Ct);

                                }
                                else
                                {
                                    await
                                        Log.ReportWarningFormatAsync(Ct,
                                            "Trigger '{0}' failed to evaluate. Make sure the trigger value and device value is numeric.",
                                            trigger.Name);
                                }

                                break;
                            }
                        case TriggerOperator.NotEqualTo:
                            {
                                if (!changedToValue.Equals(trigger.Value))
                                {
                                    await ActivateTriggerAsync(trigger.Id, Ct);
                                }
                                break;
                            }
                    }

                }
            }
        }

        internal async Task ActivateTriggerAsync(Int64 deviceValueTriggerId, CancellationToken cancellationToken)
        {
            using (var context = new ZvsContext(EntityContextConnection))
            {
                var trigger = await context.DeviceValueTriggers
                    .FirstOrDefaultAsync(o => o.Id == deviceValueTriggerId, Ct);

                if (trigger == null)
                    return;

                await Log.ReportInfoFormatAsync(Ct, "Trigger '{0}' caused {1} {2}.", trigger.Name,
                    trigger.TargetObjectName,
                    trigger.Description);

                var result = await CommandProcessor.RunCommandAsync(trigger.CommandId, trigger.Argument, trigger.Argument2, cancellationToken);

                await Log.ReportInfoFormatAsync(Ct, "Trigger '{0}' ended {1} errors.", trigger.Name, result.HasError ? "with" : "without");
            }
        }
    }
}
