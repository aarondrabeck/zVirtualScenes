using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using uPLibrary.Networking.M2Mqtt;
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
        private string _topicFormat = "zvirtual/{NodeNumber}/events";
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

        private string _controlTopicFormat = "zvirtual/control";
        public string ControlTopicFormat
        {
            get { return _controlTopicFormat; }
            set
            {
                if (value == _controlTopicFormat) return;
                _controlTopicFormat = value;
                NotifyPropertyChanged();
            }
        }
        private string _floodTopic = "zvirtual/events";
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
                Value = "zvirtual/{NodeNumber}/events",
                ValueType = DataType.STRING,
                Description = "Enter the topic for changes to be published to, on the broker."
            };

            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(topicSetting, o => o.TopicFormat);



            var floodTopicSetting = new PluginSetting
            {
                Name = "Flood Topic",
                Value = "zvirtual/events",
                ValueType = DataType.STRING,
                Description = "Enter the topic name for all messages to get pushed to."
            };

            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(floodTopicSetting, o => o.FloodTopic);


            var controlTopicSetting = new PluginSetting
            {
                Name = "Device Control Topic",
                Value = "zvirtual/control",
                ValueType = DataType.STRING,
                Description = "Enter the control topic for all devices."
            };

            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(controlTopicSetting, o => o.ControlTopicFormat);


        }

        private uPLibrary.Networking.M2Mqtt.MqttClient mqtt = null;
        private bool enabled = false;

        public override async Task StartAsync()
        {
            try
            {
                mqtt = new MqttClient(HostSetting, Port, false, null);
                enabled = true;
                mqtt.MqttMsgPublishReceived += mqtt_MqttMsgPublishReceived;
                mqtt.ConnectionClosed += mqtt_ConnectionClosed;
                Connect();

                var pubTopic = this.ControlTopicFormat;
                mqtt.Subscribe(new string[] {pubTopic}, new byte[] {0});
                await Publish(FloodTopic, "zVirtualScenes Connected");



            }
            catch (Exception e)
            {
                Log.ReportErrorAsync(e.Message, CancellationToken);
            }
            await Log.ReportInfoFormatAsync(CancellationToken, "{0} started", Name);
            NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.OnEntityUpdated += Plugin_OnEntityUpdated;
        }

        private void mqtt_ConnectionClosed(object sender, EventArgs e)
        {
            Log.ReportInfoAsync("Control queue hung up, reconnecting", CancellationToken);
            enabled = false;
            Connect();

        }

        private void Connect()
        {
            try
            {
                mqtt.Connect("zVirtualScenes", UserName, Password);
                Log.ReportInfoAsync("Control queue connected", CancellationToken);
                enabled = true;
            }
            catch (Exception ec)
            {
                Log.ReportErrorAsync("Control queue connection error:" + ec.ToString(), CancellationToken);
                Task.Delay(1000).Wait();
                Connect();
            }
        }

        private async void mqtt_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            try
            {
                var msg = System.Text.Encoding.UTF8.GetString(e.Message);
                if (msg == "list_nodes")
                {
                    using (var context = new ZvsContext(EntityContextConnection))
                    {
                        Publish(ControlTopicFormat, Newtonsoft.Json.JsonConvert.SerializeObject(context.Devices));
                    }
                }
                else
                {
                    var control =
                        Newtonsoft.Json.JsonConvert.DeserializeObject<DeviceCommand>(msg);
                    if (control != null)
                    {
                        using (var context = new ZvsContext(EntityContextConnection))
                        {
                            var device1 =
                                (from d1 in context.Devices where d1.Id == control.DeviceId select d1).FirstOrDefault();
                            if (device1 != null)
                            {
                                if (control.Argument1.ToLower() == "list_commands")
                                {
                                    var topic = this.TopicFormat;
                                    topic = topic.Replace("{NodeNumber}", device1.NodeNumber.ToString());
                                    string cmdList = "";
                                    foreach (var cmd in device1.Commands)
                                    {
                                        var outCmd = new
                                        {
                                            NodeNumber = cmd.Device.NodeNumber,
                                            DeviceId = cmd.DeviceId,
                                            DeviceName = cmd.Device.Name,
                                            Id = cmd.Id,
                                            Name = cmd.Name,
                                            Value = cmd.Value,

                                        };

                                        Publish(topic, Newtonsoft.Json.JsonConvert.SerializeObject(outCmd));
                                    }
                                }
                                else if (control.Argument1.ToLower() == "list_values")
                                {
                                    var topic = this.TopicFormat;
                                    topic = topic.Replace("{NodeNumber}", device1.NodeNumber.ToString());
                                    string cmdList = "";
                                    foreach (var v in device1.Values)
                                    {
                                        var outPro = new
                                        {
                                            NodeNumber = v.Device.NodeNumber,
                                            DeviceId = v.DeviceId,
                                            Genre = v.Genre,
                                            Name = v.Name,
                                            Value = v.Value,
                                            ValueType = v.ValueType                                            
                                        };

                                        Publish(topic, Newtonsoft.Json.JsonConvert.SerializeObject(outPro));
                                    }
                                }
                                else
                                {
                                    var cmd =
                                        (from c in device1.Commands where c.Id == control.CommandId select c)
                                            .FirstOrDefault
                                            ();
                                    if (cmd != null)
                                    {
                                        await
                                            this.RunCommandAsync(cmd.Id, control.Argument1, control.Argument2,
                                                CancellationToken);
                                    }
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception exc)
            {
                Publish(FloodTopic, "Could not process incoming control message:" + exc.ToString());
            }

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

                        var device = (from d in context.Devices where d.Id == dv.DeviceId select d).FirstOrDefault();

                        if (device != null)
                        {
                            var topic = this.TopicFormat;
                            topic = topic.Replace("{NodeNumber}", device.NodeNumber.ToString());

                            var message = new DeviceMessage()
                            {
                                Action = "Updated",
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

                }
                catch (Exception exc)
                {
                    Log.ReportInfoAsync(exc.Message, CancellationToken);


                }
            }
            else
            {
                Log.ReportInfoAsync("MQTT Message was not published, the plugin in turned on, but the service state is not connected.", CancellationToken);
            }
        }

        public async Task Publish(string topic, string message)
        {
            if (enabled)
            {
                mqtt.Publish(topic, System.Text.Encoding.UTF8.GetBytes(message), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, true);

                if (!string.IsNullOrEmpty(FloodTopic) && FloodTopic != topic)
                {
                    mqtt.Publish(FloodTopic, System.Text.Encoding.UTF8.GetBytes(message), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, true);
                    
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
