using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using zvs.Processor;
using zvs.Entities;
using System.Web.Http.SelfHost;
using System.Web.Http;
using WebHttpAPI.Configuration;


namespace WebHttpAPI
{
    [Export(typeof(zvsPlugin))]
    public class WebAPI : zvsPlugin
    {

        public override void ProcessDeviceCommand(zvs.Entities.QueuedDeviceCommand cmd) { }
        public override void ProcessDeviceTypeCommand(zvs.Entities.QueuedDeviceTypeCommand cmd) { }
        public override void Repoll(zvs.Entities.Device device) { }
        public override void ActivateGroup(int groupID) { }
        public override void DeactivateGroup(int groupID) { }
        private int _port = 9999;
        private bool _isSSL = true;
        bool _verbose = true;
        zvs.Processor.Logging.ILog log = zvs.Processor.Logging.LogManager.GetLogger<WebAPI>();
        
        public WebAPI() : base("WebAPI", "WebAPI Plug-in", "This plug-in acts as a HTTP server to send respond to JSON AJAX requests using the Web API.") { }

        public override void Initialize()
        {
            using (zvsContext context = new zvsContext())
            {
                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "PORT",
                    Name = "HTTP Port",
                    Value = "80",
                    ValueType = DataType.INTEGER,
                    Description = "The port that HTTP will listen for commands on."
                }, context);

                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "SECURE",
                    Name = "HTTP Secure (SSL)",
                    Value = true.ToString(),
                    ValueType = DataType.BOOL,
                    Description = "If the HTTP Server will be over SSL."
                }, context);

                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "PASSWORD",
                    Name = "Password",
                    Value = "C52632B4BCDB6F8CF0F6E4545",
                    ValueType = DataType.STRING,
                    Description = "Password that protects public facing web services."
                }, context);

                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "VERBOSE",
                    Name = "Verbose Logging",
                    Value = false.ToString(),
                    ValueType = DataType.BOOL,
                    Description = "Writes all server client communication to the log for debugging."
                }, context);

                DeviceProperty.AddOrEdit(new DeviceProperty
                {
                    UniqueIdentifier = "HTTPAPI_SHOW",
                    Name = "Show device in HTTP API",
                    Description = "If enabled this device will show in applications that use the HTTP API",
                    ValueType = DataType.BOOL,
                    Value = "true"
                }, context);

                SceneProperty.AddOrEdit(new SceneProperty
                {
                    UniqueIdentifier = "HTTPAPI_SHOW",
                    Name = "Show in HTTP API Applications",
                    Description = "If enabled this scene will show in applications that use the HTTP API",
                    Value = "true",
                    ValueType = DataType.BOOL
                }, context);

                bool.TryParse(GetSettingValue("VERBOSE", context), out _verbose);
                int.TryParse(GetSettingValue("PORT", context), out _port);
                bool.TryParse(GetSettingValue("SECURE", context), out _isSSL);
            }
        }
        protected override void SettingChanged(string settingUniqueIdentifier, string settingValue)
        {
            if (settingUniqueIdentifier == "VERBOSE")
            {
                bool.TryParse(settingValue, out _verbose);
            }
            else if (settingUniqueIdentifier == "PORT" || settingUniqueIdentifier == "SECURE")
            {
                if (this.Enabled)
                    StopHTTP();

                int.TryParse(settingValue, out _port);

                if (this.Enabled)
                    StartHTTP();
            }
        }
        protected override void StartPlugin()
        {
            StartHTTP();
        }

        protected override void StopPlugin()
        {
            StopHTTP();
        }
        HttpSelfHostServer server;
        public void StartHTTP()
        {
            var config = new SelfHostConfiguration(string.Format("http{0}://0.0.0.0:{1}", (_isSSL ? "s" : ""), _port));
            config.EnableSSL = _isSSL;

            config.Routes.MapHttpRoute("DefaultRoute", "api/v2/{controller}/{id}", new { id = RouteParameter.Optional });
            //config.Routes.MapHttpRoute("ActionSpecificRoute", "api/v2/{controller}/{action}/{id}", new { id = RouteParameter.Optional, action = RouteParameter.Optional });
            server = new HttpSelfHostServer(config);
            var resolver = new zvsDependencyResolver();
            resolver.Core = this.Core;
            config.DependencyResolver = resolver;
                       
            server.OpenAsync();
            log.Info("WebAPI Server Online");
        }
        public void StopHTTP()
        {
            server.CloseAsync();
            log.Info("WebAPI Server Offline");
        }

        public void RecycleHttp()
        {
            StopHTTP();
            StartHTTP();
        }

    }
}