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
        private ZvsContext Context { get; set; }

        public DeviceSettingBuilder(ZvsContext zvsContext)
        {
            Context = zvsContext;
        }

        public async Task RegisterAsync(DeviceSetting deviceSetting, CancellationToken cancellationToken)
        {
            if (deviceSetting == null)
            {
                return;
            }

            var setting = await Context.DeviceSettings
                .Include(o => o.Options)
                .FirstOrDefaultAsync(d => d.UniqueIdentifier == deviceSetting.UniqueIdentifier, cancellationToken);

            var changed = false;
            if (setting == null)
            {
                Context.DeviceSettings.Add(deviceSetting);
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

                foreach (var option in deviceSetting.Options.Where(option => setting.Options.All(o => o.Name != option.Name)))
                {
                    setting.Options.Add(option);
                    changed = true;
                }

                foreach (var option in setting.Options.Where(option => deviceSetting.Options.All(o => o.Name != option.Name)))
                {
                    Context.DeviceSettingOptions.Local.Remove(option);
                    changed = true;
                }
            }

            if (changed)
            {
                //return await Context.TrySaveChangesAsync(cancellationToken);
            }
        }
    }
}
