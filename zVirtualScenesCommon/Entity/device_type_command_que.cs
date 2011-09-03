using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;
using zVirtualScenesCommon;
using System.Runtime.Serialization;
using System.Data.Objects;

namespace zVirtualScenesCommon.Entity
{
    public partial class device_type_command_que : EntityObject
    {
        #region Events
        /// <summary>
        /// Called when a Builtin command is added to the que
        /// </summary>
        public static event DeviceTypeCommandAddedEventHandler DeviceTypeCommandAddedToQueEvent;
        public delegate void DeviceTypeCommandAddedEventHandler(device_type_command_que bcq);

        public static void DeviceTypeCommandAddedToQue(device_type_command_que bcq)
        {
            if (DeviceTypeCommandAddedToQueEvent != null)
                DeviceTypeCommandAddedToQueEvent(bcq);
        }

        /// <summary>
        /// Called after a command is executed
        /// </summary>
        public static event DeviceTypeCommandRunCompleteEventHandler DeviceTypeCommandRunCompleteEvent;
        public delegate void DeviceTypeCommandRunCompleteEventHandler(device_type_command_que cmd, bool withErrors, string txtError);

        public static void DeviceTypeCommandRunComplete(device_type_command_que cmd, bool withErrors, string txtError)
        {
            if (DeviceTypeCommandRunCompleteEvent != null)
                DeviceTypeCommandRunCompleteEvent(cmd, withErrors, txtError);
        }
        #endregion

        public static void Run(device_type_command_que cmd)
        {
            zvsEntityControl.zvsContext.device_type_command_que.AddObject(cmd);
            zvsEntityControl.zvsContext.SaveChanges();
            DeviceTypeCommandAddedToQue(cmd);
        }

    }
}
