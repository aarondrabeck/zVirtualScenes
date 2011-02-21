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
    class LightSwitchInterface
    {        
        private zVirtualScenes zVirtualScenesMain;
        private Socket LightSwitchSocket;
        private readonly List<Socket> LightSwitchClients = new List<Socket>();
        public AsyncCallback pfnWorkerCallBack;
        private int m_cookie = new Random().Next(65536);

        //Contructor
        public LightSwitchInterface(zVirtualScenes zvs)
        {
            zVirtualScenesMain = zvs; 
        }

        //Methods

        /// <summary>
        /// Starts listening for LightSwitch clients. 
        /// </summary>
        public void OpenLightSwitchSocket()
        {
            try
            {
                LightSwitchSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                LightSwitchSocket.Bind(new IPEndPoint(IPAddress.Any, zVirtualScenesMain.zVScenesSettings.LightSwitchPort));
                LightSwitchSocket.Listen(zVirtualScenesMain.zVScenesSettings.LightSwitchMaxConnections);
                LightSwitchSocket.BeginAccept(new AsyncCallback(OnLightSwitchClientConnect), null);
                zVirtualScenesMain.LogThis(1, "LightSwitchInterface: Started Listening for LightSwitch clients on port " + zVirtualScenesMain.zVScenesSettings.LightSwitchPort + ".");
            }
            catch (SocketException e)
            {
                zVirtualScenesMain.LogThis(2, "LightSwitchInterface: Socket Failed to Open - " + e);
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

                zVirtualScenesMain.LogThis(1, "LightSwitchInterface: Connection Attempt from: " + LightSwitchClientsSocket.RemoteEndPoint.ToString());

                // Send a welcome message to client
                string msg = "6004 ZWaveCommander Server (Connections " + LightSwitchClients.Count() + ")" + Environment.NewLine;
                SendMsgToLightSwitchClient(msg, LightSwitchClients.Count());

                // Let the worker Socket do the further processing for the just connected client
                WaitForData(LightSwitchClientsSocket, LightSwitchClients.Count(), false);

                // Since the main Socket is now free, it can go back and wait for other clients who are attempting to connect
                LightSwitchSocket.BeginAccept(new AsyncCallback(OnLightSwitchClientConnect), null);
            }
            catch (ObjectDisposedException e)
            {
                zVirtualScenesMain.LogThis(2, "LightSwitchInterface: Socket Connection Closed: " + e);
            }
            catch (SocketException e)
            {
                zVirtualScenesMain.LogThis(2, "LightSwitchInterface: Socket Exception: " + e);
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
                zVirtualScenesMain.LogThis(2, "LightSwitchInterface: Socket Exception: " + e);
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
                        zVirtualScenesMain.LogThis(1, "LightSwitchInterface: Received [" + LightSwitchClientSocket.RemoteEndPoint.ToString() + "] " + data);

                    string[] commands = data.Split('\n');

                    string version = "VER~" + zVirtualScenesMain.ProgramName;

                    foreach (string command in commands)
                    {
                        if (command.Length > 0)
                        {
                            if (command.Length <= 2)
                                continue;
                            
                            string cmd = command.TrimEnd(Environment.NewLine.ToCharArray()).ToUpper(); 
                           
                            //CLient is not verified
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
                                        zVirtualScenesMain.LogThis(1, "LightSwitchInterface: [" + LightSwitchClientSocket.RemoteEndPoint.ToString() + "] User Authenticated.");
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
                                    zVirtualScenesMain.LogThis(1, "LightSwitchInterface: [" + LightSwitchClientSocket.RemoteEndPoint.ToString() + "] User refreshed device list.");
                                    zVirtualScenesMain.ControlThinkGetDevices();

                                    foreach (Device device in zVirtualScenesMain.MasterDevices)
                                        SendMessagetoClientsSocket(LightSwitchClientSocket, device.ToLightSwitchSocketString() + Environment.NewLine);   

                                    foreach (Scene scene in zVirtualScenesMain.MasterScenes)
                                        SendMessagetoClientsSocket(LightSwitchClientSocket, scene.ToLightSwitchSocketString() + Environment.NewLine);

                                    SendMessagetoClientsSocket(LightSwitchClientSocket, "ENDLIST" + Environment.NewLine);
                                }
                                else if (cmd.StartsWith("LIST"))
                                {
                                    zVirtualScenesMain.LogThis(1, "LightSwitchInterface: [" + LightSwitchClientSocket.RemoteEndPoint.ToString() + "] User refreshed device list.");
                                    zVirtualScenesMain.ControlThinkGetDevices();

                                    foreach (Device device in zVirtualScenesMain.MasterDevices)
                                        SendMessagetoClientsSocket(LightSwitchClientSocket, device.ToLightSwitchSocketString() + Environment.NewLine);

                                    SendMessagetoClientsSocket(LightSwitchClientSocket, "ENDLIST" + Environment.NewLine);
                                }
                                else if (cmd.StartsWith("SLIST"))
                                {
                                    foreach (Scene scene in zVirtualScenesMain.MasterScenes)
                                        SendMessagetoClientsSocket(LightSwitchClientSocket, scene.ToLightSwitchSocketString() + Environment.NewLine);

                                    SendMessagetoClientsSocket(LightSwitchClientSocket, "ENDLIST" + Environment.NewLine);
                                }
                                else if (cmd.StartsWith("ZLIST"))
                                    SendMessagetoClientsSocket(LightSwitchClientSocket, "ENDLIST" + Environment.NewLine);

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
                                    //NOT USED
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
                zVirtualScenesMain.LogThis(2, "LightSwitchInterface: OnDataReceived - Socket has been closed");
            }
            catch (SocketException se)
            {
                if (se.ErrorCode == 10054) // Error code for Connection reset by peer
                    zVirtualScenesMain.LogThis(2, "LightSwitchInterface: Client " + socketData.m_clientNumber + " Disconnected.");                
                else
                    zVirtualScenesMain.LogThis(2, "LightSwitchInterface: SocketException - " + se.Message);
                
                DisconnectClientSocket(socketData);
            }
            catch (Exception e)
            {
                zVirtualScenesMain.LogThis(2, "LightSwitchInterface: [" + LightSwitchClientSocket.RemoteEndPoint.ToString() + "] Server Exception: " + e);
                
                //SEND ERROR TO CLIENT
                SendMessagetoClientsSocket(LightSwitchClientSocket, "ERR~" + e.Message + Environment.NewLine);

                Thread.Sleep(3000);
                DisconnectClientSocket(socketData);
            }
        }

        /// <summary>
        /// Send a Device Node and DEvice Level and translates to action.
        /// </summary>
        /// <param name="Node">Node of device to run</param>
        /// <param name="Level">Desired Level of deivce</param>
        /// <param name="Client">Socket of client.</param>
        /// <returns>Result of Device execution.</returns>
        private string TranslateToDeviceAction(byte Node, byte Level, Socket Client)
        {           
            foreach (Device device in zVirtualScenesMain.MasterDevices)
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

                        //Run
                        zVirtualScenesMain.RunSimpleAction(action);
                        zVirtualScenesMain.LogThis(1, "LightSwitchInterface: [" + Client.RemoteEndPoint.ToString() + "] Ran action on " + action.Name + ".");
                        return action.Name + " set to " + action.Level + "."; 
                    }
                }
            }
            return "Error Setting Device.";         
        }

        /// <summary>
        /// Send a scene ID and translates to action. 
        /// </summary>
        /// <param name="SceneID">Scene ID</param>
        /// <param name="Client">Clients Socket.</param>
        /// <returns>Result of Scene execution. </returns>
        private string TranslateToSceneActions(int SceneID, Socket Client)
        {
            foreach (Scene scene in zVirtualScenesMain.MasterScenes)
                if (scene.ID == SceneID)
                {
                    if (scene.Actions.Count > 0)
                    {
                        foreach (Action action in scene.Actions)
                        {
                            zVirtualScenesMain.RunSimpleAction(action);
                        }
                        zVirtualScenesMain.LogThis(1, "LightSwitchInterface: [" + Client.RemoteEndPoint.ToString() + "] Ran Scene " + scene.Name + " with " + scene.Actions.Count() + " actions.");
                        return "Scene " + scene.Name + " Executed.";
                    }
                    else
                        zVirtualScenesMain.LogThis(1, "Attepmted to run scene with no action.");                    
                }           
            return "Error executing device.";         
        }

        private string TranslateToThermoTEMPATUREAction(byte Node, byte Mode, int Temp, Socket Client)
        {
            string resultResponse = null;

            foreach (Device device in zVirtualScenesMain.MasterDevices)
            {
                if (device.NodeID == Node)
                {
                    //Cast device to action            
                    Action action = (Action)device;

                    switch (Mode)
                    {                        
                        case 2:
                            action.HeatPoint = Temp;
                            resultResponse = device.Name + " Heat point set to " + Temp;
                            break;
                        case 3:
                            action.CoolPoint = Temp;
                            resultResponse = device.Name + " Cool point set to " + Temp;
                            break;                        
                    }

                    if (resultResponse != null)
                    {
                        zVirtualScenesMain.RunSimpleAction(action);
                        zVirtualScenesMain.LogThis(1, "LightSwitchInterface: [" + Client.RemoteEndPoint.ToString() + "] Ran action on " + action.Name + ".");
                        return resultResponse;
                    }
                }
            }
            return "Error Setting Thermostat.";
        }

        private string TranslateToThermoAction(byte Node, byte Mode, Socket Client)
        {
            string resultResponse = null; 


            foreach (Device device in zVirtualScenesMain.MasterDevices)
            {
                if (device.NodeID == Node)
                {
                    //Cast device to action            
                    Action action = (Action)device;
                    
                    switch (Mode)
                    {
                        case 0:
                            action.HeatCoolMode = (int)Device.ThermostatMode.Off;
                            resultResponse = device.Name + " Mode set to Off.";
                            break; 
                        case 1:
                            action.HeatCoolMode = (int)Device.ThermostatMode.Auto;
                            resultResponse = device.Name + " Mode set to Auto.";
                            break;
                        case 2:
                            action.HeatCoolMode = (int)Device.ThermostatMode.Heat;
                            resultResponse = device.Name + " Mode set to Heat.";
                            break;
                        case 3:
                            action.HeatCoolMode = (int)Device.ThermostatMode.Cool;
                            resultResponse = device.Name + " Thermostat Mode set to Cool.";
                            break;
                        case 4:
                            action.FanMode = (int)Device.ThermostatFanMode.OnLow;
                            resultResponse = device.Name + " Fan set to On/OnLow.";
                            break;
                        case 5:
                            action.FanMode = (int)Device.ThermostatFanMode.AutoLow;
                            resultResponse = device.Name + " Fan set to Auto/AutoLow.";
                            break;
                        case 6:
                            action.EngeryMode = (int)Device.EnergyMode.EnergySavingMode;
                            resultResponse = device.Name + " Energy Saving Mode Enabled.";
                            break;
                        case 7:
                            action.EngeryMode = (int)Device.EnergyMode.ComfortMode;
                            resultResponse = device.Name + " Comfort Mode Enabled.";
                            break;
                    }

                    if (resultResponse != null)
                    {
                        zVirtualScenesMain.RunSimpleAction(action);
                        zVirtualScenesMain.LogThis(1, "LightSwitchInterface: [" + Client.RemoteEndPoint.ToString() + "] Ran action on " + action.Name + ".");
                        return resultResponse;
                    }                        
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
                    zVirtualScenesMain.LogThis(1, "LightSwitchInterface: SENT TO ALL - " + msg);
            }
        }

        /// <summary>
        /// Sends a message to ONE client
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
                    zVirtualScenesMain.LogThis(1, "LightSwitchInterface: SENT - " + msg);
            }
        }

        /// <summary>
        /// Sends a message to ONE client
        /// </summary>
        private void SendMsgToLightSwitchClient(string msg, int clientNumber)
        {
            
            // Convert the reply to byte array
            byte[] byData = System.Text.Encoding.UTF8.GetBytes(msg);
            Socket LightSwitchClientsSocket = (Socket)LightSwitchClients[clientNumber - 1];
            LightSwitchClientsSocket.Send(byData);

            if (zVirtualScenesMain.zVScenesSettings.LightSwitchVerbose)
                zVirtualScenesMain.LogThis(1, "LightSwitchInterface: SENT " + msg);
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
                zVirtualScenesMain.LogThis(1, "LightSwitchInterface: Socket Disconnect: " + e);
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
    }

    public class SocketPacket
    {
        // holds a reference to the socket
        public Socket m_currentSocket;

        // holds the client number for identification
        public int m_clientNumber;

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
        }
    }
}

