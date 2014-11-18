using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.SelfHost;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using zvs.DataModel;
using zvs.Processor;
using System.ComponentModel.Composition;

namespace zvsWebapi2Plugin
{
    [Export(typeof(zvsPlugin))]
    public class WebApi2Plugin : zvsPlugin
    {
        readonly zvs.Processor.Logging.ILog _log = zvs.Processor.Logging.LogManager.GetLogger<zvsPlugin>();
        private HttpSelfHostServer HttpSelfHostServer { get; set; }

        public override Guid PluginGuid
        {
            get { return Guid.Parse("f134468d-600c-4018-a001-24dd8abb9158"); }
        }

        public override string Name
        {
            get { return "WebApi 2.2 Plug-in for ZVS"; }
        }

        public override string Description
        {
            get { return "Microsoft WebApi v2.2 with oData 4 support!"; }
        }

        #region Settings

        private int _portSetting = 9909;
        public int PortSetting
        {
            get { return _portSetting; }
            set
            {
                if (value == _portSetting) return;
                _portSetting = value;
                NotifyPropertyChanged();
            }
        }

        private bool _useSslSetting;
        public bool UseSslSetting
        {
            get { return _useSslSetting; }
            set
            {
                if (value == _useSslSetting) return;
                _useSslSetting = value;
                NotifyPropertyChanged();
            }
        }

        private string _tokensSettings;
        public string TokensSettings
        {
            get { return _tokensSettings; }
            set
            {
                if (value == _tokensSettings) return;
                _tokensSettings = value;
                NotifyPropertyChanged();
            }
        }

        private bool _verboseSetting;
        public bool VerboseSetting
        {
            get { return _verboseSetting; }
            set
            {
                if (value == _verboseSetting) return;
                _verboseSetting = value;
                NotifyPropertyChanged();
            }
        }

        public override async Task OnSettingsCreating(PluginSettingBuilder settingBuilder)
        {
            var portSetting = new PluginSetting
                {
                    Name = "HTTP Port",
                    Value = "80",
                    ValueType = DataType.INTEGER,
                    Description = "The port that HTTP will listen for commands on."
                };

            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(portSetting, o => o.PortSetting);

            var sslSetting = new PluginSetting
                {
                    Name = "HTTP Secure (SSL)",
                    Value = true.ToString(),
                    ValueType = DataType.BOOL,
                    Description = "If the HTTP Server will be over SSL."
                };

            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(sslSetting, o => o.UseSslSetting);

            var tokenSetting = new PluginSetting
               {
                   Name = "X-zvsTokens",
                   Value = "CC2D226814CBC713134BD9D09B892F10A9, A0689CEF6BA3AD5FAFE018F2D796FF",
                   ValueType = DataType.STRING,
                   Description = "A comma delimited list of X-zvsTokens.  A valid X-zvsToken must be sent in the header of each API command to authorize access."
               };

            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(tokenSetting, o => o.TokensSettings);

            var verboseSettings = new PluginSetting
                {
                    Name = "Verbose Logging",
                    Value = false.ToString(),
                    ValueType = DataType.BOOL,
                    Description = "(Writes all server client communication to the log for debugging.)"
                };
            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(verboseSettings, o => o.VerboseSetting);
        }

        public enum DeviceSettingUids
        {
            ShowInWebapi
        }

        public override async Task OnDeviceSettingsCreating(DeviceSettingBuilder settingBuilder)
        {
            await settingBuilder.RegisterAsync(new DeviceSetting
                {
                    UniqueIdentifier = DeviceSettingUids.ShowInWebapi.ToString(),
                    Name = "Show device in Web API",
                    Description = "If enabled this device will show in applications that use the Web API",
                    ValueType = DataType.BOOL,
                    Value = Cache.ShowInWebapiDefaultValue.ToString()
                });
        }

        public enum SceneSettingUids
        {
            ShowInWebapi
        }

        public override async Task OnSceneSettingsCreating(SceneSettingBuilder settingBuilder)
        {
            await settingBuilder.RegisterAsync(new SceneSetting
                {
                    UniqueIdentifier = SceneSettingUids.ShowInWebapi.ToString(),
                    Name = "Show scene in Web API",
                    Description = "If enabled this scene will show in applications that use the Web API",
                    Value = Cache.ShowInWebapiDefaultValue.ToString(),
                    ValueType = DataType.BOOL
                });
        }

