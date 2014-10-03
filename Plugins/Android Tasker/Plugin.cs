using System.ComponentModel.Composition;
using System.Collections.Generic;
using System;
using System.Net;
using System.Threading;
using System.ComponentModel;
using System.IO;
using System.Linq;
using zvs.Processor;
using zvs.Entities;
using System.Threading.Tasks;
using System.Data.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;

namespace AndroidTaskerPlugin
{
    [Export(typeof(zvsPlugin))]
    public class HttpAPIPlugin : zvsPlugin
    {
        private const bool SHOW_DEVICE_IN_LIST_DEFAULT_VALUE = true;
        private const bool SHOW_SCENE_IN_LIST_DEFAULT_VALUE = true;

        public override Guid PluginGuid
        {
            get { return Guid.Parse("505d7f88-7bbf-47e2-a1c9-461a3a6edec8"); }
        }

        public override string Name
        {
            get { return "Android Tasker Plug-in for ZVS"; }
        }

        public override string Description
        {
            get { return "This plug-in accepts HTTP posts from Android's Tasker to run scenes."; }
        }

        CancellationTokenSource listnerCTS = null;
        Task listenerTask = null;
        BuiltinCommand RunSceneCommand = null;
        volatile Dictionary<string, int> SceneCache = new Dictionary<string, int>();

        zvs.Processor.Logging.ILog log = zvs.Processor.Logging.LogManager.GetLogger<HttpAPIPlugin>();


