using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zvs.Entities;
using System.Data.Entity;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace zvs.Processor
{
    public abstract class zvsAdapter : INotifyPropertyChanged
    {
        public bool IsEnabled { get; set; }
        public abstract Guid AdapterGuid { get; }
        public abstract string Name { get; }
        public abstract string Description { get; }
        protected Core Core { get; private set; }
        protected DeviceValueBuilder DeviceValueBuilder { get; private set; }
        protected DeviceCommandBuilder DeviceCommandBuilder { get; private set; }

        Logging.ILog log = Logging.LogManager.GetLogger<zvsAdapter>();

        public abstract Task StartAsync();
        public abstract Task StopAsync();

        public async Task Initialize(Core core)
        {
            Core = core;
            DeviceValueBuilder = new DeviceValueBuilder(this, core);
            DeviceCommandBuilder = new DeviceCommandBuilder(this, core);

            using (zvsContext context = new zvsContext())
            {
                var dtb = new DeviceTypeBuilder(this, core, context);
                await OnDeviceTypesCreating(dtb);

                var sb = new AdapterSettingBuilder(core, context);
                await OnSettingsCreating(sb);
            }
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
        public abstract Task RepollAsync(Device device, zvsContext context);
        public abstract Task ActivateGroupAsync(Group group, zvsContext context);
        public abstract Task DeactivateGroupAsync(Group group, zvsContext context);

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
