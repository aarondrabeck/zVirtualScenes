using System.ComponentModel.Composition;
using System.Net.Sockets;
using System.Collections.Generic;
using System;
using System.Net;
using System.Threading;
using System.Text;
using System.Security.Cryptography;
using System.ComponentModel;
using System.Linq;
using Mono.Zeroconf;
using zvs.Processor;
using zvs.DataModel;
using System.Threading.Tasks;
using LightSwitchPlugin.LightSwitch;
using System.Data.Entity;
using zvs;

namespace LightSwitchPlugin
{
    public static class AsyncExtensions
    {
        public static Task<TcpClient> AcceptTcpClientAsync(this TcpListener source, CancellationToken cancellationToken)
        {
            return Task<TcpClient>.Factory.FromAsync(source.BeginAcceptTcpClient, source.EndAcceptTcpClient, cancellationToken);
        }
    }

    [Export(typeof(ZvsPlugin))]
    public class LightSwitchPlugin : ZvsPlugin
    {
        public override Guid PluginGuid
        {
            get { return Guid.Parse("47f8325d-46cf-4240-a5b2-8c2fe5dc9920"); }
        }

        public override string Name
        {
            get { return "LightSwitch Plug-in for ZVS"; }
        }

        public override string Description
        {
            get { return "This plug-in is a server that allows LightSwitch clients to connect and control zVirtualScene devices."; }
        }

        #region Settings

        private bool _verboseSetting;
        public bool VerboseSetting
        {
            get { return _verboseSetting; }
            set
            {
                if (value == _verboseSetting) return;
                _verboseSetting = value;
                NotifyPropertyChanged();
            }
        }

        private bool _useBonjourSetting;
        public bool UseBonjourSetting
        {
            get { return _useBonjourSetting; }
            set
            {
                if (value == _useBonjourSetting) return;
                _useBonjourSetting = value;
                NotifyPropertyChanged();
            }
        }

        private bool _sortListSetting = true;
        public bool SortListSetting
        {
            get { return _sortListSetting; }
            set
            {
                if (value == _sortListSetting) return;
                _sortListSetting = value;
                NotifyPropertyChanged();
            }
        }

        private int _portSetting = 9909;
        public int PortSetting
        {
            get { return _portSetting; }
            set
            {
                if (value == _portSetting) return;
                _portSetting = value;
                NotifyPropertyChanged();
            }
        }

        private int _maxConnectionsSettings = 50;
        public int MaxConnectionsSettings
        {
            get { return _maxConnectionsSettings; }
            set
            {
                if (value == _maxConnectionsSettings) return;
                _maxConnectionsSettings = value;
                NotifyPropertyChanged();
            }
        }

        private string _passwordSetting = "";
        public string PasswordSetting
        {
            get { return _passwordSetting; }
            set
            {
                if (value == _passwordSetting) return;
                _passwordSetting = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        private readonly HashSet<LightSwitchClient> _lightSwitchClients = new HashSet<LightSwitchClient>();
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private Mono.Zeroconf.Providers.Bonjour.RegisterService _netservice;
        public enum DeviceSettingUids
        {
            ShowInLightswitch
        }

        public override async Task OnDeviceSettingsCreating(DeviceSettingBuilder settingBuilder)
        {
            await settingBuilder.RegisterAsync(new DeviceSetting
                {
                    UniqueIdentifier = DeviceSettingUids.ShowInLightswitch.ToString(),
                    Name = "Show device in LightSwitch",
                    Description = "If enabled this device will show in the LightSwitch device tab.",
                    ValueType = DataType.BOOL,
                    Value = true.ToString()
                }, CancellationToken);
        }

        public enum SceneSettingUids
        {
            ShowInLightswitch
        }

        public override async Task OnSceneSettingsCreating(SceneSettingBuilder settingBuilder)
        {
            await settingBuilder.RegisterAsync(new SceneSetting
                {
                    UniqueIdentifier = SceneSettingUids.ShowInLightswitch.ToString(),
                    Name = "Show scene in LightSwitch",
                    Description = "If enabled this scene will show in the LightSwitch scene tab.",
                    Value = true.ToString(),
                    ValueType = DataType.BOOL
                }, CancellationToken);
        }

        public override async Task OnSettingsCreating(PluginSettingBuilder settingBuilder)
        {
            var portSetting = new PluginSetting
                {
                    Name = "Port",
                    Value = (1337).ToString(),
                    ValueType = DataType.INTEGER,
                    Description = "LightSwitch will listen for connections on this port."
                };
            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(portSetting, o => o.PortSetting);

            var maxConnSetting = new PluginSetting
                {
                    Name = "Max Conn.",
                    Value = (200).ToString(),
                    ValueType = DataType.INTEGER,
                    Description = "The maximum number of connections allowed."
                };
            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(maxConnSetting, o => o.MaxConnectionsSettings);

            var verboseSettings = new PluginSetting
                 {
                     Name = "Verbose Logging",
                     Value = false.ToString(),
                     ValueType = DataType.BOOL,
                     Description = "(Writes all server client communication to the log for debugging.)"
                 };
            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(verboseSettings, o => o.VerboseSetting);

            var passwordSetting = new PluginSetting
                 {
                     Name = "Password",
                     Value = "ChaNgeMe444",
                     ValueType = DataType.STRING,
                     Description = "The password clients must use to connect to the LightSwitch server. "
                 };
            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(passwordSetting, o => o.PasswordSetting);

            var sortSetting = new PluginSetting
                {
                    Name = "Sort Device List",
                    Value = true.ToString(),
                    ValueType = DataType.BOOL,
                    Description = "(Alphabetically sorts the device list.)"
                };
            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(sortSetting, o => o.SortListSetting);

            var bonjourSetting = new PluginSetting
             {
                 Name = "Publish ZeroConf/Bonjour",
                 Value = false.ToString(),
                 ValueType = DataType.BOOL,
                 Description = "Zero configuration networking allows clients on your network to detect and connect to your LightSwitch server automatically."
             };
            await settingBuilder.Plugin(this).RegisterPluginSettingAsync(bonjourSetting, o => o.UseBonjourSetting);

        }

        private async void HttpAPIPlugin_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("PortSetting"))
            {
                if (IsEnabled)
                {
                    await StopAsync();
                    await StartAsync();
                }
            }
        }

