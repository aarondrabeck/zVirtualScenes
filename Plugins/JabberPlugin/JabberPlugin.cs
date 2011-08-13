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
using jabber.client;
using zVirtualScenesAPI.Events;

namespace JabberPlugin
{
    [Export(typeof(Plugin))]
    public class JabberPlugin : Plugin
    {
        JabberClient j;
        public volatile bool isActive;
        private bool shuttingdown = false; 
       
        public JabberPlugin()
            : base("JABBER")
        {
            PluginName = "Jabber";
        }

        protected override bool StartPlugin()
        {
            shuttingdown = false;
            API.WriteToLog(Urgency.INFO, PluginName + " plugin started.");

            j = new JabberClient();
            j.OnMessage += new MessageHandler(jabberClient1_OnMessage);
            j.OnDisconnect += new bedrock.ObjectHandler(jabberClient1_OnDisconnect);
            j.OnError += new bedrock.ExceptionHandler(jabberClient1_OnError);
            j.OnAuthError += new jabber.protocol.ProtocolHandler(jabberClient1_OnAuthError);
            j.OnAuthenticate += new bedrock.ObjectHandler(jabberClient1_OnAuthenticate);
            j.OnReadText += new bedrock.TextHandler(j_OnReadText);
            j.OnWriteText += new bedrock.TextHandler(j_OnWriteText);
            
            if (!j.IsAuthenticated)
            {
                j.User = API.GetSetting("Jabber Username");
                j.Server = API.GetSetting("Jabber Server");
                j.Password = API.GetSetting("Jabber Password");
                j.AutoRoster = false;
                j.AutoPresence = false;
                j.AutoReconnect = 50;
                j.Connect();
            }

            zVirtualSceneEvents.ValueDataChangedEvent += new zVirtualSceneEvents.ValueDataChangedEventHandler(zVirtualSceneEvents_ValueDataChangedEvent);
            
            IsReady = true;
            return true;
        }       

        protected override bool StopPlugin()
        {
            API.WriteToLog(Urgency.INFO, PluginName + " plugin ended.");

            Disconnect();

            //supresses messages sent to log while program is closing
            shuttingdown = true;

            //Wait to shutdown
            System.Threading.Thread.Sleep(500);
            isActive = false;
            j.Dispose();

            zVirtualSceneEvents.ValueDataChangedEvent -= new zVirtualSceneEvents.ValueDataChangedEventHandler(zVirtualSceneEvents_ValueDataChangedEvent);
            
            IsReady = false;
            return true;
        }

        protected override void SettingChanged(string settingName, string settingValue)
        {
            
        }

        public override void Initialize()
        {                                 
            API.DefineSetting("Jabber Server", "gmail.com", ParamType.STRING, "The Jabber server to connect to.");
            API.DefineSetting("Jabber Username", "user", ParamType.STRING, "The username of the jabber user.");
            API.DefineSetting("Jabber Password", "passw0rd", ParamType.STRING, "The password of the jabber user."); 
            API.DefineSetting("Send to", "user@gmail.com", ParamType.STRING, "Jabber users that will receive notifications. (comma seperated)");
            API.DefineSetting("Verbose Logging", "true", ParamType.BOOL, "(Writes all server client communication to the log for debugging.)");
            API.DefineSetting("Notifications to send", "DIMMER:Basic, THERMOSTAT:Temperature, SWITCH:Basic, THERMOSTAT:Operating State", ParamType.STRING, "Include all values you would like announced. Comma Seperated.");
        }

        void zVirtualSceneEvents_ValueDataChangedEvent(int ObjectId, string ValueID, string label, string Value, string PreviousValue)
        {
            string objType = API.Object.GetObjectType(ObjectId);
            string objName = API.Object.GetObjectName(ObjectId);

            string[] objTypeValuespairs = API.GetSetting("Notifications to send").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string objTypeValuespair in objTypeValuespairs)
            {
                string thisEvent = objType + ":" + label;

                if (thisEvent.Equals(objTypeValuespair.Trim()))
                   SendMessage(objName + " " + label + " changed to " + Value + ".");
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

        public void Disconnect()
        {
            if(j.IsAuthenticated)
                j.Close();
        }

        public void SendMessage(string msg)
        {
            string[] users = API.GetSetting("Send to").Split(',');

            if (msg != null)
            {
                foreach(string user in users)                
                    j.Message(user, msg);
            }
         }

        private void jabberClient1_OnAuthError(object sender, System.Xml.XmlElement rp)
        {
            if (rp.Name == "failure")
            {
                if(!shuttingdown)
                    API.WriteToLog(Urgency.WARNING, "Invalid Username or Password.");
            }
        }

        private void jabberClient1_OnAuthenticate(object sender)
        {
            if (!shuttingdown)
                API.WriteToLog(Urgency.INFO, "Connected using " + j.User + ".");
            j.Presence(jabber.protocol.client.PresenceType.available, "I am a " + API.GetProgramNameAndVersion + " server.", ":chat", 0);
            isActive = true;
        }

        private void jabberClient1_OnError(object sender, Exception ex)
        {
            if (!shuttingdown)
                API.WriteToLog(Urgency.ERROR, ex.Message);
        }

        private void jabberClient1_OnDisconnect(object sender)
        {
            API.WriteToLog(Urgency.INFO, "Disconnected.");             
        }

        private void jabberClient1_OnMessage(object sender, jabber.protocol.client.Message msg)
        {
            bool verbose = false;
            bool.TryParse(API.GetSetting("Verbose Logging"), out verbose);

            if (verbose && !shuttingdown)
                API.WriteToLog(Urgency.INFO, "[" + msg.From.User + "] says : " + msg.Body + "\n");
        }

        private void j_OnWriteText(object sender, string txt)
        {
            bool verbose = false;
            bool.TryParse(API.GetSetting("Verbose Logging"), out verbose);

            if (txt == " ") return;
            if (verbose && !shuttingdown)
                API.WriteToLog(Urgency.INFO, "SENT: " + txt);
        }

        private void j_OnReadText(object sender, string txt)
        {
            bool verbose = false;
            bool.TryParse(API.GetSetting("Verbose Logging"), out verbose);

            if (txt == " ") return;  // ignore keep-alive spaces
            if (verbose && !shuttingdown)
                API.WriteToLog(Urgency.INFO, "RECV: " + txt);
        }
        
    }
    
}

