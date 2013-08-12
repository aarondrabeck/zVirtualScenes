using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zvs.Entities;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Reflection;

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

            if (setting == null)
            {
                Context.DeviceSettings.Add(deviceSetting);
            }
            else
            {
                setting.Name = deviceSetting.Name;
                setting.Description = deviceSetting.Description;
                setting.ValueType = deviceSetting.ValueType;
                setting.Value = deviceSetting.Value;

                Context.DeviceSettingOptions.RemoveRange(setting.Options.ToList());
                setting.Options.Clear();
                deviceSetting.Options.ToList().ForEach(o => setting.Options.Add(o));
            }

            var result = await Context.TrySaveChangesAsync();
            if (result.HasError)
                Core.log.Error(result.Message);
        }
    }
}
