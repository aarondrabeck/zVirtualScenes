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
using HTTPControlPlugin;
using zVirtualScenesCommon.Entity;
using zVirtualScenesCommon;
using System.Runtime.Serialization;

namespace HTTPControl
{
    [Export(typeof(Plugin))]
    public class HTTPControlPlugin : Plugin
    {
        public volatile bool isActive;

        private static HttpListener httplistener = new HttpListener();
        private static System.Threading.AutoResetEvent listenForNextRequest = new System.Threading.AutoResetEvent(false);

        public HTTPControlPlugin()
            : base("HTTPCONTROL",
               "HTTP Control Plugin",
                "This plug-in acts as a HTTP server to accept commands to query and control zVirtualScene devices."
                ) { }       

        public override void Initialize()
        {
            DefineOrUpdateSetting(new plugin_settings
            {
                name = "PORT",
                friendly_name = "HTTP Port",
                value = "8085",
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
                    int port = 1338;
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
            try
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
                    HTTPCMDResult CMDresult = parseRequest(context.Request.RawUrl);

                    if (CMDresult.HadError)
                        WriteToLog(Urgency.ERROR, string.Format("[{0}] {1}", context.Request.UserHostName, CMDresult.Description));
                    else
                        WriteToLog(Urgency.INFO, string.Format("[{0}] {1}", context.Request.UserHostName, CMDresult.Description));
                           
                    string XMLResult;
                    if (!CMDresult.DescriptionIsXML)
                        XMLResult = SerializeResult(CMDresult);
                    else
                        XMLResult = CMDresult.Description;

                    //RESPOND
                    response.StatusCode = (int)HttpStatusCode.OK;
                    MemoryStream stream = new MemoryStream();
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(XMLResult + Environment.NewLine);
                    stream.Write(buffer, 0, buffer.Length);

                    byte[] bytes = stream.ToArray();
                    response.OutputStream.Write(bytes, 0, bytes.Length);
                }

            }
            catch (Exception ex)
            {
                WriteToLog(Urgency.ERROR, ex.Message + ex.InnerException + "END");
            }
        }

        private HTTPCMDResult parseRequest(string rawURL)
        {
            HTTPCMDResult result = new HTTPCMDResult();
            result.Description = "Command not recognized.";
            result.HadError = true;

            // string userIP = socket.Client.RemoteEndPoint.ToString();
            //http://10.1.0.56:8085/zVirtualScene?cmd=RunScene&Scene=2
            //http://10.1.0.56:8085/zVirtualScene?cmd=SetBasic&ObjID=5&arg=10
            //http://10.1.0.56:8085/zVirtualScene?cmd=SendCommand&ObjID=5&commandType=Object&command=DYNAMIC_CMD_LEVEL&arg=99
            //http://10.1.0.56:8085/zVirtualScene?cmd=ListDevices            
            //http://10.1.0.56:8085/zVirtualScene?cmd=ListScenes
            //http://10.1.0.56:8085/zVirtualScene?cmd=RepollDevice&ObjID=0
            //http://10.1.0.56:8085/zVirtualScene?cmd=GetObjectCommands&ObjID=5

            string[] acceptedCMDs = new string[] { "/zVirtualScene?cmd=RunScene&Scene=" , 
                                                        "/zVirtualScene?cmd=SetBasic&",
                                                        "/zVirtualScene?cmd=SendCommand&", 
                                                        "/zVirtualScene?cmd=ListDevices",
                                                        "/zVirtualScene?cmd=ListScenes",    
                                                        "/zVirtualScene?cmd=GetDeviceCommands&deviceID=",
                                                        "/zVirtualScene?cmd=GetBuiltinCommands",
                                                        "/zVirtualScene?cmd=ListDeviceValues&deviceID=",  };

            #region RUN SCENE
            if (rawURL.Contains(acceptedCMDs[0]))
            {
                int sceneID = 0;
                int.TryParse(rawURL.Remove(0, acceptedCMDs[0].Length), out sceneID);

                if (sceneID > 0)
                {
                    scene scene = zvsEntityControl.zvsContext.scenes.FirstOrDefault(s => s.id == sceneID);

                    if (scene != null)
                    {
                        result.Description = scene.RunScene();
                        result.HadError = false;
                    }
                    else
                    {
                        result.HadError = true;
                        result.Description = "Could not locate scene with ID " + sceneID + ".";
                    }

                    return result;
                }
                else
                {
                    result.HadError = true;
                    result.Description = "Invalid Scene.";
                }

                return result;
            }
            #endregion

            #region Set Basic
            if (rawURL.Contains(acceptedCMDs[1]))
            {
                int dID = 0;
                string arg;

                string prams = rawURL.Remove(0, acceptedCMDs[1].Length); //Strip CMD                    
                string[] values = prams.Split('&'); //Get values

                try
                {
                    dID = Convert.ToInt32(values[0].Remove(0, "deviceID=".Length));
                    arg = values[1].Remove(0, "arg=".Length);
                }
                catch
                {
                    result.HadError = true;
                    result.Description = "Error parsing command.";
                    return result;
                }

                device d = zvsEntityControl.zvsContext.devices.FirstOrDefault(o => o.id == dID);
                if (d != null)
                {
                    device_commands cmd = d.device_commands.FirstOrDefault(c => c.name == "DYNAMIC_CMD_BASIC");
                    if (cmd != null)
                    {  
                        device_command_que.Run(new device_command_que
                        {
                            device_id = d.id,
                            device_command_id = cmd.id,
                            arg = arg
                        });

                        result.Description = string.Format("Executed command '{0}{1}' on '{2}'.", cmd.friendly_name, string.IsNullOrEmpty(arg) ? arg : " to " + arg, d.friendly_name);
                        result.HadError = false;
                        return result;
                    }
                    else
                    {
                        result.HadError = true;
                        result.Description = "Command not found.";
                        return result;
                    }
                }                

                result.HadError = true;
                result.Description = "Object not found.";
                return result;
            }
            #endregion

            #region Send Command
            if (rawURL.Contains(acceptedCMDs[2]))
            {
                int dID = 0;
                string command = string.Empty;
                string arg = string.Empty;
                string strtype = string.Empty;

                string prams = rawURL.Remove(0, acceptedCMDs[2].Length); //Strip CMD                    
                string[] values = prams.Split('&'); //Get values

                try
                {
                    dID = Convert.ToInt32(values[0].Remove(0, "deviceID=".Length));
                    strtype = values[1].Remove(0, "commandType=".Length);
                    command = values[2].Remove(0, "command=".Length);
                    arg = values[3].Remove(0, "arg=".Length);                    
                }
                catch
                {
                    result.HadError = true;
                    result.Description = "Error parsing command.";
                    return result;
                }

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

                                    result.Description = string.Format("Executed command '{0}{1}' on '{2}'.", cmd.friendly_name, string.IsNullOrEmpty(arg) ? arg : " to " + arg, d.friendly_name);
                                    result.HadError = false;
                                    return result;
                                }
                                else
                                {
                                    result.HadError = true;
                                    result.Description = "Device command not found.";
                                    return result;
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

                                    result.Description = string.Format("Executed command '{0}{1}' on '{2}'.", cmd.friendly_name, string.IsNullOrEmpty(arg) ? arg : " to " + arg, d.friendly_name);
                                    result.HadError = false;
                                    return result;
                                }
                                else
                                {
                                    result.HadError = true;
                                    result.Description = "Device type command not found.";
                                    return result;
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

                                result.Description = string.Format("Executed command '{0}{1}'", cmd.friendly_name, string.IsNullOrEmpty(arg) ? arg : " to " + arg);
                                result.HadError = false;
                                return result;
                            }
                            else
                            {
                                result.HadError = true;
                                result.Description = "Builtin command not found.";
                                return result;
                            }
                        }
                        
                }               

