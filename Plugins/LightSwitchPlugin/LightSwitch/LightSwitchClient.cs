using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using zvs;
using zvs.DataModel;

namespace LightSwitchPlugin.LightSwitch
{
    public class LightSwitchClient : ILightSwitchChannel
    {
        public TcpClient TcpClient { get; private set; }
        public int Nonce { get; private set; }
        public bool IsAuthenticated { get; set; }
        public string RemoteEndPoint { get; private set; }

        #region Events
        public delegate void OnDataReceivedEventHandler(object sender, LightSwitchDataEventArgs args);
        public event OnDataReceivedEventHandler OnDataReceived = delegate { };

        public delegate void OnDataSentEventHandler(object sender, LightSwitchDataEventArgs args);
        public event OnDataSentEventHandler OnDataSent = delegate { };

        public delegate void OnConnectionClosedEventHandler(object sender, LightSwitchClientEventArgs args);
        public event OnConnectionClosedEventHandler OnConnectionClosed = delegate { };

        public delegate void OnConnectionEstabilishedEventHandler(object sender, LightSwitchClientEventArgs args);
        public event OnConnectionEstabilishedEventHandler OnConnectionEstabilished = delegate { };

        public delegate void OnCmdIphoneEventHandler(object sender, LightSwitchClientEventArgs args);
        public event OnCmdIphoneEventHandler OnCmdIphone = delegate { };

        public delegate void OnCmdVersionEventHandler(object sender, LightSwitchClientEventArgs args);
        public event OnCmdVersionEventHandler OnCmdVersion = delegate { };

        public delegate void OnCmdServerEventHandler(object sender, LightSwitchClientEventArgs args);
        public event OnCmdServerEventHandler OnCmdServer = delegate { };

        public delegate void OnCmdTerminateEventHandler(object sender, LightSwitchClientEventArgs args);
        public event OnCmdTerminateEventHandler OnCmdTerminate = delegate { };

        public delegate void OnCmdAListEventHandler(object sender, LightSwitchClientEventArgs args);
        public event OnCmdAListEventHandler OnCmdAList = delegate { };

        public delegate void OnCmdSListEventHandler(object sender, LightSwitchClientEventArgs args);
        public event OnCmdSListEventHandler OnCmdSList = delegate { };

        public delegate void OnCmdListEventHandler(object sender, LightSwitchClientEventArgs args);
        public event OnCmdListEventHandler OnCmdList = delegate { };

        public delegate void OnCmdZListEventHandler(object sender, LightSwitchClientEventArgs args);
        public event OnCmdZListEventHandler OnCmdZList = delegate { };

        public delegate void OnCmdPasswordEventHandler(object sender, OnPasswordEventArgs args);
        public event OnCmdPasswordEventHandler OnCmdPassword = delegate { };

        public delegate void OnCmdDeviceEventHandler(object sender, OnDeviceEventArgs args);
        public event OnCmdDeviceEventHandler OnCmdDevice = delegate { };

        public delegate void OnCmdSceneEventHandler(object sender, OnSceneEventArgs args);
        public event OnCmdSceneEventHandler OnCmdScene = delegate { };

        public delegate void OnCmdZoneEventHandler(object sender, OnZoneEventArgs args);
        public event OnCmdZoneEventHandler OnCmdZone = delegate { };

        public delegate void OnCmdThermTempEventHandler(object sender, OnThermTempEventArgs args);
        public event OnCmdThermTempEventHandler OnCmdThermTemp = delegate { };

        public delegate void OnCmdThermModeEventHandler(object sender, OnThermModeEventArgs args);
        public event OnCmdThermModeEventHandler OnCmdThermMode = delegate { };
        #endregion

        public IFeedback<LogEntry> Log { get; set; }

        public LightSwitchClient(TcpClient tcpClient)
        {
            IsAuthenticated = false;
            TcpClient = tcpClient;
            Nonce = new Random().Next(65536);
            RemoteEndPoint = TcpClient.Connected ? TcpClient.Client.RemoteEndPoint.ToString() : "Unknown";
        }

        public void StartMonitoring()
        {
            Task.Factory.StartNew(() => { ProcessConnection(TcpClient); });
        }

        public void Disconnect()
        {
            if (TcpClient.Connected)
                TcpClient.Close();
        }

        public async void ProcessConnection(TcpClient tcpClient)
        {
            var message = new byte[1024];

            var networkStream = tcpClient.GetStream();

            if (OnConnectionEstabilished != null)
                OnConnectionEstabilished(this, new LightSwitchClientEventArgs(this));

            while (true)
            {
                int bytesRead;
                try
                {
                    bytesRead = await networkStream.ReadAsync(message, 0, 1024);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    break;
                }

                if (bytesRead == 0)
                {
                    //the client has disconnected from the server
                    break;
                }

                var encoder = new ASCIIEncoding();
                if (OnDataReceived == null) continue;
                var incomingMessage = encoder.GetString(message, 0, bytesRead);
                OnDataReceived(this, new LightSwitchDataEventArgs(this, incomingMessage));
                LightSwitchProtocol.DecodeIncoming(incomingMessage, this);
            }
            tcpClient.Close();
            if (OnConnectionClosed != null)
                OnConnectionClosed(this, new LightSwitchClientEventArgs(this));
        }

