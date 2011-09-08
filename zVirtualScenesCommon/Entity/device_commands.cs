using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;

namespace zVirtualScenesCommon.Entity
{
    public partial class device_commands : EntityObject
    {
        public void Run(string argument = "")
        {
            device_command_que cmd = new device_command_que { device_command_id = this.id, arg = argument, device_id = this.device.id };
            cmd.Run();
        }       
        
    }
}
