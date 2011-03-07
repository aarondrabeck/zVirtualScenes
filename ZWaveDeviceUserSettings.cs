using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zVirtualScenesApplication
{
    public class ZWaveDeviceUserSettings
    {
        public uint HomeID { get; set; }
        public byte NodeID { get; set; }
        public string Name { get; set; }
        public bool SendJabberNotifications { get; set; }
        public int NotificationDetailLevel { get; set; }
        public int MinAlertTemp { get; set; }
        public int MaxAlertTemp { get; set; }
        public string GroupName { get; set; }
        public bool ShowInLightSwitchGUI { get; set; }
        public bool MomentaryOnMode { get; set; }
        public int MomentaryTimespan { get; set; }

        public ZWaveDeviceUserSettings()
        {
            this.HomeID = 0;
            this.NodeID = 0;
            this.Name = "Default Device";
            this.SendJabberNotifications = false;
            this.NotificationDetailLevel = 1;
            this.MinAlertTemp = 40;
            this.MaxAlertTemp = 90;
            this.GroupName = "<None>";
            this.ShowInLightSwitchGUI = true;
            this.MomentaryOnMode = false;
            this.MomentaryTimespan = 1;
        }

        public string GlbUniqueID()
        {
            return this.HomeID.ToString() + this.NodeID.ToString();
        }

        public static implicit operator ZWaveDeviceUserSettings(ZWaveDevice thisDevice)
        {
           ZWaveDeviceUserSettings newcDevice = new ZWaveDeviceUserSettings();
            //ONLY INCLUDE NOT ACTION PROPERTIES HERE
           newcDevice.HomeID = thisDevice.HomeID;
           newcDevice.NodeID = thisDevice.NodeID;
           newcDevice.Name = thisDevice.Name;
           newcDevice.GroupName = thisDevice.GroupName;
           newcDevice.SendJabberNotifications = thisDevice.SendJabberNotifications;
           newcDevice.MaxAlertTemp = thisDevice.MaxAlertTemp;
           newcDevice.MinAlertTemp = thisDevice.MinAlertTemp;
           newcDevice.NotificationDetailLevel = thisDevice.NotificationDetailLevel;
           newcDevice.ShowInLightSwitchGUI = thisDevice.ShowInLightSwitchGUI;
           newcDevice.MomentaryOnMode = thisDevice.MomentaryOnMode;
           newcDevice.MomentaryTimespan = thisDevice.MomentaryTimespan;
           return newcDevice;
        }
    }
}
