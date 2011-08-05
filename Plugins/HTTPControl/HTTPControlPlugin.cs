using System.ComponentModel.Composition;
using System.Net.Sockets;
using System.Collections.Generic;
using System;
using System.Net;
using System.Threading;
using System.Text;
using System.Security.Cryptography;
using zVirtualScenesAPI;
using zVirtualScenesAPI.Structs;
using System.Data;
using zVirtualScenesAPI.Events;
using zVirtualScenesApplication.Structs;
using System.ComponentModel;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using HTTPControlPlugin;

namespace HTTPControl
{
    [Export(typeof(Plugin))]
    public class HTTPControlPlugin : Plugin
    {
        public volatile bool isActive;

        private static HttpListener httplistener = new HttpListener();
        private static System.Threading.AutoResetEvent listenForNextRequest = new System.Threading.AutoResetEvent(false);

        public HTTPControlPlugin()
            : base("HTTPControl")
        {
            PluginName = "HTTP Control";
        }

        public override void Initialize()
        {
            API.InstallObjectType("HTTPCONTROL", false);
            API.NewObject(1, "HTTPCONTROL", "HTTP Control");
            API.DefineSetting("Port", "8085", ParamType.INTEGER, "The port that HTTP will listen for commands on.");
        }

        protected override bool StartPlugin()
        {
            try
            {
                if (!httplistener.IsListening)
                {
                    int port = 1338;
                    int.TryParse(API.GetSetting("Port"), out port);
                    httplistener.Prefixes.Add("http://*:" + port + "/");
                    httplistener.Start();

                    ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(HttpListen));

                    API.WriteToLog(Urgency.INFO, string.Format("{0} plugin started on port {1}.", PluginName, port));
                }
            }
            catch (Exception ex)
            {
                API.WriteToLog(Urgency.ERROR, "Error while starting. " + ex.Message);
            }

            IsReady = true;
            return true;
        }

        protected override bool StopPlugin()
        {
            API.WriteToLog(Urgency.INFO, PluginName + " plugin ended.");

            try
            {
                if (httplistener != null && httplistener.IsListening)
                {
                    httplistener.Stop();
                    API.WriteToLog(Urgency.INFO, string.Format("{0} plugin shutdown.", PluginName));
                }
            }
            catch (Exception ex)
            {
                API.WriteToLog(Urgency.ERROR, "Error while shuting down. " + ex.Message);
            }

            IsReady = false;
            return true;
        }

        protected override void SettingChanged(string settingName, string settingValue)
        {
           
        }

        public override void ProcessCommand(QuedCommand cmd)
        {
        }

        public override void Repoll(string id)
        {
        }

        public override void ActivateGroup(string GroupName)
        { }

