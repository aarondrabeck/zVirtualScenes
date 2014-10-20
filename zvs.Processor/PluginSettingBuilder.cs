using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using zvs.DataModel;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Reflection;
using System.ComponentModel;

namespace zvs.Processor
{
    public class PluginSettingBuilder
    {
        private IFeedback<LogEntry> Log { get; set; }
        private ZvsContext Context { get; set; }

        public PluginSettingBuilder(IFeedback<LogEntry> log, ZvsContext zvsContext)
        {
            Context = zvsContext;
            Log = log;
        }

        public PlugingTypeConfiguration<T> Plugin<T>(T plugin) where T : zvsPlugin
        {
            return new PlugingTypeConfiguration<T>(plugin, this);
        }

        public class PlugingTypeConfiguration<T> where T : zvsPlugin
        {
            private T Plugin { get; set; }
            private PluginSettingBuilder PluginSettingBuilder { get; set; }

            public PlugingTypeConfiguration(T plugin, PluginSettingBuilder sb)
            {
                Plugin = plugin;
                PluginSettingBuilder = sb;
            }


            public async Task RegisterPluginSettingAsync<T, R>(PluginSetting pluginSetting,
               Expression<Func<T, R>> property, CancellationToken cancellationToken) where T : zvsPlugin
            {
                var memberExpression = property.Body as MemberExpression;
                if (memberExpression != null)
                {
                    var propertyInfo = memberExpression.Member as PropertyInfo;
                    if (propertyInfo == null)
                    {
                        throw new ArgumentException("The lambda expression 'property' should point to a valid Property");
                    }

                    pluginSetting.UniqueIdentifier = propertyInfo.Name;
                }

                var plugin = await PluginSettingBuilder.Context.Plugins.FirstOrDefaultAsync(p => p.PluginGuid == Plugin.PluginGuid, cancellationToken);

                if (plugin == null)
                    return;

                var changed = false;

                var existingPs = await PluginSettingBuilder.Context.PluginSettings.FirstOrDefaultAsync(o => o.Plugin.Id == plugin.Id && o.UniqueIdentifier == pluginSetting.UniqueIdentifier, cancellationToken);
                if (existingPs == null)
                {
                    plugin.Settings.Add(pluginSetting);
                    changed = true;
                }
                else
                {
                    PropertyChangedEventHandler handler = (s, a) => changed = true;
                    existingPs.PropertyChanged += handler;

                    existingPs.Name = pluginSetting.Name;
                    existingPs.Description = pluginSetting.Description;
                    existingPs.ValueType = pluginSetting.ValueType;
                    existingPs.PropertyChanged -= handler;

                    foreach (var option in pluginSetting.Options.Where(option => existingPs.Options.All(o => o.Name != option.Name)))
                    {
                        existingPs.Options.Add(option);
                        changed = true;
                    }

                    foreach (var option in existingPs.Options.Where(option => pluginSetting.Options.All(o => o.Name != option.Name)))
                    {
                        PluginSettingBuilder.Context.PluginSettingOptions.Local.Remove(option);
                        changed = true;
                    }
                }
                if (changed)
                {
                    var result = await PluginSettingBuilder.Context.TrySaveChangesAsync(cancellationToken);
                    if (result.HasError)
                        await PluginSettingBuilder.Log.ReportErrorAsync(result.Message,cancellationToken);
                }
            }
        }

    }
}
