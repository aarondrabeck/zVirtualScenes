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
using zvs.Processor;
using zvs.Entities;


namespace NOAAPlugin
{
    [Export(typeof(zvsPlugin))]
    public class NOAAPlugin : zvsPlugin
    {
        public DateTime Sunrise { get { return _sunrise; } }
        public DateTime Sunset { get { return _sunset; } }
        public Double Lat { get { return _Lat; } }
        public Double Long { get { return _Long; } }

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
        zvs.Processor.Logging.ILog log = zvs.Processor.Logging.LogManager.GetLogger<NOAAPlugin>();
        public NOAAPlugin()
            : base("NOAA",
               "NOAA Plug-in",
                "This plug-in will add Sunrise and Sunset control to zVirtualScenes."
                ) { }

        public override void Initialize()
        {
            using (zvsContext context = new zvsContext())
            {
                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "LAT",
                    Name = "Latitude",
                    Value = "37.6772222222222",
                    ValueType = DataType.DECIMAL,
                    Description = "Your Latitude in Decimal Notation. ex. 37.6772222222222"
                }, context);

                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "LOG",
                    Name = "Longitude",
                    Value = "-113.061666666667",
                    ValueType = DataType.DECIMAL,
                    Description = "Your Longitude in Decimal Notation. ex. -113.061666666667"
                }, context);

                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "DELAY_SUNRISE",
                    Name = "Minutes to delay sunrise",
                    Value = (0).ToString(),
                    ValueType = DataType.DECIMAL,
                    Description = "The minutes to delay sunrise as a positive or negative number"
                }, context);

                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "DELAY_SUNSET",
                    Name = "Minutes to delay sunset",
                    Value = (0).ToString(),
                    ValueType = DataType.DECIMAL,
                    Description = "The minutes to delay sunset as a positive or negative number"
                }, context);

                SceneProperty.AddOrEdit(new SceneProperty
                {
                    UniqueIdentifier = "ACTIVATE_SUNRISE",
                    Name = "Activate at Sunrise",
                    Description = "Activates this scene at sunrise",
                    Value = "false",
                    ValueType = DataType.BOOL
                }, context);

                SceneProperty.AddOrEdit(new SceneProperty
                {
                    UniqueIdentifier = "ACTIVATE_SUNSET",
                    Name = "Activate at Sunset",
                    Description = "Activates this scene at sunset",
                    Value = "false",
                    ValueType = DataType.BOOL
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
            log.InfoFormat("{0} started. Today's Sunrise: {1}, Today's Sunset: {2}", this.Name, _sunrise.ToString("T"), _sunset.ToString("T"));

            if (Utils.DebugMode)
            {
                timerNOAA.Interval = 10000;
            }
            else
            {
                timerNOAA.Interval = 60000;
            }
            timerNOAA.Elapsed += new ElapsedEventHandler(timerNOAA_Elapsed);
            timerNOAA.Enabled = true;

            IsReady = true;
        }

        protected override void StopPlugin()
        {
            timerNOAA.Elapsed -= new ElapsedEventHandler(timerNOAA_Elapsed);
            timerNOAA.Enabled = false;

            log.InfoFormat("{0} plug-in stopped.", this.Name);

            IsReady = false;
        }

        protected override void SettingChanged(string settingUniqueIdentifier, string settingValue)
        {
            if (settingUniqueIdentifier == "DELAY_SUNRISE")
            {
                Double.TryParse(settingValue, out _SunriseDelay);
            }
            else if (settingUniqueIdentifier == "DELAY_SUNSET")
            {
                Double.TryParse(settingValue, out _SunsetDelay);
            }
            else if (settingUniqueIdentifier == "LAT")
            {
                Double.TryParse(settingValue, out _Lat);
            }
            else if (settingUniqueIdentifier == "LOG")
            {
                Double.TryParse(settingValue, out _Long);
            }

            CalculateSunriseSet();
            log.Info(string.Format("Lat/Long updated.  New Sunrise: {0}, New Sunset: {1}", _sunrise.ToString("T"), _sunset.ToString("T")));
        }

        public override void ProcessDeviceCommand(zvs.Entities.QueuedDeviceCommand cmd) { }

        public override void ProcessDeviceTypeCommand(zvs.Entities.QueuedDeviceTypeCommand cmd) { }

        public override void Repoll(zvs.Entities.Device device) { }

        public override void ActivateGroup(int groupID) { }

        public override void DeactivateGroup(int groupID) { }

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
                using (zvsContext context = new zvsContext())
                {
                    Double MinsBetweenTimeSunrise = (_sunrise.TimeOfDay - DateTime.Now.TimeOfDay).TotalMinutes;
                    log.InfoFormat("RISE: MinsBetweenTimeSunrise={0}, _sunrise={1}, DateTime.Now.TimeOfDay={2}", MinsBetweenTimeSunrise, _sunrise, DateTime.Now.TimeOfDay);

                    if (MinsBetweenTimeSunrise < 1 && MinsBetweenTimeSunrise > 0)
                    {
                        log.Info("It is now sunrise. Activating sunrise scenes.");
                        foreach (Scene scene in context.Scenes)
                        {
                            string value = ScenePropertyValue.GetPropertyValue(context, scene, "ACTIVATE_SUNRISE");
                            bool activate = false;
                            bool.TryParse(value, out activate);

                            if (activate)
                            {
                                BuiltinCommand cmd = context.BuiltinCommands.FirstOrDefault(c => c.UniqueIdentifier == "RUN_SCENE");
                                if (cmd != null)
                                {
                                    CommandProcessor cp = new CommandProcessor(Core);
                                    cp.RunBuiltinCommand(context, cmd, scene.SceneId.ToString());
                                }
                            }
                        }
                        CalculateSunriseSet();
                    }

                    Double MinsBetweenTimeSunset = (_sunset.TimeOfDay - DateTime.Now.TimeOfDay).TotalMinutes;

                    log.InfoFormat("SET: MinsBetweenTimeSunset={0}, _sunset={1}, DateTime.Now.TimeOfDay={2}", MinsBetweenTimeSunset, _sunset, DateTime.Now.TimeOfDay);


                    if (MinsBetweenTimeSunset < 1 && MinsBetweenTimeSunset > 0)
                    {
                        log.Info("It is now sunset. Activating sunset scenes.");
                        foreach (Scene scene in context.Scenes)
                        {
                            string value = ScenePropertyValue.GetPropertyValue(context, scene, "ACTIVATE_SUNSET");
                            bool activate = false;
                            bool.TryParse(value, out activate);

                            if (activate)
                            {
                                BuiltinCommand cmd = context.BuiltinCommands.FirstOrDefault(c => c.UniqueIdentifier == "RUN_SCENE");
                                if (cmd != null)
                                {
                                    CommandProcessor cp = new CommandProcessor(Core);
                                    cp.RunBuiltinCommand(context, cmd, scene.SceneId.ToString());
                                }
                            }
                        }
                        CalculateSunriseSet();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Warn("Error calculating Sunrise/Sunset. - " + ex.Message);
            }
        }
        #endregion

    }

}

