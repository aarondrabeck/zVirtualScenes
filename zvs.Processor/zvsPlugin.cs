using System;
using System.Threading;
using System.Threading.Tasks;
using zvs.DataModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace zvs.Processor
{
    public abstract class ZvsPlugin : INotifyPropertyChanged
    {
        public bool IsEnabled { get; set; }
        public abstract Guid PluginGuid { get; }
        public abstract string Name { get; }
        public abstract string Description { get; }
        protected IFeedback<LogEntry> Log { get; private set; }
        protected IEntityContextConnection EntityContextConnection { get; set; }
        protected CancellationToken CancellationToken { get; set; }

        public abstract Task StartAsync();
        public abstract Task StopAsync();

        public async Task Initialize(IFeedback<LogEntry> log, IEntityContextConnection entityContextConnection)
        {
            EntityContextConnection = entityContextConnection;
            Log = log;

            var sb = new PluginSettingBuilder(entityContextConnection, CancellationToken);
            await OnSettingsCreating(sb);

            var ssb = new SceneSettingBuilder(EntityContextConnection);
            await OnSceneSettingsCreating(ssb);

            var dsb = new DeviceSettingBuilder(EntityContextConnection);
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

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}