using System.ComponentModel.Composition;
using System.Collections.Generic;
using System;
using System.Net;
using System.Threading;
using System.Text;
using System.Security.Cryptography;
using System.ComponentModel;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using System.Linq;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using System.Web;
using System.Collections.Specialized;
using System.Net.Mime;
using zvs.Processor;
using zvs.DataModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Data.Entity;

namespace HttpAPI
{
    [Export(typeof(zvsPlugin))]
    public class HttpAPIPlugin : zvsPlugin
    {
        private const bool SHOW_DEVICE_IN_LIST_DEFAULT_VALUE = true;
        private const bool SHOW_SCENE_IN_LIST_DEFAULT_VALUE = true;

        public override Guid PluginGuid
        {
            get { return Guid.Parse("779f0fe1-5281-4d22-a87c-58a4ad65efd0"); }
        }

        public override string Name
        {
            get { return "HttpAPI Plug-in for ZVS"; }
        }

        public override string Description
        {
            get { return "This plug-in acts as a HTTP server to send respond to JSON AJAX requests."; }
        }

        public Dictionary<Guid, string> GuidToPluginName = new Dictionary<Guid, string>();

        public volatile bool isActive;
        private Guid CookieValue;
        private List<string> IssuedTokens = new List<string>();
        private static HttpListener httplistener = new HttpListener();
        private static System.Threading.AutoResetEvent listenForNextRequest = new System.Threading.AutoResetEvent(false);
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

