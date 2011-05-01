using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zVirtualScenesApplication
{
    public class UserSettings
    {
        //GENERAL SETTINGS
        public enum interfaces
        {
            controlthinkUSB,
            OpenZwave
        }
        public interfaces zvs_interface { get; set; }
        public int PollingInterval { get; set; }
        public int LongLinesLimit { get; set; }

        //HTPP INTERFACE
        public bool zHTTPListenEnabled { get; set; }
        public int ZHTTPPort { get; set; }

        //LightSwitch INTERFACE
        public bool LightSwitchEnabled { get; set; }
        public bool LightSwitchVerbose { get; set; }
        public int LightSwitchPort { get; set; }
        public int LightSwitchMaxConnections { get; set; }
        public string LightSwitchPassword { get; set; }
        public bool LightSwitchSortDeviceList { get; set; }

        //Jabber INTERFACE
        public bool JabberEnanbled { get; set; }
        public bool JabberVerbose { get; set; }
        public string JabberUser { get; set; }
        public string JabberPassword { get; set; }        
        public string JabberServer { get; set; }
        public string JabberSendToUser { get; set; }

        //NOAA
        public bool EnableNOAA { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        //XML Socket INTERFACE
        public bool   XMLSocketEnabled         { get; set; }
        public bool   XMLSocketVerbose         { get; set; }
        public int    XMLSocketPort            { get; set; }
        public int    XMLSocketMaxConnections  { get; set; }
        public bool   XMLSocketAllowiViewer    { get; set; }
        public bool   XMLSocketAllowAndroid    { get; set; }
        public string XMLSocketAndroidPassword { get; set; }
        
        public UserSettings()
        {
            this.PollingInterval = 60;
            this.LongLinesLimit = 1000;

            //Defaults
            //HTTP
            this.zHTTPListenEnabled = true;
            this.ZHTTPPort = 8085;

            //LightSwitch
            this.LightSwitchEnabled = true;
            this.LightSwitchVerbose = false;
            this.LightSwitchPort = 1337;
            this.LightSwitchMaxConnections = 50;
            this.LightSwitchPassword = "1234";
            this.LightSwitchSortDeviceList = true;

            //Jabber
            this.JabberEnanbled = false; 
            this.JabberVerbose = false;             
            this.JabberServer = "gmail.com";
            this.JabberUser = "";
            this.JabberPassword = "";
            this.JabberSendToUser = "";

            //NOAA
            this.EnableNOAA = false;
            this.Longitude = -113.06166666666667;
            this.Latitude = 37.67722222222222;

            //XML socket
            this.XMLSocketEnabled = false;
            this.XMLSocketVerbose = false;
            this.XMLSocketPort = 1338;
            this.XMLSocketMaxConnections = 50;
            this.XMLSocketAllowiViewer = false;
            this.XMLSocketAllowAndroid = false;
            this.XMLSocketAndroidPassword = "changeme";
                        
        }

    }
}
