using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zvs.Entities
{
    [Table("DeviceCommands", Schema = "ZVS")]
    public partial class DeviceCommand : Command
    {  
        public virtual Device Device { get; set; }

        public void Run(zvsContext context, string argument = "")
        {
            QueuedDeviceCommand cmd = new QueuedDeviceCommand { Command = this, Argument = argument, Device = this.Device };
            cmd.Run(context);
        }
    }
}
