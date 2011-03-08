using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zVirtualScenesApplication
{
    public class Settings
    {
        //GENERAL SETTINGS
        public int PollingInterval { get; set; }

        //HTPP INTERFACE
        public bool zHTTPListenEnabled { get; set; }
        public int ZHTTPPort { get; set; }

        //LightSwitch INTERFACE
        public bool LightSwitchEnabled { get; set; }
        public bool LightSwitchVerbose { get; set; }
        public int LightSwitchPort { get; set; }
        public int LightSwitchMaxConnections { get; set; }
        public string LightSwitchPassword { get; set; }
        public bool LightSwitchDisableAuth { get; set; }

        //Jabber INTERFACE
        public bool JabberEnanbled { get; set; }
        public bool JabberVerbose { get; set; }
        public string JabberUser { get; set; }
        public string JabberPassword { get; set; }        
        public string JabberServer { get; set; }
        public string JabberSendToUser { get; set; }

        //NOAA
        public bool EnableNOAA { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }

        public Settings()
        {
            this.PollingInterval = 10;

            //Defaults
            this.zHTTPListenEnabled = true;
            this.ZHTTPPort = 8085;
            this.LightSwitchEnabled = true;
            this.LightSwitchVerbose = false;
            this.LightSwitchPort = 1337;
            this.LightSwitchMaxConnections = 50;
            this.LightSwitchPassword = "1234";
            this.LightSwitchDisableAuth = false;

            //Jabber
            this.JabberEnanbled = false; 
            this.JabberVerbose = false;             
            this.JabberServer = "gmail.com";
            this.JabberUser = "";
            this.JabberPassword = "";
            this.JabberSendToUser = "";

            //NOAA
            this.EnableNOAA = false;
            this.Longitude = "113,3,42,W";
            this.Latitude = "37,40,38,N";
        }

    }
}
