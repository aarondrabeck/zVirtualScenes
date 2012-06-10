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
using zVirtualScenes;
using zVirtualScenesModel;

namespace HttpAPI
{
    [Export(typeof(Plugin))]
    public class HttpAPIPlugin : Plugin
    {
        public volatile bool isActive;
        bool verbose = true;
        private Guid CookieValue;
        private static HttpListener httplistener = new HttpListener();
        private static System.Threading.AutoResetEvent listenForNextRequest = new System.Threading.AutoResetEvent(false);

        public HttpAPIPlugin()
            : base("HttpAPI",
               "HttpAPI Plugin",
                "This plug-in acts as a HTTP server to send respond to JSON AJAX requests."
                ) { }

        public override void Initialize()
        {
            using (zvsLocalDBEntities context = new zvsLocalDBEntities())
            {
                DefineOrUpdateSetting(new plugin_settings
                {
                    name = "PORT",
                    friendly_name = "HTTP Port",
                    value = "80",
                    value_data_type = (int)Data_Types.INTEGER,
                    description = "The port that HTTP will listen for commands on."
                }, context);

                DefineOrUpdateSetting(new plugin_settings
                {
                    name = "PASSWORD",
                    friendly_name = "Password",
                    value = "C52632B4BCDB6F8CF0F6E4545",
                    value_data_type = (int)Data_Types.STRING,
                    description = "Password that protects public facing web services."
                }, context);

                DefineOrUpdateSetting(new plugin_settings
                {
                    name = "VERBOSE",
                    friendly_name = "Verbose Logging",
                    value = false.ToString(),
                    value_data_type = (int)Data_Types.BOOL,
                    description = "(Writes all server client communication to the log for debugging.)"
                }, context);
            }
        }

        protected override void StartPlugin()
        {
            try
            {
                httplistener = new HttpListener();

                if (!HttpListener.IsSupported)
                {
                    WriteToLog(Urgency.ERROR, "Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                    return;
                }

                if (!httplistener.IsListening)
                {
                    using (zvsLocalDBEntities context = new zvsLocalDBEntities())
                    {
                        bool.TryParse(GetSettingValue("VERBOSE", context), out verbose);

                        CookieValue = Guid.NewGuid();
                        int port = 9999;
                        int.TryParse(GetSettingValue("PORT", context), out port);
                        httplistener.Prefixes.Add("http://*:" + port + "/");
                        //httplistener.AuthenticationSchemes = AuthenticationSchemes.Negotiate; 
                        //httplistener.IgnoreWriteExceptions = true; 
                        httplistener.Start();


                        ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(HttpListen));

                        WriteToLog(Urgency.INFO, string.Format("{0} plugin started on port {1}.", this.Friendly_Name, port));
                    }
                }
            }
            catch (Exception ex)
            {
                WriteToLog(Urgency.ERROR, "Error while starting. " + ex.Message);
            }

            this.IsReady = true;
        }

        protected override void StopPlugin()
        {
            WriteToLog(Urgency.INFO, this.Friendly_Name + " plugin ended.");

            try
            {
                if (httplistener != null && httplistener.IsListening)
                {
                    httplistener.Stop();
                    WriteToLog(Urgency.INFO, string.Format("{0} plugin shutdown."));
                }
            }
            catch (Exception ex)
            {
                WriteToLog(Urgency.ERROR, "Error while shuting down. " + ex.Message);
            }

            this.IsReady = false;
        }

        protected override void SettingChanged(string settingName, string settingValue)
        {
        }
        public override bool ProcessDeviceCommand(device_command_que cmd)
        {
            return true;
        }
        public override bool ProcessDeviceTypeCommand(device_type_command_que cmd)
        {
            return true;
        }
        public override bool Repoll(device device)
        {
            return true;
        }
        public override bool ActivateGroup(int groupID)
        {
            return true;
        }
        public override bool DeactivateGroup(int groupID)
        {
            return true;
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
                WriteToLog(Urgency.ERROR, ex.Message);
            }
        }

