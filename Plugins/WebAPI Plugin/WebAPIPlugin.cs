using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.SelfHost;
using System.Web.Mvc;
using WebAPI.Configuration;
using zvs.Entities;
using zvs.Processor;

namespace WebAPI
{
    [Export(typeof(zvsPlugin))]
    public class WebAPIPlugin : zvsPlugin
    {
        zvs.Processor.Logging.ILog log = zvs.Processor.Logging.LogManager.GetLogger<zvsPlugin>();

        public override Guid PluginGuid
        {
            get { return Guid.Parse("6b2c505b-e50c-48c8-9159-1d75ef2efadf"); }
        }

        public override string Name
        {
            get { return "WebAPI Plug-in for ZVS"; }
        }

        public override string Description
        {
            get { return "This plug-in acts as a HTTP server to send respond to JSON AJAX requests using the Web API."; }
        }

        #region Settings

        private int _PortSetting = 9909;
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

        private bool _UseSSLSetting;
        public bool UseSSLSetting
        {
            get { return _UseSSLSetting; }
            set
            {
                if (value != _UseSSLSetting)
                {
                    _UseSSLSetting = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _TokensSettings;
        public string TokensSettings
        {
            get { return _TokensSettings; }
            set
            {
                if (value != _TokensSettings)
                {
                    _TokensSettings = value;
                    NotifyPropertyChanged();
                }
            }
        }

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

            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(sslSetting, o => o.UseSSLSetting);

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
            SHOW_IN_WEBAPI
        }

        public override async Task OnDeviceSettingsCreating(DeviceSettingBuilder settingBuilder)
        {
            await settingBuilder.RegisterAsync(new DeviceSetting
                {
                    UniqueIdentifier = DeviceSettingUids.SHOW_IN_WEBAPI.ToString(),
                    Name = "Show device in Web API",
                    Description = "If enabled this device will show in applications that use the Web API",
                    ValueType = DataType.BOOL,
                    Value = Cache.SHOW_IN_WEBAPI_DEFAULT_VALUE.ToString()
                });
        }

        public enum SceneSettingUids
        {
            SHOW_IN_WEBAPI
        }

        public override async Task OnSceneSettingsCreating(SceneSettingBuilder settingBuilder)
        {
            await settingBuilder.RegisterAsync(new SceneSetting
                {
                    UniqueIdentifier = SceneSettingUids.SHOW_IN_WEBAPI.ToString(),
                    Name = "Show scene in Web API",
                    Description = "If enabled this scene will show in applications that use the Web API",
                    Value = Cache.SHOW_IN_WEBAPI_DEFAULT_VALUE.ToString(),
                    ValueType = DataType.BOOL
                });
        }

        #endregion

        private async void HttpAPIPlugin_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("PortSetting") || e.PropertyName.Equals("UseSSLSetting"))
            {
                if (IsEnabled)
                {
                    await StopAsync();
                    await StartAsync();
                }
            }
        }

        public override Task StartAsync()
        {
            PropertyChanged += HttpAPIPlugin_PropertyChanged;

            Task.Run(() =>
            {
                StartHTTP();
            });

            return Task.FromResult(0);
        }

        public override Task StopAsync()
        {
            PropertyChanged -= HttpAPIPlugin_PropertyChanged;
            StopHTTP();
            return Task.FromResult(0);
        }

        HttpSelfHostServer server;
        public async void StartHTTP()
        {
            Cache.SigularToPluralEntityDictionary = new Dictionary<string, string>();

            //Cache all poco names in a dictionary for the DTO factory to use
            Assembly assembly = Assembly.Load("zvs.Entities");
            var pocoTypes = assembly.GetTypes()
                .Where(p => p.IsClass && !p.IsDefined(typeof(CompilerGeneratedAttribute), false))
                .ToList();

            foreach (var pocoType in pocoTypes)
            {
                var singular = pocoType.Name;
                var plural = pocoType.NamePlural();

                if (!Cache.SigularToPluralEntityDictionary.ContainsKey(singular))
                    Cache.SigularToPluralEntityDictionary.Add(singular, plural);
            }

            var config = new SelfHostConfiguration(string.Format("http{0}://0.0.0.0:{1}", (UseSSLSetting ? "s" : ""), PortSetting));
            config.EnableSSL = UseSSLSetting;

            config.Formatters.JsonFormatter.Indent = true;
            //sends the string value of the enum rather than the int
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter());
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new ContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            config.EnableCors();

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

            //NO ONE CAN GET XML
            config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();

            //BROWSERS GET JSON
            config.Formatters[0].SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

            server = new HttpSelfHostServer(config);

            var resolver = new zvsDependencyResolver();
            resolver.WebAPIPlugin = this;
            config.DependencyResolver = resolver;

            try
            {
                await server.OpenAsync();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            log.InfoFormat("WebAPI Server Online on port {0} {1} SSL", PortSetting, UseSSLSetting ? "using" : "not using");
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

        public override Task DeviceValueChangedAsync(long deviceValueId, string newValue, string oldValue)
        {
            return Task.FromResult(0);
        }
    }
}
