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
using System.Linq;
using ZeroconfService;
using zvs.Processor;
using zvs.Entities;



namespace LightSwitchPlugin
{
    [Export(typeof(zvsPlugin))]
    public class LightSwitchPlugin : zvsPlugin
    {
        private Socket LightSwitchSocket;
        private readonly List<Socket> LightSwitchClients = new List<Socket>();
        public AsyncCallback pfnWorkerCallBack;
        private int m_cookie = new Random().Next(65536);
        public volatile bool isActive = false;
        private NetService netservice = null;
        private bool _verbose = false;
        private bool _useBonjour = false;
        private bool _sort_list = true;
        private int _port = 9909;
        private int _max_conn = 50;
        zvs.Processor.Logging.ILog log = zvs.Processor.Logging.LogManager.GetLogger<LightSwitchPlugin>();
        public LightSwitchPlugin()
            : base("LIGHTSWITCH",
               "LightSwitch Plug-in",
                "This plug-in is a server that allows LightSwitch clients to connect and control zVirtualScene devices."
                ) { }

        public override void Initialize()
        {
            using (zvsContext context = new zvsContext())
            {
                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "PORT",
                    Name = "Port",
                    Value = (1337).ToString(),
                    ValueType = DataType.INTEGER,
                    Description = "LightSwitch will listen for connections on this port."
                }, context);

                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "MAXCONN",
                    Name = "Max Conn.",
                    Value = (200).ToString(),
                    ValueType = DataType.INTEGER,
                    Description = "The maximum number of connections allowed."
                }, context);

                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "VERBOSE",
                    Name = "Verbose Logging",
                    Value = false.ToString(),
                    ValueType = DataType.BOOL,
                    Description = "(Writes all server client communication to the log for debugging.)"
                }, context);

                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "PASSWORD",
                    Name = "Password",
                    Value = "ChaNgeMe444",
                    ValueType = DataType.STRING,
                    Description = "The password clients must use to connect to the LightSwitch server. "
                }, context);

                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "SORTLIST",
                    Name = "Sort Device List",
                    Value = true.ToString(),
                    ValueType = DataType.BOOL,
                    Description = "(Alphabetically sorts the device list.)"
                }, context);

                DefineOrUpdateSetting(new PluginSetting
                {
                    UniqueIdentifier = "PUBLISHZEROCFG",
                    Name = "Publish ZeroConf/Bonjour",
                    Value = false.ToString(),
                    ValueType = DataType.BOOL,
                    Description = "Zero configuration networking allows clients on your network to detect and connect to your LightSwitch server automatically."
                }, context);

                DeviceProperty.AddOrEdit(new DeviceProperty
                {
                    UniqueIdentifier = "SHOWINLSLIST",
                    Name = "Show device in LightSwitch",
                    Description = "If enabled this device will show in the LightSwitch device tab.",
                    ValueType = DataType.BOOL,
                    Value = "true"
                }, context);

                SceneProperty.AddOrEdit(new SceneProperty
                {
                    UniqueIdentifier = "SHOWSCENEINLSLIST",
                    Name = "Show scene in LightSwitch",
                    Description = "If enabled this scene will show in the LightSwitch scene tab.",
                    Value = "true",
                    ValueType = DataType.BOOL
                }, context);

                bool.TryParse(GetSettingValue("VERBOSE", context), out _verbose);
                bool.TryParse(GetSettingValue("PUBLISHZEROCFG", context), out _useBonjour);
                bool.TryParse(GetSettingValue("SORTLIST", context), out _sort_list);
                int.TryParse(GetSettingValue("PORT", context), out _port);
                int.TryParse(GetSettingValue("MAXCONN", context), out _max_conn);
            }
        }

        protected override void StartPlugin()
        {
            using (zvsContext context = new zvsContext())
            {
                DeviceValue.DeviceValueDataChangedEvent += DeviceValue_DeviceValueDataChangedEvent;
                // zvs.Processor.CommandProcessor.onProcessingCommandBegin += PluginManager_onProcessingCommandBegin;
                // zvs.Processor.PluginManager.onProcessingCommandEnd += PluginManager_onProcessingCommandEnd;
                OpenLightSwitchSocket();
            }
        }

        protected override void StopPlugin()
        {
            DeviceValue.DeviceValueDataChangedEvent -= DeviceValue_DeviceValueDataChangedEvent;
            //zvs.Processor.PluginManager.onProcessingCommandBegin -= PluginManager_onProcessingCommandBegin;
            //zvs.Processor.PluginManager.onProcessingCommandEnd -= PluginManager_onProcessingCommandEnd;
            CloseLightSwitchSocket();
        }

        protected override void SettingChanged(string UniqueIdentifier, string settingValue)
        {
            if (UniqueIdentifier == "VERBOSE")
            {
                bool.TryParse(settingValue, out _verbose);
            }
            else if (UniqueIdentifier == "PUBLISHZEROCFG")
            {
                bool.TryParse(settingValue, out _useBonjour);
                publishZeroConf();
            }
            else if (UniqueIdentifier == "SORTLIST")
            {
                bool.TryParse(settingValue, out _sort_list);
            }
            else if (UniqueIdentifier == "PORT")
            {
                if (this.Enabled)
                    CloseLightSwitchSocket();

                int.TryParse(settingValue, out _port);

                if (this.Enabled)
                    OpenLightSwitchSocket();

            }
            else if (UniqueIdentifier == "MAXCONN")
            {
                if (this.Enabled)
                    CloseLightSwitchSocket();

                int.TryParse(settingValue, out _max_conn);

                if (this.Enabled)
                    OpenLightSwitchSocket();
            }
        }

        private void publishZeroConf()
        {
            if (_useBonjour)
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
                    log.Fatal(ex);
                }
            }
            else
            {
                if (netservice == null)
                    netservice.Dispose();
            }
        }

        public override void ProcessDeviceCommand(zvs.Entities.QueuedDeviceCommand cmd) { }

        public override void ProcessDeviceTypeCommand(zvs.Entities.QueuedDeviceTypeCommand cmd) { }

        public override void Repoll(zvs.Entities.Device device) { }

        public override void ActivateGroup(int groupID) { }

        public override void DeactivateGroup(int groupID) { }

        private void DeviceValue_DeviceValueDataChangedEvent(object sender, DeviceValue.ValueDataChangedEventArgs args)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (s, a) =>
            {
                using (zvsContext context = new zvsContext())
                {
                    DeviceValue dv = context.DeviceValues.FirstOrDefault(v => v.Id == args.DeviceValueId);
                    if (dv != null)
                    {
                        if (dv.Name == "Basic")
                        {
                            string UpdateString = DeviceToString(dv.Device);
                            if (!string.IsNullOrEmpty(UpdateString))
                            {
                                BroadcastMessage("UPDATE~" + UpdateString + Environment.NewLine);
                                BroadcastMessage("ENDLIST" + Environment.NewLine);
                            }

                            string device_name = string.Empty;
                            device_name = dv.Device.Name;
                            BroadcastMessage("MSG~" + "'" + device_name + "' " + dv.Name + " changed to " + args.newValue + Environment.NewLine);
                        }
                    }
                }
            };
            bw.RunWorkerAsync();

        }

        //void PluginManager_onProcessingCommandEnd(object sender, onProcessingCommandEventArgs args)
        //{
        //    BackgroundWorker bw = new BackgroundWorker();
        //    bw.DoWork += (s, a) =>
        //    {
        //        if (args.hasErrors)
        //            BroadcastMessage("ERR~" + args.Details + Environment.NewLine);
        //    };
        //    bw.RunWorkerAsync();
        //}

        //void PluginManager_onProcessingCommandBegin(object sender, PluginManager.onProcessingCommandEventArgs args)
        //{
        //    BackgroundWorker bw = new BackgroundWorker();
        //    bw.DoWork += (s, a) =>
        //    {
        //        if (args.hasErrors)
        //            BroadcastMessage("ERR~" + args.Details + Environment.NewLine);
        //    };
        //    bw.RunWorkerAsync();
        //}

        /// <summary>
        /// Starts listening for LightSwitch clients. 
        /// </summary>
        public void OpenLightSwitchSocket()
        {
            if (LightSwitchSocket == null || !isActive)
            {
                try
                {
                    using (zvsContext context = new zvsContext())
                    {
                        isActive = true;
                        LightSwitchSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        LightSwitchSocket.Bind(new IPEndPoint(IPAddress.Any, _port));
                        LightSwitchSocket.Listen(_max_conn);
                        LightSwitchSocket.BeginAccept(new AsyncCallback(OnLightSwitchClientConnect), null);
                        log.Info("LightSwitch server started on port " + _port);
                        IsReady = true;
                    }
                }
                catch (SocketException e)
                {
                    log.Error("Socket Failed to Open - " + e);
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
                log.Info("LightSwitch server stopped");
                isActive = false;
                IsReady = false;
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

                log.Info("Connection Attempt from: " + LightSwitchClientsSocket.RemoteEndPoint.ToString());

                // Send a welcome message to client                
                string msg = "LightSwitch zVirtualScenes Plug-in (Active Connections " + LightSwitchClients.Count + ")" + Environment.NewLine;
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
                log.Error("Socket Exception: " + e);
            }
            catch (Exception)
            { }
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
                log.Error("Socket Exception: " + e);
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
                using (zvsContext context = new zvsContext())
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

                        if (_verbose)
                            log.Info("Received [" + LightSwitchClientSocket.RemoteEndPoint.ToString() + "] " + data);

                        string[] commands = data.Split('\n');

                        string version = "VER~" + Utils.ApplicationNameAndVersion;

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
                                        string hashedPassword = EncodePassword(Convert.ToString(m_cookie) + ":" + GetSettingValue("PASSWORD", context));
                                        //hashedPassword = "638831F3AF6F32B25D6F1C961CBBA393"

                                        if (inputPassword.StartsWith(hashedPassword))
                                        {
                                            socketData.m_verified = true;
                                            SendMessagetoClientsSocket(LightSwitchClientSocket, version + Environment.NewLine);
                                            log.Info("[" + LightSwitchClientSocket.RemoteEndPoint.ToString() + "] User Authenticated.");
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
                                        log.Info("[" + LightSwitchClientSocket.RemoteEndPoint.ToString() + "] User requested device list.");

                                        sendDeviceList(LightSwitchClientSocket);

                                        SendSceneList(LightSwitchClientSocket);

                                        SendZoneList(LightSwitchClientSocket);

                                        SendMessagetoClientsSocket(LightSwitchClientSocket, "ENDLIST" + Environment.NewLine);
                                    }
                                    else if (cmd.StartsWith("LIST")) //DEVICES
                                    {
                                        log.Info("[" + LightSwitchClientSocket.RemoteEndPoint.ToString() + "] User requested device list.");

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
                                        log.Info("[" + LightSwitchClientSocket.RemoteEndPoint.ToString() + "] User requested zone/group list.");

                                        SendZoneList(LightSwitchClientSocket);
                                        SendMessagetoClientsSocket(LightSwitchClientSocket, "ENDLIST" + Environment.NewLine);
                                    }
                                    else if (cmd.StartsWith("DEVICE"))
                                    {
                                        string[] values = cmd.Split('~');
                                        //NOTIFY ALL CLIENTS
                                        ExecuteZVSCommand(Convert.ToInt32(values[1]), Convert.ToByte(values[2]), LightSwitchClientSocket);
                                    }
                                    else if (cmd.StartsWith("SCENE"))
                                    {
                                        string[] values = cmd.Split('~');
                                        //NOTIFY ALL CLIENTS
                                        ExecuteZVSCommand(Convert.ToInt32(values[1]), LightSwitchClientSocket);
                                    }
                                    else if (cmd.StartsWith("ZONE"))
                                    {
                                        string[] values = cmd.Split('~');
                                        if (values.Length > 1)
                                        {
                                            int groupId = int.TryParse(values[1], out groupId) ? groupId : 0;
                                            string cmdUniqId = (values[2].Equals("255") ? "GROUP_ON" : "GROUP_OFF");

                                            Group g = context.Groups.FirstOrDefault(o => o.Id == groupId);
                                            if (g != null)
                                            {
                                                BuiltinCommand zvs_cmd = context.BuiltinCommands.FirstOrDefault(c => c.UniqueIdentifier == cmdUniqId);
                                                if (zvs_cmd != null)
                                                {
                                                    string result = string.Format("[{0}] Ran {1} on group '{2}'", LightSwitchClientSocket.RemoteEndPoint.ToString(), zvs_cmd.Name, g.Name);
                                                    log.Info(result);
                                                    BroadcastMessage("MSG~" + result + Environment.NewLine);

                                                    CommandProcessor cp = new CommandProcessor(Core);
                                                    cp.RunBuiltinCommand(context, zvs_cmd, g.Id.ToString());
                                                }
                                            }

                                        }

                                    }
                                    else if (cmd.StartsWith("THERMMODE"))
                                    {
                                        string[] values = cmd.Split('~');
                                        //NOTIFY ALL CLIENTS
                                        ExecuteZVSThermostatCommand(Convert.ToInt32(values[1]), Convert.ToByte(values[2]), LightSwitchClientSocket);
                                    }
                                    else if (cmd.StartsWith("THERMTEMP"))
                                    {
                                        string[] values = cmd.Split('~');
                                        //NOTIFY ALL CLIENTS
                                        ExecuteZVSThermostatCommand(Convert.ToInt32(values[1]), Convert.ToByte(values[2]), Convert.ToInt32(values[3]), LightSwitchClientSocket);
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
            }
            catch (ObjectDisposedException)
            {
                log.Error("OnDataReceived - Socket has been closed");
            }
            catch (SocketException se)
            {
                if (se.ErrorCode == 10054) // Error code for Connection reset by peer
                    log.Error("Client " + socketData.m_clientNumber + " Disconnected.");
                else
                    log.Error("SocketException - " + se.Message);

                DisconnectClientSocket(socketData);
            }
            catch (Exception e)
            {
                log.Error("[" + LightSwitchClientSocket.RemoteEndPoint.ToString() + "] Server Exception: " + e);

                //SEND ERROR TO CLIENT
                SendMessagetoClientsSocket(LightSwitchClientSocket, "ERR~" + e.Message + Environment.NewLine);

                Thread.Sleep(3000);
                DisconnectClientSocket(socketData);
            }
        }

        private void SendSceneList(Socket LightSwitchClientSocket)
        {
            using (zvsContext context = new zvsContext())
            {
                foreach (Scene scene in context.Scenes)
                {
                    bool show = false;
                    bool.TryParse(ScenePropertyValue.GetPropertyValue(context, scene, "SHOWSCENEINLSLIST"), out show);

                    if (show)
                        SendMessagetoClientsSocket(LightSwitchClientSocket, "SCENE~" + scene.Name + "~" + scene.Id + Environment.NewLine);
                }
            }
        }

        private void SendZoneList(Socket LightSwitchClientSocket)
        {
            using (zvsContext context = new zvsContext())
            {
                foreach (Group g in context.Groups)
                {
                    SendMessagetoClientsSocket(LightSwitchClientSocket, "ZONE~" + g.Name + "~" + g.Id + Environment.NewLine);
                }
            }
        }

        private void sendDeviceList(Socket LightSwitchClientSocket)
        {
            List<string> LS_devices = new List<string>();

            using (zvsContext context = new zvsContext())
            {
                //Get Devices
                foreach (Device d in context.Devices)
                {
                    bool show = true;
                    bool.TryParse(DevicePropertyValue.GetDevicePropertyValue(context, d, "SHOWINLSLIST"), out show);

                    if (show)
                    {
                        string device_str = DeviceToString(d);
                        if (!string.IsNullOrEmpty(device_str))
                            LS_devices.Add(device_str);
                    }
                }

                if (_sort_list)
                    LS_devices.Sort();

                //Send to Client
                foreach (string d_str in LS_devices)
                    SendMessagetoClientsSocket(LightSwitchClientSocket, "DEVICE~" + d_str + Environment.NewLine);
            }

        }

        private string DeviceToString(Device d)
        {
            switch (d.Type.UniqueIdentifier)
            {
                case "SWITCH":
                    return d.Name + "~" + d.Id + "~" + (d.CurrentLevelInt > 0 ? "255" : "0") + "~" + "BinarySwitch";
                case "DIMMER":
                    return d.Name + "~" + d.Id + "~" + (int)d.CurrentLevelInt + "~" + "MultiLevelSwitch"; //careful lightswitch can only handle ints
                case "THERMOSTAT":
                    return d.Name + "~" + d.Id + "~" + (int)d.CurrentLevelInt + "~" + "Thermostat"; //careful lightswitch can only handle ints
                case "SENSOR":
                    return d.Name + "~" + d.Id + "~" + (int)d.CurrentLevelInt + "~" + "Sensor"; //careful lightswitch can only handle ints
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
        private void ExecuteZVSCommand(int device_id, byte Level, Socket Client)
        {
            using (zvsContext context = new zvsContext())
            {
                Device d = context.Devices.FirstOrDefault(o => o.Id == device_id);

                if (d != null)
                {
                    switch (d.Type.UniqueIdentifier)
                    {
                        case "SWITCH":
                            {
                                string cmdUniqueId = (Level == 0 ? "TURNOFF" : "TURNON");
                                DeviceTypeCommand cmd = d.Type.Commands.FirstOrDefault(c => c.UniqueIdentifier == cmdUniqueId);
                                if (cmd != null)
                                {
                                    string result = string.Format("[{0}] Executed command '{1}' on '{2}'.", Client.RemoteEndPoint.ToString(), cmd.Name, d.Name);
                                    log.Info(result);
                                    CommandProcessor cp = new CommandProcessor(Core);
                                    cp.RunDeviceTypeCommand(context, cmd, d);
                                    return;
                                }
                                break;
                            }
                        case "DIMMER":
                            {
                                string l = (Level == 255 ? "99" : Level.ToString());
                                if (d.Type.Plugin.UniqueIdentifier == "OPENZWAVE")
                                {
                                    if (ExecuteDynamicCMD(context, d, "DYNAMIC_CMD_BASIC", l, Client))
                                        return;
                                }
                                if (d.Type.Plugin.UniqueIdentifier == "THINKSTICK")
                                {
                                    if (ExecuteDynamicCMD(context, d, "BASIC", l, Client))
                                        return;
                                }
                                break;
                            }
                    }
                }
                BroadcastMessage("ERR~Error setting device # " + device_id + ". Try Again");
            }
        }

        /// <summary>
        /// Runs a zVirtualScene Scene
        /// </summary>
        /// <param name="SceneID">Scene ID</param>
        /// <param name="Client">Clients Socket.</param>
        private void ExecuteZVSCommand(int SceneID, Socket Client)
        {
            using (zvsContext context = new zvsContext())
            {
                BuiltinCommand cmd = context.BuiltinCommands.FirstOrDefault(c => c.UniqueIdentifier == "RUN_SCENE");
                if (cmd != null)
                {
                    CommandProcessor cp = new CommandProcessor(Core);
                    cp.onProcessingCommandBegin += (s, a) =>
                    {
                        BroadcastMessage("MSG~" + a.Details + Environment.NewLine);
                    };
                    cp.onProcessingCommandEnd += (s, a) =>
                    {
                        BroadcastMessage("MSG~" + a.Details + Environment.NewLine);
                    };
                    cp.RunBuiltinCommand(context, cmd, SceneID.ToString());
                }
            }
        }

        private bool ExecuteDynamicCMD(zvsContext context, Device d, string cmdUniqueId, string arg, Socket Client)
        {
            DeviceCommand cmd = d.Commands.FirstOrDefault(c => c.UniqueIdentifier == cmdUniqueId);
            if (cmd != null)
            {
                string result = string.Format("[{0}] Executed command '{1}{2}' on '{3}'.", Client.RemoteEndPoint.ToString(), cmd.Name, string.IsNullOrEmpty(arg) ? arg : " to " + arg, d.Name);
                log.Info(result);
                CommandProcessor cp = new CommandProcessor(Core);
                cp.RunDeviceCommand(context, cmd, arg);
                return true;
            }
            return false;
        }

        private Dictionary<string, string> ThermoTempCommandTranslations = new Dictionary<string, string>()
        {
            {"THINKSTICK2", "DYNAMIC_SP_R207_Heating1"},
            {"OPENZWAVE2", "DYNAMIC_CMD_HEATING 1"},
            {"THINKSTICK3", "DYNAMIC_SP_R207_Cooling1"},
            {"OPENZWAVE3", "DYNAMIC_CMD_COOLING 1"} 
        };

        private void ExecuteZVSThermostatCommand(int deviceID, byte Mode, int Temp, Socket Client)
        {
            using (zvsContext context = new zvsContext())
            {
                Device d = context.Devices.FirstOrDefault(o => o.Id == deviceID);
                if (d != null && d.Type.UniqueIdentifier.Equals("THERMOSTAT"))
                {
                    string plugin = d.Type.Plugin.UniqueIdentifier;
                    string key = plugin + Mode;

                    if (ThermoTempCommandTranslations.ContainsKey(key))
                    {
                        if (ExecuteDynamicCMD(context, d, ThermoTempCommandTranslations[key], Temp.ToString(), Client))
                            return;
                    }
                }
            }
            BroadcastMessage("ERR~Error setting device # " + deviceID + ". Try Agian");
        }

        private class zvsCMD
        {
            public string CmdName;
            public string arg;
        }

        private Dictionary<string, zvsCMD> ThermoCommandTranslations = new Dictionary<string, zvsCMD>()
        {
            {"THINKSTICK0", new zvsCMD() { CmdName="MODE", arg="Off"}},
            {"OPENZWAVE0", new zvsCMD() { CmdName="DYNAMIC_CMD_MODE", arg="Off"}},
            {"THINKSTICK1", new zvsCMD() { CmdName="MODE", arg="Auto"}},
            {"OPENZWAVE1", new zvsCMD() { CmdName="DYNAMIC_CMD_MODE", arg="Auto"}},
            {"THINKSTICK2", new zvsCMD() { CmdName="MODE", arg="Heat"}},
            {"OPENZWAVE2", new zvsCMD() { CmdName="DYNAMIC_CMD_MODE", arg="Heat"}},
            {"THINKSTICK3", new zvsCMD() { CmdName="MODE", arg="Cool"}},
            {"OPENZWAVE3", new zvsCMD() { CmdName="DYNAMIC_CMD_MODE", arg="Cool"}},
            {"THINKSTICK4", new zvsCMD() { CmdName="FAN_MODE", arg="OnLow"}},
            {"OPENZWAVE4", new zvsCMD() { CmdName="DYNAMIC_CMD_FAN MODE", arg="On Low"}},
            {"THINKSTICK5", new zvsCMD() { CmdName="FAN_MODE", arg="AutoLow"}},
            {"OPENZWAVE5", new zvsCMD() { CmdName="DYNAMIC_CMD_FAN MODE", arg="Auto Low"}}

        };

        private void ExecuteZVSThermostatCommand(int deviceID, byte Mode, Socket Client)
        {
            //PLUGINNAME-0 -->CmdName,Arg 
            using (zvsContext context = new zvsContext())
            {
                Device d = context.Devices.FirstOrDefault(o => o.Id == deviceID);
                if (d != null && d.Type.UniqueIdentifier.Equals("THERMOSTAT"))
                {
                    string plugin = d.Type.Plugin.UniqueIdentifier;
                    string key = plugin + Mode;

                    if (ThermoCommandTranslations.ContainsKey(key))
                    {
                        zvsCMD cmd = ThermoCommandTranslations[key];
                        if (ExecuteDynamicCMD(context, d, cmd.CmdName, cmd.arg, Client))
                            return;
                    }

                    switch (Mode)
                    {
                        case 6:
                            {
                                DeviceTypeCommand cmd = d.Type.Commands.FirstOrDefault(c => c.UniqueIdentifier == "SETENERGYMODE");
                                if (cmd != null)
                                {
                                    log.Info("[" + Client.RemoteEndPoint.ToString() + "] Executed command " + cmd.Name + " on " + d.Name + ".");
                                    CommandProcessor cp = new CommandProcessor(Core);
                                    cp.RunDeviceTypeCommand(context, cmd, d);
                                    return;
                                }
                                break;
                            }
                        case 7:
                            {
                                DeviceTypeCommand cmd = d.Type.Commands.FirstOrDefault(c => c.UniqueIdentifier == "SETCONFORTMODE");
                                if (cmd != null)
                                {
                                    log.Info("[" + Client.RemoteEndPoint.ToString() + "] Executed command " + cmd.Name + " on " + d.Name + ".");
                                    CommandProcessor cp = new CommandProcessor(Core);
                                    cp.RunDeviceTypeCommand(context, cmd, d);
                                    return;
                                }
                                break;
                            }
                    }
                }
            }
            BroadcastMessage("ERR~Error setting device # " + deviceID + ". Try Again");
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
                    if (workerSocket != null && workerSocket.Connected)
                    {
                        try
                        {
                            workerSocket.Send(byData);
                        }
                        catch (SocketException se)
                        {
                            if (_verbose)
                                log.Error("Socket Exception: " + se.Message);

                            return;
                        }
                    }

                if (_verbose)
                    log.Info("SENT TO ALL - " + msg);

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
                if (LightSwitchClientSocket != null && LightSwitchClientSocket.Connected)
                {
                    try
                    {
                        LightSwitchClientSocket.Send(byData);
                    }
                    catch (SocketException se)
                    {
                        if (_verbose)
                            log.Error("Socket Exception: " + se.Message);
                    }
                }

                if (_verbose)
                    log.Info("SENT - " + msg);

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

            if (LightSwitchClientsSocket != null && LightSwitchClientsSocket.Connected)
            {
                try
                {
                    LightSwitchClientsSocket.Send(byData);
                }
                catch (SocketException se)
                {
                    if (_verbose)
                        log.Error("Socket Exception: " + se.Message);
                }
            }


            if (_verbose)
                log.Info("SENT " + msg);

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
                log.Info("Socket Disconnect: " + e);
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
            using (zvsContext context = new zvsContext())
            {
                int port = 9909;
                int.TryParse(GetSettingValue("PORT", context), out port);

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
                dict.Add("Version", Utils.ApplicationNameAndVersion);
                netservice.TXTRecordData = NetService.DataFromTXTRecordDictionary(dict);
                netservice.Publish();
            }

        }

        void publishService_DidPublishService(NetService service)
        {
            log.Info(String.Format("Published Service: domain({0}) type({1}) name({2})", service.Domain, service.Type, service.Name));
        }

        void publishService_DidNotPublishService(NetService service, DNSServiceException ex)
        {
            log.Error(ex.Message);
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

