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
    public partial class QueuedDeviceCommand : QueuedCommand
    {
        public virtual Device Device { get; set; }

        public static QueuedDeviceCommand Create(DeviceCommand dc, string argument)
        {
            return new QueuedDeviceCommand()
            {
                Device = dc.Device,
                Command = dc,
                Argument = argument
            };
        }

        //Methods
        public void Run(zvsContext context)
        {
            Run(this, context);
        }

        public static void Run(QueuedDeviceCommand cmd, zvsContext context)
        {
            context.QueuedCommands.Add(cmd);
            context.SaveChanges();
            QueuedCommand.AddNewCommandCommand(new NewCommandArgs(cmd));
        }
    }
}
