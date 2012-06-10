using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace zVirtualScenesModel
{
    public partial class device_type_command_que
    {   
        //Events
        /// <summary>
        /// Called when a Builtin command is added to the que
        /// </summary>
        public static event DeviceTypeCommandAddedEventHandler DeviceTypeCommandAddedToQueEvent;
        public delegate void DeviceTypeCommandAddedEventHandler(int device_type_command_que_id);

        public static void DeviceTypeCommandAddedToQue(int id)
        {
            if (DeviceTypeCommandAddedToQueEvent != null)
                DeviceTypeCommandAddedToQueEvent(id);
        }
       

        //Methods
        public void Run(zvsLocalDBEntities context)
        {
            Run(this, context);
        }

        public static void Run(device_type_command_que cmd, zvsLocalDBEntities context)
        {
            context.device_type_command_que.Add(cmd);
            context.SaveChanges();
            DeviceTypeCommandAddedToQue(cmd.id);
        }        
    }
}
