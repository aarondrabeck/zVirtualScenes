using System;
using System.Threading;
using System.Threading.Tasks;
using zvs.DataModel;

namespace zvs.Processor
{
    public class ChangeListener
    {
        private IEntityContextConnection EntityContextConnection { get; set; }
        private IFeedback<LogEntry> Log { get; set; }
        private CancellationToken Ct { get; set; }
        bool IsRunning { get; set; }
        public ChangeListener(IFeedback<LogEntry> log, IEntityContextConnection entityContextConnection)
        {
            if (log == null)
                throw new ArgumentNullException("log");


            if (entityContextConnection == null)
                throw new ArgumentNullException("entityContextConnection");

            Log = log;
            EntityContextConnection = entityContextConnection;
        }

        public async Task StartAsync(CancellationToken ct)
        {
            if (IsRunning)
            {
                await Log.ReportWarningAsync("Change listener is already started", ct);
                return;
            }

            Ct = ct;
            NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.OnEntityUpdated += ChangeNotificationsOnOnEntityUpdated;
            IsRunning = true;

            await Log.ReportInfoAsync("Change listener started, now listening for device values changes", ct);
        }

        public async Task StopAsync(CancellationToken ct)
        {
            if (!IsRunning)
            {
                await Log.ReportWarningAsync("Change listener is not started", ct);
                return;
            }

            NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.OnEntityUpdated -= ChangeNotificationsOnOnEntityUpdated;
            IsRunning = false;

            await Log.ReportInfoAsync("Change listener stopped, no longer listening for device values changes", ct);
        }

        private async void ChangeNotificationsOnOnEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.EntityUpdatedArgs entityUpdatedArgs)
        {
            //TODO: listen to more changes and get device name
            if (entityUpdatedArgs.NewEntity.Value != entityUpdatedArgs.OldEntity.Value)
                await Log.ReportInfoFormatAsync(Ct, "Value changed from {0} to {1} on device id {2}",
                        entityUpdatedArgs.OldEntity.Value,
                        entityUpdatedArgs.NewEntity.Value, 
                        entityUpdatedArgs.NewEntity.DeviceId);
        }
    }
}
