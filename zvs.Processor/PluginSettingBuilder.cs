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
        public static async Task RegisterPluginSettingAsync<T,R>(this zvs.Processor.PluginSettingBuilder.PlugingTypeConfiguration<T> pspb, PluginSetting pluginSetting, 
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

            var existing_ps = await pspb.PluginSettingBuilder.Context.PluginSettings.FirstOrDefaultAsync(o => o.Plugin.Id == plugin.Id && o.UniqueIdentifier == pluginSetting.UniqueIdentifier);
            if (existing_ps == null)
            {
                plugin.Settings.Add(pluginSetting);
            }
            else
            {
                existing_ps.Description = pluginSetting.Description;
                existing_ps.Name = pluginSetting.Name;
                existing_ps.ValueType = pluginSetting.ValueType;

                pspb.PluginSettingBuilder.Context.PluginSettingOptions.RemoveRange(existing_ps.Options);

                foreach (var option in pluginSetting.Options)
                    existing_ps.Options.Add(option);
            }

            var result = await pspb.PluginSettingBuilder.Context.TrySaveChangesAsync();
            if (result.HasError)
                pspb.PluginSettingBuilder.Core.log.Error(result.Message);
        }
    }
}