        public void HttpListenerCallback(IAsyncResult result)
        {
            HttpListener listener = (HttpListener)result.AsyncState;
            HttpListenerContext context = null;

            if (listener == null) return;

            try
            {
                context = listener.EndGetContext(result);
            }
            catch
            {
                return;
            }
            finally
            {
                listenForNextRequest.Set();
            }

            if (context == null)
                return;

            // Obtain a response object
            using (System.Net.HttpListenerResponse response = context.Response)
            {
                string ip = string.Empty;
                if (context.Request.RemoteEndPoint != null && context.Request.RemoteEndPoint.Address != null) { ip = context.Request.RemoteEndPoint.Address.ToString(); };

                if (verbose)
                    WriteToLog(Urgency.INFO, string.Format("[{0}] Incoming '{1}' request to '{2}' with user agent '{3}'", ip, context.Request.HttpMethod, context.Request.RawUrl, context.Request.UserAgent));

                //doesnt have valid cookies 
                if (context.Request.Cookies.Count == 0 || (context.Request.Cookies.Count > 0 && context.Request.Cookies["zvs"] != null && context.Request.Cookies["zvs"].Value != CookieValue.ToString()))
                {
                    bool allowed = false;
                    if (!context.Request.RawUrl.ToLower().StartsWith("/api")) { allowed = true; }
                    if (context.Request.Url.Segments.Length == 3 && context.Request.Url.Segments[2].ToLower().StartsWith("login") && (context.Request.HttpMethod == "POST" || context.Request.HttpMethod == "GET")) { allowed = true; }
                    if (context.Request.Url.Segments.Length == 3 && context.Request.Url.Segments[2].ToLower().StartsWith("logout") && context.Request.HttpMethod == "POST") { allowed = true; }

                    if (!allowed)
                    {
                        WriteToLog(Urgency.INFO, string.Format("[{0}] was denied access to '{1}'", ip, context.Request.RawUrl));
                        sendResponse((int)HttpStatusCode.NonAuthoritativeInformation, "203 Access Denied", "You do not have permission to access this resource.", context);
                        return;
                    }
                }

                if (context.Request.Url.Segments.Length > 2 && context.Request.Url.Segments[1].ToLower().Equals("api/"))
                {
                    object result_obj = GetResponse(context.Request, response);

                    //Serialize depending type
                    string RespondWith = "json";
                    if (!string.IsNullOrEmpty(context.Request.QueryString["type"]) && context.Request.QueryString["type"].Trim().ToLower() == "xml")
                        RespondWith = "xml";

                    switch (RespondWith)
                    {
                        case "json":
                            {

                                response.ContentType = "application/json;charset=utf-8";
                                response.StatusCode = (int)HttpStatusCode.OK;
                                MemoryStream stream = new MemoryStream();
                                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(toJSON(result_obj, context.Request.QueryString["callback"]));
                                stream.Write(buffer, 0, buffer.Length);
                                byte[] bytes = stream.ToArray();
                                response.OutputStream.Write(bytes, 0, bytes.Length);
                                stream.Close();
                                break;
                            }
                        case "xml":
                            {
                                response.ContentType = "text/xml;charset=utf-8";
                                response.StatusCode = (int)HttpStatusCode.OK;
                                XElement xml = result_obj.ToXml();
                                string xmlstring = xml.ToString();
                                MemoryStream stream = new MemoryStream();
                                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(xmlstring);
                                stream.Write(buffer, 0, buffer.Length);
                                byte[] bytes = stream.ToArray();
                                response.OutputStream.Write(bytes, 0, bytes.Length);
                                stream.Close();
                                break;
                            }
                    }
                }
                else
                {
                    string htdocs_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"plugins\htdocs");

                    string relative_file_path = context.Request.Url.LocalPath.Replace("/", @"\");
                    if (context.Request.RawUrl.Equals("/"))
                        relative_file_path = @"\index.htm";

                    FileInfo f = new FileInfo(htdocs_path + relative_file_path);

                    if (!f.Exists)
                    {
                        sendResponse((int)HttpStatusCode.NotFound, "404 Not Found", "The resource requested does not exist.", context);
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
                            Console.WriteLine("HTTAPI ERROR: {0}", e.Message);
                        }
                    }
                }
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

            context.Response.ContentType = "text/html";
            context.Response.StatusCode = statusCode;

            //context.Response.AddHeader("Content-Length: ", headerData.Length.ToString());
            context.Response.OutputStream.Write(headerData, 0, headerData.Length);
            context.Response.Close();
        }

