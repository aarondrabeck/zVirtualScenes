using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Growl.Connector;
using System.Drawing;

namespace zVirtualScenesApplication
{
    class GrowlInterface
    {
        private static string LOG_INTERFACE = "GROWL";
        public formzVirtualScenes formzVirtualScenesMain;
        public const string NOTIFY_DEVICE_LEVEL_CHANGE = "DEVICE_LEVEL_CHANGE";
        public const string NOTIFY_TEMP_ALERT = "TEMP_ALERT";
        public GrowlConnector GrowlConnector = new GrowlConnector();

        public void RegisterGrowl()
        {
            try
            {
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formzVirtualScenes));
                Growl.Connector.Application application = new Growl.Connector.Application("zVirtualScenes");
                string exePath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
                application.Icon = new Bitmap(System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceStream("zVirtualScenesApplication.Resources.zvirtualscenes57.png"));

                NotificationType DeviceLevelChange = new NotificationType(NOTIFY_DEVICE_LEVEL_CHANGE, "Device Level Changed");
                DeviceLevelChange.Icon = new Bitmap(System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceStream("zVirtualScenesApplication.Resources.Broadcast48.png"));

                NotificationType Temp = new NotificationType(NOTIFY_TEMP_ALERT, "Urgent Temperture Alert");
                Temp.Icon = new Bitmap(System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceStream("zVirtualScenesApplication.Resources.temperature48.png"));

                GrowlConnector.Register(application, new NotificationType[] { DeviceLevelChange, Temp });
                formzVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "Registered Growl Interface.", LOG_INTERFACE);
            }
            catch (Exception ex)
            {
                formzVirtualScenesMain.AddLogEntry(UrgencyLevel.ERROR, "Error registering Growl. " + ex.Message, LOG_INTERFACE);
            }
        }

        public void Notify(string notificationName, string id, string title, string text)
        {
            Notification notification = new Notification("zVirtualScenes", notificationName, id, title, text);
            GrowlConnector.Notify(notification);
        }
    }
}
