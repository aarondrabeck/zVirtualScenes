using System.Net.Sockets;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System;
using System.Net;
using System.Threading;
using System.Text;
using System.Data;
using System.Timers;
using System.ComponentModel;
using System.Linq;
using zVirtualScenes;
using zVirtualScenesModel;

namespace NOAAPlugin
{
    [Export(typeof(Plugin))]
    public class NOAAPlugin : Plugin
    {
        public volatile bool isActive;
        private System.Timers.Timer timerNOAA = new System.Timers.Timer();
        private Double Lat = 30.6772222222222;
        private Double Log = -100.061666666667;

        public NOAAPlugin()
            : base("NOAA",
               "NOAA Plugin",
                "This plug-in will add Sunrise and Sunset control to zVirtualScenes."
                ) { }

        public override void Initialize()
        {
            using (zvsLocalDBEntities context = new zvsLocalDBEntities())
            {
                DefineOrUpdateSetting(new plugin_settings
                {
                    name = "LAT",
                    friendly_name = "Latitude",
                    value = "37.6772222222222",
                    value_data_type = (int)Data_Types.DECIMAL,
                    description = "Your Latitude in Decimal Notation. ex. 37.6772222222222"
                }, context);

                DefineOrUpdateSetting( new plugin_settings
                {
                    name = "LOG",
                    friendly_name = "Longitude",
                    value = "-113.061666666667",
                    value_data_type = (int)Data_Types.DECIMAL,
                    description = "Your Longitude in Decimal Notation. ex. -113.061666666667"
                }, context);

                scene_property.AddOrEdit(new scene_property
                {
                    name = "ACTIVATE_SUNRISE",
                    friendly_name = "Activate at Sunrise",
                    description = "Activate this scene at sunrise.",
                    defualt_value = "false",
                    value_data_type = (int)Data_Types.BOOL
                },context);

                scene_property.AddOrEdit( new scene_property
                {
                    name = "ACTIVATE_SUNSET",
                    friendly_name = "Activate at Sunset",
                    description = "Activate this scene at sunset.",
                    defualt_value = "false",
                    value_data_type = (int)Data_Types.BOOL
                },context);
            }
        }

        protected override void StartPlugin()
        {
            using (zvsLocalDBEntities context = new zvsLocalDBEntities())
            {
                LoadLatint(context);
            }

            DateTime date = DateTime.Today;
            bool isSunrise = false;
            bool isSunset = false;
            DateTime sunrise = DateTime.Now;
            DateTime sunset = DateTime.Now;

            SunTimes.Instance.CalculateSunRiseSetTimes(Lat, Log, date, ref sunrise, ref sunset, ref isSunrise, ref isSunset);

            WriteToLog(Urgency.INFO, this.FriendlyName + " plugin started." + "(Today's Sunrise: " + sunrise.ToString("T") + ", Sunset: " + sunset.ToString("T") + ")");

            timerNOAA.Interval = 60000;
            timerNOAA.Elapsed += new ElapsedEventHandler(timerNOAA_Elapsed);
            timerNOAA.Enabled = true;

            IsReady = true;
        }