        /// <summary>
        /// Send command to a client
        /// </summary>
        public async Task<Result> SendCommandAsync(LightSwitchCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (command.RawCommand.Length > 0 && TcpClient.Connected)
            {
                try
                {
                    var byteArray = command.ToBytes();
                    var clientStream = TcpClient.GetStream();
                    await clientStream.WriteAsync(byteArray, 0, byteArray.Length);
                }
                catch (SocketException se)
                {
                    return Result.ReportError(string.Format("LightSwitch Socket Exception: {0}", se.Message));
                }
                catch (Exception e)
                {
                    return Result.ReportError(string.Format("LightSwitch Exception: " + e.Message));
                }
                OnDataSent(this, new LightSwitchDataEventArgs(this, command.RawCommand));
            }
            return Result.ReportSuccess();
        }

        #region ILightSwitchChannel Methods

        public void OnIphone()
        {
            OnCmdIphone(this, new LightSwitchClientEventArgs(this));
        }

        public void OnPassword(string password)
        {
            OnCmdPassword(this, new OnPasswordEventArgs(this, password));
        }

        public void OnVersion()
        {
            OnCmdVersion(this, new LightSwitchClientEventArgs(this));
        }

        public void OnServer()
        {
            OnCmdServer(this, new LightSwitchClientEventArgs(this));
        }

        public void OnTerminate()
        {
            OnCmdTerminate(this, new LightSwitchClientEventArgs(this));
        }

        public void OnAList()
        {
            OnCmdAList(this, new LightSwitchClientEventArgs(this));
        }

        public void OnSList()
        {
            OnCmdSList(this, new LightSwitchClientEventArgs(this));
        }

        public void OnList()
        {
            OnCmdList(this, new LightSwitchClientEventArgs(this));
        }

        public void OnZList()
        {
            OnCmdZList(this, new LightSwitchClientEventArgs(this));
        }

        public void OnDevice(string deviceId, string level, string type)
        {
            OnCmdDevice(this, new OnDeviceEventArgs(this, deviceId, level, type));
        }

        public void OnScene(string sceneId)
        {
            OnCmdScene(this, new OnSceneEventArgs(this, sceneId));
        }

        public void OnZone(string zoneId, string level)
        {
            OnCmdZone(this, new OnZoneEventArgs(this, zoneId, level));
        }

        public void OnThermTemp(string deviceId, string mode, string temp, string type)
        {
            OnCmdThermTemp(this, new OnThermTempEventArgs(this, deviceId, mode, temp, type));
        }

        public void OnThermMode(string deviceId, string mode, string type)
        {
            OnCmdThermMode(this, new OnThermModeEventArgs(this, deviceId, mode, type));
        }
        #endregion
    }

    #region Event Args
    public class LightSwitchClientEventArgs : EventArgs
    {
        public LightSwitchClient LightSwitchClient { get; private set; }
        public LightSwitchClientEventArgs(LightSwitchClient lightSwitchClient)
        {
            LightSwitchClient = lightSwitchClient;
        }
    }

    public class LightSwitchDataEventArgs : LightSwitchClientEventArgs
    {
        public string RawData { get; private set; }
        public LightSwitchDataEventArgs(LightSwitchClient lightSwitchClient, String rawData)
            : base(lightSwitchClient)
        {
            RawData = rawData;
        }
    }

    public class OnPasswordEventArgs : LightSwitchClientEventArgs
    {
        public string Password { get; private set; }
        public OnPasswordEventArgs(LightSwitchClient lightSwitchClient, string password)
            : base(lightSwitchClient)
        {
            Password = password;
        }
    }

    public class OnDeviceEventArgs : LightSwitchClientEventArgs
    {
        public string DeviceId { get; private set; }
        public string Level { get; private set; }
        public string Type { get; private set; }
        public OnDeviceEventArgs(LightSwitchClient lightSwitchClient, string deviceId, string level, string type)
            : base(lightSwitchClient)
        {
            DeviceId = deviceId;
            Level = level;
            Type = type;
        }
    }

    public class OnSceneEventArgs : LightSwitchClientEventArgs
    {
        public string SceneId { get; private set; }
        public OnSceneEventArgs(LightSwitchClient lightSwitchClient, string sceneId)
            : base(lightSwitchClient)
        {
            SceneId = sceneId;
        }
    }

    public class OnZoneEventArgs : LightSwitchClientEventArgs
    {
        public string ZoneId { get; private set; }
        public string Level { get; private set; }
        public OnZoneEventArgs(LightSwitchClient lightSwitchClient, string zoneId, string level)
            : base(lightSwitchClient)
        {
            ZoneId = zoneId;
            Level = level;
        }
    }

    public class OnThermTempEventArgs : LightSwitchClientEventArgs
    {
        public string DeviceId { get; private set; }
        public string Mode { get; private set; }
        public string Temp { get; private set; }
        public string Type { get; private set; }
        public OnThermTempEventArgs(LightSwitchClient lightSwitchClient, string deviceId, string mode, string temp, string type)
            : base(lightSwitchClient)
        {
            DeviceId = deviceId;
            Mode = mode;
            Temp = temp;
            Type = type;
        }
    }

    public class OnThermModeEventArgs : LightSwitchClientEventArgs
    {
        public string DeviceId { get; private set; }
        public string Mode { get; private set; }
        public string Type { get; private set; }
        public OnThermModeEventArgs(LightSwitchClient lightSwitchClient, string deviceId, string mode, string type)
            : base(lightSwitchClient)
        {
            DeviceId = deviceId;
            Mode = mode;
            Type = type;
        }
    }
    #endregion
}
