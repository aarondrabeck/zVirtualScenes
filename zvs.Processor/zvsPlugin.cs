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
    public abstract class zvsPlugin : INotifyPropertyChanged
    {
        public bool IsEnabled { get; set; }
        public abstract Guid PluginGuid { get; }
        public abstract string Name { get; }
        public abstract string Description { get; }
        protected Core Core { get; private set; }

        Logging.ILog log = Logging.LogManager.GetLogger<zvsAdapter>();

        public abstract Task StartAsync();
        public abstract Task StopAsync();

        public async Task Initialize(Core core)
        {
            Core = core;

            using (zvsContext context = new zvsContext())
            {
                var sb = new PluginSettingBuilder(core, context);
                await OnSettingsCreating(sb);
            }        
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