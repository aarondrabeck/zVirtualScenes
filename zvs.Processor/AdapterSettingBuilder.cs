using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using zvs.DataModel;

namespace zvs.Processor
{
    public class AdapterSettingBuilder
    {
        private IEntityContextConnection EntityContextConnection { get; }
        private CancellationToken CancellationToken { get; }
        public AdapterSettingBuilder(IEntityContextConnection entityContextConnection, CancellationToken cancellationToken)
        {
            if (entityContextConnection == null)
                throw new ArgumentNullException(nameof(entityContextConnection));

            EntityContextConnection = entityContextConnection;
            CancellationToken = cancellationToken;
        }

        public AdapterTypeConfiguration<T> Adapter<T>(T adapter) where T : ZvsAdapter
        {
            return new AdapterTypeConfiguration<T>(adapter, this);
        }

        public class AdapterTypeConfiguration<T> where T : ZvsAdapter
        {
            private T Adapter { get; }
            private AdapterSettingBuilder AdapterSettingBuilder { get; }

            public AdapterTypeConfiguration(T adapter, AdapterSettingBuilder sb)
            {
                Adapter = adapter;
                AdapterSettingBuilder = sb;
            }

            public async Task<Result> RegisterAdapterSettingAsync(AdapterSetting adapterSetting, Expression<Func<T, Object>> property)
            {
                MemberExpression memberExpression = (property.Body.NodeType == ExpressionType.Convert) ? (MemberExpression)((UnaryExpression)property.Body).Operand : property.Body as MemberExpression;
                if (memberExpression != null)
                {
                    var propertyInfo = memberExpression.Member as PropertyInfo;
                    if (propertyInfo == null)
                    {
                        throw new ArgumentException("The lambda expression 'property' should point to a valid Property");
                    }

                    adapterSetting.UniqueIdentifier = propertyInfo.Name;
                }

                using (var context = new ZvsContext(AdapterSettingBuilder.EntityContextConnection))
                {
                    var adapter =
                        await
                            context.Adapters.FirstOrDefaultAsync(p => p.AdapterGuid == Adapter.AdapterGuid,
                                AdapterSettingBuilder.CancellationToken);
                    if (adapter == null)
                        return Result.ReportError("Invalid Adapter");

                    var existingAdapter = await context.AdapterSettings.FirstOrDefaultAsync(
                                o => o.Adapter.Id == adapter.Id && o.UniqueIdentifier == adapterSetting.UniqueIdentifier,
                                AdapterSettingBuilder.CancellationToken);
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

                        var added = adapterSetting.Options.Where(
                            option => existingAdapter.Options.All(o => o.Name != option.Name)).ToList();
                        foreach (var option in added)
                        {
                            existingAdapter.Options.Add(option);
                            changed = true;
                        }

                        var removed = existingAdapter.Options.Where(
                            option => adapterSetting.Options.All(o => o.Name != option.Name)).ToList();
                        foreach (var option in removed)
                        {
                            context.AdapterSettingOptions.Local.Remove(option);
                            changed = true;
                        }
                    }

                    if (changed)
                        return await context.TrySaveChangesAsync(AdapterSettingBuilder.CancellationToken);
                }
                return Result.ReportSuccess("Nothing to update");
            }
        }

        public async Task<Result> RegisterDeviceSettingAsync(DeviceSetting deviceSetting)
        {
            using (var context = new ZvsContext(EntityContextConnection))
            {
                var existingDp = await context.DeviceSettings
                    .Include(o => o.Options)
                    .FirstOrDefaultAsync(d => d.UniqueIdentifier == deviceSetting.UniqueIdentifier, CancellationToken);

                var changed = false;
                if (existingDp == null)
                {
                    context.DeviceSettings.Add(deviceSetting);
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

                    var added = deviceSetting.Options.Where(option => existingDp.Options.All(o => o.Name != option.Name)).ToList();
                    foreach (var option in added)
                    {
                        existingDp.Options.Add(option);
                        changed = true;
                    }

                    var removed = existingDp.Options.Where(option => deviceSetting.Options.All(o => o.Name != option.Name)).ToList();
                    foreach (var option in removed)
                    {
                        context.DeviceSettingOptions.Local.Remove(option);
                        changed = true;
                    }
                }

                if (changed)
                    return await context.TrySaveChangesAsync(CancellationToken);
            }
            return Result.ReportSuccess("Nothing to update");
        }

        public async Task<Result> RegisterDeviceTypeSettingAsync(DeviceTypeSetting deviceTypeSetting)
        {
            using (var context = new ZvsContext(EntityContextConnection))
            {
                var existingDp = await context.DeviceTypeSettings
                    .Include(o => o.Options)
                    .FirstOrDefaultAsync(d =>
                        d.UniqueIdentifier == deviceTypeSetting.UniqueIdentifier &&
                        d.DeviceTypeId == deviceTypeSetting.DeviceTypeId, CancellationToken);

                var changed = false;
                if (existingDp == null)
                {
                    context.DeviceTypeSettings.Add(deviceTypeSetting);
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

                    var added = deviceTypeSetting.Options.Where(option => existingDp.Options.All(o => o.Name != option.Name)).ToList();
                    foreach (var option in added)
                    {
                        existingDp.Options.Add(option);
                        changed = true;
                    }

                    var removed = existingDp.Options.Where(option => deviceTypeSetting.Options.All(o => o.Name != option.Name)).ToList();
                    foreach (var option in removed)
                    {
                        context.DeviceTypeSettingOptions.Local.Remove(option);
                        changed = true;
                    }
                }

                if (changed)
                    return await context.TrySaveChangesAsync(CancellationToken);
            }
            return Result.ReportSuccess("Nothing to update");
        }
    }
}
