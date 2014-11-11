using System.Collections.Generic;
using System.ComponentModel;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using zvs.DataModel;
using System.Threading.Tasks;

namespace zvs.Processor
{
    public class AdapterManager : IAdapterManager
    {
        private IEnumerable<ZvsAdapter> Adapters { get; set; } 
        private IEntityContextConnection EntityContextConnection { get; set; }
        private IFeedback<LogEntry> Log { get; set; }

        private readonly Dictionary<Guid, ZvsAdapter> _adapterLookup = new Dictionary<Guid, ZvsAdapter>();

        public AdapterManager(IEnumerable<ZvsAdapter> adapters, IEntityContextConnection entityContextConnection, IFeedback<LogEntry> log)
        {
            if (adapters == null)
                throw new ArgumentNullException("adapters");

            if (entityContextConnection == null)
                throw new ArgumentNullException("entityContextConnection");

            if (log == null)
                throw new ArgumentNullException("log");

            EntityContextConnection = entityContextConnection;
            Log = log;
            Adapters = adapters;

            Log.Source = "Adapter Manager";

            _adapterLookup = Adapters.ToDictionary(o => o.AdapterGuid, o => o);
        }

        public ZvsAdapter FindZvsAdapter(Guid adapterGuid)
        {
            return !_adapterLookup.ContainsKey(adapterGuid) ? null : _adapterLookup[adapterGuid];
        }

        public IReadOnlyList<ZvsAdapter> GetZvsAdapters()
        {
            return Adapters.ToList();
        }

        public async Task InitializeAdaptersAsync(CancellationToken cancellationToken)
        {
            using (var context = new ZvsContext(EntityContextConnection))
            {
                // Iterate the adapters found in dlls
                foreach (var adapter in Adapters)
                {
                    //keeps this adapter in scope 
                    var zvsAdapter = adapter;

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
                        {
                            await
                                Log.ReportErrorFormatAsync(cancellationToken,
                                    "Adapter not loaded. Error while saving loaded '{0}' adapter to database. {1}", zvsAdapter.Name, result.Message);
                            break;
                        }
                    }

                    await Log.ReportInfoFormatAsync(cancellationToken, "Initializing '{0}'", zvsAdapter.Name);

                    //Plug-in need access to the zvsEngine in order to use the Logger
                    await zvsAdapter.Initialize(Log, EntityContextConnection);

                    //Reload just installed settings
                    var adapterSettings = await context.AdapterSettings
                        .Include(o => o.Adapter)
                        .Where(p => p.Adapter.AdapterGuid == zvsAdapter.AdapterGuid)
                        .ToListAsync(cancellationToken);

                    //Set plug-in settings from database values
                    foreach (var setting in adapterSettings)
                    {
                        var prop = zvsAdapter.GetType().GetProperty(setting.UniqueIdentifier);
                        if (prop == null)
                        {
                            await
                                Log.ReportErrorFormatAsync(cancellationToken,
                                    "Cannot find property called {0} on this adapter", setting.UniqueIdentifier);
                            continue;
                        }

                        try
                        {
                            var convertedValue =
                                TypeDescriptor.GetConverter(prop.PropertyType).ConvertFrom(setting.Value);
                            prop.SetValue(zvsAdapter, convertedValue);
                        }
                        catch
                        {
                            Log.ReportErrorFormatAsync(cancellationToken, "Cannot cast value on adapter setting {0}", setting.UniqueIdentifier).Wait(cancellationToken);
                        }

                    }

                    if (dbAdapter.IsEnabled)
                        await zvsAdapter.StartAsync();
                }
            }

        }

        public async Task<Result> EnableAdapterAsync(Guid adapterGuid, CancellationToken cancellationToken)
        {
            var adapter = FindZvsAdapter(adapterGuid);
            if (adapter == null)
                return Result.ReportErrorFormat("Unable to enable adapter with Guid of {0}", adapterGuid);

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
            return Result.ReportSuccess();
        }

        public async Task<Result> DisableAdapterAsync(Guid adapterGuid, CancellationToken cancellationToken)
        {
            var adapter = FindZvsAdapter(adapterGuid);
            if (adapter == null)
                return Result.ReportErrorFormat("Unable to disable adapter with Guid of {0}", adapterGuid);

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
            return Result.ReportSuccess();
        }



    }
}

