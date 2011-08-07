using System.Net.Sockets;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System;
using System.Net;
using System.Threading;
using System.Text;
using zVirtualScenesAPI;
using zVirtualScenesAPI.Structs;
using System.Data;
using zVirtualScenesAPI.Events;
using System.Timers;
using System.ComponentModel;
using zVirtualScenesApplication.Structs;
using System.Linq;

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
            : base("NOAA")
        {
            PluginName = "NOAA";
        }

        public override void Initialize()
        {
            API.InstallObjectType("NOAA", false);
            API.DefineSetting("Latitude", "37.6772222222222", ParamType.DECIMAL, "Your Latitude in Decimal Notation. ex. 37.6772222222222");
            API.DefineSetting("Longitude", "-113.061666666667", ParamType.DECIMAL, "Your Longitude in Decimal Notation. ex. -113.061666666667");

            API.Scenes.Properties.New("Activate at Sunrise", "Activate this scene at sunrise.", "false", ParamType.BOOL);
            API.Scenes.Properties.New("Activate at Sunset", "Activate this scene at sunset.", "false", ParamType.BOOL);

            //Remove plugin properties from old version
            API.RemovePluginSetting("Sunrise Scenes");
            API.RemovePluginSetting("Sunset Scenes");

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
           
            API.WriteToLog(Urgency.INFO, PluginName + " plugin started." + "(Today's Sunrise: " + sunrise.ToString("T") + ", Sunset: " + sunset.ToString("T") + ")");
                        
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

            API.WriteToLog(Urgency.INFO, PluginName + " plugin ended.");
                        
            IsReady = false;
            return true;
        }       

        protected override void SettingChanged(string settingName, string settingValue)
        {            
        }               

        public override void ProcessCommand(QuedCommand cmd)
        {
        }

        public override void Repoll(string id)
        {
        }

        public override void ActivateGroup(string GroupName)
        { 
        }

        public override void DeactivateGroup(string GroupName)
        { 
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
            Double.TryParse(API.GetSetting("Latitude"), out Lat);

            Long = -100.061666666667;
            Double.TryParse(API.GetSetting("Longitude"), out Long);
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
                    API.WriteToLog(Urgency.INFO, "It is now sunrise. Activating sunrise scenes.");

                    foreach (Scene scene in API.Scenes.GetScenes())
                    {
                        string propertyvalue = API.Scenes.Properties.GetScenePropertyValue(scene.id, "Activate at Sunrise");
                        bool activate = false;
                        bool.TryParse(propertyvalue, out activate);

                        if (activate)
                        {
                            string result = scene.RunScene();
                            API.WriteToLog(Urgency.INFO, "Sunrise: " + result);
                        }
                    }       
                }

                Double MinsBetweenTimeSunset = (sunset.TimeOfDay - DateTime.Now.TimeOfDay).TotalMinutes;
                if (MinsBetweenTimeSunset < 1 && MinsBetweenTimeSunset > 0)
                {
                    API.WriteToLog(Urgency.INFO, "It is now sunset. Activating sunrise scenes.");

                    foreach (Scene scene in API.Scenes.GetScenes())
                    {
                        string propertyvalue = API.Scenes.Properties.GetScenePropertyValue(scene.id, "Activate at Sunset");
                        bool activate = false;
                        bool.TryParse(propertyvalue, out activate);

                        if (activate)
                        {
                            string result = scene.RunScene();
                            API.WriteToLog(Urgency.INFO, "Sunset: " + result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                API.WriteToLog(Urgency.WARNING, "Error calulating Sunrise/Sunset. - " + ex.Message);
            }
        } 
        #endregion
        
    }
    
}