        #endregion

        private async void HttpAPIPlugin_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!e.PropertyName.Equals("PortSetting") && !e.PropertyName.Equals("UseSSLSetting")) return;
            if (!IsEnabled) return;

            await StopAsync();
            await StartAsync();
        }

        public override async Task StartAsync()
        {
            PropertyChanged += HttpAPIPlugin_PropertyChanged;
            await StartHttp();
        }

        public override Task StopAsync()
        {
            PropertyChanged -= HttpAPIPlugin_PropertyChanged;
            StopHttp();
            return Task.FromResult(0);
        }


        private async Task StartHttp()
        {
            var baseAddress = new Uri(string.Format("http://localhost:{0}/", PortSetting));
            try
            {
                // Set up server configuration
                var config = new HttpSelfHostConfiguration(baseAddress);
                // config.Services.Replace(typeof(ITraceWriter), new SimpleTracer());

                //Add our way too simple Authentication 
                config.Filters.Add(new ZvsAuthenticatioFilter(this));

                // Web API routes
                config.MapHttpAttributeRoutes();

                var resolver = new WebApi2PluginDependencyResolver(this);
                config.DependencyResolver = resolver;

                config.EnableCors(new EnableCorsAttribute("*", "*", "*"));

                //config.Routes.MapHttpRoute(
                //    name: "DefaultApi",
                //    routeTemplate: "api/{controller}/{id}",
                //    defaults: new { id = RouteParameter.Optional }
                //);

                var builder = new ODataConventionModelBuilder();
                var deviceType = builder.EntityType<Device>();
                deviceType.Ignore(t => t.LastHeardFrom);
                deviceType.Property(t => t.EdmLastHeardFrom).Name = "LastHeardFrom";

                var scheduledTaskType = builder.EntityType<ScheduledTask>();
                scheduledTaskType.Ignore(t => t.StartTime);
                scheduledTaskType.Property(t => t.EdmStartTime).Name = "StartTime";

                var deviceValueHistoryTaskType = builder.EntityType<DeviceValueHistory>();
                deviceValueHistoryTaskType.Ignore(t => t.DateTime);
                deviceValueHistoryTaskType.Property(t => t.EdmDateTime).Name = "DateTime";

                builder.EntitySet<Command>("Commands");
                var cExecute = builder.EntityType<Command>().Action("Execute");
                cExecute.Parameter<string>("Argument");
                cExecute.Parameter<string>("Argument2");
                builder.EntitySet<BuiltinCommand>("BuiltinCommands");
                builder.EntitySet<Device>("Devices");
                builder.EntitySet<DeviceCommand>("DeviceCommands");
                builder.EntitySet<DeviceTypeCommand>("DeviceTypeCommands");
                builder.EntitySet<DeviceValueTrigger>("DeviceValueTriggers");
                builder.EntitySet<DeviceValue>("DeviceValues");
                builder.EntitySet<DeviceValueHistory>("DeviceValueHistories");
                builder.EntitySet<Group>("Groups");
                builder.EntitySet<Scene>("Scenes");
                builder.EntitySet<Scene>("SceneCommands");
                builder.EntitySet<ScheduledTask>("ScheduledTasks");

                builder.EntitySet<zvs.Processor.Logging.LogItem>("LogItems");

                builder.Namespace = "Actions";
                config.MapODataServiceRoute("ODataRoute", "odata4", builder.GetEdmModel());

                //config.MapODataServiceRoute(
                //routeName: "ODataRoute",
                //routePrefix: null,
                //model: builder.GetEdmModel());

                // Create server
                HttpSelfHostServer = new HttpSelfHostServer(config);

                // Start listening
                await HttpSelfHostServer.OpenAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not start server: {0}", e.GetBaseException().Message);
            }

            _log.InfoFormat("WebApi2 Server Online on port {0} {1} SSL", baseAddress, UseSslSetting ? "using" : "not using");
        }

        private async void StopHttp()
        {
            // if (HttpSelfHostServer == null) return;

            //  await HttpSelfHostServer.CloseAsync();
            // _log.Info("WebApi2 Server Offline");
        }

        public override Task DeviceValueChangedAsync(long deviceValueId, string newValue, string oldValue)
        {
            return Task.FromResult(0);
        }
    }
}
