using System.ComponentModel.Composition;
using System.Net.Sockets;
using System.Collections.Generic;
using System;
using System.Net;
using System.Threading;
using System.Text;
using System.Security.Cryptography;
using zVirtualScenesCommon.Entity;
using zVirtualScenesCommon;
using System.Data;
using System.ComponentModel;
using System.Linq;
using ZeroconfService;
using zVirtualScenesAPI;


namespace LightSwitchPlugin
{
    [Export(typeof(Plugin))]
    public class LightSwitchPlugin : Plugin
    {
        private Socket LightSwitchSocket;
        private readonly List<Socket> LightSwitchClients = new List<Socket>();
        public AsyncCallback pfnWorkerCallBack;
        private int m_cookie = new Random().Next(65536);
        public volatile bool isActive = false;
        private NetService netservice = null;

        public LightSwitchPlugin()
            : base("LIGHTSWITCH",
               "LightSwitch Plugin",
                "This plug-in is a server that allows LightSwitch clients to connect and control zVirtualScene devices."
                ) { }

        public override void Initialize()
        {
            DefineOrUpdateSetting(new plugin_settings
            {
                name = "PORT",
                friendly_name = "Port",
                value = (1337).ToString(),
                value_data_type = (int)Data_Types.INTEGER,
                description = "LightSwitch will listen for connections on this port."
            });

            DefineOrUpdateSetting(new plugin_settings
            {
                name = "MAXCONN",
                friendly_name = "Max Conn.",
                value = (200).ToString(),
                value_data_type = (int)Data_Types.INTEGER,
                description = "The maximum number of connections allowed."
            });

            DefineOrUpdateSetting(new plugin_settings
            {
                name = "VERBOSE",
                friendly_name = "Verbose Logging",
                value = false.ToString(),
                value_data_type = (int)Data_Types.BOOL,
                description = "(Writes all server client communication to the log for debugging.)"
            });

            DefineOrUpdateSetting(new plugin_settings
            {
                name = "PASSWORD",
                friendly_name = "Password",
                value = "ChaNgeMe444",
                value_data_type = (int)Data_Types.STRING,
                description = "The password clients must use to connect to the LightSwitch server. "
            });

            DefineOrUpdateSetting(new plugin_settings
            {
                name = "SORTLIST",
                friendly_name = "Sort Device List",
                value = true.ToString(),
                value_data_type = (int)Data_Types.BOOL,
                description = "(Alphabetically sorts the device list.)"
            });

            DefineOrUpdateSetting(new plugin_settings
            {
                name = "PUBLISHZEROCFG",
                friendly_name = "Publish ZeroConf/Bonjour",
                value = false.ToString(),
                value_data_type = (int)Data_Types.BOOL,
                description = "Zero configuration networking allows clients on your network to detect and connect to your LightSwitch server automatically."
            });

            device_propertys.DefineOrUpdateDeviceProperty(new device_propertys
            {
                name = "SHOWINLSLIST",
                friendly_name = "If enabled this device will show in the LightSwitch device tab.",
                value_data_type = (int)Data_Types.BOOL,
                default_value = "true"
            });

            scene_property.DefineOrUpdateProperty(new scene_property
            {
                name = "SHOWSCENEINLSLIST",
                friendly_name = "Show in Lightswitch List",
                description = "If enabled this scene will show in the LightSwitch scene tab.",
                defualt_value = "true",
                value_data_type = (int)Data_Types.BOOL
            });

        }

