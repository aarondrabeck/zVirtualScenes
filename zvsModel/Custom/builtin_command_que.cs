using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace zVirtualScenesModel
{
    public partial class builtin_command_que
    {
        //Events
        /// <summary>
        /// Called when a Builtin command is added to the que
        /// </summary>
        public static event BuiltinCommandAddedEventHandler BuiltinCommandAddedToQueEvent;
        public delegate void BuiltinCommandAddedEventHandler(int builtin_command_que_id);

        public static void BuiltinCommandAddedToQue(int id)
        {
            if (BuiltinCommandAddedToQueEvent != null)
                BuiltinCommandAddedToQueEvent(id);
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
        
        //Methods
        public void Run(zvsLocalDBEntities context)
        {
            Run(this, context);
        }

        public static void Run(builtin_command_que cmd, zvsLocalDBEntities context)
        {
            context.builtin_command_que.Add(cmd);
            context.SaveChanges();
            BuiltinCommandAddedToQue(cmd.id);
        }        
    }
}
