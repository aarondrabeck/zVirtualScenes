using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using zvs.DataModel;
using System.Data.Entity;
using System.Reflection;
using System.Linq.Expressions;
using System.ComponentModel;

namespace zvs.Processor
{
    public class AdapterSettingBuilder
    {
        private IFeedback<LogEntry> Log { get; set; }
        private ZvsContext Context { get; set; }

        public AdapterSettingBuilder(IFeedback<LogEntry> log, ZvsContext context)
        {
            Log = log;
            Context = context;
        }

        public AdapterTypeConfiguration<T> Adapter<T>(T adapter) where T : ZvsAdapter
        {
            return new AdapterTypeConfiguration<T>(adapter, this);
        }

        public class AdapterTypeConfiguration<T> where T : ZvsAdapter
        {
            private T Adapter { get; set; }
            private AdapterSettingBuilder AdapterSettingBuilder { get; set; }

            public AdapterTypeConfiguration(T adapter, AdapterSettingBuilder sb)
            {
                Adapter = adapter;
                AdapterSettingBuilder = sb;
            }

            public async Task RegisterAdapterSettingAsync<T, R>(AdapterSetting adapterSetting,Expression<Func<T, R>> property, CancellationToken cancellationToken) where T : ZvsAdapter
            {
                var memberExpression = property.Body as MemberExpression;
                if (memberExpression != null)
                {
                    var propertyInfo = memberExpression.Member as PropertyInfo;
                    if (propertyInfo == null)
                    {
                        throw new ArgumentException("The lambda expression 'property' should point to a valid Property");
                    }

                    adapterSetting.UniqueIdentifier = propertyInfo.Name;
                }


                var adapter = await AdapterSettingBuilder.Context.Adapters.FirstOrDefaultAsync(p => p.AdapterGuid ==Adapter.AdapterGuid, cancellationToken);
                if (adapter == null)
                    return;

                var existingAdapter = await AdapterSettingBuilder.Context.AdapterSettings.FirstOrDefaultAsync(o => o.Adapter.Id == adapter.Id &&
                    o.UniqueIdentifier == adapterSetting.UniqueIdentifier, cancellationToken);
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

                    foreach (var option in adapterSetting.Options.Where(option => existingAdapter.Options.All(o => o.Name != option.Name)))
                    {
                        existingAdapter.Options.Add(option);
                        changed = true;
                    }

                    foreach (var option in existingAdapter.Options.Where(option => adapterSetting.Options.All(o => o.Name != option.Name)))
                    {
                        AdapterSettingBuilder.Context.AdapterSettingOptions.Local.Remove(option);
                        changed = true;
                    }
                }

                if (changed)
                {
                    var result = await AdapterSettingBuilder.Context.TrySaveChangesAsync(cancellationToken);
                    if (result.HasError)
                        await AdapterSettingBuilder.Log.ReportErrorAsync(result.Message,cancellationToken);
                }
            }
        }

        public async Task RegisterDeviceSettingAsync(DeviceSetting deviceSetting, CancellationToken cancellationToken)
        {
            var existingDp = await Context.DeviceSettings
                .Include(o => o.Options)
                .FirstOrDefaultAsync(d => d.UniqueIdentifier == deviceSetting.UniqueIdentifier, cancellationToken);

            var changed = false;
            if (existingDp == null)
            {
                Context.DeviceSettings.Add(deviceSetting);
                changed = true;
            }
            else
            {
                PropertyChangedEventHandler handler = (s, a) => changed = true;
                existingDp.PropertyChanged += handler;

                existingDp.Name = deviceSetting.Name;
                existingDp.Description = deviceSetting.Description;
                existingDp.ValueType = deviceSetting.ValueType;
                existingDp.Value = deviceSetting.Value;

                existingDp.PropertyChanged -= handler;

                foreach (var option in deviceSetting.Options.Where(option => existingDp.Options.All(o => o.Name != option.Name)))
                {
                    existingDp.Options.Add(option);
                    changed = true;
                }

                foreach (var option in existingDp.Options.Where(option => deviceSetting.Options.All(o => o.Name != option.Name)))
                {
                    Context.DeviceSettingOptions.Local.Remove(option);
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

        public async Task RegisterDeviceTypeSettingAsync(DeviceTypeSetting deviceTypeSetting, CancellationToken cancellationToken)
        {
            var existingDp = await Context.DeviceTypeSettings
                .Include(o => o.Options)
                .FirstOrDefaultAsync(d =>
                    d.UniqueIdentifier == deviceTypeSetting.UniqueIdentifier &&
                    d.DeviceTypeId == deviceTypeSetting.DeviceTypeId, cancellationToken);

            var changed = false;
            if (existingDp == null)
            {
                Context.DeviceTypeSettings.Add(deviceTypeSetting);
                changed = true;
            }
            else
            {
                PropertyChangedEventHandler handler = (s, a) => changed = true;
                existingDp.PropertyChanged += handler;

                existingDp.Name = deviceTypeSetting.Name;
                existingDp.Description = deviceTypeSetting.Description;
                existingDp.ValueType = deviceTypeSetting.ValueType;
                existingDp.Value = deviceTypeSetting.Value;

                existingDp.PropertyChanged -= handler;

                foreach (var option in deviceTypeSetting.Options.Where(option => existingDp.Options.All(o => o.Name != option.Name)))
                {
                    existingDp.Options.Add(option);
                    changed = true;
                }

                foreach (var option in existingDp.Options.Where(option => deviceTypeSetting.Options.All(o => o.Name != option.Name)))
                {
                    Context.DeviceSettingOptions.Local.Remove(option);
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
