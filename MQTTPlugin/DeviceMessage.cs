using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTTPlugin
{
    public class DeviceMessage
    {
        public string DeviceName { get; set; }
        public int DeviceId { get; set; }
        public int NodeNumber { get; set; }
        public string PropertyName { get; set; }
        public string PropertyType { get; set; }
        public string PropertyValue { get; set; }
    }
}
