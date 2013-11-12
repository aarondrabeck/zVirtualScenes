using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zvs.Entities;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Reflection;
using System.ComponentModel;

namespace zvs.Processor
{
    public class DeviceSettingBuilder
    {
        public Core Core { get; private set; }
        public zvsContext Context { get; private set; }

        public DeviceSettingBuilder(Core core, zvsContext context)
        {
            Core = core;
            Context = context;
        }

        public async Task RegisterAsync(DeviceSetting deviceSetting)
        {
            if (deviceSetting == null)
            {
                return;
            }

            DeviceSetting setting = await Context.DeviceSettings
                .Include(o => o.Options)
                .FirstOrDefaultAsync(d => d.UniqueIdentifier == deviceSetting.UniqueIdentifier);

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

                foreach (var option in deviceSetting.Options)
                {
                    if (!setting.Options.Any(o => o.Name == option.Name))
                    {
                        setting.Options.Add(option);
                        changed = true;
                    }
                }

                foreach (var option in setting.Options)
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
    }
}
