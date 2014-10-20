using System;
using System.Threading;
using System.Threading.Tasks;
using zvs.DataModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace zvs.Processor
{
    public abstract class zvsPlugin : INotifyPropertyChanged
    {
        public bool IsEnabled { get; set; }
        public abstract Guid PluginGuid { get; }
        public abstract string Name { get; }
        public abstract string Description { get; }

        public abstract Task StartAsync(CancellationToken cancellationToken);
        public abstract Task StopAsync(CancellationToken cancellationToken);

        public async Task Initialize(IFeedback<LogEntry> log, ZvsContext zvsContext)
        {
            var sb = new PluginSettingBuilder(log, zvsContext);
            await OnSettingsCreating(sb);

            var ssb = new SceneSettingBuilder(log, zvsContext);
            await OnSceneSettingsCreating(ssb);

            var dsb = new DeviceSettingBuilder(log, zvsContext);
            await OnDeviceSettingsCreating(dsb);
        }

        public virtual Task OnDeviceSettingsCreating(DeviceSettingBuilder settingBuilder)
        {
            return Task.FromResult(0);
        }

        public virtual Task OnSceneSettingsCreating(SceneSettingBuilder settingBuilder)
        {
            return Task.FromResult(0);
        }

        public virtual Task OnSettingsCreating(PluginSettingBuilder settingBuilder)
        {
            return Task.FromResult(0);
        }

        public abstract Task DeviceValueChangedAsync(Int64 deviceValueId, string newValue, string oldValue);

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}