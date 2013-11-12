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
    public class PluginSettingBuilder
    {
        public Core Core { get; private set; }
        public zvsContext Context { get; private set; }

        public PluginSettingBuilder(Core core, zvsContext context)
        {
            Core = core;
            Context = context;
        }

        public PlugingTypeConfiguration<T> Plugin<T>(T plugin) where T : zvsPlugin
        {
            return new PlugingTypeConfiguration<T>(plugin, this);
        }

        public class PlugingTypeConfiguration<T> where T : zvsPlugin
        {
            public T Plugin { get; private set; }
            public PluginSettingBuilder PluginSettingBuilder { get; private set; }

            public PlugingTypeConfiguration(T plugin, PluginSettingBuilder sb)
            {
                Plugin = plugin;
                PluginSettingBuilder = sb;
            }
        }
    }

    public static class PlugingTypeConfigurationExtensions
    {
        public static async Task RegisterPluginSettingAsync<T, R>(this zvs.Processor.PluginSettingBuilder.PlugingTypeConfiguration<T> pspb, PluginSetting pluginSetting,
            Expression<Func<T, R>> property) where T : zvsPlugin
        {
            var propertyInfo = (property.Body as MemberExpression).Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException("The lambda expression 'property' should point to a valid Property");
            }

            pluginSetting.UniqueIdentifier = propertyInfo.Name;

            var plugin = await pspb.PluginSettingBuilder.Context.Plugins.FirstOrDefaultAsync(p => p.PluginGuid == pspb.Plugin.PluginGuid);

            if (plugin == null)
                return;

            var changed = false;

            var existing_ps = await pspb.PluginSettingBuilder.Context.PluginSettings.FirstOrDefaultAsync(o => o.Plugin.Id == plugin.Id && o.UniqueIdentifier == pluginSetting.UniqueIdentifier);
            if (existing_ps == null)
            {
                plugin.Settings.Add(pluginSetting);
                changed = true;
            }
            else
            {
                PropertyChangedEventHandler handler = (s, a) => changed = true;
                existing_ps.PropertyChanged += handler;

                existing_ps.Name = pluginSetting.Name;
                existing_ps.Description = pluginSetting.Description;
                existing_ps.ValueType = pluginSetting.ValueType;
                existing_ps.Value = pluginSetting.Value;
                existing_ps.PropertyChanged -= handler;

                foreach (var option in pluginSetting.Options)
                {
                    if (!existing_ps.Options.Any(o => o.Name == option.Name))
                    {
                        existing_ps.Options.Add(option);
                        changed = true;
                    }
                }

                foreach (var option in existing_ps.Options)
                {
                    if (!pluginSetting.Options.Any(o => o.Name == option.Name))
                    {
                        pspb.PluginSettingBuilder.Context.PluginSettingOptions.Local.Remove(option);
                        changed = true;
                    }
                }
            }
            if (changed)
            {
                var result = await pspb.PluginSettingBuilder.Context.TrySaveChangesAsync();
                if (result.HasError)
                    pspb.PluginSettingBuilder.Core.log.Error(result.Message);
            }
        }
    }
}
