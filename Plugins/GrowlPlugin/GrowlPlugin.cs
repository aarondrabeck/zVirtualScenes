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
using zVirtualScenes;
using zVirtualScenesModel;

namespace GrowlPlugin
{
    [Export(typeof(Plugin))]
    public class GrowlPlugin : Plugin
    {
        public GrowlPlugin()
            : base("GROWL",
               "Growl Plugin",
                "This plug-in will send notifications to Growl."
                ) { }

        public const string NOTIFY_DEVICE_VALUE_CHANGE = "DEVICE_VALUE_CHANGE";
        public GrowlConnector GrowlConnector = new GrowlConnector();

        public override void Initialize()
        {
            using (zvsLocalDBEntities context = new zvsLocalDBEntities())
            {
                DefineOrUpdateSetting(new plugin_settings
                {
                    name = "NOTIFICATIONS",
                    friendly_name = "Notifications to send",
                    value = "DIMMER:Basic, THERMOSTAT:Temperature, SWITCH:Basic, THERMOSTAT:Operating State",
                    value_data_type = (int)Data_Types.STRING,
                    description = "Include all values you would like announced. Comma Seperated."
                }, context);
            }
        }

        protected override void StartPlugin()
        {

            WriteToLog(Urgency.INFO, this.Friendly_Name + " plugin started.");
            device_values.DeviceValueDataChangedEvent+=new device_values.ValueDataChangedEventHandler(device_values_DeviceValueDataChangedEvent); 
            RegisterGrowl(); 

            IsReady = true;
        }     

        protected override void StopPlugin()
        {
            WriteToLog(Urgency.INFO, this.Friendly_Name + " plugin ended.");
            device_values.DeviceValueDataChangedEvent -= new device_values.ValueDataChangedEventHandler(device_values_DeviceValueDataChangedEvent); 

            IsReady = false;
        }

       

        void device_values_DeviceValueDataChangedEvent(object sender, device_values.ValueDataChangedEventArgs args)
        {
            if (IsReady)
            {
                using (zvsLocalDBEntities context = new zvsLocalDBEntities())
                {
                    device_values dv = context.device_values.FirstOrDefault(v => v.id == args.device_value_id);
                    if (dv != null)
                    {

                        string[] deviceTypeValuespairs = GetSettingValue("NOTIFICATIONS", context).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string deviceTypeValuespair in deviceTypeValuespairs)
                        {
                            string thisEvent = dv.device.device_types.name + ":" + dv.label_name;

                            if (thisEvent.Equals(deviceTypeValuespair.Trim()))
                            {
                                Notification notification = new Notification("zVirtualScenes", NOTIFY_DEVICE_VALUE_CHANGE, "0", dv.device.friendly_name + " " + dv.label_name, "Changed to " + args.newValue + " from " + args.oldValue + ".");
                                GrowlConnector.Notify(notification);
                            }
                        }
                    }
                }
            }
        }

        protected override void SettingChanged(string settingName, string settingValue)
        {
        }
        public override bool ProcessDeviceCommand(device_command_que cmd)
        {
            return true;
        }
        public override bool ProcessDeviceTypeCommand(device_type_command_que cmd)
        {
            return true;
        }
        public override bool Repoll(device device)
        {
            return true;
        }
        public override bool ActivateGroup(int groupID)
        {
            return true;
        }
        public override bool DeactivateGroup(int groupID)
        {
            return true;
        }     

        public void RegisterGrowl()
        {
            try
            {
                //string[] resourcenames = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();
                //foreach (string rname in resourcenames)
                //{
                //    API.WriteToLog(Urgency.INFO, rname);
                //}

                Growl.Connector.Application application = new Growl.Connector.Application("zVirtualScenes");
                string exePath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
                application.Icon = new Bitmap(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("GrowlPlugin.zvirtualscenes57.png"));

                NotificationType DeviceValueChange = new NotificationType(NOTIFY_DEVICE_VALUE_CHANGE, "Device Value Changed");
                DeviceValueChange.Icon = new Bitmap(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("GrowlPlugin.Broadcast48.png"));

               
                GrowlConnector.Register(application, new NotificationType[] { DeviceValueChange });
                WriteToLog(Urgency.INFO, "Registered Growl interface.");
            }
            catch (Exception ex)
            {
                WriteToLog(Urgency.ERROR, "Error registering Growl. " + ex.Message);
            }
        }
    }
    
}

