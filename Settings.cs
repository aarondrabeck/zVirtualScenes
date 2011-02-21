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

        }

    }
}