        protected override void StopPlugin()
        {
            timerNOAA.Elapsed -= new ElapsedEventHandler(timerNOAA_Elapsed);
            timerNOAA.Enabled = false;

            WriteToLog(Urgency.INFO, this.FriendlyName + " plugin ended.");

            IsReady = false;
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

        #region NOAA

        public bool isDark()
        {
            using (zvsLocalDBEntities context = new zvsLocalDBEntities())
            {
                LoadLatint(context);
            }

            DateTime date = DateTime.Today;
            bool isSunrise = false;
            bool isSunset = false;
            DateTime sunrise = DateTime.Now;
            DateTime sunset = DateTime.Now;

            SunTimes.Instance.CalculateSunRiseSetTimes(Lat, Log, date, ref sunrise, ref sunset, ref isSunrise, ref isSunset);

            if (DateTime.Now.TimeOfDay < sunrise.TimeOfDay || DateTime.Now.TimeOfDay > sunset.TimeOfDay)
                return true;

            return false;
        }

        private void LoadLatint(zvsLocalDBEntities context)
        {
            Lat = 30.6772222222222;
            Double.TryParse(GetSettingValue("LAT", context), out Lat);

            Log = -100.061666666667;
            Double.TryParse(GetSettingValue("LOG", context), out Log);
        }

        void timerNOAA_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                using (zvsLocalDBEntities context = new zvsLocalDBEntities())
                {
                    LoadLatint(context);

                    DateTime date = DateTime.Today;
                    bool isSunrise = false;
                    bool isSunset = false;
                    DateTime sunrise = DateTime.Now;
                    DateTime sunset = DateTime.Now;

                    SunTimes.Instance.CalculateSunRiseSetTimes(Lat, Log, date, ref sunrise, ref sunset, ref isSunrise, ref isSunset);

                    Double MinsBetweenTimeSunrise = (sunrise.TimeOfDay - DateTime.Now.TimeOfDay).TotalMinutes;
                    if (MinsBetweenTimeSunrise < 1 && MinsBetweenTimeSunrise > 0)
                    {
                        WriteToLog(Urgency.INFO, "It is now sunrise. Activating sunrise scenes.");
                        foreach (scene scene in context.scenes)
                        {
                            string value = scene_property_value.GetPropertyValue(context, scene.id, "ACTIVATE_SUNRISE");
                            bool activate = false;
                            bool.TryParse(value, out activate);

                            if (activate)
                            {
                                SceneRunner sr = new SceneRunner();
                                SceneRunner.onSceneRunEventHandler startHandler = null;
                                startHandler = (s, args) =>
                                {
                                    if (args.SceneRunnerGUID == sr.SceneRunnerGUID)
                                    {
                                        SceneRunner.onSceneRunBegin -= startHandler;
                                        WriteToLog(Urgency.INFO, "Sunrise: " + args.Details);
                                        
                                        #region LISTEN FOR ENDING
                                        SceneRunner.onSceneRunEventHandler handler = null;
                                        handler = (se, end_args) =>
                                        {
                                            if (end_args.SceneRunnerGUID == sr.SceneRunnerGUID)
                                            {
                                                SceneRunner.onSceneRunComplete -= handler;
                                                WriteToLog(Urgency.INFO, "Sunrise: " + end_args.Details);                                                
                                            }
                                        };
                                        SceneRunner.onSceneRunComplete += handler;
                                        #endregion
                                    }
                                };
                                SceneRunner.onSceneRunBegin += startHandler;
                                sr.RunScene(scene.id);                               
                            }

                        }
                    }

                    Double MinsBetweenTimeSunset = (sunset.TimeOfDay - DateTime.Now.TimeOfDay).TotalMinutes;
                    if (MinsBetweenTimeSunset < 1 && MinsBetweenTimeSunset > 0)
                    {
                        WriteToLog(Urgency.INFO, "It is now sunset. Activating sunrise scenes.");
                        foreach (scene scene in context.scenes)
                        {
                            string value = scene_property_value.GetPropertyValue(context, scene.id, "ACTIVATE_SUNSET");
                            bool activate = false;
                            bool.TryParse(value, out activate);

                            if (activate)
                            {
                                SceneRunner sr = new SceneRunner();
                                SceneRunner.onSceneRunEventHandler startHandler = null;
                                startHandler = (s, args) =>
                                {
                                    if (args.SceneRunnerGUID == sr.SceneRunnerGUID)
                                    {
                                        SceneRunner.onSceneRunBegin -= startHandler;
                                        WriteToLog(Urgency.INFO, "Sunset: " + args.Details);

                                        #region LISTEN FOR ENDING
                                        SceneRunner.onSceneRunEventHandler handler = null;
                                        handler = (se, end_args) =>
                                        {
                                            if (end_args.SceneRunnerGUID == sr.SceneRunnerGUID)
                                            {
                                                SceneRunner.onSceneRunComplete -= handler;
                                                WriteToLog(Urgency.INFO, "Sunset: " + end_args.Details);
                                            }
                                        };
                                        SceneRunner.onSceneRunComplete += handler;
                                        #endregion
                                    }
                                };
                                SceneRunner.onSceneRunBegin += startHandler;
                                sr.RunScene(scene.id);  
                            }
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