        protected override bool StartPlugin()
        {
            device_values.DeviceValueDataChangedEvent += new device_values.ValueDataChangedEventHandler(device_values_DeviceValueDataChangedEvent);
            builtin_command_que.BuiltinCommandRunCompleteEvent +=new builtin_command_que.BuiltinCommandRunCompleteEventHandler(builtin_command_que_BuiltinCommandRunCompleteEvent);
            device_type_command_que.DeviceTypeCommandRunCompleteEvent +=new device_type_command_que.DeviceTypeCommandRunCompleteEventHandler(device_type_command_que_DeviceTypeCommandRunCompleteEvent);
            device_command_que.DeviceCommandRunCompleteEvent+=new device_command_que.DeviceCommandRunCompleteEventHandler(device_command_que_DeviceCommandRunCompleteEvent);
            zvsEntityControl.SceneRunCompleteEvent += new zvsEntityControl.SceneRunCompleteEventHandler(zvsEntityControl_SceneRunCompleteEvent);
            
            WriteToLog(Urgency.INFO, this.Friendly_Name + " plugin started.");

            OpenLightSwitchSocket();

            bool useBonjour = false;
            bool.TryParse(GetSettingValue("PUBLISHZEROCFG"), out useBonjour);

            if (useBonjour)
            {
                try
                {
                    if (netservice == null)
                        PublishZeroconf();
                    else
                    {
                        netservice.Dispose();
                        PublishZeroconf();
                    }

                }
                catch (Exception ex)
                {
                    WriteToLog(Urgency.ERROR, ex.Message);
                }
            }

            IsReady = true;
            return true;
        }

        protected override bool StopPlugin()
        {
            device_values.DeviceValueDataChangedEvent -= new device_values.ValueDataChangedEventHandler(device_values_DeviceValueDataChangedEvent);
            builtin_command_que.BuiltinCommandRunCompleteEvent -= new builtin_command_que.BuiltinCommandRunCompleteEventHandler(builtin_command_que_BuiltinCommandRunCompleteEvent);
            device_type_command_que.DeviceTypeCommandRunCompleteEvent -= new device_type_command_que.DeviceTypeCommandRunCompleteEventHandler(device_type_command_que_DeviceTypeCommandRunCompleteEvent);
            device_command_que.DeviceCommandRunCompleteEvent -= new device_command_que.DeviceCommandRunCompleteEventHandler(device_command_que_DeviceCommandRunCompleteEvent);
            zvsEntityControl.SceneRunCompleteEvent -= new zvsEntityControl.SceneRunCompleteEventHandler(zvsEntityControl_SceneRunCompleteEvent);
                        
            WriteToLog(Urgency.INFO, this.Friendly_Name + " plugin ended.");
            CloseLightSwitchSocket();
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

        void device_values_DeviceValueDataChangedEvent(object sender, string PreviousValue)
        {
            device_values dv = (device_values)sender;
            string UpdateString = DeviceToString(dv);

            if (!string.IsNullOrEmpty(UpdateString))
            {
                BroadcastMessage("UPDATE~" + DeviceToString(dv) + Environment.NewLine);
                BroadcastMessage("ENDLIST" + Environment.NewLine);
            }

            string device_name = string.Empty;
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                device device = db.devices.FirstOrDefault(d => d.id == dv.device_id);
                if (device != null)
                    device_name = device.friendly_name;
            }

            BroadcastMessage("MSG~" + "'" + device_name + "' " + dv.label_name + " changed to " + dv.value + Environment.NewLine);
        }       

        void zvsEntityControl_SceneRunCompleteEvent(long scene_id, int ErrorCount)
        {
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                scene scene = db.scenes.FirstOrDefault(s => s.id == scene_id);
                if (scene != null)
                {
                    BroadcastMessage("MSG~" + "Scene '" + scene.friendly_name + "' has completed with " + ErrorCount + " errors." + Environment.NewLine);
                }
            }
        }

        void device_command_que_DeviceCommandRunCompleteEvent(device_command_que cmd, bool withErrors, string txtError)
        {
            if (withErrors)
                BroadcastMessage("ERR~" + txtError + Environment.NewLine);
        }

        void device_type_command_que_DeviceTypeCommandRunCompleteEvent(device_type_command_que cmd, bool withErrors, string txtError)
        {
            if (withErrors)
                BroadcastMessage("ERR~" + txtError + Environment.NewLine);
        }

        void builtin_command_que_BuiltinCommandRunCompleteEvent(builtin_command_que cmd, bool withErrors, string txtError)
        {
            if (withErrors)
                BroadcastMessage("ERR~" + txtError + Environment.NewLine);
        }

