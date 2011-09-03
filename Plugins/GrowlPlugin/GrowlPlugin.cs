using System.Net.Sockets;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System;
using System.Net;
using System.Threading;
using System.Text;
using zVirtualScenesAPI;
using zVirtualScenesAPI.Structs;
using System.Data;
using zVirtualScenesAPI.Events;
using Growl.Connector;
using System.Drawing;

namespace GrowlPlugin
{
    [Export(typeof(zvsPlugin))]
    public class GrowlPlugin : zvsPlugin
    {
        public GrowlPlugin()
            : base("GROWL")
        {
            PluginName = "Growl";
        }

        public const string NOTIFY_DEVICE_VALUE_CHANGE = "DEVICE_VALUE_CHANGE";
        public GrowlConnector GrowlConnector = new GrowlConnector();

        protected override bool StartPlugin()
        {

            zvsAPI.WriteToLog(Urgency.INFO, PluginName + " plugin started.");
            zvsEvents.ValueDataChangedEvent += new zvsEvents.ValueDataChangedEventHandler(zVirtualSceneEvents_ValueDataChangedEvent);
            
            RegisterGrowl(); 

            IsReady = true;
            return true;
        }

        protected override bool StopPlugin()
        {
            zvsAPI.WriteToLog(Urgency.INFO, PluginName + " plugin ended.");
            zvsEvents.ValueDataChangedEvent -= new zvsEvents.ValueDataChangedEventHandler(zVirtualSceneEvents_ValueDataChangedEvent);

            IsReady = false;
            return true;
        }

        protected override void SettingChanged(string settingName, string settingValue)
        {
        }

        public override void Initialize()
        {
            zvsAPI.DefineSetting("Notifications to send", "DIMMER:Basic, THERMOSTAT:Temperature, SWITCH:Basic, THERMOSTAT:Operating State", Data_Types.STRING, "Include all values you would like announced. Comma Seperated.");
        }

        void zVirtualSceneEvents_ValueDataChangedEvent(int ObjectId, string ValueID, string label, string Value, string PreviousValue)
        {
            if (IsReady)
            {
                string objType = zvsAPI.Object.GetObjectType(ObjectId);
                string objName = zvsAPI.Object.GetObjectName(ObjectId);

                string[] objTypeValuespairs = zvsAPI.GetSetting("Notifications to send").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string objTypeValuespair in objTypeValuespairs)
                {
                    string thisEvent = objType + ":" + label;

                    if (thisEvent.Equals(objTypeValuespair.Trim()))
                    {
                        Notification notification = new Notification("zVirtualScenes", NOTIFY_DEVICE_VALUE_CHANGE, "0", objName + " " + label, "Changed to " + Value + " from " + PreviousValue + ".");
                        GrowlConnector.Notify(notification);
                    }
                }
            }
        }

        public override void ProcessCommand(QuedCommand cmd)
        {
        }

        public override void Repoll(string id)
        {
        }

        public override void ActivateGroup(string GroupName)
        { }

        public override void DeactivateGroup(string GroupName)
        { }

        public void RegisterGrowl()
        {
            try
            {
                //string[] resourcenames = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();
                //foreach (string rname in resourcenames)
                //{
                //    API.WriteToLog(Urgency.INFO, rname);
                //}

                Growl.Connector.Application application = new Growl.Connector.Application("zVirtualScenes");
                string exePath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
                application.Icon = new Bitmap(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("GrowlPlugin.zvirtualscenes57.png"));

                NotificationType DeviceValueChange = new NotificationType(NOTIFY_DEVICE_VALUE_CHANGE, "Device Value Changed");
                DeviceValueChange.Icon = new Bitmap(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("GrowlPlugin.Broadcast48.png"));

               
                GrowlConnector.Register(application, new NotificationType[] { DeviceValueChange });
                zvsAPI.WriteToLog(Urgency.INFO, "Registered Growl Interface.");
            }
            catch (Exception ex)
            {
                zvsAPI.WriteToLog(Urgency.ERROR, "Error registering Growl. " + ex.Message);
            }
        }
    }
    
}

