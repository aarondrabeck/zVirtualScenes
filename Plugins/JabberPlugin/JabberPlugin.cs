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
using System.ComponentModel;

namespace JabberPlugin
{
    [Export(typeof(zvsPlugin))]
    public class JabberPlugin : zvsPlugin
    {
        JabberClient j;
        public volatile bool isActive;
        private string UserName = string.Empty;
        private string Password = string.Empty;
        private string Server = string.Empty;
        private bool Verbose = false;
        private List<string> SendToList = new List<string>();
        //private bool UseSSL = true;
        private int Port = 5222;
        zvs.Processor.Logging.ILog log = zvs.Processor.Logging.LogManager.GetLogger<JabberPlugin>();
        public JabberPlugin()
            : base("JABBER",
               "Jabber/Gtalk Plug-in",
                "This plug-in will send customizable notifications to one or more jabber / Google Talk users."
                ) { }

        public override void Initialize()
        {
            using (zvsContext Context = new zvsContext())
            {
                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "JABBERSERVER",
                    Name = "Jabber Server",
                    Value = "talk.l.google.com",
                    ValueType = DataType.STRING,
                    Description = "The Jabber server to connect to (ex. talk.l.google.com)"
                }, Context);

                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "JABBERPORT",
                    Name = "Jabber Port",
                    Value = "5222",
                    ValueType = DataType.INTEGER,
                    Description = "The port for the Jabber server"
                }, Context);

                //DefineOrUpdateSetting(new PluginSetting
                //{
                //    UniqueIdentifier = "JABBERSSL",
                //    Name = "User SSL",
                //    Value = "false",
                //    ValueType = DataType.BOOL,
                //    Description = "Toggle the use of SSL when connection to Jabber"
                //}, Context);

                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "JABBERUSER",
                    Name = "Jabber Username",
                    Value = "user",
                    ValueType = DataType.STRING,
                    Description = "The username of the jabber user"
                }, Context);

                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "JABBERPASSWORD",
                    Name = "Jabber Password",
                    Value = "passw0rd",
                    ValueType = DataType.STRING,
                    Description = "The password of the jabber user"
                }, Context);

               
                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "JABBERSENDTO",
                    Name = "Send notifications to",
                    Value = "user@gmail.com",
                    ValueType = DataType.STRING,
                    Description = "Jabber users that will receive notifications. (comma separated)"
                }, Context);

                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "JABBERVERBOSE",
                    Name = "Verbose Logging",
                    Value = "false",
                    ValueType = DataType.BOOL,
                    Description = "Writes all server client communication to the log for debugging"
                }, Context);

                                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "JABBERNOTIFICATIONS",
                    Name = "Notifications to send",
                    Value = "DIMMER:Basic, THERMOSTAT:Temperature, SWITCH:Basic, THERMOSTAT:Operating State",
                    ValueType = DataType.STRING,
                    Description = "Include all values you would like announced. (comma separated)"
                }, Context);
            }

            j = new JabberClient();
            j.OnMessage += new MessageHandler(jabberClient1_OnMessage);
            j.OnDisconnect += new bedrock.ObjectHandler(jabberClient1_OnDisconnect);
            j.OnError += new bedrock.ExceptionHandler(jabberClient1_OnError);
            j.OnAuthError += new jabber.protocol.ProtocolHandler(jabberClient1_OnAuthError);
            j.OnAuthenticate += new bedrock.ObjectHandler(jabberClient1_OnAuthenticate);
            j.OnReadText += new bedrock.TextHandler(j_OnReadText);
            j.OnWriteText += new bedrock.TextHandler(j_OnWriteText);
            j.OnInvalidCertificate += new System.Net.Security.RemoteCertificateValidationCallback(j_OnInvalidCertificate);
            j.AutoRoster = false;
            j.AutoPresence = false;

            using (zvsContext Context = new zvsContext())
            {
                UserName = GetSettingValue("JABBERUSER", Context);
                Server = GetSettingValue("JABBERSERVER", Context);
                Password = GetSettingValue("JABBERPASSWORD", Context);
                bool.TryParse(GetSettingValue("JABBERVERBOSE", Context), out Verbose);
                SendToList = new List<string>(GetSettingValue("JABBERSENDTO", Context).Split(','));
                int.TryParse(GetSettingValue("JABBERPORT", Context), out Port);
                //bool.TryParse(GetSettingValue("JABBERSSL", Context), out UseSSL);
            }

        }

        protected override void SettingChanged(string settingUniqueIdentifier, string settingValue)
        {
            switch (settingUniqueIdentifier)
            {
                case "JABBERUSER":
                    {
                        if (Enabled)
                            Disconnect();

                        UserName = settingValue;

                        if (Enabled)
                            Connect();

                        break;
                    }
                case "JABBERSERVER":
                    {
                        if (Enabled)
                            Disconnect();

                        Server = settingValue;

                        if (Enabled)
                            Connect();

                        break;
                    }
                case "JABBERPASSWORD":
                    {
                        if (Enabled)
                            Disconnect();

                        Password = settingValue;

                        if (Enabled)
                            Connect();

                        break;
                    }
                case "JABBERVERBOSE":
                    {
                        bool.TryParse(settingValue, out Verbose);
                        break;
                    }
                case "JABBERSENDTO":
                    {
                        SendToList = new List<string>(settingValue.Split(','));
                        break;
                    }
                case "JABBERPORT":
                    {
                        if (Enabled)
                            Disconnect();

                        int.TryParse(settingValue, out Port);

                        if (Enabled)
                            Connect();
                        break;
                    }
                //case "JABBERSSL":
                //    {
                //        if (Enabled)
                //            Disconnect();

                //        bool.TryParse(settingValue, out UseSSL);

                //        if (Enabled)
                //            Connect();
                //        break;
                //    }

            }
        }

        protected override void StartPlugin()
        {
            DeviceValue.DeviceValueDataChangedEvent += new DeviceValue.ValueDataChangedEventHandler(device_values_DeviceValueDataChangedEvent);
            Connect();
            IsReady = true;
            log.InfoFormat("{0} plug-in started.", this.Name);
        }

        protected override void StopPlugin()
        {
            DeviceValue.DeviceValueDataChangedEvent -= new DeviceValue.ValueDataChangedEventHandler(device_values_DeviceValueDataChangedEvent);
            Disconnect();
            IsReady = false;
            log.InfoFormat("{0} plug-in stopped.", this.Name);
        }

        private void Connect()
        {
            try
            {
                if (j.IsAuthenticated)
                    Disconnect();

                j.User = UserName;
                j.Server = Server;
                j.Password = Password;
             //   j.SSL = UseSSL;
                j.Port = Port;
                j.Connect();
            }
            catch (Exception ex)
            {
                log.Fatal(ex);
            }
        }

        private void Disconnect()
        {
            j.Close();
        }

        private void device_values_DeviceValueDataChangedEvent(object sender, DeviceValue.ValueDataChangedEventArgs args)
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
                            string[] objTypeValuespairs = GetSettingValue("JABBERNOTIFICATIONS", context).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (string objTypeValuespair in objTypeValuespairs)
                            {
                                string thisEvent = dv.Device.Type.UniqueIdentifier + ":" + dv.Name;

                                if (thisEvent.Equals(objTypeValuespair.Trim()))
                                    SendMessage(dv.Device.Name + " " + dv.Name + " changed to " + dv.Value);
                            }

                        }
                    }
                };
                bw.RunWorkerAsync();
            }
        }

        public override void ProcessDeviceCommand(zvs.Entities.QueuedDeviceCommand cmd) { }

        public override void ProcessDeviceTypeCommand(zvs.Entities.QueuedDeviceTypeCommand cmd) { }

        public override void Repoll(zvs.Entities.Device device) { }

        public override void ActivateGroup(int groupID) { }

        public override void DeactivateGroup(int groupID) { }

        private bool j_OnInvalidCertificate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {            
            log.Warn("Invalid Certificate");
            return true;
        }

       

        private void SendMessage(string msg)
        {
            using (zvsContext Context = new zvsContext())
            {
                if (msg != null)
                {
                    foreach (string user in SendToList)
                        j.Message(user, msg);
                }
            }
        }

        private void jabberClient1_OnAuthError(object sender, System.Xml.XmlElement rp)
        {
            if (rp.Name == "failure")
                log.Warn("Invalid username or password.");
        }

        private void jabberClient1_OnAuthenticate(object sender)
        {
            log.Info("Jabber connected using " + j.User);
            j.Presence(jabber.protocol.client.PresenceType.available, "I am a " + Utils.ApplicationNameAndVersion + " server.", ":chat", 0);
            isActive = true;
        }

        private void jabberClient1_OnError(object sender, Exception ex)
        {
            log.Error(ex.Message);
        }

        private void jabberClient1_OnDisconnect(object sender)
        {
            log.Info("Jabber disconnected");
        }

        private void jabberClient1_OnMessage(object sender, jabber.protocol.client.Message msg)
        {
            if (Verbose)
                log.Info("[" + msg.From.User + "] says : " + msg.Body + "\n");
        }

        private void j_OnWriteText(object sender, string txt)
        {
            if (txt == " ") return;
            if (Verbose)
                log.Info("SENT: " + txt);
        }

        private void j_OnReadText(object sender, string txt)
        {
            if (txt == " ") return;  // ignore keep-alive spaces
            if (Verbose)
                log.Info("RECV: " + txt);
        }

    }

}

