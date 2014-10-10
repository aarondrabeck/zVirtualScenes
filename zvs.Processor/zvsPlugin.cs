using System;
using System.Threading.Tasks;
using zvs.Entities;
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
        public Core Core { get; private set; }

        Logging.ILog log = Logging.LogManager.GetLogger<zvsAdapter>();

        public abstract Task StartAsync();
        public abstract Task StopAsync();

        public async Task Initialize(Core core)
        {
            Core = core;

            using (var context = new zvsContext())
            {
                var sb = new PluginSettingBuilder(core, context);
                await OnSettingsCreating(sb);

                var ssb = new SceneSettingBuilder(core, context);
                await OnSceneSettingsCreating(ssb);

                var dsb = new DeviceSettingBuilder(core, context);
                await OnDeviceSettingsCreating(dsb);
            }        
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