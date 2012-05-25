using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using zVirtualScenesCommon.Entity;
using zVirtualScenesCommon.Util;
using zVirtualScenesCommon;

namespace zVirtualScenes.Triggers
{
    public class TriggerManager
    {
        private Core Core;
        private List<device_value_triggers> triggers = new List<device_value_triggers>();
        private bool isRunning = false;

        public TriggerManager(Core Core, bool autoStart = true)
        {
            this.Core = Core;

            if (autoStart)
                Start();
        }

        public void Start()
        {
            if (!isRunning)
            {
                zvsEntityControl.TriggerModified += new zvsEntityControl.TriggerModifiedEventHandler(zvsEntityControl_TriggerModified);
                device_values.DeviceValueDataChangedEvent += new device_values.ValueDataChangedEventHandler(device_values_DeviceValueDataChangedEvent);

                GetTriggers();

                isRunning = true;
            }
        }

        public void Stop()
        {
            if (isRunning)
            {
                zvsEntityControl.TriggerModified -= new zvsEntityControl.TriggerModifiedEventHandler(zvsEntityControl_TriggerModified);
                device_values.DeviceValueDataChangedEvent -= new device_values.ValueDataChangedEventHandler(device_values_DeviceValueDataChangedEvent);

                isRunning = false;
            }
        }

        private void zvsEntityControl_TriggerModified(object sender, string PropertyModified)
        {
            //Reload the trigger list when they are modified.
            GetTriggers();
        }

        private void GetTriggers()
        {
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                triggers = db.device_value_triggers.ToList();
            }
        }

        private void device_values_DeviceValueDataChangedEvent(object sender, device_values.ValueDataChangedEventArgs args)
        {
            //Fire triggers
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                device_values dv = db.device_values.FirstOrDefault(v => v.id == args.device_value_id);
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
                                            Core.Logger.WriteToLog(Urgency.INFO, string.Format("Trigger '{0}' caused scene '{1}' to activate.", trigger.Name, trigger.scene.friendly_name), "TRIGGER");
                                            Core.Logger.WriteToLog(Urgency.INFO, trigger.scene.RunScene(db), "TRIGGER");
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
                                                Core.Logger.WriteToLog(Urgency.INFO, string.Format("Trigger '{0}' caused scene '{1}' to activate.", trigger.Name, trigger.scene.friendly_name), "TRIGGER");
                                                Core.Logger.WriteToLog(Urgency.INFO, trigger.scene.RunScene(db), "TRIGGER");
                                            }
                                        }
                                        else
                                            Core.Logger.WriteToLog(Urgency.INFO, string.Format("Trigger '{0}' failed to evaluate. Make sure the trigger value and device value is numeric.", trigger.Name), "TRIGGER");

                                        break;
                                    }
                                case device_value_triggers.TRIGGER_OPERATORS.LessThan:
                                    {
                                        double deviceValue = 0;
                                        double triggerValue = 0;

                                        if (double.TryParse(dv.value2, out deviceValue) && double.TryParse(trigger.trigger_value, out triggerValue))
                                        {
                                            if (deviceValue < triggerValue)
                                            {
                                                Core.Logger.WriteToLog(Urgency.INFO, string.Format("Trigger '{0}' caused scene '{1}' to activate.", trigger.Name, trigger.scene.friendly_name), "TRIGGER");
                                                Core.Logger.WriteToLog(Urgency.INFO, trigger.scene.RunScene(db), "TRIGGER");
                                            }
                                        }
                                        else
                                            Core.Logger.WriteToLog(Urgency.INFO, string.Format("Trigger '{0}' failed to evaluate. Make sure the trigger value and device value is numeric.", trigger.Name), "TRIGGER");

                                        break;
                                    }
                                case device_value_triggers.TRIGGER_OPERATORS.NotEqualTo:
                                    {
                                        if (!dv.value2.Equals(trigger.trigger_value))
                                        {
                                            Core.Logger.WriteToLog(Urgency.INFO, string.Format("Trigger '{0}' caused scene '{1}' to activate.", trigger.Name, trigger.scene.friendly_name), "TRIGGER");
                                            Core.Logger.WriteToLog(Urgency.INFO, trigger.scene.RunScene(db), "TRIGGER");
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


    }
}
