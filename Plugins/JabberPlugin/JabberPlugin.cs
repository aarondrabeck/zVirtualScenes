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
    [Export(typeof(zvsPlugin))]
    public class JabberPlugin : zvsPlugin
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
            zvsAPI.WriteToLog(Urgency.INFO, PluginName + " plugin started.");

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
                j.User = zvsAPI.GetSetting("Jabber Username");
                j.Server = zvsAPI.GetSetting("Jabber Server");
                j.Password = zvsAPI.GetSetting("Jabber Password");
                j.AutoRoster = false;
                j.AutoPresence = false;
                j.AutoReconnect = 50;
                j.Connect();
            }

            zvsEvents.ValueDataChangedEvent += new zvsEvents.ValueDataChangedEventHandler(zVirtualSceneEvents_ValueDataChangedEvent);
            
            IsReady = true;
            return true;
        }       

        protected override bool StopPlugin()
        {
            zvsAPI.WriteToLog(Urgency.INFO, PluginName + " plugin ended.");

            Disconnect();

            //supresses messages sent to log while program is closing
            shuttingdown = true;

            //Wait to shutdown
            System.Threading.Thread.Sleep(500);
            isActive = false;
            j.Dispose();

            zvsEvents.ValueDataChangedEvent -= new zvsEvents.ValueDataChangedEventHandler(zVirtualSceneEvents_ValueDataChangedEvent);
            
            IsReady = false;
            return true;
        }

        protected override void SettingChanged(string settingName, string settingValue)
        {
            
        }

        public override void Initialize()
        {                                 
            zvsAPI.DefineSetting("Jabber Server", "gmail.com", Data_Types.STRING, "The Jabber server to connect to.");
            zvsAPI.DefineSetting("Jabber Username", "user", Data_Types.STRING, "The username of the jabber user.");
            zvsAPI.DefineSetting("Jabber Password", "passw0rd", Data_Types.STRING, "The password of the jabber user."); 
            zvsAPI.DefineSetting("Send to", "user@gmail.com", Data_Types.STRING, "Jabber users that will receive notifications. (comma seperated)");
            zvsAPI.DefineSetting("Verbose Logging", "true", Data_Types.BOOL, "(Writes all server client communication to the log for debugging.)");
            zvsAPI.DefineSetting("Notifications to send", "DIMMER:Basic, THERMOSTAT:Temperature, SWITCH:Basic, THERMOSTAT:Operating State", Data_Types.STRING, "Include all values you would like announced. Comma Seperated.");
        }

        void zVirtualSceneEvents_ValueDataChangedEvent(int ObjectId, string ValueID, string label, string Value, string PreviousValue)
        {
            string objType = zvsAPI.Object.GetObjectType(ObjectId);
            string objName = zvsAPI.Object.GetObjectName(ObjectId);

            string[] objTypeValuespairs = zvsAPI.GetSetting("Notifications to send").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

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
            string[] users = zvsAPI.GetSetting("Send to").Split(',');

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
                    zvsAPI.WriteToLog(Urgency.WARNING, "Invalid Username or Password.");
            }
        }

        private void jabberClient1_OnAuthenticate(object sender)
        {
            if (!shuttingdown)
                zvsAPI.WriteToLog(Urgency.INFO, "Connected using " + j.User + ".");
            j.Presence(jabber.protocol.client.PresenceType.available, "I am a " + zvsAPI.GetProgramNameAndVersion + " server.", ":chat", 0);
            isActive = true;
        }

        private void jabberClient1_OnError(object sender, Exception ex)
        {
            if (!shuttingdown)
                zvsAPI.WriteToLog(Urgency.ERROR, ex.Message);
        }

        private void jabberClient1_OnDisconnect(object sender)
        {
            zvsAPI.WriteToLog(Urgency.INFO, "Disconnected.");             
        }

        private void jabberClient1_OnMessage(object sender, jabber.protocol.client.Message msg)
        {
            bool verbose = false;
            bool.TryParse(zvsAPI.GetSetting("Verbose Logging"), out verbose);

            if (verbose && !shuttingdown)
                zvsAPI.WriteToLog(Urgency.INFO, "[" + msg.From.User + "] says : " + msg.Body + "\n");
        }

        private void j_OnWriteText(object sender, string txt)
        {
            bool verbose = false;
            bool.TryParse(zvsAPI.GetSetting("Verbose Logging"), out verbose);

            if (txt == " ") return;
            if (verbose && !shuttingdown)
                zvsAPI.WriteToLog(Urgency.INFO, "SENT: " + txt);
        }

        private void j_OnReadText(object sender, string txt)
        {
            bool verbose = false;
            bool.TryParse(zvsAPI.GetSetting("Verbose Logging"), out verbose);

            if (txt == " ") return;  // ignore keep-alive spaces
            if (verbose && !shuttingdown)
                zvsAPI.WriteToLog(Urgency.INFO, "RECV: " + txt);
        }
        
    }
    
}

