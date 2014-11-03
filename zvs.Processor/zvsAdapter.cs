using System;
using System.Threading;
using System.Threading.Tasks;
using zvs.DataModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace zvs.Processor
{
    public abstract class ZvsAdapter : INotifyPropertyChanged
    {
        public bool IsEnabled { get; set; }
        public abstract Guid AdapterGuid { get; }
        public abstract string Name { get; }
        public abstract string Description { get; }
        protected IFeedback<LogEntry> Log { get; private set; }
        protected ZvsContext Context { get; set; }
        protected DeviceValueBuilder DeviceValueBuilder { get; private set; }
        protected DeviceCommandBuilder DeviceCommandBuilder { get; private set; }

        public abstract Task StartAsync(CancellationToken cancellationToken);
        public abstract Task StopAsync(CancellationToken cancellationToken);

        public async Task Initialize(IFeedback<LogEntry> log, ZvsContext zvsContext)
        {
            Context = zvsContext;
            Log = log;

            DeviceValueBuilder = new DeviceValueBuilder(log, Context);
            DeviceCommandBuilder = new DeviceCommandBuilder(log, Context);

            var dtb = new DeviceTypeBuilder(log, Context);
            await OnDeviceTypesCreating(dtb);

            var sb = new AdapterSettingBuilder(log, Context);
            await OnSettingsCreating(sb);
        }

        public virtual Task OnSettingsCreating(AdapterSettingBuilder settingBuilder)
        {
            return Task.FromResult(0);
        }

        public virtual Task OnDeviceTypesCreating(DeviceTypeBuilder deviceTypeBuilder)
        {
            return Task.FromResult(0);
        }

        public abstract Task ProcessDeviceTypeCommandAsync(DeviceType deviceType, Device device, DeviceTypeCommand command, string argument);
        public abstract Task ProcessDeviceCommandAsync(Device device, DeviceCommand command, string argument, string argument2);
        public abstract Task RepollAsync(Device device);
        public abstract Task ActivateGroupAsync(Group group);
        public abstract Task DeactivateGroupAsync(Group group);

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