        /// <summary>
        /// Starts listening for LightSwitch clients. 
        /// </summary>
        public void OpenLightSwitchSocket()
        {
            if (LightSwitchSocket == null || !isActive)
            {
                try
                {
                    int port = 9909;
                    int.TryParse(GetSettingValue("PORT"), out port);

                    int max_conn = 50;
                    int.TryParse(GetSettingValue("MAXCONN"), out max_conn);

                    isActive = true;
                    LightSwitchSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    LightSwitchSocket.Bind(new IPEndPoint(IPAddress.Any, port));
                    LightSwitchSocket.Listen(max_conn);
                    LightSwitchSocket.BeginAccept(new AsyncCallback(OnLightSwitchClientConnect), null);
                    WriteToLog(Urgency.INFO, "Started listening for LightSwitch clients on port " + port + ".");
                }
                catch (SocketException e)
                {
                    WriteToLog(Urgency.ERROR, "Socket Failed to Open - " + e);
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
                WriteToLog(Urgency.INFO, "Stopped listening for new clients.");
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

                WriteToLog(Urgency.INFO, "Connection Attempt from: " + LightSwitchClientsSocket.RemoteEndPoint.ToString());

                // Send a welcome message to client                
                string msg = "LightSwitch zVirtualScenes Plugin (Active Connections " + LightSwitchClients.Count + ")" + Environment.NewLine;
                SendMsgToLightSwitchClient(msg, LightSwitchClients.Count);

                // Let the worker Socket do the further processing for the just connected client
                WaitForData(LightSwitchClientsSocket, LightSwitchClients.Count, false);

                // Since the main Socket is now free, it can go back and wait for other clients who are attempting to connect
                LightSwitchSocket.BeginAccept(new AsyncCallback(OnLightSwitchClientConnect), null);
            }
            catch (ObjectDisposedException)
            {
            }
            catch (SocketException e)
            {
                WriteToLog(Urgency.ERROR, "Socket Exception: " + e);
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
                WriteToLog(Urgency.ERROR, "Socket Exception: " + e);
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


                    bool verbose = true;
                    bool.TryParse(GetSettingValue("VERBOSE"), out verbose);

                    if (verbose)
                        WriteToLog(Urgency.INFO, "Received [" + LightSwitchClientSocket.RemoteEndPoint.ToString() + "] " + data);

                    string[] commands = data.Split('\n');

                    string version = "VER~" + zvsEntityControl.zvsNameAndVersion;

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
                                    string hashedPassword = EncodePassword(Convert.ToString(m_cookie) + ":" + GetSettingValue("PASSWORD"));
                                    //hashedPassword = "638831F3AF6F32B25D6F1C961CBBA393"

                                    if (inputPassword.StartsWith(hashedPassword))
                                    {
                                        socketData.m_verified = true;
                                        SendMessagetoClientsSocket(LightSwitchClientSocket, version + Environment.NewLine);
                                        WriteToLog(Urgency.INFO, "[" + LightSwitchClientSocket.RemoteEndPoint.ToString() + "] User Authenticated.");
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

                                else if (cmd.StartsWith("ALIST"))  //DEVICES, SCENES AND ZONES.
                                {
                                    WriteToLog(Urgency.INFO, "[" + LightSwitchClientSocket.RemoteEndPoint.ToString() + "] User requested device list.");

                                    sendDeviceList(LightSwitchClientSocket);

                                    SendSceneList(LightSwitchClientSocket);

                                    SendZoneList(LightSwitchClientSocket);

                                    SendMessagetoClientsSocket(LightSwitchClientSocket, "ENDLIST" + Environment.NewLine);
                                }
                                else if (cmd.StartsWith("LIST")) //DEVICES
                                {
                                    WriteToLog(Urgency.INFO, "[" + LightSwitchClientSocket.RemoteEndPoint.ToString() + "] User requested device list.");

                                    sendDeviceList(LightSwitchClientSocket);

                                    SendMessagetoClientsSocket(LightSwitchClientSocket, "ENDLIST" + Environment.NewLine);
                                }
                                else if (cmd.StartsWith("SLIST"))  //SCENES
                                {
                                    SendSceneList(LightSwitchClientSocket);

                                    SendMessagetoClientsSocket(LightSwitchClientSocket, "ENDLIST" + Environment.NewLine);
                                }
                                else if (cmd.StartsWith("ZLIST")) //ZONES
                                {
                                    WriteToLog(Urgency.INFO, "[" + LightSwitchClientSocket.RemoteEndPoint.ToString() + "] User requested zone/group list.");

                                    SendZoneList(LightSwitchClientSocket);
                                    SendMessagetoClientsSocket(LightSwitchClientSocket, "ENDLIST" + Environment.NewLine);
                                }
                                else if (cmd.StartsWith("DEVICE"))
                                {
                                    string[] values = cmd.Split('~');
                                    //NOTIFY ALL CLIENTS
                                    ExecuteZVSCommand(Convert.ToInt64(values[1]), Convert.ToByte(values[2]), LightSwitchClientSocket);
                                }
                                else if (cmd.StartsWith("SCENE"))
                                {
                                    string[] values = cmd.Split('~');
                                    //NOTIFY ALL CLIENTS
                                    ExecuteZVSCommand(Convert.ToInt64(values[1]), LightSwitchClientSocket);
                                }
                                else if (cmd.StartsWith("ZONE"))
                                {
                                    string[] values = cmd.Split('~');
                                    if(values.Length > 1) 
                                    {
                                        long groupId = long.TryParse(values[1], out groupId) ? groupId : 0;
                                        string cmd_name = (values[2].Equals("255") ? "GROUP_ON" : "GROUP_OFF");
                                        using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                                        {
                                            group g = db.groups.FirstOrDefault(o => o.id == groupId);
                                            if (g != null)
                                            {
                                                builtin_commands zvs_cmd = db.builtin_commands.FirstOrDefault(c => c.name == cmd_name);
                                                if (zvs_cmd != null)
                                                {
                                                    string result = string.Format("[{0}] Ran {1} on group '{2}'", LightSwitchClientSocket.RemoteEndPoint.ToString(), zvs_cmd.friendly_name, g.name);
                                                    WriteToLog(Urgency.INFO, result);
                                                    BroadcastMessage("MSG~" + result + Environment.NewLine);

                                                    zvs_cmd.Run(g.id.ToString());
                                                }
                                            }
                                        }
                                    }                                    
                                    
                                }
                                else if (cmd.StartsWith("THERMMODE"))
                                {
                                    string[] values = cmd.Split('~');
                                    //NOTIFY ALL CLIENTS
                                    ExecuteZVSThermostatCommand(Convert.ToInt64(values[1]), Convert.ToByte(values[2]), LightSwitchClientSocket);
                                }
                                else if (cmd.StartsWith("THERMTEMP"))
                                {
                                    string[] values = cmd.Split('~');
                                    //NOTIFY ALL CLIENTS
                                    ExecuteZVSThermostatCommand(Convert.ToInt64(values[1]), Convert.ToByte(values[2]), Convert.ToInt32(values[3]), LightSwitchClientSocket);
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
                WriteToLog(Urgency.ERROR, "OnDataReceived - Socket has been closed");
            }
            catch (SocketException se)
            {
                if (se.ErrorCode == 10054) // Error code for Connection reset by peer
                    WriteToLog(Urgency.ERROR, "Client " + socketData.m_clientNumber + " Disconnected.");
                else
                    WriteToLog(Urgency.ERROR, "SocketException - " + se.Message);

                DisconnectClientSocket(socketData);
            }
            catch (Exception e)
            {
                WriteToLog(Urgency.ERROR, "[" + LightSwitchClientSocket.RemoteEndPoint.ToString() + "] Server Exception: " + e);

                //SEND ERROR TO CLIENT
                SendMessagetoClientsSocket(LightSwitchClientSocket, "ERR~" + e.Message + Environment.NewLine);

                Thread.Sleep(3000);
                DisconnectClientSocket(socketData);
            }
        }

        private void SendSceneList(Socket LightSwitchClientSocket)
        {
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                foreach (scene scene in db.scenes)
                {
                    bool show = false;
                    bool.TryParse(scene_property_value.GetPropertyValue(db, scene.id, "SHOWSCENEINLSLIST"), out show);

                    if (show)
                        SendMessagetoClientsSocket(LightSwitchClientSocket, "SCENE~" + scene.friendly_name + "~" + scene.id + Environment.NewLine);
                }
            }
        }

        private void SendZoneList(Socket LightSwitchClientSocket)
        {
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                foreach (group g in db.groups)
                {
                    SendMessagetoClientsSocket(LightSwitchClientSocket, "ZONE~" + g.name + "~" + g.id + Environment.NewLine);
                }
            }     
        }                       

        private void sendDeviceList(Socket LightSwitchClientSocket)
        {
            List<string> LS_devices = new List<string>();

            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                //Get Devices
                foreach (device d in db.devices)
                {
                    bool show = true;
                    bool.TryParse(device_property_values.GetDevicePropertyValue(d.id, "SHOWINLSLIST"), out show);

                    if (show)
                    {
                        string device_str = DeviceToString(d);
                        if (!string.IsNullOrEmpty(device_str))
                            LS_devices.Add(device_str);
                    }
                }
            }

            //Optional Sort By Name
            bool sort_list = true;
            bool.TryParse(GetSettingValue("SORTLIST"), out sort_list);
            if (sort_list)
                LS_devices.Sort();

            //Send to Client
            foreach (string d_str in LS_devices)
                SendMessagetoClientsSocket(LightSwitchClientSocket, "DEVICE~" + d_str + Environment.NewLine);

            
        }

