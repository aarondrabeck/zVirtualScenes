using System;
using System.ComponentModel.Composition;
using System.Speech.Synthesis;
using zVirtualScenesAPI;
using zVirtualScenesAPI.Events;
using System.Collections.Generic;

namespace SpeechPlugin
{
    [Export(typeof(zvsPlugin))]
    public class SpeechPlugin : zvsPlugin
    {
        private SpeechSynthesizer _synth;

        public SpeechPlugin()
            : base("SPEECH")
        {
            PluginName = "Speech";
        }

        protected override bool StartPlugin()
        {
            zvsEvents.ValueDataChangedEvent += new zvsEvents.ValueDataChangedEventHandler(zVirtualSceneEvents_ValueChangedEvent);

            zvsAPI.WriteToLog(Urgency.INFO, PluginName + " plugin started.");
            _synth.SpeakAsync("Speech Started!");
            IsReady = true;
            return true;
        }

        protected override bool StopPlugin()
        {
            zvsEvents.ValueDataChangedEvent -= new zvsEvents.ValueDataChangedEventHandler(zVirtualSceneEvents_ValueChangedEvent);
            zvsAPI.WriteToLog(Urgency.INFO, PluginName + " plugin ended.");
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

            zvsAPI.InstallObjectType("SPEECH", false);
            zvsAPI.NewObjectTypeCommand("SPEECH", "SAY", "Say", Data_Types.STRING, "Used to make the computer say any command you want");

            zvsAPI.NewObject(1, "SPEECH", "SPEECH");

            zvsAPI.DefineSetting("Enable announce on", "Level", Data_Types.LIST, "Select the values to annouce.");
            zvsAPI.NewPluginSettingOption("Enable announce on", "Switch Level");
            zvsAPI.NewPluginSettingOption("Enable announce on", "Dimmer Level");
            zvsAPI.NewPluginSettingOption("Enable announce on", "Thermostat Operating State and Temp");
            zvsAPI.NewPluginSettingOption("Enable announce on", "All of the above");
            zvsAPI.NewPluginSettingOption("Enable announce on", "Custom");
            zvsAPI.DefineSetting("Announce on custom values", "DIMMER:Basic, THERMOSTAT:Temperature, SWITCH:Basic, THERMOSTAT:Operating State", Data_Types.STRING, "Include all values you would like announced. Comma Seperated.");

        }

        public override void ProcessCommand(QuedCommand cmd)
        {
            Command cmdInfo = zvsAPI.Commands.GetCommand(cmd.CommandId, cmd.cmdtype);
            switch (cmdInfo.Name)
            {
                case "SAY":
                    _synth.SpeakAsync(cmd.Argument);
                    break;
            }
        }

        void zVirtualSceneEvents_ValueChangedEvent(int ObjectId, string ValueID, string label, string Value, string PreviousValue)
        {
            string objType = zvsAPI.Object.GetObjectType(ObjectId);
            string objName = zvsAPI.Object.GetObjectName(ObjectId);

            string AnnounceSetting = zvsAPI.GetSetting("Enable Announce On");

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
                string[] objTypeValuespairs = zvsAPI.GetSetting("Announce on custom values").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

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
