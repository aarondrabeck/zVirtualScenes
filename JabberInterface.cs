using System;
using System.Threading;
using jabber.client; 

namespace zVirtualScenesApplication
{
    public class JabberInterface
    {
        private static string LOG_INTERFACE = "JABBER"; 
        public formzVirtualScenes zVirtualScenesMain;
        JabberClient j;
        public volatile bool isActive; 

        public JabberInterface()
        {
            j = new JabberClient();
            j.OnMessage += new MessageHandler(jabberClient1_OnMessage);
            j.OnDisconnect += new bedrock.ObjectHandler(jabberClient1_OnDisconnect);
            j.OnError += new bedrock.ExceptionHandler(jabberClient1_OnError);
            j.OnAuthError += new jabber.protocol.ProtocolHandler(jabberClient1_OnAuthError);
            j.OnAuthenticate += new bedrock.ObjectHandler(jabberClient1_OnAuthenticate);
            j.OnReadText += new bedrock.TextHandler(j_OnReadText);
            j.OnWriteText += new bedrock.TextHandler(j_OnWriteText);            
        }

        public void Connect()
        {
            if(!j.IsAuthenticated)
            {
                j.User = zVirtualScenesMain.zVScenesSettings.JabberUser;
                j.Server = zVirtualScenesMain.zVScenesSettings.JabberServer;
                j.Password = zVirtualScenesMain.zVScenesSettings.JabberPassword;
                j.AutoRoster = false;
                j.AutoPresence = false;
                j.AutoReconnect = 50;                
                j.Connect();
            }            
        }

        public void Disconnect()
        {
            if(j.IsAuthenticated)
                j.Close();
        }

        public void SendMessage(string msg)
        {
            string[] users = zVirtualScenesMain.zVScenesSettings.JabberSendToUser.Split(',');

            if (msg != null)
            {
                foreach(string user in users)                
                    j.Message(user, msg);
            }
         }

        void jabberClient1_OnAuthError(object sender, System.Xml.XmlElement rp)
        {
            if (rp.Name == "failure")
            {
                zVirtualScenesMain.AddLogEntry(UrgencyLevel.WARNING, "Invalid Username or Password.", LOG_INTERFACE);
            }
        }

        private void jabberClient1_OnAuthenticate(object sender)
        {
            zVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "Connected using " + zVirtualScenesMain.zVScenesSettings.JabberUser +".", LOG_INTERFACE);
            j.Presence(jabber.protocol.client.PresenceType.available, "I am a " + zVirtualScenesMain.ProgramName + " server.", ":chat", 0);
            isActive = true;
        }

        void jabberClient1_OnError(object sender, Exception ex)
        {
            zVirtualScenesMain.AddLogEntry(UrgencyLevel.ERROR, ex.Message, LOG_INTERFACE);
        }

        void jabberClient1_OnDisconnect(object sender)
        {
            zVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "Disconnected.", LOG_INTERFACE);
            isActive = false; 
        }

        private void jabberClient1_OnMessage(object sender, jabber.protocol.client.Message msg)
        {
            if (zVirtualScenesMain.zVScenesSettings.JabberVerbose)
                zVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "[" + msg.From.User +  "] says : " + msg.Body + "\n", LOG_INTERFACE);
        }

        private void j_OnWriteText(object sender, string txt)
        {
            if (txt == " ") return;
            if (zVirtualScenesMain.zVScenesSettings.JabberVerbose)
                zVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "SENT: " + txt, LOG_INTERFACE);
        }

        private void j_OnReadText(object sender, string txt)
        {
            if (txt == " ") return;  // ignore keep-alive spaces
            if (zVirtualScenesMain.zVScenesSettings.JabberVerbose)
                zVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "RECV: " + txt, LOG_INTERFACE);
        }
    }
}
