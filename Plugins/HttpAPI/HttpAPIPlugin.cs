using System.ComponentModel.Composition;
using System.Net.Sockets;
using System.Collections.Generic;
using System;
using System.Net;
using System.Threading;
using System.Text;
using System.Security.Cryptography;
using System.Data;
using System.ComponentModel;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using System.Linq;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using System.Reflection;
using System.Web;
using System.Collections.Specialized;
using System.Net.Mime;
using zvs.Processor;
using zvs.Entities;
using zvs.Processor.Logging;


namespace HttpAPI
{
    [Export(typeof(zvsPlugin))]
    public class HttpAPIPlugin : zvsPlugin
    {
        public volatile bool isActive;
        bool _verbose = true;
        private Guid CookieValue;
        private List<string> IssuedTokens = new List<string>();
        private static HttpListener httplistener = new HttpListener();
        private static System.Threading.AutoResetEvent listenForNextRequest = new System.Threading.AutoResetEvent(false);
        private int _port = 9999;
        zvs.Processor.Logging.ILog log = zvs.Processor.Logging.LogManager.GetLogger<HttpAPIPlugin>();
        public HttpAPIPlugin()
            : base("HttpAPI",
               "HttpAPI Plug-in",
                "This plug-in acts as a HTTP server to send respond to JSON AJAX requests."
                ) { }

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


                    httplistener.Prefixes.Add("http://*:" + _port + "/");
                    //httplistener.AuthenticationSchemes = AuthenticationSchemes.Negotiate; 
                    //httplistener.IgnoreWriteExceptions = true; 
                    httplistener.Start();

                    ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(HttpListen));

