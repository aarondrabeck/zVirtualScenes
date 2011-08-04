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

        public LightSwitchPlugin()
            : base("LIGHTSWITCH")
        {
            PluginName = "LightSwitch";
        }

        protected override bool StartPlugin()
        {
            zVirtualSceneEvents.ValueDataChangedEvent += new zVirtualSceneEvents.ValueDataChangedEventHandler(zVirtualSceneEvents_ValueDataChangedEvent);
            zVirtualSceneEvents.CommandRunCompleteEvent +=new zVirtualSceneEvents.CommandRunCompleteEventHandler(zVirtualSceneEvents_CommandRunCompleteEvent);
            zVirtualSceneEvents.SceneRunCompleteEvent += new zVirtualSceneEvents.SceneRunCompleteEventHandler(zVirtualSceneEvents_SceneRunCompleteEvent);
            API.WriteToLog(Urgency.INFO, PluginName + " plugin started.");
            OpenLightSwitchSocket();
            IsReady = true;
            return true;
        }   

        protected override bool StopPlugin()
        {
            zVirtualSceneEvents.ValueDataChangedEvent -= new zVirtualSceneEvents.ValueDataChangedEventHandler(zVirtualSceneEvents_ValueDataChangedEvent);
            zVirtualSceneEvents.CommandRunCompleteEvent -= new zVirtualSceneEvents.CommandRunCompleteEventHandler(zVirtualSceneEvents_CommandRunCompleteEvent);
            zVirtualSceneEvents.SceneRunCompleteEvent -= new zVirtualSceneEvents.SceneRunCompleteEventHandler(zVirtualSceneEvents_SceneRunCompleteEvent);
            
            API.WriteToLog(Urgency.INFO, PluginName + " plugin ended.");
            CloseLightSwitchSocket();
            IsReady = false;
            return true;
        }

        protected override void SettingChanged(string settingName, string settingValue)
        {
            //throw new NotImplementedException();
        }

        public override void Initialize()
        {

            API.InstallObjectType("LIGHTSWITCH", false);
            //API.InstallObjectTypeCommand("SPEECH", "Say", "SAY", ParamType.STRING);

            API.NewObject(1, "LIGHTSWITCH", "LIGHTSWITCH");
            API.DefineSetting("Port", "1337", ParamType.INTEGER, "LightSwitch will listen for connections on this port.");
            API.DefineSetting("Max Conn.", "200", ParamType.INTEGER, "The maximum number of connections allowed.");
            API.DefineSetting("Verbose Logging", "true", ParamType.BOOL, "(Writes all server client communication to the log for debugging.)");
            API.DefineSetting("Password", "ChaNgeMe444", ParamType.STRING, "The password clients must use to connect to the LightSwitch server. ");
            API.DefineSetting("Sort Device List", "true", ParamType.BOOL, "(Alphabetically sorts the device list.)");

            API.Object.Properties.NewObjectProperty("SHOWINLSLIST", "Show in Lightswitch List", "true", ParamType.BOOL);

            //API.NewObjectProperty("TESTLIST", "Test List", "NO", ParamType.LIST);
            //API.NewObjectPropertyOption("TESTLIST", "YES");
            //API.NewObjectPropertyOption("TESTLIST", "NO");
            //API.NewObjectPropertyOption("TESTLIST", "MAYBE"); 

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

        void zVirtualSceneEvents_ValueDataChangedEvent(int ObjectId, string ValueID, string label, string Value, string PreviousValue)
        {
            zwObject obj = (zwObject)API.Object.GetObject(ObjectId);
            BroadcastMessage("UPDATE~" + TranslateObjectToLightSwitchString(obj) + Environment.NewLine);
            BroadcastMessage("ENDLIST" + Environment.NewLine);

            BroadcastMessage("MSG~" + "'" + obj.Name + "' " + label + " changed to " + Value + Environment.NewLine);
        }

        void zVirtualSceneEvents_SceneRunCompleteEvent(int SceneID, int ErrorCount)
        {
            Scene scene = API.Scenes.GetScene(SceneID);
            if (scene != null)
            {
                BroadcastMessage("MSG~" + "Scene '" + scene.txt_name + "' has completed with " + ErrorCount + " errors." + Environment.NewLine);
            }
        }

        void zVirtualSceneEvents_CommandRunCompleteEvent(int QueID, bool withErrors, string txtError)
        {
            if(withErrors)
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
                    int.TryParse(API.GetSetting("Port"), out port);

                    int max_conn = 50;
                    int.TryParse(API.GetSetting("Max Conn."), out max_conn);

                    isActive = true;
                    LightSwitchSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    LightSwitchSocket.Bind(new IPEndPoint(IPAddress.Any, port));
                    LightSwitchSocket.Listen(max_conn);
                    LightSwitchSocket.BeginAccept(new AsyncCallback(OnLightSwitchClientConnect), null);
                    API.WriteToLog(Urgency.INFO, "Started listening for LightSwitch clients on port " + port + ".");
                }
                catch (SocketException e)
                {
                    API.WriteToLog(Urgency.ERROR, "Socket Failed to Open - " + e);
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
                API.WriteToLog(Urgency.INFO, "Stopped listening for new clients.");
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

                API.WriteToLog(Urgency.INFO, "Connection Attempt from: " + LightSwitchClientsSocket.RemoteEndPoint.ToString());

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
                API.WriteToLog(Urgency.ERROR, "Socket Exception: " + e);
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
                API.WriteToLog(Urgency.ERROR, "Socket Exception: " + e);
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
                    bool.TryParse(API.GetSetting("Verbose Logging"), out verbose);

                    if (verbose)
                        API.WriteToLog(Urgency.INFO, "Received [" + LightSwitchClientSocket.RemoteEndPoint.ToString() + "] " + data);

                    string[] commands = data.Split('\n');

                    string version = "VER~" + API.GetProgramNameAndVersion;

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
                                    string hashedPassword = EncodePassword(Convert.ToString(m_cookie) + ":" + API.GetSetting("Password"));
                                    //hashedPassword = "638831F3AF6F32B25D6F1C961CBBA393"

                                    if (inputPassword.StartsWith(hashedPassword))
                                    {
                                        socketData.m_verified = true;
                                        SendMessagetoClientsSocket(LightSwitchClientSocket, version + Environment.NewLine);
                                        API.WriteToLog(Urgency.INFO, "[" + LightSwitchClientSocket.RemoteEndPoint.ToString() + "] User Authenticated.");
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
                                    API.WriteToLog(Urgency.INFO, "[" + LightSwitchClientSocket.RemoteEndPoint.ToString() + "] User requested device list.");

                                    sendDeviceList(LightSwitchClientSocket);

                                    BindingList<Scene> scenes = API.Scenes.GetScenes();
                                    foreach (Scene scene in scenes)
                                        SendMessagetoClientsSocket(LightSwitchClientSocket, "SCENE~" + scene.txt_name + "~" + scene.id + Environment.NewLine);
                              
                                    sendZoneList(LightSwitchClientSocket);

                                    SendMessagetoClientsSocket(LightSwitchClientSocket, "ENDLIST" + Environment.NewLine);
                                }
                                else if (cmd.StartsWith("LIST")) //DEVICES
                                {
                                    API.WriteToLog(Urgency.INFO, "[" + LightSwitchClientSocket.RemoteEndPoint.ToString() + "] User requested device list.");

                                    sendDeviceList(LightSwitchClientSocket);

                                    SendMessagetoClientsSocket(LightSwitchClientSocket, "ENDLIST" + Environment.NewLine);
                                }
                                else if (cmd.StartsWith("SLIST"))  //SCENES
                                {
                                    //TODO: OPTION FOR EACH SCENE... if (scene.ShowInLightSwitchGUI)
                                    BindingList<Scene> scenes = API.Scenes.GetScenes();                                    
                                    foreach(Scene scene in scenes)
                                        SendMessagetoClientsSocket(LightSwitchClientSocket, "SCENE~" + scene.txt_name + "~" + scene.id + Environment.NewLine);
                              
                                    SendMessagetoClientsSocket(LightSwitchClientSocket, "ENDLIST" + Environment.NewLine);
                                }
                                else if (cmd.StartsWith("ZLIST")) //ZONES
                                {
                                    API.WriteToLog(Urgency.INFO, "[" + LightSwitchClientSocket.RemoteEndPoint.ToString() + "] User requested zone/group list.");

                                    sendZoneList(LightSwitchClientSocket);
                                    SendMessagetoClientsSocket(LightSwitchClientSocket, "ENDLIST" + Environment.NewLine);
                                }
                                else if (cmd.StartsWith("DEVICE"))
                                {
                                    string[] values = cmd.Split('~');
                                    //NOTIFY ALL CLIENTS
                                    BroadcastMessage(TranslateToObjectAction(Convert.ToByte(values[1]), Convert.ToByte(values[2]), LightSwitchClientSocket) + Environment.NewLine);
                                }
                                else if (cmd.StartsWith("SCENE"))
                                {
                                    string[] values = cmd.Split('~');
                                    //NOTIFY ALL CLIENTS
                                    TranslateToSceneActions(Convert.ToByte(values[1]), LightSwitchClientSocket);
                                }
                                else if (cmd.StartsWith("ZONE"))
                                {
                                    string[] values = cmd.Split('~');

                                    int cmdId = 0;

                                    int groupId = 0;
                                    int.TryParse(values[1], out groupId);
                                    string groupName = API.Groups.GetGroupName(groupId);

                                    string action = ""; 
                                    switch(values[2])
                                    {
                                        case "255":
                                        {
                                            action = "activated";
                                            cmdId = API.Commands.GetBuiltinCommandId("GROUP_ON"); 
                                            break;
                                        }
                                        case "0":
                                        {
                                            action = "deactivated";
                                            cmdId = API.Commands.GetBuiltinCommandId("GROUP_OFF"); 
                                            break;
                                        }
                                    }

                                    API.Commands.InstallQueCommandAndProcess(new QuedCommand { cmdtype = cmdType.Builtin, Argument = groupName, CommandId = cmdId });

                                    API.WriteToLog(Urgency.INFO, "[" + LightSwitchClientSocket.RemoteEndPoint.ToString() + "] " + "All devices in group '" + groupName + "' " + action + ".");
                                    BroadcastMessage("MSG~" + "All devices in group '" + groupName + "' " + action + "." + Environment.NewLine);
                                }
                                else if (cmd.StartsWith("THERMMODE"))
                                {
                                    string[] values = cmd.Split('~');
                                    //NOTIFY ALL CLIENTS
                                    BroadcastMessage(TranslateToThermoAction(Convert.ToByte(values[1]), Convert.ToByte(values[2]), LightSwitchClientSocket) + Environment.NewLine);
                                }
                                else if (cmd.StartsWith("THERMTEMP"))
                                {
                                    string[] values = cmd.Split('~');
                                    //NOTIFY ALL CLIENTS
                                    BroadcastMessage(TranslateToThermoTEMPATUREAction(Convert.ToByte(values[1]), Convert.ToByte(values[2]), Convert.ToInt32(values[3]), LightSwitchClientSocket) + Environment.NewLine);
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
                API.WriteToLog(Urgency.ERROR, "OnDataReceived - Socket has been closed");
            }
            catch (SocketException se)
            {
                if (se.ErrorCode == 10054) // Error code for Connection reset by peer
                    API.WriteToLog(Urgency.ERROR, "Client " + socketData.m_clientNumber + " Disconnected.");
                else
                    API.WriteToLog(Urgency.ERROR, "SocketException - " + se.Message);

                DisconnectClientSocket(socketData);
            }
            catch (Exception e)
            {
                API.WriteToLog(Urgency.ERROR, "[" + LightSwitchClientSocket.RemoteEndPoint.ToString() + "] Server Exception: " + e);

                //SEND ERROR TO CLIENT
                SendMessagetoClientsSocket(LightSwitchClientSocket, "ERR~" + e.Message + Environment.NewLine);

                Thread.Sleep(3000);
                DisconnectClientSocket(socketData);
            }
        }

        private void sendZoneList(Socket LightSwitchClientSocket)
        {
            DataTable groups = zVirtualScenesAPI.API.Groups.GetGroups();

            foreach (DataRow dr in groups.Rows)
                SendMessagetoClientsSocket(LightSwitchClientSocket, "ZONE~" + dr["txt_group_name"].ToString() + "~" + dr["id"].ToString() + Environment.NewLine);
        }                       

        private void sendDeviceList(Socket LightSwitchClientSocket)
        {
             List<zwObject> ojbects = new List<zwObject>(); 
             List<String> strObjects = new List<string>();

             ojbects = zwObject.ConvertObjDataTabletoObjList(API.Object.GetObjects(true));

           foreach (zwObject ojb in ojbects)
           {
               bool show = true;
               bool.TryParse(API.Object.Properties.GetObjectPropertyValue(ojb.ID, "SHOWINLSLIST"), out show);

               if (show)
               {
                   string device = TranslateObjectToLightSwitchString(ojb);
                   if (device != null)
                       strObjects.Add(device);
               }
           }

           bool sort_list = true;
           bool.TryParse(API.GetSetting("Sort Device List"), out sort_list);

            if (sort_list)
                strObjects.Sort();

            foreach (string obj in strObjects)
                SendMessagetoClientsSocket(LightSwitchClientSocket, "DEVICE~" + obj + Environment.NewLine);
        }


        public string TranslateObjectToLightSwitchString(zwObject obj)
        {
            switch (obj.Type)
            {
                case "SWITCH":
                    return obj.Name + "~" + obj.Node_ID + "~" + (obj.On ? "255" : "0") + "~" + "BinarySwitch";
                case "DIMMER":
                    return obj.Name + "~" + obj.Node_ID + "~" + obj.Level + "~" + "MultiLevelSwitch";                 
                case "THERMOSTAT":
                    return obj.Name + "~" + obj.Node_ID + "~" + obj.Temperature + "~" + "Thermostat";   
                case "SENSOR":
                    return obj.Name + "~" + obj.Node_ID + "~" + obj.Level + "~" + "Sensor";                
            }
            return null;
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
        /// Set levels for objects when a Lightswitch action level string is recieved.
        /// </summary>
        /// <param name="Node"></param>
        /// <param name="Level"></param>
        /// <param name="Client"></param>
        /// <returns></returns>
        private string TranslateToObjectAction(byte Node, byte Level, Socket Client)
        {
            string result = "ERR~Error setting node " + Node.ToString() + ". Try Agian";
            DataTable dt = API.Object.GetObjectByNodeID(Node.ToString());

            if (dt != null && dt.Rows.Count > 0)
            {
                zwObject zwObj = (zwObject)dt.Rows[0];
                int commandId;

                switch (zwObj.Type)
                {
                    case "SWITCH":
                        string state = (Level == 0 ? "OFF" : "ON");
                        commandId = (Level == 0 ? API.Commands.GetObjectTypeCommandId(zwObj.Type_ID, "TURNOFF") : API.Commands.GetObjectTypeCommandId(zwObj.Type_ID, "TURNON"));
                        API.Commands.InstallQueCommandAndProcess(new QuedCommand { cmdtype = cmdType.ObjectType, CommandId = commandId, ObjectId = zwObj.ID });                   
                        
                        result = "MSG~Turned " + zwObj.Name + (Level == 0 ?  "Off" : "On") + "."; 
                        break; 
                    case "DIMMER":
                        if (Level == 255)
                            Level = 99;
                        commandId = API.Commands.GetObjectCommandId(zwObj.ID, "DYNAMIC_CMD_BASIC");
                        API.Commands.InstallQueCommandAndProcess(new QuedCommand { cmdtype = cmdType.Object, CommandId = commandId, ObjectId = zwObj.ID, Argument = Level.ToString() });

                        result = "MSG~Set " + zwObj.Name + " to " + Level.ToString() + "."; 
                        break;
                }
            }

           API.WriteToLog(Urgency.INFO,"[" + Client.RemoteEndPoint.ToString() + "] " + result);            
           return result;
        }

        /// <summary>
        /// Runs a zVirtualScene Scene
        /// </summary>
        /// <param name="SceneID">Scene ID</param>
        /// <param name="Client">Clients Socket.</param>
        private void TranslateToSceneActions(int SceneID, Socket Client)
        {
            Scene scene = API.Scenes.GetScene(SceneID);

            if (scene != null)
            {
                string result = scene.RunScene();
                API.WriteToLog(Urgency.INFO, "[" + Client.RemoteEndPoint.ToString() + "] " + result);
                BroadcastMessage("MSG~" + result + Environment.NewLine);
            }
        }

        private string TranslateToThermoTEMPATUREAction(byte Node, byte Mode, int Temp, Socket Client)
        {
            string result = "ERR~Error setting node " + Node.ToString() + ". Try Agian";
            DataTable dt = API.Object.GetObjectByNodeID(Node.ToString());

            if (dt != null && dt.Rows.Count > 0)
            {
                zwObject zwObj = (zwObject)dt.Rows[0];
                int commandId;

                if (zwObj.Type.Equals("THERMOSTAT"))
                {
                    switch (Mode)
                    {
                        case 2:
                            commandId = API.Commands.GetObjectCommandId(zwObj.ID, "DYNAMIC_CMD_HEATING 1");

                            API.Commands.InstallQueCommandAndProcess(new QuedCommand { cmdtype = cmdType.Object, CommandId = commandId, ObjectId = zwObj.ID, Argument = Temp.ToString() });
                            result = "MSG~Set " + zwObj.Name + " Heat Point to " +Temp.ToString()+".";
                            break;
                        case 3:
                            commandId = API.Commands.GetObjectCommandId(zwObj.ID, "DYNAMIC_CMD_COOLING 1");
                            API.Commands.InstallQueCommandAndProcess(new QuedCommand { cmdtype = cmdType.Object, CommandId = commandId, ObjectId = zwObj.ID, Argument = Temp.ToString() });
                            result = "MSG~Set " + zwObj.Name + " Cool Point to " +Temp.ToString()+".";
                            break;
                    }
                }
            }
            API.WriteToLog(Urgency.INFO, "[" + Client.RemoteEndPoint.ToString() + "] " + result);
            return result;
        }

        private string TranslateToThermoAction(byte Node, byte Mode, Socket Client)
        {
            string result = "ERR~Error setting node " + Node.ToString() + ". Try Agian";
            DataTable dt = API.Object.GetObjectByNodeID(Node.ToString());

            if (dt != null && dt.Rows.Count > 0)
            {
                zwObject zwObj = (zwObject)dt.Rows[0];
                int commandId;

                if (zwObj.Type.Equals("THERMOSTAT"))
                {
                    switch (Mode)
                    {
                        case 0:
                            commandId = API.Commands.GetObjectCommandId(zwObj.ID, "DYNAMIC_CMD_MODE");
                            API.Commands.InstallQueCommandAndProcess(new QuedCommand { cmdtype = cmdType.Object, CommandId = commandId, ObjectId = zwObj.ID, Argument = "Off" });                           
                            result = "MSG~Set " + zwObj.Name + " to Mode to Off.";
                            break;
                        case 1:
                            commandId = API.Commands.GetObjectCommandId(zwObj.ID, "DYNAMIC_CMD_MODE");
                            API.Commands.InstallQueCommandAndProcess(new QuedCommand { cmdtype = cmdType.Object, CommandId = commandId, ObjectId = zwObj.ID, Argument = "Auto" });
                            result = "MSG~Set " + zwObj.Name + " to Mode to Auto.";
                            break;
                        case 2:
                            commandId = API.Commands.GetObjectCommandId(zwObj.ID, "DYNAMIC_CMD_MODE");
                            API.Commands.InstallQueCommandAndProcess(new QuedCommand { cmdtype = cmdType.Object, CommandId = commandId, ObjectId = zwObj.ID, Argument = "Heat" });
                            result = "MSG~Set " + zwObj.Name + " to Mode to Heat.";
                            break;
                        case 3:
                            commandId = API.Commands.GetObjectCommandId(zwObj.ID, "DYNAMIC_CMD_MODE");
                            API.Commands.InstallQueCommandAndProcess(new QuedCommand { cmdtype = cmdType.Object, CommandId = commandId, ObjectId = zwObj.ID, Argument = "Cool" });
                            result = "MSG~Set " + zwObj.Name + " to Mode to Cool.";
                            break;
                        case 4:
                            commandId = API.Commands.GetObjectCommandId(zwObj.ID, "DYNAMIC_CMD_FAN MODE");
                            API.Commands.InstallQueCommandAndProcess(new QuedCommand { cmdtype = cmdType.Object, CommandId = commandId, ObjectId = zwObj.ID, Argument = "On Low" });
                            result = "MSG~Set " + zwObj.Name + " to Fan Mode to On-Low.";                           
                            break;
                        case 5:
                            commandId = API.Commands.GetObjectCommandId(zwObj.ID, "DYNAMIC_CMD_FAN MODE");
                            API.Commands.InstallQueCommandAndProcess(new QuedCommand { cmdtype = cmdType.Object, CommandId = commandId, ObjectId = zwObj.ID, Argument = "Auto Low" });
                            result = "MSG~Set " + zwObj.Name + " to Fan Mode to Auto-Low."; 
                            break;
                        case 6:
                            commandId = API.Commands.GetObjectTypeCommandId(zwObj.Type_ID, "SETENERGYMODE");
                            API.Commands.InstallQueCommandAndProcess(new QuedCommand { cmdtype = cmdType.ObjectType, CommandId = commandId, ObjectId = zwObj.ID });
                            result = "MSG~Set " + zwObj.Name + " to Energy Mode.";                             
                            break;
                        case 7:
                            commandId = API.Commands.GetObjectTypeCommandId(zwObj.Type_ID, "SETCONFORTMODE");
                            API.Commands.InstallQueCommandAndProcess(new QuedCommand { cmdtype = cmdType.ObjectType, CommandId = commandId, ObjectId = zwObj.ID });
                            result = "MSG~Set " + zwObj.Name + " to Confort Mode.";                             
                            break;
                    }                    
                }
            }
            API.WriteToLog(Urgency.INFO, "[" + Client.RemoteEndPoint.ToString() + "] " + result);
            return result;
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
                bool.TryParse(API.GetSetting("Verbose Logging"), out verbose);

                if (verbose)
                    API.WriteToLog(Urgency.INFO, "SENT TO ALL - " + msg);
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
                bool.TryParse(API.GetSetting("Verbose Logging"), out verbose);

                if (verbose)
                    API.WriteToLog(Urgency.INFO, "SENT - " + msg);
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
            bool.TryParse(API.GetSetting("LS_VERBOSE"), out verbose);

            if (verbose)
                API.WriteToLog(Urgency.INFO, "SENT " + msg);
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
                API.WriteToLog(Urgency.INFO, "Socket Disconnect: " + e);
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

