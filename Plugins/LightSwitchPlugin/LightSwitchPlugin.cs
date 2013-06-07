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
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.IO;
using System.Diagnostics;
using LightSwitchPlugin.LightSwitch;

namespace LightSwitchPlugin
{
    public static class AsyncExtensions
    {
        public static Task<TcpClient> AcceptTcpClientAsync(this TcpListener source, CancellationToken cancellationToken)
        {
            return Task<TcpClient>.Factory.FromAsync(new Func<AsyncCallback, object, IAsyncResult>(source.BeginAcceptTcpClient),
                                                     new Func<IAsyncResult, TcpClient>(source.EndAcceptTcpClient), cancellationToken);
        }
    }

    [Export(typeof(zvsPlugin))]
    public class LightSwitchPlugin : zvsPlugin
    {
        private HashSet<LightSwitchClient> LightSwitchClients = new HashSet<LightSwitchClient>();
        private CancellationTokenSource _cts = new CancellationTokenSource();
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

                string error = null;
                DeviceProperty.TryAddOrEdit(new DeviceProperty
                {
                    UniqueIdentifier = "SHOWINLSLIST",
                    Name = "Show device in LightSwitch",
                    Description = "If enabled this device will show in the LightSwitch device tab.",
                    ValueType = DataType.BOOL,
                    Value = "true"
                }, context, out error);

                SceneProperty.TryAddOrEdit(new SceneProperty
                {
                    UniqueIdentifier = "SHOWSCENEINLSLIST",
                    Name = "Show scene in LightSwitch",
                    Description = "If enabled this scene will show in the LightSwitch scene tab.",
                    Value = "true",
                    ValueType = DataType.BOOL
                }, context, out error);

                if (!string.IsNullOrEmpty(error))
                    log.Error(error);

