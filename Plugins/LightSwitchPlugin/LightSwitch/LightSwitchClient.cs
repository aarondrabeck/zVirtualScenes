using LightSwitchPlugin.LightSwitch;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LightSwitchPlugin
{
    public class LightSwitchClient : ILightSwitchChannel
    {
        public TcpClient TcpClient { get; private set; }
        public int Nonce { get; private set; }
        public bool isAuthenticated { get; set; }
        public string RemoteEndPoint { get; private set; }
        zvs.Processor.Logging.ILog log = zvs.Processor.Logging.LogManager.GetLogger<LightSwitchPlugin>();

        #region Events
        public delegate void onDataReceivedEventHandler(object sender, LightSwitchDataEventArgs args);
        public event onDataReceivedEventHandler onDataReceived = delegate { };

        public delegate void onDataSentEventHandler(object sender, LightSwitchDataEventArgs args);
        public event onDataSentEventHandler onDataSent = delegate { };

        public delegate void onConnectionClosedEventHandler(object sender, LightSwitchClientEventArgs args);
        public event onConnectionClosedEventHandler onConnectionClosed = delegate { };

        public delegate void onConnectionEstabilishedEventHandler(object sender, LightSwitchClientEventArgs args);
        public event onConnectionEstabilishedEventHandler onConnectionEstabilished = delegate { };

        public delegate void onCmdIphoneEventHandler(object sender, LightSwitchClientEventArgs args);
        public event onCmdIphoneEventHandler onCmdIphone = delegate { };

        public delegate void onCmdVersionEventHandler(object sender, LightSwitchClientEventArgs args);
        public event onCmdVersionEventHandler onCmdVersion = delegate { };

        public delegate void onCmdServerEventHandler(object sender, LightSwitchClientEventArgs args);
        public event onCmdServerEventHandler onCmdServer = delegate { };

        public delegate void onCmdTerminateEventHandler(object sender, LightSwitchClientEventArgs args);
        public event onCmdTerminateEventHandler onCmdTerminate = delegate { };

        public delegate void onCmdAListEventHandler(object sender, LightSwitchClientEventArgs args);
        public event onCmdAListEventHandler onCmdAList = delegate { };

        public delegate void onCmdSListEventHandler(object sender, LightSwitchClientEventArgs args);
        public event onCmdSListEventHandler onCmdSList = delegate { };

        public delegate void onCmdListEventHandler(object sender, LightSwitchClientEventArgs args);
        public event onCmdListEventHandler onCmdList = delegate { };

        public delegate void onCmdZListEventHandler(object sender, LightSwitchClientEventArgs args);
        public event onCmdZListEventHandler onCmdZList = delegate { };

        public delegate void onCmdPasswordEventHandler(object sender, onPasswordEventArgs args);
        public event onCmdPasswordEventHandler onCmdPassword = delegate { };

        public delegate void onCmdDeviceEventHandler(object sender, onDeviceEventArgs args);
        public event onCmdDeviceEventHandler onCmdDevice = delegate { };

        public delegate void onCmdSceneEventHandler(object sender, onSceneEventArgs args);
        public event onCmdSceneEventHandler onCmdScene = delegate { };

        public delegate void onCmdZoneEventHandler(object sender, onZoneEventArgs args);
        public event onCmdZoneEventHandler onCmdZone = delegate { };

        public delegate void onCmdThermTempEventHandler(object sender, onThermTempEventArgs args);
        public event onCmdThermTempEventHandler onCmdThermTemp = delegate { };

        public delegate void onCmdThermModeEventHandler(object sender, onThermModeEventArgs args);
        public event onCmdThermModeEventHandler onCmdThermMode = delegate { };
        #endregion

        public LightSwitchClient(TcpClient tcpClient)
        {
            isAuthenticated = false;
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
            byte[] message = new byte[1024];
            int bytesRead;

            NetworkStream networkStream = tcpClient.GetStream();

            if (this.onConnectionEstabilished != null)
                this.onConnectionEstabilished(this, new LightSwitchClientEventArgs(this));

            while (true)
            {
                bytesRead = 0;
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

                ASCIIEncoding encoder = new ASCIIEncoding();
                if (this.onDataReceived != null)
                {
                    string incomingMessage = encoder.GetString(message, 0, bytesRead);
                    this.onDataReceived(this, new LightSwitchDataEventArgs(this, incomingMessage));
                    LightSwitch.LightSwitchProtocol.DecodeIncoming(incomingMessage, this);
                }
            }
            tcpClient.Close();
            if (this.onConnectionClosed != null)
                this.onConnectionClosed(this, new LightSwitchClientEventArgs(this));
        }

        /// <summary>
        /// Send command to a client
        /// </summary>
        /// <param name="client"></param>
        /// <param name="msg"></param>
        public async Task SendCommandAsync(LightSwitchCommand command)
        {
            if (command.RawCommand.Length > 0 && this.TcpClient.Connected)
            {
                try
                {
                    var byteArray = command.ToBytes();
                    var clientStream = this.TcpClient.GetStream();
                    await clientStream.WriteAsync(byteArray, 0, byteArray.Length);
                }
                catch (SocketException se)
                {
                    log.Error("LightSwitch Socket Exception: " + se.Message);
                    return;
                }
                catch (Exception e)
                {
                    log.Error("LightSwitch Exception: " + e.Message);
                    return;
                }
                onDataSent(this, new LightSwitchDataEventArgs(this, command.RawCommand));
            }
        }

        #region ILightSwitchChannel Methods
        
        public void onIphone()
        {
            onCmdIphone(this, new LightSwitchClientEventArgs(this));
        }

        public void onPassword(string password)
        {
            onCmdPassword(this, new onPasswordEventArgs(this, password));
        }

        public void onVersion()
        {
            onCmdVersion(this, new LightSwitchClientEventArgs(this));
        }

        public void onServer()
        {
            onCmdServer(this, new LightSwitchClientEventArgs(this));
        }

        public void onTerminate()
        {
            onCmdTerminate(this, new LightSwitchClientEventArgs(this));
        }

        public void onAList()
        {
            onCmdAList(this, new LightSwitchClientEventArgs(this));
        }

        public void onSList()
        {
            onCmdSList(this, new LightSwitchClientEventArgs(this));
        }

        public void onList()
        {
            onCmdList(this, new LightSwitchClientEventArgs(this));
        }

        public void onZList()
        {
            onCmdZList(this, new LightSwitchClientEventArgs(this));
        }

        public void onDevice(string deviceId, string level, string type)
        {
            onCmdDevice(this, new onDeviceEventArgs(this, deviceId, level, type));
        }

        public void onScene(string sceneId)
        {
            onCmdScene(this, new onSceneEventArgs(this, sceneId));
        }

        public void onZone(string zoneId, string level)
        {
            onCmdZone(this, new onZoneEventArgs(this, zoneId, level));
        }

        public void onThermTemp(string deviceId, string mode, string temp, string type)
        {
            onCmdThermTemp(this, new onThermTempEventArgs(this, deviceId, mode, temp, type));
        }

        public void onThermMode(string deviceId, string mode, string type)
        {
            onCmdThermMode(this, new onThermModeEventArgs(this, deviceId, mode, type));
        }
        #endregion
    }

    #region Event Args
    public class LightSwitchClientEventArgs : EventArgs
    {
        public LightSwitchClient LightSwitchClient { get; private set; }
        public LightSwitchClientEventArgs(LightSwitchClient lightSwitchClient)
        {
            this.LightSwitchClient = lightSwitchClient;
        }
    }

    public class LightSwitchDataEventArgs : LightSwitchClientEventArgs
    {
        public string RawData { get; private set; }
        public LightSwitchDataEventArgs(LightSwitchClient lightSwitchClient, String rawData)
            : base(lightSwitchClient)
        {
            this.RawData = rawData;
        }
    }

    public class onPasswordEventArgs : LightSwitchClientEventArgs
    {
        public string Password { get; private set; }
        public onPasswordEventArgs(LightSwitchClient lightSwitchClient, string password)
            : base(lightSwitchClient)
        {
            this.Password = password;
        }
    }

    public class onDeviceEventArgs : LightSwitchClientEventArgs
    {
        public string DeviceId { get; private set; }
        public string Level { get; private set; }
        public string Type { get; private set; }
        public onDeviceEventArgs(LightSwitchClient lightSwitchClient, string deviceId, string level, string type)
            : base(lightSwitchClient)
        {
            this.DeviceId = deviceId;
            this.Level = level;
            this.Type = type;
        }
    }

    public class onSceneEventArgs : LightSwitchClientEventArgs
    {
        public string SceneId { get; private set; }
        public onSceneEventArgs(LightSwitchClient lightSwitchClient, string sceneId)
            : base(lightSwitchClient)
        {
            this.SceneId = sceneId;
        }
    }

    public class onZoneEventArgs : LightSwitchClientEventArgs
    {
        public string ZoneId { get; private set; }
        public string Level { get; private set; }
        public onZoneEventArgs(LightSwitchClient lightSwitchClient, string zoneId, string level)
            : base(lightSwitchClient)
        {
            this.ZoneId = zoneId;
            this.Level = level;
        }
    }

    public class onThermTempEventArgs : LightSwitchClientEventArgs
    {
        public string DeviceId { get; private set; }
        public string Mode { get; private set; }
        public string Temp { get; private set; }
        public string Type { get; private set; }
        public onThermTempEventArgs(LightSwitchClient lightSwitchClient, string deviceId, string mode, string temp, string type)
            : base(lightSwitchClient)
        {
            this.DeviceId = deviceId;
            this.Mode = mode;
            this.Temp = temp;
            this.Type = type;
        }
    }

    public class onThermModeEventArgs : LightSwitchClientEventArgs
    {
        public string DeviceId { get; private set; }
        public string Mode { get; private set; }
        public string Type { get; private set; }
        public onThermModeEventArgs(LightSwitchClient lightSwitchClient, string deviceId, string mode, string type)
            : base(lightSwitchClient)
        {
            this.DeviceId = deviceId;
            this.Mode = mode;
            this.Type = type;
        }
    }
    #endregion
}
