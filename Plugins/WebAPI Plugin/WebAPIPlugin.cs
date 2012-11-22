using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.SelfHost;
using System.Web.Mvc;
using WebAPI.Configuration;
using WebAPI.Cors;
using zvs.Entities;
using zvs.Processor;

namespace WebAPI
{
    [Export(typeof(zvsPlugin))]
    public class WebAPIPlugin : zvsPlugin
    {

        public override void ProcessDeviceCommand(zvs.Entities.QueuedDeviceCommand cmd) { }
        public override void ProcessDeviceTypeCommand(zvs.Entities.QueuedDeviceTypeCommand cmd) { }
        public override void Repoll(zvs.Entities.Device device) { }
        public override void ActivateGroup(int groupID) { }
        public override void DeactivateGroup(int groupID) { }
        private int _port = 9999;
        private bool _isSSL = true;
        bool _verbose = true;
        zvs.Processor.Logging.ILog log = zvs.Processor.Logging.LogManager.GetLogger<zvsPlugin>();

        public WebAPIPlugin() : base("WebAPI", "WebAPI Plug-in ALPHA 1", "This plug-in acts as a HTTP server to send respond to JSON AJAX requests using the Web API.") { }

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
                    UniqueIdentifier = "TOKENS",
                    Name = "X-zvsTokens",
                    Value = "CC2D226814CBC713134BD9D09B892F10A9, A0689CEF6BA3AD5FAFE018F2D796FF",
                    ValueType = DataType.STRING,
                    Description = "A comma delimited list of X-zvsTokens.  A valid X-zvsToken must be sent in the header of each API command to authorize access."
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
        public async void StartHTTP()
        {
            var config = new SelfHostConfiguration(string.Format("http{0}://0.0.0.0:{1}", (_isSSL ? "s" : ""), _port));
            config.EnableSSL = _isSSL;

            config.Formatters.JsonFormatter.Indent = true;
            //sends the string value of the enum rather than the int
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter());
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new ContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            //Enable CORS preflight selector...ie Respond to OPTIONS
            config.Services.Replace(typeof(IHttpActionSelector), new CorsPreflightActionSelector());

            config.Routes.MapHttpRoute(
                name: "V2Route",
                routeTemplate: "v2/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional },
                constraints: new { namespacing = new ControllerNamespacingConstraint("WebAPI.Controllers.v2") }
            );

            config.Routes.MapHttpRoute(
               name: "V2RouteNested",
               routeTemplate: "v2/{controller}/{parentId}/{nestedCollectionName}",
               defaults: new { id = RouteParameter.Optional },
               constraints: new { namespacing = new ControllerNamespacingConstraint("WebAPI.Controllers.v2") }
            );

            config.Routes.MapHttpRoute(
                 name: "MVC",
                 routeTemplate: "{controller}/{action}",
                 defaults: new { controller = "Home", action = "Index" }
            );

            server = new HttpSelfHostServer(config);

            var resolver = new zvsDependencyResolver();
            resolver.Core = this.Core;
            config.DependencyResolver = resolver;

            await server.OpenAsync();
            log.InfoFormat("WebAPI Server Online on port {0} {1} SSL", _port, _isSSL ? "using" : "not using");
        }

        public async void StopHTTP()
        {
            await server.CloseAsync();
            log.Info("WebAPI Server Offline");
        }

        public void RecycleHttp()
        {
            StopHTTP();
            StartHTTP();
        }

    }
}