        private object GetResponse(HttpListenerRequest request, HttpListenerResponse response)
        {
            string ip = string.Empty;
            if (request.RemoteEndPoint != null && request.RemoteEndPoint.Address != null) { ip = request.RemoteEndPoint.Address.ToString(); };

            if (request.Url.Segments.Length == 3 && request.Url.Segments[2].ToLower().StartsWith("devices") && request.HttpMethod == "GET")
            {
                List<object> devices = new List<object>();
                using (zvsLocalDBEntities context = new zvsLocalDBEntities())
                {
                    foreach (device d in context.devices.OrderBy(o => o.friendly_name))
                    {
                       

                        var device = new
                        {
                            id = d.id,
                            name = d.friendly_name,
                            on_off = d.current_level_int == 0 ? "OFF" : "ON",
                            level = d.current_level_int,
                            level_txt = d.current_level_txt,
                            type = d.device_types.name
                        };

                        devices.Add(device);
                    }
                }
                return new { success = true, devices = devices.ToArray() };
            }

            if (request.Url.Segments.Length == 4 && request.Url.Segments[2].ToLower().Equals("device/") && request.HttpMethod == "GET")
            {
                int id = 0;
                int.TryParse(request.Url.Segments[3].Replace("/", ""), out id);
                if (id > 0)
                {
                    using (zvsLocalDBEntities context = new zvsLocalDBEntities())
                    {
                        device d = context.devices.FirstOrDefault(o => o.id == id);

                        if (d != null)
                        {
                            int level = d.current_level_int;

                            string on_off = string.Empty;
                            if (level == 0)
                                on_off = "OFF";
                            else if (level > 98)
                                on_off = "ON";
                            else
                                on_off = "DIM";

                            StringBuilder sb = new StringBuilder();
                            d.group_devices.ToList().ForEach((o) => sb.Append(o.group.name + " "));

                            var details = new
                            {
                                id = d.id,
                                name = d.friendly_name,
                                on_off = on_off,
                                level = d.current_level_int,
                                level_txt = d.current_level_txt,
                                type = d.device_types.name,
                                type_txt = d.device_types.friendly_name,
                                last_heard_from = d.last_heard_from.HasValue ? d.last_heard_from.Value.ToString() : "",
                                groups = sb.ToString(),
                                mode = d.device_values.FirstOrDefault(o => o.label_name == "Mode") == null ? "" : d.device_values.FirstOrDefault(o => o.label_name == "Mode").value2,
                                fan_mode = d.device_values.FirstOrDefault(o => o.label_name == "Fan Mode") == null ? "" : d.device_values.FirstOrDefault(o => o.label_name == "Fan Mode").value2,
                                op_state = d.device_values.FirstOrDefault(o => o.label_name == "Operating State") == null ? "" : d.device_values.FirstOrDefault(o => o.label_name == "Operating State").value2,
                                fan_state = d.device_values.FirstOrDefault(o => o.label_name == "Fan State") == null ? "" : d.device_values.FirstOrDefault(o => o.label_name == "Fan State").value2,
                                heat_p = d.device_values.FirstOrDefault(o => o.label_name == "Heating 1") == null ? "" : d.device_values.FirstOrDefault(o => o.label_name == "Heating 1").value2,
                                cool_p = d.device_values.FirstOrDefault(o => o.label_name == "Cooling 1") == null ? "" : d.device_values.FirstOrDefault(o => o.label_name == "Cooling 1").value2,
                                esm = d.device_values.FirstOrDefault(o => o.label_name == "Basic") == null ? "" : d.device_values.FirstOrDefault(o => o.label_name == "Basic").value2
                            };
                            return new { success = true, details = details };
                        }
                        else
                            return new { success = false, reason = "Device not found." };
                    }
                }
            }

            if (request.Url.Segments.Length == 5 && request.Url.Segments[2].ToLower().Equals("device/") && request.Url.Segments[4].ToLower().StartsWith("values") && request.HttpMethod == "GET")
            {
                int id = 0;
                int.TryParse(request.Url.Segments[3].Replace("/", ""), out id);
                if (id > 0)
                {
                    using (zvsLocalDBEntities context = new zvsLocalDBEntities())
                    {
                        device d = context.devices.FirstOrDefault(o => o.id == id);

                        if (d != null)
                        {
                            List<object> values = new List<object>();
                            foreach (device_values v in d.device_values)
                            {
                                values.Add(new
                                {
                                    value_id = v.value_id,
                                    value = v.value2,
                                    grene = v.genre,
                                    index2 = v.index2,
                                    read_only = v.read_only,
                                    label_name = v.label_name,
                                    type = v.type,
                                    id = v.id
                                });
                            }

                            return new { success = true, values = values.ToArray() };
                        }
                        else
                            return new { success = false, reason = "Device not found." };
                    }
                }
            }

