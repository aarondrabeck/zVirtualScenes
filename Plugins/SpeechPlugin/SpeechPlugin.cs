using System;
using System.ComponentModel.Composition;
using System.Speech.Synthesis;
using zVirtualScenesAPI;
using zVirtualScenesAPI.Events;
using System.Collections.Generic;
using zVirtualScenesAPI.Structs;

namespace SpeechPlugin
{
    [Export(typeof(Plugin))]
    public class SpeechPlugin : Plugin
    {
        private SpeechSynthesizer _synth;

        public SpeechPlugin()
            : base("SPEECH")
        {
            PluginName = "Speech";
        }

        protected override bool StartPlugin()
        {
            zVirtualSceneEvents.ValueDataChangedEvent += new zVirtualSceneEvents.ValueDataChangedEventHandler(zVirtualSceneEvents_ValueChangedEvent);

            API.WriteToLog(Urgency.INFO, PluginName + " plugin started.");
            _synth.SpeakAsync("Speech Started!");
            IsReady = true;
            return true;
        }

        protected override bool StopPlugin()
        {
            zVirtualSceneEvents.ValueDataChangedEvent -= new zVirtualSceneEvents.ValueDataChangedEventHandler(zVirtualSceneEvents_ValueChangedEvent);
            API.WriteToLog(Urgency.INFO, PluginName + " plugin ended.");
            _synth.Dispose();
            IsReady = false;
            return true;
        }

        public override void ActivateGroup(string GroupName)
        { }

        public override void DeactivateGroup(string GroupName)
        { }

        protected override void SettingChanged(string settingName, string settingValue)
        {
            
        }

        public override void Initialize()
        {
            _synth = new SpeechSynthesizer();

            API.InstallObjectType("SPEECH", false);
            API.NewObjectTypeCommand("SPEECH", "SAY", "Say", ParamType.STRING, "Used to make the computer say any command you want");

            API.NewObject(1, "SPEECH", "SPEECH");

            API.DefineSetting("Enable announce on", "Level", ParamType.LIST, "Select the values to annouce.");
            API.NewPluginSettingOption("Enable announce on", "Switch Level");
            API.NewPluginSettingOption("Enable announce on", "Dimmer Level");
            API.NewPluginSettingOption("Enable announce on", "Thermostat Operating State and Temp");
            API.NewPluginSettingOption("Enable announce on", "All of the above");
            API.NewPluginSettingOption("Enable announce on", "Custom");
            API.DefineSetting("Announce on custom values", "DIMMER:Basic, THERMOSTAT:Temperature, SWITCH:Basic, THERMOSTAT:Operating State", ParamType.STRING, "Include all values you would like announced. Comma Seperated.");

        }

        public override void ProcessCommand(QuedCommand cmd)
        {
            Command cmdInfo = API.Commands.GetCommand(cmd.CommandId, cmd.cmdtype);
            switch (cmdInfo.Name)
            {
                case "SAY":
                    _synth.SpeakAsync(cmd.Argument);
                    break;
            }
        }

        void zVirtualSceneEvents_ValueChangedEvent(int ObjectId, string ValueID, string label, string Value, string PreviousValue)
        {
            string objType = API.Object.GetObjectType(ObjectId);
            string objName = API.Object.GetObjectName(ObjectId);

            string AnnounceSetting = API.GetSetting("Enable Announce On");

            if(AnnounceSetting == "Switch Level" || AnnounceSetting == "All of the above")
            {
                if (objType == "SWITCH" && label == "Basic")
                {                
                    _synth.SpeakAsync(objName + " switched " + (Value == "255" ? "On" : "Off") + ".");
                }
            }

            if(AnnounceSetting == "Dimmer Level" || AnnounceSetting == "All of the above")
            {
                if (objType == "DIMMER" && label == "Level")
                {                
                _synth.SpeakAsync(objName + " " + label + " changed to " + Value + ".");
                }
            }

            if(AnnounceSetting == "Thermostat Operating State and Temp" || AnnounceSetting == "All of the above")
            {
                if (objType == "THERMOSTAT" && label == "Temperature")
                {
                    _synth.SpeakAsync(objName + " " + label + " changed to " + Value + ".");
                }

                if (objType == "THERMOSTAT" && label == "Operating State")
                {
                    _synth.SpeakAsync(objName + " " + label + " changed to " + Value + ".");
                }
            }
            if (AnnounceSetting == "Custom")
            {
                string[] objTypeValuespairs = API.GetSetting("Announce on custom values").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string objTypeValuespair in objTypeValuespairs)
                {
                    string thisEvent = objType + ":" + label;

                    if(thisEvent.Equals(objTypeValuespair.Trim()))
                        _synth.SpeakAsync(objName + " " + label + " changed to " + Value + ".");
                }
            }            
        }

        public override void Repoll(string id)
        {

        }
    }
}
