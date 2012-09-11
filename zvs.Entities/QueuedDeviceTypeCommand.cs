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
    public partial class QueuedDeviceTypeCommand : QueuedCommand
    {
        [Required]
        public virtual Device Device { get; set; }

        public static QueuedDeviceTypeCommand Create(DeviceTypeCommand dtc, Device d, string argument)
        {
            return new QueuedDeviceTypeCommand()
            {
                Device = d,                
                Command = dtc,
                Argument = argument
            };
        }

        //Methods
        public void Run(zvsContext context)
        {
            Run(this, context);
        }

        public static void Run(QueuedDeviceTypeCommand cmd, zvsContext context)
        {
            context.QueuedCommands.Add(cmd);
            context.SaveChanges();
            QueuedCommand.AddNewCommandCommand(new NewCommandArgs(cmd));
        }    
    }
}