                result.HadError = true;
                result.Description = "Object not found.";
                return result;
            }
            #endregion

            #region Device Listing
            if (rawURL.Contains(acceptedCMDs[3]))
            {
                try
                {

                    List<zvsDevice> zvsDevices = new List<zvsDevice>();

                    foreach(device d in zvsEntityControl.zvsContext.devices)
                    {
                        zvsDevices.Add(new zvsDevice { friendly_name = d.friendly_name, id = d.id, type_name = d.device_types.name, friendly_level = d.GetLevelText()} );
                    }

                    XmlWriterSettings xmlwritersettings = new XmlWriterSettings();
                    xmlwritersettings.NewLineHandling = NewLineHandling.None;
                    xmlwritersettings.Indent = false;

                    StringWriter devices = new StringWriter();
                    XmlSerializer DevicetoXML = new System.Xml.Serialization.XmlSerializer(zvsDevices.GetType());
                    DevicetoXML.Serialize(XmlWriter.Create(devices, xmlwritersettings), zvsDevices);
                    result.HadError = false;
                    result.Description = devices.ToString();
                    result.DescriptionIsXML = true;
                    return result;
                }
                catch (Exception ex)
                {
                    result.HadError = true;
                    result.Description = "Error getting device list. " + ex.Message + ex.InnerException;
                    return result;
                }
            }
            #endregion

            #region Scene Listing
            if (rawURL.Contains(acceptedCMDs[4]))
            { 
                List<zvsScene> Scenes = new List<zvsScene>();

                foreach (scene s in zvsEntityControl.zvsContext.scenes)                                    
                    Scenes.Add(new zvsScene { ID = s.id, friendly_name = s.friendly_name } );
                

                XmlWriterSettings xmlwritersettings = new XmlWriterSettings();
                xmlwritersettings.NewLineHandling = NewLineHandling.None;
                xmlwritersettings.Indent = false;

                StringWriter scenes = new StringWriter();
                XmlSerializer ScenetoXML = new System.Xml.Serialization.XmlSerializer(Scenes.GetType());
                ScenetoXML.Serialize(XmlWriter.Create(scenes, xmlwritersettings), Scenes);
                result.HadError = false;
                result.Description = scenes.ToString();
                result.DescriptionIsXML = true;
                return result;
            }
            #endregion   

            #region Get Device Commands
            if (rawURL.Contains(acceptedCMDs[5]))
            {
                int dId = 0;
                int.TryParse(rawURL.Remove(0, acceptedCMDs[5].Length), out dId);

                device d = zvsEntityControl.zvsContext.devices.FirstOrDefault(o => o.id == dId);
                if (d != null)
                {
                    List<zvsCommand> zvscommands = new List<zvsCommand>();
                    foreach (device_commands cmd in d.device_commands)
                    {
                        zvscommands.Add(new zvsCommand
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
                        zvscommands.Add(new zvsCommand
                        {
                            CommandId = cmd.id,
                            CommandType = "device_type",
                            FriendlyName = cmd.friendly_name,
                            HelpText = cmd.help,
                            Name = cmd.name
                        });
                    }

                    XmlWriterSettings xmlwritersettings = new XmlWriterSettings();
                    xmlwritersettings.NewLineHandling = NewLineHandling.None;
                    xmlwritersettings.Indent = false;

                    StringWriter devices = new StringWriter();
                    XmlSerializer DevicetoXML = new System.Xml.Serialization.XmlSerializer(zvscommands.GetType());
                    DevicetoXML.Serialize(XmlWriter.Create(devices, xmlwritersettings), zvscommands);
                    result.HadError = false;
                    result.Description = devices.ToString();
                    result.DescriptionIsXML = true;
                    return result;
                }
                else
                {
                    result.HadError = true;
                    result.Description = "Device ID not vaild.";
                    return result;
                }
            }
            #endregion

            #region Get Builtin Commands
            if (rawURL.Contains(acceptedCMDs[6]))
            {
                
                    List<zvsCommand> zvscommands = new List<zvsCommand>();
                    foreach (builtin_commands cmd in zvsEntityControl.zvsContext.builtin_commands)
                    {
                        zvscommands.Add(new zvsCommand
                        {
                            CommandId = cmd.id,
                            CommandType = "builtin",
                            FriendlyName = cmd.friendly_name,
                            HelpText = cmd.help,
                            Name = cmd.name
                        });
                    }                  

                    XmlWriterSettings xmlwritersettings = new XmlWriterSettings();
                    xmlwritersettings.NewLineHandling = NewLineHandling.None;
                    xmlwritersettings.Indent = false;

                    StringWriter devices = new StringWriter();
                    XmlSerializer DevicetoXML = new System.Xml.Serialization.XmlSerializer(zvscommands.GetType());
                    DevicetoXML.Serialize(XmlWriter.Create(devices, xmlwritersettings), zvscommands);
                    result.HadError = false;
                    result.Description = devices.ToString();
                    result.DescriptionIsXML = true;
                    return result;
                
            }
            #endregion

            #region Get Device Commands
            if (rawURL.Contains(acceptedCMDs[7]))
            {
                int dId = 0;
                int.TryParse(rawURL.Remove(0, acceptedCMDs[7].Length), out dId);

                device d = zvsEntityControl.zvsContext.devices.FirstOrDefault(o => o.id == dId);
                if (d != null)
                {
                    List<zvsValues> zvsvalues = new List<zvsValues>();
                    foreach (device_values val in d.device_values)
                    {
                        zvsvalues.Add(new zvsValues
                        {
                            id = val.id,
                            genre = val.genre,
                            index = val.index,
                            label_name = val.label_name,
                            type = val.type,
                            value = val.value,
                            value_id = val.value_id
                        });
                    }                   

                    XmlWriterSettings xmlwritersettings = new XmlWriterSettings();
                    xmlwritersettings.NewLineHandling = NewLineHandling.None;
                    xmlwritersettings.Indent = false;

                    StringWriter devices = new StringWriter();
                    XmlSerializer DevicetoXML = new System.Xml.Serialization.XmlSerializer(zvsvalues.GetType());
                    DevicetoXML.Serialize(XmlWriter.Create(devices, xmlwritersettings), zvsvalues);
                    result.HadError = false;
                    result.Description = devices.ToString();
                    result.DescriptionIsXML = true;
                    return result;
                }
                else
                {
                    result.HadError = true;
                    result.Description = "Device ID not vaild.";
                    return result;
                }
            }
            #endregion
            
                     
            return result;
        }

        private string SerializeResult(HTTPCMDResult result)
        {
            XmlWriterSettings xmlwritersettings = new XmlWriterSettings();
            xmlwritersettings.NewLineHandling = NewLineHandling.None;
            xmlwritersettings.Indent = false;

            StringWriter result_str = new StringWriter();
            XmlSerializer ResulttoXML = new System.Xml.Serialization.XmlSerializer(typeof(HTTPCMDResult));
            ResulttoXML.Serialize(XmlWriter.Create(result_str, xmlwritersettings), result);
            return result_str.ToString();
        }
    }
}