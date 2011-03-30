using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace zVirtualScenesApplication
{
    public class LightSwitchInterface
    {
        private static string LOG_INTERFACE = "LIGHTSWITCH";
        public formzVirtualScenes zVirtualScenesMain;
        private Socket LightSwitchSocket;
        private readonly List<Socket> LightSwitchClients = new List<Socket>();
        public AsyncCallback pfnWorkerCallBack;
        private int m_cookie = new Random().Next(65536);
        public volatile bool isActive = false; 

        /// <summary>
        /// Starts listening for LightSwitch clients. 
        /// </summary>
        public void OpenLightSwitchSocket()
        {
            if (LightSwitchSocket == null || !isActive)
            {
                try
                {
                    isActive = true;
                    LightSwitchSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    LightSwitchSocket.Bind(new IPEndPoint(IPAddress.Any, zVirtualScenesMain.zVScenesSettings.LightSwitchPort));
                    LightSwitchSocket.Listen(zVirtualScenesMain.zVScenesSettings.LightSwitchMaxConnections);
                    LightSwitchSocket.BeginAccept(new AsyncCallback(OnLightSwitchClientConnect), null);
                    zVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "Started listening for LightSwitch clients on port " + zVirtualScenesMain.zVScenesSettings.LightSwitchPort + ".", LOG_INTERFACE);
                }
                catch (SocketException e)
                {
                    zVirtualScenesMain.AddLogEntry(UrgencyLevel.ERROR, "Socket Failed to Open - " + e, LOG_INTERFACE);
                }
            }
        }

        public void CloseLightSwitchSocket()
        {
            if (LightSwitchSocket != null && isActive)
            {
                LightSwitchSocket.Close();

                foreach (Socket client in LightSwitchClients)
                {
                    if (client.Connected)
                    {
                        client.Close();
                    }
                }

                zVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "Stopped listening for new clients.", LOG_INTERFACE);
                isActive = false;
            }
        }

        /// <summary>
        /// Welcomes client and opens individualized socket for them to clear main socket.
        /// </summary>
        /// <param name="asyn"></param>
        private void OnLightSwitchClientConnect(IAsyncResult asyn)
        {
            try
            {
                //accept and create new socket
                Socket LightSwitchClientsSocket = LightSwitchSocket.EndAccept(asyn);

                lock (LightSwitchClients)
                    LightSwitchClients.Add(LightSwitchClientsSocket);

                zVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "Connection Attempt from: " + LightSwitchClientsSocket.RemoteEndPoint.ToString(), LOG_INTERFACE);
             
                // Send a welcome message to client                
                string msg = "Hybrid LightSwitch / iViewer Server (Active Connections " + LightSwitchClients.Count() + ")" + Environment.NewLine;
                SendMsgToLightSwitchClient(msg, LightSwitchClients.Count());

                // Let the worker Socket do the further processing for the just connected client
                WaitForData(LightSwitchClientsSocket, LightSwitchClients.Count(), false);

                // Since the main Socket is now free, it can go back and wait for other clients who are attempting to connect
                LightSwitchSocket.BeginAccept(new AsyncCallback(OnLightSwitchClientConnect), null);
            }
            catch (ObjectDisposedException e)
            {
                //zVirtualScenesMain.AddLogEntry(UrgencyLevel.ERROR, "Socket Connection Closed: " + e, LOG_INTERFACE);
            }
            catch (SocketException e)
            {
                zVirtualScenesMain.AddLogEntry(UrgencyLevel.ERROR, "Socket Exception: " + e, LOG_INTERFACE);
            }
        }

        /// <summary>
        /// Waits for client data and handles it when recieved
        /// </summary>
        /// <param name="soc"></param>
        /// <param name="clientNumber"></param>
        /// <param name="verified"></param>
        public void WaitForData(System.Net.Sockets.Socket soc, int clientNumber, bool verified)
        {
            try
            {
                if (pfnWorkerCallBack == null)
                {
                    // Specify the call back function which is to be invoked when there is any write activity by the connected client
                    pfnWorkerCallBack = new AsyncCallback(OnDataReceived);
                }
                SocketPacket theSocPkt = new SocketPacket(soc, clientNumber, verified);
                soc.BeginReceive(theSocPkt.dataBuffer, 0, theSocPkt.dataBuffer.Length, SocketFlags.None, pfnWorkerCallBack, theSocPkt);
            }
            catch (SocketException e)
            {
                zVirtualScenesMain.AddLogEntry(UrgencyLevel.ERROR, "Socket Exception: " + e, LOG_INTERFACE);
            }
        }

        /// <summary>
        /// This the call back function which will be invoked when the socket detects any client writing of data on the stream
        /// </summary>
        /// <param name="asyn">Object containing Socket Packet</param>
        public void OnDataReceived(IAsyncResult asyn)
        {
            SocketPacket socketData = (SocketPacket)asyn.AsyncState;
            Socket LightSwitchClientSocket = (Socket)socketData.m_currentSocket;

            if (!LightSwitchClientSocket.Connected)
                return;

            try
            {
                // Complete the BeginReceive() asynchronous call by EndReceive() method which will return the number of characters written to the stream  by the client
                int iRx = LightSwitchClientSocket.EndReceive(asyn);

                //this socket was closed
                if (iRx == 0)
                {
                    DisconnectClientSocket(socketData);
                    return;
                }

                if (iRx > 2)
                {
                    char[] chars = new char[iRx + 1];
                    // Extract the characters as a buffer
                    System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                    int charLen = d.GetChars(socketData.dataBuffer, 0, iRx, chars, 0);
                    string data = new string(chars);

                    if(zVirtualScenesMain.zVScenesSettings.LightSwitchVerbose)
                        zVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "Received [" + LightSwitchClientSocket.RemoteEndPoint.ToString() + "] " + data, LOG_INTERFACE);

                    string[] commands = data.Split('\n');

                    string version = "VER~" + zVirtualScenesMain.ProgramName;

                    foreach (string command in commands)
                    {
                        if (command.Length > 0)
                        {
                            if (command.Length <= 2)
                                continue;
                            
                            string cmd = command.TrimEnd(Environment.NewLine.ToCharArray()).ToUpper();                                                        
                           
                            if (cmd.StartsWith("IPHONE"))  //Send salt to phone
                            {
                                socketData.m_verified = false;
                                SendMessagetoClientsSocket(LightSwitchClientSocket, "COOKIE~" + Convert.ToString(m_cookie) + Environment.NewLine);
                            }

                            if (!socketData.m_verified)
                            {
                                //If not verified attept to verify                            
                                if (cmd.StartsWith("PASSWORD"))
                                {
                                    string[] values = cmd.Split('~');
                                    string inputPassword = values[1];
                                    string hashedPassword = EncodePassword(Convert.ToString(m_cookie) + ":" + zVirtualScenesMain.zVScenesSettings.LightSwitchPassword);

                                    if (inputPassword.StartsWith(hashedPassword))
                                    {
                                        socketData.m_verified = true;
                                        SendMessagetoClientsSocket(LightSwitchClientSocket, version + Environment.NewLine);
                                        zVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "[" + LightSwitchClientSocket.RemoteEndPoint.ToString() + "] User Authenticated.", LOG_INTERFACE);
                                    }
                                    else
                                    {                                        
                                        socketData.m_verified = false;
                                        throw new Exception("Passwords do not match");                                        
                                    }
                                }
                            }
                            else //CLIENT IS VERIFIED
                            {
                                if (cmd.StartsWith("VERSION"))                                    
                                    SendMessagetoClientsSocket(LightSwitchClientSocket, version + Environment.NewLine);

                                else if (cmd.StartsWith("SERVER"))
                                    SendMessagetoClientsSocket(LightSwitchClientSocket, version + Environment.NewLine);

                                else if (cmd.StartsWith("TERMINATE"))
                                    throw new Exception("Terminating client socket...");
                                
                                else if (cmd.StartsWith("ALIST"))
                                {
                                    zVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "[" + LightSwitchClientSocket.RemoteEndPoint.ToString() + "] User refreshed device list.", LOG_INTERFACE);

                                    sendDeviceList(LightSwitchClientSocket);                                    

                                    foreach (Scene scene in zVirtualScenesMain.MasterScenes)
                                        if (scene.ShowInLightSwitchGUI)
                                            SendMessagetoClientsSocket(LightSwitchClientSocket, scene.ToLightSwitchSocketString() + Environment.NewLine);

                                    int ID = 0;
                                    foreach (string group in zVirtualScenesMain.groups)
                                    {
                                        ID++;
                                        SendMessagetoClientsSocket(LightSwitchClientSocket, "ZONE~" + group + "~" + ID + Environment.NewLine);
                                    }

                                    SendMessagetoClientsSocket(LightSwitchClientSocket, "ENDLIST" + Environment.NewLine);
                                }
                                else if (cmd.StartsWith("LIST"))
                                {
                                    zVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "[" + LightSwitchClientSocket.RemoteEndPoint.ToString() + "] User refreshed device list.", LOG_INTERFACE);

                                    sendDeviceList(LightSwitchClientSocket);

                                    SendMessagetoClientsSocket(LightSwitchClientSocket, "ENDLIST" + Environment.NewLine);
                                }
                                else if (cmd.StartsWith("SLIST"))
                                {
                                    foreach (Scene scene in zVirtualScenesMain.MasterScenes)
                                        if(scene.ShowInLightSwitchGUI)
                                            SendMessagetoClientsSocket(LightSwitchClientSocket, scene.ToLightSwitchSocketString() + Environment.NewLine);

                                    SendMessagetoClientsSocket(LightSwitchClientSocket, "ENDLIST" + Environment.NewLine);
                                }
                                else if (cmd.StartsWith("ZLIST"))
                                {
                                    int ID = 0; 
                                    foreach (string group in zVirtualScenesMain.groups)
                                    {
                                        ID++;
                                        SendMessagetoClientsSocket(LightSwitchClientSocket, "ZONE~" + group + "~" + ID + Environment.NewLine);
                                    }
                                    SendMessagetoClientsSocket(LightSwitchClientSocket, "ENDLIST" + Environment.NewLine);
                                }
                                else if (cmd.StartsWith("DEVICE"))
                                {
                                    string[] values = cmd.Split('~');
                                    //NOTIFY ALL CLIENTS
                                    BroadcastMessage("MSG~" + TranslateToDeviceAction(Convert.ToByte(values[1]), Convert.ToByte(values[2]), LightSwitchClientSocket) + Environment.NewLine);
                                }                                
                                else if (cmd.StartsWith("SCENE"))
                                {
                                    string[] values = cmd.Split('~');
                                    //NOTIFY ALL CLIENTS
                                    BroadcastMessage("MSG~" + TranslateToSceneActions(Convert.ToByte(values[1]), LightSwitchClientSocket) + Environment.NewLine); 
                                }
                                else if (cmd.StartsWith("ZONE"))
                                {
                                    string[] values = cmd.Split('~');

                                    int ID = 0;
                                    foreach (string group in zVirtualScenesMain.groups)
                                    {
                                        ID++;
                                        if(ID == Convert.ToByte(values[1]))
                                        {                                            
                                            byte level = Convert.ToByte(values[2]);

                                            if (level == 255)
                                                level = 99;

                                            SceneResult result = zVirtualScenesMain.ActivateGroup(group, level);
                                            zVirtualScenesMain.AddLogEntry((UrgencyLevel)result.ResultType, "[" + LightSwitchClientSocket.RemoteEndPoint.ToString() + "] " + result.Description, LOG_INTERFACE);
                                            BroadcastMessage("MSG~" + result.Description + Environment.NewLine);
                                        }
                                    }
                                    
                                }
                                else if (cmd.StartsWith("THERMMODE"))
                                {
                                    string[] values = cmd.Split('~');
                                    //NOTIFY ALL CLIENTS
                                    BroadcastMessage("MSG~" + TranslateToThermoAction(Convert.ToByte(values[1]), Convert.ToByte(values[2]), LightSwitchClientSocket) + Environment.NewLine);
                                }
                                else if (cmd.StartsWith("THERMTEMP"))
                                {
                                    string[] values = cmd.Split('~');
                                    //NOTIFY ALL CLIENTS
                                    BroadcastMessage("MSG~" + TranslateToThermoTEMPATUREAction(Convert.ToByte(values[1]), Convert.ToByte(values[2]), Convert.ToInt32(values[3]), LightSwitchClientSocket) + Environment.NewLine);
                                }
                                else
                                {
                                    throw new Exception("Terminating Due To Unknown Socket Command: " + data);
                                }                                	
                            }
                        }
                    }
                }

                // Continue the waiting for data on the Socket
                WaitForData(socketData.m_currentSocket, socketData.m_clientNumber, socketData.m_verified);

            }
            catch (ObjectDisposedException)
            {
                zVirtualScenesMain.AddLogEntry(UrgencyLevel.ERROR, "OnDataReceived - Socket has been closed", LOG_INTERFACE);
            }
            catch (SocketException se)
            {
                if (se.ErrorCode == 10054) // Error code for Connection reset by peer
                    zVirtualScenesMain.AddLogEntry(UrgencyLevel.ERROR, "Client " + socketData.m_clientNumber + " Disconnected.", LOG_INTERFACE);                
                else
                    zVirtualScenesMain.AddLogEntry(UrgencyLevel.ERROR, "SocketException - " + se.Message, LOG_INTERFACE);
                
                DisconnectClientSocket(socketData);
            }
            catch (Exception e)
            {
                zVirtualScenesMain.AddLogEntry(UrgencyLevel.ERROR, "[" + LightSwitchClientSocket.RemoteEndPoint.ToString() + "] Server Exception: " + e, LOG_INTERFACE);
                
                //SEND ERROR TO CLIENT
                SendMessagetoClientsSocket(LightSwitchClientSocket, "ERR~" + e.Message + Environment.NewLine);

                Thread.Sleep(3000);
                DisconnectClientSocket(socketData);
            }
        }

        private void sendDeviceList(Socket LightSwitchClientSocket)
        {
            List<String> devices = new List<string>();

            foreach (ZWaveDevice device in zVirtualScenesMain.MasterDevices)
                if (device.ShowInLightSwitchGUI)
                    devices.Add(TranslateDeviceToLightSwitchString(device));

            if(zVirtualScenesMain.zVScenesSettings.LightSwitchSortDeviceList)
                devices.Sort(); 

            foreach (string device in devices)
                SendMessagetoClientsSocket(LightSwitchClientSocket, "DEVICE~" + device + Environment.NewLine);
        }

        /// <summary>
        /// Send a Device Node and Device Level and translates to action.
        /// </summary>
        /// <param name="Node">Node of device to run</param>
        /// <param name="Level">Desired Level of deivce</param>
        /// <param name="Client">Socket of client.</param>
        /// <returns>Result of Device execution.</returns>
        private string TranslateToDeviceAction(byte Node, byte Level, Socket Client)
        {           
            foreach (ZWaveDevice device in zVirtualScenesMain.MasterDevices)
            {
                if (device.NodeID == Node)
                {
                    Action action = (Action)device;

                    if (action != null)
                    {
                        //set Level
                        if (Level == 255)
                            Level = 99;
                        action.Level = Level;

                        //Run and log
                        ActionResult result = action.Run(zVirtualScenesMain);
                        zVirtualScenesMain.AddLogEntry((UrgencyLevel)result.ResultType, "[" + Client.RemoteEndPoint.ToString() + "] " + result.Description, LOG_INTERFACE);

                        //return result to app
                        return result.ResultType + " " + result.Description; 
                    }
                }
            }
            return "Error Setting Device.";         
        }

        /// <summary>
        /// Runs a zVirtualScene Scene
        /// </summary>
        /// <param name="SceneID">Scene ID</param>
        /// <param name="Client">Clients Socket.</param>
        /// <returns>Result of Scene execution. </returns>
        private string TranslateToSceneActions(int SceneID, Socket Client)
        {
            foreach (Scene scene in zVirtualScenesMain.MasterScenes)
                if (scene.ID == SceneID)
                {
                    SceneResult result = scene.Run(zVirtualScenesMain);
                    zVirtualScenesMain.AddLogEntry((UrgencyLevel)result.ResultType, "[" + Client.RemoteEndPoint.ToString() + "] " + result.Description, LOG_INTERFACE);

                    return result.ResultType.ToString() + " " + result.Description;
                 }           
            return "Error executing scene.";         
        }

        private string TranslateToThermoTEMPATUREAction(byte Node, byte Mode, int Temp, Socket Client)
        {
            foreach (ZWaveDevice device in zVirtualScenesMain.MasterDevices)
            {
                if (device.NodeID == Node)
                {
                    //Cast device to action            
                    Action action = (Action)device;

                    switch (Mode)
                    {                        
                        case 2:
                            action.HeatPoint = Temp;
                            break;
                        case 3:
                            action.CoolPoint = Temp;
                            break;                        
                    }
                    ActionResult result = action.Run(zVirtualScenesMain);
                    zVirtualScenesMain.AddLogEntry((UrgencyLevel)result.ResultType, "[" + Client.RemoteEndPoint.ToString() + "] " + result.Description, LOG_INTERFACE);                         
                    return result.ResultType + " " + result.Description; ;                    
                }
            }
            return "Error Setting Thermostat.";
        }

        private string TranslateToThermoAction(byte Node, byte Mode, Socket Client)
        {
            foreach (ZWaveDevice device in zVirtualScenesMain.MasterDevices)
            {
                if (device.NodeID == Node)
                {
                    //Cast device to action            
                    Action action = (Action)device;
                    
                    switch (Mode)
                    {
                        case 0:
                            action.HeatCoolMode = (int)ZWaveDevice.ThermostatMode.Off;
                            break; 
                        case 1:
                            action.HeatCoolMode = (int)ZWaveDevice.ThermostatMode.Auto;
                            break;
                        case 2:
                            action.HeatCoolMode = (int)ZWaveDevice.ThermostatMode.Heat;
                            break;
                        case 3:
                            action.HeatCoolMode = (int)ZWaveDevice.ThermostatMode.Cool;
                            break;
                        case 4:
                            action.FanMode = (int)ZWaveDevice.ThermostatFanMode.OnLow;
                            break;
                        case 5:
                            action.FanMode = (int)ZWaveDevice.ThermostatFanMode.AutoLow;
                            break;
                        case 6:
                            action.EngeryMode = (int)ZWaveDevice.EnergyMode.EnergySavingMode;
                            break;
                        case 7:
                            action.EngeryMode = (int)ZWaveDevice.EnergyMode.ComfortMode;
                            break;
                    }
                    ActionResult result = action.Run(zVirtualScenesMain);
                    zVirtualScenesMain.AddLogEntry((UrgencyLevel)result.ResultType, "[" + Client.RemoteEndPoint.ToString() + "] " + result.Description, LOG_INTERFACE);                        
                    return result.ResultType + " " + result.Description;                                           
                }
            }
            return "Error Setting Thermostat.";
        }

        /// <summary>
        /// Sends a message to ALL connected clients.
        /// </summary>
        /// <param name="msg">the message to send</param>
        public void BroadcastMessage(string msg)
        {
            if (msg.Length > 0)
            {
                // Convert the reply to byte array
                byte[] byData = System.Text.Encoding.UTF8.GetBytes(msg);
                
                foreach (Socket workerSocket in LightSwitchClients)
                    if (workerSocket != null)
                        workerSocket.Send(byData);
                
                if (zVirtualScenesMain.zVScenesSettings.LightSwitchVerbose)
                    zVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "SENT TO ALL - " + msg, LOG_INTERFACE);
            }
        }

        /// <summary>
        /// Sends a message to ONE client by socket
        /// </summary>
        /// <param name="msg">the message to send</param>
        public void SendMessagetoClientsSocket(Socket LightSwitchClientSocket, string msg)
        {
            if (msg.Length > 0)
            {
                // Convert the reply to byte array
                byte[] byData = System.Text.Encoding.UTF8.GetBytes(msg);
                LightSwitchClientSocket.Send(byData);

                if (zVirtualScenesMain.zVScenesSettings.LightSwitchVerbose)
                    zVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "SENT - " + msg, LOG_INTERFACE);
            }
        }

        /// <summary>
        /// Sends a message to ONE client by client number
        /// </summary>
        private void SendMsgToLightSwitchClient(string msg, int clientNumber)
        {
            
            // Convert the reply to byte array
            byte[] byData = System.Text.Encoding.UTF8.GetBytes(msg);
            Socket LightSwitchClientsSocket = (Socket)LightSwitchClients[clientNumber - 1];
            LightSwitchClientsSocket.Send(byData);

            if (zVirtualScenesMain.zVScenesSettings.LightSwitchVerbose)
                zVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "SENT " + msg, LOG_INTERFACE);
        }

        private void DisconnectClientSocket(SocketPacket socketData)
        {
            Socket LightSwitchClientsSocket = (Socket)socketData.m_currentSocket;
            try
            {
                // Remove the reference to the worker socket of the closed client so that this object will get garbage collected
                lock (LightSwitchClients)
                    LightSwitchClients.Remove(LightSwitchClientsSocket);

                LightSwitchClientsSocket.Close();
                LightSwitchClientsSocket = null;
            }
            catch (Exception e)
            {
                zVirtualScenesMain.AddLogEntry(UrgencyLevel.INFO, "Socket Disconnect: " + e, LOG_INTERFACE);
            }
        }

        public string EncodePassword(string originalPassword)
        {
            // Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
            Byte[] originalBytes = ASCIIEncoding.Default.GetBytes(originalPassword);
            Byte[] encodedBytes = new MD5CryptoServiceProvider().ComputeHash(originalBytes);

            StringBuilder result = new StringBuilder();
            for (int i = 0; i < encodedBytes.Length; i++)
                result.Append(encodedBytes[i].ToString("x2"));

            return result.ToString().ToUpper();
        }

        //Light Switch Socket Format 
        //byData = System.Text.Encoding.UTF8.GetBytes("DEVICE~Bedroom Lights~0~60~MultiLevelSceneSwitch" + Environment.NewLine);
        //workerSocket.Send(byData);
        //byData = System.Text.Encoding.UTF8.GetBytes("DEVICE~Garage Light~1~0~BinarySwitch" + Environment.NewLine);
        //workerSocket.Send(byData);
        //byData = System.Text.Encoding.UTF8.GetBytes("DEVICE~Thermostat~3~75~Thermostat" + Environment.NewLine);
        //workerSocket.Send(byData);
        //byData = System.Text.Encoding.UTF8.GetBytes("DEVICE~Electric Blinds~4~100~WindowCovering" + Environment.NewLine);
        //workerSocket.Send(byData);
        //byData = System.Text.Encoding.UTF8.GetBytes("DEVICE~Motion Detector~5~0~Sensor" + Environment.NewLine);
        //workerSocket.Send(byData);
        //byData = System.Text.Encoding.UTF8.GetBytes("DEVICE~House (AWAY MODE)~6~0~Status" + Environment.NewLine);
        //workerSocket.Send(byData);
        public string TranslateDeviceToLightSwitchString(ZWaveDevice device)
        {
            if (device.Type == zVirtualScenesApplication.ZWaveDevice.ZWaveDeviceTypes.BinarySwitch)
                return device.Name + "~" + device.NodeID + "~" + device.Level + "~" + "BinarySwitch";
            else if (device.Type == zVirtualScenesApplication.ZWaveDevice.ZWaveDeviceTypes.Thermostat)
                return device.Name + "~" + device.NodeID + "~" + device.Temp + "~" + device.Type;
            else
                return device.Name + "~" + device.NodeID + "~" + device.Level + "~" + device.Type;
        }
    }

    public class SocketPacket
    {   
        // holds a reference to the socket
        public Socket m_currentSocket;

        // holds the client number for identification
        public int m_clientNumber;

        //Is a IViewer Client?
        public bool iViewerClient; 

        // Buffer to store the data sent by the client
        public byte[] dataBuffer = new byte[1024];

        // flag whether this has passed authentication
        public bool m_verified = false;

        // Constructor which takes a Socket and a client number
        public SocketPacket(Socket socket, int clientNumber, bool verified)
        {
            m_currentSocket = socket;
            m_clientNumber = clientNumber;
            m_verified = verified;
            iViewerClient = false; 
        }
    }
}