        public override Task StartAsync()
        {
            PropertyChanged += HttpAPIPlugin_PropertyChanged;

            PublishZeroConf();

            Task.Run(() =>
                {
                    //Run it its own thread...
                    StartLightSwitchServer();
                });

            return Task.FromResult(0);
        }

        public async override Task StopAsync()
        {
            PropertyChanged -= HttpAPIPlugin_PropertyChanged;
            await StopLightSwitchServer();
        }

        private void PublishZeroConf()
        {
            if (UseBonjourSetting)
            {
                try
                {
                    if (_netservice == null)
                        PublishZeroconf();
                    else
                    {
                        _netservice.Dispose();
                        PublishZeroconf();
                    }

                }
                catch (Exception ex)
                {
                    Log.ReportErrorAsync(ex.Message, CancellationToken.None).Wait();
                }
            }
            else
            {
                if (_netservice != null)
                    _netservice.Dispose();
            }
        }

        async void SpeechPlugin_OnEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.EntityUpdatedArgs e)
        {
            using (var context = new ZvsContext(EntityContextConnection))
            {
                var dv = await context.DeviceValues
                    .Include(o => o.Device)
                    .Include(o => o.Device.Type)
                    .FirstOrDefaultAsync(v => v.Id == e.NewEntity.Id, CancellationToken);

                var newValue = e.NewEntity.Value;

                if (dv == null)
                    return;

                if (dv.Name != "Basic") return;
                if (!_zvsTypeToLsType.ContainsKey(dv.Device.Type.UniqueIdentifier))
                    return;

                var level = newValue;
                var type = _zvsTypeToLsType[dv.Device.Type.UniqueIdentifier];

                int l;
                int.TryParse(newValue, out l);

                if (dv.Device.Type.UniqueIdentifier == "SWITCH")
                    level = (l > 0 ? "255" : "0");

                await BroadcastCommandAsync(LightSwitchProtocol.CreateUpdateCmd(dv.Device.Name, dv.Device.Id.ToString(), level, type));
                await BroadcastCommandAsync(LightSwitchProtocol.CreateEndListCmd());
                await BroadcastCommandAsync(LightSwitchProtocol.CreateMsgCmdFormat("'{0}' {1} changed to {2}", dv.Device.Name, dv.Name, newValue));
            }
        }

