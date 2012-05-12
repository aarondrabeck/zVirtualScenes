using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;

namespace zVirtualScenesCommon.Entity
{
    public partial class device_type_commands : EntityObject
    {
        public void Run(int device_id, string argument = "")
        {
            device_type_command_que cmd = new device_type_command_que { device_type_command_id = this.id, arg = argument, device_id = device_id };
            cmd.Run();
        }       
        
    }
}
