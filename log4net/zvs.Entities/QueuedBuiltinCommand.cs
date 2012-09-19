using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zvs.Entities
{
    public partial class QueuedBuiltinCommand : QueuedCommand
    {   
        //Methods
        public void Run(zvsContext context)
        {
            Run(this, context);
        }

        public static QueuedBuiltinCommand Create(BuiltinCommand command, string argument)
        {
            return new QueuedBuiltinCommand()
            {
                Command = command,
                Argument = argument
            };
        }

        public static void Run(QueuedBuiltinCommand cmd, zvsContext context)
        {
            context.QueuedCommands.Add(cmd);
            context.SaveChanges();
            QueuedCommand.AddNewCommandCommand(new NewCommandArgs(cmd));
        } 
    }
}
