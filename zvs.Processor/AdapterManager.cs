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
        private bool IsRunning { get; set; }

        private readonly Dictionary<Guid, ZvsAdapter> _adapterLookup = new Dictionary<Guid, ZvsAdapter>();
        private readonly Dictionary<int, Guid> _adapterIdToGuid = new Dictionary<int, Guid>();

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

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (IsRunning)
            {
                await
                    Log.ReportWarningAsync("Cannot start adapter manager because it is already running!",
                        cancellationToken);
                return;
            }
            IsRunning = true;

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
                        await SetAdapterProperty(adapter, setting.UniqueIdentifier, setting.Value, CancellationToken.None);

                    if (dbAdapter.IsEnabled)
                        await zvsAdapter.StartAsync();

                    if (!_adapterIdToGuid.ContainsKey(dbAdapter.Id))
                        _adapterIdToGuid.Add(dbAdapter.Id, dbAdapter.AdapterGuid);
                }
            }
            NotifyEntityChangeContext.ChangeNotifications<AdapterSetting>.OnEntityUpdated += ChangeNotificationsOnOnEntityUpdated;
        }


        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (!IsRunning)
            {
                await
                    Log.ReportWarningAsync("Cannot stop adapter manager because it is not running!",
                        cancellationToken);
                return;
            }

            IsRunning = false;

            foreach (var adapter in Adapters)
            {
                await adapter.StopAsync();
            }

            NotifyEntityChangeContext.ChangeNotifications<AdapterSetting>.OnEntityUpdated -= ChangeNotificationsOnOnEntityUpdated;
        }
        private async void ChangeNotificationsOnOnEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<AdapterSetting>.EntityUpdatedArgs entityUpdatedArgs)
        {
            var adapter = FindZvsAdapter(entityUpdatedArgs.NewEntity.AdapterId);
            if (adapter == null)
                return;

            await SetAdapterProperty(adapter, entityUpdatedArgs.NewEntity.UniqueIdentifier, entityUpdatedArgs.NewEntity.Value, CancellationToken.None);
        }

        public ZvsAdapter FindZvsAdapter(int adapterId)
        {
            if (!_adapterIdToGuid.ContainsKey(adapterId))
                return null;

            var guid = _adapterIdToGuid[adapterId];
            return !_adapterLookup.ContainsKey(guid) ? null : _adapterLookup[guid];
        }

        internal async Task SetAdapterProperty(ZvsAdapter zvsAdapter, string propertyName, object value, CancellationToken cancellationToken)
        {
            var prop = zvsAdapter.GetType().GetProperty(propertyName);
            if (prop == null)
            {
                await Log.ReportErrorFormatAsync(cancellationToken, "Cannot find property called {0} on this adapter", propertyName);
                return;
            }

            try
            {
                var convertedValue = TypeDescriptor.GetConverter(prop.PropertyType).ConvertFrom(value);
                prop.SetValue(zvsAdapter, convertedValue);
            }
            catch
            {
                Log.ReportErrorFormatAsync(cancellationToken, "Cannot cast value on {0} on this adapter", propertyName).Wait(cancellationToken);
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

