using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using zvs.Entities;


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
            string msg = string.Format("{0}, TriggerID:{1}", args.Details, args.TriggerID);
            if (args.Errors)
                Core.log.Error(msg);
            else
                Core.log.Info(msg);

            if (onTriggerBegin != null)
                onTriggerBegin(this, args);
        }

        private void TriggerEnd(onTriggerEventArgs args)
        {
            string msg = string.Format("{0}, TriggerID:{1}", args.Details, args.TriggerID);
            if (args.Errors)
                Core.log.Error(msg);
            else
                Core.log.Info(msg);

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

        private void device_values_DeviceValueDataChangedEvent(object sender, DeviceValue.ValueDataChangedEventArgs args)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (s, a) =>
            {
                using (zvsContext context = new zvsContext())
                {
                    DeviceValue dv = context.DeviceValues.FirstOrDefault(v => v.DeviceValueId == args.DeviceValueId);
                    if (dv != null)
                    {
                        //Event Triggering
                        foreach (DeviceValueTrigger trigger in dv.Triggers.Where(t => t.isEnabled))
                        {
                            switch (trigger.Operator)
                            {
                                case TriggerOperator.EqualTo:
                                    {
                                        if (dv.Value.Equals(trigger.Value))
                                        {
                                            ActivateTriggerScene(trigger);
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
                                                ActivateTriggerScene(trigger);
                                            }
                                        }
                                        else
                                        {
                                            if (onTriggerBegin != null)
                                            {
                                                onTriggerBegin(this, new onTriggerEventArgs(trigger.DeviceValueTriggerId,
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
                                                ActivateTriggerScene(trigger);

                                        }
                                        else
                                        {
                                            if (onTriggerBegin != null)
                                            {
                                                onTriggerBegin(this, new onTriggerEventArgs(trigger.DeviceValueTriggerId,
                                                    string.Format("Trigger '{0}' failed to evaluate. Make sure the trigger value and device value is numeric.", trigger.Name), true));
                                            }
                                        }

                                        break;
                                    }
                                case TriggerOperator.NotEqualTo:
                                    {
                                        if (!dv.Value.Equals(trigger.Value))
                                        {
                                            ActivateTriggerScene(trigger);
                                        }
                                        break;
                                    }
                            }

                        }
                    }
                }
            };
            bw.RunWorkerAsync();
        }

        private void ActivateTriggerScene(DeviceValueTrigger trigger)
        {
            using (zvsContext context = new zvsContext())
            {
                BuiltinCommand cmd = context.BuiltinCommands.FirstOrDefault(c => c.UniqueIdentifier == "RUN_SCENE");
                if (cmd != null)
                {
                    TriggerBegin(new onTriggerEventArgs(trigger.DeviceValueTriggerId,
                                string.Format("Trigger '{0}' caused scene '{1}' to activate.", trigger.Name, trigger.Scene.Name), false));

                    CommandProcessor cp = new CommandProcessor(Core);
                    cp.onProcessingCommandEnd += (s, a) =>
                    {
                        TriggerEnd(new onTriggerEventArgs(trigger.DeviceValueTriggerId,
                                string.Format("Trigger '{0}' ended.", trigger.Name), false));
                    };
                    cp.RunBuiltinCommand(context, cmd, trigger.Scene.SceneId.ToString());
                }
            }
        }
    }
}
