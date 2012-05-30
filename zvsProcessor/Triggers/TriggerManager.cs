using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using zVirtualScenesModel;

namespace zVirtualScenes.Triggers
{
    public class TriggerManager : IDisposable
    {
        private Core Core;
        private zvsLocalDBEntities context;

        public TriggerManager(Core Core)
        {
            this.Core = Core;
            context = new zvsLocalDBEntities();
            device_values.DeviceValueDataChangedEvent += new device_values.ValueDataChangedEventHandler(device_values_DeviceValueDataChangedEvent);
        }

        public void Dispose()
        {
            device_values.DeviceValueDataChangedEvent -= new device_values.ValueDataChangedEventHandler(device_values_DeviceValueDataChangedEvent);                
        }

        private void device_values_DeviceValueDataChangedEvent(object sender, device_values.ValueDataChangedEventArgs args)
        {
            device_values dv = context.device_values.Local.FirstOrDefault(v => v.id == args.device_value_id);
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
                                        Core.Logger.WriteToLog(Urgency.INFO, trigger.scene.RunScene(), "TRIGGER");
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
                                            Core.Logger.WriteToLog(Urgency.INFO, trigger.scene.RunScene(), "TRIGGER");
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
                                            Core.Logger.WriteToLog(Urgency.INFO, trigger.scene.RunScene(), "TRIGGER");
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
                                        Core.Logger.WriteToLog(Urgency.INFO, trigger.scene.RunScene(), "TRIGGER");
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
