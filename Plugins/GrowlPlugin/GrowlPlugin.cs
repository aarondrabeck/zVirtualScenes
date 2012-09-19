using System.Net.Sockets;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System;
using System.Net;
using System.Threading;
using System.Text;
using System.Data;
using Growl.Connector;
using System.Drawing;
using System.Linq;
using zvs.Processor;
using zvs.Entities;


namespace GrowlPlugin
{
    [Export(typeof(zvsPlugin))]
    public class GrowlPlugin : zvsPlugin
    {
        public GrowlPlugin()
            : base("GROWL",
               "Growl Plug-in",
                "This plug-in will send notifications to Growl."
                ) { }

        public const string NOTIFY_DEVICE_VALUE_CHANGE = "DEVICE_VALUE_CHANGE";
        public GrowlConnector GrowlConnector = new GrowlConnector();
        zvs.Processor.Logging.ILog log = zvs.Processor.Logging.LogManager.GetLogger<GrowlPlugin>();
        public override void Initialize()
        {
            using (zvsContext context = new zvsContext())
            {
                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "NOTIFICATIONS",
                    Name = "Notifications to send",
                    Value = "DIMMER:Basic, THERMOSTAT:Temperature, SWITCH:Basic, THERMOSTAT:Operating State",
                    ValueType = DataType.STRING,
                    Description = "Include all values you would like announced. Comma Separated."
                }, context);
            }
        }

        protected override void StartPlugin()
        {

            log.Info(this.Name + " started");
            DeviceValue.DeviceValueDataChangedEvent +=DeviceValue_DeviceValueDataChangedEvent;
            RegisterGrowl(); 

            IsReady = true;
        }

        void DeviceValue_DeviceValueDataChangedEvent(object sender, DeviceValue.ValueDataChangedEventArgs args)
        {
            if (IsReady)
            {
                using (zvsContext context = new zvsContext())
                {
                    DeviceValue dv = context.DeviceValues.FirstOrDefault(v => v.DeviceValueId == args.DeviceValueId);
                    if (dv != null)
                    {

                        string[] deviceTypeValuespairs = GetSettingValue("NOTIFICATIONS", context).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string deviceTypeValuespair in deviceTypeValuespairs)
                        {
                            string thisEvent = dv.Device.Type.Name + ":" + dv.Name;

                            if (thisEvent.Equals(deviceTypeValuespair.Trim()))
                            {
                                Notification notification = new Notification("zVirtualScenes", NOTIFY_DEVICE_VALUE_CHANGE, "0", dv.Device.Name + " " + dv.Name, "Changed to " + args.newValue + " from " + args.oldValue + ".");
                                GrowlConnector.Notify(notification);
                            }
                        }
                    }
                }
            }
        }     

        protected override void StopPlugin()
        {
            log.InfoFormat("{0}  stopped", this.Name);
            DeviceValue.DeviceValueDataChangedEvent -= DeviceValue_DeviceValueDataChangedEvent;
            IsReady = false;
        }

        protected override void SettingChanged(string settingUniqueIdentifier, string settingValue) { }

        public override void ProcessDeviceCommand(zvs.Entities.QueuedDeviceCommand cmd) { }

        public override void ProcessDeviceTypeCommand(zvs.Entities.QueuedDeviceTypeCommand cmd) { }

        public override void Repoll(zvs.Entities.Device device) { }

        public override void ActivateGroup(int groupID) { }

        public override void DeactivateGroup(int groupID) { }

        public void RegisterGrowl()
        {
            try
            {
                //string[] resourcenames = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();
                //foreach (string rname in resourcenames)
                //{
                //    API.log.Info(rname);
                //}

                Growl.Connector.Application application = new Growl.Connector.Application("zVirtualScenes");
                string exePath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
                application.Icon = new Bitmap(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("GrowlPlugin.zvirtualscenes57.png"));

                NotificationType DeviceValueChange = new NotificationType(NOTIFY_DEVICE_VALUE_CHANGE, "Device Value Changed");
                DeviceValueChange.Icon = new Bitmap(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("GrowlPlugin.Broadcast48.png"));

               
                GrowlConnector.Register(application, new NotificationType[] { DeviceValueChange });
                log.Info("Registered Growl interface.");
            }
            catch (Exception ex)
            {
                log.Error("Error registering Growl.", ex);
            }
        }
    }
    
}