                bool.TryParse(GetSettingValue("VERBOSE", context), out _verbose);
                bool.TryParse(GetSettingValue("PUBLISHZEROCFG", context), out _useBonjour);
                bool.TryParse(GetSettingValue("SORTLIST", context), out _sort_list);
                int.TryParse(GetSettingValue("PORT", context), out _port);
                int.TryParse(GetSettingValue("MAXCONN", context), out _max_conn);
            }
        }

        protected override void StartPlugin()
        {
            DeviceValue.DeviceValueDataChangedEvent += DeviceValue_DeviceValueDataChangedEvent;
            StartLightSwitchServer();
            publishZeroConf();
        }

        protected async override void StopPlugin()
        {
            DeviceValue.DeviceValueDataChangedEvent -= DeviceValue_DeviceValueDataChangedEvent;
            await StopLightSwitchServer();
        }

        protected async override void SettingChanged(string UniqueIdentifier, string settingValue)
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
                    await StopLightSwitchServer();

                int.TryParse(settingValue, out _port);

                if (this.Enabled)
                    StartLightSwitchServer();

            }
            else if (UniqueIdentifier == "MAXCONN")
            {
                if (this.Enabled)
                    await StopLightSwitchServer();

                int.TryParse(settingValue, out _max_conn);

                if (this.Enabled)
                    StartLightSwitchServer();
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

        public override void ProcessCommand(int queuedCommandId) { }

        public override void Repoll(zvs.Entities.Device device) { }

        public override void ActivateGroup(int groupID) { }

        public override void DeactivateGroup(int groupID) { }

        private async void DeviceValue_DeviceValueDataChangedEvent(object sender, DeviceValue.ValueDataChangedEventArgs args)
        {
            await Task.Run(async () =>
            {
                using (zvsContext context = new zvsContext())
                {
                    DeviceValue dv = context.DeviceValues.FirstOrDefault(v => v.Id == args.DeviceValueId);
                    if (dv != null)
                    {
                        if (dv.Name == "Basic")
                        {
                            if (!ZVSTypeToLSType.ContainsKey(dv.Device.Type.UniqueIdentifier))
                                return;

                            string level = ((int)dv.Device.CurrentLevelInt).ToString();
                            var type = ZVSTypeToLSType[dv.Device.Type.UniqueIdentifier];

                            if (dv.Device.Type.UniqueIdentifier == "SWITCH")
                                level = (dv.Device.CurrentLevelInt > 0 ? "255" : "0");

                            await BroadcastCommand(LightSwitchProtocol.CreateUpdateCmd(dv.Device.Name, dv.Device.Id.ToString(), level, type));
                            await BroadcastCommand(LightSwitchProtocol.CreateEndListCmd());
                            await BroadcastCommand(LightSwitchProtocol.CreateMsgCmdFormat("'{0}' {1} changed to {2}", dv.Device.Name, dv.Name, args.newValue));
                        }
                    }
                }
            });

        }

        public void StartLightSwitchServer()
        {
            _cts = new CancellationTokenSource();
            var listener = new TcpListener(IPAddress.Any, _port);
            listener.Server.NoDelay = true;
            listener.Server.LingerState = new LingerOption(true, 2);
            listener.Start();
            log.Info("LightSwitch server started on port " + _port);

            while (true)
            {
                var task = listener.AcceptTcpClientAsync();
                try
                {
                    task.Wait(_cts.Token);
                }
                catch (OperationCanceledException e)
                {
                    if (e.CancellationToken == _cts.Token)
                        break;
                }

                var client = task.Result;
                var lightSwitchClient = new LightSwitchClient(client);
                LightSwitchClients.Add(lightSwitchClient);

                lightSwitchClient.onConnectionEstabilished += lightSwitchClient_ConnectionEstabilished;
                lightSwitchClient.onConnectionClosed += lightSwitchClient_ConnectionClosed;

                lightSwitchClient.onDataReceived += lightSwitchClient_DataReceived;
                lightSwitchClient.onDataSent += lightSwitchClient_DataSent;

                lightSwitchClient.onCmdAList += lightSwitchClient_onCmdAList;
                lightSwitchClient.onCmdDevice += lightSwitchClient_onCmdDevice;
                lightSwitchClient.onCmdIphone += lightSwitchClient_onCmdIphone;
                lightSwitchClient.onCmdList += lightSwitchClient_onCmdList;
                lightSwitchClient.onCmdPassword += lightSwitchClient_onCmdPassword;
                lightSwitchClient.onCmdScene += lightSwitchClient_onCmdScene;
                lightSwitchClient.onCmdServer += lightSwitchClient_onCmdServer;
                lightSwitchClient.onCmdSList += lightSwitchClient_onCmdSList;
                lightSwitchClient.onCmdTerminate += lightSwitchClient_onCmdTerminate;
                lightSwitchClient.onCmdThermMode += lightSwitchClient_onCmdThermMode;
                lightSwitchClient.onCmdThermTemp += lightSwitchClient_onCmdThermTemp;
                lightSwitchClient.onCmdVersion += lightSwitchClient_onCmdVersion;
                lightSwitchClient.onCmdZList += lightSwitchClient_onCmdZList;
                lightSwitchClient.onCmdZone += lightSwitchClient_onCmdZone;
               
                lightSwitchClient.StartMonitoring();
            }
            listener.Stop();
            log.Info("LightSwitch server stopped");
        }

        void lightSwitchClient_DataSent(object sender, LightSwitchDataEventArgs args)
        {
            if (_verbose && !string.IsNullOrEmpty(args.RawData))
                log.InfoFormat("Sent to [{0}]: {1}", args.LightSwitchClient.RemoteEndPoint, args.RawData.Trim());
        }

        #region LightSwitch Client Events

        void lightSwitchClient_DataReceived(object sender, LightSwitchDataEventArgs args)
        {
            if (_verbose && !string.IsNullOrEmpty(args.RawData))
                log.InfoFormat("Received from [{0}]: {1}", args.LightSwitchClient.RemoteEndPoint, args.RawData.Trim());
        }

        async void lightSwitchClient_onCmdIphone(object sender, LightSwitchClientEventArgs args)
        {
            args.LightSwitchClient.isAuthenticated = false;
            await args.LightSwitchClient.SendCommandAsync(LightSwitchProtocol.CreateCookieCmd(args.LightSwitchClient.Nonce.ToString()));
        }
        async void lightSwitchClient_onCmdPassword(object sender, onPasswordEventArgs args)
        {
            using (zvsContext context = new zvsContext())
            {
                var lightSwitchClient = args.LightSwitchClient;
                string hashedPassword = EncodePassword(string.Format("{0}:{1}", lightSwitchClient.Nonce, GetSettingValue("PASSWORD", context)));

                if (args.Password.StartsWith(hashedPassword))
                {
                    lightSwitchClient.isAuthenticated = true;
                    await lightSwitchClient.SendCommandAsync(LightSwitchProtocol.CreateVersionCmd(Utils.ApplicationNameAndVersion));

                    log.InfoFormat("Received [{0}] User Authenticated", lightSwitchClient.RemoteEndPoint);
                }
                else
                {
                    lightSwitchClient.isAuthenticated = false;
                    lightSwitchClient.Disconnect();
                }
            }
        }

        async void lightSwitchClient_onCmdAList(object sender, LightSwitchClientEventArgs args)
        {
            if (!args.LightSwitchClient.isAuthenticated)
                args.LightSwitchClient.Disconnect();

            log.InfoFormat("Received [{0}] User requested device, scene and group list", args.LightSwitchClient.RemoteEndPoint);

            await SendDeviceListAsync(args.LightSwitchClient);
            await SendSceneListAsync(args.LightSwitchClient);
            await SendZoneListAsync(args.LightSwitchClient);

            await args.LightSwitchClient.SendCommandAsync(LightSwitchProtocol.CreateEndListCmd());
        }
        async void lightSwitchClient_onCmdZList(object sender, LightSwitchClientEventArgs args)
        {
            log.InfoFormat("Received [{0}] User requested zone list", args.LightSwitchClient.RemoteEndPoint);
            await SendZoneListAsync(args.LightSwitchClient);
            await args.LightSwitchClient.SendCommandAsync(LightSwitchProtocol.CreateEndListCmd());
        }
        async void lightSwitchClient_onCmdSList(object sender, LightSwitchClientEventArgs args)
        {
            if (!args.LightSwitchClient.isAuthenticated)
                args.LightSwitchClient.Disconnect();

            log.InfoFormat("Received [{0}] User requested scene list", args.LightSwitchClient.RemoteEndPoint);
            await SendSceneListAsync(args.LightSwitchClient);
            await args.LightSwitchClient.SendCommandAsync(LightSwitchProtocol.CreateEndListCmd());
        }
        async void lightSwitchClient_onCmdList(object sender, LightSwitchClientEventArgs args)
        {
            if (!args.LightSwitchClient.isAuthenticated)
                args.LightSwitchClient.Disconnect();

            log.InfoFormat("Received [{0}] User requested device list", args.LightSwitchClient.RemoteEndPoint);
            await SendDeviceListAsync(args.LightSwitchClient);
            await args.LightSwitchClient.SendCommandAsync(LightSwitchProtocol.CreateEndListCmd());
        }

        async void lightSwitchClient_onCmdVersion(object sender, LightSwitchClientEventArgs args)
        {
            if (!args.LightSwitchClient.isAuthenticated)
                args.LightSwitchClient.Disconnect();

            await args.LightSwitchClient.SendCommandAsync(LightSwitchProtocol.CreateVersionCmd(Utils.ApplicationNameAndVersion));
        }
        void lightSwitchClient_onCmdTerminate(object sender, LightSwitchClientEventArgs args)
        {
            args.LightSwitchClient.Disconnect();
        }
        async void lightSwitchClient_onCmdServer(object sender, LightSwitchClientEventArgs args)
        {
            if (!args.LightSwitchClient.isAuthenticated)
                args.LightSwitchClient.Disconnect();

            await args.LightSwitchClient.SendCommandAsync(LightSwitchProtocol.CreateVersionCmd(Utils.ApplicationNameAndVersion));
        }

        async void lightSwitchClient_onCmdDevice(object sender, onDeviceEventArgs args)
        {
            if (!args.LightSwitchClient.isAuthenticated)
                args.LightSwitchClient.Disconnect();

            int deviceId = int.TryParse(args.DeviceId, out deviceId) ? deviceId : 0;
            byte level = byte.TryParse(args.Level, out level) ? level : (byte)0;

            using (zvsContext context = new zvsContext())
            {
                Device d = context.Devices.FirstOrDefault(o => o.Id == deviceId);
                if (d == null)
                {
                    var error = "Cannot locate device";
                    await args.LightSwitchClient.SendCommandAsync(LightSwitchProtocol.CreateErrorMsgCmd(error));
                    log.Error(error);

                    return;
                }

                string arg1 = null;
                string arg2 = null;
                int commandId = 0;
                string cmdMsg = string.Empty;

                #region Find Command

                switch (d.Type.UniqueIdentifier)
                {
                    case "SWITCH":
                        {
                            string cmdUniqueId = (level == 0 ? "TURNOFF" : "TURNON");

                            DeviceTypeCommand dtcmd = d.Type.Commands.FirstOrDefault(c => c.UniqueIdentifier == cmdUniqueId);
                            if (dtcmd == null)
                            {
                                var error = "Cannot locate zvs command";
                                await args.LightSwitchClient.SendCommandAsync(LightSwitchProtocol.CreateErrorMsgCmd(error));
                                log.Error(error);

                                return;
                            }

                            cmdMsg = string.Format("[{0}] Executed command '{1}' on '{2}'.", args.LightSwitchClient.RemoteEndPoint, dtcmd.Name, d.Name);
                            commandId = dtcmd.Id;
                            arg1 = null;
                            arg2 = d.Id.ToString();
                            break;
                        }
                    case "DIMMER":
                        {
                            string l = (level == 255 ? "99" : level.ToString());
                            if (!PluginToDimmerBasicCommand.ContainsKey(d.Type.Plugin.UniqueIdentifier))
                            {
                                var error = "No command defines for this plug-in";
                                await args.LightSwitchClient.SendCommandAsync(LightSwitchProtocol.CreateErrorMsgCmd(error));
                                log.Error(error);

                                return;
                            }

                            var cmdUniqueId = PluginToDimmerBasicCommand[d.Type.Plugin.UniqueIdentifier];
                            DeviceCommand dcmd = d.Commands.FirstOrDefault(c => c.UniqueIdentifier.Contains(cmdUniqueId));
                            if (dcmd == null)
                            {
                                var error = "Cannot locate zvs command";
                                await args.LightSwitchClient.SendCommandAsync(LightSwitchProtocol.CreateErrorMsgCmd(error));
                                log.Error(error);

                                return;
                            }

                            cmdMsg = string.Format("[{0}] Executed command '{1}' on '{2}'.", args.LightSwitchClient.RemoteEndPoint, dcmd.Name, d.Name);
                            commandId = dcmd.Id;
                            arg1 = l;
                            arg2 = d.Id.ToString();

                            break;
                        }
                }
                #endregion

                if (commandId == 0)
                {
                    var error = "Cannot locate zvs command";
                    await args.LightSwitchClient.SendCommandAsync(LightSwitchProtocol.CreateErrorMsgCmd(error));
                    log.Error(error);

                    return;
                }

                log.Info(cmdMsg);
                CommandProcessor cp = new CommandProcessor(Core);
                var commandResult = await cp.RunCommandAsync(this, commandId, arg1, arg2);

                if (commandResult.HasErrors)
                    await args.LightSwitchClient.SendCommandAsync(LightSwitchProtocol.CreateErrorMsgCmd(commandResult.Details));
                else
                    await BroadcastCommand(LightSwitchProtocol.CreateMsgCmd(cmdMsg));
            }
        }
        async void lightSwitchClient_onCmdZone(object sender, onZoneEventArgs args)
        {
            int groupId = int.TryParse(args.ZoneId, out groupId) ? groupId : 0;
            string cmdUniqId = (args.Level.Equals("255") ? "GROUP_ON" : "GROUP_OFF");

            using (zvsContext context = new zvsContext())
            {
                Group g = context.Groups.FirstOrDefault(o => o.Id == groupId);
                if (g != null)
                {
                    BuiltinCommand zvs_cmd = context.BuiltinCommands.FirstOrDefault(c => c.UniqueIdentifier == cmdUniqId);
                    if (zvs_cmd == null)
                    {
                        var error = "Cannot locate zvs command";
                        await args.LightSwitchClient.SendCommandAsync(LightSwitchProtocol.CreateErrorMsgCmd(error));
                        log.Error(error);

                        return;
                    }
                    string result = string.Format("[{0}] Ran {1} on group '{2}'", args.LightSwitchClient.RemoteEndPoint, zvs_cmd.Name, g.Name);
                    log.Info(result);

                    CommandProcessor cp = new CommandProcessor(Core);
                    var r = await cp.RunCommandAsync(this, zvs_cmd.Id, g.Id.ToString());
                    if (r.HasErrors)
                        await args.LightSwitchClient.SendCommandAsync(LightSwitchProtocol.CreateErrorMsgCmd(r.Details));
                    else
                        await BroadcastCommand(LightSwitchProtocol.CreateMsgCmd(result));

                }
            }
        }
        async void lightSwitchClient_onCmdScene(object sender, onSceneEventArgs args)
        {
            if (!args.LightSwitchClient.isAuthenticated)
                args.LightSwitchClient.Disconnect();

            int sceneId = int.TryParse(args.SceneId, out sceneId) ? sceneId : 0;
            using (zvsContext context = new zvsContext())
            {
                BuiltinCommand bcmd = context.BuiltinCommands.FirstOrDefault(c => c.UniqueIdentifier == "RUN_SCENE");
                if (bcmd == null)
                {
                    var error = "Cannot locate zvs command";
                    await args.LightSwitchClient.SendCommandAsync(LightSwitchProtocol.CreateErrorMsgCmd(error));
                    log.Error(error);

                    return;
                }

                CommandProcessor cp = new CommandProcessor(Core);
                var r = await cp.RunCommandAsync(this, bcmd.Id, sceneId.ToString());

                if (r.HasErrors)
                    await args.LightSwitchClient.SendCommandAsync(LightSwitchProtocol.CreateErrorMsgCmd(r.Details));
                else
                    await BroadcastCommand(LightSwitchProtocol.CreateMsgCmd(r.Details));
            }
        }

        async void lightSwitchClient_onCmdThermTemp(object sender, onThermTempEventArgs args)
        {
            if (!args.LightSwitchClient.isAuthenticated)
                args.LightSwitchClient.Disconnect();

            int deviceId = int.TryParse(args.DeviceId, out deviceId) ? deviceId : 0;

            using (zvsContext context = new zvsContext())
            {
                Device d = context.Devices.FirstOrDefault(o => o.Id == deviceId);
                if (d == null)
                {
                    var error = "Cannot locate thermostat";
                    await args.LightSwitchClient.SendCommandAsync(LightSwitchProtocol.CreateErrorMsgCmd(error));
                    log.Error(error);

                    return;
                }

                string plugin = d.Type.Plugin.UniqueIdentifier;
                string key = plugin + args.Mode;

                if (!ThermoTempCommandTranslations.ContainsKey(key))
                {
                    var error = "No command defined for this plug-in";
                    await args.LightSwitchClient.SendCommandAsync(LightSwitchProtocol.CreateErrorMsgCmd(error));
                    log.Error(error);

                    return;
                }

                DeviceCommand dcmd = d.Commands.FirstOrDefault(c => c.UniqueIdentifier.StartsWith(ThermoTempCommandTranslations[key]));
                if (dcmd == null)
                {
                    var error = "Cannot locate zvs command";
                    await args.LightSwitchClient.SendCommandAsync(LightSwitchProtocol.CreateErrorMsgCmd(error));
                    log.Error(error);

                    return;
                }

                var cmdMsg = string.Format("[{0}] Executed command '{1}' on '{2}'.", args.LightSwitchClient.RemoteEndPoint, dcmd.Name, d.Name);
                log.Info(cmdMsg);
                CommandProcessor cp = new CommandProcessor(Core);
                var commandResult = await cp.RunCommandAsync(this, dcmd.Id, args.Temp);

                if (commandResult.HasErrors)
                    await args.LightSwitchClient.SendCommandAsync(LightSwitchProtocol.CreateErrorMsgCmd(commandResult.Details));
                else
                    await BroadcastCommand(LightSwitchProtocol.CreateMsgCmd(cmdMsg));
            }
        }
        async void lightSwitchClient_onCmdThermMode(object sender, onThermModeEventArgs args)
        {
            if (!args.LightSwitchClient.isAuthenticated)
                args.LightSwitchClient.Disconnect();

            int deviceId = int.TryParse(args.DeviceId, out deviceId) ? deviceId : 0;
            int mode = int.TryParse(args.Mode, out mode) ? mode : 0;

            string arg1 = null;
            string arg2 = null;
            int commandId = 0;
            string cmdMsg = string.Empty;

            //PLUGINNAME-0 -->CmdName,Arg 
            using (zvsContext context = new zvsContext())
            {
                Device d = context.Devices.FirstOrDefault(o => o.Id == deviceId);
                if (d == null)
                {
                    var error = "Cannot locate thermostat";
                    await args.LightSwitchClient.SendCommandAsync(LightSwitchProtocol.CreateErrorMsgCmd(error));
                    log.Error(error);

                    return;
                }

                if (mode < 6)
                {
                    string plugin = d.Type.Plugin.UniqueIdentifier;
                    string key = plugin + args.Mode;

                    if (!ThermoCommandTranslations.ContainsKey(key))
                    {
                        var error = "No command defined for this plug-in";
                        await args.LightSwitchClient.SendCommandAsync(LightSwitchProtocol.CreateErrorMsgCmd(error));
                        log.Error(error);

                        return;
                    }

                    zvsCMD cmd = ThermoCommandTranslations[key];
                    DeviceCommand dcmd = d.Commands.FirstOrDefault(c => c.UniqueIdentifier.Contains(cmd.CmdName));
                    if (dcmd == null)
                    {
                        var error = "Cannot locate zvs command";
                        await args.LightSwitchClient.SendCommandAsync(LightSwitchProtocol.CreateErrorMsgCmd(error));
                        log.Error(error);

                        return;
                    }

                    commandId = dcmd.Id;
                    arg1 = cmd.arg;
                    cmdMsg = string.Format("[{0}] Executed command '{1}' on '{2}'.", args.LightSwitchClient.RemoteEndPoint, dcmd.Name, d.Name);

                }
                else if (mode == 6)
                {
                    DeviceTypeCommand dcmd = d.Type.Commands.FirstOrDefault(c => c.UniqueIdentifier == "SETENERGYMODE");
                    if (dcmd == null)
                    {
                        var error = "Cannot locate zvs command";
                        await args.LightSwitchClient.SendCommandAsync(LightSwitchProtocol.CreateErrorMsgCmd(error));
                        log.Error(error);

                        return;
                    }

                    commandId = dcmd.Id;
                    arg1 = null;
                    arg2 = d.Id.ToString();
                    cmdMsg = string.Format("[{0}] Executed command '{1}' on '{2}'.", args.LightSwitchClient.RemoteEndPoint, dcmd.Name, d.Name);
                }
                else if (mode == 7)
                {
                    DeviceTypeCommand dcmd = d.Type.Commands.FirstOrDefault(c => c.UniqueIdentifier == "SETCONFORTMODE");
                    if (dcmd == null)
                    {
                        var error = "Cannot locate zvs command";
                        await args.LightSwitchClient.SendCommandAsync(LightSwitchProtocol.CreateErrorMsgCmd(error));
                        log.Error(error);

                        return;
                    }

                    commandId = dcmd.Id;
                    arg1 = null;
                    arg2 = d.Id.ToString();
                    cmdMsg = string.Format("[{0}] Executed command '{1}' on '{2}'.", args.LightSwitchClient.RemoteEndPoint, dcmd.Name, d.Name);
                }
            }

            if (commandId == 0)
            {
                var error = "Cannot locate zvs command";
                await args.LightSwitchClient.SendCommandAsync(LightSwitchProtocol.CreateErrorMsgCmd(error));
                log.Error(error);

                return;
            }

            log.Info(cmdMsg);
            CommandProcessor cp = new CommandProcessor(Core);
            var commandResult = await cp.RunCommandAsync(this, commandId, arg1, arg2);

            if (commandResult.HasErrors)
                await args.LightSwitchClient.SendCommandAsync(LightSwitchProtocol.CreateErrorMsgCmd(commandResult.Details));
            else
                await BroadcastCommand(LightSwitchProtocol.CreateMsgCmd(cmdMsg));
        }
        #endregion

        private Dictionary<string, string> PluginToDimmerBasicCommand = new Dictionary<string, string>()
        {
            {"OPENZWAVE", "DYNAMIC_CMD_BASIC"},
            {"THINKSTICK", "BASIC"} 
        };

        private Dictionary<string, string> ThermoTempCommandTranslations = new Dictionary<string, string>()
        {
            {"THINKSTICK2", "DYNAMIC_SP_R207_Heating1"},
            {"OPENZWAVE2", "DYNAMIC_CMD_HEATING 1_"},
            {"THINKSTICK3", "DYNAMIC_SP_R207_Cooling1"},
            {"OPENZWAVE3", "DYNAMIC_CMD_COOLING 1_"} 
        };

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

        public async Task StopLightSwitchServer()
        {
            await BroadcastCommand(LightSwitchProtocol.CreateMsgCmd("Server shutting down..."));

            foreach (var client in LightSwitchClients)
                client.Disconnect();

            _cts.Cancel();
        }

        void lightSwitchClient_ConnectionClosed(object sender, LightSwitchClientEventArgs args)
        {
            log.InfoFormat("Client {0} disconnected", args.LightSwitchClient.RemoteEndPoint);

            if (LightSwitchClients.Contains(args.LightSwitchClient))
                LightSwitchClients.Remove(args.LightSwitchClient);

            args.LightSwitchClient.onConnectionEstabilished -= lightSwitchClient_ConnectionEstabilished;
            args.LightSwitchClient.onConnectionClosed -= lightSwitchClient_ConnectionClosed;

            args.LightSwitchClient.onDataReceived -= lightSwitchClient_DataReceived;
            args.LightSwitchClient.onDataSent -= lightSwitchClient_DataSent;

            args.LightSwitchClient.onCmdAList -= lightSwitchClient_onCmdAList;
            args.LightSwitchClient.onCmdDevice -= lightSwitchClient_onCmdDevice;
            args.LightSwitchClient.onCmdIphone -= lightSwitchClient_onCmdIphone;
            args.LightSwitchClient.onCmdList -= lightSwitchClient_onCmdList;
            args.LightSwitchClient.onCmdPassword -= lightSwitchClient_onCmdPassword;
            args.LightSwitchClient.onCmdScene -= lightSwitchClient_onCmdScene;
            args.LightSwitchClient.onCmdServer -= lightSwitchClient_onCmdServer;
            args.LightSwitchClient.onCmdSList -= lightSwitchClient_onCmdSList;
            args.LightSwitchClient.onCmdTerminate -= lightSwitchClient_onCmdTerminate;
            args.LightSwitchClient.onCmdThermMode -= lightSwitchClient_onCmdThermMode;
            args.LightSwitchClient.onCmdThermTemp -= lightSwitchClient_onCmdThermTemp;
            args.LightSwitchClient.onCmdVersion -= lightSwitchClient_onCmdVersion;
            args.LightSwitchClient.onCmdZList -= lightSwitchClient_onCmdZList;
            args.LightSwitchClient.onCmdZone -= lightSwitchClient_onCmdZone;
        }

        async void lightSwitchClient_ConnectionEstabilished(object sender, LightSwitchClientEventArgs args)
        {
            if (LightSwitchClients.Count > _max_conn)
            {
                await args.LightSwitchClient.SendCommandAsync(LightSwitchProtocol.CreateMsgCmd("Max clients reached!"));
                args.LightSwitchClient.Disconnect();
            }

            log.InfoFormat("Client {0} connected", args.LightSwitchClient.RemoteEndPoint);
            await args.LightSwitchClient.SendCommandAsync(LightSwitchProtocol.CreateInfoCmdFormat("LightSwitch zVirtualScenes Plug-in (Active Connections {0}){1}", LightSwitchClients.Count, Environment.NewLine));
        }

        /// <summary>
        /// Sends a message to ALL connected clients.
        /// </summary>
        /// <param name="command">the message to send</param>
        public async Task BroadcastCommand(LightSwitchCommand command)
        {
            foreach (var client in LightSwitchClients)
                await client.SendCommandAsync(command);
        }

        #region Lists
        private async Task SendSceneListAsync(LightSwitchClient client)
        {
            using (zvsContext context = new zvsContext())
            {
                foreach (Scene scene in context.Scenes.OrderBy(o => o.SortOrder))
                {
                    bool show = false;
                    bool.TryParse(ScenePropertyValue.GetPropertyValue(context, scene, "SHOWSCENEINLSLIST"), out show);
                    if (!show)
                        continue;

                    await client.SendCommandAsync(LightSwitchProtocol.CreateSceneCmd(scene.Name, scene.Id.ToString()));
                }
            }
        }

        private async Task SendZoneListAsync(LightSwitchClient client)
        {
            using (zvsContext context = new zvsContext())
                foreach (Group g in context.Groups.OrderBy(o => o.Name))
                    await client.SendCommandAsync(LightSwitchProtocol.CreateZoneCmd(g.Name, g.Id.ToString()));
        }

        private Dictionary<string, LightSwitchProtocol.DeviceTypes> ZVSTypeToLSType = new Dictionary<string, LightSwitchProtocol.DeviceTypes>()
        {
            {"SWITCH", LightSwitchProtocol.DeviceTypes.BinarySwitch},
            {"DIMMER", LightSwitchProtocol.DeviceTypes.MultiLevelSwitch} ,
            {"THERMOSTAT", LightSwitchProtocol.DeviceTypes.Thermostat},
            {"SENSOR", LightSwitchProtocol.DeviceTypes.Sensor},
        };

        private async Task SendDeviceListAsync(LightSwitchClient client)
        {
            using (zvsContext context = new zvsContext())
            {
                //Get Devices
                foreach (Device device in context.Devices.OrderBy(o => o.Name).Where(o => o.Type.UniqueIdentifier != "CONTROLLER"))
                {
                    bool show = true;
                    bool.TryParse(DevicePropertyValue.GetPropertyValue(context, device, "SHOWINLSLIST"), out show);
                    if (!show)
                        continue;

                    if (!ZVSTypeToLSType.ContainsKey(device.Type.UniqueIdentifier))
                        continue;

                    string level = ((int)device.CurrentLevelInt).ToString();
                    var type = ZVSTypeToLSType[device.Type.UniqueIdentifier];

                    if (device.Type.UniqueIdentifier == "SWITCH")
                        level = (device.CurrentLevelInt > 0 ? "255" : "0");

                    await client.SendCommandAsync(LightSwitchProtocol.CreateDeviceCmd(device.Name, device.Id.ToString(), level, type));

                }
            }
        }

        #endregion

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
}