        private string DeviceToString(device d)
        {            
            switch (d.device_types.name)
            {
                case "SWITCH":
                    {
                        int level = 0;
                        device_values dv = d.device_values.FirstOrDefault(v => v.label_name == "Basic");
                        if (dv != null)
                            int.TryParse(dv.value, out level);

                        return d.friendly_name + "~" + d.id + "~" + (level > 0 ? "255" : "0") + "~" + "BinarySwitch";
                    }
                case "DIMMER":
                    {
                        int level = 0;
                        device_values dv = d.device_values.FirstOrDefault(v => v.label_name == "Basic");
                        if (dv != null)
                            int.TryParse(dv.value, out level);

                        return d.friendly_name + "~" + d.id + "~" + level + "~" + "MultiLevelSwitch";
                    }
                case "THERMOSTAT":
                    {
                        int temp = 0;
                        device_values dv_temp = d.device_values.FirstOrDefault(v => v.label_name == "Temperature");
                        if (dv_temp != null)
                            int.TryParse(dv_temp.value, out temp);

                        return d.friendly_name + "~" + d.id + "~" + temp + "~" + "Thermostat";
                    }
                case "SENSOR":
                    {
                        int level = 0;
                        device_values dv = d.device_values.FirstOrDefault(v => v.label_name == "Basic");
                        if (dv != null)
                            int.TryParse(dv.value, out level);

                        return d.friendly_name + "~" + d.id + "~" + level + "~" + "Sensor";
                    }
            }
            return string.Empty;
        }