        private string _PasswordSetting = "";
        public string PasswordSetting
        {
            get { return _PasswordSetting; }
            set
            {
                if (value != _PasswordSetting)
                {
                    _PasswordSetting = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public override async Task OnSettingsCreating(PluginSettingBuilder settingBuilder)
        {
            var portSetting = new PluginSetting
            {
                UniqueIdentifier = "PORT",
                Name = "HTTP Port",
                Value = "8085",
                ValueType = DataType.INTEGER,
                Description = "The port that HTTP will listen for commands on."
            };
            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(portSetting, o => o.PortSetting);

            var pwSetting = new PluginSetting
                {
                    UniqueIdentifier = "PASSWORD",
                    Name = "Password",
                    Value = "C52632B4BCDB6F8CF0F6E4545",
                    ValueType = DataType.STRING,
                    Description = "Password that protects public facing web services."
                };
            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(pwSetting, o => o.PasswordSetting);

            var verboseSetting = new PluginSetting
            {
                UniqueIdentifier = "VERBOSE",
                Name = "Verbose Logging",
                Value = false.ToString(),
                ValueType = DataType.BOOL,
                Description = "Writes all server client communication to the log for debugging."
            };
            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(verboseSetting, o => o.VerboseSetting);
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
            var zwaveKey = Guid.Parse("70f91ca6-08bb-406a-a60f-aeb13f50aae8");
            if (!GuidToPluginName.ContainsKey(zwaveKey))
                GuidToPluginName.Add(zwaveKey, "OPENZWAVE");

            var thinkStickKey = Guid.Parse("c4e1c021-c49f-489d-88cb-ec52bbae3be5");
            if (!GuidToPluginName.ContainsKey(thinkStickKey))
                GuidToPluginName.Add(thinkStickKey, "THINKSTICK");

            StartHTTP();
            PropertyChanged += HttpAPIPlugin_PropertyChanged;
            return Task.FromResult(0);
        }

        private async void HttpAPIPlugin_PropertyChanged(object sender, PropertyChangedEventArgs e)
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

        public override Task StopAsync()
        {
            StopHTTP();
            PropertyChanged -= HttpAPIPlugin_PropertyChanged;
            return Task.FromResult(0);
        }

        public override Task DeviceValueChangedAsync(long deviceValueId, string newValue, string oldValue)
        {
            return Task.FromResult(0);
        }

        private void StartHTTP()
        {
            try
            {
                httplistener = new HttpListener();
                if (!HttpListener.IsSupported)
                {
                    log.Fatal("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                    return;
                }

                if (!httplistener.IsListening)
                {
                    CookieValue = Guid.NewGuid();
                    httplistener.Prefixes.Add("http://*:" + PortSetting + "/");
                    //httplistener.AuthenticationSchemes = AuthenticationSchemes.Negotiate; 
                    //httplistener.IgnoreWriteExceptions = true; 
                    httplistener.Start();

                    ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(HttpListen));
                    log.Info(string.Format("HTTP server started on port {0}", PortSetting));
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while starting. " + ex.Message);
            }
        }

        private void StopHTTP()
        {
            try
            {
                if (httplistener != null && httplistener.IsListening)
                {
                    httplistener.Stop();
                    log.Info("HTTP server stopped");
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while shutting down. " + ex.Message);
            }
        }

        /// <summary>
        /// <para>128 bytes X 8  = 1024 bits = 2^1024 key space</para>
        /// </summary>
        /// <returns></returns>
        private static string GenerateRandomToken()
        {
            RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();
            byte[] randomBytes = new byte[128];
            random.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        private void HttpListen(object state)
        {
            try
            {
                while (httplistener.IsListening)
                {
                    httplistener.BeginGetContext(new AsyncCallback(HttpListenerCallback), httplistener);
                    listenForNextRequest.WaitOne();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
        }

        private bool hasVaildCookie(HttpListenerContext httpListenerContext)
        {
            if (httpListenerContext.Request.Cookies.Count > 0 &&
               httpListenerContext.Request.Cookies["zvs"] != null &&
               httpListenerContext.Request.Cookies["zvs"].Value != CookieValue.ToString())
                return true;

            return false;
        }

        private string IssueNewToken()
        {
            string token = GenerateRandomToken();
            IssuedTokens.Add(token);
            return token;
        }

        private bool hasVaildToken(HttpListenerContext httpListenerContext)
        {
            string UserToken = httpListenerContext.Request.Headers["zvstoken"];

            if (string.IsNullOrEmpty(UserToken))
                return false;

            if (IssuedTokens.Contains(UserToken))
                return true;

            return false;
        }

        public async void HttpListenerCallback(IAsyncResult result)
        {
            try
            {

                HttpListener listener = (HttpListener)result.AsyncState;
                HttpListenerContext httpListenerContext = null;

                if (listener == null) return;

                try
                {
                    httpListenerContext = listener.EndGetContext(result);
                }
                catch
                {
                    return;
                }
                finally
                {
                    listenForNextRequest.Set();
                }

                if (httpListenerContext == null)
                    return;

                // Obtain a response object
                using (System.Net.HttpListenerResponse response = httpListenerContext.Response)
                {
                    string ip = string.Empty;
                    if (httpListenerContext.Request.RemoteEndPoint != null && httpListenerContext.Request.RemoteEndPoint.Address != null) { ip = httpListenerContext.Request.RemoteEndPoint.Address.ToString(); };

                    if (VerboseSetting)
                        log.Info(string.Format("[{0}] Incoming '{1}' request to '{2}' with user agent '{3}'", ip, httpListenerContext.Request.HttpMethod, httpListenerContext.Request.RawUrl, httpListenerContext.Request.UserAgent));

                    #region enabling CORS for HTTP clients and cross domain access
                    string origin = httpListenerContext.Request.Headers["Origin"];
                    if (string.IsNullOrEmpty(origin) || origin == "null")
                        response.Headers.Add("Access-Control-Allow-Origin", "*");
                    else
                        response.Headers.Add("Access-Control-Allow-Origin", origin);

                    //CORS OPTION REQUEST
                    if (httpListenerContext.Request.HttpMethod == "OPTIONS")
                    {

                        string requestHeaders = httpListenerContext.Request.Headers["Access-Control-Request-Headers"];
                        response.Headers.Add("Access-Control-Allow-Methods", "HEAD, GET, POST, PUT, DELETE, OPTIONS");
                        response.Headers.Add("Access-Control-Max-Age", "3628800");
                        response.Headers.Add("Access-Control-Allow-Headers", requestHeaders);

                        httpListenerContext.Response.StatusCode = (int)HttpStatusCode.OK;
                        sendResponse((int)HttpStatusCode.OK, "202 OK", "", httpListenerContext);
                        httpListenerContext.Response.Close();
                        //return;

                        return;
                    }
                    #endregion


                    if (!hasVaildToken(httpListenerContext) && !hasVaildCookie(httpListenerContext))
                    {
                        //If a user does not have a token or cookie, only allow certain things....
                        bool allowed = false;
                        string reason = string.Empty;

                        if (!httpListenerContext.Request.RawUrl.ToLower().StartsWith("/api"))
                        {
                            allowed = true;
                            reason = string.Empty;
                        }
                        else
                        {
                            reason = "Only certain API calls are allowed without credentials.";


                            if (httpListenerContext.Request.Url.Segments.Length == 3 &&
                                httpListenerContext.Request.Url.Segments[2].ToLower().StartsWith("login")
                                && (httpListenerContext.Request.HttpMethod == "POST" ||
                                httpListenerContext.Request.HttpMethod == "GET"))
                            {
                                allowed = true;
                                reason = string.Empty;
                            }
                            else
                            {

                                if (httpListenerContext.Request.Url.Segments.Length == 3 &&
                                    httpListenerContext.Request.Url.Segments[2].ToLower().StartsWith("logout") &&
                                    httpListenerContext.Request.HttpMethod == "POST")
                                {
                                    allowed = true;
                                    reason = string.Empty;
                                }
                                else
                                {
                                    reason = "Only login and logout API calls are allowed.";
                                }
                            }
                        }
                        if (!allowed)
                        {
                            log.WarnFormat("[{0}] was denied access to '{1}' because:{2}", ip, httpListenerContext.Request.RawUrl, reason);
                            sendResponse((int)HttpStatusCode.NonAuthoritativeInformation, "203 Access Denied", "You do not have permission to access this resource.", httpListenerContext);
                            return;
                        }
                    }

                    if (httpListenerContext.Request.Url.Segments.Length > 2 &&
                        httpListenerContext.Request.Url.Segments[1].ToLower().Equals("api/"))
                    {
                        object result_obj = await GetResponse(httpListenerContext);

                        //Serialize depending type
                        string RespondWith = "json";
                        if (!string.IsNullOrEmpty(httpListenerContext.Request.QueryString["type"]) &&
                            httpListenerContext.Request.QueryString["type"].Trim().ToLower() == "xml")
                            RespondWith = "xml";

                        switch (RespondWith)
                        {
                            case "json":
                                {

                                    response.ContentType = "application/json;charset=utf-8";
                                    response.StatusCode = (int)HttpStatusCode.OK;
                                    using (MemoryStream stream = new MemoryStream())
                                    {
                                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(toJSON(result_obj, httpListenerContext.Request.QueryString["callback"]));
                                        stream.Write(buffer, 0, buffer.Length);
                                        byte[] bytes = stream.ToArray();
                                        if (response.OutputStream != null && response.OutputStream.CanWrite)
                                        {
                                            response.OutputStream.Write(bytes, 0, bytes.Length);
                                        }
                                    }
                                    break;
                                }
                            case "xml":
                                {
                                    response.ContentType = "text/xml;charset=utf-8";
                                    response.StatusCode = (int)HttpStatusCode.OK;
                                    XElement xml = result_obj.ToXml();
                                    string xmlstring = xml.ToString();
                                    using (MemoryStream stream = new MemoryStream())
                                    {
                                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(xmlstring);
                                        stream.Write(buffer, 0, buffer.Length);
                                        byte[] bytes = stream.ToArray();
                                        if (response.OutputStream != null && response.OutputStream.CanWrite)
                                        {
                                            response.OutputStream.Write(bytes, 0, bytes.Length);
                                        }
                                    }
                                    break;
                                }
                        }
                    }
                    else
                    {
                        string htdocs_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"plugins\htdocs");

                        string relative_file_path = httpListenerContext.Request.Url.LocalPath.Replace("/", @"\");
                        if (httpListenerContext.Request.RawUrl.Equals("/"))
                            relative_file_path = @"\index.htm";

                        if (relative_file_path.ToLower() == "\\tile.png")
                        {

                            var color = httpListenerContext.Request.QueryString["color"];
                            if (string.IsNullOrEmpty(color)) color = "Black";
                            var style = httpListenerContext.Request.QueryString["style"];
                            if (string.IsNullOrEmpty(style)) style = "Simple";

                            response.ContentType = "image/png";
                            byte[] buffer;
                            using (var context = new ZvsContext())
                            {
                                buffer = await ShellTile(context, color, style);
                            }
                            response.ContentLength64 = buffer.Length;
                            try
                            {
                                using (var s = response.OutputStream)
                                    s.Write(buffer, 0, buffer.Length);
                            }
                            catch (Exception e)
                            {
                                log.Error(e);
                            }


                        }
                        else
                        {

                            FileInfo f = new FileInfo(htdocs_path + relative_file_path);

                            if (!f.Exists)
                            {
                                sendResponse((int)HttpStatusCode.NotFound, "404 Not Found", "The resource requested does not exist.", httpListenerContext);
                            }
                            else
                            {
                                string contentType = string.Empty;
                                response.StatusCode = (int)HttpStatusCode.OK;
                                switch (f.Extension.ToLower())
                                {
                                    case ".js": contentType = "application/javascript"; break;
                                    case ".css": contentType = "text/css"; break;
                                    case ".manifest": contentType = "text/cache-manifest"; break;

                                    //images
                                    case ".gif": contentType = MediaTypeNames.Image.Gif; break;
                                    case ".jpg":
                                    case ".jpeg": contentType = MediaTypeNames.Image.Jpeg; break;
                                    case ".tiff": contentType = MediaTypeNames.Image.Tiff; break;
                                    case ".png": contentType = "image/png"; break;
                                    case ".ico": contentType = "image/ico"; break;

                                    // application
                                    case ".pdf": contentType = MediaTypeNames.Application.Pdf; break;
                                    case ".zip": contentType = MediaTypeNames.Application.Zip; break;

                                    // text
                                    case ".htm":
                                    case ".html": contentType = MediaTypeNames.Text.Html; break;
                                    case ".txt": contentType = MediaTypeNames.Text.Plain; break;
                                    case ".xml": contentType = MediaTypeNames.Text.Xml; break;
                                    default: contentType = MediaTypeNames.Application.Octet; break;
                                }

                                response.ContentType = contentType;
                                byte[] buffer = File.ReadAllBytes(f.ToString());
                                response.ContentLength64 = buffer.Length;
                                try
                                {
                                    using (var s = response.OutputStream)
                                        s.Write(buffer, 0, buffer.Length);
                                }
                                catch (Exception e)
                                {
                                    log.Error(e);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Fatal(e);
            }
        }

        private void sendResponse(int statusCode, String status, String statusMessage, HttpListenerContext context)
        {
            System.Text.StringBuilder data = new System.Text.StringBuilder();

            data.AppendLine("<html><body>");
            data.AppendLine("<h1>" + status + "</h1>");
            data.AppendLine("<p>" + statusMessage + "</p>");
            data.AppendLine("</body></html>");

            byte[] headerData = System.Text.Encoding.ASCII.GetBytes(data.ToString());

            context.Response.StatusCode = statusCode;

            //context.Response.AddHeader("Content-Length: ", headerData.Length.ToString());
            context.Response.OutputStream.Write(headerData, 0, headerData.Length);
            context.Response.Close();
        }

        private async Task<object> GetResponse(HttpListenerContext httpListenerContext)
        {
            string ip = string.Empty;
            if (httpListenerContext.Request.RemoteEndPoint != null &&
                httpListenerContext.Request.RemoteEndPoint.Address != null)
            {
                ip = httpListenerContext.Request.RemoteEndPoint.Address.ToString();
            };

            //TODO: Read from the in memory logger if available
            if (httpListenerContext.Request.Url.Segments.Length == 3 &&
                httpListenerContext.Request.Url.Segments[2].ToLower().StartsWith("logentries") &&
                httpListenerContext.Request.HttpMethod == "GET")
            {
                return new { success = true, logentries = zvs.Processor.Logging.EventedLog.Items.OrderByDescending(o => o.Datetime).Take(30).ToArray() };
            }
            if (httpListenerContext.Request.Url.Segments.Length == 3 &&
                httpListenerContext.Request.Url.Segments[2].ToLower().StartsWith("logentries") &&
                httpListenerContext.Request.HttpMethod == "DELETE")
            {
                zvs.Processor.Logging.EventedLog.Clear();
                return new { success = true };
            }

            if (httpListenerContext.Request.Url.Segments.Length == 3 &&
                httpListenerContext.Request.Url.Segments[2].ToLower().StartsWith("devices") &&
                httpListenerContext.Request.HttpMethod == "GET")
            {
                List<object> devices = new List<object>();
                using (ZvsContext context = new ZvsContext())
                {
                    //Get Devices
                    var settingUid = DeviceSettingUids.SHOW_IN_HTTPAPI.ToString();
                    var defaultSettingShouldShow = SHOW_DEVICE_IN_LIST_DEFAULT_VALUE;

                    var dbDevices = await context.Devices
                        .Where(o => (o.DeviceSettingValues.All(p => p.DeviceSetting.UniqueIdentifier != settingUid) && defaultSettingShouldShow) || //Show all objects where no explicit setting has been create yet and the defaultSetting is to show
                                     o.DeviceSettingValues.Any(p => p.DeviceSetting.UniqueIdentifier == settingUid && p.Value.Equals("true"))) //Show all objects where an explicit setting has been create and set to show
                        .Include(o => o.Type)
                        .Include(o => o.Type.Adapter)
                        .OrderBy(o => o.Name)
                        .ToListAsync();

                    foreach (var d in dbDevices)
                    {
                        var pluginName = "unknown";

                        if (GuidToPluginName.ContainsKey(d.Type.Adapter.AdapterGuid))
                            pluginName = GuidToPluginName[d.Type.Adapter.AdapterGuid];

                        var device = new
                        {
                            id = d.Id,
                            name = d.Name,
                            on_off = d.CurrentLevelInt == 0 ? "OFF" : "ON",
                            level = d.CurrentLevelInt,
                            level_txt = d.CurrentLevelText,
                            type = d.Type.UniqueIdentifier,
                            plugin_name = pluginName
                        };

                        devices.Add(device);

                    }
                }
                return new { success = true, devices = devices.ToArray() };
            }

            if (httpListenerContext.Request.Url.Segments.Length == 4 &&
                httpListenerContext.Request.Url.Segments[2].ToLower().Equals("device/") &&
                httpListenerContext.Request.HttpMethod == "GET")
            {
                int id = 0;
                int.TryParse(httpListenerContext.Request.Url.Segments[3].Replace("/", ""), out id);

                using (var context = new ZvsContext())
                {
                    var d = await context.Devices
                        .Include(o => o.Type)
                        .Include(o => o.Values)
                        .Include(o => o.Type.Adapter)
                        .FirstOrDefaultAsync(o => o.Id == id);

                    if (d != null)
                    {
                        double level = 0;

                        if (d.CurrentLevelInt.HasValue)
                            level = d.CurrentLevelInt.Value;

                        string on_off = string.Empty;
                        if (level == 0)
                            on_off = "OFF";
                        else if (level > 98)
                            on_off = "ON";
                        else
                            on_off = "DIM";

                        var sb = new StringBuilder();
                        d.Groups.ToList().ForEach((o) => sb.Append(o.Name + " "));
                        var pluginName = "unknown";

                        if (GuidToPluginName.ContainsKey(d.Type.Adapter.AdapterGuid))
                            pluginName = GuidToPluginName[d.Type.Adapter.AdapterGuid];

                        var details = new
                        {
                            id = d.Id,
                            name = d.Name,
                            on_off = on_off,
                            level = d.CurrentLevelInt,
                            level_txt = d.CurrentLevelText,
                            type = d.Type.UniqueIdentifier,
                            type_txt = d.Type.Name,
                            last_heard_from = d.LastHeardFrom.ToString(),
                            groups = sb.ToString(),
                            mode = d.Values.FirstOrDefault(o => o.Name == "Mode") == null ? "" : d.Values.FirstOrDefault(o => o.Name == "Mode").Value,
                            fan_mode = d.Values.FirstOrDefault(o => o.Name == "Fan Mode") == null ? "" : d.Values.FirstOrDefault(o => o.Name == "Fan Mode").Value,
                            op_state = d.Values.FirstOrDefault(o => o.Name == "Operating State") == null ? "" : d.Values.FirstOrDefault(o => o.Name == "Operating State").Value,
                            fan_state = d.Values.FirstOrDefault(o => o.Name == "Fan State") == null ? "" : d.Values.FirstOrDefault(o => o.Name == "Fan State").Value,
                            heat_p = d.Values.FirstOrDefault(o => o.Name == "Heating 1" || o.Name == "Heating1") == null ? "" : d.Values.FirstOrDefault(o => o.Name == "Heating 1" || o.Name == "Heating1").Value,
                            cool_p = d.Values.FirstOrDefault(o => o.Name == "Cooling 1" || o.Name == "Cooling1") == null ? "" : d.Values.FirstOrDefault(o => o.Name == "Cooling 1" || o.Name == "Cooling1").Value,
                            esm = d.Values.FirstOrDefault(o => o.Name == "SetBack Mode") == null ? "" : d.Values.FirstOrDefault(o => o.Name == "SetBack Mode").Value,
                            plugin_name = pluginName
                        };
                        return new { success = true, details = details };
                    }
                    else
                        return new { success = false, reason = "Device not found." };
                }
            }

            if (httpListenerContext.Request.Url.Segments.Length == 5 &&
                httpListenerContext.Request.Url.Segments[2].ToLower().Equals("device/") &&
                httpListenerContext.Request.Url.Segments[4].ToLower().StartsWith("values") &&
                httpListenerContext.Request.HttpMethod == "GET")
            {
                int id = 0;
                int.TryParse(httpListenerContext.Request.Url.Segments[3].Replace("/", ""), out id);

                using (var context = new ZvsContext())
                {
                    var deviceValues = await context.DeviceValues
                        .Where(o => o.DeviceId == id)
                        .ToListAsync();

                    List<object> values = new List<object>();
                    foreach (var v in deviceValues)
                    {
                        values.Add(new
                        {
                            value_id = v.UniqueIdentifier,
                            value = v.Value,
                            grene = v.Genre,
                            index2 = v.Index,
                            read_only = v.IsReadOnly,
                            label_name = v.Name,
                            type = v.ValueType,
                            id = v.Id
                        });
                    }
                    return new { success = true, values = values.ToArray() };
                }
            }

            if (httpListenerContext.Request.Url.Segments.Length == 3 &&
                httpListenerContext.Request.Url.Segments[2].ToLower().StartsWith("scenes") &&
                httpListenerContext.Request.HttpMethod == "GET")
            {
                using (ZvsContext context = new ZvsContext())
                {
                    List<object> scenes = new List<object>();

                    var settingUid = SceneSettingUids.SHOW_IN_HTTPAPI.ToString();
                    var defaultSettingShouldShow = SHOW_SCENE_IN_LIST_DEFAULT_VALUE;

                    var dbScenes = await context.Scenes
                        .Where(o => (o.SettingValues.All(p => p.SceneSetting.UniqueIdentifier != settingUid) && defaultSettingShouldShow) || //Show all objects where no explicit setting has been create yet and the defaultSetting is to show
                                     o.SettingValues.Any(p => p.SceneSetting.UniqueIdentifier == settingUid && p.Value.Equals("true"))) //Show all objects where an explicit setting has been create and set to show
                        .OrderBy(o => o.SortOrder)
                        .Include(o => o.Commands)
                        .ToListAsync();

                    foreach (var scene in dbScenes)
                    {
                        bool show = false;
                        string prop = await SceneSettingValue.GetPropertyValueAsync(context, scene, SceneSettingUids.SHOW_IN_HTTPAPI.ToString());
                        bool.TryParse(prop, out show);

                        if (show)
                        {
                            scenes.Add(new
                             {
                                 id = scene.Id,
                                 name = scene.Name,
                                 is_running = scene.IsRunning,
                                 cmd_count = scene.Commands.Count()
                             });
                        }
                    }

                    return new { success = true, scenes = scenes.ToArray() };
                }
            }

            if (httpListenerContext.Request.Url.Segments.Length == 4 &&
                httpListenerContext.Request.Url.Segments[2].ToLower().Equals("scene/") &&
                httpListenerContext.Request.HttpMethod == "GET")
            {
                int sID = 0;
                int.TryParse(httpListenerContext.Request.Url.Segments[3], out sID);

                using (var context = new ZvsContext())
                {
                    var scene = await context.Scenes
                        .Include(o => o.Commands)
                        .FirstOrDefaultAsync(s => s.Id == sID);

                    var sCmds = scene.Commands
                                .OrderBy(o => o.SortOrder)
                                .Select(sc => new
                                {
                                    device = sc.StoredCommand.TargetObjectName,
                                    action = sc.StoredCommand.Description,
                                    order = (sc.SortOrder + 1)
                                }).ToArray();

                    if (scene != null)
                    {
                        var s = new
                        {
                            id = scene.Id,
                            name = scene.Name,
                            is_running = scene.IsRunning,
                            cmd_count = scene.Commands.Count(),
                            cmds = sCmds
                        };

                        return new { success = true, scene = s };
                    }
                    else
                        return new { success = false, reason = "Scene not found." };
                }
            }

            if (httpListenerContext.Request.Url.Segments.Length == 4 &&
                httpListenerContext.Request.Url.Segments[2].ToLower().Equals("scene/") &&
                httpListenerContext.Request.HttpMethod == "POST")
            {
                NameValueCollection postData = GetPostData(httpListenerContext.Request);

                int sID = 0;
                int.TryParse(httpListenerContext.Request.Url.Segments[3], out sID);

                bool is_running = false;
                bool.TryParse(postData["is_running"], out is_running);
                string name = postData["name"];

                using (var context = new ZvsContext())
                {
                    var scene = await context.Scenes
                        .FirstOrDefaultAsync(s => s.Id == sID);

                    if (scene != null)
                    {
                        if (is_running)
                        {
                            var cmd = await context.BuiltinCommands.FirstOrDefaultAsync(c => c.UniqueIdentifier == "RUN_SCENE");
                            if (cmd != null)
                            {
                                CommandProcessor cp = new CommandProcessor(ZvsEngine);
                                await Task.Run(async () => await cp.RunCommandAsync(this, cmd, sID.ToString()));
                            }
                            return new { success = true, desc = "Scene Started." };
                        }

                        if (!string.IsNullOrEmpty(name))
                        {
                            scene.Name = name;

                            var result = await context.TrySaveChangesAsync();
                            if (result.HasError)
                            {
                                log.Error(result.Message);
                                return new { success = false, desc = result.Message };
                            }
                            else
                                return new { success = true, desc = "Scene Name Updated." };
                        }

                    }
                    else
                        return new { success = false, reason = "Scene not found." };
                }
            }

            if (httpListenerContext.Request.Url.Segments.Length == 3 && httpListenerContext.Request.Url.Segments[2].ToLower().StartsWith("groups") && httpListenerContext.Request.HttpMethod == "GET")
            {
                using (ZvsContext db = new ZvsContext())
                {
                    var groups = await db.Groups.Select(o => new
                             {
                                 id = o.Id,
                                 name = o.Name,
                                 count = o.Devices.Count()
                             })
                             .ToListAsync();

                    return new { success = true, groups = groups };
                }
            }

            if (httpListenerContext.Request.Url.Segments.Length == 4 &&
                httpListenerContext.Request.Url.Segments[2].ToLower().Equals("group/") &&
                httpListenerContext.Request.HttpMethod == "GET")
            {

                int gID = 0;
                int.TryParse(httpListenerContext.Request.Url.Segments[3], out gID);

                using (var db = new ZvsContext())
                {
                    var group = await db.Groups
                        .Include(o => o.Devices)
                        .FirstOrDefaultAsync(g => g.Id == gID);

                    if (group != null)
                    {
                        var g = new
                        {
                            id = group.Id,
                            name = group.Name,
                            devices = group.Devices.Select(gd => new
                            {
                                id = gd.Id,
                                name = gd.Name,
                                type = gd.Type.Name
                            }).ToArray()
                        };
                        return new { success = true, group = g };
                    }
                    else
                        return new { success = false, reason = "Group not found." };
                }
            }

            if (httpListenerContext.Request.Url.Segments.Length == 5 &&
                httpListenerContext.Request.Url.Segments[2].ToLower().Equals("device/") &&
                httpListenerContext.Request.Url.Segments[4].ToLower().StartsWith("commands") &&
                httpListenerContext.Request.HttpMethod == "GET")
            {
                int id = 0;
                int.TryParse(httpListenerContext.Request.Url.Segments[3].Replace("/", ""), out id);
                if (id > 0)
                {
                    using (var db = new ZvsContext())
                    {
                        var d = await db.Devices
                            .Include(o => o.Commands)
                            .Include(o => o.Type.Commands)
                            .FirstOrDefaultAsync(o => o.Id == id);

                        if (d != null)
                        {
                            var deviceCommands = d.Commands.Select(cmd =>
                                new
                                {
                                    id = cmd.Id,
                                    type = "device",
                                    friendlyname = cmd.Name,
                                    helptext = cmd.Help,
                                    name = cmd.UniqueIdentifier
                                }).ToList();


                            deviceCommands.AddRange(d.Type.Commands.Select(cmd => new
                                {
                                    id = cmd.Id,
                                    type = "device_type",
                                    friendlyname = cmd.Name,
                                    helptext = cmd.Help,
                                    name = cmd.UniqueIdentifier
                                }));

                            return new { success = true, DeviceCommand = deviceCommands };
                        }
                        else
                            return new { success = false, reason = "Device not found." };
                    }
                }
            }

            if ((httpListenerContext.Request.Url.Segments.Length == 5 ||
                httpListenerContext.Request.Url.Segments.Length == 6) &&
                httpListenerContext.Request.Url.Segments[2].ToLower().Equals("device/") &&
                httpListenerContext.Request.Url.Segments[4].ToLower().StartsWith("command") &&
                httpListenerContext.Request.HttpMethod == "POST")
            {
                NameValueCollection postData = GetPostData(httpListenerContext.Request);

                int c_id = 0;
                if (httpListenerContext.Request.Url.Segments.Length == 6)
                    int.TryParse(httpListenerContext.Request.Url.Segments[5].Replace("/", ""), out c_id);

                int id = 0;
                int.TryParse(httpListenerContext.Request.Url.Segments[3].Replace("/", ""), out id);
                if (id > 0)
                {
                    using (var context = new ZvsContext())
                    {
                        var d = await context.Devices
                            .Include(o => o.Commands)
                            .Include(o => o.Type.Commands)
                            .FirstOrDefaultAsync(o => o.Id == id);

                        if (d != null)
                        {
                            string arg = postData["arg"];
                            string strtype = postData["type"];
                            string cmdUnqId = postData["name"];
                            string cmdName = postData["friendlyname"];

                            switch (strtype)
                            {
                                case "device":
                                    {
                                        //If the user sends the command name in the post, ignore the ID if sent and do a lookup by name
                                        DeviceCommand cmd = null;
                                        if (!string.IsNullOrEmpty(cmdName))
                                            cmd = d.Commands.FirstOrDefault(c => c.Name.Equals(cmdName));
                                        else if (!string.IsNullOrEmpty(cmdUnqId))
                                            cmd = d.Commands.FirstOrDefault(c => c.UniqueIdentifier.Contains(cmdUnqId));
                                        else if (c_id > 0)
                                            cmd = d.Commands.FirstOrDefault(c => c.Id == c_id);
                                        if (cmd != null)
                                        {
                                            log.Info(string.Format("[{0}] Running command {1}", ip, cmd.Name));
                                            CommandProcessor cp = new CommandProcessor(ZvsEngine);
                                            await Task.Run(async () => await cp.RunCommandAsync(this, cmd, arg));
                                            return new { success = true };
                                        }
                                        else
                                            return new { success = false, reason = "Device command not found." };
                                    }
                                case "device_type":
                                    {
                                        //If the user sends the command name in the post, ignore the ID if sent and do a lookup by name
                                        DeviceTypeCommand cmd = null;
                                        if (!string.IsNullOrEmpty(cmdName))
                                            cmd = d.Type.Commands.FirstOrDefault(c => c.Name.Equals(cmdName));
                                        else if (!string.IsNullOrEmpty(cmdUnqId))
                                            cmd = d.Type.Commands.FirstOrDefault(c => c.UniqueIdentifier.Contains(cmdUnqId));
                                        else if (c_id > 0)
                                            cmd = d.Type.Commands.FirstOrDefault(c => c.Id == c_id);

                                        if (cmd != null)
                                        {

                                            log.Info(string.Format("[{0}] Running command {1}", ip, cmd.Name));
                                            CommandProcessor cp = new CommandProcessor(ZvsEngine);
                                            await Task.Run(async () => await cp.RunCommandAsync(this, cmd, arg, d.Id.ToString()));

                                            return new { success = true };
                                        }
                                        return new { success = false, reason = "Device type command not found." };
                                    }
                                default:
                                    {
                                        return new { success = false, reason = "Invalid command type." };
                                    }
                            }
                        }
                        else
                            return new { success = false, reason = "Device not found." };
                    }
                }

            }

            //TODO: add search for commands per device ID.

            if (httpListenerContext.Request.Url.Segments.Length == 3 &&
                httpListenerContext.Request.Url.Segments[2].ToLower().StartsWith("commands") &&
                httpListenerContext.Request.HttpMethod == "GET")
            {
                List<object> bi_commands = new List<object>();
                using (var context = new ZvsContext())
                {
                    var bCommands = await context.BuiltinCommands.Select(cmd => new
                        {
                            id = cmd.Id,
                            friendlyname = cmd.Name,
                            helptext = cmd.Help,
                            name = cmd.UniqueIdentifier
                        }).ToListAsync();

                    return new { success = true, builtin_commands = bCommands.ToArray() };
                }
            }

            if ((httpListenerContext.Request.Url.Segments.Length == 3 ||
                httpListenerContext.Request.Url.Segments.Length == 4) &&
                httpListenerContext.Request.Url.Segments[2].ToLower().StartsWith("command") &&
                httpListenerContext.Request.HttpMethod == "POST")
            {
                NameValueCollection postData = GetPostData(httpListenerContext.Request);
                string arg = postData["arg"];
                string cmdUniqId = postData["name"];
                string Name = postData["friendlyname"];

                int id = 0;
                if (httpListenerContext.Request.Url.Segments.Length == 4)
                    int.TryParse(httpListenerContext.Request.Url.Segments[3].Replace("/", ""), out id);

                using (var context = new ZvsContext())
                {
                    BuiltinCommand cmd = null;
                    if (!string.IsNullOrEmpty(Name))
                        cmd = await context.Commands.OfType<BuiltinCommand>().FirstOrDefaultAsync(c => c.Name.Equals(Name));
                    else if (!string.IsNullOrEmpty(cmdUniqId))
                        cmd = await context.Commands.OfType<BuiltinCommand>().FirstOrDefaultAsync(c => c.UniqueIdentifier.Contains(cmdUniqId));
                    else
                        cmd = await context.Commands.OfType<BuiltinCommand>().FirstOrDefaultAsync(c => c.Id == id);

                    if (cmd != null)
                    {
                        log.Info(string.Format("[{0}] Running command {1}", ip, cmd.Name));
                        CommandProcessor cp = new CommandProcessor(ZvsEngine);
                        await Task.Run(async () => await cp.RunCommandAsync(this, cmd, arg));

                        return new { success = true };
                    }
                }
            }

            if (httpListenerContext.Request.Url.Segments.Length == 3 &&
                httpListenerContext.Request.Url.Segments[2].ToLower().StartsWith("logout") &&
                httpListenerContext.Request.HttpMethod == "POST")
            {
                log.Info(string.Format("[{0}] Logged out.", ip));
                Cookie c = new Cookie("zvs", "No Access");
                c.Expires = DateTime.Today.AddDays(-5);
                c.Domain = "";
                c.Path = "/";
                httpListenerContext.Response.Cookies.Add(c);

                return new { success = true, isLoggedIn = false };
            }

            if (httpListenerContext.Request.Url.Segments.Length == 3 &&
                httpListenerContext.Request.Url.Segments[2].ToLower().StartsWith("login") &&
                httpListenerContext.Request.HttpMethod == "GET")
            {
                bool isLoggedin;

                if (hasVaildCookie(httpListenerContext) || hasVaildToken(httpListenerContext))
                    isLoggedin = true;
                else
                    isLoggedin = false;

                return new { success = true, isLoggedIn = isLoggedin };
            }

            if (httpListenerContext.Request.Url.Segments.Length == 3 &&
                httpListenerContext.Request.Url.Segments[2].ToLower().StartsWith("login") &&
                httpListenerContext.Request.HttpMethod == "POST")
            {
                NameValueCollection postData = GetPostData(httpListenerContext.Request);
                using (ZvsContext context = new ZvsContext())
                {
                    if (postData["password"] == PasswordSetting)
                    {
                        Cookie c = new Cookie("zvs", CookieValue.ToString());
                        c.Expires = DateTime.Today.AddDays(5);
                        c.Domain = "";// httpListenerContext.Request.Headers["Host"]; //orgin.Authority;
                        c.Path = "/";
                        httpListenerContext.Response.Cookies.Add(c);

                        //Create a token 
                        string token = IssueNewToken();
                        httpListenerContext.Response.Headers.Add("zvstoken", token);
                        log.Info(string.Format("[{0}] Login succeeded. UserAgent '{1}'", ip, httpListenerContext.Request.UserAgent));

                        return new { success = true, zvstoken = token };
                    }
                    else
                    {
                        log.Info(string.Format("[{0}] Login failed using password '{1}' and UserAgent '{2}'",
                            ip,
                            postData["password"],
                            httpListenerContext.Request.UserAgent));

                        return new { success = false };
                    }
                }
            }

            return new { success = false, reason = "Invalid Command" };
        }

        private NameValueCollection GetPostData(HttpListenerRequest request)
        {
            string input = null;
            using (StreamReader reader = new StreamReader(request.InputStream))
            {
                input = reader.ReadToEnd();
            }
            return HttpUtility.ParseQueryString(input);
        }

        private string toJSON<T>(T obj, string callback)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            if (!string.IsNullOrEmpty(callback))
                return callback + "(" + js.Serialize(obj) + ");";
            else
                return js.Serialize(obj);

        }

        private string toXML<T>(T obj)
        {
            XmlWriterSettings xmlwritersettings = new XmlWriterSettings();
            xmlwritersettings.NewLineHandling = NewLineHandling.None;
            xmlwritersettings.Indent = false;
            xmlwritersettings.Encoding = Encoding.UTF8;
            XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));

            string utf8;
            using (Utf8StringWriter writer = new Utf8StringWriter())
            {
                serializer.Serialize(XmlWriter.Create(writer, xmlwritersettings), obj);
                utf8 = writer.ToString();
            }
            return utf8;
        }

        private async Task<byte[]> ShellTile(ZvsContext context, string color, string style)
        {
            int deviceOnCount = 0;
            int sceneRunningCount = 0;
            foreach (Device d in await context.Devices.ToListAsync())
            {
                bool show = true;
                bool.TryParse(await DeviceSettingValue.GetDevicePropertyValueAsync(context, d, DeviceSettingUids.SHOW_IN_HTTPAPI.ToString()), out show);

                if (show)
                    if (d.CurrentLevelInt > 0) deviceOnCount++;
            }
            foreach (Scene scene in await context.Scenes.ToListAsync())
            {
                bool show = false;
                string prop = await SceneSettingValue.GetPropertyValueAsync(context, scene, SceneSettingUids.SHOW_IN_HTTPAPI.ToString());
                bool.TryParse(prop, out show);

                if (show && scene.IsRunning)
                    sceneRunningCount++;
            }

            var asm = typeof(HttpAPI.HttpAPIPlugin).Assembly;
            using (var template = asm.GetManifestResourceStream("HttpAPI.TileTemplate.png"))
            {
                System.Drawing.Image canvas = System.Drawing.Image.FromStream(template);
                var g = Graphics.FromImage(canvas);
                Font font = new Font(FontFamily.GenericSansSerif, 20);

                Color c = Color.FromName(color);
                if (c == null) c = Color.Blue;
                SolidBrush s = new SolidBrush(c);

                log.InfoFormat("Tile Style:{0}, Color:{1}, Devices On:{2}, Scenes Active:{3}", style, color, deviceOnCount, sceneRunningCount);
                if (style.ToLower() == "dands")
                {

                    g.DrawString(string.Format("Devices: {0}", deviceOnCount), font, s, new PointF(50, 10));
                    g.DrawString(string.Format("Scenes:  {0}", sceneRunningCount), font, s, new PointF(50, 35));
                }
                else
                {
                    g.DrawString(string.Format("Devices: {0}", deviceOnCount), font, s, new PointF(50, 10));
                }

                using (var stm = new MemoryStream())
                {
                    canvas.Save(stm, System.Drawing.Imaging.ImageFormat.Png);
                    var bytes = new byte[stm.Length];
                    if (stm.CanSeek && stm.Position > 0) stm.Seek(0, SeekOrigin.Begin);
                    stm.Read(bytes, 0, bytes.Length);
                    return bytes;
                }
            }

        }
        public class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }
        }
    }
}