using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace zVirtualScenesModel
{
    public partial class device_command_que
    {

        //Events
        /// <summary>
        /// Called when a Builtin command is added to the que
        /// </summary>
        public static event DeviceCommandAddedEventHandler DeviceCommandAddedToQueEvent;
        public delegate void DeviceCommandAddedEventHandler(int device_command_que_id);

        public static void DeviceCommandAddedToQue(int bcq)
        {
            if (DeviceCommandAddedToQueEvent != null)
                DeviceCommandAddedToQueEvent(bcq);
        }

        /// <summary>
        /// Called after a command is executed
        /// </summary>
        public static event DeviceCommandRunCompleteEventHandler DeviceCommandRunCompleteEvent;
        public delegate void DeviceCommandRunCompleteEventHandler(device_command_que cmd, bool withErrors, string txtError);

        public static void DeviceCommandRunComplete(device_command_que cmd, bool withErrors, string txtError)
        {
            if (DeviceCommandRunCompleteEvent != null)
                DeviceCommandRunCompleteEvent(cmd, withErrors, txtError);
        }

        //Methods
        public void Run(zvsLocalDBEntities context)
        {
            Run(this, context);
        }

        public static void Run(device_command_que cmd, zvsLocalDBEntities context)
        {
            context.device_command_que.Add(cmd);
            context.SaveChanges();
            DeviceCommandAddedToQue(cmd.id);
        }
    }
}
