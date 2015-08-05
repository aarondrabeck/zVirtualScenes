using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using zvs.DataModel;

namespace zvs.Processor
{
    public class PluginManager : IPluginManager
    {
        private IEnumerable<ZvsPlugin> Plugins { get; }
        private IEntityContextConnection EntityContextConnection { get; }
        private IFeedback<LogEntry> Log { get; }
        private IAdapterManager AdapterManager { get; }
        private bool IsRunning { get; set; }

        private readonly Dictionary<Guid, ZvsPlugin> _pluginLookup = new Dictionary<Guid, ZvsPlugin>();
        private readonly Dictionary<int, Guid> _pluginIdToGuid = new Dictionary<int, Guid>();

        public PluginManager(IEnumerable<ZvsPlugin> plugins, IEntityContextConnection entityContextConnection, IFeedback<LogEntry> log, IAdapterManager adapterManager)
        {
            if (plugins == null)
                throw new ArgumentNullException(nameof(plugins));

            if (entityContextConnection == null)
                throw new ArgumentNullException(nameof(entityContextConnection));

            if (log == null)
                throw new ArgumentNullException(nameof(log));

            if (adapterManager == null)
                throw new ArgumentNullException(nameof(adapterManager));

            EntityContextConnection = entityContextConnection;
            Log = log;
            Plugins = plugins;
            AdapterManager = adapterManager;

            Log.Source = "Plugin Manager";

            _pluginLookup = Plugins.ToDictionary(o => o.PluginGuid, o => o);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (IsRunning)
            {
                await
                    Log.ReportWarningAsync("Cannot start plugin manager because it is already running!",
                        cancellationToken);
                return;
            }
            IsRunning = true;

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
                        await SetPluginProperty(plugin, setting.UniqueIdentifier, setting.Value, CancellationToken.None);

                    if (dbPlugin.IsEnabled)
                        await zvsPlugin.StartAsync();

                    if (!_pluginIdToGuid.ContainsKey(dbPlugin.Id))
                        _pluginIdToGuid.Add(dbPlugin.Id, dbPlugin.PluginGuid);
                }
            }
            NotifyEntityChangeContext.ChangeNotifications<PluginSetting>.OnEntityUpdated += PluginManager_OnEntityUpdated;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (!IsRunning)
            {
                await
                    Log.ReportWarningAsync("Cannot stop plugin manager because it is not running!",
                        cancellationToken);
                return;
            }

            IsRunning = false;

            foreach (var plugin in Plugins)
            {
                await plugin.StopAsync();
            }

            NotifyEntityChangeContext.ChangeNotifications<PluginSetting>.OnEntityUpdated -= PluginManager_OnEntityUpdated;
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

        internal async Task SetPluginProperty(ZvsPlugin zvsPlugin, string propertyName, object value, CancellationToken cancellationToken)
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