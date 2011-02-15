using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zVirtualScenesApplication
{
    public class Settings
    {
        public string ZcomIP { get; set; }
        public int ZcomPort { get; set; }
        public bool zHTTPListenEnabled { get; set; }
        public int ZHTTPPort { get; set; }

        public Settings()
        {
            //Defaults
            this.ZcomIP = "127.0.0.1";
            this.ZcomPort = 8084;
            this.zHTTPListenEnabled = true;
            this.ZHTTPPort = 8085;
        }

    }
}
