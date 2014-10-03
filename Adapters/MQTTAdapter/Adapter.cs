using System;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.ComponentModel;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.Script.Serialization;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using zvs.Processor;
using zvs.Entities;
using System.Threading.Tasks;

namespace MQTTAdapter
{
    [Export(typeof (zvsAdapter))]
    public class Adapter : zvsAdapter
    {
        private async void Adapter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (IsEnabled)
            {
            }
        }

        public override Guid AdapterGuid
        {
            get { return Guid.Parse("E1D546C1-D729-430C-9D2A-71F4B9BCAF09"); }
        }

        public override string Name
        {
            get { return "MQTT Adapter for ZVS"; }
        }

        public override string Description
        {
            get
            {
                return
                    "This adapter allows you to connect zVirtualScenes to a MQTT 3.1 Compliant server.  zVirtualScenes will automatically subscribe to (and capture data from) all topics on the MQTT server.  It will also automatically register topics as nodes (devices)\r\n\r\nIf you need an MQTT Server, please consider: http://mosquitto.org/download/";
            }
        }

        public override async Task OnDeviceTypesCreating(DeviceTypeBuilder deviceTypeBuilder)
        {
            //Sensors
            DeviceType sensor_dt = new DeviceType
                {
                    UniqueIdentifier = "MQTT Device",
                    Name = "MQTT Device",
                    ShowInList = true
                };
            SensorTypeId = await deviceTypeBuilder.RegisterAsync(sensor_dt);
        }

        private int SensorTypeId = -1;


