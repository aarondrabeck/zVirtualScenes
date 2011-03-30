using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Xml;
using System.IO;
using System.Xml.Serialization;

namespace zVirtualScenesApplication
{
    public class SocketClient
    {
        public Socket ClientsSocket { get; set; }
        public type ClientType { get; set; }
        public bool isAuthenticated { get; set; }
        public byte[] dataBuffer = new byte[1024];
        
        public enum type
        {
            Unknown,
            iViewer,
            Android,
            iphone,
            iPad,
            Web, 
            Blackberry
        }

        public SocketClient()
        {
            this.isAuthenticated = false; 
        }

        /// <summary>
        /// Sends string to socket client and return the sent message for logging.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public void SendMessage(string msg, formzVirtualScenes zVirtualScenesMain)
        {
            if (msg.Length > 0)
            {
                byte[] byData = System.Text.Encoding.UTF8.GetBytes(msg + Environment.NewLine);
                this.ClientsSocket.Send(byData);
            }

            if (zVirtualScenesMain.zVScenesSettings.XMLSocketVerbose)
                zVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "SENT [" + this.ClientsSocket.RemoteEndPoint.ToString() + "]  " + msg, XMLSocketInterface.LOG_INTERFACE);
        }
    }

    public class XMLSocketInterface
    {
        public const string LOG_INTERFACE = "XML SOCKET";

        public formzVirtualScenes zVirtualScenesMain;
        public List<SocketClient> SocketClients = new List<SocketClient>();
        public AsyncCallback ProcessDataCallBack;
        private Socket MainListeningSocket;
        public volatile bool isLisenting;        

        public XMLSocketInterface()
        {
            isLisenting = false;
        }

        public void StartListening()
        {
            if (MainListeningSocket == null || !isLisenting)
            {
                try
                {
                    isLisenting = true;
                    MainListeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    MainListeningSocket.Bind(new IPEndPoint(IPAddress.Any, zVirtualScenesMain.zVScenesSettings.XMLSocketPort));
                    MainListeningSocket.Listen(zVirtualScenesMain.zVScenesSettings.XMLSocketMaxConnections);
                    MainListeningSocket.BeginAccept(new AsyncCallback(OnConnect), null);
                    zVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "Listening on port " + zVirtualScenesMain.zVScenesSettings.XMLSocketPort + ".", LOG_INTERFACE);
                }
                catch (SocketException e)
                {
                    zVirtualScenesMain.AddLogEntry(UrgencyLevel.ERROR, "Failed to Open Socket - " + e, LOG_INTERFACE);
                }
            }
        }

        public void StopListening()
        {
            if (MainListeningSocket != null && isLisenting)
            {
                MainListeningSocket.Close();   

                //Boot each connected client.
                foreach (SocketClient client in SocketClients)
                {
                    if (client.ClientsSocket != null && client.ClientsSocket.Connected)
                    {
                        client.ClientsSocket.Close();
                    }
                }

                //Stop listening for new conenctions
                             
                zVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "Stopped Listening for new connections.", LOG_INTERFACE);
                isLisenting = false;
            }
        }

        private void OnConnect(IAsyncResult asyn)
        {
            try
            {
                SocketClient newClient = new SocketClient();
                newClient.ClientsSocket = MainListeningSocket.EndAccept(asyn);              

                lock (SocketClients)
                    SocketClients.Add(newClient);

                zVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "Client [" + newClient.ClientsSocket.RemoteEndPoint.ToString() + "] connected. Connection count now " + SocketClients.Count() + ".", LOG_INTERFACE);
                newClient.SendMessage("<zVirtualScenesSocketServer ver=\"1.1\" />", zVirtualScenesMain);
                WaitForClientData(newClient.ClientsSocket.RemoteEndPoint);
                
                //Listen for new clients
                MainListeningSocket.BeginAccept(new AsyncCallback(OnConnect), null);
            }
            catch (ObjectDisposedException)
            {
            }
            catch (SocketException se)
            {
                zVirtualScenesMain.AddLogEntry(UrgencyLevel.ERROR, "SE: " + se.Message, LOG_INTERFACE);
            }
        }

        private void HandleDisconnect(SocketClient client)
        {            
            try
            {
                lock (SocketClients)
                    SocketClients.Remove(client);

                zVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "Client [" + client.ClientsSocket.RemoteEndPoint.ToString() + "] disconnected. Connection count now " + SocketClients.Count() + ".", LOG_INTERFACE);
                
                client.ClientsSocket.Close();
                client.ClientsSocket = null;
            }
            catch (Exception e)
            {
                zVirtualScenesMain.AddLogEntry(UrgencyLevel.ERROR, "SD: " + e.Message, LOG_INTERFACE);
            }
        }

        /// <summary>
        /// Sends a message to all authenicated clients
        /// </summary>
        /// <param name="msg"></param>
        public void BroadcastMessage(string msg)
        {
            if (this.isLisenting)
            {
                foreach (SocketClient client in SocketClients)
                    if (client.ClientsSocket != null && client.isAuthenticated)
                        client.SendMessage(msg, zVirtualScenesMain);
            }
        }

        public void WaitForClientData(EndPoint UserIPPort)
        {
            try
            {
                if (ProcessDataCallBack == null)
                    ProcessDataCallBack = new AsyncCallback(OnDataReceived);

                foreach(SocketClient client in SocketClients)
                    if (client.ClientsSocket.RemoteEndPoint == UserIPPort)
                    {
                        client.ClientsSocket.BeginReceive(client.dataBuffer, 0, client.dataBuffer.Length, SocketFlags.None, ProcessDataCallBack, client);
                        break;
                    }
            }
            catch (SocketException se)
            {
                zVirtualScenesMain.AddLogEntry(UrgencyLevel.ERROR, "SE: " + se, LOG_INTERFACE);
            }
        }

        public void OnDataReceived(IAsyncResult asyn)
        {
            SocketClient client = (SocketClient)asyn.AsyncState;

            //string XMLDelaration = "<?xml version=\"1.0\" encoding=\"utf-16\"?>";

            if (!client.ClientsSocket.Connected)
                return;

            try
            {                
                int iRx = client.ClientsSocket.EndReceive(asyn);

                //this socket was closed
                if (iRx == 0)
                {
                    HandleDisconnect(client);
                    return;
                }

                if (iRx > 2)
                {
                    char[] chars = new char[iRx + 1];
                    // Extract the characters as a buffer
                    System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                    int charLen = d.GetChars(client.dataBuffer, 0, iRx, chars, 0);
                    string data = new string(chars);

                    if(zVirtualScenesMain.zVScenesSettings.XMLSocketVerbose)
                        zVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "RECV [" + client.ClientsSocket.RemoteEndPoint.ToString() + "] " + data, LOG_INTERFACE);

                    string[] commands = data.Split('\n');

                    foreach (string command in commands)
                    {
                        if (command.Length > 0)
                        {
                            if (command.Length <= 2)
                                continue;
                            
                            string cmd = command.TrimEnd(Environment.NewLine.ToCharArray());
                            try
                            {
                                XmlDocument doc = new XmlDocument();
                                doc.LoadXml(cmd);

                                XmlElement XMLElement = doc.DocumentElement;

                                //Authentication differnt clients
                                if (!client.isAuthenticated)
                                {
                                    
                                    if(XMLElement.Name == "auth")
                                    {
                                        client.ClientType = (SocketClient.type)Enum.Parse(typeof(SocketClient.type), XMLElement.GetAttribute("type"));
                                        
                                        //iViewer
                                        if(zVirtualScenesMain.zVScenesSettings.XMLSocketAllowiViewer && client.ClientType == SocketClient.type.iViewer)
                                        {
                                                client.isAuthenticated = true;                                                
                                                client.SendMessage("<auth result=\"OK\" />", zVirtualScenesMain);
                                                WaitForClientData(client.ClientsSocket.RemoteEndPoint);
                                                return;
                                        }
                                        
                                        //Android
                                        if(zVirtualScenesMain.zVScenesSettings.XMLSocketAllowAndroid && client.ClientType == SocketClient.type.Android)
                                        {                                            
                                            string hashedPassword = XMLElement.GetAttribute("password");

                                            if (hashedPassword == zVirtualScenesMain.zVScenesSettings.XMLSocketAndroidPassword)
                                            {
                                                client.isAuthenticated = true;
                                                client.SendMessage("<auth result=\"OK\" />", zVirtualScenesMain);
                                            }
                                            else
                                                client.SendMessage("<auth result=\"Invalid Password\" />", zVirtualScenesMain);

                                            WaitForClientData(client.ClientsSocket.RemoteEndPoint);
                                            return;                                                
                                        }
                                        throw new Exception("Authentication Failed.");
                                    }
                                    throw new Exception("Invalid XML syntax or XML parameters.");                                   
                                }
                                //Handle Commands
                                XmlWriterSettings xmlwritersettings = new XmlWriterSettings();
                                xmlwritersettings.NewLineHandling = NewLineHandling.None;
                                xmlwritersettings.Indent = false;

                                if (client.isAuthenticated) 
                                {
                                    byte nodeID;
                                    byte level;
                                    switch (XMLElement.Name)
                                    {
                                        case "listdevices":
                                            StringWriter devices = new StringWriter();
                                            XmlSerializer DevicetoXML = new System.Xml.Serialization.XmlSerializer(zVirtualScenesMain.MasterDevices.GetType());
                                            DevicetoXML.Serialize(XmlWriter.Create(devices,xmlwritersettings),zVirtualScenesMain.MasterDevices);
                                            client.SendMessage(devices.ToString(), zVirtualScenesMain);
                                            WaitForClientData(client.ClientsSocket.RemoteEndPoint);
                                            return;
                                        case "listscenes":
                                            StringWriter scenes = new StringWriter();
                                            XmlSerializer ScenetoXML = new System.Xml.Serialization.XmlSerializer(zVirtualScenesMain.MasterScenes.GetType());
                                            ScenetoXML.Serialize(XmlWriter.Create(scenes, xmlwritersettings), zVirtualScenesMain.MasterScenes);
                                            client.SendMessage(scenes.ToString(), zVirtualScenesMain);
                                            WaitForClientData(client.ClientsSocket.RemoteEndPoint);
                                            return;

                                        case "runscene":
                                            int SceneID = Convert.ToInt32( XMLElement.GetAttribute("id"));

                                            if (SceneID > 0)
                                            {
                                                foreach (Scene scene in zVirtualScenesMain.MasterScenes)
                                                    if (scene.ID == SceneID)
                                                    {
                                                        SceneResult result = scene.Run(zVirtualScenesMain);
                                                        zVirtualScenesMain.AddLogEntry((UrgencyLevel)result.ResultType, "[" + client.ClientsSocket.RemoteEndPoint.ToString() + "] " + result.Description,LOG_INTERFACE);
                                                        client.SendMessage("<runscene result=\"" + result.Description +"\" />", zVirtualScenesMain);
                                                        WaitForClientData(client.ClientsSocket.RemoteEndPoint);
                                                        return;
                                                    }
                                            }
                                            client.SendMessage("<runscene result=\"Invalid scene ID.\" />", zVirtualScenesMain);
                                            WaitForClientData(client.ClientsSocket.RemoteEndPoint);
                                            return; 

                                        case "repoll":
                                            nodeID = Convert.ToByte(XMLElement.GetAttribute("node"));
                                            zVirtualScenesMain.RepollDevices(nodeID);

                                            zVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "[" + client.ClientsSocket.RemoteEndPoint.ToString() + "] Repolled,  " + (nodeID == 0 ? "ALL devices." : "node " + nodeID + "."), LOG_INTERFACE);
                                            client.SendMessage("<repoll result=\"Repolled " + (nodeID == 0 ? "ALL devices." : "node " + nodeID + ".") +"\" />", zVirtualScenesMain);
                                            WaitForClientData(client.ClientsSocket.RemoteEndPoint);
                                            return; 

                                        case "activategroup":
                                            string group = XMLElement.GetAttribute("group");
                                            level = Convert.ToByte(XMLElement.GetAttribute("level"));
                                            SceneResult groupsetresult = zVirtualScenesMain.ActivateGroup(group, level);
                                            zVirtualScenesMain.AddLogEntry((UrgencyLevel)groupsetresult.ResultType, "[" + client.ClientsSocket.RemoteEndPoint.ToString() + "] " + groupsetresult.Description, LOG_INTERFACE);
                                            client.SendMessage("<activategroup result=\"" + groupsetresult.Description + "\" alertlevel=\"" + groupsetresult.ResultType + "\" />", zVirtualScenesMain);
                                            WaitForClientData(client.ClientsSocket.RemoteEndPoint);
                                            return;

                                        case "alterlevel":
                                            nodeID = Convert.ToByte(XMLElement.GetAttribute("node"));
                                            int change = Convert.ToInt32(XMLElement.GetAttribute("changelevel"));

                                            //Find device and create action 
                                            foreach (ZWaveDevice device in zVirtualScenesMain.MasterDevices)
                                            {
                                                if (device.NodeID == nodeID)
                                                {
                                                    int newLevel = device.Level;
                                                    if (change > 0)
                                                        newLevel = device.Level + change;  
                                                    else if (change < 0)
                                                        newLevel = device.Level - Math.Abs(change);

                                                    if (newLevel < 0)
                                                        newLevel = 0;
                                                    else if (newLevel > 99)
                                                        newLevel = 99;

                                                    level = Convert.ToByte(newLevel);
                                                    Action action = (Action)device;

                                                    if (action.ZWaveType == ZWaveDevice.ZWaveDeviceTypes.MultiLevelSwitch)
                                                    {
                                                        action.Level = level;
                                                        ActionResult result = action.Run(zVirtualScenesMain);
                                                        client.SendMessage("<alterlevel result=\"" + result.Description + "\" alertlevel=\"" + result.ResultType + "\" />", zVirtualScenesMain);
                                                        WaitForClientData(client.ClientsSocket.RemoteEndPoint);
                                                        return;
                                                    }
                                                }
                                            }

                                            client.SendMessage("<alterlevel result=\"Device not found.\" alertlevel=\"Error\" />", zVirtualScenesMain);
                                            WaitForClientData(client.ClientsSocket.RemoteEndPoint);
                                            return;

                                        case "setswitch":
                                            nodeID = Convert.ToByte(XMLElement.GetAttribute("node"));
                                            level = Convert.ToByte(XMLElement.GetAttribute("level"));                                            
                                            
                                            //Find device and create action 
                                            foreach (ZWaveDevice device in zVirtualScenesMain.MasterDevices)
                                            {
                                                if (device.NodeID == nodeID)
                                                {
                                                    Action action = (Action)device;

                                                    if (action.ZWaveType == ZWaveDevice.ZWaveDeviceTypes.BinarySwitch || action.ZWaveType == ZWaveDevice.ZWaveDeviceTypes.MultiLevelSwitch)
                                                    {
                                                        action.Level = level;
                                                        ActionResult result = action.Run(zVirtualScenesMain);
                                                        client.SendMessage("<setswitch result=\"" + result.Description + "\" alertlevel=\"" + result.ResultType + "\" />", zVirtualScenesMain);                                                        
                                                        WaitForClientData(client.ClientsSocket.RemoteEndPoint);
                                                        return; 
                                                    }
                                                }
                                            }

                                            client.SendMessage("<setdevice result=\"Device not found.\" alertlevel=\"Error\" />", zVirtualScenesMain);                                              
                                            WaitForClientData(client.ClientsSocket.RemoteEndPoint);
                                            return;

                                        case "setthermo":
                                            int HeatCoolMode = -1;
                                            int FanMode = -1;
                                            int EngeryMode = -1;
                                            int HeatPoint = -1;
                                            int CoolPoint = -1;

                                            nodeID = Convert.ToByte(XMLElement.GetAttribute("node"));
                                            HeatCoolMode = Convert.ToInt32(XMLElement.GetAttribute("HeatCoolMode"));
                                            FanMode = Convert.ToInt32(XMLElement.GetAttribute("FanMode"));
                                            EngeryMode = Convert.ToInt32(XMLElement.GetAttribute("EngeryMode"));
                                            HeatPoint = Convert.ToInt32(XMLElement.GetAttribute("HeatPoint"));
                                            CoolPoint = Convert.ToInt32(XMLElement.GetAttribute("CoolPoint"));

                                            //Find device and create action 
                                            foreach (ZWaveDevice device in zVirtualScenesMain.MasterDevices)
                                            {
                                                if (device.NodeID == nodeID)
                                                {
                                                    Action action = (Action)device;
                                                    if (action.ZWaveType == ZWaveDevice.ZWaveDeviceTypes.Thermostat)
                                                    {
                                                        action.HeatCoolMode = HeatCoolMode;
                                                        action.FanMode = FanMode;
                                                        action.EngeryMode = EngeryMode;
                                                        action.HeatPoint = HeatPoint;
                                                        action.CoolPoint = CoolPoint;

                                                        ActionResult result = action.Run(zVirtualScenesMain);
                                                        client.SendMessage("<setthermo result=\"" + result.Description + "\" alertlevel=\"" + result.ResultType + "\" />", zVirtualScenesMain);
                                                        WaitForClientData(client.ClientsSocket.RemoteEndPoint);
                                                        return;
                                                    }
                                                }
                                            }

                                            client.SendMessage("<setthermo result=\"Device not found.\" alertlevel=\"Error\" />", zVirtualScenesMain);
                                            WaitForClientData(client.ClientsSocket.RemoteEndPoint);
                                            return; 
                                    }
                                    throw new Exception("Invalid XML command.");
                                }

                            }
                            catch (Exception e)
                            {
                                throw new Exception(e.Message);
                            }
                        }
                    }
                }

                WaitForClientData(client.ClientsSocket.RemoteEndPoint);
            }            
            catch (ObjectDisposedException)
            {
                zVirtualScenesMain.AddLogEntry(UrgencyLevel.ERROR,  "Socket has been closed.", LOG_INTERFACE);
            }
            catch (SocketException sEX)
            {
                if (sEX.ErrorCode == 10054) // Error code for Connection reset by peer
                    zVirtualScenesMain.AddLogEntry(UrgencyLevel.ERROR,  "Connection reset by [" + client.ClientsSocket.RemoteEndPoint.ToString() + "].", LOG_INTERFACE);                
                else
                    zVirtualScenesMain.AddLogEntry(UrgencyLevel.ERROR,  "User [" + client.ClientsSocket.RemoteEndPoint.ToString() + "] " + sEX.Message, LOG_INTERFACE);

                HandleDisconnect(client);
            }
            catch (Exception ex)
            {
                zVirtualScenesMain.AddLogEntry(UrgencyLevel.WARNING,  "Client [" + client.ClientsSocket.RemoteEndPoint.ToString() + "] " + ex.Message, LOG_INTERFACE);
                client.SendMessage("<error msg=\"" + ex.Message + "\" />", zVirtualScenesMain);
                WaitForClientData(client.ClientsSocket.RemoteEndPoint);
            }
        }


    }
}
