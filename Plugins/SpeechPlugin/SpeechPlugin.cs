using System;
using System.ComponentModel.Composition;
using System.Speech.Synthesis;
using zvs.Processor;
using System.Data.Entity;
using System.ComponentModel;
using zvs.DataModel;
using System.Threading.Tasks;

namespace SpeechPlugin
{
    [Export(typeof(zvsPlugin))]
    public class SpeechPlugin : zvsPlugin, INotifyPropertyChanged, IDisposable
    {
        private SpeechSynthesizer _synth;
        zvs.Processor.Logging.ILog log = zvs.Processor.Logging.LogManager.GetLogger<SpeechPlugin>();

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

        private string _AnnounceOptionSetting = "";
        public string AnnounceOptionSetting
        {
            get { return _AnnounceOptionSetting; }
            set
            {
                if (value != _AnnounceOptionSetting)
                {
                    _AnnounceOptionSetting = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _CustomAnnounceSetting = "";
        public string CustomAnnounceSetting
        {
            get { return _CustomAnnounceSetting; }
            set
            {
                if (value != _CustomAnnounceSetting)
                {
                    _CustomAnnounceSetting = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public override async Task OnSettingsCreating(PluginSettingBuilder settingBuilder)
        {
            PluginSetting annouceoptionssetting = new PluginSetting
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
               Value = "DIMMER:Basic, THERMOSTAT:Temperature, SWITCH:Basic, THERMOSTAT:Operating State",
               ValueType = DataType.STRING,
               Description = "Include all values you would like announced. (DEVICE_TYPE_NAME:VALUE_LABEL_NAME) Comma Separated."
           };

            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(customsetting, o => o.CustomAnnounceSetting);
        }

        public override Task StartAsync()
        {
            _synth = new SpeechSynthesizer();
            log.Info(this.Name + " started");
            _synth.SpeakAsync("Speech Started!");
            return Task.FromResult(0);
        }

        public override Task StopAsync()
        {
            log.Info(this.Name + " stopped");
            _synth.Dispose();
            return Task.FromResult(0);
        }

        public override async Task DeviceValueChangedAsync(Int64 deviceValueId, string newValue, string oldValue)
        {
            using (ZvsContext context = new ZvsContext())
            {
                DeviceValue dv = await context.DeviceValues.FirstOrDefaultAsync(v => v.Id == deviceValueId);
                if (dv == null)
                    return;

                if (AnnounceOptionSetting == "Switch Level" || AnnounceOptionSetting == "All of the above")
                {
                    if (dv.Device.Type.UniqueIdentifier == "SWITCH" && dv.Name == "Basic")
                    {
                        _synth.SpeakAsync(dv.Device.Name + " switched " + (dv.Value == "255" ? "On" : "Off") + ".");
                    }
                }

                if (AnnounceOptionSetting == "Dimmer Level" || AnnounceOptionSetting == "All of the above")
                {
                    if (dv.Device.Type.UniqueIdentifier == "DIMMER" && dv.Name == "Level")
                    {
                        _synth.SpeakAsync(dv.Device.Name + " " + dv.Name + " changed to " + dv.Value + ".");
                    }
                }

                if (AnnounceOptionSetting == "Thermostat Operating State and Temp" || AnnounceOptionSetting == "All of the above")
                {
                    if (dv.Device.Type.UniqueIdentifier == "THERMOSTAT" && dv.Name == "Temperature")
                    {
                        _synth.SpeakAsync(dv.Device.Name + " " + dv.Name + " changed to " + dv.Value + ".");
                    }

                    if (dv.Device.Type.UniqueIdentifier == "THERMOSTAT" && dv.Name == "Operating State")
                    {
                        _synth.SpeakAsync(dv.Device.Name + " " + dv.Name + " changed to " + dv.Value + ".");
                    }
                }
                if (AnnounceOptionSetting == "Custom")
                {
                    string[] objTypeValuespairs = CustomAnnounceSetting.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string objTypeValuespair in objTypeValuespairs)
                    {
                        string thisEvent = dv.Device.Type.UniqueIdentifier + ":" + dv.Name;

                        if (thisEvent.Equals(objTypeValuespair.Trim()))
                            _synth.SpeakAsync(dv.Device.Name + " " + dv.Name + " changed to " + dv.Value + ".");
                    }
                }
            }
        }

        public void Announce(String announceText)
        {
            _synth.SpeakAsync(announceText);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._synth == null)
                {
                    return;
                }

                _synth.Dispose();
            }
        }
    }
}
