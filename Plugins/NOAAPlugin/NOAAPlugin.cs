using System.Net.Sockets;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System;
using System.Net;
using System.Threading;
using System.Text;
using zVirtualScenesAPI;
using System.Data;
using System.Timers;
using System.ComponentModel;
using System.Linq;
using zVirtualScenesCommon.Entity;
using zVirtualScenesCommon;

namespace NOAAPlugin
{
   [Export(typeof(Plugin))]
    public class NOAAPlugin : Plugin
    {        
        public volatile bool isActive;
        private System.Timers.Timer timerNOAA = new System.Timers.Timer();
        private Double Lat = 30.6772222222222;
        private Double Long = -100.061666666667;

        public NOAAPlugin()
            : base("NOAA",
               "NOAA Plugin",
                "This plug-in will add Sunrise and Sunset control to zVirtualScenes."
                ) { }

        public override void Initialize()
        {
            DefineOrUpdateSetting(new plugin_settings
            {
                name = "LAT",
                friendly_name = "Latitude",
                value = "37.6772222222222",
                value_data_type = (int)Data_Types.DECIMAL,
                description = "Your Latitude in Decimal Notation. ex. 37.6772222222222"
            });

            DefineOrUpdateSetting(new plugin_settings
            {
                name = "LONG",
                friendly_name = "Polling Interval",
                value = "-113.061666666667",
                value_data_type = (int)Data_Types.DECIMAL,
                description = "Your Longitude in Decimal Notation. ex. -113.061666666667"
            });

            scene_property.DefineOrUpdateProperty(new scene_property 
            {
                name = "ACTIVATE_SUNRISE",
                friendly_name = "Activate at Sunrise",
                description = "Activate this scene at sunrise.",
                defualt_value = "false",
                value_data_type = (int)Data_Types.BOOL
            });

            scene_property.DefineOrUpdateProperty(new scene_property
            {
                name = "ACTIVATE_SUNSET",
                friendly_name = "Activate at Sunset",
                description = "Activate this scene at sunset.",
                defualt_value = "false",
                value_data_type = (int)Data_Types.BOOL
            });
        } 

        protected override bool StartPlugin()
        {
            LoadLatLong();

            DateTime date = DateTime.Today;
            bool isSunrise = false;
            bool isSunset = false;
            DateTime sunrise = DateTime.Now;
            DateTime sunset = DateTime.Now;

            SunTimes.Instance.CalculateSunRiseSetTimes(Lat, Long, date, ref sunrise, ref sunset, ref isSunrise, ref isSunset);
           
            WriteToLog(Urgency.INFO, this.FriendlyName + " plugin started." + "(Today's Sunrise: " + sunrise.ToString("T") + ", Sunset: " + sunset.ToString("T") + ")");
                        
            timerNOAA.Interval = 60000;
            timerNOAA.Elapsed += new ElapsedEventHandler(timerNOAA_Elapsed);
            timerNOAA.Enabled = true;

            IsReady = true;
            return true;
        }

        protected override bool StopPlugin()
        {
            timerNOAA.Elapsed -= new ElapsedEventHandler(timerNOAA_Elapsed);
            timerNOAA.Enabled = false;

            WriteToLog(Urgency.INFO, this.FriendlyName + " plugin ended.");
                        
            IsReady = false;
            return true;
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
        public override bool ActivateGroup(long groupID)
        {
            return true;
        }
        public override bool DeactivateGroup(long groupID)
        {
            return true;
        }     

        #region NOAA

        public bool isDark()
        {
            LoadLatLong();

            DateTime date = DateTime.Today;
            bool isSunrise = false;
            bool isSunset = false;
            DateTime sunrise = DateTime.Now;
            DateTime sunset = DateTime.Now;

            SunTimes.Instance.CalculateSunRiseSetTimes(Lat, Long, date, ref sunrise, ref sunset, ref isSunrise, ref isSunset);

            if (DateTime.Now.TimeOfDay < sunrise.TimeOfDay || DateTime.Now.TimeOfDay > sunset.TimeOfDay)
                return true;

            return false;
        }

        private void LoadLatLong()
        {
            Lat = 30.6772222222222;
            Double.TryParse(GetSettingValue("LAT"), out Lat);

            Long = -100.061666666667;
            Double.TryParse(GetSettingValue("LONG"), out Long);
        }

        void timerNOAA_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                LoadLatLong();

                DateTime date = DateTime.Today;
                bool isSunrise = false;
                bool isSunset = false;
                DateTime sunrise = DateTime.Now;
                DateTime sunset = DateTime.Now;

                SunTimes.Instance.CalculateSunRiseSetTimes(Lat, Long, date, ref sunrise, ref sunset, ref isSunrise, ref isSunset);

                Double MinsBetweenTimeSunrise = (sunrise.TimeOfDay - DateTime.Now.TimeOfDay).TotalMinutes;
                if (MinsBetweenTimeSunrise < 1 && MinsBetweenTimeSunrise > 0)
                {
                    WriteToLog(Urgency.INFO, "It is now sunrise. Activating sunrise scenes.");

                    foreach (scene scene in zvsEntityControl.zvsContext.scenes)
                    {
                        string value = scene_property_value.GetPropertyValue(zvsEntityControl.zvsContext, scene.id, "ACTIVATE_SUNRISE");
                        bool activate = false;
                        bool.TryParse(value, out activate);

                        if (activate)
                        {
                            WriteToLog(Urgency.INFO, "Sunrise: " + scene.RunScene());
                        }
                    }       
                }

                Double MinsBetweenTimeSunset = (sunset.TimeOfDay - DateTime.Now.TimeOfDay).TotalMinutes;
                if (MinsBetweenTimeSunset < 1 && MinsBetweenTimeSunset > 0)
                {
                    WriteToLog(Urgency.INFO, "It is now sunset. Activating sunrise scenes.");

                    foreach (scene scene in zvsEntityControl.zvsContext.scenes)
                    {
                        string value = scene_property_value.GetPropertyValue(zvsEntityControl.zvsContext, scene.id, "ACTIVATE_SUNSET");
                        bool activate = false;
                        bool.TryParse(value, out activate);

                        if (activate)
                        {
                            WriteToLog(Urgency.INFO, "Sunset: " + scene.RunScene());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteToLog(Urgency.WARNING, "Error calulating Sunrise/Sunset. - " + ex.Message);
            }
        } 
        #endregion
        
    }
    
}

