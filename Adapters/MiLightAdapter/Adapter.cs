using System;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using zvs;
using zvs.DataModel;
using zvs.Processor;

namespace MiLightAdapter
{

    /// <summary>
    /// This plugin will connect up to 4 milight wifi controllers, each of which have 4 scene options
    /// </summary>
    [Export(typeof(ZvsAdapter))]
    public class Adapter : ZvsAdapter
    {
        private readonly Controller _controller = new Controller();

        public override Guid AdapterGuid
        {
            get { return Guid.Parse("4578EA73-90F3-4A20-9E60-36F395462CA9"); }
        }

        public override string Name
        {
            get { return "MiLight Adapter"; }
        }

        public override string Description
        {
            get { return "Control your MiLight lighting."; }
        }

        public override async Task StartAsync()
        {
            await Log.ReportInfoAsync(string.Format("{0} Started", Name), CancellationToken);
            await AddNewWifiControllerToDatabase(WiFi1Setting);
            _controller.AddController(WiFi1Setting);

            await AddNewWifiControllerToDatabase(WiFi2Setting);
            _controller.AddController(WiFi2Setting);

            await AddNewWifiControllerToDatabase(WiFi3Setting);
            _controller.AddController(WiFi3Setting);

            await AddNewWifiControllerToDatabase(WiFi4Setting);
            _controller.AddController(WiFi4Setting);

        }

        public override async Task StopAsync()
        {
            await Log.ReportInfoAsync(string.Format("{0} Stopped", Name), CancellationToken);
        }

        public override async Task ProcessDeviceTypeCommandAsync(DeviceType deviceType, Device device,
            DeviceTypeCommand command, string argument)
        {

            var miCommand = command.CustomData1;
            var zone = command.CustomData2;
            var ip = (from d in device.Values where d.Name == "IPAddress" select d.Value).FirstOrDefault();
            decimal level;
            decimal.TryParse(argument, out level);
            await _controller.Send(ip, miCommand, zone, level);

            await Log.ReportInfoAsync(string.Format("{0} Command Sent Command:{1}, Zone:{2}, IP:{3}, Level:{4}", Name, miCommand, zone, ip, level), CancellationToken);
        }

        public override Task ProcessDeviceCommandAsync(Device device, DeviceCommand command, string argument, string argument2)
        {
            return Task.FromResult(0);
        }

        public override Task RepollAsync(Device device)
        {
            return Task.FromResult(0);
        }

        public override Task ActivateGroupAsync(Group @group)
        {
            return Task.FromResult(0);
        }

        public override Task DeactivateGroupAsync(Group @group)
        {
            return Task.FromResult(0);
        }

        private string _wifi1Setting = "";
        public string WiFi1Setting
        {
            get { return _wifi1Setting; }
            set
            {
                if (value == _wifi1Setting) return;
                _wifi1Setting = value;
                NotifyPropertyChanged();
            }
        }

        private string _wifi2Setting = "";
        public string WiFi2Setting
        {
            get { return _wifi2Setting; }
            set
            {
                if (value == _wifi2Setting) return;
                _wifi2Setting = value;
                NotifyPropertyChanged();
            }
        }
        private string _wifi3Setting = "";
        public string WiFi3Setting
        {
            get { return _wifi3Setting; }
            set
            {
                if (value == _wifi3Setting) return;
                _wifi3Setting = value;
                NotifyPropertyChanged();
            }
        }
        private string _wifi4Setting = "";
        public string WiFi4Setting
        {
            get { return _wifi4Setting; }
            set
            {
                if (value == _wifi4Setting) return;
                _wifi4Setting = value;
                NotifyPropertyChanged();
            }
        }



