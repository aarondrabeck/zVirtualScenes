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
    [Table("DeviceTypeCommands", Schema = "ZVS")]
    public partial class DeviceTypeCommand : Command
    {
        public virtual DeviceType DeviceType { get; set; }

        public void Run(zvsContext context, Device Device, string Argument = "")
        {
            QueuedDeviceTypeCommand cmd = new QueuedDeviceTypeCommand { Command = this, Argument = Argument, Device = Device };
            cmd.Run(context);
        }
    }
}
