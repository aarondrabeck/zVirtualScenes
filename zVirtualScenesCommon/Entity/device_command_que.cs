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
    public partial class device_command_que : EntityObject
    {

        #region Events
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
        #endregion

        public void Run()
        {
            Run(this);
        }

        public static void Run(device_command_que cmd)
        {
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                db.device_command_que.AddObject(cmd);
                db.SaveChanges();
                DeviceCommandAddedToQue(cmd.id);
            }
        }
    }
}
