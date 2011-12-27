using System.ComponentModel.Composition;
using System.Net.Sockets;
using System.Collections.Generic;
using System;
using System.Net;
using System.Threading;
using System.Text;
using System.Security.Cryptography;
using zVirtualScenesAPI;
using System.Data;
using System.ComponentModel;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using System.Linq;
using zVirtualScenesCommon.Entity;
using zVirtualScenesCommon;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using System.Reflection;
using System.Web;
using System.Collections.Specialized;

namespace zvsMobile
{
    [Export(typeof(Plugin))]
    public class zvsMobilePlugin : Plugin
    {
        public volatile bool isActive;

        private static HttpListener httplistener = new HttpListener();
        private static System.Threading.AutoResetEvent listenForNextRequest = new System.Threading.AutoResetEvent(false);

        public zvsMobilePlugin()
            : base("ZVSMOBILE",
               "zvsMobile Plugin",
                "This plug-in acts as a HTTP server to send respond to JSON AJAX requests."
                ) { }       

        public override void Initialize()
        {
            DefineOrUpdateSetting(new plugin_settings
            {
                name = "PORT",
                friendly_name = "HTTP Port",
                value = "9999",
                value_data_type = (int)Data_Types.INTEGER,
                description = "The port that HTTP will listen for commands on."
            });

            DefineOrUpdateSetting(new plugin_settings
            {
                name = "PASSWORD",
                friendly_name = "Password",
                value = "C52632B4BCDB6F8CF0F6E4545",
                value_data_type = (int)Data_Types.STRING,
                description = "Password that protects public facing web services."
            }); 
        }

        protected override bool StartPlugin()
        {
            try
            {
                if (!httplistener.IsListening)
                {
                    int port = 9999;
                    int.TryParse(GetSettingValue("PORT"), out port);
                    httplistener.Prefixes.Add("http://*:" + port + "/");
                    httplistener.Start();

                    ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(HttpListen));

                    WriteToLog(Urgency.INFO, string.Format("{0} plugin started on port {1}.", this.Friendly_Name, port));
                }
            }
            catch (Exception ex)
            {
                WriteToLog(Urgency.ERROR, "Error while starting. " + ex.Message);
            }

            IsReady = true;
            return true;
        }

        protected override bool StopPlugin()
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

            IsReady = false;
            return true;
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
        public override bool ActivateGroup(long groupID)
        {
            return true;
        }
        public override bool DeactivateGroup(long groupID)
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
                //Get response object

                object result_obj = GetResponse(context.Request);

                //Serialize depending type
                string RespondWith = "json";
                if (!string.IsNullOrEmpty(context.Request.QueryString["type"]) && context.Request.QueryString["type"].Trim().ToLower() == "xml")
                    RespondWith = "xml";