            if (request.Url.Segments.Length == 3 && request.Url.Segments[2].ToLower().StartsWith("scenes") && request.HttpMethod == "GET")
            {
                using (zvsLocalDBEntities context = new zvsLocalDBEntities())
                {
                    var q0 = from d in context.scenes
                             select new
                             {
                                 id = d.id,
                                 name = d.friendly_name,
                                 is_running = d.is_running,
                                 cmd_count = d.scene_commands.Count()
                             };


                    return new { success = true, scenes = q0.ToArray() };
                }
            }

            if (request.Url.Segments.Length == 4 && request.Url.Segments[2].ToLower().Equals("scene/") && request.HttpMethod == "GET")
            {
                int sID = 0;
                int.TryParse(request.Url.Segments[3], out sID);

                using (zvsLocalDBEntities context = new zvsLocalDBEntities())
                {
                    scene scene = context.scenes.FirstOrDefault(s => s.id == sID);

                    if (scene != null)
                    {
                        List<object> s_cmds = new List<object>();
                        foreach (scene_commands sc in scene.scene_commands.OrderBy(o => o.sort_order))
                        {
                            s_cmds.Add(new
                            {
                                device = sc.Actionable_Object,
                                action = sc.Action_Description,
                                order = (sc.sort_order + 1)
                            });
                        }
                        var s = new
                        {
                            id = scene.id,
                            name = scene.friendly_name,
                            is_running = scene.is_running,
                            cmd_count = scene.scene_commands.Count(),
                            cmds = s_cmds.ToArray()
                        };
                        return new { success = true, scene = s };
                    }
                    else
                        return new { success = false, reason = "Scene not found." };
                }
            }

            if (request.Url.Segments.Length == 4 && request.Url.Segments[2].ToLower().Equals("scene/") && request.HttpMethod == "POST")
            {
                NameValueCollection postData = GetPostData(request);

                int sID = 0;
                int.TryParse(request.Url.Segments[3], out sID);

                bool is_running = false;
                bool.TryParse(postData["is_running"], out is_running);
                string name = postData["name"];

                using (zvsLocalDBEntities db = new zvsLocalDBEntities())
                {
                    scene scene = db.scenes.FirstOrDefault(s => s.id == sID);

                    if (scene != null)
                    {
                        if (is_running)
                        {
                            SceneRunner sr = new SceneRunner();
                            sr.RunScene(scene.id);
                            return new { success = true, desc = "Scene Started." };
                        }

                        if (!string.IsNullOrEmpty(name))
                        {
                            scene.friendly_name = name;
                            db.SaveChanges();
                            return new { success = true, desc = "Scene Name Updated." };
                        }
                    }
                    else
                        return new { success = false, reason = "Scene not found." };
                }
            }

            if (request.Url.Segments.Length == 3 && request.Url.Segments[2].ToLower().StartsWith("groups") && request.HttpMethod == "GET")
            {
                using (zvsLocalDBEntities db = new zvsLocalDBEntities())
                {
                    var q0 = from g in db.groups
                             select new
                             {
                                 id = g.id,
                                 name = g.name,
                                 count = g.group_devices.Count()
                             };

                    return new { success = true, groups = q0.ToArray() };
                }
            }

            if (request.Url.Segments.Length == 4 && request.Url.Segments[2].ToLower().Equals("group/") && request.HttpMethod == "GET")
            {

                int gID = 0;
                int.TryParse(request.Url.Segments[3], out gID);

                using (zvsLocalDBEntities db = new zvsLocalDBEntities())
                {
                    group group = db.groups.FirstOrDefault(g => g.id == gID);

                    if (group != null)
                    {
                        List<object> group_devices = new List<object>();
                        foreach (group_devices gd in group.group_devices)
                        {
                            group_devices.Add(new
                            {
                                id = gd.device.id,
                                name = gd.device.friendly_name,
                                type = gd.device.device_types.name
                            });
                        }
                        var g = new
                        {
                            id = group.id,
                            name = group.name,
                            devices = group_devices.ToArray()
                        };
                        return new { success = true, group = g };
                    }
                    else
                        return new { success = false, reason = "Group not found." };
                }
            }