                    this.IsReady = true;
                    log.Info(string.Format("HTTP server started on port {0}", _port));
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
                    this.IsReady = false;
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while shutting down. " + ex.Message);
            }
        }

        protected override void SettingChanged(string settingUniqueIdentifier, string settingValue)
        {
            if (settingUniqueIdentifier == "VERBOSE")
            {
                bool.TryParse(settingValue, out _verbose);
            }
            else if (settingUniqueIdentifier == "PORT")
            {
                if (this.Enabled)
                    StopHTTP();

                int.TryParse(settingValue, out _port);

                if (this.Enabled)
                    StartHTTP();
            }
        }

        public override void ProcessDeviceCommand(zvs.Entities.QueuedDeviceCommand cmd) { }

        public override void ProcessDeviceTypeCommand(zvs.Entities.QueuedDeviceTypeCommand cmd) { }

        public override void Repoll(zvs.Entities.Device device) { }

        public override void ActivateGroup(int groupID) { }

        public override void DeactivateGroup(int groupID) { }

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

        public void HttpListenerCallback(IAsyncResult result)
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

                    if (_verbose)
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
                        httpListenerContext.Response.Close();
                        return;
                        //sendResponse((int)HttpStatusCode.OK, "202 OK", "", httpListenerContext);
                        // return;
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
                        }
                        else
                        {
                            reason = "Only certain API calls are allowed without credentials.";
                        }

                        if (httpListenerContext.Request.Url.Segments.Length == 3 &&
                            httpListenerContext.Request.Url.Segments[2].ToLower().StartsWith("login")
                            && (httpListenerContext.Request.HttpMethod == "POST" ||
                            httpListenerContext.Request.HttpMethod == "GET"))
                        {
                            allowed = true;
                        }
                        
                        if (httpListenerContext.Request.Url.Segments.Length == 3 &&
                            httpListenerContext.Request.Url.Segments[2].ToLower().StartsWith("logout") &&
                            httpListenerContext.Request.HttpMethod == "POST")
                        {
                            allowed = true;
                        }
                        
                        reason = "Only login and logout API calls are allowed.";
                        if (!allowed)
                        {
                            log.WarnFormat("[{0}] was denied access to '{1}' because:{2}", ip, httpListenerContext.Request.RawUrl, reason);
                            sendResponse((int)HttpStatusCode.NonAuthoritativeInformation, "203 Access Denied", "You do not have permission to access this resource.", httpListenerContext);
                            return;
                        }
                    }

                    if (httpListenerContext.Request.Url.Segments.Length > 2 && httpListenerContext.Request.Url.Segments[1].ToLower().Equals("api/"))
                    {
                        object result_obj = GetResponse(httpListenerContext);

                        //Serialize depending type
                        string RespondWith = "json";
                        if (!string.IsNullOrEmpty(httpListenerContext.Request.QueryString["type"]) && httpListenerContext.Request.QueryString["type"].Trim().ToLower() == "xml")
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

        private object GetResponse(HttpListenerContext httpListenerContext)
        {
            string ip = string.Empty;
            if (httpListenerContext.Request.RemoteEndPoint != null && httpListenerContext.Request.RemoteEndPoint.Address != null) { ip = httpListenerContext.Request.RemoteEndPoint.Address.ToString(); };


            //TODO: Read from the in memory logger if available
            if (httpListenerContext.Request.Url.Segments.Length == 3 && httpListenerContext.Request.Url.Segments[2].ToLower().StartsWith("logentries") && httpListenerContext.Request.HttpMethod == "GET")
            {
                return new { success = true, logentries = zvs.Processor.Logging.EventedLog.Items.OrderByDescending(o => o.Datetime).Take(30).ToArray() };
            }
            if (httpListenerContext.Request.Url.Segments.Length == 3 && httpListenerContext.Request.Url.Segments[2].ToLower().StartsWith("logentries") && httpListenerContext.Request.HttpMethod == "DELETE")
            {
                zvs.Processor.Logging.EventedLog.Clear();
                return new { success = true };
            }

            if (httpListenerContext.Request.Url.Segments.Length == 3 && httpListenerContext.Request.Url.Segments[2].ToLower().StartsWith("devices") && httpListenerContext.Request.HttpMethod == "GET")
            {
                List<object> devices = new List<object>();
                using (zvsContext context = new zvsContext())
                {
                    foreach (Device d in context.Devices.OrderBy(o => o.Name))
                    {
                        bool show = true;
                        bool.TryParse(DevicePropertyValue.GetDevicePropertyValue(context, d, "HTTPAPI_SHOW"), out show);

                        if (show)
                        {
                            var device = new
                            {
                                id = d.DeviceId,
                                name = d.Name,
                                on_off = d.CurrentLevelInt == 0 ? "OFF" : "ON",
                                level = d.CurrentLevelInt,
                                level_txt = d.CurrentLevelText,
                                type = d.Type.UniqueIdentifier,
                                plugin_name = d.Type.Plugin.UniqueIdentifier
                            };

                            devices.Add(device);
                        }
                    }
                }
                return new { success = true, devices = devices.ToArray() };
            }

            if (httpListenerContext.Request.Url.Segments.Length == 4 && httpListenerContext.Request.Url.Segments[2].ToLower().Equals("device/") && httpListenerContext.Request.HttpMethod == "GET")
            {
                int id = 0;
                int.TryParse(httpListenerContext.Request.Url.Segments[3].Replace("/", ""), out id);
                if (id > 0)
                {
                    using (zvsContext context = new zvsContext())
                    {
                        Device d = context.Devices.FirstOrDefault(o => o.DeviceId == id);

                        if (d != null)
                        {
                            int level = 0;

                            if (d.CurrentLevelInt.HasValue)
                                level = d.CurrentLevelInt.Value;

                            string on_off = string.Empty;
                            if (level == 0)
                                on_off = "OFF";
                            else if (level > 98)
                                on_off = "ON";
                            else
                                on_off = "DIM";

                            StringBuilder sb = new StringBuilder();
                            d.Groups.ToList().ForEach((o) => sb.Append(o.Name + " "));

                            var details = new
                            {
                                id = d.DeviceId,
                                name = d.Name,
                                on_off = on_off,
                                level = d.CurrentLevelInt,
                                level_txt = d.CurrentLevelText,
                                type = d.Type.UniqueIdentifier,
                                type_txt = d.Type.Name,
                                last_heard_from = d.LastHeardFrom.HasValue ? d.LastHeardFrom.Value.ToString() : "",
                                groups = sb.ToString(),
                                mode = d.Values.FirstOrDefault(o => o.Name == "Mode") == null ? "" : d.Values.FirstOrDefault(o => o.Name == "Mode").Value,
                                fan_mode = d.Values.FirstOrDefault(o => o.Name == "Fan Mode") == null ? "" : d.Values.FirstOrDefault(o => o.Name == "Fan Mode").Value,
                                op_state = d.Values.FirstOrDefault(o => o.Name == "Operating State") == null ? "" : d.Values.FirstOrDefault(o => o.Name == "Operating State").Value,
                                fan_state = d.Values.FirstOrDefault(o => o.Name == "Fan State") == null ? "" : d.Values.FirstOrDefault(o => o.Name == "Fan State").Value,
                                heat_p = d.Values.FirstOrDefault(o => o.Name == "Heating 1" || o.Name == "Heating1") == null ? "" : d.Values.FirstOrDefault(o => o.Name == "Heating 1" || o.Name == "Heating1").Value,
                                cool_p = d.Values.FirstOrDefault(o => o.Name == "Cooling 1" || o.Name == "Cooling1") == null ? "" : d.Values.FirstOrDefault(o => o.Name == "Cooling 1" || o.Name == "Cooling1").Value,
                                esm = d.Values.FirstOrDefault(o => o.Name == "SetBack Mode") == null ? "" : d.Values.FirstOrDefault(o => o.Name == "SetBack Mode").Value,
                                plugin_name = d.Type.Plugin.UniqueIdentifier
                            };
                            return new { success = true, details = details };
                        }
                        else
                            return new { success = false, reason = "Device not found." };
                    }
                }
            }

            if (httpListenerContext.Request.Url.Segments.Length == 5 && httpListenerContext.Request.Url.Segments[2].ToLower().Equals("device/") && httpListenerContext.Request.Url.Segments[4].ToLower().StartsWith("values") && httpListenerContext.Request.HttpMethod == "GET")
            {
                int id = 0;
                int.TryParse(httpListenerContext.Request.Url.Segments[3].Replace("/", ""), out id);
                if (id > 0)
                {
                    using (zvsContext context = new zvsContext())
                    {
                        Device d = context.Devices.FirstOrDefault(o => o.DeviceId == id);

                        if (d != null)
                        {
                            List<object> values = new List<object>();
                            foreach (DeviceValue v in d.Values)
                            {
                                values.Add(new
                                {
                                    value_id = v.UniqueIdentifier,
                                    value = v.Value,
                                    grene = v.Genre,
                                    index2 = v.Index,
                                    read_only = v.isReadOnly,
                                    label_name = v.Name,
                                    type = v.ValueType,
                                    id = v.DeviceValueId
                                });
                            }

                            return new { success = true, values = values.ToArray() };
                        }
                        else
                            return new { success = false, reason = "Device not found." };
                    }
                }
            }

            if (httpListenerContext.Request.Url.Segments.Length == 3 && httpListenerContext.Request.Url.Segments[2].ToLower().StartsWith("scenes") && httpListenerContext.Request.HttpMethod == "GET")
            {
                using (zvsContext context = new zvsContext())
                {
                    List<object> scenes = new List<object>();
                    foreach (Scene scene in context.Scenes)
                    {
                        bool show = false;
                        string prop = ScenePropertyValue.GetPropertyValue(context, scene, "HTTPAPI_SHOW");
                        bool.TryParse(prop, out show);

                        if (show)
                        {
                            scenes.Add(new
                             {
                                 id = scene.SceneId,
                                 name = scene.Name,
                                 is_running = scene.isRunning,
                                 cmd_count = scene.Commands.Count()
                             });
                        }
                    }

                    return new { success = true, scenes = scenes.ToArray() };
                }
            }

            if (httpListenerContext.Request.Url.Segments.Length == 4 && httpListenerContext.Request.Url.Segments[2].ToLower().Equals("scene/") && httpListenerContext.Request.HttpMethod == "GET")
            {
                int sID = 0;
                int.TryParse(httpListenerContext.Request.Url.Segments[3], out sID);

                using (zvsContext context = new zvsContext())
                {
                    Scene scene = context.Scenes.FirstOrDefault(s => s.SceneId == sID);

                    if (scene != null)
                    {
                        List<object> s_cmds = new List<object>();
                        foreach (SceneCommand sc in scene.Commands.OrderBy(o => o.SortOrder))
                        {
                            s_cmds.Add(new
                            {
                                device = sc.StoredCommand.ActionableObject,
                                action = sc.StoredCommand.ActionDescription,
                                order = (sc.SortOrder + 1)
                            });
                        }
                        var s = new
                        {
                            id = scene.SceneId,
                            name = scene.Name,
                            is_running = scene.isRunning,
                            cmd_count = scene.Commands.Count(),
                            cmds = s_cmds.ToArray()
                        };
                        return new { success = true, scene = s };
                    }
                    else
                        return new { success = false, reason = "Scene not found." };
                }
            }

            if (httpListenerContext.Request.Url.Segments.Length == 4 && httpListenerContext.Request.Url.Segments[2].ToLower().Equals("scene/") && httpListenerContext.Request.HttpMethod == "POST")
            {
                NameValueCollection postData = GetPostData(httpListenerContext.Request);

                int sID = 0;
                int.TryParse(httpListenerContext.Request.Url.Segments[3], out sID);

                bool is_running = false;
                bool.TryParse(postData["is_running"], out is_running);
                string name = postData["name"];

                using (zvsContext context = new zvsContext())
                {
                    Scene scene = context.Scenes.FirstOrDefault(s => s.SceneId == sID);

                    if (scene != null)
                    {
                        if (is_running)
                        {
                            BuiltinCommand cmd = context.BuiltinCommands.FirstOrDefault(c => c.UniqueIdentifier == "RUN_SCENE");
                            if (cmd != null)
                            {
                                CommandProcessor cp = new CommandProcessor(Core);
                                cp.RunBuiltinCommand(context, cmd, sID.ToString());
                            }
                            return new { success = true, desc = "Scene Started." };
                        }

                        if (!string.IsNullOrEmpty(name))
                        {
                            scene.Name = name;
                            context.SaveChanges();
                            return new { success = true, desc = "Scene Name Updated." };
                        }

                    }
                    else
                        return new { success = false, reason = "Scene not found." };
                }
            }

            if (httpListenerContext.Request.Url.Segments.Length == 3 && httpListenerContext.Request.Url.Segments[2].ToLower().StartsWith("groups") && httpListenerContext.Request.HttpMethod == "GET")
            {
                using (zvsContext db = new zvsContext())
                {
                    var q0 = from g in db.Groups
                             select new
                             {
                                 id = g.GroupId,
                                 name = g.Name,
                                 count = g.Devices.Count()
                             };

                    return new { success = true, groups = q0.ToArray() };
                }
            }

            if (httpListenerContext.Request.Url.Segments.Length == 4 && httpListenerContext.Request.Url.Segments[2].ToLower().Equals("group/") && httpListenerContext.Request.HttpMethod == "GET")
            {

                int gID = 0;
                int.TryParse(httpListenerContext.Request.Url.Segments[3], out gID);

                using (zvsContext db = new zvsContext())
                {
                    Group group = db.Groups.FirstOrDefault(g => g.GroupId == gID);

                    if (group != null)
                    {
                        List<object> group_devices = new List<object>();
                        foreach (Device gd in group.Devices)
                        {
                            group_devices.Add(new
                            {
                                id = gd.DeviceId,
                                name = gd.Name,
                                type = gd.Type.Name
                            });
                        }
                        var g = new
                        {
                            id = group.GroupId,
                            name = group.Name,
                            devices = group_devices.ToArray()
                        };
                        return new { success = true, group = g };
                    }
                    else
                        return new { success = false, reason = "Group not found." };
                }
            }

            if (httpListenerContext.Request.Url.Segments.Length == 5 && httpListenerContext.Request.Url.Segments[2].ToLower().Equals("device/") &&
                httpListenerContext.Request.Url.Segments[4].ToLower().StartsWith("commands") && httpListenerContext.Request.HttpMethod == "GET")
            {
                int id = 0;
                int.TryParse(httpListenerContext.Request.Url.Segments[3].Replace("/", ""), out id);
                if (id > 0)
                {
                    using (zvsContext db = new zvsContext())
                    {
                        Device d = db.Devices.FirstOrDefault(o => o.DeviceId == id);
                        if (d != null)
                        {
                            List<object> DeviceCommand = new List<object>();
                            foreach (DeviceCommand cmd in d.Commands)
                            {
                                DeviceCommand.Add(new
                                {
                                    id = cmd.CommandId,
                                    type = "device",
                                    friendlyname = cmd.Name,
                                    helptext = cmd.Help,
                                    name = cmd.UniqueIdentifier
                                });
                            }

                            foreach (DeviceTypeCommand cmd in d.Type.Commands)
                            {
                                DeviceCommand.Add(new
                                {
                                    id = cmd.CommandId,
                                    type = "device_type",
                                    friendlyname = cmd.Name,
                                    helptext = cmd.Help,
                                    name = cmd.UniqueIdentifier
                                });
                            }

                            return new { success = true, DeviceCommand = DeviceCommand.ToArray() };

                        }
                        else
                            return new { success = false, reason = "Device not found." };
                    }
                }
            }

            if ((httpListenerContext.Request.Url.Segments.Length == 5 || httpListenerContext.Request.Url.Segments.Length == 6) && httpListenerContext.Request.Url.Segments[2].ToLower().Equals("device/") &&
                httpListenerContext.Request.Url.Segments[4].ToLower().StartsWith("command") && httpListenerContext.Request.HttpMethod == "POST")
            {
                NameValueCollection postData = GetPostData(httpListenerContext.Request);

                int c_id = 0;
                if (httpListenerContext.Request.Url.Segments.Length == 6)
                    int.TryParse(httpListenerContext.Request.Url.Segments[5].Replace("/", ""), out c_id);

                int id = 0;
                int.TryParse(httpListenerContext.Request.Url.Segments[3].Replace("/", ""), out id);
                if (id > 0)
                {
                    using (zvsContext context = new zvsContext())
                    {
                        Device d = context.Devices.FirstOrDefault(o => o.DeviceId == id);

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
                                            cmd = d.Commands.FirstOrDefault(c => c.UniqueIdentifier.Equals(cmdUnqId));
                                        else if (c_id > 0)
                                            cmd = d.Commands.FirstOrDefault(c => c.CommandId == c_id);
                                        if (cmd != null)
                                        {
                                            log.Info(string.Format("[{0}] Running command {1}", ip, cmd.Name));
                                            CommandProcessor cp = new CommandProcessor(Core);
                                            cp.RunDeviceCommand(context, cmd, arg);
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
                                            cmd = d.Type.Commands.FirstOrDefault(c => c.UniqueIdentifier.Equals(cmdUnqId));
                                        else if (c_id > 0)
                                            cmd = d.Type.Commands.FirstOrDefault(c => c.CommandId == c_id);

                                        if (cmd != null)
                                        {

                                            log.Info(string.Format("[{0}] Running command {1}", ip, cmd.Name));
                                            CommandProcessor cp = new CommandProcessor(Core);
                                            cp.RunDeviceTypeCommand(context, cmd, d, arg);

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

            if (httpListenerContext.Request.Url.Segments.Length == 3 && httpListenerContext.Request.Url.Segments[2].ToLower().StartsWith("commands") && httpListenerContext.Request.HttpMethod == "GET")
            {
                List<object> bi_commands = new List<object>();
                using (zvsContext context = new zvsContext())
                {
                    foreach (BuiltinCommand cmd in context.BuiltinCommands)
                    {
                        bi_commands.Add(new
                        {
                            id = cmd.CommandId,
                            friendlyname = cmd.Name,
                            helptext = cmd.Help,
                            name = cmd.UniqueIdentifier
                        });
                    }
                    return new { success = true, builtin_commands = bi_commands.ToArray() };
                }
            }

            if ((httpListenerContext.Request.Url.Segments.Length == 3 || httpListenerContext.Request.Url.Segments.Length == 4) && httpListenerContext.Request.Url.Segments[2].ToLower().StartsWith("command") && httpListenerContext.Request.HttpMethod == "POST")
            {
                NameValueCollection postData = GetPostData(httpListenerContext.Request);
                string arg = postData["arg"];
                string cmdUniqId = postData["name"];
                string Name = postData["friendlyname"];

                int id = 0;
                if (httpListenerContext.Request.Url.Segments.Length == 4)
                    int.TryParse(httpListenerContext.Request.Url.Segments[3].Replace("/", ""), out id);

                using (zvsContext context = new zvsContext())
                {
                    BuiltinCommand cmd = null;
                    if (!string.IsNullOrEmpty(Name))
                        cmd = context.Commands.OfType<BuiltinCommand>().FirstOrDefault(c => c.Name.Equals(Name));
                    else if (!string.IsNullOrEmpty(cmdUniqId))
                        cmd = context.Commands.OfType<BuiltinCommand>().FirstOrDefault(c => c.UniqueIdentifier.Equals(cmdUniqId));
                    else
                        cmd = context.Commands.OfType<BuiltinCommand>().FirstOrDefault(c => c.CommandId == id);

                    if (cmd != null)
                    {
                        log.Info(string.Format("[{0}] Running command {1}", ip, cmd.Name));
                        CommandProcessor cp = new CommandProcessor(Core);
                        cp.RunBuiltinCommand(context, cmd, arg);

                        return new { success = true };
                    }
                }
            }

            if (httpListenerContext.Request.Url.Segments.Length == 3 && httpListenerContext.Request.Url.Segments[2].ToLower().StartsWith("logout") && httpListenerContext.Request.HttpMethod == "POST")
            {
                log.Info(string.Format("[{0}] Logged out.", ip));
                Cookie c = new Cookie("zvs", "No Access");
                c.Expires = DateTime.Today.AddDays(-5);
                c.Domain = "";
                c.Path = "/";
                httpListenerContext.Response.Cookies.Add(c);

                return new { success = true, isLoggedIn = false };
            }

            if (httpListenerContext.Request.Url.Segments.Length == 3 && httpListenerContext.Request.Url.Segments[2].ToLower().StartsWith("login") && httpListenerContext.Request.HttpMethod == "GET")
            {
                bool isLoggedin;

                if (hasVaildCookie(httpListenerContext) || hasVaildToken(httpListenerContext))
                    isLoggedin = true;
                else
                    isLoggedin = false;

                return new { success = true, isLoggedIn = isLoggedin };
            }

            if (httpListenerContext.Request.Url.Segments.Length == 3 && httpListenerContext.Request.Url.Segments[2].ToLower().StartsWith("login") && httpListenerContext.Request.HttpMethod == "POST")
            {
                NameValueCollection postData = GetPostData(httpListenerContext.Request);
                using (zvsContext context = new zvsContext())
                {
                    if (postData["password"] == GetSettingValue("PASSWORD", context))
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
                        log.Info(string.Format("[{0}] Login failed using password '{1}' and UserAgent '{2}'", ip, postData["password"], httpListenerContext.Request.UserAgent));
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

        public class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }
        }


    }
}