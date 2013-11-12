using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zvs.Entities;
using System.Data.Entity;
using System.ComponentModel;

namespace zvs.Processor
{
    public class DeviceTypeBuilder : AdapterBuilder
    {
        protected zvsContext Context { get; set; }
        public DeviceTypeBuilder(zvsAdapter zvsAdapter, Core core, zvsContext context)
            : base(zvsAdapter, core)
        {
            Context = context;
        }

        public async Task<int> RegisterAsync(DeviceType deviceType)
        {
            //Does device type exist? 
            var existing_dt = await Context.DeviceTypes.Include(o => o.Commands)
                .FirstOrDefaultAsync(o =>
                o.Adapter.AdapterGuid == Adapter.AdapterGuid
                && o.UniqueIdentifier == deviceType.UniqueIdentifier);

            var changed = false;
            if (existing_dt == null)
            {
                existing_dt = deviceType;
                var adapter = await Context.Adapters.FirstOrDefaultAsync(o => o.AdapterGuid == Adapter.AdapterGuid);

                if (adapter != null)
                {
                    adapter.DeviceTypes.Add(existing_dt);
                    changed = true;
                }
            }
            else
            {
                existing_dt.Name = deviceType.Name;
                existing_dt.ShowInList = deviceType.ShowInList;

                foreach (DeviceTypeCommand dtc in deviceType.Commands)
                {
                    DeviceTypeCommand existing_dtc = await Context.DeviceTypeCommands
                        .Include(o => o.Options)
                        .SingleOrDefaultAsync(o =>
                            o.DeviceTypeId == existing_dt.Id &&
                            o.UniqueIdentifier == dtc.UniqueIdentifier);

                    if (existing_dtc == null)
                    {
                        existing_dt.Commands.Add(dtc);
                        changed = true;
                    }
                    else
                    {

                        PropertyChangedEventHandler handler = (s, a) => changed = true;
                        existing_dtc.PropertyChanged += handler;

                        existing_dtc.Name = dtc.Name;
                        existing_dtc.Help = dtc.Help;
                        existing_dtc.CustomData1 = dtc.CustomData1;
                        existing_dtc.CustomData2 = dtc.CustomData2;
                        existing_dtc.ArgumentType = dtc.ArgumentType;
                        existing_dtc.Description = dtc.Description;

                        existing_dtc.PropertyChanged -= handler;

                        foreach (var option in dtc.Options)
                        {
                            if (!existing_dtc.Options.Any(o => o.Name == option.Name))
                            {
                                existing_dtc.Options.Add(option);
                                changed = true;
                            }
                        }

                        foreach (var option in existing_dtc.Options)
                        {
                            if (!dtc.Options.Any(o => o.Name == option.Name))
                            {
                                Context.CommandOptions.Local.Remove(option);
                                changed = true;
                            }
                        }
                    }
                }
            }

            if (changed)
            {
                var result = await Context.TrySaveChangesAsync();
                if (result.HasError)
                    Core.log.Error(result.Message);
            }

            return existing_dt.Id;
        }

    }

    public class CommandOptionComparer : IEqualityComparer<CommandOption>
    {
        public bool Equals(CommandOption x, CommandOption y)
        {
            return x.Name == y.Name;
        }

        public int GetHashCode(CommandOption obj)
        {
            return 37 * obj.Name.GetHashCode() + 19 * obj.Name.GetHashCode();
        }

    }
}
