using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zVirtualScenesApplication
{
    public class CustomDeviceProperties
    {
        public uint HomeID { get; set; }
        public byte NodeID { get; set; }
        public string Name { get; set; }
        public bool SendJabberNotifications { get; set; }
        public int NotificationDetailLevel { get; set; }
        public int MinAlertTemp { get; set; }
        public int MaxAlertTemp { get; set; }        

        public CustomDeviceProperties()
        {
            this.HomeID = 0;
            this.NodeID = 0;
            this.Name = "Default Device";
            this.SendJabberNotifications = false;
            this.NotificationDetailLevel = 1;
            this.MinAlertTemp = 40;
            this.MaxAlertTemp = 90;
        }

        public string GlbUniqueID()
        {
            return this.HomeID.ToString() + this.NodeID.ToString();
        }
    }
}