        private Task Publish(DeviceType deviceType, Device device, DeviceTypeCommand deviceTypeCommand,
                             DeviceCommand deviceCommand, string argument)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            var o =
                new
                    {
                        DeviceName = device.Name,
                        DeviceCommandName = (deviceCommand == null ? null : deviceCommand.Name),
                        DeviceTypeCommandName = (deviceTypeCommand == null ? null : deviceTypeCommand.Name),
                        DeviceTypeCommandValue = (deviceTypeCommand == null ? null : deviceTypeCommand.Value),
                        Argument = argument,
                        DeviceTypeName = (device == null || device.Type == null ? null : device.Type.Name),
                        device.CurrentLevelInt,
                        device.CurrentLevelText
                    };
            var str = js.Serialize(o);
            client.Publish(SystemTopic, Encoding.UTF8.GetBytes(str));
            return Task.FromResult(0);
        }

        public override Task ProcessDeviceTypeCommandAsync(DeviceType deviceType, Device device, DeviceTypeCommand command, string argument)
        {
            return Publish(deviceType, device, command, null, argument);
           
        }

        public override Task ProcessDeviceCommandAsync(Device device, DeviceCommand command, string argument,
                                                       string argument2)
        {
            return Publish(device.Type, device, null, command, argument);

        }

        public override async Task OnSettingsCreating(AdapterSettingBuilder settingBuilder)
        {
            var hostSetting = new AdapterSetting
                {
                    Name = "Host",
                    Value = "127.0.0.1",
                    ValueType = DataType.STRING,
                    Description = "The host machine which your MQTT server is installed."
                };
            var portSetting = new AdapterSetting
                {
                    Name = "Port",
                    Value = "1883",
                    ValueType = DataType.INTEGER,
                    Description =
                        "The port on host machine which your MQTT server is installed. (1883 - default, 8883 - SSL)"
                };

            await settingBuilder.Adapter(this).RegisterAdapterSettingAsync(hostSetting, o => o.HostSetting);
            await settingBuilder.Adapter(this).RegisterAdapterSettingAsync(portSetting, o => o.PortSetting);

        }

        public override async Task StartAsync()
        {
            await StartAdapter();
        }

        public override async Task StopAsync()
        {
            await StopAdapter();
        }

        private string _PortSetting = "1883";

        public string PortSetting
        {
            get { return _PortSetting; }
            set
            {
                if (value != _PortSetting)
                {
                    _PortSetting = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _HostSetting = "127.0.0.1";

        public string HostSetting
        {
            get { return _HostSetting; }
            set
            {
                if (value != _HostSetting)
                {
                    _HostSetting = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private zvs.Processor.Logging.ILog log = zvs.Processor.Logging.LogManager.GetLogger<Adapter>();
        private bool isShuttingDown = false;

        private MqttClient client;

        private string SystemTopic = string.Format("{0}/zVirtualScenes", System.Environment.MachineName);
        private Task StartAdapter()
        {
            if (isShuttingDown)
            {
                Core.log.InfoFormat("{0} driver cannot start because it is still shutting down", this.Name);
                return Task.FromResult(0);
            }

            this.PropertyChanged += Adapter_PropertyChanged;
            try
            {

                return Task.Run(() =>
                    {
                        try
                        {
                            //Task.Delay(5000);
                            int port = 1883;
                            int.TryParse(PortSetting, out port);
                            IPAddress address = IPAddress.Loopback;
                            IPAddress.TryParse(HostSetting, out address);
                            if (port != 1883)
                            {
                                client = new MqttClient(address, port,false,new X509Certificate());
                            }
                            else
                            {
                                client = new MqttClient(address);
                            }
                            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
                            client.Connect(SystemTopic);
                            client.Subscribe(new string[] {"#"}, new byte[] {0});
                            Core.log.InfoFormat("{0} connected and subscribed!", this.Name);
                            AddMQTTAdDevice();
                        }
                        catch (Exception e)
                        {
                            log.Fatal("Could not connect the MQTT Server!", e);
                            log.Info("If you need a MQTT Server, consider:  http://mosquitto.org/download/");
                        }

                    });

            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return Task.FromResult(0);
            }

        }

        private void client_MqttMsgPublishReceived(object sender,
                                                   uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {
            AddSensor(e);
        }

        private async Task StopAdapter()
        {
            if (!isShuttingDown)
            {
                this.PropertyChanged -= Adapter_PropertyChanged;

                isShuttingDown = true;

                await Task.Run(() =>
                    {
                        if (client != null && client.IsConnected) client.Disconnect();
                    });

                isShuttingDown = false;
                Core.log.InfoFormat("{0} driver stopped", this.Name);
            }
        }

        public override Task RepollAsync(Device device, zvsContext context)
        {

            //m_manager.RequestNodeState(m_homeId, nodeNumber);
            return Task.FromResult(0);
        }

        public override Task ActivateGroupAsync(Group @group, zvsContext context)
        {
            //throw new NotImplementedException();
            return null;
        }

        public override Task DeactivateGroupAsync(Group @group, zvsContext context)
        {
            //throw new NotImplementedException();
            return null;
        }

        private DeviceCommand motionCommand;

        private void AddCommand(int deviceID, string name, zvsContext context)
        {
            context.DeviceCommands.FirstOrDefaultAsync(d => d.Name == name && d.DeviceId == deviceID).ContinueWith(
                t =>
                    {
                        if (motionCommand != null)
                            return;

                        motionCommand =
                            context.DeviceCommands.Add(new DeviceCommand()
                                {
                                    DeviceId = deviceID,
                                    Description = name,
                                    Name = name,
                                    UniqueIdentifier = Guid.NewGuid().ToString(),
                                    ArgumentType = DataType.STRING,
                                    Help = name,
                                    CustomData1 = "Value",
                                    CustomData2 = name,
                                    SortOrder = 0
                                
                                });

                        context.TrySaveChangesAsync().ContinueWith(tt =>
                            {
                                if (tt.Result.HasError)
                                    Core.log.Error(tt.Result.Message);

                            }).Wait();
                    }).Wait();

        }

        private async Task AddOrUpdateValue(string name, string value, int deviceId, DataType dataType, string valueName,
                                            string genre, zvsContext context)
        {
            context.DeviceValues.FirstOrDefaultAsync(d => d.DeviceId == deviceId && d.Name == valueName)
                   .ContinueWith(t
                                 =>
                       {
                           DeviceValue dv = t.Result;
                           if (dv == null)
                           {
                               dv = new DeviceValue
                                   {
                                       DeviceId = deviceId,
                                       Name = valueName,
                                       ValueType = dataType,
                                       Genre = genre
                                   };

                               context.DeviceValues.Add(dv);
                           }

                           dv.Value = value;

                           context.TrySaveChangesAsync().ContinueWith(tt =>
                               {
                                   if (tt.Result.HasError)
                                       Core.log.Error(tt.Result.Message);

                               }).Wait();

                       }).Wait();


        }


        //private System.Random rnd = new Random();
        private static object sensor_lock = new object();

        private async Task AddMQTTAdDevice()
        {
            using(zvsContext context = new zvsContext())
                {


                    context.Devices.FirstOrDefaultAsync(
                        d => d.Type.Adapter.AdapterGuid == this.AdapterGuid && d.Name == SystemTopic).ContinueWith(t =>
                            {
                                if (t.Result == null)
                                {
                                    log.Info("MQTT broker not found, registering.");
                                    Device dev = new Device
                                        {
                                            //NodeNumber = rnd.Next((int) byte.MinValue + 100, (int) byte.MaxValue),
                                            DeviceTypeId = SensorTypeId,
                                            Name = SystemTopic,
                                        };

                                    //dev.Commands.Add(motionCommand);
                                    context.Devices.Add(dev);

                                    context.TrySaveChangesAsync().ContinueWith(tt =>
                                        {
                                            if (tt.Result.HasError)
                                                Core.log.Error(tt.Result.Message);

                                            AddCommand(dev.Id, "Publish to Broker", context);
                                        }).Wait();
                                }

                            }).Wait();

                }
        }

        private async Task AddSensor(MqttMsgPublishEventArgs e)
        {
            var name = e.Topic;
            var value = System.Text.UTF8Encoding.UTF8.GetString(e.Message);

            using (zvsContext context = new zvsContext())
            {

                context.Devices
                       .FirstOrDefaultAsync(
                           d => d.Type.Adapter.AdapterGuid == this.AdapterGuid &&
                                d.Name == name).ContinueWith(t =>
                                    {
                                        Device dev = t.Result;
                                        if (dev == null)
                                        {
                                            log.Info("New MQTT device found, registering:" + name);
                                            dev = new Device
                                                {
                                                    ////NodeNumber =
                                                    ////    rnd.Next((int) byte.MinValue + 100, (int) byte.MaxValue),
                                                    DeviceTypeId = SensorTypeId,
                                                    Name = name,
                                                    CurrentLevelInt = 0,
                                                    CurrentLevelText = value
                                                };

                                            //dev.Commands.Add(motionCommand);
                                            context.Devices.Add(dev);

                                            context.TrySaveChangesAsync().ContinueWith(tt =>
                                                {
                                                    if (tt.Result.HasError)
                                                        Core.log.Error(tt.Result.Message);

                                                }).Wait();
                                        }

                                        AddOrUpdateValue(name, System.DateTime.Now.ToString(), dev.Id, DataType.STRING,
                                                         "Date", "Audit", context);
                                        AddOrUpdateValue(name, value, dev.Id, DataType.STRING, "Value", "MQTT", context);
                                        AddOrUpdateValue(name, e.QosLevel.ToString(), dev.Id, DataType.BYTE, "QosLevel",
                                                         "MQTT", context);
                                        AddOrUpdateValue(name, e.Topic, dev.Id, DataType.STRING, "Topic", "MQTT",
                                                         context);
                                        AddOrUpdateValue(name, e.Retain.ToString(), dev.Id, DataType.BOOL, "Retain",
                                                         "MQTT",
                                                         context);

                                    }).Wait();

            }

        }

    }

}