        private string DeviceToString(device_values dv)
        {
            //Only send applicable updated to LightSwitch
            if (dv.label_name == "Basic")
            {
                int level = 0;
                int.TryParse(dv.value, out level);

                switch (dv.device.device_types.name)
                {
                    case "SWITCH":
                        return dv.device.friendly_name + "~" + dv.device.id + "~" + (level > 0 ? "255" : "0") + "~" + "BinarySwitch";
                    case "DIMMER":
                        return dv.device.friendly_name + "~" + dv.device.id + "~" + level + "~" + "MultiLevelSwitch";
                    case "SENSOR":
                        return dv.device.friendly_name + "~" + dv.device.id + "~" + level + "~" + "Sensor";
                }
            }
            else if (dv.label_name == "Temperature")
            {
                if (dv.device.device_types.name.Equals("THERMOSTAT"))
                {
                    int temp = 0;
                    int.TryParse(dv.value, out temp);
                    return dv.device.friendly_name + "~" + dv.device.id + "~" + temp + "~" + "Thermostat";
                }
            }
            return string.Empty;
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

        /// <summary>
        /// Set levels for devices when a Lightswitch action level string is recieved.
        /// </summary>
        /// <param name="Node"></param>
        /// <param name="Level"></param>
        /// <param name="Client"></param>
        /// <returns></returns>
        private void ExecuteZVSCommand(long device_id, byte Level, Socket Client)
        {
            using (zvsEntities2 context = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                device d = context.devices.FirstOrDefault(o => o.id == device_id);

                if (d != null)
                {
                    switch (d.device_types.name)
                    {
                        case "SWITCH":
                            {
                                string cmd_name = (Level == 0 ? "TURNOFF" : "TURNON");
                                device_type_commands cmd = d.device_types.device_type_commands.FirstOrDefault(c => c.name == cmd_name);
                                if (cmd != null)
                                {
                                    string result = string.Format("[{0}] Executed command '{1}' on '{2}'.", Client.RemoteEndPoint.ToString(), cmd.friendly_name, d.friendly_name);
                                    WriteToLog(Urgency.INFO, result);

                                    device_type_command_que.Run(new device_type_command_que
                                    {
                                        device_id = d.id,
                                        device_type_command_id = cmd.id,
                                        arg = string.Empty
                                    });
                                    
                                    return;
                                }
                                break;
                            }
                        case "DIMMER":
                            {
                                if(ExecuteDynamicCMD(d, "DYNAMIC_CMD_BASIC", (Level == 255 ? "99" : Level.ToString()), Client))
                                    return;                                
                                break;
                            }
                    }                    
                }
                BroadcastMessage("ERR~Error setting device # " + device_id + ". Try Agian");
            }  
        }

        /// <summary>
        /// Runs a zVirtualScene Scene
        /// </summary>
        /// <param name="SceneID">Scene ID</param>
        /// <param name="Client">Clients Socket.</param>
        private void ExecuteZVSCommand(long SceneID, Socket Client)
        {
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                scene scene = db.scenes.FirstOrDefault(s => s.id == SceneID);
                if (scene != null)
                {
                    string result = scene.RunScene();
                    WriteToLog(Urgency.INFO, "[" + Client.RemoteEndPoint.ToString() + "] " + result);
                    BroadcastMessage("MSG~" + result + Environment.NewLine);
                }
            }
        }

