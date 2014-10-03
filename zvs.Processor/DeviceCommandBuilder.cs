using System;
using System.Linq;
using System.Threading.Tasks;
using zvs.Entities;
using System.Data.Entity;
using System.ComponentModel;

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

            if (deviceCommand.Device.Id == 0)
            {
                Core.log.ErrorFormat("Command builder cannot find device in database with id of {0}", deviceCommand.Device.Id);
                return;
            }

            //Does device type exist? 
            var existing_dc = await context.DeviceCommands
                .Include(o => o.Options)
                .FirstOrDefaultAsync(c => c.UniqueIdentifier == deviceCommand.UniqueIdentifier &&
                c.DeviceId == deviceCommand.DeviceId);

            var changed = false;
            if (existing_dc == null)
            {
                context.DeviceCommands.Add(deviceCommand);
                changed = true;
            }
            else
            {
                PropertyChangedEventHandler handler = (s, a) => changed = true;
                existing_dc.PropertyChanged += handler;

                existing_dc.ArgumentType = deviceCommand.ArgumentType;
                existing_dc.CustomData1 = deviceCommand.CustomData1;
                existing_dc.CustomData2 = deviceCommand.CustomData2;
                existing_dc.Description = deviceCommand.Description;
                existing_dc.Name = deviceCommand.Name;
                existing_dc.Help = deviceCommand.Help;
                existing_dc.SortOrder = deviceCommand.SortOrder;

                existing_dc.PropertyChanged -= handler;

                foreach (var option in deviceCommand.Options)
                {
                    if (!existing_dc.Options.Any(o => o.Name == option.Name))
                    {
                        existing_dc.Options.Add(option);
                        changed = true;
                    }
                }

                foreach (var option in existing_dc.Options)
                {
                    if (!deviceCommand.Options.Any(o => o.Name == option.Name))
                    {
                        context.CommandOptions.Local.Remove(option);
                        changed = true;
                    }
                }
            }
            if (changed)
            {
                var result = await context.TrySaveChangesAsync();
                if (result.HasError)
                    Core.log.Error(result.Message);
            }
        }
    }
}
