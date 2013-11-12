using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Data;
using System.Linq;
using System.Threading;
using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Windows.Threading;
using zvs.Entities;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace zvs.Processor
{
    public class PluginManager
    {
        private const int Verbose = 10;
        private const string Name = "Plug-in Manager";

        public PluginManager()
        {
            DeviceValue.DeviceValueDataChangedEvent += DeviceValue_DeviceValueDataChangedEvent;
        }

        private async void DeviceValue_DeviceValueDataChangedEvent(object sender, DeviceValue.ValueDataChangedEventArgs e)
        {
            foreach (var plugin in PluginGuidToPluginDictionary.Values.Where(o => o.IsEnabled))
            {
                await plugin.DeviceValueChangedAsync(e.DeviceValueId, e.newValue, e.oldValue);
            }
        }

        public Core Core { get; private set; }

#pragma warning disable 649
        [ImportMany]
        private IEnumerable<zvsPlugin> _Plugins;
#pragma warning restore 649

        private Dictionary<Guid, zvsPlugin> PluginLookup = new Dictionary<Guid, zvsPlugin>();

        public async Task LoadPluginsAsync(Core core)
        {
            Core = core;
            SafeDirectoryCatalog catalog = new SafeDirectoryCatalog("plugins");
            CompositionContainer compositionContainer = new CompositionContainer(catalog);
            compositionContainer.ComposeParts(this);

            if (catalog.LoadExceptionTypeNames.Count > 0)
            {
                Core.log.WarnFormat(@"The following plug-ins could not be loaded because they are incompatible: {0}. To resolve this issue, update or uninstall the listed plug-in's.",
                    string.Join(", ", catalog.LoadExceptionTypeNames));
            }

            using (zvsContext context = new zvsContext())
            {
                // Iterate the plug-ins found in dlls
                foreach (var plugin in _Plugins)
                {
                    //keeps this plug-in in scope 
                    var zvsPlugin = plugin;

                    if (!PluginLookup.ContainsKey(zvsPlugin.PluginGuid))
                        PluginLookup.Add(zvsPlugin.PluginGuid, zvsPlugin);

                    //Check Database for this plug-in
                    var dbPlugin = await context.Plugins
                        .FirstOrDefaultAsync(p => p.PluginGuid == zvsPlugin.PluginGuid);

                    var changed = false;

                    if (dbPlugin == null)
                    {
                        dbPlugin = new Plugin();
                        dbPlugin.PluginGuid = zvsPlugin.PluginGuid;
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

                    string msg = string.Format("Initializing '{0}'", zvsPlugin.Name);
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
            get { return new ReadOnlyDictionary<Guid, zvsPlugin>(PluginLookup); }
        }

        public async void EnablePluginAsync(Guid pluginGuid)
        {
            if (PluginLookup.ContainsKey(pluginGuid))
            {
                PluginLookup[pluginGuid].IsEnabled = true;
                await PluginLookup[pluginGuid].StartAsync();
            }

            //Save Database Value
            using (zvsContext context = new zvsContext())
            {
                var a = await context.Plugins.FirstOrDefaultAsync(o => o.PluginGuid == pluginGuid);
                if (a != null)
                    a.IsEnabled = true;

                await context.TrySaveChangesAsync();
            }
        }

        public async void DisablePluginAsync(Guid pluginGuid)
        {
            if (PluginLookup.ContainsKey(pluginGuid))
            {
                PluginLookup[pluginGuid].IsEnabled = false;
                await PluginLookup[pluginGuid].StopAsync();
            }

            //Save Database Value
            using (zvsContext context = new zvsContext())
            {
                var a = await context.Plugins.FirstOrDefaultAsync(o => o.PluginGuid == pluginGuid);
                if (a != null)
                    a.IsEnabled = false;

                await context.TrySaveChangesAsync();
            }
        }

        public void NotifyPluginSettingsChanged(PluginSetting pluginSetting)
        {
            if (!PluginLookup.ContainsKey(pluginSetting.Plugin.PluginGuid))
                return;

            var plugin = PluginLookup[pluginSetting.Plugin.PluginGuid];
            SetPluginProperty(plugin, pluginSetting.UniqueIdentifier, pluginSetting.Value);
        }

        public zvsPlugin GetPlugin(string uniqueIdentifier)
        {
            return _Plugins.FirstOrDefault(p => p.Name == uniqueIdentifier);
        }

        public IEnumerable<zvsPlugin> GetPlugins()
        {
            return _Plugins;
        }

        private void SetPluginProperty(object zvsPlugin, string PropertyName, object value)
        {
            var prop = zvsPlugin.GetType().GetProperty(PropertyName);
            if (prop == null)
            {
                Core.log.ErrorFormat("Cannot find property called {0} on this plug-in", PropertyName);
                return;
            }

            try
            {
                var convertedValue = TypeDescriptor.GetConverter(prop.PropertyType).ConvertFrom(value);
                prop.SetValue(zvsPlugin, convertedValue);
            }
            catch
            {
                Core.log.ErrorFormat("Cannot cast value on {0} on this adapter", PropertyName);
            }
        }
    }
}