using System;
using System.Linq;
using zvs.Entities;
using System.Data.Entity;
using System.Threading.Tasks;

namespace zvs.Processor.Triggers
{
    internal class TriggerManager : IDisposable
    {
        private Core Core;
        #region Events
        public delegate void onTriggerBeginEventHandler(object sender, onTriggerEventArgs args);
        public delegate void onTriggerEndEventHandler(object sender, onTriggerEventArgs args);
        public class onTriggerEventArgs : EventArgs
        {
            public string Details { get; private set; }
            public int TriggerID { get; private set; }
            public bool Errors { get; private set; }

            public onTriggerEventArgs(int TriggerID, string Details, bool hasErrors)
            {
                this.TriggerID = TriggerID;
                this.Details = Details;
                this.Errors = hasErrors;
            }
        }
        /// <summary>
        /// Called when a scene has been called to be executed.
        /// </summary>
        public static event onTriggerBeginEventHandler onTriggerBegin;
        public static event onTriggerEndEventHandler onTriggerEnd;

        private void TriggerBegin(onTriggerEventArgs args)
        {
            if (args.Errors)
                Core.log.Error(args.Details);
            else
                Core.log.Info(args.Details);

            if (onTriggerBegin != null)
                onTriggerBegin(this, args);
        }

        private void TriggerEnd(onTriggerEventArgs args)
        {
            if (args.Errors)
                Core.log.Error(args.Details);
            else
                Core.log.Info(args.Details);

            if (onTriggerEnd != null)
                onTriggerEnd(this, args);
        }

        #endregion

        public TriggerManager(Core core)
        {
            Core = core;
            DeviceValue.DeviceValueDataChangedEvent += new DeviceValue.ValueDataChangedEventHandler(device_values_DeviceValueDataChangedEvent);
        }

        public void Dispose()
        {
            DeviceValue.DeviceValueDataChangedEvent -= new DeviceValue.ValueDataChangedEventHandler(device_values_DeviceValueDataChangedEvent);
        }

        private async void device_values_DeviceValueDataChangedEvent(object sender, DeviceValue.ValueDataChangedEventArgs args)
        {
            using (zvsContext context = new zvsContext())
            {
                var dv = await context.DeviceValues
                     .Include(o => o.Triggers)
                     .FirstOrDefaultAsync(v => v.Id == args.DeviceValueId);

                if (dv == null)
                    return;

                //Event Triggering
                foreach (DeviceValueTrigger trigger in dv.Triggers.Where(t => t.isEnabled))
                {
                    switch (trigger.Operator)
                    {
                        case TriggerOperator.EqualTo:
                            {
                                if (dv.Value.Equals(trigger.Value))
                                {
                                    await ActivateTriggerAsync(context, trigger.Id);
                                }
                                break;
                            }
                        case TriggerOperator.GreaterThan:
                            {
                                double deviceValue = 0;
                                double triggerValue = 0;

                                if (double.TryParse(dv.Value, out deviceValue) && double.TryParse(trigger.Value, out triggerValue))
                                {
                                    if (deviceValue > triggerValue)
                                    {
                                        await ActivateTriggerAsync(context, trigger.Id);
                                    }
                                }
                                else
                                {
                                    if (onTriggerBegin != null)
                                    {
                                        onTriggerBegin(this, new onTriggerEventArgs(trigger.Id,
                                            string.Format("Trigger '{0}' failed to evaluate. Make sure the trigger value and device value is numeric.", trigger.Name), true));
                                    }
                                }

                                break;
                            }
                        case TriggerOperator.LessThan:
                            {
                                double deviceValue = 0;
                                double triggerValue = 0;

                                if (double.TryParse(dv.Value, out deviceValue) && double.TryParse(trigger.Value, out triggerValue))
                                {
                                    if (deviceValue < triggerValue)
                                        await ActivateTriggerAsync(context, trigger.Id);

                                }
                                else
                                {
                                    if (onTriggerBegin != null)
                                    {
                                        onTriggerBegin(this, new onTriggerEventArgs(trigger.Id,
                                            string.Format("Trigger '{0}' failed to evaluate. Make sure the trigger value and device value is numeric.", trigger.Name), true));
                                    }
                                }

                                break;
                            }
                        case TriggerOperator.NotEqualTo:
                            {
                                if (!dv.Value.Equals(trigger.Value))
                                {
                                    await ActivateTriggerAsync(context, trigger.Id);
                                }
                                break;
                            }
                    }

                }
            }
        }

        private async Task ActivateTriggerAsync(zvsContext context, Int64 DeviceValueTriggerId)
        {
            var trigger = await context.DeviceValueTriggers
                .Include(o => o.StoredCommand)
                .FirstOrDefaultAsync(o => o.Id == DeviceValueTriggerId);

            if (trigger == null)
                return;

            TriggerBegin(new onTriggerEventArgs(trigger.Id,
                        string.Format("Trigger '{0}' caused {1} {2}.", trigger.Name,
                        trigger.StoredCommand.TargetObjectName,
                         trigger.StoredCommand.Description), false));

            CommandProcessor cp = new CommandProcessor(Core);
            CommandProcessorResult result = await cp.RunStoredCommandAsync(trigger, trigger.StoredCommand.Id);

            TriggerEnd(new onTriggerEventArgs(trigger.Id, string.Format("Trigger '{0}' ended {1} errors.", trigger.Name, result.HasErrors ? "with" : "without"), false));
        }
    }
}
