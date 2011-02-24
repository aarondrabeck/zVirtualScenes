using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zVirtualScenesApplication
{
    public class Settings
    {
        public bool zHTTPListenEnabled { get; set; }
        public int ZHTTPPort { get; set; }

        //LightSwitch Settings
        public bool LightSwitchEnabled { get; set; }
        public bool LightSwitchVerbose { get; set; }
        public int LightSwitchPort { get; set; }
        public int LightSwitchMaxConnections { get; set; }
        public string LightSwitchPassword { get; set; }

        //Jabber
        public bool JabberEnanbled { get; set; }
        public bool JabberVerbose { get; set; }
        public string JabberUser { get; set; }
        public string JabberPassword { get; set; }        
        public string JabberServer { get; set; }
        public string JabberSendToUser { get; set; }

        public Settings()
        {
            //Defaults
            this.zHTTPListenEnabled = true;
            this.ZHTTPPort = 8085;
            this.LightSwitchEnabled = true;
            this.LightSwitchVerbose = false;
            this.LightSwitchPort = 1337;
            this.LightSwitchMaxConnections = 50;
            this.LightSwitchPassword = "1234";

            //Jabber
            this.JabberEnanbled = false; 
            this.JabberVerbose = false;             
            this.JabberServer = "gmail.com";
            this.JabberUser = "";
            this.JabberPassword = "";
            this.JabberSendToUser = "";
        }

    }
}
