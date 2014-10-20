using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using zvs.DataModel;
using System.Data.Entity;
using System.ComponentModel;

namespace zvs.Processor
{
    public class DeviceCommandBuilder
    {
        private IFeedback<LogEntry> Log { get; set; }
        private ZvsContext Context { get; set; }
        public DeviceCommandBuilder(IFeedback<LogEntry> log, ZvsContext zvsContext)
        {
            Context = zvsContext;
            Log = log;
        }
        public async Task RegisterAsync(DeviceCommand deviceCommand, CancellationToken cancellationToken)
        {
            if (deviceCommand == null || deviceCommand.Device == null)
            {
                await Log.ReportErrorAsync("You must send device when registering a device command!", cancellationToken);
                return;
            }

            if (deviceCommand.Device.Id == 0)
            {
                await Log.ReportErrorFormatAsync(cancellationToken, "Command builder cannot find device in database with id of {0}", deviceCommand.Device.Id);
                return;
            }

            //Does device type exist? 
            var existingDc = await Context.DeviceCommands
                .Include(o => o.Options)
                .FirstOrDefaultAsync(c => c.UniqueIdentifier == deviceCommand.UniqueIdentifier &&
                c.DeviceId == deviceCommand.DeviceId, cancellationToken);

            var changed = false;
            if (existingDc == null)
            {
                Context.DeviceCommands.Add(deviceCommand);
                changed = true;
            }
            else
            {
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

                foreach (var option in deviceCommand.Options.Where(option => existingDc.Options.All(o => o.Name != option.Name)))
                {
                    existingDc.Options.Add(option);
                    changed = true;
                }

                foreach (var option in existingDc.Options.Where(option => deviceCommand.Options.All(o => o.Name != option.Name)))
                {
                    Context.CommandOptions.Local.Remove(option);
                    changed = true;
                }
            }
            if (changed)
            {
                var result = await Context.TrySaveChangesAsync(cancellationToken);
                if (result.HasError)
                    await Log.ReportErrorAsync(result.Message, cancellationToken);
            }
        }
    }
}