        private bool ExecuteDynamicCMD(device d, string device_cmd_name, string arg, Socket Client)
        {
            device_commands cmd = d.device_commands.FirstOrDefault(c => c.name == device_cmd_name);
            if (cmd != null)
            {
                string result = string.Format("[{0}] Executed command '{1}{2}' on '{3}'.", Client.RemoteEndPoint.ToString(), cmd.friendly_name, string.IsNullOrEmpty(arg) ? arg : " to " + arg, d.friendly_name);
                WriteToLog(Urgency.INFO, result);

                device_command_que.Run(new device_command_que
                {
                    device_id = d.id,
                    device_command_id = cmd.id,
                    arg = arg
                });
                
                return true;
            }
            return false;
        }

        private void ExecuteZVSThermostatCommand(long deviceID, byte Mode, int Temp, Socket Client)
        {
            using (zvsEntities2 context = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                device d = context.devices.FirstOrDefault(o => o.id == deviceID);

                if (d != null && d.device_types.name.Equals("THERMOSTAT"))
                {
                    switch (Mode)
                    {
                        case 2:
                            {
                                if (ExecuteDynamicCMD(d, "DYNAMIC_CMD_HEATING 1", Temp.ToString(), Client))
                                    return;    
                                break;
                            }
                        case 3:
                            {
                                if (ExecuteDynamicCMD(d, "DYNAMIC_CMD_COOLING 1", Temp.ToString(), Client))
                                    return;
                                break;
                            }
                    }
                }
            }
            BroadcastMessage("ERR~Error setting device # " + deviceID + ". Try Agian");
        }