        public override async Task OnSettingsCreating(AdapterSettingBuilder settingBuilder)
        {
            var wifi1IpSetting = new AdapterSetting
            {
                Name = "WiFi Controller 1 IP Address",
                Value = "",
                ValueType = DataType.STRING,
                Description = "The IP Address of the first WiFi Controller."
            };

            var wifi1SettingResult =
                await settingBuilder.Adapter(this).RegisterAdapterSettingAsync(wifi1IpSetting, o => o.WiFi1Setting);

            if (wifi1SettingResult.HasError)
                await
                    Log.ReportErrorFormatAsync(CancellationToken,
                        "An error occured when registering the wifi controller 1 setting. {0}",
                        wifi1SettingResult.Message);



            var wifi2IpSetting = new AdapterSetting
            {
                Name = "WiFi Controller 2 IP Address",
                Value = "",
                ValueType = DataType.STRING,
                Description = "The IP Address of the second WiFi Controller."
            };

            var wifi2SettingResult =
                await settingBuilder.Adapter(this).RegisterAdapterSettingAsync(wifi2IpSetting, o => o.WiFi2Setting);

            if (wifi2SettingResult.HasError)
                await
                    Log.ReportErrorFormatAsync(CancellationToken,
                        "An error occured when registering the wifi controller 2 setting. {0}",
                        wifi2SettingResult.Message);

            var wifi3IpSetting = new AdapterSetting
            {
                Name = "WiFi Controller 3 IP Address",
                Value = "",
                ValueType = DataType.STRING,
                Description = "The IP Address of the third WiFi Controller."
            };

            var wifi3SettingResult =
                await settingBuilder.Adapter(this).RegisterAdapterSettingAsync(wifi3IpSetting, o => o.WiFi3Setting);

            if (wifi3SettingResult.HasError)
                await
                    Log.ReportErrorFormatAsync(CancellationToken,
                        "An error occured when registering the wifi controller 1 setting. {0}",
                        wifi3SettingResult.Message);

            var wifi4IpSetting = new AdapterSetting
            {
                Name = "WiFi Controller 4 IP Address",
                Value = "",
                ValueType = DataType.STRING,
                Description = "The IP Address of the fourth WiFi Controller."
            };

            var wifi4SettingResult =
                await settingBuilder.Adapter(this).RegisterAdapterSettingAsync(wifi4IpSetting, o => o.WiFi4Setting);

            if (wifi4SettingResult.HasError)
                await
                    Log.ReportErrorFormatAsync(CancellationToken,
                        "An error occured when registering the wifi controller 4 setting. {0}",
                        wifi4SettingResult.Message);






            await base.OnSettingsCreating(settingBuilder);
        }

        public override async Task OnDeviceTypesCreating(DeviceTypeBuilder deviceTypeBuilder)
        {
            //Dimmer Type Devices
            var dimmerDt = new DeviceType
            {
                UniqueIdentifier = MiLightDeviceTypes.Color.ToString(),
                Name = "MiLight Color Light",
                ShowInList = true
            };
            dimmerDt.Commands.Add(new DeviceTypeCommand
            {
                UniqueIdentifier = "Z1TURNON",
                Name = "Zone 1, Turn On",
                ArgumentType = DataType.NONE,
                CustomData1 = "On",
                CustomData2 = "One",
                Description = "Turns Zone 1 On."
            });
            dimmerDt.Commands.Add(new DeviceTypeCommand
            {
                UniqueIdentifier = "Z1TURNOFF",
                Name = "Zone 1, Turn Off",
                CustomData1 = "Off",
                CustomData2 = "One",
                ArgumentType = DataType.NONE,
                Description = "Turns Zone 1 Off."
            });
            dimmerDt.Commands.Add(new DeviceTypeCommand
            {
                UniqueIdentifier = "Z2TURNON",
                Name = "Zone 2, Turn On",
                ArgumentType = DataType.NONE,
                CustomData1 = "On",
                CustomData2 = "Two",
                Description = "Turns Zone 2 On."
            });
            dimmerDt.Commands.Add(new DeviceTypeCommand
            {
                UniqueIdentifier = "Z2TURNOFF",
                Name = "Zone 2, Turn Off",
                CustomData1 = "Off",
                CustomData2 = "Two",
                ArgumentType = DataType.NONE,
                Description = "Turns Zone 2 Off."
            });

            dimmerDt.Commands.Add(new DeviceTypeCommand
            {
                UniqueIdentifier = "Z3TURNON",
                Name = "Zone 3, Turn On",
                ArgumentType = DataType.NONE,
                CustomData1 = "On",
                CustomData2 = "Three",
                Description = "Turns Zone 3 On."
            });

            dimmerDt.Commands.Add(new DeviceTypeCommand
            {
                UniqueIdentifier = "Z3TURNOFF",
                Name = "Zone 3, Turn Off",
                CustomData1 = "Off",
                CustomData2 = "Three",
                ArgumentType = DataType.NONE,
                Description = "Turns Zone 3 Off."
            });

            dimmerDt.Commands.Add(new DeviceTypeCommand
            {
                UniqueIdentifier = "Z4TURNON",
                Name = "Zone 4, Turn On",
                ArgumentType = DataType.NONE,
                CustomData1 = "On",
                CustomData2 = "Four",
                Description = "Turns Zone 4 On."
            });

            dimmerDt.Commands.Add(new DeviceTypeCommand
            {
                UniqueIdentifier = "Z4TURNOFF",
                Name = "Zone 4, Turn Off",
                CustomData1 = "Off",
                CustomData2 = "Four",
                ArgumentType = DataType.NONE,
                Description = "Turns Zone 4 Off."
            });

            dimmerDt.Commands.Add(new DeviceTypeCommand
            {
                UniqueIdentifier = "ALLOFF",
                Name = "All Off",
                CustomData1 = "AllOff",
                CustomData2 = "",
                ArgumentType = DataType.NONE,
                Description = "Turns All Zones Off."
            });

            dimmerDt.Commands.Add(new DeviceTypeCommand
            {
                UniqueIdentifier = "ALLON",
                Name = "All On",
                CustomData1 = "AllOn",
                CustomData2 = "",
                ArgumentType = DataType.NONE,
                Description = "Turns All Zones On."
            });

            dimmerDt.Commands.Add(new DeviceTypeCommand
            {
                UniqueIdentifier = "HUE",
                Name = "Hue",
                CustomData1 = "Hue",
                CustomData2 = "",
                ArgumentType = DataType.DECIMAL,
                Description = "Changes the current zone the specified hue."
            });

            dimmerDt.Commands.Add(new DeviceTypeCommand
            {
                UniqueIdentifier = "SETBRIGHTNESS",
                Name = "SetBrightness",
                CustomData1 = "SetBrightness",
                CustomData2 = "",
                ArgumentType = DataType.INTEGER,
                Description = "Changes the current zone the specified brightness."
            });

            dimmerDt.Commands.Add(new DeviceTypeCommand
            {
                UniqueIdentifier = "EFFECTDOWN",
                Name = "Previous effect",
                ArgumentType = DataType.NONE,
                CustomData1 = "EffectDown",
                CustomData2 = "",
                Description = "Changes the current zone to the previous effect."
            });

            dimmerDt.Commands.Add(new DeviceTypeCommand
            {
                UniqueIdentifier = "EFFECTUP",
                Name = "Next Effect",
                CustomData1 = "EffectUp",
                CustomData2 = "",
                ArgumentType = DataType.NONE,
                Description = "Changes the current zone to the next effect."
            });

            dimmerDt.Commands.Add(new DeviceTypeCommand
            {
                UniqueIdentifier = "SPEEDDOWN",
                Name = "Slower speed",
                ArgumentType = DataType.NONE,
                CustomData1 = "SpeedDown",
                CustomData2 = "",
                Description = "Changes the current effect to a slower speed."
            });
            dimmerDt.Commands.Add(new DeviceTypeCommand
            {
                UniqueIdentifier = "SPEEDUP",
                Name = "Faster Speed",
                CustomData1 = "SpeedUp",
                CustomData2 = "",
                ArgumentType = DataType.NONE,
                Description = "Changes the current effect to a faster speed."
            });

            var dimmerSaveResult = await deviceTypeBuilder.RegisterAsync(AdapterGuid, dimmerDt, CancellationToken);
            if (dimmerSaveResult.HasError)
                await
                    Log.ReportErrorFormatAsync(CancellationToken,
                        "An error occured when registering the OpenZWave dimmer device type. {0}",
                        dimmerSaveResult.Message);

            using (var context = new ZvsContext(EntityContextConnection))
            {
                DimmerTypeId =
                    await
                        context.DeviceTypes.Where(o => o.UniqueIdentifier == MiLightDeviceTypes.Color.ToString())
                            .Select(o => o.Id)
                            .FirstOrDefaultAsync();
            }

            await base.OnDeviceTypesCreating(deviceTypeBuilder);
        }

