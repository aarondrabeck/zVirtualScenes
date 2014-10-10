using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System;
using System.ComponentModel;
using System.Data.Entity;
using zvs.Entities;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace zvs.Processor
{
    public class PluginManager
    {
        public PluginManager()
        {
            DeviceValue.DeviceValueDataChangedEvent += DeviceValue_DeviceValueDataChangedEvent;
        }

        private async void DeviceValue_DeviceValueDataChangedEvent(object sender, DeviceValue.ValueDataChangedEventArgs e)
        {
            foreach (var plugin in PluginGuidToPluginDictionary.Values.Where(o => o.IsEnabled))
            {
                await plugin.DeviceValueChangedAsync(e.DeviceValueId, e.NewValue, e.OldValue);
            }
        }

        private Core Core { get; set; }

#pragma warning disable 649
        [ImportMany]
        private IEnumerable<zvsPlugin> _plugins;
#pragma warning restore 649

        private readonly Dictionary<Guid, zvsPlugin> _pluginLookup = new Dictionary<Guid, zvsPlugin>();

        public async Task LoadPluginsAsync(Core core)
        {
            Core = core;
            var catalog = new SafeDirectoryCatalog("plugins");
            var compositionContainer = new CompositionContainer(catalog);
            compositionContainer.ComposeParts(this);

            if (catalog.LoadErrors.Count > 0)
            {
                Core.log.WarnFormat(@"The following plug-ins could not be loaded: {0}",
                    string.Join(", " + Environment.NewLine, catalog.LoadErrors));
            }

            using (var context = new zvsContext())
            {
                // Iterate the plug-ins found in dlls
                foreach (var plugin in _plugins)
                {
                    //keeps this plug-in in scope 
                    var zvsPlugin = plugin;

                    if (!_pluginLookup.ContainsKey(zvsPlugin.PluginGuid))
                        _pluginLookup.Add(zvsPlugin.PluginGuid, zvsPlugin);

                    //Check Database for this plug-in
                    var dbPlugin = await context.Plugins
                        .FirstOrDefaultAsync(p => p.PluginGuid == zvsPlugin.PluginGuid);

                    var changed = false;

                    if (dbPlugin == null)
                    {
                        dbPlugin = new Plugin
                        {
                            PluginGuid = zvsPlugin.PluginGuid
                        };
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
                        var result = await context.TrySaveChangesAsync();
                        if (result.HasError)
                            core.log.Error(result.Message);
                    }

                    var msg = string.Format("Initializing '{0}'", zvsPlugin.Name);
                    Core.log.Info(msg);

                    //Plug-in need access to the core in order to use the Logger
                    await zvsPlugin.Initialize(Core);

                    //Reload just installed settings
                    dbPlugin = await context.Plugins
                      .Include(o => o.Settings)
                      .FirstOrDefaultAsync(p => p.PluginGuid == zvsPlugin.PluginGuid);

                    //Set plug-in settings from database values
                    foreach (var setting in dbPlugin.Settings)
                    {
                        SetPluginProperty(zvsPlugin, setting.UniqueIdentifier, setting.Value);
                    }

                    if (dbPlugin.IsEnabled)
                        await zvsPlugin.StartAsync();

                }
            }
        }

        public ReadOnlyDictionary<Guid, zvsPlugin> PluginGuidToPluginDictionary
        {
            get { return new ReadOnlyDictionary<Guid, zvsPlugin>(_pluginLookup); }
        }

        public async void EnablePluginAsync(Guid pluginGuid)
        {
            if (_pluginLookup.ContainsKey(pluginGuid))
            {
                _pluginLookup[pluginGuid].IsEnabled = true;
                await _pluginLookup[pluginGuid].StartAsync();
            }

            //Save Database Value
            using (var context = new zvsContext())
            {
                var a = await context.Plugins.FirstOrDefaultAsync(o => o.PluginGuid == pluginGuid);
                if (a != null)
                    a.IsEnabled = true;

                await context.TrySaveChangesAsync();
            }
        }

        public async void DisablePluginAsync(Guid pluginGuid)
        {
            if (_pluginLookup.ContainsKey(pluginGuid))
            {
                _pluginLookup[pluginGuid].IsEnabled = false;
                await _pluginLookup[pluginGuid].StopAsync();
            }

            //Save Database Value
            using (var context = new zvsContext())
            {
                var a = await context.Plugins.FirstOrDefaultAsync(o => o.PluginGuid == pluginGuid);
                if (a != null)
                    a.IsEnabled = false;

                await context.TrySaveChangesAsync();
            }
        }

        public void NotifyPluginSettingsChanged(PluginSetting pluginSetting)
        {
            if (!_pluginLookup.ContainsKey(pluginSetting.Plugin.PluginGuid))
                return;

            var plugin = _pluginLookup[pluginSetting.Plugin.PluginGuid];
            SetPluginProperty(plugin, pluginSetting.UniqueIdentifier, pluginSetting.Value);
        }

        public zvsPlugin GetPlugin(string uniqueIdentifier)
        {
            return _plugins.FirstOrDefault(p => p.Name == uniqueIdentifier);
        }

        public IEnumerable<zvsPlugin> GetPlugins()
        {
            return _plugins;
        }

        private void SetPluginProperty(object zvsPlugin, string propertyName, object value)
        {
            var prop = zvsPlugin.GetType().GetProperty(propertyName);
            if (prop == null)
            {
                Core.log.ErrorFormat("Cannot find property called {0} on this plug-in", propertyName);
                return;
            }

            try
            {
                var convertedValue = TypeDescriptor.GetConverter(prop.PropertyType).ConvertFrom(value);
                prop.SetValue(zvsPlugin, convertedValue);
            }
            catch
            {
                Core.log.ErrorFormat("Cannot cast value on {0} on this adapter", propertyName);
            }
        }
    }
}