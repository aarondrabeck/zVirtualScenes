namespace MiLight.Net.WifiCintroller
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    using MiLight.Net.Contracts;

    public class WifiController : IController
    {
        private static readonly ManualResetEvent connectDone = new ManualResetEvent(false);
        private static readonly ManualResetEvent sendDone = new ManualResetEvent(false);

        private readonly string ip;

        private readonly int port;

        public string IPAddress { get; private set; }
        public WifiController(string ip, int port = 8899)
        {
            this.ip = ip;
            this.IPAddress = ip;
            this.port = port;
        }

        /// <summary>
        /// Sends the command to the wifi controller, also has a 100 millisecond delay to ensure commands are not lost
        /// </summary>
        /// <param name="command">The command to control the lights</param>
        public async Task Send(byte[] command)
        {
            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            var serverAddr = System.Net.IPAddress.Parse(this.ip);

            var endPoint = new IPEndPoint(serverAddr, this.port);

            sock.BeginConnect(endPoint, ConnectCallback, sock);
            connectDone.WaitOne();

            sock.BeginSend(command, 0, command.Length, 0, SendCallback, sock);

            sendDone.WaitOne();
            //sock.SendTo(command, endPoint);

            System.Threading.Thread.Sleep(100);
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            var client = (Socket)ar.AsyncState;

            client.EndConnect(ar);
            connectDone.Set();
        }

        private static void SendCallback(IAsyncResult ar)
        {
            var client = (Socket)ar.AsyncState;

            client.EndSend(ar);

            sendDone.Set();
        }
    }
}