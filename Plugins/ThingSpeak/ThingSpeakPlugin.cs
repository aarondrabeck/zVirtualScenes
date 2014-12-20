using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Threading.Tasks;
using ThingSpeak.Client;
using zvs;
using zvs.DataModel;
using zvs.Processor;

namespace ThingSpeak
{
    [Export(typeof(ZvsPlugin))]
    public class ThingSpeakPlugin : ZvsPlugin
    {
        private ThingSpeakClient ThingSpeakClient { get; set; }

        public override Guid PluginGuid
        {
            get { return Guid.Parse("14539d4e-6ac3-42cb-a930-172a1ec73db8"); }
        }

        public override string Name
        {
            get { return "Thing Speak Plugin for ZVS"; }
        }

        public override string Description
        {
            get { return "This plug-in will upload device changes to https://thingspeak.com."; }
        }

        public string ApiKey { get; set; }
        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public string Field3 { get; set; }
        public string Field4 { get; set; }
        public string Field5 { get; set; }
        public string Field6 { get; set; }
        public string Field7 { get; set; }
        public string Field8 { get; set; }

        public override async Task OnSettingsCreating(PluginSettingBuilder settingBuilder)
        {
            var apiKey = new PluginSetting
            {
                UniqueIdentifier = "APIWRITEKEY",
                Name = "Write API Key",
                Value = "",
                ValueType = DataType.STRING,
                Description = "Write API Key from your channel on https://thingspeak.com/"
            };
            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(apiKey, o => o.ApiKey);

            var feild1 = new PluginSetting
            {
                UniqueIdentifier = "FIELD1",
                Name = "Field1 Device",
                Value = "",
                ValueType = DataType.STRING,
                Description = "The Value will be which zVirtual Device name you want mapped to ThingSpeak's field"
            };
            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(feild1, o => o.Field1);

            var feild2 = new PluginSetting
            {
                UniqueIdentifier = "FIELD2",
                Name = "Field2 Device",
                Value = "",
                ValueType = DataType.STRING,
                Description = "The Value will be which zVirtual Device name you want mapped to ThingSpeak's field"
            };
            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(feild2, o => o.Field2);

            var feild3 = new PluginSetting
            {
                UniqueIdentifier = "FIELD3",
                Name = "Field3 Device",
                Value = "",
                ValueType = DataType.STRING,
                Description = "The Value will be which zVirtual Device name you want mapped to ThingSpeak's field"
            };
            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(feild3, o => o.Field3);

            var feild4 = new PluginSetting
            {
                UniqueIdentifier = "FIELD4",
                Name = "Field4 Device",
                Value = "",
                ValueType = DataType.STRING,
                Description = "The Value will be which zVirtual Device name you want mapped to ThingSpeak's field"
            };
            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(feild4, o => o.Field4);

            var feild5 = new PluginSetting
            {
                UniqueIdentifier = "FIELD5",
                Name = "Field5 Device",
                Value = "",
                ValueType = DataType.STRING,
                Description = "The Value will be which zVirtual Device name you want mapped to ThingSpeak's field"
            };
            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(feild5, o => o.Field5);

            var feild6 = new PluginSetting
            {
                UniqueIdentifier = "FIELD6",
                Name = "Field6 Device",
                Value = "",
                ValueType = DataType.STRING,
                Description = "The Value will be which zVirtual Device name you want mapped to ThingSpeak's field"
            };
            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(feild6, o => o.Field6);

            var feild7 = new PluginSetting
            {
                UniqueIdentifier = "FIELD7",
                Name = "Field7 Device",
                Value = "",
                ValueType = DataType.STRING,
                Description = "The Value will be which zVirtual Device name you want mapped to ThingSpeak's field"
            };
            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(feild7, o => o.Field7);

            var feild8 = new PluginSetting
            {
                UniqueIdentifier = "FIELD8",
                Name = "Field8 Device",
                Value = "",
                ValueType = DataType.STRING,
                Description = "The Value will be which zVirtual Device name you want mapped to ThingSpeak's field"
            };
            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(feild8, o => o.Field8);
        }

        public override async Task StartAsync()
        {
            if (string.IsNullOrWhiteSpace(ApiKey))
            {
                await Log.ReportWarningFormatAsync(CancellationToken, "Could not start {0}, missing API key ", Name);
                return;
            }

            ThingSpeakClient = new ThingSpeakClient(ApiKey);
            NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.OnEntityUpdated += ChangeNotificationsOnOnEntityUpdated;
            await Log.ReportInfoFormatAsync(CancellationToken, "{0} started", Name);
        }

        public override async Task StopAsync()
        {
            NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.OnEntityUpdated -= ChangeNotificationsOnOnEntityUpdated;
            await Log.ReportInfoFormatAsync(CancellationToken, "{0} stopped", Name);
        }

        private void ChangeNotificationsOnOnEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.EntityUpdatedArgs entityUpdatedArgs)
        {
            var bw = new BackgroundWorker();
            bw.DoWork += async (s, a) =>
            {
                try
                {
                    await Log.ReportInfoFormatAsync(CancellationToken, "{0} working!", Name);
                    using (var context = new ZvsContext(EntityContextConnection))
                    {
                        var dv = await context.DeviceValues
                            .FirstOrDefaultAsync(v => v.Id == entityUpdatedArgs.NewEntity.Id, CancellationToken);

                        await Log.ReportInfoFormatAsync(CancellationToken, "DeviceValueId : {1}, new:{2}, old:{3}, dv.Value:{4}", (dv != null && dv.Value != null), entityUpdatedArgs.NewEntity.Id, entityUpdatedArgs.NewEntity.Value, entityUpdatedArgs.OldEntity.Value);
                        if (dv == null || dv.Value == null) return;

                        var name = dv.Device.Name;
                        await Log.ReportInfoFormatAsync(CancellationToken, "[{0}] value name: [{1}], Value: [{2}]", Name, name, dv.Value);
                        if (name == Field1)
                        {
                            await Log.ReportInfoFormatAsync(CancellationToken, "Sending {0} to ThingSpeak as Field1, Value={1}", name, dv.Value);
                            short response;
                            var success = ThingSpeakClient.SendDataToThingSpeak(out response, dv.Value);
                            await Log.ReportInfoFormatAsync(CancellationToken, "ThingSpeak results ({0}): success:{1}, response:{2}", name, success, response);

                        }
                        if (name == Field2)
                        {
                            await Log.ReportInfoFormatAsync(CancellationToken, "Sending {0} to ThingSpeak as Field2, Value={1}", name, dv.Value);
                            short response;
                            var success = ThingSpeakClient.SendDataToThingSpeak(out response, null, dv.Value);
                            await Log.ReportInfoFormatAsync(CancellationToken, "ThingSpeak results ({0}): success:{1}, response:{2}", name, success, response);
                        }
                        if (name == Field3)
                        {
                            await Log.ReportInfoFormatAsync(CancellationToken, "Sending {0} to ThingSpeak as Field3, Value={1}", name, dv.Value);
                            short response;
                            var success = ThingSpeakClient.SendDataToThingSpeak(out response, null, null, dv.Value);
                            await Log.ReportInfoFormatAsync(CancellationToken, "ThingSpeak results ({0}): success:{1}, response:{2}", name, success, response);
                        }
                        if (name == Field4)
                        {
                            await Log.ReportInfoFormatAsync(CancellationToken, "Sending {0} to ThingSpeak as Field4, Value={1}", name, dv.Value);
                            short response;
                            var success = ThingSpeakClient.SendDataToThingSpeak(out response, null, null, null, dv.Value);
                            await Log.ReportInfoFormatAsync(CancellationToken, "ThingSpeak results ({0}): success:{1}, response:{2}", name, success, response);
                        }
                        if (name == Field5)
                        {
                            await Log.ReportInfoFormatAsync(CancellationToken, "Sending {0} to ThingSpeak as Field5, Value={1}", name, dv.Value);
                            short response;
                            var success = ThingSpeakClient.SendDataToThingSpeak(out response, null, null, null, null, dv.Value);
                            await Log.ReportInfoFormatAsync(CancellationToken, "ThingSpeak results ({0}): success:{1}, response:{2}", name, success, response);
                        }
                        if (name == Field6)
                        {
                            await Log.ReportInfoFormatAsync(CancellationToken, "Sending {0} to ThingSpeak as Field6, Value={1}", name, dv.Value);
                            short response;
                            var success = ThingSpeakClient.SendDataToThingSpeak(out response, null, null, null, null, null, dv.Value);
                            await Log.ReportInfoFormatAsync(CancellationToken, "ThingSpeak results ({0}): success:{1}, response:{2}", name, success, response);
                        }
                        if (name == Field7)
                        {
                            await Log.ReportInfoFormatAsync(CancellationToken, "Sending {0} to ThingSpeak as Field7, Value={1}", name, dv.Value);
                            short response;
                            var success = ThingSpeakClient.SendDataToThingSpeak(out response, null, null, null, null, null, null, dv.Value);
                            await Log.ReportInfoFormatAsync(CancellationToken, "ThingSpeak results ({0}): success:{1}, response:{2}", name, success, response);
                        }
                        if (name == Field8)
                        {
                            await Log.ReportInfoFormatAsync(CancellationToken, "Sending {0} to ThingSpeak as Field8, Value={1}", name, dv.Value);
                            short response;
                            var success = ThingSpeakClient.SendDataToThingSpeak(out response, null, null, null, null, null, null, null, dv.Value);
                            await Log.ReportInfoFormatAsync(CancellationToken, "ThingSpeak results ({0}): success:{1}, response:{2}", name, success, response);
                        }
                    }

                }
                catch (Exception e)
                {
                    Log.ReportErrorAsync(e.Message, CancellationToken).Wait();
                }

            };
            bw.RunWorkerAsync();
        }
    }
}