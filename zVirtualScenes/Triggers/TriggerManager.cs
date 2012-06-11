using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using zVirtualScenesModel;

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
            device_values.DeviceValueDataChangedEvent += new device_values.ValueDataChangedEventHandler(device_values_DeviceValueDataChangedEvent);
        }

        public void Dispose()
        {
            device_values.DeviceValueDataChangedEvent -= new device_values.ValueDataChangedEventHandler(device_values_DeviceValueDataChangedEvent);
        }

        private void device_values_DeviceValueDataChangedEvent(object sender, device_values.ValueDataChangedEventArgs args)
        {
            using (zvsLocalDBEntities context = new zvsLocalDBEntities())
            {
                device_values dv = context.device_values.FirstOrDefault(v => v.id == args.device_value_id);
                if (dv != null)
                {
                    //Event Triggering
                    foreach (device_value_triggers trigger in dv.device_value_triggers.Where(t => t.enabled))
                    {
                        if (((device_value_triggers.TRIGGER_TYPE)trigger.trigger_type) == device_value_triggers.TRIGGER_TYPE.Basic)
                        {
                            switch ((device_value_triggers.TRIGGER_OPERATORS)trigger.trigger_operator)
                            {
                                case device_value_triggers.TRIGGER_OPERATORS.EqualTo:
                                    {
                                        if (dv.value2.Equals(trigger.trigger_value))
                                        {
                                            ActivateTriggerScene(trigger);
                                        }
                                        break;
                                    }
                                case device_value_triggers.TRIGGER_OPERATORS.GreaterThan:
                                    {
                                        double deviceValue = 0;
                                        double triggerValue = 0;

                                        if (double.TryParse(dv.value2, out deviceValue) && double.TryParse(trigger.trigger_value, out triggerValue))
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
                                                onTriggerStart(this, new onTriggerStartEventArgs(trigger.id,
                                                    string.Format("Trigger '{0}' failed to evaluate. Make sure the trigger value and device value is numeric.", trigger.Name), true));
                                            }
                                        }

                                        break;
                                    }
                                case device_value_triggers.TRIGGER_OPERATORS.LessThan:
                                    {
                                        double deviceValue = 0;
                                        double triggerValue = 0;

                                        if (double.TryParse(dv.value2, out deviceValue) && double.TryParse(trigger.trigger_value, out triggerValue))
                                        {
                                            if (deviceValue < triggerValue)                                            
                                                ActivateTriggerScene(trigger);
                                            
                                        }
                                        else
                                        {
                                            if (onTriggerStart != null)
                                            {
                                                onTriggerStart(this, new onTriggerStartEventArgs(trigger.id,
                                                    string.Format("Trigger '{0}' failed to evaluate. Make sure the trigger value and device value is numeric.", trigger.Name), true));
                                            }
                                        }

                                        break;
                                    }
                                case device_value_triggers.TRIGGER_OPERATORS.NotEqualTo:
                                    {
                                        if (!dv.value2.Equals(trigger.trigger_value))
                                        {
                                            ActivateTriggerScene(trigger);
                                        }
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            //Core.triggerManager.RunScript(trigger);
                        }
                    }
                }
            }
        }

        private void ActivateTriggerScene(device_value_triggers trigger)
        {
            SceneRunner.onSceneRunEventHandler startHandler = null;
            startHandler = (s, args) =>
            {
                if (args.SceneID == trigger.scene.id)
                {
                    SceneRunner.onSceneRunBegin -= startHandler;

                    if (onTriggerStart != null)
                    {
                        onTriggerStart(this, new onTriggerStartEventArgs(trigger.id,
                            string.Format("Trigger '{0}' caused scene '{1}' to activate.", trigger.Name, trigger.scene.friendly_name), false));
                    }
                }
            };
            SceneRunner.onSceneRunBegin += startHandler;
            SceneRunner sr = new SceneRunner();
            sr.RunScene(trigger.scene.id);
        }
    }
}
