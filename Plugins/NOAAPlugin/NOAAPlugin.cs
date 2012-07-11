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
        private Double _Lat = 30.6772222222222;
        private Double _Long = -100.061666666667;
        private Double _SunriseDelay = 0;
        private Double _SunsetDelay = 0;

        private DateTime _date = DateTime.Today;
        private bool _isSunrise = false;
        private bool _isSunset = false;
        private DateTime _sunrise = DateTime.Now;
        private DateTime _sunset = DateTime.Now;

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

                DefineOrUpdateSetting(new plugin_settings
                {
                    name = "LOG",
                    friendly_name = "Longitude",
                    value = "-113.061666666667",
                    value_data_type = (int)Data_Types.DECIMAL,
                    description = "Your Longitude in Decimal Notation. ex. -113.061666666667"
                }, context);

                DefineOrUpdateSetting(new plugin_settings
                {
                    name = "DELAY_SUNRISE",
                    friendly_name = "Minutes to delay sunrise",
                    value = (0).ToString(),
                    value_data_type = (int)Data_Types.DECIMAL,
                    description = "The minutes to delay sunrise as a positive or negative number"
                }, context);

                DefineOrUpdateSetting(new plugin_settings
                {
                    name = "DELAY_SUNSET",
                    friendly_name = "Minutes to delay sunset",
                    value = (0).ToString(),
                    value_data_type = (int)Data_Types.DECIMAL,
                    description = "The minutes to delay sunset as a positive or negative number"
                }, context);

                scene_property.AddOrEdit(new scene_property
                {
                    name = "ACTIVATE_SUNRISE",
                    friendly_name = "Activate at Sunrise",
                    description = "Activates this scene at sunrise",
                    defualt_value = "false",
                    value_data_type = (int)Data_Types.BOOL
                }, context);

                scene_property.AddOrEdit(new scene_property
                {
                    name = "ACTIVATE_SUNSET",
                    friendly_name = "Activate at Sunset",
                    description = "Activates this scene at sunset",
                    defualt_value = "false",
                    value_data_type = (int)Data_Types.BOOL
                }, context);

                Double.TryParse(GetSettingValue("DELAY_SUNRISE", context), out _SunriseDelay);
                Double.TryParse(GetSettingValue("DELAY_SUNSET", context), out _SunsetDelay);
                Double.TryParse(GetSettingValue("LAT", context), out _Lat);
                Double.TryParse(GetSettingValue("LOG", context), out _Long);
                CalculateSunriseSet();
            }
        }

        protected override void StartPlugin()
        {
            WriteToLog(Urgency.INFO, string.Format("{0} started. Today's Sunrise: {1}, Today's Sunset: {2}", this.FriendlyName, _sunrise.ToString("T"), _sunset.ToString("T")));

            timerNOAA.Interval = 60000;
            timerNOAA.Elapsed += new ElapsedEventHandler(timerNOAA_Elapsed);
            timerNOAA.Enabled = true;

            IsReady = true;
        }

        protected override void StopPlugin()
        {
            timerNOAA.Elapsed -= new ElapsedEventHandler(timerNOAA_Elapsed);
            timerNOAA.Enabled = false;

            WriteToLog(Urgency.INFO, this.FriendlyName + " stopped");

            IsReady = false;
        }

        protected override void SettingChanged(string settingName, string settingValue)
        {
            if (settingName == "DELAY_SUNRISE")
            {
                Double.TryParse(settingValue, out _SunriseDelay);
            }
            else if (settingName == "DELAY_SUNSET")
            {
                Double.TryParse(settingValue, out _SunsetDelay);
            }
            else if (settingName == "LAT")
            {
                Double.TryParse(settingValue, out _Lat);
            }
            else if (settingName == "LOG")
            {
                Double.TryParse(settingValue, out _Long);
            }

            CalculateSunriseSet();
            WriteToLog(Urgency.INFO, string.Format("Lat/Long updated.  New Sunrise: {0}, New Sunset: {1}", _sunrise.ToString("T"), _sunset.ToString("T")));
        }

        public override void ProcessDeviceCommand(device_command_que cmd)
        {
        }
        public override void ProcessDeviceTypeCommand(device_type_command_que cmd)
        {
        }
        public override void Repoll(device device)
        {
        }
        public override void ActivateGroup(int groupID)
        {
        }
        public override void DeactivateGroup(int groupID)
        {
        }

        #region NOAA

        private void CalculateSunriseSet()
        {
            SunTimes.Instance.CalculateSunRiseSetTimes(_Lat, _Long, _date, ref _sunrise, ref _sunset, ref _isSunrise, ref _isSunset);

            //Add delays
            if (_sunrise != null)
                _sunrise = _sunrise.AddMinutes(_SunriseDelay);

            if (_sunset != null)
                _sunset = _sunset.AddMinutes(_SunsetDelay);
        }

        public bool isDark()
        {
            if (DateTime.Now.TimeOfDay < _sunrise.TimeOfDay || DateTime.Now.TimeOfDay > _sunset.TimeOfDay)
                return true;

            return false;
        }

        void timerNOAA_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                using (zvsLocalDBEntities context = new zvsLocalDBEntities())
                {
                    Double MinsBetweenTimeSunrise = (_sunrise.TimeOfDay - DateTime.Now.TimeOfDay).TotalMinutes;
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
                        CalculateSunriseSet();
                    }

                    Double MinsBetweenTimeSunset = (_sunset.TimeOfDay - DateTime.Now.TimeOfDay).TotalMinutes;
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
                        CalculateSunriseSet();
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

