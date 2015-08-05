using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using zvs.DataModel;

namespace zvs.Processor
{
    public class DeviceCommandBuilder
    {
        private IEntityContextConnection EntityContextConnection { get; }
        public DeviceCommandBuilder(IEntityContextConnection entityContextConnection)
        {
            if (entityContextConnection == null)
                throw new ArgumentNullException(nameof(entityContextConnection));

            EntityContextConnection = entityContextConnection;
        }
        public async Task<Result> RegisterAsync(int deviceId, DeviceCommand deviceCommand, CancellationToken cancellationToken)
        {
            if (deviceCommand == null)
                return Result.ReportError("Device command is null");

            using (var context = new ZvsContext(EntityContextConnection))
            {
                var device = await context.Devices.FirstOrDefaultAsync(o => o.Id == deviceId, cancellationToken);

                if(device == null )
                    return Result.ReportError("Invalid device id");

                //Does device type exist? 
                var existingDc = await context.DeviceCommands
                    .Include(o => o.Options)
                    .FirstOrDefaultAsync(c => c.UniqueIdentifier == deviceCommand.UniqueIdentifier &&
                                              c.DeviceId == deviceId, cancellationToken);

                if (existingDc == null)
                {
                    device.Commands.Add(deviceCommand);
                    return await context.TrySaveChangesAsync(cancellationToken);
                }

                var changed = false;
                PropertyChangedEventHandler handler = (s, a) => changed = true;
                existingDc.PropertyChanged += handler;

                existingDc.ArgumentType = deviceCommand.ArgumentType;
                existingDc.CustomData1 = deviceCommand.CustomData1;
                existingDc.CustomData2 = deviceCommand.CustomData2;
                existingDc.Description = deviceCommand.Description;
                existingDc.Name = deviceCommand.Name;
                existingDc.Help = deviceCommand.Help;
                existingDc.SortOrder = deviceCommand.SortOrder;

                existingDc.PropertyChanged -= handler;

                var addedOptions = deviceCommand.Options.Where(option => existingDc.Options.All(o => o.Name != option.Name)).ToList();
                foreach (var option in addedOptions)
                {
                    existingDc.Options.Add(option);
                    changed = true;
                }

                var removedOptions = existingDc.Options.Where(option => deviceCommand.Options.All(o => o.Name != option.Name)).ToList();
                foreach (var option in removedOptions)
                {
                    context.CommandOptions.Local.Remove(option);
                    changed = true;
                }

                if (changed)
                    return await context.TrySaveChangesAsync(cancellationToken);

                return Result.ReportSuccess("Nothing to update");
            }
        }
    }
}
