using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using zvs.DataModel;

namespace zvs.Processor
{
    public class TriggerManager
    {
        private ZvsContext Context { get; set; }
        private IFeedback<LogEntry> Log { get; set; }
        private CancellationToken Ct { get; set; }
        private ICommandProcessor CommandProcessor { get; set; }

        bool IsRunning { get; set; }
        public TriggerManager(IFeedback<LogEntry> log, ICommandProcessor commandProcessor, ZvsContext context)
        {
            if(log == null)
                throw new ArgumentNullException("log");

            if (commandProcessor == null)
                throw new ArgumentNullException("commandProcessor");

            if (context == null)
                throw new ArgumentNullException("context");

            Context = context;
            CommandProcessor = commandProcessor;
            Log = log;
        }

        public async Task StartAsync(CancellationToken ct)
        {
            if (IsRunning)
            {
                await Log.ReportWarningAsync("Trigger manager is already started", ct);
                return;
            }

            Ct = ct;
            NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.OnEntityUpdated += TriggerManager_OnEntityUpdated;
            IsRunning = true;

            await Log.ReportInfoAsync("Trigger manager started, now listening for device values changes", ct);
        }

        public async Task StopAsync(CancellationToken ct)
        {
            if (!IsRunning)
            {
                await Log.ReportWarningAsync("Trigger manager is not started", ct);
                return;
            }

            NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.OnEntityUpdated -= TriggerManager_OnEntityUpdated;
            IsRunning = false;

            await Log.ReportInfoAsync("Trigger manager stopped, no longer listening for device values changes", ct);
        }

        async void TriggerManager_OnEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.EntityUpdatedArgs e)
        {
            if (e.NewEntity.Value == e.OldEntity.Value) 
                return;

            //Get triggers for this value
            var dv = await Context.DeviceValues
                .Include(o => o.Triggers)
                .FirstOrDefaultAsync(v => v.Id == e.NewEntity.Id, Ct);
            
            if (dv == null)
                return;

            foreach (var trigger in dv.Triggers.Where(t => t.isEnabled))
            {
                switch (trigger.Operator)
                {
                    case TriggerOperator.EqualTo:
                    {
                        if (dv.Value.Equals(trigger.Value))
                        {
                            await ActivateTriggerAsync(trigger.Id, Ct);
                        }
                        break;
                    }
                    case TriggerOperator.GreaterThan:
                    {
                        double deviceValue;
                        double triggerValue;

                        if (double.TryParse(dv.Value, out deviceValue) && double.TryParse(trigger.Value, out triggerValue))
                        {
                            if (deviceValue > triggerValue)
                            {
                                await ActivateTriggerAsync(trigger.Id, Ct);
                            }
                        }
                        else
                        {
                            await
                                Log.ReportErrorFormatAsync(Ct,
                                    "Trigger '{0}' failed to evaluate. Make sure the trigger value and device value is numeric.",
                                    trigger.Name);
                        }

                        break;
                    }
                    case TriggerOperator.LessThan:
                    {
                        double deviceValue;
                        double triggerValue;

                        if (double.TryParse(dv.Value, out deviceValue) && double.TryParse(trigger.Value, out triggerValue))
                        {
                            if (deviceValue < triggerValue)
                                await ActivateTriggerAsync(trigger.Id, Ct);

                        }
                        else
                        {
                            await
                                Log.ReportErrorFormatAsync(Ct, "Trigger '{0}' failed to evaluate. Make sure the trigger value and device value is numeric.", trigger.Name);
                        }

                        break;
                    }
                    case TriggerOperator.NotEqualTo:
                    {
                        if (!dv.Value.Equals(trigger.Value))
                        {
                            await ActivateTriggerAsync(trigger.Id, Ct);
                        }
                        break;
                    }
                }

            }
        }

       internal async Task ActivateTriggerAsync(Int64 deviceValueTriggerId, CancellationToken cancellationToken)
        {
            var trigger = await Context.DeviceValueTriggers
                .Include(o => o.StoredCommand)
                .FirstOrDefaultAsync(o => o.Id == deviceValueTriggerId, Ct);

            if (trigger == null)
                return;

            await Log.ReportInfoFormatAsync(Ct, "Trigger '{0}' caused {1} {2}.", trigger.Name,
                trigger.StoredCommand.TargetObjectName,
                trigger.StoredCommand.Description);

            var result = await CommandProcessor.RunStoredCommandAsync(trigger, trigger.StoredCommand.Id, cancellationToken);

            await Log.ReportInfoFormatAsync(Ct, "Trigger '{0}' ended {1} errors.", trigger.Name, result.HasError ? "with" : "without");

        }
    }
}
