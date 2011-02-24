using System;
using System.Threading;
using jabber.client; 

namespace zVirtualScenesApplication
{
    class JabberInterface
    {
        private formzVirtualScenes zVirtualScenesMain;
        JabberClient j;

        public JabberInterface(formzVirtualScenes zvs)
        {
            zVirtualScenesMain = zvs;
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
                zVirtualScenesMain.LogThis(1, "JABBER: Invalid Username or Password.");
            }
        }

        private void jabberClient1_OnAuthenticate(object sender)
        {
            zVirtualScenesMain.LogThis(1, "JABBER: Connected");
            j.Presence(jabber.protocol.client.PresenceType.available, "I am a " + zVirtualScenesMain.ProgramName + " server.", ":chat", 0);
        }

        void jabberClient1_OnError(object sender, Exception ex)
        {
            zVirtualScenesMain.LogThis(1, "JABBER: Error connecting. - " + ex.Message);
        }

        void jabberClient1_OnDisconnect(object sender)
        {
            zVirtualScenesMain.LogThis(1, "JABBER: Disconnected");
        }

        private void jabberClient1_OnMessage(object sender, jabber.protocol.client.Message msg)
        {
            if (zVirtualScenesMain.zVScenesSettings.JabberVerbose)            
                zVirtualScenesMain.LogThis(1, "JABBER: " + msg.From.User + " Says : " + msg.Body + "\n");
        }

        private void j_OnWriteText(object sender, string txt)
        {
            if (txt == " ") return;
            if (zVirtualScenesMain.zVScenesSettings.JabberVerbose)  
                zVirtualScenesMain.LogThis(1, "JABBER SEND: " + txt);
        }

        private void j_OnReadText(object sender, string txt)
        {
            if (txt == " ") return;  // ignore keep-alive spaces
            if (zVirtualScenesMain.zVScenesSettings.JabberVerbose)            
                zVirtualScenesMain.LogThis(1, "JABBER RECV: " + txt);
        }
    }
}
