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
    public class AdapterManager
    {
        private const int Verbose = 10;
        private const string Name = "Adapter Manager";

        public Core Core { get; private set; }

#pragma warning disable 649
        [ImportMany]
        private IEnumerable<zvsAdapter> _Adapters;
#pragma warning restore 649

        private Dictionary<Guid, zvsAdapter> AdapterLookup = new Dictionary<Guid, zvsAdapter>();

        public void LoadAdaptersAsync(Core core)
        {
            Task.Run(async () =>
                {
                    Core = core;
                    SafeDirectoryCatalog catalog = new SafeDirectoryCatalog("adapters");
                    CompositionContainer compositionContainer = new CompositionContainer(catalog);
                    compositionContainer.ComposeParts(this);

                    if (catalog.LoadExceptionTypeNames.Count > 0)
                    {
                        Core.log.WarnFormat(@"The following adapters could not be loaded because they are incompatible: {0}. To resolve this issue, update or uninstall the listed adapter's.",
                            string.Join(", ", catalog.LoadExceptionTypeNames));
                    }

                    using (zvsContext context = new zvsContext())
                    {
                        // Iterate the adapters found in dlls
                        foreach (var adapter in _Adapters)
                        {
                            //keeps this adapter in scope 
                            var zvsAdapter = adapter;

                            if (!AdapterLookup.ContainsKey(zvsAdapter.AdapterGuid))
                                AdapterLookup.Add(zvsAdapter.AdapterGuid, zvsAdapter);

                            //Check Database for this adapter
                            var dbAdapter = await context.Adapters
                                .FirstOrDefaultAsync(p => p.AdapterGuid == zvsAdapter.AdapterGuid);

                            var changed = false;
                            if (dbAdapter == null)
                            {
                                dbAdapter = new Adapter();
                                dbAdapter.AdapterGuid = zvsAdapter.AdapterGuid;
                                context.Adapters.Add(dbAdapter);
                                changed = true;
                            }

                            //Update Name and Description
                            zvsAdapter.IsEnabled = dbAdapter.IsEnabled;

                            if (dbAdapter.Name != zvsAdapter.Name)
                            {
                                dbAdapter.Name = zvsAdapter.Name;
                                changed = true;
                            }

                            if (dbAdapter.Description != zvsAdapter.Description)
                            {
                                dbAdapter.Description = zvsAdapter.Description;
                                changed = true;
                            }

                            if (changed)
                            {
                                var result = await context.TrySaveChangesAsync();
                                if (result.HasError)
                                    core.log.Error(result.Message);
                            }

                            string msg = string.Format("Initializing '{0}'", zvsAdapter.Name);
                            Core.log.Info(msg);

                            //Plug-in need access to the core in order to use the Logger
                            await zvsAdapter.Initialize(Core);

                            //Reload just installed settings
                            dbAdapter = await context.Adapters
                                .Include(o => o.Settings)
                                .FirstOrDefaultAsync(p => p.AdapterGuid == zvsAdapter.AdapterGuid);

                            //Set plug-in settings from database values
                            foreach (var setting in dbAdapter.Settings)
                            {
                                SetAdapterProperty(zvsAdapter, setting.UniqueIdentifier, setting.Value);
                            }

                            if (dbAdapter.IsEnabled)
                                await zvsAdapter.StartAsync();
                        }
                    }
                });
        }

        public ReadOnlyDictionary<Guid, zvsAdapter> AdapterGuidToAdapterDictionary
        {
            get { return new ReadOnlyDictionary<Guid, zvsAdapter>(AdapterLookup); }
        }

        public async void EnableAdapterAsync(Guid adapterGuid)
        {
            if (AdapterLookup.ContainsKey(adapterGuid))
            {
                AdapterLookup[adapterGuid].IsEnabled = true;
                await AdapterLookup[adapterGuid].StartAsync();
            }

            //Save Database Value
            using (zvsContext context = new zvsContext())
            {
                var a = await context.Adapters.FirstOrDefaultAsync(o => o.AdapterGuid == adapterGuid);
                if (a != null)
                    a.IsEnabled = true;

                await context.TrySaveChangesAsync();
            }
        }

        public async void DisableAdapterAsync(Guid adapterGuid)
        {
            if (AdapterLookup.ContainsKey(adapterGuid))
            {
                AdapterLookup[adapterGuid].IsEnabled = false;
                await AdapterLookup[adapterGuid].StopAsync();
            }

            //Save Database Value
            using (zvsContext context = new zvsContext())
            {
                var a = await context.Adapters.FirstOrDefaultAsync(o => o.AdapterGuid == adapterGuid);
                if (a != null)
                    a.IsEnabled = false;

                await context.TrySaveChangesAsync();
            }
        }

        public void NotifyAdapterSettingsChanged(AdapterSetting adapterSetting)
        {
            if (!AdapterLookup.ContainsKey(adapterSetting.Adapter.AdapterGuid))
                return;

            var adapter = AdapterLookup[adapterSetting.Adapter.AdapterGuid];
            SetAdapterProperty(adapter, adapterSetting.UniqueIdentifier, adapterSetting.Value);
        }

        private void SetAdapterProperty(object zvsAdapter, string PropertyName, object value)
        {
            var prop = zvsAdapter.GetType().GetProperty(PropertyName);
            if (prop == null)
            {
                Core.log.ErrorFormat("Cannot find property called {0} on this adapter", PropertyName);
                return;
            }

            try
            {
                var convertedValue = TypeDescriptor.GetConverter(prop.PropertyType).ConvertFrom(value);
                prop.SetValue(zvsAdapter, convertedValue);
            }
            catch
            {
                Core.log.ErrorFormat("Cannot cast value on {0} on this adapter", PropertyName);
            }
        }
    }
}

