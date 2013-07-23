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
                        .Include(o => o.Settings)
                        .FirstOrDefaultAsync(p => p.PluginGuid == zvsPlugin.PluginGuid);

                    if (dbPlugin == null)
                    {
                        dbPlugin = new Plugin();
                        dbPlugin.PluginGuid = zvsPlugin.PluginGuid;
                        context.Plugins.Add(dbPlugin);
                    }

                    //Update Name and Description
                    zvsPlugin.IsEnabled = dbPlugin.IsEnabled;
                    dbPlugin.Name = zvsPlugin.Name;
                    dbPlugin.Description = zvsPlugin.Description;

                    //Set plug-in settings from database values
                    foreach (var setting in dbPlugin.Settings)
                    {
                        SetPluginProperty(zvsPlugin, setting.UniqueIdentifier, setting.Value);
                    }

                    var result = await context.TrySaveChangesAsync();
                    if (result.HasError)
                        core.log.Error(result.Message);

                    string msg = string.Format("Initializing '{0}'", zvsPlugin.Name);
                    Core.log.Info(msg);

                    //Plug-in need access to the core in order to use the Logger
                    await zvsPlugin.Initialize(Core);

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



































//using System.Collections.Generic;
//using System.ComponentModel.Composition;
//using System.ComponentModel.Composition.Hosting;
//using System.Data;
//using System.Linq;
//using System.Threading;
//using System;
//using System.ComponentModel;

//using System.Windows.Threading;
//using zvs.Entities;

//namespace zvs.Processor
//{
//    public class PluginManager
//    {
//        private const int verbose = 10;
//        private const string _Name = "Plug-in Manager";
//        private Core Core;

//        public delegate void onPluginInitializedEventHandler(object sender, onPluginInitializedEventArgs args);
//        public class onPluginInitializedEventArgs : EventArgs
//        {
//            public string Details = string.Empty;

//            public onPluginInitializedEventArgs(string Details)
//            {
//                this.Details = Details;
//            }
//        }
//        public static event onPluginInitializedEventHandler onPluginInitialized;

//        [ImportMany]
//        #pragma warning disable 649
//        private IEnumerable<zvsPlugin> _plugins;
//        #pragma warning restore 649

//        public PluginManager(Core Core)
//        {
//            this.Core = Core;
//            DirectoryCatalog catalog = new DirectoryCatalog("plugins");
//            CompositionContainer compositionContainer = new CompositionContainer(catalog);
//            compositionContainer.ComposeParts(this);

//            using (zvsContext context = new zvsContext())
//            {
//                // Iterate the plug-in
//                foreach (zvsPlugin p in _plugins)
//                {
//                    //keeps this plug-in in scope 
//                    var p2 = p;

//                    //Plug-in need access to the core in order to use the Logger
//                    p2.Core = this.Core;

//                    //initialize each plug-in async.
//                    BackgroundWorker pluginInitializer = new BackgroundWorker();
//                    pluginInitializer.DoWork += (object sender, DoWorkEventArgs e) =>
//                    {
//                        string msg = string.Format("Initializing '{0}'", p2.Name);
//                        Core.log.Info(msg);
//                        if (onPluginInitialized != null)
//                            onPluginInitialized(this, new onPluginInitializedEventArgs(msg));

//                        p2.Initialize();
//                        p2.Start();
//                    };
//                    pluginInitializer.RunWorkerAsync();
//                }
//            }

//        }

//        public IEnumerable<zvsPlugin> GetPlugins()
//        {
//            return _plugins;
//        }

//        public zvsPlugin GetPlugin(string uniqueIdentifier)
//        {
//            return _plugins.FirstOrDefault(p => p.UniqueIdentifier == uniqueIdentifier);
//        }

//        public void NotifyPluginSettingsChanged(AdapterSetting ps)
//        {
//            zvsPlugin p = GetPlugin(ps.Plugin.UniqueIdentifier);
//            if (p != null)
//                p.SettingsChange(ps);
//        }
//    }
//}

