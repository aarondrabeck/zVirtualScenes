using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;
using zVirtualScenesCommon;
using System.Runtime.Serialization;
using System.Data.Objects;

namespace zVirtualScenesCommon.Entity
{
    public partial class device_value_triggers : EntityObject
    {
        public enum TRIGGER_OPERATORS
        {
            GreaterThan,
            LessThan,
            EqualTo,
            NotEqualTo
        }

        public string FriendlyName
        {
            get
            {
                string trigger_op_name = Enum.GetName(typeof(TRIGGER_OPERATORS), this.trigger_operator);

                return string.Format("When {0} {1} is {2} {3} activate scene '{4}'", this.device_values.device.friendly_name,
                                                                this.device_values.label_name,
                                                                trigger_op_name,
                                                                this.trigger_value,
                                                                this.scene.friendly_name);
            }
        }
    }
}
