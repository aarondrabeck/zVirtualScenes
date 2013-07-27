using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zvs.Entities;
using System.Data.Entity;
using System.Reflection;
using System.Linq.Expressions;

namespace zvs.Processor
{
    public class AdapterSettingBuilder
    {
        public Core Core { get; private set; }
        public zvsContext Context { get; private set; }

        public AdapterSettingBuilder(Core core, zvsContext context)
        {
            Core = core;
            Context = context;
        }

        public AdapterTypeConfiguration<T> Adapter<T>(T adapter) where T : zvsAdapter
        {
            return new AdapterTypeConfiguration<T>(adapter, this);
        }

        public class AdapterTypeConfiguration<T> where T : zvsAdapter
        {
            public T Adapter { get; private set; }
            public AdapterSettingBuilder AdapterSettingBuilder { get; private set; }

            public AdapterTypeConfiguration(T adapter, AdapterSettingBuilder sb)
            {
                Adapter = adapter;
                AdapterSettingBuilder = sb;
            }
        }
        
        public async Task RegisterDeviceSettingAsync(DeviceSetting deviceSetting)
        {
            var existing_dp = await Context.DeviceSettings
                .Include(o=> o.Options)
                .FirstOrDefaultAsync(d => d.UniqueIdentifier == deviceSetting.UniqueIdentifier);

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
            var existing_dp = await Context.DeviceTypeSettings
                .Include(o => o.Options)
                .FirstOrDefaultAsync(d =>
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

    public static class AdapterTypeConfigurationExtensions
    {
        public static async Task RegisterAdapterSettingAsync<T, R>(this zvs.Processor.AdapterSettingBuilder.AdapterTypeConfiguration<T> adsb, AdapterSetting adapterSetting, 
            Expression<Func<T, R>> property) where T : zvsAdapter
        {
            var propertyInfo = (property.Body as MemberExpression).Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException("The lambda expression 'property' should point to a valid Property");
            }

            adapterSetting.UniqueIdentifier = propertyInfo.Name;


            var adapter = await adsb.AdapterSettingBuilder.Context.Adapters.FirstOrDefaultAsync(p => p.AdapterGuid == adsb.Adapter.AdapterGuid);
            if (adapter == null)
                return;

            AdapterSetting existingAdapter = await adsb.AdapterSettingBuilder.Context.AdapterSettings.FirstOrDefaultAsync(o => o.Adapter.Id == adapter.Id &&
                o.UniqueIdentifier == adapterSetting.UniqueIdentifier);

            if (existingAdapter == null)
            {
                adapter.Settings.Add(adapterSetting);
            }
            else
            {
                existingAdapter.Description = adapterSetting.Description;
                existingAdapter.Name = adapterSetting.Name;
                existingAdapter.ValueType = adapterSetting.ValueType;
                adsb.AdapterSettingBuilder.Context.AdapterSettingOptions.RemoveRange(existingAdapter.Options);

                foreach (var option in existingAdapter.Options)
                    existingAdapter.Options.Add(option);
            }

            var result = await adsb.AdapterSettingBuilder.Context.TrySaveChangesAsync();
            if (result.HasError)
                adsb.AdapterSettingBuilder.Core.log.Error(result.Message);

        }
    }
}
