using System.ComponentModel.Composition;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Timers;
using System.ComponentModel;
using System.Linq;
using zvs.Processor;
using zvs.DataModel;
using System.Threading.Tasks;
using System.Data.Entity;

namespace NOAAPlugin
{
    [Export(typeof(ZvsPlugin))]
    public class NoaaPlugin : ZvsPlugin
    {
        public volatile bool IsActive;
        private readonly Timer _timerNoaa = new Timer();

        private DateTime _date = DateTime.Today;
        private bool _isSunrise;
        private bool _isSunset;
        private DateTime _sunrise = DateTime.Now;
        private DateTime _sunset = DateTime.Now;

        private double _latitudeSetting = 30.6772222222222;
        public double LatitudeSetting
        {
            get { return _latitudeSetting; }
            set
            {
                if (value == _latitudeSetting) return;
                _latitudeSetting = value;
                NotifyPropertyChanged();
            }
        }

        private Double _longitudeSetting = -100.061666666667;
        public Double LongitudeSetting
        {
            get { return _longitudeSetting; }
            set
            {
                if (value == _longitudeSetting) return;
                _longitudeSetting = value;
                NotifyPropertyChanged();
            }
        }

        private Double _sunriseDelaySetting;
        public Double SunriseDelaySetting
        {
            get { return _sunriseDelaySetting; }
            set
            {
                if (value == _sunriseDelaySetting) return;
                _sunriseDelaySetting = value;
                NotifyPropertyChanged();
            }
        }

        private Double _sunsetDelaySetting;
        public Double SunsetDelaySetting
        {
            get { return _sunsetDelaySetting; }
            set
            {
                if (value == _sunsetDelaySetting) return;
                _sunsetDelaySetting = value;
                NotifyPropertyChanged();
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
                Value = "0",
                ValueType = DataType.DECIMAL,
                Description = "The minutes to delay sunrise as a positive or negative number"
            };
            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(delaySunriseSetting, o => o.SunriseDelaySetting);

            var delaySunsetSetting = new PluginSetting
            {
                UniqueIdentifier = "DELAY_SUNSET",
                Name = "Minutes to delay sunset",
                Value = "0",
                ValueType = DataType.DECIMAL,
                Description = "The minutes to delay sunset as a positive or negative number"
            };
            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(delaySunsetSetting, o => o.SunsetDelaySetting);
        }

        public enum SceneSettingUids
        {
            ActivateAtSunrise,
            ActivateAtSunset
        }

        public override async Task OnSceneSettingsCreating(SceneSettingBuilder settingBuilder)
        {
            await settingBuilder.RegisterAsync(new SceneSetting
            {
                UniqueIdentifier = SceneSettingUids.ActivateAtSunrise.ToString(),
                Name = "Activate at Sunrise",
                Description = "Activates this scene at sunrise",
                Value = "false",
                ValueType = DataType.BOOL
            }, CancellationToken);

            await settingBuilder.RegisterAsync(new SceneSetting
            {
                UniqueIdentifier = SceneSettingUids.ActivateAtSunset.ToString(),
                Name = "Activate at Sunset",
                Description = "Activates this scene at sunset",
                Value = "false",
                ValueType = DataType.BOOL
            }, CancellationToken);
        }

        private async void RecalcAndLogNewTimes()
        {
            CalculateSunriseSet();
            await Log.ReportInfoFormatAsync(CancellationToken, "{0} started. Today's Sunrise: {1}, Today's Sunset: {2}", Name, _sunrise.ToString("T"), _sunset.ToString("T"));
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

            _timerNoaa.Interval = 60000;
            PropertyChanged += NOAAPlugin_PropertyChanged;
            _timerNoaa.Elapsed += timerNOAA_Elapsed;
            _timerNoaa.Enabled = true;

            return Task.FromResult(0);
        }

        public override async Task StopAsync()
        {
            PropertyChanged -= NOAAPlugin_PropertyChanged;
            _timerNoaa.Elapsed -= timerNOAA_Elapsed;
            _timerNoaa.Enabled = false;

            await Log.ReportInfoFormatAsync(CancellationToken, "{0} plug-in stopped.", Name);
        }

        #region NOAA

        private void CalculateSunriseSet()
        {
            _date = DateTime.Now;
            SunTimes.Instance.CalculateSunRiseSetTimes(LatitudeSetting, LongitudeSetting, _date, ref _sunrise, ref _sunset, ref _isSunrise, ref _isSunset);

            //Add delays
            if (_sunrise != null)
                _sunrise = _sunrise.AddMinutes(_sunriseDelaySetting);

            if (_sunset != null)
                _sunset = _sunset.AddMinutes(_sunsetDelaySetting);
        }

        public bool IsDark()
        {
            return DateTime.Now.TimeOfDay < _sunrise.TimeOfDay || DateTime.Now.TimeOfDay > _sunset.TimeOfDay;
        }

        private async void timerNOAA_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                CalculateSunriseSet();

                using (var context = new ZvsContext(EntityContextConnection))
                {
                    var minsBetweenTimeSunrise = (_sunrise.TimeOfDay - DateTime.Now.TimeOfDay).TotalMinutes;
                    Debug.WriteLine("RISE: MinsBetweenTimeSunrise={0}, _sunrise={1}, DateTime.Now.TimeOfDay={2}", minsBetweenTimeSunrise, _sunrise, DateTime.Now.TimeOfDay);

                    if (minsBetweenTimeSunrise < 1 && minsBetweenTimeSunrise > 0)
                    {
                        await Log.ReportInfoAsync("It is now sunrise. Activating sunrise scenes.", CancellationToken);
                        foreach (var scene in context.Scenes)
                        {
                            var value = await SceneSettingValue.GetPropertyValueAsync(context, scene, SceneSettingUids.ActivateAtSunrise.ToString());
                            bool activate;
                            bool.TryParse(value, out activate);

                            if (!activate) continue;
                            var cmd = await context.BuiltinCommands.FirstOrDefaultAsync(c => c.UniqueIdentifier == "RUN_SCENE");
                            if (cmd == null) continue;
                            await RunCommandAsync(cmd.Id, scene.Id.ToString(CultureInfo.InvariantCulture), string.Empty, CancellationToken);
                        }
                    }

                    var minsBetweenTimeSunset = (_sunset.TimeOfDay - DateTime.Now.TimeOfDay).TotalMinutes;

                    Debug.WriteLine("SET: MinsBetweenTimeSunset={0}, _sunset={1}, DateTime.Now.TimeOfDay={2}", minsBetweenTimeSunset, _sunset, DateTime.Now.TimeOfDay);


                    if (!(minsBetweenTimeSunset < 1) || !(minsBetweenTimeSunset > 0)) return;
                    await Log.ReportInfoAsync("It is now sunset. Activating sunset scenes.", CancellationToken);
                    foreach (var scene in context.Scenes)
                    {
                        var value = await SceneSettingValue.GetPropertyValueAsync(context, scene, SceneSettingUids.ActivateAtSunset.ToString());
                        bool activate;
                        bool.TryParse(value, out activate);

                        if (!activate) continue;
                        var cmd = context.BuiltinCommands.FirstOrDefault(c => c.UniqueIdentifier == "RUN_SCENE");
                        if (cmd == null) continue;
                        await RunCommandAsync(cmd.Id, scene.Id.ToString(CultureInfo.InvariantCulture), string.Empty, CancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ReportErrorFormatAsync(CancellationToken, "Error calculating Sunrise/Sunset. {0}", ex.Message).Wait();
            }
        }
        #endregion
    }
}