        public override void DeactivateGroup(string GroupName)
        { }

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
                API.WriteToLog(Urgency.ERROR, ex.Message);
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
                        API.WriteToLog(Urgency.ERROR, string.Format("[{0}] {1}", context.Request.UserHostName, CMDresult.Description));
                    else
                        API.WriteToLog(Urgency.INFO, string.Format("[{0}] {1}", context.Request.UserHostName, CMDresult.Description));
                           
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
                API.WriteToLog(Urgency.ERROR, ex.Message + ex.InnerException + "END");
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
                                                        "/zVirtualScene?cmd=ListObjects",
                                                        "/zVirtualScene?cmd=ListScenes",    
                                                        "/zVirtualScene?cmd=RepollDevice&ObjID=",
                                                        "/zVirtualScene?cmd=GetObjectCommands&ObjID="};

            #region RUN SCENE
            if (rawURL.Contains(acceptedCMDs[0]))
            {
                int sceneID = 0;
                int.TryParse(rawURL.Remove(0, acceptedCMDs[0].Length), out sceneID);

                if (sceneID > 0)
                {
                    Scene scene = API.Scenes.GetScene(sceneID);

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
                int ObjID = 0;
                byte level = 0;

                string prams = rawURL.Remove(0, acceptedCMDs[1].Length); //Strip CMD                    
                string[] values = prams.Split('&'); //Get values

                try
                {
                    ObjID = Convert.ToInt32(values[0].Remove(0, "ObjID=".Length));
                    level = Convert.ToByte(values[1].Remove(0, "arg=".Length));
                }
                catch
                {
                    result.HadError = true;
                    result.Description = "Error parsing command.";
                    return result;
                }


                if (ObjID > 0)
                {
                    string ObjName = API.Object.GetObjectName(ObjID);
                    int commandId = API.Commands.GetObjectCommandId(ObjID, "DYNAMIC_CMD_BASIC");

                    if (commandId > 0)
                    {
                        API.Commands.InstallQueCommandAndProcess(new QuedCommand
                        {
                            cmdtype = cmdType.Object,
                            CommandId = commandId,
                            ObjectId = ObjID,
                            Argument = level.ToString()
                        });

                        result.HadError = false;
                        result.Description = ObjName + " basic set to " + level;
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
                int ObjID = 0;
                string command = string.Empty;
                string arg = string.Empty;
                string strtype = string.Empty;
                cmdType type = cmdType.Object;




                string prams = rawURL.Remove(0, acceptedCMDs[2].Length); //Strip CMD                    
                string[] values = prams.Split('&'); //Get values

                try
                {
                    ObjID = Convert.ToInt32(values[0].Remove(0, "ObjID=".Length));
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

                if (!string.IsNullOrEmpty(strtype))
                {
                    Enum.TryParse(strtype, out type);
                }
                else
                {
                    result.HadError = true;
                    result.Description = "Error parsing command type.";
                    return result;
                }

                if (ObjID > 0)
                {
                    string ObjName = API.Object.GetObjectName(ObjID);

                    int commandId = 0;
                    if(type == cmdType.Object)
                        commandId = API.Commands.GetObjectCommandId(ObjID, command);
                    else if(type == cmdType.ObjectType)
                        commandId = API.Commands.GetObjectTypeCommandId(ObjID, command);

                    if (commandId > 0)
                    {
                        API.Commands.InstallQueCommandAndProcess(new QuedCommand
                        {
                            cmdtype = type,
                            CommandId = commandId,
                            ObjectId = ObjID,
                            Argument = arg
                        });

                        result.HadError = false;
                        result.Description = ObjName + " " + command + " set to " + arg;
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

            #region Object Listing
            if (rawURL.Contains(acceptedCMDs[3]))
            {
                try
                {
                    List<zvsObject> Devices = new List<zvsObject>();
                    DataTable dt = API.Object.GetObjects(true);

                    foreach (DataRow dr in dt.Rows)
                    {
                        zvsObject device = new zvsObject();
                        int.TryParse(dr["id"].ToString(), out device.ObjectID);
                        device.Name = dr["txt_object_name"].ToString();
                        device.Type = dr["txt_object_type"].ToString();                        
                        Devices.Add(device);
                    }

                    XmlWriterSettings xmlwritersettings = new XmlWriterSettings();
                    xmlwritersettings.NewLineHandling = NewLineHandling.None;
                    xmlwritersettings.Indent = false;

                    StringWriter devices = new StringWriter();
                    XmlSerializer DevicetoXML = new System.Xml.Serialization.XmlSerializer(Devices.GetType());
                    DevicetoXML.Serialize(XmlWriter.Create(devices, xmlwritersettings), Devices);
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
                BindingList<Scene> APIScenes = API.Scenes.GetScenes();

                List<zvsScene> Scenes = new List<zvsScene>();

                foreach (zVirtualScenesApplication.Structs.Scene APIScene in APIScenes)
                {
                    zvsScene scene = new zvsScene();
                    scene.ID = APIScene.id;
                    scene.Name = APIScene.txt_name;
                    Scenes.Add(scene);
                }

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

            #region Repoll
            if (rawURL.Contains(acceptedCMDs[5]))
            {
                int ObjID = 0;
                int.TryParse(rawURL.Remove(0, acceptedCMDs[5].Length), out ObjID);                            

                if (ObjID == 0)  //REPOLL ALL
                {
                    int cmdId = API.Commands.GetBuiltinCommandId("REPOLL_ALL");
                    API.Commands.InstallQueCommandAndProcess(new QuedCommand { 
                        cmdtype = cmdType.Builtin, 
                        CommandId = cmdId });

                    result.HadError = false;
                    result.Description = "Repolling All Objects...";
                    return result;
                }
                else
                {
                    int cmdId = API.Commands.GetBuiltinCommandId("REPOLL_ME");                    
                    API.Commands.InstallQueCommandAndProcess(new QuedCommand { 
                        cmdtype = cmdType.Builtin, 
                        CommandId = cmdId,
                        Argument = ObjID.ToString()
                    });

                    result.HadError = false;
                    result.Description = "Repolling object #" + ObjID;
                    return result;
                }
            }
            #endregion

            #region Get Object Commands
            if (rawURL.Contains(acceptedCMDs[6]))
            {
                int ObjID = 0;
                int.TryParse(rawURL.Remove(0, acceptedCMDs[6].Length), out ObjID);

                if (ObjID > 0)
                {
                    List<Command> commands = new List<Command>();
                    commands = API.Commands.GetAllObjectCommandsForObjectasCMD(ObjID);
                    commands.AddRange(API.Commands.GetAllObjectTypeCommandsForObjectasCMD(ObjID));

                    List<zvsObjectCommand> zvsObjectCommands = new List<zvsObjectCommand>();
                    foreach (Command cmd in commands)
                    {
                        zvsObjectCommand zvsObjCmd = new zvsObjectCommand();
                        zvsObjCmd.CommandId = cmd.CommandId;
                        zvsObjCmd.FriendlyName = cmd.FriendlyName;
                        zvsObjCmd.HelpText = cmd.HelpText;
                        zvsObjCmd.Name = cmd.Name;
                        zvsObjCmd.paramType = cmd.paramType;
                        zvsObjCmd.cmdtype = cmd.cmdtype;
                        zvsObjectCommands.Add(zvsObjCmd);
                    }

                    XmlWriterSettings xmlwritersettings = new XmlWriterSettings();
                    xmlwritersettings.NewLineHandling = NewLineHandling.None;
                    xmlwritersettings.Indent = false;

                    StringWriter devices = new StringWriter();
                    XmlSerializer DevicetoXML = new System.Xml.Serialization.XmlSerializer(zvsObjectCommands.GetType());
                    DevicetoXML.Serialize(XmlWriter.Create(devices, xmlwritersettings), zvsObjectCommands);
                    result.HadError = false;
                    result.Description = devices.ToString();
                    result.DescriptionIsXML = true;
                    return result;
                }
                else
                {
                    result.HadError = true;
                    result.Description = "Object ID not vaild.";
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