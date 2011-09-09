using System.Net.Sockets;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System;
using System.Net;
using System.Threading;
using System.Text;
using System.Data;
using jabber.client;
using zVirtualScenesAPI;
using zVirtualScenesCommon;
using zVirtualScenesCommon.Entity;

namespace JabberPlugin
{
    [Export(typeof(Plugin))]
    public class JabberPlugin : Plugin
    {
        JabberClient j;
        public volatile bool isActive;
        private bool shuttingdown = false;

        public JabberPlugin()
            : base("JABBER",
               "Jabber/Gtalk Plugin",
                "This plug-in will send customizable notifications to one or more jabber / gtalk users."
                ) { }

        public override void Initialize()
        {
            DefineOrUpdateSetting(new plugin_settings
            {
                name = "JABBERSERVER",
                friendly_name = "Jabber Server",
                value = "talk.google.com",
                value_data_type = (int)Data_Types.STRING,
                description = "The Jabber server to connect to (ex. talk.google.com)."
            });

            DefineOrUpdateSetting(new plugin_settings
            {
                name = "JABBERUSER",
                friendly_name = "Jabber Username",
                value = "user",
                value_data_type = (int)Data_Types.STRING,
                description = "The username of the jabber user."
            });

            DefineOrUpdateSetting(new plugin_settings
            {
                name = "JABBERPASSWORD",
                friendly_name = "Jabber Password",
                value = "passw0rd",
                value_data_type = (int)Data_Types.STRING,
                description = "The password of the jabber user."
            });

            DefineOrUpdateSetting(new plugin_settings
            {
                name = "JABBERSENDTO",
                friendly_name = "Send notifications to",
                value = "user@gmail.com",
                value_data_type = (int)Data_Types.STRING,
                description = "Jabber users that will receive notifications. (comma seperated)"
            });

            DefineOrUpdateSetting(new plugin_settings
            {
                name = "JABBERVERBOSE",
                friendly_name = "Verbose Logging",
                value = "false",
                value_data_type = (int)Data_Types.BOOL,
                description = "(Writes all server client communication to the log for debugging.)"
            });

            DefineOrUpdateSetting(new plugin_settings
            {
                name = "JABBERNOTIFICATIONS",
                friendly_name = "Notifications to send",
                value = "DIMMER:Basic, THERMOSTAT:Temperature, SWITCH:Basic, THERMOSTAT:Operating State",
                value_data_type = (int)Data_Types.STRING,
                description = "Include all values you would like announced. Comma Seperated."
            });
        }
               
        protected override bool StartPlugin()
        {
            shuttingdown = false;
            WriteToLog(Urgency.INFO, this.Friendly_Name + " plugin started.");

            j = new JabberClient();
            j.OnMessage += new MessageHandler(jabberClient1_OnMessage);
            j.OnDisconnect += new bedrock.ObjectHandler(jabberClient1_OnDisconnect);
            j.OnError += new bedrock.ExceptionHandler(jabberClient1_OnError);
            j.OnAuthError += new jabber.protocol.ProtocolHandler(jabberClient1_OnAuthError);
            j.OnAuthenticate += new bedrock.ObjectHandler(jabberClient1_OnAuthenticate);
            j.OnReadText += new bedrock.TextHandler(j_OnReadText);
            j.OnWriteText += new bedrock.TextHandler(j_OnWriteText);
            j.OnInvalidCertificate += new System.Net.Security.RemoteCertificateValidationCallback(j_OnInvalidCertificate);
            
            if (!j.IsAuthenticated)
            {
                j.User = GetSettingValue("JABBERUSER");
                j.Server = GetSettingValue("JABBERSERVER");
                j.Password = GetSettingValue("JABBERPASSWORD");
                j.AutoRoster = false;
                j.AutoPresence = false;
                j.AutoReconnect = 50;
                j.Connect();
            }

            device_values.DeviceValueDataChangedEvent += new device_values.ValueDataChangedEventHandler(device_values_DeviceValueDataChangedEvent);
                        
            IsReady = true;
            return true;
        }

        bool j_OnInvalidCertificate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            throw new NotImplementedException();
        }               

        protected override bool StopPlugin()
        {
            WriteToLog(Urgency.INFO, this.Friendly_Name + " plugin ended.");
            Disconnect();

            //supresses messages sent to log while program is closing
            shuttingdown = true;

            //Wait to shutdown
            System.Threading.Thread.Sleep(500);
            isActive = false;
            j.Dispose();

            device_values.DeviceValueDataChangedEvent -= new device_values.ValueDataChangedEventHandler(device_values_DeviceValueDataChangedEvent);
            
            IsReady = false;
            return true;
        }

      

        void device_values_DeviceValueDataChangedEvent(object sender, string PreviousValue)
        {
            device_values dv = (device_values)sender;
                        
            string[] objTypeValuespairs = GetSettingValue("JABBERNOTIFICATIONS").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string objTypeValuespair in objTypeValuespairs)
            {
                string thisEvent = dv.device.device_types.name + ":" + dv.label_name;

                if (thisEvent.Equals(objTypeValuespair.Trim()))
                    SendMessage(dv.device.friendly_name + " " + dv.label_name + " changed to " + dv.value + ".");
            }
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
        public override bool ActivateGroup(long groupID)
        {
            return true;
        }
        public override bool DeactivateGroup(long groupID)
        {
            return true;
        }           

        public void Disconnect()
        {
            if(j.IsAuthenticated)
                j.Close();
        }

        public void SendMessage(string msg)
        {
            string[] users = GetSettingValue("JABBERSENDTO").Split(',');

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
                    WriteToLog(Urgency.WARNING, "Invalid Username or Password.");
            }
        }

        private void jabberClient1_OnAuthenticate(object sender)
        {
            if (!shuttingdown)
                WriteToLog(Urgency.INFO, "Connected using " + j.User + ".");
            j.Presence(jabber.protocol.client.PresenceType.available, "I am a " + zvsEntityControl.zvsNameAndVersion + " server.", ":chat", 0);
            isActive = true;
        }

        private void jabberClient1_OnError(object sender, Exception ex)
        {
            if (!shuttingdown)
                WriteToLog(Urgency.ERROR, ex.Message);
        }

        private void jabberClient1_OnDisconnect(object sender)
        {
            WriteToLog(Urgency.INFO, "Disconnected.");             
        }

        private void jabberClient1_OnMessage(object sender, jabber.protocol.client.Message msg)
        {
            bool verbose = false;
            bool.TryParse(GetSettingValue("JABBERVERBOSE"), out verbose);

            if (verbose && !shuttingdown)
                WriteToLog(Urgency.INFO, "[" + msg.From.User + "] says : " + msg.Body + "\n");
        }

        private void j_OnWriteText(object sender, string txt)
        {
            bool verbose = false;
            bool.TryParse(GetSettingValue("JABBERVERBOSE"), out verbose);

            if (txt == " ") return;
            if (verbose && !shuttingdown)
                WriteToLog(Urgency.INFO, "SENT: " + txt);
        }

        private void j_OnReadText(object sender, string txt)
        {
            bool verbose = false;
            bool.TryParse(GetSettingValue("JABBERVERBOSE"), out verbose);

            if (txt == " ") return;  // ignore keep-alive spaces
            if (verbose && !shuttingdown)
                WriteToLog(Urgency.INFO, "RECV: " + txt);
        }
        
    }
    
}

