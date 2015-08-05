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
    public class PluginSettingBuilder
    {
        private IEntityContextConnection EntityContextConnection { get; }
        private CancellationToken CancellationToken { get; }
        public PluginSettingBuilder(IEntityContextConnection entityContextConnection, CancellationToken cancellationToken)
        {
            if (entityContextConnection == null)
                throw new ArgumentNullException(nameof(entityContextConnection));

            EntityContextConnection = entityContextConnection;
            CancellationToken = cancellationToken;
        }

        public PluginTypeConfiguration<T> Plugin<T>(T plugin) where T : ZvsPlugin
        {
            return new PluginTypeConfiguration<T>(plugin, this);
        }

        public class PluginTypeConfiguration<T> where T : ZvsPlugin
        {
            private T Plugin { get; }
            private PluginSettingBuilder PluginSettingBuilder { get; }

            public PluginTypeConfiguration(T plugin, PluginSettingBuilder sb)
            {
                Plugin = plugin;
                PluginSettingBuilder = sb;
            }

            public async Task<Result> RegisterPluginSettingAsync(PluginSetting pluginSetting, Expression<Func<T, Object>> property)
            {
                MemberExpression memberExpression = (property.Body.NodeType == ExpressionType.Convert) ? (MemberExpression)((UnaryExpression)property.Body).Operand : property.Body as MemberExpression;
                if (memberExpression != null)
                {
                    var propertyInfo = memberExpression.Member as PropertyInfo;
                    if (propertyInfo == null)
                    {
                        throw new ArgumentException("The lambda expression 'property' should point to a valid Property");
                    }

                    pluginSetting.UniqueIdentifier = propertyInfo.Name;
                }

                using (var context = new ZvsContext(PluginSettingBuilder.EntityContextConnection))
                {
                    var plugin =
                        await
                            context.Plugins.FirstOrDefaultAsync(p => p.PluginGuid == Plugin.PluginGuid,
                                PluginSettingBuilder.CancellationToken);
                    if (plugin == null)
                        return Result.ReportError("Invalid Plugin");

                    var existingPlugin = await context.PluginSettings.FirstOrDefaultAsync(
                                o => o.Plugin.Id == plugin.Id && o.UniqueIdentifier == pluginSetting.UniqueIdentifier,
                                PluginSettingBuilder.CancellationToken);
                    var changed = false;
                    if (existingPlugin == null)
                    {
                        plugin.Settings.Add(pluginSetting);
                        changed = true;
                    }
                    else
                    {

                        PropertyChangedEventHandler handler = (s, a) => changed = true;
                        existingPlugin.PropertyChanged += handler;

                        existingPlugin.Description = pluginSetting.Description;
                        existingPlugin.Name = pluginSetting.Name;
                        existingPlugin.ValueType = pluginSetting.ValueType;

                        existingPlugin.PropertyChanged -= handler;

                        var added = pluginSetting.Options.Where(
                            option => existingPlugin.Options.All(o => o.Name != option.Name)).ToList();
                        foreach (var option in added)
                        {
                            existingPlugin.Options.Add(option);
                            changed = true;
                        }

                        var removed = existingPlugin.Options.Where(
                            option => pluginSetting.Options.All(o => o.Name != option.Name)).ToList();
                        foreach (var option in removed)
                        {
                            context.PluginSettingOptions.Local.Remove(option);
                            changed = true;
                        }
                    }

                    if (changed)
                        return await context.TrySaveChangesAsync(PluginSettingBuilder.CancellationToken);
                }
                return Result.ReportSuccess("Nothing to update");
            }
        }
    }
}
