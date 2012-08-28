using System.Net.Sockets;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System;
using System.Net;
using System.Threading;
using System.Text;
using System.Data;
using jabber.client;
using zvs.Processor;
using zvs.Entities;
using System.Linq;

namespace JabberPlugin
{
    [Export(typeof(zvsPlugin))]
    public class JabberPlugin : zvsPlugin
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
            using (zvsContext Context = new zvsContext())
            {
                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "JABBERSERVER",
                    Name = "Jabber Server",
                    Value = "talk.google.com",
                    ValueType = DataType.STRING,
                    Description = "The Jabber server to connect to (ex. talk.google.com)."
                }, Context);

                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "JABBERUSER",
                    Name = "Jabber Username",
                    Value = "user",
                    ValueType = DataType.STRING,
                    Description = "The username of the jabber user."
                }, Context);

                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "JABBERPASSWORD",
                    Name = "Jabber Password",
                    Value = "passw0rd",
                    ValueType = DataType.STRING,
                    Description = "The password of the jabber user."
                }, Context);

                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "JABBERSENDTO",
                    Name = "Send notifications to",
                    Value = "user@gmail.com",
                    ValueType = DataType.STRING,
                    Description = "Jabber users that will receive notifications. (comma seperated)"
                }, Context);

                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "JABBERVERBOSE",
                    Name = "Verbose Logging",
                    Value = "false",
                    ValueType = DataType.BOOL,
                    Description = "(Writes all server client communication to the log for debugging.)"
                }, Context);

                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "JABBERNOTIFICATIONS",
                    Name = "Notifications to send",
                    Value = "DIMMER:Basic, THERMOSTAT:Temperature, SWITCH:Basic, THERMOSTAT:Operating State",
                    ValueType = DataType.STRING,
                    Description = "Include all values you would like announced. Comma Seperated."
                }, Context);
            }
        }
               
        protected override void StartPlugin()
        {
            shuttingdown = false;
            WriteToLog(Urgency.INFO, this.Name + " plugin started.");

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
                using (zvsContext Context = new zvsContext())
                {

                    j.User = GetSettingValue("JABBERUSER", Context);
                    j.Server = GetSettingValue("JABBERSERVER", Context);
                    j.Password = GetSettingValue("JABBERPASSWORD", Context);
                }
                j.AutoRoster = false;
                j.AutoPresence = false;
                j.AutoReconnect = 50;
                j.Connect();

            }

            DeviceValue.DeviceValueDataChangedEvent += new DeviceValue.ValueDataChangedEventHandler(device_values_DeviceValueDataChangedEvent);
                        
            IsReady = true;
            //return true;
        }

        bool j_OnInvalidCertificate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            throw new NotImplementedException();
        }               

        protected override void StopPlugin()
        {
            WriteToLog(Urgency.INFO, this.Name + " plugin ended.");
            Disconnect();

            //supresses messages sent to log while program is closing
            shuttingdown = true;

            //Wait to shutdown
            System.Threading.Thread.Sleep(500);
            isActive = false;
            j.Dispose();

            DeviceValue.DeviceValueDataChangedEvent -= new DeviceValue.ValueDataChangedEventHandler(device_values_DeviceValueDataChangedEvent);
            
            IsReady = false;
            //return true;
        }


        /// <summary>
        /// TODO: Needs to actually work.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void device_values_DeviceValueDataChangedEvent(object sender, DeviceValue.ValueDataChangedEventArgs args)
        {
            //using (zvsContext Context = new zvsContext())
            //{
            //    //IQueryable<Device> devices =  args.DeviceValueId
            //    //if (devices != null)
            //    //{
            //    //    foreach (Device d in devices)
            //    //    {
            //    string[] objTypeValuespairs = GetSettingValue("JABBERNOTIFICATIONS", Context).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            //    foreach (string objTypeValuespair in objTypeValuespairs)
            //    {
            //        string thisEvent = args.DeviceValueId + ":" + args.newValue;

            //        if (thisEvent.Equals(objTypeValuespair.Trim()))
            //            SendMessage(args.DeviceValueId + " changed to " + d.Values + ".");
            //    }
            //    //}
            //    //}
            //}
        }

        protected override void SettingChanged(string settingName, string settingValue)
        {
        }
        public override void ProcessDeviceCommand(QueuedDeviceCommand cmd)
        {
            
        }
        public override void ProcessDeviceTypeCommand(QueuedDeviceTypeCommand cmd)
        {
        }
        public override void Repoll(Device device)
        {
            
        }
        public override void ActivateGroup(int groupID)
        {
        }
        public override void DeactivateGroup(int groupID)
        {
        }           

        public void Disconnect()
        {
            if(j.IsAuthenticated)
                j.Close();
        }

        public void SendMessage(string msg)
        {
            using (zvsContext Context = new zvsContext())
            {
                string[] users = GetSettingValue("JABBERSENDTO", Context).Split(',');

                if (msg != null)
                {
                    foreach (string user in users)
                        j.Message(user, msg);
                }
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
            j.Presence(jabber.protocol.client.PresenceType.available, "I am a " + Utils.ApplicationNameAndVersion + " server.", ":chat", 0);
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
            using (zvsContext Context = new zvsContext())
            {
                bool verbose = false;
                bool.TryParse(GetSettingValue("JABBERVERBOSE", Context), out verbose);

                if (verbose && !shuttingdown)
                    WriteToLog(Urgency.INFO, "[" + msg.From.User + "] says : " + msg.Body + "\n");
            }
        }

        private void j_OnWriteText(object sender, string txt)
        {
            using (zvsContext Context = new zvsContext())
            {
                bool verbose = false;
                bool.TryParse(GetSettingValue("JABBERVERBOSE", Context), out verbose);

                if (txt == " ") return;
                if (verbose && !shuttingdown)
                    WriteToLog(Urgency.INFO, "SENT: " + txt);
            }
        }

        private void j_OnReadText(object sender, string txt)
        {
            using (zvsContext Context = new zvsContext())
            {
                bool verbose = false;
                bool.TryParse(GetSettingValue("JABBERVERBOSE", Context), out verbose);

                if (txt == " ") return;  // ignore keep-alive spaces
                if (verbose && !shuttingdown)
                    WriteToLog(Urgency.INFO, "RECV: " + txt);

            }
        }
        
    }
    
}

