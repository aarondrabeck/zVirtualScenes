using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System;
using System.Data.Entity;
using System.Threading;
using zvs.DataModel;
using System.Threading.Tasks;

namespace zvs.Processor
{
    public class AdapterManager : IAdapterManager
    {
        private IEntityContextConnection EntityContextConnection { get; set; }
        private IFeedback<LogEntry> Log { get; set; }

        private readonly Dictionary<Guid, ZvsAdapter> _adapterLookup = new Dictionary<Guid, ZvsAdapter>();

        public AdapterManager(IEntityContextConnection entityContextConnection, IFeedback<LogEntry> log)
        {
            if (entityContextConnection == null)
                throw new ArgumentNullException("entityContextConnection");

            if (log == null)
                throw new ArgumentNullException("log");

            EntityContextConnection = entityContextConnection;
            Log = log;
        }

        public ZvsAdapter GetZvsAdapterByGuid(Guid adapterGuid)
        {
            return !_adapterLookup.ContainsKey(adapterGuid) ? null : _adapterLookup[adapterGuid];
        }

        public async Task LoadAdaptersAsync(CancellationToken cancellationToken)
        {
            var catalog = new SafeDirectoryCatalog("adapters");
            var compositionContainer = new CompositionContainer(catalog);
            compositionContainer.ComposeParts(this);

            if (catalog.LoadErrors.Count > 0)
            {
                await Log.ReportWarningFormatAsync(cancellationToken, @"The following plug-ins could not be loaded: {0}",
                    string.Join(", " + Environment.NewLine, catalog.LoadErrors));
            }

            using (var context = new ZvsContext(EntityContextConnection))
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
                        .FirstOrDefaultAsync(p => p.AdapterGuid == zvsAdapter.AdapterGuid, cancellationToken);

                    var changed = false;
                    if (dbAdapter == null)
                    {
                        dbAdapter = new Adapter { AdapterGuid = zvsAdapter.AdapterGuid };
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
                        var result = await context.TrySaveChangesAsync(cancellationToken);
                        if (result.HasError)
                            await Log.ReportErrorAsync(result.Message, cancellationToken);
                    }

                    var msg = string.Format("Initializing '{0}'", zvsAdapter.Name);
                    await Log.ReportInfoAsync(msg, cancellationToken);

                    //Plug-in need access to the zvsEngine in order to use the Logger
                     await zvsAdapter.Initialize(Log,EntityContextConnection);

                    //Reload just installed settings
                    dbAdapter = await context.Adapters
                        .Include(o => o.Settings)
                        .FirstOrDefaultAsync(p => p.AdapterGuid == zvsAdapter.AdapterGuid, cancellationToken);

                    //Set plug-in settings from database values
                    foreach (var setting in dbAdapter.Settings)
                    {
                        var prop = zvsAdapter.GetType().GetProperty(setting.UniqueIdentifier);
                        if (prop == null)
                        {
                            await
                                Log.ReportErrorFormatAsync(cancellationToken,
                                    "Cannot find property called {0} on this adapter", setting.UniqueIdentifier);
                            return;
                        }

                        try
                        {
                            var convertedValue =
                                TypeDescriptor.GetConverter(prop.PropertyType).ConvertFrom(setting.Value);
                            prop.SetValue(zvsAdapter, convertedValue);
                        }
                        catch
                        {
                            Log.ReportErrorFormatAsync(cancellationToken, "Cannot cast value on {0} on this adapter",
                                setting.UniqueIdentifier).Wait(cancellationToken);
                        }

                    }

                    if (dbAdapter.IsEnabled)
                        await zvsAdapter.StartAsync();
                }
            }

        }

        public async Task EnableAdapterAsync(Guid adapterGuid, CancellationToken cancellationToken)
        {

            var adapter = GetZvsAdapterByGuid(adapterGuid);
            if (adapter == null)
                return;

            adapter.IsEnabled = true;
            await adapter.StartAsync();

            using (var context = new ZvsContext(EntityContextConnection))
            {
                //Save Database Value
                var a = await context.Adapters.FirstOrDefaultAsync(o => o.AdapterGuid == adapterGuid, cancellationToken);
                if (a != null)
                    a.IsEnabled = true;

                await context.TrySaveChangesAsync(cancellationToken);
            }
        }

        public async Task DisableAdapterAsync(Guid adapterGuid, CancellationToken cancellationToken)
        {
            var adapter = GetZvsAdapterByGuid(adapterGuid);
            if (adapter == null)
                return;

            adapter.IsEnabled = false;
            await adapter.StopAsync();

            using (var context = new ZvsContext(EntityContextConnection))
            {
                //Save Database Value
                var a = await context.Adapters.FirstOrDefaultAsync(o => o.AdapterGuid == adapterGuid, cancellationToken);
                if (a != null)
                    a.IsEnabled = false;

                await context.TrySaveChangesAsync(cancellationToken);
            }
        }


#pragma warning disable 649
        [ImportMany]
        private IEnumerable<ZvsAdapter> _adapters;
#pragma warning restore 649

    }
}

