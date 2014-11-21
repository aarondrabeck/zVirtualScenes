using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Threading;
using zvs.DataModel;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace zvs.Processor
{
    public class PluginManager : IPluginManager
    {
        private IEnumerable<ZvsPlugin> Plugins { get; set; }
        private IEntityContextConnection EntityContextConnection { get; set; }
        private IFeedback<LogEntry> Log { get; set; }
        private IAdapterManager AdapterManager { get; set; }

        private readonly Dictionary<Guid, ZvsPlugin> _pluginLookup = new Dictionary<Guid, ZvsPlugin>();
        private readonly Dictionary<int, Guid> _pluginIdToGuid = new Dictionary<int, Guid>();

        public PluginManager(IEnumerable<ZvsPlugin> plugins, IEntityContextConnection entityContextConnection, IFeedback<LogEntry> log, IAdapterManager adapterManager)
        {
            if (plugins == null)
                throw new ArgumentNullException("plugins");

            if (entityContextConnection == null)
                throw new ArgumentNullException("entityContextConnection");

            if (log == null)
                throw new ArgumentNullException("log");

            if (adapterManager == null)
                throw new ArgumentNullException("adapterManager");

            EntityContextConnection = entityContextConnection;
            Log = log;
            Plugins = plugins;
            AdapterManager = adapterManager;

            Log.Source = "Plugin Manager";

            _pluginLookup = Plugins.ToDictionary(o => o.PluginGuid, o => o);
        }

        public async Task InitializePluginsAsync(CancellationToken cancellationToken)
        {
            using (var context = new ZvsContext(EntityContextConnection))
            {
                // Iterate the plugins found in dlls
                foreach (var plugin in Plugins)
                {
                    //keeps this plugin in scope 
                    var zvsPlugin = plugin;

                    //Check Database for this plugin
                    var dbPlugin = await context.Plugins
                        .FirstOrDefaultAsync(p => p.PluginGuid == zvsPlugin.PluginGuid, cancellationToken);

                    var changed = false;
                    if (dbPlugin == null)
                    {
                        dbPlugin = new Plugin { PluginGuid = zvsPlugin.PluginGuid };
                        context.Plugins.Add(dbPlugin);
                        changed = true;
                    }

                    //Update Name and Description
                    zvsPlugin.IsEnabled = dbPlugin.IsEnabled;

                    if (dbPlugin.Name != zvsPlugin.Name)
                    {
                        dbPlugin.Name = zvsPlugin.Name;
                        changed = true;
                    }

                    if (dbPlugin.Description != zvsPlugin.Description)
                    {
                        dbPlugin.Description = zvsPlugin.Description;
                        changed = true;
                    }

                    if (changed)
                    {
                        var result = await context.TrySaveChangesAsync(cancellationToken);
                        if (result.HasError)
                        {
                            await
                                Log.ReportErrorFormatAsync(cancellationToken,
                                    "Plugin not loaded. Error while saving loaded '{0}' plugin to database. {1}", zvsPlugin.Name, result.Message);
                            break;
                        }
                    }

                    await Log.ReportInfoFormatAsync(cancellationToken, "Initializing '{0}'", zvsPlugin.Name);

                    //Plug-in need access to the zvsEngine in order to use the Logger
                    var log = new DatabaseFeedback(EntityContextConnection) { Source = zvsPlugin.Name };
                    await zvsPlugin.Initialize(log, EntityContextConnection, AdapterManager);

                    //Reload just installed settings
                    var pluginSettings = await context.PluginSettings
                        .Include(o => o.Plugin)
                        .Where(p => p.Plugin.PluginGuid == zvsPlugin.PluginGuid)
                        .ToListAsync(cancellationToken);

                    //Set plug-in settings from database values
                    foreach (var setting in pluginSettings)
                    {
                        var prop = zvsPlugin.GetType().GetProperty(setting.UniqueIdentifier);
                        if (prop == null)
                        {
                            await
                                Log.ReportErrorFormatAsync(cancellationToken,
                                    "Cannot find property called {0} on this plugin", setting.UniqueIdentifier);
                            continue;
                        }

                        try
                        {
                            var convertedValue =
                                TypeDescriptor.GetConverter(prop.PropertyType).ConvertFrom(setting.Value);
                            prop.SetValue(zvsPlugin, convertedValue);
                        }
                        catch
                        {
                            Log.ReportErrorFormatAsync(cancellationToken, "Cannot cast value on plugin setting {0}", setting.UniqueIdentifier).Wait(cancellationToken);
                        }

                    }

                    if (dbPlugin.IsEnabled)
                        await zvsPlugin.StartAsync();

                    _pluginIdToGuid.Add(dbPlugin.Id, dbPlugin.PluginGuid);
                }
            }
            NotifyEntityChangeContext.ChangeNotifications<PluginSetting>.OnEntityUpdated += PluginManager_OnEntityUpdated;
        }


        public ZvsPlugin FindZvsPlugin(Guid pluginGuid)
        {
            return !_pluginLookup.ContainsKey(pluginGuid) ? null : _pluginLookup[pluginGuid];
        }

        public IReadOnlyList<ZvsPlugin> GetZvsPlugins()
        {
            return Plugins.ToList();
        }

        public async Task<Result> EnablePluginAsync(Guid pluginGuid, CancellationToken cancellationToken)
        {
            var plugin = FindZvsPlugin(pluginGuid);
            if (plugin == null)
                return Result.ReportErrorFormat("Unable to enable plugin with Guid of {0}", pluginGuid);

            plugin.IsEnabled = true;
            await plugin.StartAsync();

            using (var context = new ZvsContext(EntityContextConnection))
            {
                //Save Database Value
                var a = await context.Plugins.FirstOrDefaultAsync(o => o.PluginGuid == pluginGuid, cancellationToken);
                if (a != null)
                    a.IsEnabled = true;

                await context.TrySaveChangesAsync(cancellationToken);
            }
            return Result.ReportSuccess();
        }

        public async Task<Result> DisablePluginAsync(Guid pluginGuid, CancellationToken cancellationToken)
        {
            var plugin = FindZvsPlugin(pluginGuid);
            if (plugin == null)
                return Result.ReportErrorFormat("Unable to disable plugin with Guid of {0}", pluginGuid);

            plugin.IsEnabled = false;
            await plugin.StopAsync();

            using (var context = new ZvsContext(EntityContextConnection))
            {
                //Save Database Value
                var a = await context.Plugins.FirstOrDefaultAsync(o => o.PluginGuid == pluginGuid, cancellationToken);
                if (a != null)
                    a.IsEnabled = false;

                await context.TrySaveChangesAsync(cancellationToken);
            }
            return Result.ReportSuccess();
        }

        async void PluginManager_OnEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<PluginSetting>.EntityUpdatedArgs e)
        {
            var plugin = FindZvsPlugin(e.NewEntity.PluginId);
            if (plugin == null)
                return;

            await SetPluginProperty(plugin, e.NewEntity.UniqueIdentifier, e.NewEntity.Value, CancellationToken.None);
        }

        public ZvsPlugin FindZvsPlugin(int pluginId)
        {
            if (!_pluginIdToGuid.ContainsKey(pluginId))
                return null;

            var guid = _pluginIdToGuid[pluginId];
            return !_pluginLookup.ContainsKey(guid) ? null : _pluginLookup[guid];
        }

        private async Task SetPluginProperty(object zvsPlugin, string propertyName, object value, CancellationToken cancellationToken)
        {
            var prop = zvsPlugin.GetType().GetProperty(propertyName);
            if (prop == null)
            {
                await Log.ReportErrorFormatAsync(cancellationToken, "Cannot find property called {0} on this plug-in", propertyName);
                return;
            }

            try
            {
                var convertedValue = TypeDescriptor.GetConverter(prop.PropertyType).ConvertFrom(value);
                prop.SetValue(zvsPlugin, convertedValue);
            }
            catch
            {
                Log.ReportErrorFormatAsync(cancellationToken, "Cannot cast value on {0} on this plugin", propertyName).Wait(cancellationToken);
            }
        }


    }
}