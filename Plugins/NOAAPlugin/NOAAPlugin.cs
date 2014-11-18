using System.ComponentModel.Composition;
using System;
using System.Timers;
using System.ComponentModel;
using System.Linq;
using zvs.Processor;
using zvs.DataModel;
using System.Threading.Tasks;
using System.Data.Entity;

namespace NOAAPlugin
{
    [Export(typeof(zvsPlugin))]
    public class NOAAPlugin : zvsPlugin
    {
        public volatile bool isActive;
        private System.Timers.Timer timerNOAA = new System.Timers.Timer();

        private DateTime _date = DateTime.Today;
        private bool _isSunrise = false;
        private bool _isSunset = false;
        private DateTime _sunrise = DateTime.Now;
        private DateTime _sunset = DateTime.Now;
        zvs.Processor.Logging.ILog log = zvs.Processor.Logging.LogManager.GetLogger<NOAAPlugin>();

        private double _LatitudeSetting = 30.6772222222222;
        public double LatitudeSetting
        {
            get { return _LatitudeSetting; }
            set
            {
                if (value != _LatitudeSetting)
                {
                    _LatitudeSetting = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Double _LongitudeSetting = -100.061666666667;
        public Double LongitudeSetting
        {
            get { return _LongitudeSetting; }
            set
            {
                if (value != _LongitudeSetting)
                {
                    _LongitudeSetting = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Double _SunriseDelaySetting = 0;
        public Double SunriseDelaySetting
        {
            get { return _SunriseDelaySetting; }
            set
            {
                if (value != _SunriseDelaySetting)
                {
                    _SunriseDelaySetting = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Double _SunsetDelaySetting = 0;
        public Double SunsetDelaySetting
        {
            get { return _SunsetDelaySetting; }
            set
            {
                if (value != _SunsetDelaySetting)
                {
                    _SunsetDelaySetting = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public override Guid PluginGuid
        {
            get { return Guid.Parse("568fe426-577f-43f9-a329-9924db98a72a"); }
        }

        public override string Name
        {
            get { return "NOAA Plug-in for ZVS"; }
        }

        public override string Description
        {
            get { return "This plug-in adds Sunrise and Sunset control to zVirtualScenes."; }
        }

        public override async Task OnSettingsCreating(PluginSettingBuilder settingBuilder)
        {
            var latSetting = new PluginSetting
            {
                UniqueIdentifier = "LAT",
                Name = "Latitude",
                Value = "37.6772222222222",
                ValueType = DataType.DECIMAL,
                Description = "Your Latitude in Decimal Notation. ex. 37.6772222222222"
            };
            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(latSetting, o => o.LatitudeSetting);

            var longSetting = new PluginSetting
            {
                UniqueIdentifier = "LOG",
                Name = "Longitude",
                Value = "-113.061666666667",
                ValueType = DataType.DECIMAL,
                Description = "Your Longitude in Decimal Notation. ex. -113.061666666667"
            };
            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(longSetting, o => o.LongitudeSetting);

            var delaySunriseSetting = new PluginSetting
            {
                UniqueIdentifier = "DELAY_SUNRISE",
                Name = "Minutes to delay sunrise",
                Value = (0).ToString(),
                ValueType = DataType.DECIMAL,
                Description = "The minutes to delay sunrise as a positive or negative number"
            };
            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(delaySunriseSetting, o => o.SunriseDelaySetting);

            var delaySunsetSetting = new PluginSetting
            {
                UniqueIdentifier = "DELAY_SUNSET",
                Name = "Minutes to delay sunset",
                Value = (0).ToString(),
                ValueType = DataType.DECIMAL,
                Description = "The minutes to delay sunset as a positive or negative number"
            };
            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(delaySunsetSetting, o => o.SunsetDelaySetting);
        }

        public enum SceneSettingUids
        {
            ACTIVATE_AT_SUNRISE,
            ACTIVATE_AT_SUNSET
        }

        public override async Task OnSceneSettingsCreating(SceneSettingBuilder settingBuilder)
        {
            await settingBuilder.RegisterAsync(new SceneSetting
            {
                UniqueIdentifier = SceneSettingUids.ACTIVATE_AT_SUNRISE.ToString(),
                Name = "Activate at Sunrise",
                Description = "Activates this scene at sunrise",
                Value = "false",
                ValueType = DataType.BOOL
            });

            await settingBuilder.RegisterAsync(new SceneSetting
            {
                UniqueIdentifier = SceneSettingUids.ACTIVATE_AT_SUNSET.ToString(),
                Name = "Activate at Sunset",
                Description = "Activates this scene at sunset",
                Value = "false",
                ValueType = DataType.BOOL
            });
        }

        private void RecalcAndLogNewTimes()
        {
            CalculateSunriseSet();
            log.InfoFormat("{0} started. Today's Sunrise: {1}, Today's Sunset: {2}", this.Name, _sunrise.ToString("T"), _sunset.ToString("T"));
        }

        void NOAAPlugin_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("LatitudeSetting") ||
                e.PropertyName.Equals("LongitudeSetting") ||
                e.PropertyName.Equals("SunriseDelaySetting") ||
                e.PropertyName.Equals("SunsetDelaySetting"))

                RecalcAndLogNewTimes();
        }

        public override Task StartAsync()
        {
            RecalcAndLogNewTimes();

            timerNOAA.Interval = 60000;

            PropertyChanged += NOAAPlugin_PropertyChanged;
            timerNOAA.Elapsed += new ElapsedEventHandler(timerNOAA_Elapsed);
            timerNOAA.Enabled = true;

            return Task.FromResult(0);
        }

        public override Task StopAsync()
        {
            PropertyChanged -= NOAAPlugin_PropertyChanged;
            timerNOAA.Elapsed -= new ElapsedEventHandler(timerNOAA_Elapsed);
            timerNOAA.Enabled = false;

            log.InfoFormat("{0} plug-in stopped.", this.Name);
            return Task.FromResult(0);
        }

        public override Task DeviceValueChangedAsync(long deviceValueId, string newValue, string oldValue)
        {
            return Task.FromResult(0);
        }

        #region NOAA

        private void CalculateSunriseSet()
        {
            _date = DateTime.Now;
            SunTimes.Instance.CalculateSunRiseSetTimes(LatitudeSetting, LongitudeSetting, _date, ref _sunrise, ref _sunset, ref _isSunrise, ref _isSunset);

            //Add delays
            if (_sunrise != null)
                _sunrise = _sunrise.AddMinutes(_SunriseDelaySetting);

            if (_sunset != null)
                _sunset = _sunset.AddMinutes(_SunsetDelaySetting);
        }

        public bool isDark()
        {
            if (DateTime.Now.TimeOfDay < _sunrise.TimeOfDay || DateTime.Now.TimeOfDay > _sunset.TimeOfDay)
                return true;

            return false;
        }

        private async void timerNOAA_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                CalculateSunriseSet();

                using (ZvsContext context = new ZvsContext())
                {
                    Double MinsBetweenTimeSunrise = (_sunrise.TimeOfDay - DateTime.Now.TimeOfDay).TotalMinutes;
                    log.DebugFormat("RISE: MinsBetweenTimeSunrise={0}, _sunrise={1}, DateTime.Now.TimeOfDay={2}", MinsBetweenTimeSunrise, _sunrise, DateTime.Now.TimeOfDay);

                    if (MinsBetweenTimeSunrise < 1 && MinsBetweenTimeSunrise > 0)
                    {
                        log.Info("It is now sunrise. Activating sunrise scenes.");
                        foreach (Scene scene in context.Scenes)
                        {
                            string value = await SceneSettingValue.GetPropertyValueAsync(context, scene, SceneSettingUids.ACTIVATE_AT_SUNRISE.ToString());
                            bool activate = false;
                            bool.TryParse(value, out activate);

                            if (activate)
                            {
                                BuiltinCommand cmd = await context.BuiltinCommands.FirstOrDefaultAsync(c => c.UniqueIdentifier == "RUN_SCENE");
                                if (cmd != null)
                                {
                                    CommandProcessor cp = new CommandProcessor(ZvsEngine);
                                    await cp.RunCommandAsync(this, cmd, scene.Id.ToString());
                                }
                            }
                        }
                    }

                    Double MinsBetweenTimeSunset = (_sunset.TimeOfDay - DateTime.Now.TimeOfDay).TotalMinutes;

                    log.DebugFormat("SET: MinsBetweenTimeSunset={0}, _sunset={1}, DateTime.Now.TimeOfDay={2}", MinsBetweenTimeSunset, _sunset, DateTime.Now.TimeOfDay);


                    if (MinsBetweenTimeSunset < 1 && MinsBetweenTimeSunset > 0)
                    {
                        log.Info("It is now sunset. Activating sunset scenes.");
                        foreach (Scene scene in context.Scenes)
                        {
                            string value = await SceneSettingValue.GetPropertyValueAsync(context, scene, SceneSettingUids.ACTIVATE_AT_SUNSET.ToString());
                            bool activate = false;
                            bool.TryParse(value, out activate);

                            if (activate)
                            {
                                BuiltinCommand cmd = context.BuiltinCommands.FirstOrDefault(c => c.UniqueIdentifier == "RUN_SCENE");
                                if (cmd != null)
                                {
                                    CommandProcessor cp = new CommandProcessor(ZvsEngine);
                                    await cp.RunCommandAsync(this, cmd, scene.Id.ToString());
                                }
                            }
                        }
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

