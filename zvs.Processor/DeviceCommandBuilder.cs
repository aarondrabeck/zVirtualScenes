using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zvs.Entities;
using System.Data.Entity;

namespace zvs.Processor
{
    public class DeviceCommandBuilder : AdapterBuilder
    {
        public DeviceCommandBuilder(zvsAdapter zvsAdapter, Core core) : base(zvsAdapter, core) { }
        public async Task RegisterAsync(DeviceCommand deviceCommand, zvsContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (deviceCommand.Device == null)
            {
                Core.log.Error("You must send device when registering a device command!");
                return;
            }

            Device d = context.Devices.FirstOrDefault(o => o.Id == deviceCommand.Device.Id);

            if (d == null)
            {
                Core.log.ErrorFormat("Command builder cannot find device in database with id of {0}", deviceCommand.Device.Id);
                return;
            }

            //Does device type exist? 
            var existing_dc = await context.DeviceCommands.FirstOrDefaultAsync(c => c.UniqueIdentifier == deviceCommand.UniqueIdentifier &&
                c.DeviceId == d.Id);

            if (existing_dc == null)
            {
                d.Commands.Add(deviceCommand);
            }
            else
            {
                existing_dc.ArgumentType = deviceCommand.ArgumentType;
                existing_dc.CustomData1 = deviceCommand.CustomData1;
                existing_dc.CustomData2 = deviceCommand.CustomData2;
                existing_dc.Description = deviceCommand.Description;
                existing_dc.Name = deviceCommand.Name;
                existing_dc.Help = deviceCommand.Help;
                existing_dc.SortOrder = deviceCommand.SortOrder;
                context.CommandOptions.RemoveRange(existing_dc.Options.ToList());
                existing_dc.Options.Clear();
                deviceCommand.Options.ToList().ForEach(o => existing_dc.Options.Add(o));
            }

            var result = await context.TrySaveChangesAsync();
            if (result.HasError)
                Core.log.Error(result.Message);

        }
    }
}
