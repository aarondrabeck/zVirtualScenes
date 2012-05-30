using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace zVirtualScenesModel
{
    public partial class device_commands
    {
        public void Run(zvsLocalDBEntities context, string argument = "")
        {
            device_command_que cmd = new device_command_que { device_command_id = this.id, arg = argument, device_id = this.device.id };
            cmd.Run(context);
        }

        public static event onContextUpdatedEventHandler onContextUpdated;
        public static void CallOnContextUpdated()
        {
            if (onContextUpdated != null)
                onContextUpdated(null, new EventArgs());
        }
        
    }
}
