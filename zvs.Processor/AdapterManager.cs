using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System;
using System.ComponentModel;
using System.Data.Entity;
using zvs.Entities;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace zvs.Processor
{
    public class AdapterManager
    {
        private Core Core { get; set; }

#pragma warning disable 649
        [ImportMany]
        private IEnumerable<zvsAdapter> _adapters;
#pragma warning restore 649

        private readonly Dictionary<Guid, zvsAdapter> _adapterLookup = new Dictionary<Guid, zvsAdapter>();

        public void LoadAdaptersAsync(Core core)
        {
            Task.Run(async () =>
                {
                    Core = core;
                    var catalog = new SafeDirectoryCatalog("adapters");
                    var compositionContainer = new CompositionContainer(catalog);
                    compositionContainer.ComposeParts(this);

                    if (catalog.LoadErrors.Count > 0)
                    {
                        Core.log.WarnFormat(@"The following plug-ins could not be loaded: {0}",
                            string.Join(", " + Environment.NewLine, catalog.LoadErrors));
                    }

                    using (var context = new zvsContext())
                    {
                        // Iterate the adapters found in dlls
                        foreach (var adapter in _adapters)
                        {
                            //keeps this adapter in scope 
                            var zvsAdapter = adapter;

                            if (!_adapterLookup.ContainsKey(zvsAdapter.AdapterGuid))
                                _adapterLookup.Add(zvsAdapter.AdapterGuid, zvsAdapter);

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

                            var msg = string.Format("Initializing '{0}'", zvsAdapter.Name);
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
            get { return new ReadOnlyDictionary<Guid, zvsAdapter>(_adapterLookup); }
        }

        public async void EnableAdapterAsync(Guid adapterGuid)
        {
            if (_adapterLookup.ContainsKey(adapterGuid))
            {
                _adapterLookup[adapterGuid].IsEnabled = true;
                await _adapterLookup[adapterGuid].StartAsync();
            }

            //Save Database Value
            using (var context = new zvsContext())
            {
                var a = await context.Adapters.FirstOrDefaultAsync(o => o.AdapterGuid == adapterGuid);
                if (a != null)
                    a.IsEnabled = true;

                await context.TrySaveChangesAsync();
            }
        }

        public async void DisableAdapterAsync(Guid adapterGuid)
        {
            if (_adapterLookup.ContainsKey(adapterGuid))
            {
                _adapterLookup[adapterGuid].IsEnabled = false;
                await _adapterLookup[adapterGuid].StopAsync();
            }

            //Save Database Value
            using (var context = new zvsContext())
            {
                var a = await context.Adapters.FirstOrDefaultAsync(o => o.AdapterGuid == adapterGuid);
                if (a != null)
                    a.IsEnabled = false;

                await context.TrySaveChangesAsync();
            }
        }

        public void NotifyAdapterSettingsChanged(AdapterSetting adapterSetting)
        {
            if (!_adapterLookup.ContainsKey(adapterSetting.Adapter.AdapterGuid))
                return;

            var adapter = _adapterLookup[adapterSetting.Adapter.AdapterGuid];
            SetAdapterProperty(adapter, adapterSetting.UniqueIdentifier, adapterSetting.Value);
        }

        private void SetAdapterProperty(object zvsAdapter, string propertyName, object value)
        {
            var prop = zvsAdapter.GetType().GetProperty(propertyName);
            if (prop == null)
            {
                Core.log.ErrorFormat("Cannot find property called {0} on this adapter", propertyName);
                return;
            }

            try
            {
                var convertedValue = TypeDescriptor.GetConverter(prop.PropertyType).ConvertFrom(value);
                prop.SetValue(zvsAdapter, convertedValue);
            }
            catch
            {
                Core.log.ErrorFormat("Cannot cast value on {0} on this adapter", propertyName);
            }
        }
    }
}