        public async void StartLightSwitchServer()
        {
            _cts = new CancellationTokenSource();
            var listener = new TcpListener(IPAddress.Any, PortSetting);
            listener.Server.NoDelay = true;
            listener.Server.LingerState = new LingerOption(true, 2);
            listener.Start();
            await Log.ReportInfoFormatAsync(CancellationToken, "LightSwitch server started on port {0}", PortSetting);
            NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.OnEntityUpdated += SpeechPlugin_OnEntityUpdated;

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
                _lightSwitchClients.Add(lightSwitchClient);

                lightSwitchClient.OnConnectionEstabilished += lightSwitchClient_ConnectionEstabilished;
                lightSwitchClient.OnConnectionClosed += lightSwitchClient_ConnectionClosed;

                lightSwitchClient.OnDataReceived += lightSwitchClient_DataReceived;
                lightSwitchClient.OnDataSent += lightSwitchClient_DataSent;

                lightSwitchClient.OnCmdAList += lightSwitchClient_onCmdAList;
                lightSwitchClient.OnCmdDevice += lightSwitchClient_onCmdDevice;
                lightSwitchClient.OnCmdIphone += lightSwitchClient_onCmdIphone;
                lightSwitchClient.OnCmdList += lightSwitchClient_onCmdList;
                lightSwitchClient.OnCmdPassword += lightSwitchClient_onCmdPassword;
                lightSwitchClient.OnCmdScene += lightSwitchClient_onCmdScene;
                lightSwitchClient.OnCmdServer += lightSwitchClient_onCmdServer;
                lightSwitchClient.OnCmdSList += lightSwitchClient_onCmdSList;
                lightSwitchClient.OnCmdTerminate += lightSwitchClient_onCmdTerminate;
                lightSwitchClient.OnCmdThermMode += lightSwitchClient_onCmdThermMode;
                lightSwitchClient.OnCmdThermTemp += lightSwitchClient_onCmdThermTemp;
                lightSwitchClient.OnCmdVersion += lightSwitchClient_onCmdVersion;
                lightSwitchClient.OnCmdZList += lightSwitchClient_onCmdZList;
                lightSwitchClient.OnCmdZone += lightSwitchClient_onCmdZone;

                lightSwitchClient.StartMonitoring();
            }
            listener.Stop();
            NotifyEntityChangeContext.ChangeNotifications<DeviceValue>.OnEntityUpdated -= SpeechPlugin_OnEntityUpdated;
            await Log.ReportInfoAsync("LightSwitch server stopped", CancellationToken);
        }

        async void lightSwitchClient_DataSent(object sender, LightSwitchDataEventArgs args)
        {
            if (VerboseSetting && !string.IsNullOrEmpty(args.RawData))
                await Log.ReportInfoFormatAsync(CancellationToken, "Sent to [{0}]: {1}", args.LightSwitchClient.RemoteEndPoint, args.RawData.Trim());
        }

        private async Task SendLightSwitchCommand(LightSwitchClient lightSwitchClient, LightSwitchCommand lightSwitchCommand)
        {
            var sendCommandResult = await lightSwitchClient.SendCommandAsync(lightSwitchCommand);
            if (sendCommandResult.HasError)
                await Log.ReportErrorAsync(sendCommandResult.Message, CancellationToken);
        }

        #region LightSwitch Client Events

        async void lightSwitchClient_DataReceived(object sender, LightSwitchDataEventArgs args)
        {
            if (VerboseSetting && !string.IsNullOrEmpty(args.RawData))
                await Log.ReportInfoFormatAsync(CancellationToken, "Received from [{0}]: {1}", args.LightSwitchClient.RemoteEndPoint, args.RawData.Trim());
        }

        async void lightSwitchClient_onCmdIphone(object sender, LightSwitchClientEventArgs args)
        {
            args.LightSwitchClient.IsAuthenticated = false;
            await SendLightSwitchCommand(args.LightSwitchClient, LightSwitchProtocol.CreateCookieCmd(args.LightSwitchClient.Nonce.ToString()));
        }

        async void lightSwitchClient_onCmdPassword(object sender, OnPasswordEventArgs args)
        {
            var lightSwitchClient = args.LightSwitchClient;
            var hashedPassword = EncodePassword(string.Format("{0}:{1}", lightSwitchClient.Nonce, PasswordSetting));

            if (args.Password.StartsWith(hashedPassword))
            {
                lightSwitchClient.IsAuthenticated = true;
                await SendLightSwitchCommand(lightSwitchClient, LightSwitchProtocol.CreateVersionCmd(Utils.ApplicationNameAndVersion));
                await Log.ReportInfoFormatAsync(CancellationToken, "Received [{0}] User Authenticated", lightSwitchClient.RemoteEndPoint);
            }
            else
            {
                lightSwitchClient.IsAuthenticated = false;
                lightSwitchClient.Disconnect();
            }
        }

        async void lightSwitchClient_onCmdAList(object sender, LightSwitchClientEventArgs args)
        {
            if (!args.LightSwitchClient.IsAuthenticated)
                args.LightSwitchClient.Disconnect();

            await Log.ReportInfoFormatAsync(CancellationToken, "Received [{0}] User requested device, scene and group list", args.LightSwitchClient.RemoteEndPoint);

            await SendDeviceListAsync(args.LightSwitchClient);
            await SendSceneListAsync(args.LightSwitchClient);
            await SendZoneListAsync(args.LightSwitchClient);

            await SendLightSwitchCommand(args.LightSwitchClient, LightSwitchProtocol.CreateEndListCmd());
        }



        async void lightSwitchClient_onCmdZList(object sender, LightSwitchClientEventArgs args)
        {
            await Log.ReportInfoFormatAsync(CancellationToken, "Received [{0}] User requested zone list", args.LightSwitchClient.RemoteEndPoint);
            await SendZoneListAsync(args.LightSwitchClient);
            await SendLightSwitchCommand(args.LightSwitchClient, LightSwitchProtocol.CreateEndListCmd());
        }
        async void lightSwitchClient_onCmdSList(object sender, LightSwitchClientEventArgs args)
        {
            if (!args.LightSwitchClient.IsAuthenticated)
                args.LightSwitchClient.Disconnect();

            await Log.ReportInfoFormatAsync(CancellationToken, "Received [{0}] User requested scene list", args.LightSwitchClient.RemoteEndPoint);
            await SendSceneListAsync(args.LightSwitchClient);
            await SendLightSwitchCommand(args.LightSwitchClient, LightSwitchProtocol.CreateEndListCmd());
        }
        async void lightSwitchClient_onCmdList(object sender, LightSwitchClientEventArgs args)
        {
            if (!args.LightSwitchClient.IsAuthenticated)
                args.LightSwitchClient.Disconnect();

            await Log.ReportInfoFormatAsync(CancellationToken, "Received [{0}] User requested device list", args.LightSwitchClient.RemoteEndPoint);
            await SendDeviceListAsync(args.LightSwitchClient);
            await SendLightSwitchCommand(args.LightSwitchClient, LightSwitchProtocol.CreateEndListCmd());
        }

        async void lightSwitchClient_onCmdVersion(object sender, LightSwitchClientEventArgs args)
        {
            if (!args.LightSwitchClient.IsAuthenticated)
                args.LightSwitchClient.Disconnect();

            await SendLightSwitchCommand(args.LightSwitchClient, LightSwitchProtocol.CreateVersionCmd(Utils.ApplicationNameAndVersion));
        }
        void lightSwitchClient_onCmdTerminate(object sender, LightSwitchClientEventArgs args)
        {
            args.LightSwitchClient.Disconnect();
        }
        async void lightSwitchClient_onCmdServer(object sender, LightSwitchClientEventArgs args)
        {
            if (!args.LightSwitchClient.IsAuthenticated)
                args.LightSwitchClient.Disconnect();

            await SendLightSwitchCommand(args.LightSwitchClient, LightSwitchProtocol.CreateVersionCmd(Utils.ApplicationNameAndVersion));
        }

        async void lightSwitchClient_onCmdDevice(object sender, OnDeviceEventArgs args)
        {
            if (!args.LightSwitchClient.IsAuthenticated)
                args.LightSwitchClient.Disconnect();

            int deviceId = int.TryParse(args.DeviceId, out deviceId) ? deviceId : 0;
            byte level = byte.TryParse(args.Level, out level) ? level : (byte)0;

            using (var context = new ZvsContext(EntityContextConnection))
            {
                var d = await context.Devices
                    .Include(o => o.Type)
                    .Include(o => o.Type.Commands)
                    .FirstOrDefaultAsync(o => o.Id == deviceId);
                if (d == null)
                {
                    const string error = "Cannot locate device";
                    await SendLightSwitchCommand(args.LightSwitchClient, LightSwitchProtocol.CreateErrorMsgCmd(error));
                    await Log.ReportErrorAsync(error, CancellationToken);

                    return;
                }

                string arg1 = null;
                string arg2 = null;
                Command command = null;
                var cmdMsg = string.Empty;

                #region Find Command

                switch (d.Type.UniqueIdentifier)
                {
                    case "SWITCH":
                        {
                            var cmdUniqueId = (level == 0 ? "TURNOFF" : "TURNON");

                            var dtcmd = d.Type.Commands.FirstOrDefault(c => c.UniqueIdentifier == cmdUniqueId);
                            if (dtcmd == null)
                            {
                                const string error = "Cannot locate zvs command";
                                await SendLightSwitchCommand(args.LightSwitchClient, LightSwitchProtocol.CreateErrorMsgCmd(error));
                                await Log.ReportErrorAsync(error, CancellationToken);

                                return;
                            }

                            cmdMsg = string.Format("[{0}] Executed command '{1}' on '{2}'.", args.LightSwitchClient.RemoteEndPoint, dtcmd.Name, d.Name);
                            command = dtcmd;
                            arg1 = string.Empty;
                            arg2 = d.Id.ToString();
                            break;
                        }
                    case "DIMMER":
                        {
                            var l = (level == 255 ? "99" : level.ToString());
                            if (!_pluginToDimmerBasicCommand.ContainsKey(d.Type.Adapter.AdapterGuid))
                            {
                                const string error = "No command defines for this plug-in";
                                await SendLightSwitchCommand(args.LightSwitchClient, LightSwitchProtocol.CreateErrorMsgCmd(error));
                                await Log.ReportErrorAsync(error, CancellationToken);

                                return;
                            }

                            var cmdUniqueId = _pluginToDimmerBasicCommand[d.Type.Adapter.AdapterGuid];
                            var dcmd = d.Commands.FirstOrDefault(c => c.UniqueIdentifier.Contains(cmdUniqueId));
                            if (dcmd == null)
                            {
                                const string error = "Cannot locate zvs command";
                                await SendLightSwitchCommand(args.LightSwitchClient, LightSwitchProtocol.CreateErrorMsgCmd(error));
                                await Log.ReportErrorAsync(error, CancellationToken);

                                return;
                            }

                            cmdMsg = string.Format("[{0}] Executed command '{1}' on '{2}'.", args.LightSwitchClient.RemoteEndPoint, dcmd.Name, d.Name);
                            command = dcmd;
                            arg1 = l;
                            arg2 = d.Id.ToString();

                            break;
                        }
                }
                #endregion

                if (command == null)
                {
                    const string error = "Cannot locate zvs command";
                    await SendLightSwitchCommand(args.LightSwitchClient, LightSwitchProtocol.CreateErrorMsgCmd(error));
                    await Log.ReportErrorAsync(error, CancellationToken);

                    return;
                }

                await Log.ReportInfoAsync(cmdMsg, CancellationToken);
                var commandResult = await RunCommandAsync(command.Id, arg1, arg2, CancellationToken);
                if (commandResult.HasError)
                    await SendLightSwitchCommand(args.LightSwitchClient, LightSwitchProtocol.CreateErrorMsgCmd(commandResult.Message));
                else
                    await BroadcastCommandAsync(LightSwitchProtocol.CreateMsgCmd(cmdMsg));
            }
        }
        async void lightSwitchClient_onCmdZone(object sender, OnZoneEventArgs args)
        {
            int groupId = int.TryParse(args.ZoneId, out groupId) ? groupId : 0;
            var cmdUniqId = (args.Level.Equals("255") ? "GROUP_ON" : "GROUP_OFF");

            using (var context = new ZvsContext(EntityContextConnection))
            {
                var g = await context.Groups.FirstOrDefaultAsync(o => o.Id == groupId);
                if (g == null) return;

                var zvsCmd = await context.BuiltinCommands.FirstOrDefaultAsync(c => c.UniqueIdentifier == cmdUniqId, CancellationToken);
                if (zvsCmd == null)
                {
                    const string error = "Cannot locate zvs command";
                    await SendLightSwitchCommand(args.LightSwitchClient, LightSwitchProtocol.CreateErrorMsgCmd(error));
                    await Log.ReportErrorAsync(error, CancellationToken);
                    return;
                }
                var result = string.Format("[{0}] Ran {1} on group '{2}'", args.LightSwitchClient.RemoteEndPoint, zvsCmd.Name, g.Name);
                await Log.ReportInfoAsync(result, CancellationToken);

                var r = await RunCommandAsync(zvsCmd.Id, g.Id.ToString(), string.Empty, CancellationToken);
                if (r.HasError)
                    await SendLightSwitchCommand(args.LightSwitchClient, LightSwitchProtocol.CreateErrorMsgCmd(r.Message));
                else
                    await BroadcastCommandAsync(LightSwitchProtocol.CreateMsgCmd(result));
            }
        }
        async void lightSwitchClient_onCmdScene(object sender, OnSceneEventArgs args)
        {
            if (!args.LightSwitchClient.IsAuthenticated)
                args.LightSwitchClient.Disconnect();

            int sceneId = int.TryParse(args.SceneId, out sceneId) ? sceneId : 0;
            using (var context = new ZvsContext(EntityContextConnection))
            {
                var bcmd = await context.BuiltinCommands.FirstOrDefaultAsync(c => c.UniqueIdentifier == "RUN_SCENE", CancellationToken);
                if (bcmd == null)
                {
                    const string error = "Cannot locate zvs command";
                    await SendLightSwitchCommand(args.LightSwitchClient, LightSwitchProtocol.CreateErrorMsgCmd(error));
                    await Log.ReportErrorAsync(error, CancellationToken);

                    return;
                }

                var r = await RunCommandAsync(bcmd.Id, sceneId.ToString(), string.Empty, CancellationToken);
                if (r.HasError)
                    await SendLightSwitchCommand(args.LightSwitchClient, LightSwitchProtocol.CreateErrorMsgCmd(r.Message));
                else
                    await BroadcastCommandAsync(LightSwitchProtocol.CreateMsgCmd(r.Message));
            }
        }

        async void lightSwitchClient_onCmdThermTemp(object sender, OnThermTempEventArgs args)
        {
            if (!args.LightSwitchClient.IsAuthenticated)
                args.LightSwitchClient.Disconnect();

            int deviceId = int.TryParse(args.DeviceId, out deviceId) ? deviceId : 0;

            using (var context = new ZvsContext(EntityContextConnection))
            {
                var d = await context.Devices
                    .Include(o => o.Type.Adapter)
                    .Include(o => o.Commands)
                    .FirstOrDefaultAsync(o => o.Id == deviceId, CancellationToken);

                if (d == null)
                {
                    const string error = "Cannot locate thermostat";
                    await SendLightSwitchCommand(args.LightSwitchClient, LightSwitchProtocol.CreateErrorMsgCmd(error));
                    await Log.ReportErrorAsync(error, CancellationToken);

                    return;
                }

                var plugin = d.Type.Adapter.AdapterGuid;
                var key = plugin + args.Mode;

                if (!_thermoTempCommandTranslations.ContainsKey(key))
                {
                    const string error = "No command defined for this plug-in";
                    await SendLightSwitchCommand(args.LightSwitchClient, LightSwitchProtocol.CreateErrorMsgCmd(error));
                    await Log.ReportErrorAsync(error, CancellationToken);
                    return;
                }

                var dcmd = d.Commands.FirstOrDefault(c => c.UniqueIdentifier.StartsWith(_thermoTempCommandTranslations[key]));
                if (dcmd == null)
                {
                    const string error = "Cannot locate zvs command";
                    await SendLightSwitchCommand(args.LightSwitchClient, LightSwitchProtocol.CreateErrorMsgCmd(error));
                    await Log.ReportErrorAsync(error, CancellationToken);
                    return;
                }

                var cmdMsg = string.Format("[{0}] Executed command '{1}' on '{2}'.", args.LightSwitchClient.RemoteEndPoint, dcmd.Name, d.Name);
                await Log.ReportInfoAsync(cmdMsg, CancellationToken);
                var commandResult = await RunCommandAsync(dcmd.Id, args.Temp, string.Empty, CancellationToken);

                if (commandResult.HasError)
                    await SendLightSwitchCommand(args.LightSwitchClient, LightSwitchProtocol.CreateErrorMsgCmd(commandResult.Message));
                else
                    await BroadcastCommandAsync(LightSwitchProtocol.CreateMsgCmd(cmdMsg));
            }
        }
        async void lightSwitchClient_onCmdThermMode(object sender, OnThermModeEventArgs args)
        {
            if (!args.LightSwitchClient.IsAuthenticated)
                args.LightSwitchClient.Disconnect();

            int deviceId = int.TryParse(args.DeviceId, out deviceId) ? deviceId : 0;
            int mode = int.TryParse(args.Mode, out mode) ? mode : 0;

            string arg1 = null;
            string arg2 = null;
            Command command = null;
            var cmdMsg = string.Empty;

            //PLUGINNAME-0 -->CmdName,Arg 
            using (var context = new ZvsContext(EntityContextConnection))
            {
                var d = await context.Devices
                    .Include(o => o.Type.Adapter)
                    .Include(o => o.Type.Commands)
                    .Include(o => o.Commands)
                    .FirstOrDefaultAsync(o => o.Id == deviceId, CancellationToken);

                if (d == null)
                {
                    const string error = "Cannot locate thermostat";
                    await SendLightSwitchCommand(args.LightSwitchClient, LightSwitchProtocol.CreateErrorMsgCmd(error));
                    await Log.ReportErrorAsync(error, CancellationToken);
                    return;
                }

                if (mode < 6)
                {
                    var plugin = d.Type.Adapter.AdapterGuid.ToString();
                    var key = plugin + args.Mode;

                    if (!_thermoCommandTranslations.ContainsKey(key))
                    {
                        const string error = "No command defined for this plug-in";
                        await SendLightSwitchCommand(args.LightSwitchClient, LightSwitchProtocol.CreateErrorMsgCmd(error));
                        await Log.ReportErrorAsync(error, CancellationToken);

                        return;
                    }

                    var cmd = _thermoCommandTranslations[key];
                    var dcmd = d.Commands.FirstOrDefault(c => c.UniqueIdentifier.Contains(cmd.CmdName));
                    if (dcmd == null)
                    {
                        const string error = "Cannot locate zvs command";
                        await SendLightSwitchCommand(args.LightSwitchClient, LightSwitchProtocol.CreateErrorMsgCmd(error));
                        await Log.ReportErrorAsync(error, CancellationToken);

                        return;
                    }

                    command = dcmd;
                    arg1 = cmd.Arg;
                    cmdMsg = string.Format("[{0}] Executed command '{1}' on '{2}'.", args.LightSwitchClient.RemoteEndPoint, dcmd.Name, d.Name);

                }
                else switch (mode)
                    {
                        case 6:
                            {
                                var dcmd = d.Type.Commands.FirstOrDefault(c => c.UniqueIdentifier == "SETENERGYMODE");
                                if (dcmd == null)
                                {
                                    const string error = "Cannot locate zvs command";
                                    await SendLightSwitchCommand(args.LightSwitchClient, LightSwitchProtocol.CreateErrorMsgCmd(error));
                                    await Log.ReportErrorAsync(error, CancellationToken);
                                    return;
                                }

                                command = dcmd;
                                arg1 = string.Empty;
                                arg2 = d.Id.ToString();
                                cmdMsg = string.Format("[{0}] Executed command '{1}' on '{2}'.", args.LightSwitchClient.RemoteEndPoint, dcmd.Name, d.Name);
                            }
                            break;
                        case 7:
                            {
                                var dcmd = d.Type.Commands.FirstOrDefault(c => c.UniqueIdentifier == "SETCONFORTMODE");
                                if (dcmd == null)
                                {
                                    const string error = "Cannot locate zvs command";
                                    await SendLightSwitchCommand(args.LightSwitchClient, LightSwitchProtocol.CreateErrorMsgCmd(error));
                                    await Log.ReportErrorAsync(error, CancellationToken);

                                    return;
                                }

                                command = dcmd;
                                arg1 = string.Empty;
                                arg2 = d.Id.ToString();
                                cmdMsg = string.Format("[{0}] Executed command '{1}' on '{2}'.", args.LightSwitchClient.RemoteEndPoint, dcmd.Name, d.Name);
                            }
                            break;
                    }
            }

            if (command == null)
            {
                const string error = "Cannot locate zvs command";
                await SendLightSwitchCommand(args.LightSwitchClient, LightSwitchProtocol.CreateErrorMsgCmd(error));
                await Log.ReportErrorAsync(error, CancellationToken);

                return;
            }

            await Log.ReportInfoAsync(cmdMsg, CancellationToken);
            var commandResult = await RunCommandAsync(command.Id, arg1, arg2, CancellationToken);

            if (commandResult.HasError)
                await SendLightSwitchCommand(args.LightSwitchClient, LightSwitchProtocol.CreateErrorMsgCmd(commandResult.Message));
            else
                await BroadcastCommandAsync(LightSwitchProtocol.CreateMsgCmd(cmdMsg));
        }
        #endregion

        private readonly Dictionary<Guid, string> _pluginToDimmerBasicCommand = new Dictionary<Guid, string>()
        {
            {Guid.Parse("70f91ca6-08bb-406a-a60f-aeb13f50aae8"), "DYNAMIC_CMD_BASIC"}
           // {"THINKSTICK", "BASIC"} 
        };

        private readonly Dictionary<string, string> _thermoTempCommandTranslations = new Dictionary<string, string>()
        {
            {"THINKSTICK2", "DYNAMIC_SP_R207_Heating1"},
            {"70f91ca6-08bb-406a-a60f-aeb13f50aae82", "DYNAMIC_CMD_HEATING 1_"},
            {"THINKSTICK3", "DYNAMIC_SP_R207_Cooling1"},
            {"70f91ca6-08bb-406a-a60f-aeb13f50aae83", "DYNAMIC_CMD_COOLING 1_"} 
        };

        private class ZvsCmd
        {
            public string CmdName;
            public string Arg;
        }

        private readonly Dictionary<string, ZvsCmd> _thermoCommandTranslations = new Dictionary<string, ZvsCmd>()
        {
            {"THINKSTICK0", new ZvsCmd() { CmdName="MODE", Arg="Off"}},
            {"70f91ca6-08bb-406a-a60f-aeb13f50aae80", new ZvsCmd() { CmdName="DYNAMIC_CMD_MODE", Arg="Off"}},
            {"THINKSTICK1", new ZvsCmd() { CmdName="MODE", Arg="Auto"}},
            {"70f91ca6-08bb-406a-a60f-aeb13f50aae81", new ZvsCmd() { CmdName="DYNAMIC_CMD_MODE", Arg="Auto"}},
            {"THINKSTICK2", new ZvsCmd() { CmdName="MODE", Arg="Heat"}},
            {"70f91ca6-08bb-406a-a60f-aeb13f50aae82", new ZvsCmd() { CmdName="DYNAMIC_CMD_MODE", Arg="Heat"}},
            {"THINKSTICK3", new ZvsCmd() { CmdName="MODE", Arg="Cool"}},
            {"70f91ca6-08bb-406a-a60f-aeb13f50aae83", new ZvsCmd() { CmdName="DYNAMIC_CMD_MODE", Arg="Cool"}},
            {"THINKSTICK4", new ZvsCmd() { CmdName="FAN_MODE", Arg="OnLow"}},
            {"70f91ca6-08bb-406a-a60f-aeb13f50aae84", new ZvsCmd() { CmdName="DYNAMIC_CMD_FAN MODE", Arg="On Low"}},
            {"THINKSTICK5", new ZvsCmd() { CmdName="FAN_MODE", Arg="AutoLow"}},
            {"70f91ca6-08bb-406a-a60f-aeb13f50aae85", new ZvsCmd() { CmdName="DYNAMIC_CMD_FAN MODE", Arg="Auto Low"}}
        };

        public async Task StopLightSwitchServer()
        {
            await BroadcastCommandAsync(LightSwitchProtocol.CreateMsgCmd("Server shutting down..."));

            foreach (var client in GetSafeClients())
                client.Disconnect();

            _cts.Cancel();
        }

        async void lightSwitchClient_ConnectionClosed(object sender, LightSwitchClientEventArgs args)
        {
            await Log.ReportInfoFormatAsync(CancellationToken, "Client {0} disconnected", args.LightSwitchClient.RemoteEndPoint);

            if (_lightSwitchClients.Contains(args.LightSwitchClient))
                _lightSwitchClients.Remove(args.LightSwitchClient);

            args.LightSwitchClient.OnConnectionEstabilished -= lightSwitchClient_ConnectionEstabilished;
            args.LightSwitchClient.OnConnectionClosed -= lightSwitchClient_ConnectionClosed;

            args.LightSwitchClient.OnDataReceived -= lightSwitchClient_DataReceived;
            args.LightSwitchClient.OnDataSent -= lightSwitchClient_DataSent;

            args.LightSwitchClient.OnCmdAList -= lightSwitchClient_onCmdAList;
            args.LightSwitchClient.OnCmdDevice -= lightSwitchClient_onCmdDevice;
            args.LightSwitchClient.OnCmdIphone -= lightSwitchClient_onCmdIphone;
            args.LightSwitchClient.OnCmdList -= lightSwitchClient_onCmdList;
            args.LightSwitchClient.OnCmdPassword -= lightSwitchClient_onCmdPassword;
            args.LightSwitchClient.OnCmdScene -= lightSwitchClient_onCmdScene;
            args.LightSwitchClient.OnCmdServer -= lightSwitchClient_onCmdServer;
            args.LightSwitchClient.OnCmdSList -= lightSwitchClient_onCmdSList;
            args.LightSwitchClient.OnCmdTerminate -= lightSwitchClient_onCmdTerminate;
            args.LightSwitchClient.OnCmdThermMode -= lightSwitchClient_onCmdThermMode;
            args.LightSwitchClient.OnCmdThermTemp -= lightSwitchClient_onCmdThermTemp;
            args.LightSwitchClient.OnCmdVersion -= lightSwitchClient_onCmdVersion;
            args.LightSwitchClient.OnCmdZList -= lightSwitchClient_onCmdZList;
            args.LightSwitchClient.OnCmdZone -= lightSwitchClient_onCmdZone;
        }

        async void lightSwitchClient_ConnectionEstabilished(object sender, LightSwitchClientEventArgs args)
        {
            if (_lightSwitchClients.Count > MaxConnectionsSettings)
            {
                await SendLightSwitchCommand(args.LightSwitchClient, LightSwitchProtocol.CreateMsgCmd("Max clients reached!"));
                args.LightSwitchClient.Disconnect();
            }

            await Log.ReportInfoFormatAsync(CancellationToken, "Client {0} connected", args.LightSwitchClient.RemoteEndPoint);
            await SendLightSwitchCommand(args.LightSwitchClient, LightSwitchProtocol.CreateInfoCmdFormat("LightSwitch zVirtualScenes Plug-in (Active Connections {0}){1}", _lightSwitchClients.Count, Environment.NewLine));
        }

        /// <summary>
        /// Sends a message to ALL connected clients.
        /// </summary>
        /// <param name="command">the message to send</param>
        public async Task BroadcastCommandAsync(LightSwitchCommand command)
        {
            foreach (var client in GetSafeClients())
                await SendLightSwitchCommand(client, command);
        }

        private IEnumerable<LightSwitchClient> GetSafeClients()
        {
            var clientCount = _lightSwitchClients.Count();
            var clients = new LightSwitchClient[clientCount];
            _lightSwitchClients.CopyTo(clients);
            return clients;
        }

        #region Lists
        private async Task SendSceneListAsync(LightSwitchClient client)
        {
            using (var context = new ZvsContext(EntityContextConnection))
            {
                var settingUid = SceneSettingUids.ShowInLightswitch.ToString();

                var scenes = await context.Scenes
                    .Where(o => (o.SettingValues.All(p => p.SceneSetting.UniqueIdentifier != settingUid)) || //Show all objects where no explicit setting has been create yet and the defaultSetting is to show
                                 o.SettingValues.Any(p => p.SceneSetting.UniqueIdentifier == settingUid && p.Value.Equals("true"))) //Show all objects where an explicit setting has been create and set to show
                    .OrderBy(o => o.SortOrder)
                    .ToListAsync();

                foreach (var scene in scenes)
                    await SendLightSwitchCommand(client, LightSwitchProtocol.CreateSceneCmd(scene.Name, scene.Id.ToString()));
            }
        }

        private async Task SendZoneListAsync(LightSwitchClient client)
        {
            using (var context = new ZvsContext(EntityContextConnection))
                foreach (var g in await context.Groups.OrderBy(o => o.Name).ToListAsync())
                    await SendLightSwitchCommand(client, LightSwitchProtocol.CreateZoneCmd(g.Name, g.Id.ToString()));
        }

        private readonly Dictionary<string, LightSwitchProtocol.DeviceTypes> _zvsTypeToLsType = new Dictionary<string, LightSwitchProtocol.DeviceTypes>()
        {
            {"Switch", LightSwitchProtocol.DeviceTypes.BinarySwitch},
            {"Dimmer", LightSwitchProtocol.DeviceTypes.MultiLevelSwitch},
            {"Thermostat", LightSwitchProtocol.DeviceTypes.Thermostat},
            {"Sensor", LightSwitchProtocol.DeviceTypes.Sensor},
        };

        private async Task SendDeviceListAsync(LightSwitchClient client)
        {
            using (var context = new ZvsContext(EntityContextConnection))
            {
                //Get Devices
                var settingUid = DeviceSettingUids.ShowInLightswitch.ToString();

                var devices = await context.Devices
                    .Where(o => (o.DeviceSettingValues.All(p => p.DeviceSetting.UniqueIdentifier != settingUid)) || //Show all objects where no explicit setting has been create yet and the defaultSetting is to show
                                 o.DeviceSettingValues.Any(p => p.DeviceSetting.UniqueIdentifier == settingUid && p.Value.Equals("true"))) //Show all objects where an explicit setting has been create and set to show
                    .Include(o => o.Type)
                    .OrderBy(o => o.Location)
                    .ThenBy(o=> o.Name)
                    .Where(o => o.Type.UniqueIdentifier != "Controller")
                    .ToListAsync();

                foreach (var device in devices)
                {
                    if (!_zvsTypeToLsType.ContainsKey(device.Type.UniqueIdentifier))
                        continue;

                    var level = ((int)device.CurrentLevelInt).ToString();
                    var type = _zvsTypeToLsType[device.Type.UniqueIdentifier];

                    if (device.Type.UniqueIdentifier == "switch")
                        level = (device.CurrentLevelInt > 0 ? "255" : "0");

                    var deviceName = string.Format("{0}{1}", string.IsNullOrWhiteSpace(device.Location) ? "" : device.Location + " ", device.Name);

                    await SendLightSwitchCommand(client, LightSwitchProtocol.CreateDeviceCmd(deviceName, device.Id.ToString(), level, type));

                }
            }
        }

        #endregion

        public string EncodePassword(string originalPassword)
        {
            // Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
            var originalBytes = Encoding.Default.GetBytes(originalPassword);
            var encodedBytes = new MD5CryptoServiceProvider().ComputeHash(originalBytes);

            var result = new StringBuilder();
            foreach (var t in encodedBytes)
                result.Append(t.ToString("x2"));

            return result.ToString().ToUpper();
        }

        #region ZeroConf/Bonjour

        private void PublishZeroconf()
        {
            var name = "Lightswitch " + Environment.MachineName;
            _netservice = new Mono.Zeroconf.Providers.Bonjour.RegisterService
            {
                Name = name,
                RegType = "_lightswitch._tcp",
                ReplyDomain = "",
                Port = (short)PortSetting
            };

            _netservice.Response += netservice_Response;

            // TxtRecords are optional
            var txtRecord = new TxtRecord
            {
                {"txtvers", "1"},
                {"ServiceName", name},
                {"MachineName", Environment.MachineName},
                {"OS", Environment.OSVersion.ToString()},
                {"IPAddress", "127.0.0.1"},
                {"Version", Utils.ApplicationNameAndVersion}
            };
            //txt_record.Add("Password", "false");
            _netservice.TxtRecord = txtRecord;

            _netservice.Register();
        }

        async void netservice_Response(object o, RegisterServiceEventArgs args)
        {
            await Log.ReportInfoFormatAsync(CancellationToken, "Published Service: isRegistered:{0},  type: {1} name:{2}", args.IsRegistered, args.Service.RegType, args.Service.Name);
        }
        #endregion
    }
}