        #region Settings
        private bool _VerboseSetting = false;
        public bool VerboseSetting
        {
            get { return _VerboseSetting; }
            set
            {
                if (value != _VerboseSetting)
                {
                    _VerboseSetting = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _PortSetting = 8021;
        public int PortSetting
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

        private string _Token = "";
        public string Token
        {
            get { return _Token; }
            set
            {
                if (value != _Token)
                {
                    _Token = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public override async Task OnSettingsCreating(PluginSettingBuilder settingBuilder)
        {
            var portSetting = new PluginSetting
            {
                UniqueIdentifier = "PORT",
                Name = "HTTP Lisenting Port",
                Value = "8021",
                ValueType = DataType.INTEGER,
                Description = "The port that HTTP will listen for commands on."
            };
            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(portSetting, o => o.PortSetting);

            var pwSetting = new PluginSetting
                {
                    UniqueIdentifier = "TOKEN",
                    Name = "Token",
                    Value = "d8712c2ecb2f4049b3245919394754ee",
                    ValueType = DataType.STRING,
                    Description = "Token required in each HTTP Post"
                };
            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(pwSetting, o => o.Token);

            var verboseSetting = new PluginSetting
            {
                UniqueIdentifier = "VERBOSE",
                Name = "Verbose Logging",
                Value = false.ToString(),
                ValueType = DataType.BOOL,
                Description = "Writes all server client communication to the log for debugging."
            };
            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(verboseSetting, o => o.VerboseSetting);

            zvsContext.ChangeNotifications<Scene>.onEntityUpdated += HttpAPIPlugin_onEntityUpdated;
            zvsContext.ChangeNotifications<Scene>.onEntityDeleted += HttpAPIPlugin_onEntityDeleted;
            zvsContext.ChangeNotifications<Scene>.onEntityAdded += HttpAPIPlugin_onEntityAdded;
        }

        async void HttpAPIPlugin_onEntityAdded(object sender, NotifyEntityChangeContext.ChangeNotifications<Scene>.EntityAddedArgs e)
        {
            await CreateSceneDictionary();
        }

        async void HttpAPIPlugin_onEntityDeleted(object sender, NotifyEntityChangeContext.ChangeNotifications<Scene>.EntityDeletedArgs e)
        {
            await CreateSceneDictionary();
        }

        async void HttpAPIPlugin_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<Scene>.EntityUpdatedArgs e)
        {
            await CreateSceneDictionary();
        }

        public enum DeviceSettingUids
        {
            SHOW_IN_HTTPAPI
        }

        public override async Task OnDeviceSettingsCreating(DeviceSettingBuilder settingBuilder)
        {
            await settingBuilder.RegisterAsync(new DeviceSetting
            {
                UniqueIdentifier = DeviceSettingUids.SHOW_IN_HTTPAPI.ToString(),
                Name = "Show device in HTTP API",
                Description = "If enabled this device will show in the HTTPAPI device collection.",
                ValueType = DataType.BOOL,
                Value = SHOW_DEVICE_IN_LIST_DEFAULT_VALUE.ToString()
            });
        }

        public enum SceneSettingUids
        {
            SHOW_IN_HTTPAPI
        }

        public override async Task OnSceneSettingsCreating(SceneSettingBuilder settingBuilder)
        {
            await settingBuilder.RegisterAsync(new SceneSetting
            {
                UniqueIdentifier = SceneSettingUids.SHOW_IN_HTTPAPI.ToString(),
                Name = "Show scene in HTTP API",
                Description = "If enabled this scene will show in the HTTPAPI scene collection.",
                Value = SHOW_SCENE_IN_LIST_DEFAULT_VALUE.ToString(),
                ValueType = DataType.BOOL
            });
        }

        #endregion
        public override Task StartAsync()
        {
            Task.Run(async () =>
            {
                using (var context = new zvsContext())
                {
                    //cache scene command
                    RunSceneCommand = await context.BuiltinCommands.FirstOrDefaultAsync(c => c.UniqueIdentifier == "RUN_SCENE");

                    //cache scene names to int
                    await CreateSceneDictionary();
                }

                if (RunSceneCommand == null)
                {
                    log.Error("Error while starting. Run scene command not found.");
                    return;
                }

                try
                {
                    StartHTTP();
                }
                catch (Exception ex)
                {
                    log.Error("Error while starting. " + ex.Message);
                }

                PropertyChanged += Plugin_PropertyChanged;
            });
            return Task.FromResult(0);
        }

        private async Task CreateSceneDictionary()
        {
            using (var context = new zvsContext())
            {
                var scenes = await context.Scenes.Select(o => new { id = o.Id, name = o.Name.ToLower() }).ToListAsync();

                lock (SceneCache)
                {
                    SceneCache.Clear();
                    foreach (var scene in scenes)
                    {
                        if (!SceneCache.ContainsKey(scene.name))
                            SceneCache.Add(scene.name, scene.id);
                    }
                }
            }
        }

        private async void Plugin_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("PortSetting"))
            {
                if (IsEnabled)
                {
                    await StopAsync();
                    await StartAsync();
                }
            }
        }

        public override async Task StopAsync()
        {
            try
            {
                await StopHTTP();
            }
            catch (Exception ex)
            {
                log.Error("Error while shutting down. " + ex.Message);
            }
            PropertyChanged -= Plugin_PropertyChanged;
        }

        public override Task DeviceValueChangedAsync(long deviceValueId, string newValue, string oldValue)
        {
            return Task.FromResult(0);
        }

        private void StartHTTP()
        {
            listnerCTS = new CancellationTokenSource();
            listenerTask = StartServer(listnerCTS.Token);
        }

        private async Task StopHTTP()
        {
            listnerCTS.Cancel();

            if (listenerTask != null)
                await Task.WhenAll(listenerTask);
        }

        public Task StartServer(CancellationToken cancellationToken)
        {
            if (!HttpListener.IsSupported)
                throw new NotSupportedException("Windows XP SP2 or Server 2003 is required.");

            var listener = new HttpListener();
            listener.Prefixes.Add("http://+:" + PortSetting + "/");
            listener.Start();
            cancellationToken.Register(listener.Stop);
            log.Info(string.Format("HTTP server started on port {0}", PortSetting));

            var tcs = new TaskCompletionSource<object>();
            listener.GetContextAsync().ContinueWith(async o =>
            {
                try
                {
                    while (true)
                    {
                        var context = await o;
                        var request = context.Request;
                        using (var response = context.Response)
                        {
                            await ProcessRequest(request, response);
                        }
                        o = listener.GetContextAsync();
                    }
                }
                catch (HttpListenerException)
                {
                    // Ignored.
                    tcs.TrySetResult(0);
                }
                catch (Exception e)
                {
                    tcs.TrySetException(e);
                }
                finally
                {
                    listener.Close();
                    log.Info("HTTP server stopped");
                }
            });

            return tcs.Task;
        }

        public async Task ProcessRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            string ip = string.Empty;
            if (request.RemoteEndPoint != null && request.RemoteEndPoint.Address != null) { ip = request.RemoteEndPoint.Address.ToString(); };

            if (VerboseSetting)
                log.Info(string.Format("[{0}] Incoming {1} request to {2} with user agent {3}", ip, request.HttpMethod, request.RawUrl, request.UserAgent));

            var streamReader = new StreamReader(request.InputStream);

            bool isRequestValid = true;

            JObject jObj = null;
            try
            {
                jObj = JObject.Parse(streamReader.ReadToEnd());
            }
            catch (Exception)
            {
                isRequestValid = false;
            }

            JsonSchema schema = JsonSchema.Parse(@"{'type':'object','$schema': 'http://json-schema.org/draft-03/schema','id': 'http://jsonschema.net','required':true,'properties':{ 'command': { 'type':'object', 'id': 'http://jsonschema.net/command', 'required':true, 'properties':{ 'action': { 'type':'string', 'id': 'http://jsonschema.net/command/action', 'required':true }, 'name': { 'type':'string', 'id': 'http://jsonschema.net/command/name', 'required':true } } }, 'token': { 'type':'string', 'id': 'http://jsonschema.net/token', 'required':true } }}");

            IList<string> errMessages = new List<string>();
            if (jObj != null)
                isRequestValid = jObj.IsValid(schema, out errMessages);

            if (!isRequestValid)
            {
                var message = "Bad request. Try again.";
                if (errMessages.Count > 0)
                    message = String.Join(", ", errMessages);

                await response.SendResponse(message, HttpStatusCode.BadRequest);
                return;
            }

            var payload = JsonConvert.DeserializeObject<Payload>(jObj.ToString());

            //Authorize
            if (!payload.token.Equals(Token))
            {
                await response.SendResponse("Unauthorized", HttpStatusCode.Unauthorized);
                return;
            }

            switch (payload.command.action)
            {
                case "runScene":
                    {
                        if (string.IsNullOrWhiteSpace(payload.command.name))
                        {
                            await response.SendResponse("Invalid Scene", HttpStatusCode.BadRequest);
                            break;
                        }

                        var sceneLowerCase = payload.command.name.ToLower();
                        if (!SceneCache.ContainsKey(sceneLowerCase))
                        {
                            await response.SendResponse("Scene not found", HttpStatusCode.BadRequest);
                            break;
                        }

                        var sId = SceneCache[sceneLowerCase];
                        CommandProcessor cp = new CommandProcessor(Core);
                        await Task.Run(async () => await cp.RunCommandAsync(this, RunSceneCommand, sId.ToString()));
                        await response.SendResponse("Scene started", HttpStatusCode.OK);
                    }
                    break;
            }
        }


        public class Payload
        {
            public string token { get; set; }
            public JsonCommand command { get; set; }
        }

        public class JsonCommand
        {
            public string action { get; set; }
            public string name { get; set; }

        }

    }
}