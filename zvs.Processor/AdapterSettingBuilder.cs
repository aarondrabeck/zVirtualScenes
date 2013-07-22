using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zvs.Entities;
using System.Data.Entity;

namespace zvs.Processor
{
    public class AdapterSettingBuilder : AdapterBuilder
    {
        protected zvsContext Context { get; set; }
        public AdapterSettingBuilder(zvsAdapter zvsAdapter, Core core, zvsContext context)
            : base(zvsAdapter, core)
        {
            Context = context;
        }

        public async Task RegisterAdapterSettingAsync(AdapterSetting adapterSetting)
        {
            var adapter = await Context.Adapters.FirstOrDefaultAsync(p => p.AdapterGuid == Adapter.AdapterGuid);
            if (adapter != null)
            {
                AdapterSetting existing_ps = await Context.AdapterSettings.FirstOrDefaultAsync(o => o.Adapter.Id == adapter.Id && o.UniqueIdentifier == adapterSetting.UniqueIdentifier);
                if (existing_ps == null)
                {
                    adapter.Settings.Add(adapterSetting);
                }
                else
                {
                    existing_ps.Description = adapterSetting.Description;
                    existing_ps.Name = adapterSetting.Name;
                    existing_ps.ValueType = adapterSetting.ValueType;
                    existing_ps.Options = adapterSetting.Options;
                }

                var result = await Context.TrySaveChangesAsync();
                if (result.HasError)
                    Core.log.Error(result.Message);
            }
        }

        public async Task RegisterDeviceSettingAsync(DeviceSetting deviceSetting)
        {
            var existing_dp = await Context.DeviceSettings.FirstOrDefaultAsync(d => d.UniqueIdentifier == deviceSetting.UniqueIdentifier);

            if (existing_dp == null)
            {
                Context.DeviceSettings.Add(deviceSetting);
            }
            else
            {
                existing_dp.Name = deviceSetting.Name;
                existing_dp.Description = deviceSetting.Description;
                existing_dp.ValueType = deviceSetting.ValueType;
                existing_dp.Value = deviceSetting.Value;

                Context.DeviceSettingOptions.RemoveRange(existing_dp.Options.ToList());
                existing_dp.Options.Clear();
                deviceSetting.Options.ToList().ForEach(o => existing_dp.Options.Add(o));
            }

            var result = await Context.TrySaveChangesAsync();
            if (result.HasError)
                Core.log.Error(result.Message);
        }

        public async Task RegisterDeviceTypeSettingAsync(DeviceTypeSetting deviceTypeSetting)
        {
            var existing_dp = await Context.DeviceTypeSettings.FirstOrDefaultAsync(d =>
                d.UniqueIdentifier == deviceTypeSetting.UniqueIdentifier &&
                d.DeviceTypeId == deviceTypeSetting.DeviceTypeId);

            if (existing_dp == null)
            {
                Context.DeviceTypeSettings.Add(deviceTypeSetting);
            }
            else
            {
                existing_dp.Name = deviceTypeSetting.Name;
                existing_dp.Description = deviceTypeSetting.Description;
                existing_dp.ValueType = deviceTypeSetting.ValueType;
                existing_dp.Value = deviceTypeSetting.Value;

                Context.DeviceSettingOptions.RemoveRange(existing_dp.Options.ToList());
                existing_dp.Options.Clear();
                deviceTypeSetting.Options.ToList().ForEach(o => existing_dp.Options.Add(o));
            }

            var result = await Context.TrySaveChangesAsync();
            if (result.HasError)
                Core.log.Error(result.Message);
        }

    }
}