            if (request.Url.Segments.Length == 5 && request.Url.Segments[2].ToLower().Equals("device/") &&
                request.Url.Segments[4].ToLower().StartsWith("commands") && request.HttpMethod == "GET")
            {
                int id = 0;
                int.TryParse(request.Url.Segments[3].Replace("/", ""), out id);
                if (id > 0)
                {
                    using (zvsLocalDBEntities db = new zvsLocalDBEntities())
                    {
                        device d = db.devices.FirstOrDefault(o => o.id == id);
                        if (d != null)
                        {
                            List<object> device_commands = new List<object>();
                            foreach (device_commands cmd in d.device_commands)
                            {
                                device_commands.Add(new
                                {
                                    id = cmd.id,
                                    type = "device",
                                    friendlyname = cmd.friendly_name,
                                    helptext = cmd.help,
                                    name = cmd.name
                                });
                            }

                            foreach (device_type_commands cmd in d.device_types.device_type_commands)
                            {
                                device_commands.Add(new
                                {
                                    id = cmd.id,
                                    type = "device_type",
                                    friendlyname = cmd.friendly_name,
                                    helptext = cmd.help,
                                    name = cmd.name
                                });
                            }

                            return new { success = true, device_commands = device_commands.ToArray() };

                        }
                        else
                            return new { success = false, reason = "Device not found." };
                    }
                }
            }

