using System;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Threading.Tasks;
using Newtonsoft.Json;
using uPLibrary.Networking.M2Mqtt.Messages;
using zvs;
using zvs.DataModel;
using zvs.Processor;

namespace MQTTPlugin
{
    [Export(typeof(ZvsPlugin))]
    public class MqttPlugin : ZvsPlugin, IDisposable
    {
        public override Guid PluginGuid
        {
            get { return Guid.Parse("202C4161-1F92-44DD-8794-8C83C1C29DFD"); }
        }
        public override string Name
        {
            get { return "MQTT Plug-in for ZVS"; }
        }

        public override string Description
        {
            get { return "This plug-in will publish any device changes to the specified."; }
        }

        private string _hostSetting = "";
        public string HostSetting
        {
            get { return _hostSetting; }
            set
            {
                if (value == _hostSetting) return;
                _hostSetting = value;
                NotifyPropertyChanged();
            }
        }

        private int _port = 1833;
        public int Port
        {
            get { return _port; }
            set
            {
                if (value == _port) return;
                _port = value;
                NotifyPropertyChanged();
            }
        }

        private string _userName = "";
        public string UserName
        {
            get { return _userName; }
            set
            {
                if (value == _userName) return;
                _userName = value;
                NotifyPropertyChanged();
            }
        }

        private string _password = "";
        public string Password
        {
            get { return _password; }
            set
            {
                if (value == _password) return;
                _password = value;
                NotifyPropertyChanged();
            }
        }
        private string _topicFormat = "/devices/{Device.Id}/events";
        public string TopicFormat
        {
            get { return _topicFormat; }
            set
            {
                if (value == _topicFormat) return;
                _topicFormat = value;
                NotifyPropertyChanged();
            }
        }
        private string _floodTopic = "/devices/flood/";
        public string FloodTopic
        {
            get { return _floodTopic; }
            set
            {
                if (value == _floodTopic) return;
                _floodTopic = value;
                NotifyPropertyChanged();
            }
        }
        public override async Task OnSettingsCreating(PluginSettingBuilder settingBuilder)
        {
            var annouceoptionssetting = new PluginSetting
            {
                Name = "Host",
                Value = "mqtt.myserver.com",
                ValueType = DataType.STRING,
                Description = "Enter the Host name for the broker."
            };

            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(annouceoptionssetting, o => o.HostSetting);


            var portSetting = new PluginSetting
            {
                Name = "Port",
                Value = "1883",
                ValueType = DataType.INTEGER,
                Description = "Enter the port for the broker."
            };

            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(portSetting, o => o.Port);


            var userNameSetting = new PluginSetting
            {
                Name = "Username",
                Value = "admin",
                ValueType = DataType.STRING,
                Description = "Enter the username for the broker."
            };

            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(userNameSetting, o => o.UserName);


            var passwordSetting = new PluginSetting
            {
                Name = "Password",
                Value = "brokerpassword",
                ValueType = DataType.STRING,
                Description = "Enter the password for the broker."
            };

            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(passwordSetting, o => o.Password);


            var topicSetting = new PluginSetting
            {
                Name = "Topic",
                Value = "/devices/{Device.Id}/",
                ValueType = DataType.STRING,
                Description = "Enter the topic for changes to be published to, on the broker."
            };

            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(topicSetting, o => o.TopicFormat);



            var floodTopicSetting = new PluginSetting
            {
                Name = "Flood Topic",
                Value = "/devices/flood/",
                ValueType = DataType.STRING,
                Description = "Enter the topic name for all messages to get pushed to."
            };

            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(floodTopicSetting, o => o.FloodTopic);


        }

        private Charlotte.MQTTConnection mqtt = null;
        private bool enabled = false;

        public override async Task StartAsync()
        {
            try
            {
                mqtt = new Charlotte.MQTTConnection(HostSetting, Port, UserName, Password);
                mqtt.Connect();
                enabled = true;
                await Publish(FloodTopic, "zVirtualScenes Connected");
            }
            catch (Exception e)
            {
                Log.ReportErrorAsync(e.Message, CancellationToken);
            }
            await Log.ReportInfoFormatAsync(CancellationToken, "{0} started", Name);
            NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.OnEntityUpdated += Plugin_OnEntityUpdated;
        }


        public override async Task StopAsync()
        {
            NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.OnEntityUpdated -= Plugin_OnEntityUpdated;
            try
            {
                await Publish(FloodTopic, "zVirtualScenes Disconnecting");

                mqtt.Disconnect();
                enabled = false;

                await Log.ReportInfoFormatAsync(CancellationToken, "{0} stopped", Name);
           
            }
            catch (Exception)
            {
            }
            
        }

        private async void Plugin_OnEntityUpdated(object sender,
            NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.EntityUpdatedArgs e)
        {
            if (enabled)
            {
                try
                {
                    var newId = e.NewEntity.Id;
                    using (var context = new ZvsContext(EntityContextConnection))
                    {
                        var dv = await context.DeviceValues
                            .Include(o => o.Device.Type)
                            .FirstOrDefaultAsync(v => v.Id == newId, CancellationToken);

                        if (dv == null)
                            return;

                        var device = e.NewEntity.Device;

                        var topic = this.TopicFormat;
                        topic = topic.Replace("{Device.Id}", device.Id.ToString());
                        topic = topic.Replace("{Device.Name}", device.Name);
                        topic = topic.Replace("{Device.NodeNumber}", device.NodeNumber.ToString());



                        var message = new DeviceMessage()
                        {
                            DeviceId = device.Id,
                            DeviceName = device.Name,
                            NodeNumber = device.NodeNumber,
                            PropertyName = dv.Name,
                            PropertyType = dv.ValueType.ToString(),
                            PropertyValue = dv.Value
                        };
                        var msg = Newtonsoft.Json.JsonConvert.SerializeObject(message);
                        await Publish(topic, msg);

                    }

                }
                catch (Exception exc)
                {
                    Log.ReportInfoAsync(exc.Message, CancellationToken);

                    
                }
            }
        }

        public async Task Publish(string topic, string message)
        {
            if (enabled)
            {
                await Log.ReportInfoFormatAsync(CancellationToken, "MQTT, Publishing Topic:{0}, Message:{1}", topic, message);
                mqtt.Publish(topic, message, MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, true);

                if (!string.IsNullOrEmpty(FloodTopic) && FloodTopic != topic)
                {
                    mqtt.Publish(FloodTopic, message, MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, true);
                    
                }
                await Log.ReportInfoFormatAsync(CancellationToken, "MQTT, Published Topic:{0}, Message:{1}", topic, message);

            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
        }
    }
}
