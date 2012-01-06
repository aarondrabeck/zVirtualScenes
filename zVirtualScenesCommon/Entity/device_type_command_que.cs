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
        public delegate void DeviceTypeCommandAddedEventHandler(long device_type_command_que_id);

        public static void DeviceTypeCommandAddedToQue(long id)
        {
            if (DeviceTypeCommandAddedToQueEvent != null)
                DeviceTypeCommandAddedToQueEvent(id);
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

        public void Run()
        {
            Run(this);
        }

        public static void Run(device_type_command_que cmd)
        {
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                db.device_type_command_que.AddObject(cmd);
                db.SaveChanges();
                DeviceTypeCommandAddedToQue(cmd.id);
            }
        }

    }
}
