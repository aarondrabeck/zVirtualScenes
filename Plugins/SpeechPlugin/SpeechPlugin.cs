using System;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using zvs.DataModel;
using zvs.Processor;

namespace SpeechPlugin
{
    [Export(typeof(ZvsPlugin))]
    public class SpeechPlugin : ZvsPlugin, IDisposable
    {
        private SpeechSynthesizer _synth;

        public override Guid PluginGuid
        {
            get { return Guid.Parse("e843db7b-9fbe-444e-a53c-3858806e67ee"); }
        }

        public override string Name
        {
            get { return "Speech Announce Plug-in for ZVS"; }
        }

        public override string Description
        {
            get { return "This plug-in will announce when zVirtualScene devices change values. It can be customized to announce only desired value changes."; }
        }

        private string _announceOptionSetting = "";
        public string AnnounceOptionSetting
        {
            get { return _announceOptionSetting; }
            set
            {
                if (value == _announceOptionSetting) return;
                _announceOptionSetting = value;
                NotifyPropertyChanged();
            }
        }

        private string _customAnnounceSetting = "";
        public string CustomAnnounceSetting
        {
            get { return _customAnnounceSetting; }
            set
            {
                if (value == _customAnnounceSetting) return;
                _customAnnounceSetting = value;
                NotifyPropertyChanged();
            }
        }

        public override async Task OnSettingsCreating(PluginSettingBuilder settingBuilder)
        {
            var annouceoptionssetting = new PluginSetting
            {
                Name = "Announce options",
                Value = "Level",
                ValueType = DataType.LIST,
                Description = "Select the values you would like announced."
            };
            annouceoptionssetting.Options.Add(new PluginSettingOption { Name = "Switch Level" });
            annouceoptionssetting.Options.Add(new PluginSettingOption { Name = "Dimmer Level" });
            annouceoptionssetting.Options.Add(new PluginSettingOption { Name = "Thermostat Operating State and Temp" });
            annouceoptionssetting.Options.Add(new PluginSettingOption { Name = "All of the above" });
            annouceoptionssetting.Options.Add(new PluginSettingOption { Name = "Custom" });

            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(annouceoptionssetting, o => o.AnnounceOptionSetting);

            var customsetting = new PluginSetting
           {
               Name = "Announce on custom values",
               Value = "dimmer:Basic, thermostat:Temperature, switch:Basic, thermostat:Operating State",
               ValueType = DataType.STRING,
               Description = "Include all values you would like announced. (DEVICE_TYPE_NAME:VALUE_LABEL_NAME) Comma Separated."
           };

            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(customsetting, o => o.CustomAnnounceSetting);
        }

        public override async Task StartAsync()
        {
            _synth = new SpeechSynthesizer();
            await Log.ReportInfoFormatAsync(CancellationToken, "{0} started", Name);
            NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.OnEntityUpdated += SpeechPlugin_OnEntityUpdated;
            _synth.SpeakAsync("Speech Started!");
        }


        public override async Task StopAsync()
        {
            await Log.ReportInfoFormatAsync(CancellationToken, "{0} stopped", Name);
            NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.OnEntityUpdated -= SpeechPlugin_OnEntityUpdated;
            _synth.Dispose();
        }

        async void SpeechPlugin_OnEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.EntityUpdatedArgs e)
        {
            using (var context = new ZvsContext(EntityContextConnection))
            {
                var dv = await context.DeviceValues
                    .Include(o => o.Device.Type)
                    .FirstOrDefaultAsync(v => v.Id == e.NewEntity.Id, CancellationToken);

                if (dv == null)
                    return;

                var deviceTypeUId = dv.Device.Type.UniqueIdentifier;

                if (AnnounceOptionSetting == "Switch Level" || AnnounceOptionSetting == "All of the above")
                {
                    if (deviceTypeUId == "SWITCH" && dv.Name == "Basic")
                    {
                        _synth.SpeakAsync(dv.Device.Name + " switched " + (dv.Value == "255" ? "On" : "Off") + ".");
                    }
                }

                if (AnnounceOptionSetting == "Dimmer Level" || AnnounceOptionSetting == "All of the above")
                {
                    if (deviceTypeUId == "DIMMER" && dv.Name == "Level")
                    {
                        _synth.SpeakAsync(dv.Device.Name + " " + dv.Name + " changed to " + dv.Value + ".");
                    }
                }

                if (AnnounceOptionSetting == "Thermostat Operating State and Temp" || AnnounceOptionSetting == "All of the above")
                {
                    if (deviceTypeUId == "THERMOSTAT" && dv.Name == "Temperature")
                    {
                        _synth.SpeakAsync(dv.Device.Name + " " + dv.Name + " changed to " + dv.Value + ".");
                    }

                    if (deviceTypeUId == "THERMOSTAT" && dv.Name == "Operating State")
                    {
                        _synth.SpeakAsync(dv.Device.Name + " " + dv.Name + " changed to " + dv.Value + ".");
                    }
                }
                if (AnnounceOptionSetting != "Custom") return;
                var objTypeValuespairs = CustomAnnounceSetting.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var objTypeValuespair in objTypeValuespairs)
                {
                    var thisEvent = string.Format("{0}:{1}", deviceTypeUId, dv.Name);

                    if (thisEvent.Equals(objTypeValuespair.Trim()))
                        _synth.SpeakAsync(string.Format("{0} {1}  changed to {2}.", dv.Device.Name, dv.Name, dv.Value));
                }
            }
        }

        public void Announce(String announceText)
        {
            _synth.SpeakAsync(announceText);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (_synth == null)
            {
                return;
            }

            _synth.Dispose();
        }
    }
}
