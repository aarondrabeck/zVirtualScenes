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
                if (request.Url.Segments[2].ToLower().Contains("devices") && request.Url.Segments.Length == 3 && request.HttpMethod == "GET")
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

                if (request.RawUrl.Contains("/JSON/SendCmd"))
                {
                    long dID = 0;
                    long.TryParse(request.QueryString["id"], out dID);

                    string command = request.QueryString["cmd"];
                    string arg = request.QueryString["arg"];
                    string strtype = request.QueryString["type"];

                    if (!string.IsNullOrEmpty(strtype))
                    {
                        switch (strtype)
                        {
                            case "device":
                                {
                                    device d = zvsEntityControl.zvsContext.devices.FirstOrDefault(o => o.id == dID);
                                    if (d != null)
                                    {
                                        device_commands cmd = d.device_commands.FirstOrDefault(c => c.name == command);
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
                                    }
                                    break;
                                }
                            case "device_type":
                                {
                                    device d = zvsEntityControl.zvsContext.devices.FirstOrDefault(o => o.id == dID);
                                    if (d != null)
                                    {
                                        device_type_commands cmd = d.device_types.device_type_commands.FirstOrDefault(c => c.name == command);
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
                                    }
                                    break;
                                }
                            case "builtin":
                                {
                                    builtin_commands cmd = zvsEntityControl.zvsContext.builtin_commands.FirstOrDefault(c => c.name == command);
                                    if (cmd != null)
                                    {
                                        builtin_command_que.Run(new builtin_command_que
                                        {
                                            builtin_command_id = cmd.id,
                                            arg = arg
                                        });
                                        return new { success = true };
                                    }
                                    break;
                                }
                        }
                    }
                }

                if (request.RawUrl.ToLower().Contains("/scenes"))
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

                if (request.RawUrl.Contains("/JSON/GetSceneDetails"))
                {

                    long sID = 0;
                    long.TryParse(request.QueryString["id"], out sID);

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
                }

                if (request.RawUrl.Contains("/JSON/ActivateScene"))
                {
                    long sID = 0;
                    long.TryParse(request.QueryString["id"], out sID);

                    scene scene = zvsEntityControl.zvsContext.scenes.FirstOrDefault(s => s.id == sID);

                    if (scene != null)
                    {
                        string r = scene.RunScene();
                        return new { success = true, desc = r };

                    }
                }

                if (request.RawUrl.Contains("/JSON/GetGroupList"))
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

                if (request.RawUrl.ToLower().Contains("/groups"))
                {

                    long gID = 0;
                    long.TryParse(request.QueryString["id"], out gID);

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
                }
            }

            return new { success = false, reason = "Invalid Command" };    
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