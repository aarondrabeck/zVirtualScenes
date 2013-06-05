using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zvs.Entities;
using zvs.Processor;

namespace ThingSpeak
{
    [Export(typeof(zvsPlugin))]
    public class ThingSpeakPlugin : zvsPlugin
    {
        ThingSpeakClient client = new ThingSpeakClient();
        zvs.Processor.Logging.ILog log = zvs.Processor.Logging.LogManager.GetLogger<ThingSpeakPlugin>();

        public ThingSpeakPlugin()
            : base("THINGSPEAK",
               "Thing Speak Plug-in",
                "This plug-in will upload device changes to https://thingspeak.com."
                ) { }

        public override void Initialize()
        {
            using (zvsContext Context = new zvsContext())
            {
                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "APIWRITEKEY",
                    Name = "Write API Key",
                    Value = "",
                    ValueType = DataType.STRING,
                    Description = "Write API Key from your channel on https://thingspeak.com/"
                }, Context);
                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "FIELD1",
                    Name = "Field1 Device",
                    Value = "",
                    ValueType = DataType.STRING,
                    Description = "The Value will be which zVirtual Device name you want mapped to ThingSpeak's field"
                }, Context);
                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "FIELD2",
                    Name = "Field2 Device",
                    Value = "",
                    ValueType = DataType.STRING,
                    Description = "The Value will be which zVirtual Device name you want mapped to ThingSpeak's field"
                }, Context);
                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "FIELD3",
                    Name = "Field3 Device",
                    Value = "",
                    ValueType = DataType.STRING,
                    Description = "The Value will be which zVirtual Device name you want mapped to ThingSpeak's field"
                }, Context);
                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "FIELD4",
                    Name = "Field4 Device",
                    Value = "",
                    ValueType = DataType.STRING,
                    Description = "The Value will be which zVirtual Device name you want mapped to ThingSpeak's field"
                }, Context);
                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "FIELD5",
                    Name = "Field5 Device",
                    Value = "",
                    ValueType = DataType.STRING,
                    Description = "The Value will be which zVirtual Device name you want mapped to ThingSpeak's field"
                }, Context);
                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "FIELD6",
                    Name = "Field6 Device",
                    Value = "",
                    ValueType = DataType.STRING,
                    Description = "The Value will be which zVirtual Device name you want mapped to ThingSpeak's field"
                }, Context);
                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "FIELD7",
                    Name = "Field7 Device",
                    Value = "",
                    ValueType = DataType.STRING,
                    Description = "The Value will be which zVirtual Device name you want mapped to ThingSpeak's field"
                }, Context);
                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "FIELD8",
                    Name = "Field8 Device",
                    Value = "",
                    ValueType = DataType.STRING,
                    Description = "The Value will be which zVirtual Device name you want mapped to ThingSpeak's field"
                }, Context);

                client.APIKey = GetSettingValue("APIWRITEKEY", Context);
            }
        }
        protected override void StartPlugin()
        {
            log.InfoFormat("Starting {0} ", Name);
            if (!string.IsNullOrEmpty(client.APIKey))
            {
                DeviceValue.DeviceValueDataChangedEvent += DeviceValue_DeviceValueDataChangedEvent;
                IsReady = true;
                log.InfoFormat("Started {0} ", Name);
            }
            else
            {
                log.InfoFormat("Did not start {0}, missing API key ", Name);
            }
        }

        protected override void StopPlugin()
        {
            log.InfoFormat("Stopping {0} ", Name);
            IsReady = false;
            DeviceValue.DeviceValueDataChangedEvent -= DeviceValue_DeviceValueDataChangedEvent;

        }
        protected override void SettingChanged(string settingsettingUniqueIdentifier, string settingValue) { }

        public override void ProcessCommand(int queuedCommandId) { }

        public override void Repoll(zvs.Entities.Device device) { }

        public override void ActivateGroup(int groupID) { }

        public override void DeactivateGroup(int groupID) { }

        private void DeviceValue_DeviceValueDataChangedEvent(object sender, DeviceValue.ValueDataChangedEventArgs args)
        {
            if (IsReady)
            {
                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += (s, a) =>
                {
                    try
                    {
                        log.DebugFormat("{0} working!", Name);
                        using (zvsContext context = new zvsContext())
                        {
                            DeviceValue dv = context.DeviceValues.FirstOrDefault(v => v.Id == args.DeviceValueId);
                            log.DebugFormat("DeviceValueId : {1}, new:{2}, old:{3}, dv.Value:{4}, dv:{0}", (dv != null && dv.Value != null), args.DeviceValueId, args.newValue, args.oldValue, dv.Value);
                            if (dv != null && dv.Value != null)
                            {

                                var name = dv.Device.Name;
                                log.DebugFormat("[{0}] value name: [{1}], Value: [{2}]", Name, name, dv.Value);
                                if (name == GetSettingValue("FIELD1", context))
                                {
                                    log.DebugFormat("Sending {0} to ThingSpeak as Field1, Value={1}", name, dv.Value);
                                    short response = 0;
                                    var success = client.SendDataToThingSpeak(out response, new string[] { dv.Value });
                                    log.DebugFormat("ThingSpeak results ({0}): success:{1}, response:{2}", name, success, response);

                                }
                                if (name == GetSettingValue("FIELD2", context))
                                {
                                    log.DebugFormat("Sending {0} to ThingSpeak as Field2, Value={1}", name, dv.Value);
                                    short response = 0;
                                    var success = client.SendDataToThingSpeak(out response, new string[] { null, dv.Value });
                                    log.DebugFormat("ThingSpeak results ({0}): success:{1}, response:{2}", name, success, response);
                                }
                                if (name == GetSettingValue("FIELD3", context))
                                {
                                    log.DebugFormat("Sending {0} to ThingSpeak as Field3, Value={1}", name, dv.Value);
                                    short response = 0;
                                    var success = client.SendDataToThingSpeak(out response, new string[] { null, null, dv.Value });
                                    log.DebugFormat("ThingSpeak results ({0}): success:{1}, response:{2}", name, success, response);
                                }
                                if (name == GetSettingValue("FIELD4", context))
                                {
                                    log.DebugFormat("Sending {0} to ThingSpeak as Field4, Value={1}", name, dv.Value);
                                    short response = 0;
                                    var success = client.SendDataToThingSpeak(out response, new string[] { null, null, null, dv.Value });
                                    log.DebugFormat("ThingSpeak results ({0}): success:{1}, response:{2}", name, success, response);
                                }
                                if (name == GetSettingValue("FIELD5", context))
                                {
                                    log.DebugFormat("Sending {0} to ThingSpeak as Field5, Value={1}", name, dv.Value);
                                    short response = 0;
                                    var success = client.SendDataToThingSpeak(out response, new string[] { null, null, null, null, dv.Value });
                                    log.DebugFormat("ThingSpeak results ({0}): success:{1}, response:{2}", name, success, response);
                                }
                                if (name == GetSettingValue("FIELD6", context))
                                {
                                    log.DebugFormat("Sending {0} to ThingSpeak as Field6, Value={1}", name, dv.Value);
                                    short response = 0;
                                    var success = client.SendDataToThingSpeak(out response, new string[] { null, null, null, null, null, dv.Value });
                                    log.DebugFormat("ThingSpeak results ({0}): success:{1}, response:{2}", name, success, response);
                                }
                                if (name == GetSettingValue("FIELD7", context))
                                {
                                    log.DebugFormat("Sending {0} to ThingSpeak as Field7, Value={1}", name, dv.Value);
                                    short response = 0;
                                    var success = client.SendDataToThingSpeak(out response, new string[] { null, null, null, null, null, null, dv.Value });
                                    log.DebugFormat("ThingSpeak results ({0}): success:{1}, response:{2}", name, success, response);
                                }
                                if (name == GetSettingValue("FIELD8", context))
                                {
                                    log.DebugFormat("Sending {0} to ThingSpeak as Field8, Value={1}", name, dv.Value);
                                    short response = 0;
                                    var success = client.SendDataToThingSpeak(out response, new string[] { null, null, null, null, null, null, null, dv.Value });
                                    log.DebugFormat("ThingSpeak results ({0}): success:{1}, response:{2}", name, success, response);
                                }
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        log.Error(e);
                    }

                };
                bw.RunWorkerAsync();
            }
        }
    }
}