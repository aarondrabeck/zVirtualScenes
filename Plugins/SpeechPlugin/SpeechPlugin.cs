using System;
using System.ComponentModel.Composition;
using System.Speech.Synthesis;
using System.Collections.Generic;
using zVirtualScenesAPI;
using zVirtualScenesCommon.Entity;
using zVirtualScenesCommon;
using System.Linq;

namespace SpeechPlugin
{
    [Export(typeof(Plugin))]
    public class SpeechPlugin : Plugin
    {
        private SpeechSynthesizer _synth;

        public SpeechPlugin()
            : base("SPEECH",
               "Speech Annouce Plugin",
                "This plug-in will announce when zVirtualScene devices change values. It can be customized to announce only desired value changes."
                ) { }

        public override void Initialize()
        {
            _synth = new SpeechSynthesizer();

            plugin_settings  ps  = new plugin_settings
            {
                name = "ANNOUCEOPTIONS",
                friendly_name = "Announce options",
                value = "Level",
                value_data_type = (int)Data_Types.LIST,
                description = "Select the values you would like announced."
            };
            ps.plugin_setting_options.Add(new plugin_setting_options { options = "Switch Level" } );
            ps.plugin_setting_options.Add(new plugin_setting_options { options = "Dimmer Level" });
            ps.plugin_setting_options.Add(new plugin_setting_options { options = "Thermostat Operating State and Temp" });
            ps.plugin_setting_options.Add(new plugin_setting_options { options = "All of the above" });
            ps.plugin_setting_options.Add(new plugin_setting_options { options = "Custom" });
            DefineOrUpdateSetting(ps);

            DefineOrUpdateSetting(new plugin_settings
            {
                name = "CUSTOMVALUES",
                friendly_name = "Announce on custom values",
                value = "DIMMER:Basic, THERMOSTAT:Temperature, SWITCH:Basic, THERMOSTAT:Operating State",
                value_data_type = (int)Data_Types.STRING,
                description = "Include all values you would like announced. (DEVICE_TYPE_NAME:VALUE_LABEL_NAME) Comma Seperated."
            });
        }

        protected override bool StartPlugin()
        {
            device_values.DeviceValueDataChangedEvent += new device_values.ValueDataChangedEventHandler(device_values_DeviceValueDataChangedEvent);
            WriteToLog(Urgency.INFO, this.Friendly_Name + " plugin started.");
            _synth.SpeakAsync("Speech Started!");
            IsReady = true;
            return true;
        }

        protected override bool StopPlugin()
        {
            device_values.DeviceValueDataChangedEvent -= new device_values.ValueDataChangedEventHandler(device_values_DeviceValueDataChangedEvent);
            WriteToLog(Urgency.INFO, this.Friendly_Name + " plugin ended.");
            _synth.Dispose();
            IsReady = false;
            return true;
        }

        protected override void SettingChanged(string settingName, string settingValue)
        {
        }
        public override bool ProcessDeviceCommand(device_command_que cmd)
        {
            return true;
        }
        public override bool ProcessDeviceTypeCommand(device_type_command_que cmd)
        {
            return true;
        }
        public override bool Repoll(device device)
        {
            return true;
        }
        public override bool ActivateGroup(int groupID)
        {
            return true;
        }
        public override bool DeactivateGroup(int groupID)
        {
            return true;
        }             
                
        void device_values_DeviceValueDataChangedEvent(object sender, device_values.ValueDataChangedEventArgs args)
        {
            if (IsReady)
            {
                using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                {
                    device_values dv = db.device_values.FirstOrDefault(v => v.id == args.device_value_id);
                    if (dv != null)
                    {

                        string user_selected_announce_option = GetSettingValue("ANNOUCEOPTIONS");

                        if (user_selected_announce_option == "Switch Level" || user_selected_announce_option == "All of the above")
                        {
                            if (dv.device.device_types.name == "SWITCH" && dv.label_name == "Basic")
                            {
                                _synth.SpeakAsync(dv.device.friendly_name + " switched " + (dv.value2 == "255" ? "On" : "Off") + ".");
                            }
                        }

                        if (user_selected_announce_option == "Dimmer Level" || user_selected_announce_option == "All of the above")
                        {
                            if (dv.device.device_types.name == "DIMMER" && dv.label_name == "Level")
                            {
                                _synth.SpeakAsync(dv.device.friendly_name + " " + dv.label_name + " changed to " + dv.value2 + ".");
                            }
                        }

                        if (user_selected_announce_option == "Thermostat Operating State and Temp" || user_selected_announce_option == "All of the above")
                        {
                            if (dv.device.device_types.name == "THERMOSTAT" && dv.label_name == "Temperature")
                            {
                                _synth.SpeakAsync(dv.device.friendly_name + " " + dv.label_name + " changed to " + dv.value2 + ".");
                            }

                            if (dv.device.device_types.name == "THERMOSTAT" && dv.label_name == "Operating State")
                            {
                                _synth.SpeakAsync(dv.device.friendly_name + " " + dv.label_name + " changed to " + dv.value2 + ".");
                            }
                        }
                        if (user_selected_announce_option == "Custom")
                        {
                            string[] objTypeValuespairs = GetSettingValue("CUSTOMVALUES").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (string objTypeValuespair in objTypeValuespairs)
                            {
                                string thisEvent = dv.device.device_types.name + ":" + dv.label_name;

                                if (thisEvent.Equals(objTypeValuespair.Trim()))
                                    _synth.SpeakAsync(dv.device.friendly_name + " " + dv.label_name + " changed to " + dv.value2 + ".");
                            }
                        }
                    }
                }
            }
        }       
    }
}