                switch (RespondWith)
                {
                    case "json":
                        {
                                
                            response.ContentType = "application/javascript;charset=utf-8";
                            response.StatusCode = (int)HttpStatusCode.OK;
                            MemoryStream stream = new MemoryStream();
                            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(toJSON(result_obj, context.Request.QueryString["callback"]));
                            stream.Write(buffer, 0, buffer.Length);
                            byte[] bytes = stream.ToArray();
                            response.OutputStream.Write(bytes, 0, bytes.Length); 
                            break; 
                        }
                    case "xml":
                        {
                            response.ContentType = "text/xml;charset=utf-8";
                            response.StatusCode = (int)HttpStatusCode.OK;
                            MemoryStream stream = new MemoryStream();
                            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(result_obj.ToXml().ToString());
                            stream.Write(buffer, 0, buffer.Length);
                            byte[] bytes = stream.ToArray();
                            response.OutputStream.Write(bytes, 0, bytes.Length);
                            break; 
                        }
                }
            }
        }

        private object GetResponse(HttpListenerRequest request)
        {
            if (request.Url.Segments.Length > 2 && request.Url.Segments[1].ToLower().Equals("api/"))
            {
                if (request.Url.Segments[2].ToLower().StartsWith("devices") && request.Url.Segments.Length == 3 && request.HttpMethod == "GET")
                {
                    List<object> devices = new List<object>();
                    foreach (device d in zvsEntityControl.zvsContext.devices.OrderBy(o => o.friendly_name))
                    {
                        var device = new
                        {
                            id = d.id,
                            name = d.friendly_name,
                            on_off = d.GetLevelMeter() > 0 ? "ON" : "OFF",
                            level = d.GetLevelMeter(),
                            level_txt = d.GetLevelText(),
                            type = d.device_types.name
                        };

                        devices.Add(device);
                    }
                    return new { success = true, devices = devices };
                }

                if (request.Url.Segments[2].ToLower().Equals("device/") && request.Url.Segments.Length == 4 && request.HttpMethod == "GET")
                {
                    long id = 0;
                    long.TryParse(request.Url.Segments[3].Replace("/",""), out id);
                    if (id > 0)
                    {
                        device d = zvsEntityControl.zvsContext.devices.FirstOrDefault(o => o.id == id);

                        if (d != null)
                        {
                            string on_off = string.Empty;
                            if (d.GetLevelMeter() == 0)
                                on_off = "OFF";
                            else if (d.GetLevelMeter() > 98)
                                on_off = "ON";
                            else
                                on_off = "DIM";

                            var details = new
                            {
                                id = d.id,
                                name = d.friendly_name,
                                on_off = on_off,
                                level = d.GetLevelMeter(),
                                level_txt = d.GetLevelText(),
                                type = d.device_types.name,
                                type_txt = d.device_types.friendly_name,
                                last_heard_from = d.last_heard_from.HasValue ? d.last_heard_from.Value.ToString() : "",
                                groups = d.GetGroups,
                                mode = d.device_values.FirstOrDefault(o => o.label_name == "Mode") == null ? "" : d.device_values.FirstOrDefault(o => o.label_name == "Mode").value,
                                fan_mode = d.device_values.FirstOrDefault(o => o.label_name == "Fan Mode") == null ? "" : d.device_values.FirstOrDefault(o => o.label_name == "Fan Mode").value,
                                op_state = d.device_values.FirstOrDefault(o => o.label_name == "Operating State") == null ? "" : d.device_values.FirstOrDefault(o => o.label_name == "Operating State").value,
                                fan_state = d.device_values.FirstOrDefault(o => o.label_name == "Fan State") == null ? "" : d.device_values.FirstOrDefault(o => o.label_name == "Fan State").value,
                                heat_p = d.device_values.FirstOrDefault(o => o.label_name == "Heating 1") == null ? "" : d.device_values.FirstOrDefault(o => o.label_name == "Heating 1").value,
                                cool_p = d.device_values.FirstOrDefault(o => o.label_name == "Cooling 1") == null ? "" : d.device_values.FirstOrDefault(o => o.label_name == "Cooling 1").value,
                                esm = d.device_values.FirstOrDefault(o => o.label_name == "Basic") == null ? "" : d.device_values.FirstOrDefault(o => o.label_name == "Basic").value
                            };
                            return new { success = true, details = details };
                        }
                        else
                            return new { success = false, reason = "Device not found." };
                    }
                }

                if (request.Url.Segments[2].ToLower().Equals("device_values/") && request.Url.Segments.Length == 4 && request.HttpMethod == "GET")
                {
                    long id = 0;
                    long.TryParse(request.Url.Segments[3].Replace("/", ""), out id);
                    if (id > 0)
                    {
                        device d = zvsEntityControl.zvsContext.devices.FirstOrDefault(o => o.id == id);

                        if (d != null)
                        {
                            List<object> values = new List<object>();
                            foreach (device_values v in d.device_values)
                            {
                                values.Add(new
                                {
                                    value_id = v.value_id,
                                    value = v.value,
                                    grene = v.genre,
                                    index = v.index, 
                                    read_only = v.read_only,
                                    label_name = v.label_name,
                                    type = v.type,
                                    id = v.id                                    
                                });
                            }

                            return new { success = true, values = values};
                        }
                        else
                            return new { success = false, reason = "Device not found." };
                    }
                }

                if (request.Url.Segments[2].ToLower().StartsWith("scenes") && request.Url.Segments.Length == 3 && request.HttpMethod == "GET")
                {
                    var q0 = from d in zvsEntityControl.zvsContext.scenes
                             select new
                             {
                                 id = d.id,
                                 name = d.friendly_name,
                                 is_running = d.is_running,
                                 cmd_count = d.scene_commands.Count()
                             };

                    return new { success = true, scenes = q0 };
                }

                if (request.Url.Segments[2].ToLower().Equals("scene/") && request.Url.Segments.Length == 4 && request.HttpMethod == "GET")
                {
                    long sID = 0;
                    long.TryParse(request.Url.Segments[3], out sID);

                    scene scene = zvsEntityControl.zvsContext.scenes.FirstOrDefault(s => s.id == sID);

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
                            cmds = s_cmds
                        };
                        return new { success = true, scene = s };
                    }
                    else
                        return new { success = false, reason = "Scene not found." };
                }

                if (request.Url.Segments[2].ToLower().Equals("scene/") && request.Url.Segments.Length == 4 && request.HttpMethod == "POST")
                {
                    NameValueCollection postData = GetPostData(request);                     
                    
                    long sID = 0;
                    long.TryParse(request.Url.Segments[3], out sID);
                                       
                    bool is_running = false;
                    bool.TryParse(postData["is_running"], out is_running);
                    string name = postData["name"];

                    scene scene = zvsEntityControl.zvsContext.scenes.FirstOrDefault(s => s.id == sID);

                    if (scene != null )
                    {
                        if (is_running)
                        {
                            string r = scene.RunScene();
                            return new { success = true, desc = r };
                        }

                        if (!string.IsNullOrEmpty(name))
                        {
                            scene.friendly_name = name;
                            zvsEntityControl.zvsContext.SaveChanges();
                            return new { success = true, desc = "Scene Name Updated." };
                        }
                       

                    }
                    else
                        return new { success = false, reason = "Scene not found." };
                }

                if (request.Url.Segments[2].ToLower().StartsWith("groups") && request.Url.Segments.Length == 3 && request.HttpMethod == "GET")
                {
                    var q0 = from g in zvsEntityControl.zvsContext.groups
                             select new
                             {
                                 id = g.id,
                                 name = g.name,
                                 count = g.group_devices.Count()
                             };

                    return new { success = true, groups = q0 };
                }

                if (request.Url.Segments[2].ToLower().Equals("group/") && request.Url.Segments.Length == 4 && request.HttpMethod == "GET")
                {

                    long gID = 0;
                    long.TryParse(request.Url.Segments[3], out gID);

                    group group = zvsEntityControl.zvsContext.groups.FirstOrDefault(g => g.id == gID);

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
                            devices = group_devices
                        };
                        return new { success = true, group = g };
                    }
                    else
                        return new { success = false, reason = "Group not found." };
                }

                if (request.Url.Segments[2].ToLower().Equals("device/") && request.Url.Segments.Length == 5 &&
                    request.Url.Segments[4].ToLower().StartsWith("commands") && request.HttpMethod == "GET")
                {
                    long id = 0;
                    long.TryParse(request.Url.Segments[3].Replace("/", ""), out id);
                    if (id > 0)
                    {
                        device d = zvsEntityControl.zvsContext.devices.FirstOrDefault(o => o.id == id);

                        if (d != null)
                        {
                            List<object> device_commands = new List<object>();
                            foreach (device_commands cmd in d.device_commands)
                            {
                                device_commands.Add(new
                                {
                                    CommandId = cmd.id,
                                    CommandType = "device",
                                    FriendlyName = cmd.friendly_name,
                                    HelpText = cmd.help,
                                    Name = cmd.name
                                });
                            }

                            foreach (device_type_commands cmd in d.device_types.device_type_commands)
                            {
                                device_commands.Add(new
                                {
                                    CommandId = cmd.id,
                                    CommandType = "device_type",
                                    FriendlyName = cmd.friendly_name,
                                    HelpText = cmd.help,
                                    Name = cmd.name
                                });
                            }

                            return new { success = true, device_commands = device_commands };
                        }
                        else
                            return new { success = false, reason = "Device not found." };
                    }
                }
                
                if (request.Url.Segments[2].ToLower().Equals("device/") && request.Url.Segments.Length == 6 &&
                    request.Url.Segments[4].ToLower().Equals("command/") && request.HttpMethod == "POST")
                {
                    NameValueCollection postData = GetPostData(request);  

                    long id = 0;
                    long.TryParse(request.Url.Segments[3].Replace("/", ""), out id);
                    if (id > 0)
                    {
                        device d = zvsEntityControl.zvsContext.devices.FirstOrDefault(o => o.id == id);

                        if (d != null)
                        {
                            long c_id = 0;
                            long.TryParse(request.Url.Segments[5].Replace("/", ""), out c_id);

                            string arg = postData["arg"];
                            string strtype = postData["type"];

                            switch (strtype)
                            {
                                case "device":
                                    {
                                        device_commands cmd = d.device_commands.FirstOrDefault(c => c.id == c_id);
                                        if (cmd != null)
                                        {
                                            device_command_que.Run(new device_command_que
                                            {
                                                device_id = d.id,
                                                device_command_id = cmd.id,
                                                arg = arg
                                            });

                                            return new { success = true };
                                        }  
                                        else
                                            return new { success = false, reason = "Device command not found." };
                                    }
                                case "device_type":
                                    {
                                        device_type_commands cmd = d.device_types.device_type_commands.FirstOrDefault(c => c.id == c_id);
                                        if (cmd != null)
                                        {
                                            device_type_command_que.Run(new device_type_command_que
                                            {
                                                device_id = d.id,
                                                device_type_command_id = cmd.id,
                                                arg = arg
                                            });
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

                if (request.Url.Segments[2].ToLower().StartsWith("builtin_commands") && request.Url.Segments.Length == 3 && request.HttpMethod == "GET")
                {   
                    List<object> bi_commands = new List<object>();                 
                    foreach (builtin_commands cmd in zvsEntityControl.zvsContext.builtin_commands)
                    {
                        bi_commands.Add(new
                        {
                            CommandId = cmd.id,
                            FriendlyName = cmd.friendly_name,
                            HelpText = cmd.help,
                            Name = cmd.name
                        });
                    }
                    return new { success = true, builtin_commands = bi_commands };                       
                }

                if (request.Url.Segments[2].ToLower().Equals("builtin_command/") && request.Url.Segments.Length == 4 && request.HttpMethod == "POST")
                {
                    NameValueCollection postData = GetPostData(request);
                    string arg = postData["arg"];

                    long id = 0;
                    long.TryParse(request.Url.Segments[3].Replace("/", ""), out id);

                    builtin_commands cmd = zvsEntityControl.zvsContext.builtin_commands.FirstOrDefault(c => c.id == id);
                    if (cmd != null)
                    {
                        builtin_command_que.Run(new builtin_command_que
                        {
                            builtin_command_id = cmd.id,
                            arg = arg
                        });
                        return new { success = true };
                    }
                }              

                if (request.Url.Segments[2].ToLower().StartsWith("login") && request.Url.Segments.Length == 3 && request.HttpMethod == "POST")
                {
                    NameValueCollection postData = GetPostData(request);

                    if(postData["password"] == GetSettingValue("PASSWORD"))
                        return new { success = true };
                    else
                        return new { success = false };
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

       

        //private string toXML<T>(T obj)
        //{
        //    XmlWriterSettings xmlwritersettings = new XmlWriterSettings();
        //    xmlwritersettings.NewLineHandling = NewLineHandling.None;
        //    xmlwritersettings.Indent = false;
        //    xmlwritersettings.Encoding = Encoding.UTF8;
        //    XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));

        //    string utf8;
        //    using (Utf8StringWriter writer = new Utf8StringWriter())
        //    {
        //        serializer.Serialize(XmlWriter.Create(writer, xmlwritersettings), obj);
        //        utf8 = writer.ToString();
        //    }
        //    return utf8;
        //}

        //public class Utf8StringWriter : StringWriter
        //{
        //    public override Encoding Encoding
        //    {
        //        get { return Encoding.UTF8; }
        //    }
        //}          
    
    }

    public static class ObjectExtensions
    {
        #region Private Fields
        private static readonly Type[] WriteTypes = new[] {
        typeof(string), typeof(DateTime), typeof(Enum), 
        typeof(decimal), typeof(Guid),
    };
        #endregion Private Fields
        #region .ToXml
        /// <summary>
        /// Converts an anonymous type to an XElement.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Returns the object as it's XML representation in an XElement.</returns>
        public static XElement ToXml(this object input)
        {
            return input.ToXml(null);
        }

        /// <summary>
        /// Converts an anonymous type to an XElement.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="element">The element name.</param>
        /// <returns>Returns the object as it's XML representation in an XElement.</returns>
        public static XElement ToXml(this object input, string element)
        {
            return _ToXml(input, element);
        }

        private static XElement _ToXml(object input, string element, int? arrayIndex = null, string arrayName = null)
        {
            if (input == null)
                return null;

            if (String.IsNullOrEmpty(element))
            {
                string name = input.GetType().Name;
                element = name.Contains("AnonymousType")
                    ? "Object"
                    : arrayIndex != null
                        ? arrayName + "_" + arrayIndex
                        : name;
            }

            element = XmlConvert.EncodeName(element);
            var ret = new XElement(element);

            if (input != null)
            {
                var type = input.GetType();
                var props = type.GetProperties();

                var elements = props.Select(p =>
                {
                    var pType = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                    var name = XmlConvert.EncodeName(p.Name);
                    var val = pType.IsArray ? "array" : p.GetValue(input, null);
                    var value = pType.IsArray
                        ? GetArrayElement(p, (Array)p.GetValue(input, null))
                        : pType.IsSimpleType() || pType.IsEnum
                            ? new XElement(name, val)
                            : val.ToXml(name);
                    return value;
                })
                .Where(v => v != null);

                ret.Add(elements);
            }

            return ret;
        }

        #region helpers
        /// <summary>
        /// Gets the array element.
        /// </summary>
        /// <param name="info">The property info.</param>
        /// <param name="input">The input object.</param>
        /// <returns>Returns an XElement with the array collection as child elements.</returns>
        private static XElement GetArrayElement(PropertyInfo info, Array input)
        {
            var name = XmlConvert.EncodeName(info.Name);

            XElement rootElement = new XElement(name);

            var arrayCount = input == null ? 0 : input.GetLength(0);

            for (int i = 0; i < arrayCount; i++)
            {
                var val = input.GetValue(i);
                XElement childElement = val.GetType().IsSimpleType() ? new XElement(name + "_" + i, val) : _ToXml(val, null, i, name);

                rootElement.Add(childElement);
            }

            return rootElement;
        }

        #region .IsSimpleType
        public static bool IsSimpleType(this Type type)
        {
            return type.IsPrimitive || WriteTypes.Contains(type);
        }
        #endregion .IsSimpleType

        #endregion helpers
        #endregion .ToXml
    }
}