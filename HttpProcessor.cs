using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Collections;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Net;
using System.Xml;

namespace zVirtualScenesApplication
{
    public class HttpProcessor
    {
        private static string LOG_INTERFACE = "HTTP";       
        public formzVirtualScenes zVirtualScenesMain;

        private static HttpListener httplistener = new HttpListener();
        private static System.Threading.AutoResetEvent listenForNextRequest = new System.Threading.AutoResetEvent(false);

        public bool isActive()
        {
            return httplistener.IsListening;
        }

        public void Start()
        {
            try
            {
                if (!httplistener.IsListening)
                {
                    httplistener.Prefixes.Add("http://*:" + zVirtualScenesMain.zVScenesSettings.ZHTTPPort + "/");
                    httplistener.Start();
                    ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(HttpListen));
                    zVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "Started listening on port " + zVirtualScenesMain.zVScenesSettings.ZHTTPPort, LOG_INTERFACE);
                }
            }
            catch (Exception ex)
            {
                zVirtualScenesMain.AddLogEntry(UrgencyLevel.ERROR, ex.Message, LOG_INTERFACE);
            }
        }

        public void Stop()
        {
            try
            {
                if (httplistener != null && httplistener.IsListening)
                {
                    httplistener.Stop();
                    zVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "Stopped listening for new requests.", LOG_INTERFACE);
                }
            }
            catch (Exception ex)
            {
                zVirtualScenesMain.AddLogEntry(UrgencyLevel.ERROR, ex.Message, LOG_INTERFACE);
            }
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
                zVirtualScenesMain.AddLogEntry(UrgencyLevel.ERROR, ex.Message, LOG_INTERFACE);
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
                catch //(Exception ex)
                {
                    //zVirtualScenesMain.AddLogEntry(UrgencyLevel.ERROR, ex.Message, LOG_INTERFACE);
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
                    string cmdResponse = parseRequest(context.Request.RawUrl, context.Request.UserHostName);
                    
                    //RESPOND
                    response.StatusCode = (int)HttpStatusCode.OK;
                    MemoryStream stream = new MemoryStream();
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(cmdResponse + Environment.NewLine);
                    stream.Write(buffer, 0, buffer.Length);

                    byte[] bytes = stream.ToArray();
                    response.OutputStream.Write(bytes, 0, bytes.Length);

                    //LOG.Warn("URL not a valid Zwave Request");
                    //response.StatusCode = (int)HttpStatusCode.Forbidden;
                    //response.OutputStream.WriteByte(0);
                    
                }

            }
            catch (Exception ex)
            {
                zVirtualScenesMain.AddLogEntry(UrgencyLevel.ERROR, ex.Message, LOG_INTERFACE);
            }
        }

        public string parseRequest(string rawURL, string userIP)
        {
            // string userIP = socket.Client.RemoteEndPoint.ToString();

            zVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "[" + userIP + "] " + rawURL, LOG_INTERFACE);

            //http:/localhost:8085//zVirtualScene?cmd=RunScene&Scene=0 
            //http:/localhost:8085/zVirtualScene?cmd=MultilevelPowerSwitch&node=2&level=0 
            //http://localhost:8085/zVirtualScene?cmd=ThermoStat&node=7&HeatCoolMode=-1&FanMode=-1&EngeryMode=-1&HeatPoint=-1&CoolPoint=-1
            //http://localhost:8085/zVirtualScene?cmd=ListDevices            
            //http://localhost:8085/zVirtualScene?cmd=ListScenes
            //http://localhost:8085/zVirtualScene?cmd=BinaryPowerSwitch&node=2&state=ON
            //zVirtualScene?cmd=RepollDevices
            string[] acceptedCMDs = new string[] { "/zVirtualScene?cmd=RunScene&Scene=" , 
                                                        "/zVirtualScene?cmd=MultilevelPowerSwitch&",
                                                        "/zVirtualScene?cmd=ThermoStat&", 
                                                        "/zVirtualScene?cmd=ListDevices",
                                                        "/zVirtualScene?cmd=ListScenes",                     
                                                        "/zVirtualScene?cmd=BinaryPowerSwitch&",
                                                        "/zVirtualScene?cmd=RepollDevices",
                                                        "/zVirtualScene?cmd=RepollDevice&node="};

            #region RUN SCENE
            if (rawURL.Contains(acceptedCMDs[0]))
            {
                int sceneID = 0;
                try { sceneID = Convert.ToInt32(rawURL.Remove(0, acceptedCMDs[0].Length)); }
                catch
                {
                    zVirtualScenesMain.AddLogEntry(UrgencyLevel.WARNING, "[" + userIP + "] No such scene.", LOG_INTERFACE);  //LOG
                    return "ERR: No such scene."; //FEEDBACK to USER                
                }
                if (sceneID > 0)
                {
                    foreach (Scene scene in zVirtualScenesMain.MasterScenes)
                        if (scene.ID == sceneID)
                        {
                            SceneResult result = scene.Run(zVirtualScenesMain);
                            zVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "[" + userIP + "] " + result.Description, LOG_INTERFACE);
                            return result.Description; 
                        }
                }
                zVirtualScenesMain.AddLogEntry(UrgencyLevel.WARNING, "[" + userIP + "] Cannot find scene.", LOG_INTERFACE);
                return "ERR: Cannot find scene.";
            }
            #endregion

            #region Run MultilevelPowerSwitch
            if (rawURL.Contains(acceptedCMDs[1]))
            {
                int node = 0;
                byte level = 0;
                try
                {
                    string prams = rawURL.Remove(0, acceptedCMDs[1].Length); //Strip CMD
                    string[] values = prams.Split('&'); //Get values

                    node = Convert.ToInt32(values[0].Remove(0, "node=".Length));
                    level = Convert.ToByte(values[1].Remove(0, "level=".Length));
                }
                catch
                {
                    zVirtualScenesMain.AddLogEntry(UrgencyLevel.WARNING, "[" + userIP + "] Error parsing command. ", LOG_INTERFACE);  //LOG
                    return "ERR: Error parsing command, check syntax. ";
                }

                if (node > 0)
                {
                    foreach (ZWaveDevice device in zVirtualScenesMain.MasterDevices)
                    {
                        if (device.NodeID == node && device.Type == ZWaveDevice.ZWaveDeviceTypes.MultiLevelSwitch)
                        {
                            Action action = (Action)device;

                            if (action != null)
                            {
                                //set Level
                                if (level == 255)
                                    level = 99;
                                action.Level = level;


                                ActionResult result = action.Run(zVirtualScenesMain);
                                zVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "[" + userIP + "] " + result.Description, LOG_INTERFACE);
                                return result.Description;
                            }
                        }
                    }
                }
                zVirtualScenesMain.AddLogEntry(UrgencyLevel.WARNING, "[" + userIP + "] Cannot find device.", LOG_INTERFACE);
                return "ERR: Cannot find device.";
            }
            #endregion

            #region RUN THERMO
            if (rawURL.Contains(acceptedCMDs[2]))
            {
                int node = 0;
                int HeatCoolMode = -1;
                int FanMode = -1;
                int EngeryMode = -1;
                int HeatPoint = -1;
                int CoolPoint = -1;

                try
                {
                    string prams = rawURL.Remove(0, acceptedCMDs[2].Length); //Strip CMD
                    string[] values = prams.Split('&'); //Get values

                    node = Convert.ToInt32(values[0].Remove(0, "node=".Length));
                    HeatCoolMode = Convert.ToInt32(values[1].Remove(0, "HeatCoolMode=".Length));
                    FanMode = Convert.ToInt32(values[2].Remove(0, "FanMode=".Length));
                    EngeryMode = Convert.ToInt32(values[3].Remove(0, "EngeryMode=".Length));
                    HeatPoint = Convert.ToInt32(values[4].Remove(0, "HeatPoint=".Length));
                    CoolPoint = Convert.ToInt32(values[5].Remove(0, "CoolPoint=".Length));
                }
                catch (Exception e)
                {
                    zVirtualScenesMain.AddLogEntry(UrgencyLevel.WARNING, "[" + userIP + "] Error parsing command - " + e, LOG_INTERFACE);  //LOG
                    return "ERR: Error parsing command, check syntax.";
                }

                if (node > 0)
                {
                    foreach (ZWaveDevice device in zVirtualScenesMain.MasterDevices)
                    {
                        if (device.NodeID == node && device.Type == ZWaveDevice.ZWaveDeviceTypes.Thermostat)
                        {
                            Action action = (Action)device;
                            action.HeatCoolMode = HeatCoolMode;
                            action.FanMode = FanMode;
                            action.EngeryMode = EngeryMode;
                            action.HeatPoint = HeatPoint;
                            action.CoolPoint = CoolPoint;


                            ActionResult result = action.Run(zVirtualScenesMain);
                            zVirtualScenesMain.AddLogEntry((UrgencyLevel)result.ResultType, "[" + userIP + "] " + result.Description, LOG_INTERFACE);
                            return result.Description;
                        }
                    }
                }
                zVirtualScenesMain.AddLogEntry(UrgencyLevel.WARNING, "[" + userIP + "] Cannot find device.", LOG_INTERFACE);
                return "ERR: Cannot find device.";
            }
            #endregion

            #region Device Listing
            if (rawURL.Contains(acceptedCMDs[3]))
            {
                zVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "[" + userIP + "] Requested a XML device listing.", LOG_INTERFACE);

                XmlWriterSettings xmlwritersettings = new XmlWriterSettings();
                xmlwritersettings.NewLineHandling = NewLineHandling.None;
                xmlwritersettings.Indent = false;

                StringWriter devices = new StringWriter();
                XmlSerializer DevicetoXML = new System.Xml.Serialization.XmlSerializer(zVirtualScenesMain.MasterDevices.GetType());
                DevicetoXML.Serialize(XmlWriter.Create(devices, xmlwritersettings), zVirtualScenesMain.MasterDevices);
                return devices.ToString();
            }
            #endregion

            #region scene Listing
            if (rawURL.Contains(acceptedCMDs[4]))
            {
                zVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "[" + userIP + "] Requested a XML scene listing.", LOG_INTERFACE);
                
                XmlWriterSettings xmlwritersettings = new XmlWriterSettings();
                xmlwritersettings.NewLineHandling = NewLineHandling.None;
                xmlwritersettings.Indent = false;

                StringWriter scenes = new StringWriter();
                XmlSerializer ScenetoXML = new System.Xml.Serialization.XmlSerializer(zVirtualScenesMain.MasterScenes.GetType());
                ScenetoXML.Serialize(XmlWriter.Create(scenes, xmlwritersettings), zVirtualScenesMain.MasterScenes);
                return scenes.ToString();
            }
            #endregion

            #region Run BinaryPowerSwitch
            if (rawURL.Contains(acceptedCMDs[5]))
            {
                int node = 0;
                string state = "";
                try
                {
                    string prams = rawURL.Remove(0, acceptedCMDs[5].Length); //Strip CMD
                    string[] values = prams.Split('&'); //Get values

                    node = Convert.ToInt32(values[0].Remove(0, "node=".Length));
                    state = values[1].Remove(0, "state=".Length);
                }
                catch
                {
                    zVirtualScenesMain.AddLogEntry(UrgencyLevel.WARNING, "[" + userIP + "] Error parsing command. ", LOG_INTERFACE);  //LOG
                    return "ERR: Error parsing command, check syntax.";
                }

                if (node > 0)
                {
                    foreach (ZWaveDevice device in zVirtualScenesMain.MasterDevices)
                    {
                        if (device.NodeID == node && device.Type == ZWaveDevice.ZWaveDeviceTypes.BinarySwitch)
                        {
                            Action action = (Action)device;

                            if (action != null)
                            {
                                //set Level
                                if (state == "ON")
                                    action.Level = 1;
                                else
                                    action.Level = 0;

                                ActionResult result = action.Run(zVirtualScenesMain);
                                zVirtualScenesMain.AddLogEntry((UrgencyLevel)result.ResultType, "[" + userIP + "] " + result.Description, LOG_INTERFACE);
                                return result.Description;
                            }
                        }
                    }
                }
                zVirtualScenesMain.AddLogEntry(UrgencyLevel.WARNING, "[" + userIP + "] Cannot find device.", LOG_INTERFACE);
                return "ERR: Cannot find device.";
            }
            #endregion

            #region Repoll ALL Devices
            if (rawURL.Contains(acceptedCMDs[6]))
            {
                zVirtualScenesMain.RepollDevices();
                zVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "[" + userIP + "] Repolled ZWave devices.", LOG_INTERFACE);
                return "Repolled ZWave devices.";
            }
            #endregion

            #region Repoll Device
            if (rawURL.Contains(acceptedCMDs[7]))
            {
                byte nodeID = 0;
                try { nodeID = Convert.ToByte(rawURL.Remove(0, acceptedCMDs[7].Length)); }
                catch
                {
                    zVirtualScenesMain.AddLogEntry(UrgencyLevel.WARNING, "[" + userIP + "] Invalid Node.", LOG_INTERFACE);  //LOG
                    return "ERR: Invalid Node.";
                }

                zVirtualScenesMain.RepollDevices(nodeID);
                zVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "[" + userIP + "] Repolled,  " + (nodeID == 0 ? "ALL devices." : "node " + nodeID + "."), LOG_INTERFACE);
                return "Repolled,  " + (nodeID == 0 ? "ALL devices." : "node " + nodeID + ".");
            }
            #endregion

            zVirtualScenesMain.AddLogEntry(UrgencyLevel.WARNING, "[" + userIP + "] Command not recognized. Ignored...", LOG_INTERFACE);
            return "ERR: Command not recognized. Ignored...";            
        }                   
            
  }
}
