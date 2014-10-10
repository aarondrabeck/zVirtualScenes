using System;
using System.Linq;
using System.Threading.Tasks;
using zvs.Entities;
using System.Data.Entity;
using System.Reflection;
using System.Linq.Expressions;
using System.ComponentModel;

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
                .Include(o => o.Options)
                .FirstOrDefaultAsync(d => d.UniqueIdentifier == deviceSetting.UniqueIdentifier);

            var changed = false;
            if (existing_dp == null)
            {
                Context.DeviceSettings.Add(deviceSetting);
                changed = true;
            }
            else
            {
                PropertyChangedEventHandler handler = (s, a) => changed = true;
                existing_dp.PropertyChanged += handler;

                existing_dp.Name = deviceSetting.Name;
                existing_dp.Description = deviceSetting.Description;
                existing_dp.ValueType = deviceSetting.ValueType;
                existing_dp.Value = deviceSetting.Value;

                existing_dp.PropertyChanged -= handler;

                foreach (var option in deviceSetting.Options)
                {
                    if (!existing_dp.Options.Any(o => o.Name == option.Name))
                    {
                        existing_dp.Options.Add(option);
                        changed = true;
                    }
                }

                foreach (var option in existing_dp.Options)
                {
                    if (!deviceSetting.Options.Any(o => o.Name == option.Name))
                    {
                        Context.DeviceSettingOptions.Local.Remove(option);
                        changed = true;
                    }
                }
            }

            if (changed)
            {
                var result = await Context.TrySaveChangesAsync();
                if (result.HasError)
                    Core.log.Error(result.Message);
            }
        }

        public async Task RegisterDeviceTypeSettingAsync(DeviceTypeSetting deviceTypeSetting)
        {
            var existing_dp = await Context.DeviceTypeSettings
                .Include(o => o.Options)
                .FirstOrDefaultAsync(d =>
                    d.UniqueIdentifier == deviceTypeSetting.UniqueIdentifier &&
                    d.DeviceTypeId == deviceTypeSetting.DeviceTypeId);

            var changed = false;
            if (existing_dp == null)
            {
                Context.DeviceTypeSettings.Add(deviceTypeSetting);
                changed = true;
            }
            else
            {
                PropertyChangedEventHandler handler = (s, a) => changed = true;
                existing_dp.PropertyChanged += handler;

                existing_dp.Name = deviceTypeSetting.Name;
                existing_dp.Description = deviceTypeSetting.Description;
                existing_dp.ValueType = deviceTypeSetting.ValueType;
                existing_dp.Value = deviceTypeSetting.Value;

                existing_dp.PropertyChanged -= handler;

                foreach (var option in deviceTypeSetting.Options)
                {
                    if (!existing_dp.Options.Any(o => o.Name == option.Name))
                    {
                        existing_dp.Options.Add(option);
                        changed = true;
                    }
                }

                foreach (var option in existing_dp.Options)
                {
                    if (!deviceTypeSetting.Options.Any(o => o.Name == option.Name))
                    {
                        Context.DeviceSettingOptions.Local.Remove(option);
                        changed = true;
                    }
                }
            }

            if (changed)
            {
                var result = await Context.TrySaveChangesAsync();
                if (result.HasError)
                    Core.log.Error(result.Message);
            }
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

            var existingAdapter = await adsb.AdapterSettingBuilder.Context.AdapterSettings.FirstOrDefaultAsync(o => o.Adapter.Id == adapter.Id &&
                o.UniqueIdentifier == adapterSetting.UniqueIdentifier);
            var changed = false;
            if (existingAdapter == null)
            {
                adapter.Settings.Add(adapterSetting);
                changed = true;
            }
            else
            {

                PropertyChangedEventHandler handler = (s, a) => changed = true;
                existingAdapter.PropertyChanged += handler;

                existingAdapter.Description = adapterSetting.Description;
                existingAdapter.Name = adapterSetting.Name;
                existingAdapter.ValueType = adapterSetting.ValueType;

                existingAdapter.PropertyChanged -= handler;

                foreach (var option in adapterSetting.Options)
                {
                    if (!existingAdapter.Options.Any(o => o.Name == option.Name))
                    {
                        existingAdapter.Options.Add(option);
                        changed = true;
                    }
                }

                foreach (var option in existingAdapter.Options)
                {
                    if (!adapterSetting.Options.Any(o => o.Name == option.Name))
                    {
                        adsb.AdapterSettingBuilder.Context.AdapterSettingOptions.Local.Remove(option);
                        changed = true;
                    }
                }
            }

            if (changed)
            {
                var result = await adsb.AdapterSettingBuilder.Context.TrySaveChangesAsync();
                if (result.HasError)
                    adsb.AdapterSettingBuilder.Core.log.Error(result.Message);
            }
        }
    }
}
