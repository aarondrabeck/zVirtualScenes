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
        private ZvsContext Context { get; set; }

        public DeviceTypeBuilder(IFeedback<LogEntry> log, ZvsContext zvsContext)
        {
            Context = zvsContext;
            Log = log;
        }

        public async Task<int> RegisterAsync(Guid adapterGuid, DeviceType deviceType, CancellationToken cancellationToken)
        {
            //Does device type exist? 
            var existingDt = await Context.DeviceTypes.Include(o => o.Commands)
                .FirstOrDefaultAsync(o =>
                o.Adapter.AdapterGuid == adapterGuid
                && o.UniqueIdentifier == deviceType.UniqueIdentifier, cancellationToken);

            var changed = false;
            if (existingDt == null)
            {
                existingDt = deviceType;
                var adapter = await Context.Adapters.FirstOrDefaultAsync(o => o.AdapterGuid == adapterGuid, cancellationToken);

                if (adapter != null)
                {
                    adapter.DeviceTypes.Add(existingDt);
                    changed = true;
                }
            }
            else
            {
                existingDt.Name = deviceType.Name;
                existingDt.ShowInList = deviceType.ShowInList;

                foreach (var dtc in deviceType.Commands)
                {
                    var dtc1 = dtc;
                    var existingDtc = await Context.DeviceTypeCommands
                        .Include(o => o.Options)
                        .SingleOrDefaultAsync(o =>
                            o.DeviceTypeId == existingDt.Id &&
                            o.UniqueIdentifier == dtc1.UniqueIdentifier,cancellationToken);

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

                        foreach (var option in dtc.Options.Where(option => existingDtc.Options.All(o => o.Name != option.Name)))
                        {
                            existingDtc.Options.Add(option);
                            changed = true;
                        }

                        foreach (var option in existingDtc.Options.Where(option => dtc1.Options.All(o => o.Name != option.Name)))
                        {
                            Context.CommandOptions.Local.Remove(option);
                            changed = true;
                        }
                    }
                }
            }

            if (changed)
            {
                var result = await Context.TrySaveChangesAsync(cancellationToken);
                if (result.HasError)
                    await Log.ReportErrorAsync(result.Message, cancellationToken);
            }

            return existingDt.Id;
        }

    }
}
