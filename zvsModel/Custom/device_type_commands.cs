using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace zVirtualScenesModel
{
    public partial class device_type_commands
    {
        public void Run(zvsLocalDBEntities context, int device_id, string argument = "")
        {
            device_type_command_que cmd = new device_type_command_que { device_type_command_id = this.id, arg = argument, device_id = device_id };
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