        private int DimmerTypeId { get; set; }

        private async Task AddNewWifiControllerToDatabase(string ipAddress)
        {
            if (!string.IsNullOrEmpty(ipAddress))
            {
                using (var context = new ZvsContext(EntityContextConnection))
                {
                    var devices = from d in context.Devices where d.Type.Adapter.AdapterGuid == AdapterGuid select d;
                    Device existing = null;

                    foreach (var d in devices)
                    {
                        var value =
                            (from v in d.Values
                             where v.Name == "IPAddress" && v.Value == ipAddress
                             select v).FirstOrDefault();

                        if (value != null)
                        {
                            existing = d;
                            break;
                        }
                    }

                    //If already have the device, don't install a duplicate
                    if (existing != null)
                        return;

                    existing = new Device
                    {
                        DeviceTypeId = DimmerTypeId,
                        Name = string.Format("MiLight WiFi Controller - {0}", ipAddress),
                        Location = string.Format("MiLight WiFi Controller - {0}", ipAddress),
                        CurrentLevelInt = 0,
                        CurrentLevelText = "",
                    };

                    existing.Values.Add(new DeviceValue()
                    {
                        Name = "IPAddress",
                        Value = ipAddress,
                    });

                    context.Devices.Add(existing);

                    var result = await context.TrySaveChangesAsync(CancellationToken);
                    if (result.HasError)
                        await
                            Log.ReportErrorFormatAsync(CancellationToken, "Failed to save new device. {0}",
                                result.Message);

                    await Log.ReportInfoAsync(string.Format("{0} New Controller added to the database, IP:{1}", Name, ipAddress), CancellationToken);
                }
            }
        }

    }
}