            if ((request.Url.Segments.Length == 5 || request.Url.Segments.Length == 6) && request.Url.Segments[2].ToLower().Equals("device/") &&
                request.Url.Segments[4].ToLower().StartsWith("command") && request.HttpMethod == "POST")
            {
                NameValueCollection postData = GetPostData(request);

                int c_id = 0;
                if (request.Url.Segments.Length == 6)
                    int.TryParse(request.Url.Segments[5].Replace("/", ""), out c_id);

                int id = 0;
                int.TryParse(request.Url.Segments[3].Replace("/", ""), out id);
                if (id > 0)
                {
                    using (zvsLocalDBEntities db = new zvsLocalDBEntities())
                    {
                        device d = db.devices.FirstOrDefault(o => o.id == id);

                        if (d != null)
                        {
                            string arg = postData["arg"];
                            string strtype = postData["type"];
                            string commandName = postData["name"];
                            string friendlyname = postData["friendlyname"];

                            switch (strtype)
                            {
                                case "device":
                                    {
                                        //If the user sends the command name in the post, ignore the ID if sent and doo a lookup by name
                                        device_commands cmd = null;
                                        if (!string.IsNullOrEmpty(friendlyname))
                                            cmd = d.device_commands.FirstOrDefault(c => c.friendly_name.Equals(friendlyname));
                                        else if (!string.IsNullOrEmpty(commandName))
                                            cmd = d.device_commands.FirstOrDefault(c => c.name.Equals(commandName));
                                        else if (c_id > 0)
                                            cmd = d.device_commands.FirstOrDefault(c => c.id == c_id);
                                        if (cmd != null)
                                        {
                                            using (zvsLocalDBEntities context = new zvsLocalDBEntities())
                                            {
                                                device_command_que.Run(new device_command_que
                                                {
                                                    device_id = d.id,
                                                    device_command_id = cmd.id,
                                                    arg = arg
                                                }, context);
                                            }
                                            return new { success = true };
                                        }
                                        else
                                            return new { success = false, reason = "Device command not found." };
                                    }
                                case "device_type":
                                    {
                                        //If the user sends the command name in the post, ignore the ID if sent and doo a lookup by name
                                        device_type_commands cmd = null;
                                        if (!string.IsNullOrEmpty(friendlyname))
                                            cmd = d.device_types.device_type_commands.FirstOrDefault(c => c.friendly_name.Equals(friendlyname));
                                        else if (!string.IsNullOrEmpty(commandName))
                                            cmd = d.device_types.device_type_commands.FirstOrDefault(c => c.name.Equals(commandName));
                                        else if (c_id > 0)
                                            cmd = d.device_types.device_type_commands.FirstOrDefault(c => c.id == c_id);

                                        if (cmd != null)
                                        {
                                            using (zvsLocalDBEntities context = new zvsLocalDBEntities())
                                            {
                                                device_type_command_que.Run(new device_type_command_que
                                                {
                                                    device_id = d.id,
                                                    device_type_command_id = cmd.id,
                                                    arg = arg
                                                }, context);
                                            }
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

            if (request.Url.Segments.Length == 3 && request.Url.Segments[2].ToLower().StartsWith("commands") && request.HttpMethod == "GET")
            {
                List<object> bi_commands = new List<object>();
                using (zvsLocalDBEntities db = new zvsLocalDBEntities())
                {
                    foreach (builtin_commands cmd in db.builtin_commands)
                    {
                        bi_commands.Add(new
                        {
                            id = cmd.id,
                            friendlyname = cmd.friendly_name,
                            helptext = cmd.help,
                            name = cmd.name
                        });
                    }
                    return new { success = true, builtin_commands = bi_commands.ToArray() };
                }
            }

            if ((request.Url.Segments.Length == 3 || request.Url.Segments.Length == 4) && request.Url.Segments[2].ToLower().StartsWith("command") && request.HttpMethod == "POST")
            {
                NameValueCollection postData = GetPostData(request);
                string arg = postData["arg"];
                string commandName = postData["name"];
                string friendlyname = postData["friendlyname"];

                int id = 0;
                if (request.Url.Segments.Length == 4)
                    int.TryParse(request.Url.Segments[3].Replace("/", ""), out id);

                using (zvsLocalDBEntities db = new zvsLocalDBEntities())
                {
                    builtin_commands cmd = null;
                    if (!string.IsNullOrEmpty(friendlyname))
                        cmd = db.builtin_commands.FirstOrDefault(c => c.friendly_name.Equals(friendlyname));
                    else if (!string.IsNullOrEmpty(commandName))
                        cmd = db.builtin_commands.FirstOrDefault(c => c.name.Equals(commandName));
                    else
                        cmd = db.builtin_commands.FirstOrDefault(c => c.id == id);

                    if (cmd != null)
                    {
                        using (zvsLocalDBEntities context = new zvsLocalDBEntities())
                        {
                            builtin_command_que.Run(new builtin_command_que
                            {
                                builtin_command_id = cmd.id,
                                arg = arg
                            }, context);
                        }
                        return new { success = true };
                    }
                }
            }

            if (request.Url.Segments.Length == 3 && request.Url.Segments[2].ToLower().StartsWith("logout") && request.HttpMethod == "POST")
            {
                Cookie c = new Cookie("zvs", "No Access");
                c.Expires = DateTime.Today.AddDays(-5);
                c.Domain = "";
                c.Path = "/";
                response.Cookies.Add(c);

                return new { success = true, isLoggedIn = false };
            }

            if (request.Url.Segments.Length == 3 && request.Url.Segments[2].ToLower().StartsWith("login") && request.HttpMethod == "GET")
            {
                return new { success = true, isLoggedIn = (request.Cookies.Count > 0 && request.Cookies["zvs"] != null && request.Cookies["zvs"].Value == CookieValue.ToString()) };
            }

            if (request.Url.Segments.Length == 3 && request.Url.Segments[2].ToLower().StartsWith("login") && request.HttpMethod == "POST")
            {
                NameValueCollection postData = GetPostData(request);
                using (zvsLocalDBEntities context = new zvsLocalDBEntities())
                {
                    if (postData["password"] == GetSettingValue("PASSWORD", context))
                    {
                        Cookie c = new Cookie("zvs", CookieValue.ToString());
                        c.Expires = DateTime.Today.AddDays(5);
                        c.Domain = "";
                        c.Path = "/";
                        response.Cookies.Add(c);

                        WriteToLog(Urgency.INFO, string.Format("[{0}] Login succeeded. UserAgent '{1}'", ip, request.UserAgent));

                        return new { success = true };
                    }
                    else
                    {
                        WriteToLog(Urgency.INFO, string.Format("[{0}] Login failed using password '{1}' and UserAgent '{2}'", ip, postData["password"], request.UserAgent));
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