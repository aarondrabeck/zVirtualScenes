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
    public partial class QueuedJavaScriptCommand : QueuedCommand
    {   
        //Methods
        public void Run(zvsContext context)
        {
            Run(this, context);
        }

        public static QueuedJavaScriptCommand Create(JavaScriptCommand command)
        {
            return new QueuedJavaScriptCommand()
            {
                Command = command
            };
        }

        public static void Run(QueuedJavaScriptCommand cmd, zvsContext context)
        {
            context.QueuedCommands.Add(cmd);
            context.SaveChanges();
            QueuedCommand.AddNewCommandCommand(new NewCommandArgs(cmd));
        } 
    }
}
