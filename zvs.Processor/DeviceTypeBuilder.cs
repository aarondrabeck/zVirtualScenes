using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using zvs.DataModel;
using System.Data.Entity;
using System.ComponentModel;

namespace zvs.Processor
{
    public class DeviceTypeBuilder
    {
        private IFeedback<LogEntry> Log { get; set; }
        private IEntityContextConnection EntityContextConnection { get; set; }
        public DeviceTypeBuilder(IFeedback<LogEntry> log, IEntityContextConnection entityContextConnection)
        {
            if (log == null)
                throw new ArgumentNullException("log");

            if (entityContextConnection == null)
                throw new ArgumentNullException("entityContextConnection");

            EntityContextConnection = entityContextConnection;
            Log = log;
        }

        public async Task<Result> RegisterAsync(Guid adapterGuid, DeviceType deviceType,
            CancellationToken cancellationToken)
        {
            using (var context = new ZvsContext(EntityContextConnection))
            {
                //Does device type exist? 
                var existingDt = await context.DeviceTypes
                    .Include(o => o.Commands)
                    .FirstOrDefaultAsync(o =>
                        o.Adapter.AdapterGuid == adapterGuid
                        && o.UniqueIdentifier == deviceType.UniqueIdentifier, cancellationToken);

                if (existingDt == null)
                {
                    var adapter =
                        await context.Adapters.FirstOrDefaultAsync(o => o.AdapterGuid == adapterGuid, cancellationToken);
                    if (adapter == null)
                        return Result.ReportError("Invalid adapter GuId");

                    adapter.DeviceTypes.Add(deviceType);
                    return await context.TrySaveChangesAsync(cancellationToken);
                }

                var changed = false;

                PropertyChangedEventHandler handler2 = (s, a) => changed = true;
                existingDt.PropertyChanged += handler2;
                existingDt.Name = deviceType.Name;
                existingDt.ShowInList = deviceType.ShowInList;
                existingDt.PropertyChanged -= handler2;

                foreach (var dtc in deviceType.Commands)
                {
                    var dtc1 = dtc;
                    var existingDtc = await context.DeviceTypeCommands
                        .Include(o => o.Options)
                        .SingleOrDefaultAsync(o =>
                            o.DeviceTypeId == existingDt.Id &&
                            o.UniqueIdentifier == dtc1.UniqueIdentifier, cancellationToken);

                    if (existingDtc == null)
                    {
                        existingDt.Commands.Add(dtc);
                        changed = true;
                    }
                    else
                    {

                        PropertyChangedEventHandler handler = (s, a) => changed = true;
                        existingDtc.PropertyChanged += handler;
                        existingDtc.Name = dtc.Name;
                        existingDtc.Help = dtc.Help;
                        existingDtc.CustomData1 = dtc.CustomData1;
                        existingDtc.CustomData2 = dtc.CustomData2;
                        existingDtc.ArgumentType = dtc.ArgumentType;
                        existingDtc.Description = dtc.Description;
                        existingDtc.PropertyChanged -= handler;

                        var addedOptions = dtc.Options.Where(option => existingDtc.Options.All(o => o.Name != option.Name)).ToList();
                        foreach (var option in addedOptions)
                        {
                            existingDtc.Options.Add(option);
                            changed = true;
                        }

                        var removed = existingDtc.Options.Where(option => dtc1.Options.All(o => o.Name != option.Name)).ToList();
                        foreach (var option in removed)
                        {
                            context.CommandOptions.Local.Remove(option);
                            changed = true;
                        }
                    }
                }

                if (changed)
                    return await context.TrySaveChangesAsync(cancellationToken);

                return Result.ReportSuccess("Nothing to update");
            }
        }
    }
}