        private void ExecuteZVSThermostatCommand(long deviceID, byte Mode, Socket Client)
        {

            using (zvsEntities2 context = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                device d = context.devices.FirstOrDefault(o => o.id == deviceID);

                if (d != null && d.device_types.name.Equals("THERMOSTAT"))
                {
                    switch (Mode)
                    {
                        case 0:
                            {
                                if (ExecuteDynamicCMD(d, "DYNAMIC_CMD_MODE", "Off", Client))
                                    return;
                                break;
                            }
                        case 1:
                            {
                                if (ExecuteDynamicCMD(d, "DYNAMIC_CMD_MODE", "Auto", Client))
                                    return;
                                break;
                            }
                        case 2:
                            {
                                if (ExecuteDynamicCMD(d, "DYNAMIC_CMD_MODE", "Heat", Client))
                                    return;
                                break;
                            }
                        case 3:
                            {
                                if (ExecuteDynamicCMD(d, "DYNAMIC_CMD_MODE", "Cool", Client))
                                    return;
                                break;
                            }
                        case 4:
                            {
                                if (ExecuteDynamicCMD(d, "DYNAMIC_CMD_FAN MODE", "On Low", Client))
                                    return;
                                break;
                            }
                        case 5:
                            {
                                if (ExecuteDynamicCMD(d, "DYNAMIC_CMD_FAN MODE", "Auto Low", Client))
                                    return;
                                break;
                            }
                        case 6:
                            {                               
                                device_type_commands cmd = d.device_types.device_type_commands.FirstOrDefault(c => c.name == "SETENERGYMODE");
                                if (cmd != null)
                                {
                                    WriteToLog(Urgency.INFO, "[" + Client.RemoteEndPoint.ToString() + "] Executed command " + cmd.friendly_name + " on " + d.friendly_name + ".");

                                    device_type_command_que.Run(new device_type_command_que
                                    {
                                        device_id = d.id,
                                        device_type_command_id = cmd.id,
                                        arg = string.Empty
                                    });

                                   
                                    return;
                                }
                                break;
                            }
                            case 7:
                            {                               
                                device_type_commands cmd = d.device_types.device_type_commands.FirstOrDefault(c => c.name == "SETCONFORTMODE");
                                if (cmd != null)
                                {
                                    WriteToLog(Urgency.INFO, "[" + Client.RemoteEndPoint.ToString() + "] Executed command" + cmd.friendly_name + " on " + d.friendly_name + ".");
                                    
                                    device_type_command_que.Run(new device_type_command_que
                                    {
                                        device_id = d.id,
                                        device_type_command_id = cmd.id,
                                        arg = string.Empty
                                    });

                                    return;
                                }
                                break;
                            }
                    }
                }
            }
            BroadcastMessage("ERR~Error setting device # " + deviceID + ". Try Agian");
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

                bool verbose = true;
                bool.TryParse(GetSettingValue("VERBOSE"), out verbose);

                if (verbose)
                    WriteToLog(Urgency.INFO, "SENT TO ALL - " + msg);
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


                bool verbose = true;
                bool.TryParse(GetSettingValue("VERBOSE"), out verbose);

                if (verbose)
                    WriteToLog(Urgency.INFO, "SENT - " + msg);
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

            bool verbose = true;
            bool.TryParse(GetSettingValue("VERBOSE"), out verbose);

            if (verbose)
                WriteToLog(Urgency.INFO, "SENT " + msg);
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
                WriteToLog(Urgency.INFO, "Socket Disconnect: " + e);
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


        #region ZeroConf/Bonjour

        private void PublishZeroconf()
        {
            int port = 9909;
            int.TryParse(GetSettingValue("PORT"), out port);

            string domain = "";
            String type = "_lightswitch._tcp.";
            String name = "Lightswitch " + Environment.MachineName;
            netservice = new NetService(domain, type, name, port);
            netservice.AllowMultithreadedCallbacks = true;
            netservice.DidPublishService += new NetService.ServicePublished(publishService_DidPublishService);
            netservice.DidNotPublishService += new NetService.ServiceNotPublished(publishService_DidNotPublishService);

            /* HARDCODE TXT RECORD */
            System.Collections.Hashtable dict = new System.Collections.Hashtable();
            dict = new System.Collections.Hashtable();
            dict.Add("txtvers", "1");
            dict.Add("ServiceName", name);
            dict.Add("MachineName", Environment.MachineName);
            dict.Add("OS", Environment.OSVersion.ToString());
            dict.Add("IPAddress", "127.0.0.1");
            dict.Add("Version", zvsEntityControl.zvsNameAndVersion);
            netservice.TXTRecordData = NetService.DataFromTXTRecordDictionary(dict);
            netservice.Publish();
            
        }

        void publishService_DidPublishService(NetService service)
        {
            WriteToLog(Urgency.INFO, String.Format("Published Service: domain({0}) type({1}) name({2})", service.Domain, service.Type, service.Name));
        }

        void publishService_DidNotPublishService(NetService service, DNSServiceException ex)
        {
            WriteToLog(Urgency.ERROR, ex.Message);
        }

        #endregion 
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

