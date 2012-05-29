using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace zVirtualScenesModel
{
    public partial class device_value_triggers
    {
        public enum TRIGGER_OPERATORS
        {
            GreaterThan,
            LessThan,
            EqualTo,
            NotEqualTo
        }

        public enum TRIGGER_TYPE
        {
            Basic,
            Advanced
        }

        public string FriendlyName
        {
            get
            {
                if ((TRIGGER_TYPE)this.trigger_type == TRIGGER_TYPE.Basic)
                {
                    string trigger_op_name = Enum.GetName(typeof(TRIGGER_OPERATORS), this.trigger_operator);

                    return string.Format("When {0} {1} is {2} {3} activate scene '{4}'", this.device_values.device.friendly_name,
                                                                    this.device_values.label_name,
                                                                    trigger_op_name,
                                                                    this.trigger_value,
                                                                    this.scene.friendly_name);
                }
                else
                    return "Advanced Trigger!"; // Temp fix to show something in the list for now till I figure out exactly what I should show.
            }
        }
    }
}
