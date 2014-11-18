using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using zvs.DataModel;
using System.Data.Entity;
using System.ComponentModel;

namespace zvs.Processor
{
    public class DeviceSettingBuilder
    {
        private IEntityContextConnection EntityContextConnection { get; set; }

        public DeviceSettingBuilder(IEntityContextConnection entityContextConnection)
        {
            if (entityContextConnection == null)
                throw new ArgumentNullException("entityContextConnection");

            EntityContextConnection = entityContextConnection;
        }

        public async Task<Result> RegisterAsync(DeviceSetting deviceSetting, CancellationToken cancellationToken)
        {
            if (deviceSetting == null)
                return Result.ReportError("deviceSetting is null");

            using (var context = new ZvsContext(EntityContextConnection))
            {
                var setting = await context.DeviceSettings
                    .Include(o => o.Options)
                    .FirstOrDefaultAsync(d => d.UniqueIdentifier == deviceSetting.UniqueIdentifier, cancellationToken);

                var changed = false;
                if (setting == null)
                {
                    context.DeviceSettings.Add(deviceSetting);
                    changed = true;
                }
                else
                {
                    PropertyChangedEventHandler handler = (s, a) => changed = true;
                    setting.PropertyChanged += handler;

                    setting.Name = deviceSetting.Name;
                    setting.Description = deviceSetting.Description;
                    setting.ValueType = deviceSetting.ValueType;
                    setting.Value = deviceSetting.Value;

                    setting.PropertyChanged -= handler;

                    var added = deviceSetting.Options.Where(option => setting.Options.All(o => o.Name != option.Name)).ToList();
                    foreach (var option in added)
                    {
                        setting.Options.Add(option);
                        changed = true;
                    }

                    var removed = setting.Options.Where(option => deviceSetting.Options.All(o => o.Name != option.Name)).ToList();
                    foreach (var option in removed)
                    {
                        context.DeviceSettingOptions.Local.Remove(option);
                        changed = true;
                    }
                }

                if (changed)
                    return await context.TrySaveChangesAsync(cancellationToken);

                return Result.ReportSuccess("Nothing to update");
            }
        }
    }
}
