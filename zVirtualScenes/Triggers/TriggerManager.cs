using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using zvs.Entities;


namespace zVirtualScenes.Triggers
{
    public class TriggerManager : IDisposable
    {
        #region Events
        public delegate void onTriggerStartEventHandler(object sender, onTriggerStartEventArgs args);
        public class onTriggerStartEventArgs : EventArgs
        {
            public string Details { get; private set; }
            public int TriggerID { get; private set; }
            public bool hasErrors { get; private set; }

            public onTriggerStartEventArgs(int TriggerID, string Details, bool hasErrors)
            {
                this.TriggerID = TriggerID;
                this.Details = Details;
                this.hasErrors = hasErrors;
            }
        }
        /// <summary>
        /// Called when a scene has been called to be executed.
        /// </summary>
        public static event onTriggerStartEventHandler onTriggerStart;
        #endregion

        public TriggerManager()
        {
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
                                            if (onTriggerStart != null)
                                            {
                                                onTriggerStart(this, new onTriggerStartEventArgs(trigger.DeviceValueTriggerId,
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
                                            if (onTriggerStart != null)
                                            {
                                                onTriggerStart(this, new onTriggerStartEventArgs(trigger.DeviceValueTriggerId,
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
            SceneRunner sr = new SceneRunner();
            SceneRunner.onSceneRunEventHandler startHandler = null;
            startHandler = (s, args) =>
            {
                if (args.SceneRunnerGUID == sr.SceneRunnerGUID)
                {
                    SceneRunner.onSceneRunBegin -= startHandler;

                    if (onTriggerStart != null)
                    {
                        onTriggerStart(this, new onTriggerStartEventArgs(trigger.DeviceValueTriggerId,
                            string.Format("Trigger '{0}' caused scene '{1}' to activate.", trigger.Name, trigger.Scene.Name), false));
                    }
                }
            };
            SceneRunner.onSceneRunBegin += startHandler;

            sr.RunScene(trigger.Scene.SceneId);
        }
    }
}
