using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using zvs;
using zvs.DataModel;
using zvs.Processor;

namespace MiLightAdapter
{

    /// <summary>
    /// This plugin will connect up to 4 milight wifi controllers, each of which have 4 scene buttons
    /// each button will appear as a node in ZVS
    /// This means you will see 4 nodes per wifi controller
    /// </summary>
    [Export(typeof (ZvsAdapter))]
    public class Adapter : ZvsAdapter
    {
        private Controller controller = new Controller();

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
            await Log.ReportInfoAsync("Started", CancellationToken);
            await AddNewWifiControllerToDatabase(WiFi1Setting);
            controller.AddController(WiFi1Setting);
            PropertyChanged += Adapter_PropertyChanged;
        }

        private void Adapter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "WiFi1Setting")
            {


            }
        }

        public override async Task StopAsync()
        {
            await Log.ReportInfoAsync("Stopped", CancellationToken);
        }

        public override async Task ProcessDeviceTypeCommandAsync(DeviceType deviceType, Device device,
            DeviceTypeCommand command, string argument)
        {

            var miCommand = command.CustomData1;
            var zone = command.CustomData2;
            var ip = (from d in device.Values where d.Name == "IPAddress" select d.Value).FirstOrDefault();

            controller.Send(ip, miCommand, zone);

            await Log.ReportInfoAsync("ProcessDeviceTypeCommandAsync", CancellationToken);
        }

        public override async Task ProcessDeviceCommandAsync(Device device, DeviceCommand command, string argument,
            string argument2)
        {
            await Log.ReportInfoAsync("ProcessDeviceCommandAsync", CancellationToken);
        }

        public override async Task RepollAsync(Device device)
        {
            await Log.ReportInfoAsync("RepollAsync", CancellationToken);
        }

        public override async Task ActivateGroupAsync(Group @group)
        {
            await Log.ReportInfoAsync("ActivateGroupAsync", CancellationToken);
        }

        public override async Task DeactivateGroupAsync(Group @group)
        {
            await Log.ReportInfoAsync("DeactivateGroupAsync", CancellationToken);
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


        public override async Task OnSettingsCreating(AdapterSettingBuilder settingBuilder)
        {
            var wifi1IPSetting = new AdapterSetting
            {
                Name = "WiFi Controller 1 IP Address",
                Value = "",
                ValueType = DataType.STRING,
                Description = "The IP Address of the first WiFi Controller."
            };

            var wifi1SettingResult =
                await settingBuilder.Adapter(this).RegisterAdapterSettingAsync(wifi1IPSetting, o => o.WiFi1Setting);

            if (wifi1SettingResult.HasError)
                await
                    Log.ReportErrorFormatAsync(CancellationToken,
                        "An error occured when registering the wifi controller 1 setting. {0}",
                        wifi1SettingResult.Message);

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
                UniqueIdentifier = "Z1TURNOFF",
                Name = "Zone 2, Turn Off",
                CustomData1 = "Off",
                CustomData2 = "Two",
                ArgumentType = DataType.NONE,
                Description = "Turns Zone 2 Off."
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


                }


            }
        }

    }
}