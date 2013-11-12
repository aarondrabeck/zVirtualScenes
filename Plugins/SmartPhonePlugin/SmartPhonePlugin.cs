using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using zvs.Entities;
using zvs.Processor;

namespace SmartPhonePlugin
{
    [Export(typeof(zvsPlugin))]
    public class SmartPhonePlugin : zvsPlugin
    {
        private System.Timers.Timer timerSmartPhone = new System.Timers.Timer();
        zvs.Processor.Logging.ILog log = zvs.Processor.Logging.LogManager.GetLogger<SmartPhonePlugin>();

        private static int _failures = 0;
        private static bool _lastResult = false;

        public override Guid PluginGuid
        {
            get { return Guid.Parse("8d293c1b-250a-4674-827a-6e6caf9aa8ed"); }
        }

        public override string Name
        {
            get { return "SmartPhone Plugin"; }
        }

        public override string Description
        {
            get { return "This plugin will monitor IP addresses to know when someone is home"; }
        }

        public override async Task OnSettingsCreating(PluginSettingBuilder settingBuilder)
        {
            var customsetting = new PluginSetting
            {
                UniqueIdentifier = "MON_IPS",
                Name = "Monitor IPs",
                Value = "192.168.1.100",
                ValueType = DataType.STRING,
                Description = "Include all IPs you would like monitored. Comma Separated."
            };
            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(customsetting, o => o.MonitorIpsSetting);

            var intervalsetting = new PluginSetting
            {
                UniqueIdentifier = "CHK_INT",
                Name = "Check Interval (milliseconds)",
                Value = "600000",
                ValueType = DataType.INTEGER,
                Description = "1000 milliseconds = 1 second"
            };
            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(intervalsetting, o => o.CheckInterval);

            var tolerationsetting = new PluginSetting
            {
                UniqueIdentifier = "FAIL_TOLERATION",
                Name = "Failure Toleration",
                Value = "2",
                ValueType = DataType.INTEGER,
                Description = "How many times no phones detected until the not home event is triggered"
            };
            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(tolerationsetting, o => o.Toleration);
        }

        public enum SceneSettingUids
        {
            ACTIVATE_AT_AWAY,
            ACTIVATE_AT_HOME
        }

        public override async Task OnSceneSettingsCreating(SceneSettingBuilder settingBuilder)
        {
            await settingBuilder.RegisterAsync(new SceneSetting
            {
                UniqueIdentifier = SceneSettingUids.ACTIVATE_AT_AWAY.ToString(),
                Name = "Activate when everyone leaves",
                Description = "Activates this scene when everyone leaves.",
                Value = "false",
                ValueType = DataType.BOOL
            });

            await settingBuilder.RegisterAsync(new SceneSetting
            {
                UniqueIdentifier = SceneSettingUids.ACTIVATE_AT_HOME.ToString(),
                Name = "Activate when someone is home",
                Description = "Activates this scene when someone is home.",
                Value = "false",
                ValueType = DataType.BOOL
            });
        }

        private string _monitorIpsSetting = "";
        public string MonitorIpsSetting
        {
            get { return _monitorIpsSetting; }
            set
            {
                if (value != _monitorIpsSetting)
                {
                    _monitorIpsSetting = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _checkInterval = 600000;
        public int CheckInterval
        {
            get { return _checkInterval; }
            set
            {
                if (value != _checkInterval)
                {
                    _checkInterval = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _toleration = 2;
        public int Toleration
        {
            get { return _toleration; }
            set
            {
                if (value != _toleration)
                {
                    _toleration = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public override Task StartAsync()
        {
            timerSmartPhone.Interval = _checkInterval;
            timerSmartPhone.Elapsed += timerSmartPhone_Elapsed;
            timerSmartPhone.Enabled = true;

            PropertyChanged += SmartPhonePlugin_PropertyChanged;

            return Task.FromResult(0);
        }

        public override Task StopAsync()
        {
            timerSmartPhone.Stop();
            timerSmartPhone.Elapsed -= timerSmartPhone_Elapsed;
            timerSmartPhone.Enabled = false;

            PropertyChanged -= SmartPhonePlugin_PropertyChanged;

            return Task.FromResult(0);
        }

        public override Task DeviceValueChangedAsync(long deviceValueId, string newValue, string oldValue)
        {
            return Task.FromResult(0);
        }

        public bool isHome()
        {
            return _lastResult;
        }

        private void SmartPhonePlugin_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("CheckInterval"))
                timerSmartPhone.Interval = _checkInterval;
        }

        private async void timerSmartPhone_Elapsed(object sender, ElapsedEventArgs e)
        {
            timerSmartPhone.Enabled = false;

            if (!string.IsNullOrEmpty(_monitorIpsSetting))
            {
                try
                {
                    using (zvsContext context = new zvsContext())
                    {
                        bool anyoneHome = false;

                        string[] IPs = _monitorIpsSetting.Split(',');

                        foreach (var ip in IPs)
                        {
                            if (Ping(ip))
                                anyoneHome = true;
                        }

                        if (anyoneHome)
                        {
                            if (!_lastResult)
                            {
                                _failures = 0;
                                _lastResult = true;

                                log.Info("Someone is home, activating scenes for that");
                                foreach (var scene in context.Scenes)
                                {
                                    var value = await SceneSettingValue.GetPropertyValueAsync(context, scene, SceneSettingUids.ACTIVATE_AT_HOME.ToString());
                                    bool activate;
                                    bool.TryParse(value, out activate);

                                    if (activate)
                                    {
                                        var cmd = await context.BuiltinCommands.FirstOrDefaultAsync(c => c.UniqueIdentifier == "RUN_SCENE");
                                        if (cmd != null)
                                        {
                                            var cp = new CommandProcessor(Core);
                                            await cp.RunCommandAsync(this, cmd, scene.Id.ToString());
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (_lastResult)
                            {
                                _failures++;                                

                                if (_failures >= _toleration)
                                {
                                    _lastResult = false;
                                    log.Info("No one is home, activating scenes for that");
                                    foreach (var scene in context.Scenes)
                                    {
                                        var value = await SceneSettingValue.GetPropertyValueAsync(context, scene, SceneSettingUids.ACTIVATE_AT_AWAY.ToString());
                                        bool activate;
                                        bool.TryParse(value, out activate);

                                        if (activate)
                                        {
                                            var cmd = await context.BuiltinCommands.FirstOrDefaultAsync(c => c.UniqueIdentifier == "RUN_SCENE");
                                            if (cmd != null)
                                            {
                                                var cp = new CommandProcessor(Core);
                                                await cp.RunCommandAsync(this, cmd, scene.Id.ToString());
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Warn("Error checking for SmartPhones. - " + ex.Message);
                }
            }

            timerSmartPhone.Enabled = true;
        }

        private bool Ping(string ip, bool retry = true)
        {
            var pingSender = new Ping();
            var pingOptions = new PingOptions { DontFragment = true };

            // Create a buffer of 32 bytes of data to be transmitted.
            try
            {
                var reply = pingSender.Send(ip, 120, Encoding.ASCII.GetBytes("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"), pingOptions);

                if (reply != null && reply.Status == IPStatus.Success)
                {
                    //Ping was successful
                    return true;
                }
                else if (retry)
                {
                    // Sometimes we are not successful... so try one more time
                    return this.Ping(ip, false);
                }
            }
            catch (Exception)
            {
                // Host not found?
            }

            return false;
        }
    }
}
