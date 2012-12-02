using System;
using System.ComponentModel.Composition;
using System.Speech.Synthesis;
using System.Collections.Generic;
using System.Linq;
using zvs.Processor;

using System.ComponentModel;
using zvs.Entities;

namespace SpeechPlugin
{
    [Export(typeof(zvsPlugin))]
    public class SpeechPlugin : zvsPlugin
    {
        private SpeechSynthesizer _synth;
        zvs.Processor.Logging.ILog log = zvs.Processor.Logging.LogManager.GetLogger<SpeechPlugin>();
        public SpeechPlugin()
            : base("SPEECH",
               "Speech Announce Plug-in",
                "This plug-in will announce when zVirtualScene devices change values. It can be customized to announce only desired value changes."
                ) { }

        public override void Initialize()
        {
            using (zvsContext context = new zvsContext())
            {
                PluginSetting ps = new PluginSetting
                {
                    UniqueIdentifier = "ANNOUCEOPTIONS",                    
                    Name = "Announce options",
                    Value = "Level",
                    ValueType = DataType.LIST,
                    Description = "Select the values you would like announced."
                };
                ps.Options.Add(new PluginSettingOption { Name = "Switch Level" });
                ps.Options.Add(new PluginSettingOption { Name = "Dimmer Level" });
                ps.Options.Add(new PluginSettingOption { Name = "Thermostat Operating State and Temp" });
                ps.Options.Add(new PluginSettingOption { Name = "All of the above" });
                ps.Options.Add(new PluginSettingOption { Name = "Custom" });
                DefineOrUpdateSetting(ps, context);

                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "CUSTOMVALUES",
                    Name = "Announce on custom values",
                    Value = "DIMMER:Basic, THERMOSTAT:Temperature, SWITCH:Basic, THERMOSTAT:Operating State",
                    ValueType = DataType.STRING,
                    Description = "Include all values you would like announced. (DEVICE_TYPE_NAME:VALUE_LABEL_NAME) Comma Separated."
                }, context);
            }
        }

        protected override void StartPlugin()
        {
            _synth = new SpeechSynthesizer();
            DeviceValue.DeviceValueDataChangedEvent += DeviceValue_DeviceValueDataChangedEvent;
            log.Info( this.Name + " started");
            _synth.SpeakAsync("Speech Started!");
            IsReady = true;
        }

        protected override void StopPlugin()
        {
            DeviceValue.DeviceValueDataChangedEvent -= DeviceValue_DeviceValueDataChangedEvent;
            log.Info( this.Name + " stopped");
            _synth.Dispose();
            IsReady = false;
        }

        protected override void SettingChanged(string settingsettingUniqueIdentifier, string settingValue) { }

        public override void ProcessCommand(int queuedCommandId) { }
              
        public override void Repoll(zvs.Entities.Device device) { }

        public override void ActivateGroup(int groupID) { }

        public override void DeactivateGroup(int groupID) { }

        private void DeviceValue_DeviceValueDataChangedEvent(object sender, DeviceValue.ValueDataChangedEventArgs args)
        {
            if (IsReady)
            {
                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += (s, a) =>
                {
                    using (zvsContext context = new zvsContext())
                    {
                        DeviceValue dv = context.DeviceValues.FirstOrDefault(v => v.Id == args.DeviceValueId);
                        if (dv != null)
                        {

                            string user_selected_announce_option = GetSettingValue("ANNOUCEOPTIONS", context);

                            if (user_selected_announce_option == "Switch Level" || user_selected_announce_option == "All of the above")
                            {
                                if (dv.Device.Type.UniqueIdentifier == "SWITCH" && dv.Name == "Basic")
                                {
                                    _synth.SpeakAsync(dv.Device.Name + " switched " + (dv.Value == "255" ? "On" : "Off") + ".");
                                }
                            }

                            if (user_selected_announce_option == "Dimmer Level" || user_selected_announce_option == "All of the above")
                            {
                                if (dv.Device.Type.UniqueIdentifier == "DIMMER" && dv.Name == "Level")
                                {
                                    _synth.SpeakAsync(dv.Device.Name + " " + dv.Name + " changed to " + dv.Value + ".");
                                }
                            }

                            if (user_selected_announce_option == "Thermostat Operating State and Temp" || user_selected_announce_option == "All of the above")
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
                            if (user_selected_announce_option == "Custom")
                            {
                                string[] objTypeValuespairs = GetSettingValue("CUSTOMVALUES", context).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                                foreach (string objTypeValuespair in objTypeValuespairs)
                                {
                                    string thisEvent = dv.Device.Type.UniqueIdentifier + ":" + dv.Name;

                                    if (thisEvent.Equals(objTypeValuespair.Trim()))
                                        _synth.SpeakAsync(dv.Device.Name + " " + dv.Name + " changed to " + dv.Value + ".");
                                }
                            }
                        }
                    }
                };
                bw.RunWorkerAsync();
            }
        }


    }
}
