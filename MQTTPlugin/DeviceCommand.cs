using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTTPlugin
{
    public class DeviceCommand
    {
        public int DeviceId { get; set; }
        public int CommandId { get; set; }
        public string Argument1 { get; set; }
        public string Argument2 { get; set; }
    }
}
