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
    public partial class builtin_command_que : EntityObject
    {
        #region Events
        /// <summary>
        /// Called when a Builtin command is added to the que
        /// </summary>
        public static event BuiltinCommandAddedEventHandler BuiltinCommandAddedToQueEvent;
        public delegate void BuiltinCommandAddedEventHandler(builtin_command_que bcq);

        public static void BuiltinCommandAddedToQue(builtin_command_que bcq)
        {
            if (BuiltinCommandAddedToQueEvent != null)
                BuiltinCommandAddedToQueEvent(bcq);
        } 

        /// <summary>
        /// Called after a command is executed
        /// </summary>
        public static event BuiltinCommandRunCompleteEventHandler BuiltinCommandRunCompleteEvent;
        public delegate void BuiltinCommandRunCompleteEventHandler(builtin_command_que cmd, bool withErrors, string txtError);

        public static void BuiltinCommandRunComplete(builtin_command_que cmd, bool withErrors, string txtError)
        {
            if (BuiltinCommandRunCompleteEvent != null)
                BuiltinCommandRunCompleteEvent(cmd, withErrors, txtError);
        }
        #endregion

        public static void Run(builtin_command_que cmd)
        {
            zvsEntityControl.zvsContext.builtin_command_que.AddObject(cmd);
            zvsEntityControl.zvsContext.SaveChanges();
            BuiltinCommandAddedToQue(cmd);
        }
    }
}
