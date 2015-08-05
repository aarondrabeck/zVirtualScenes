using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace MiLightAdapter.MiLight
{
    public class WifiController : IController
    {
        private static readonly ManualResetEvent ConnectDone = new ManualResetEvent(false);
        private static readonly ManualResetEvent SendDone = new ManualResetEvent(false);
        private  string Ip { get; }
        private  int Port { get; }

        public string IpAddress { get; private set; }
        public WifiController(string ip, int port = 8899)
        {
            Ip = ip;
            IpAddress = ip;
            Port = port;
        }

        /// <summary>
        /// Sends the command to the wifi controller, also has a 100 millisecond delay to ensure commands are not lost
        /// </summary>
        /// <param name="command">The command to control the lights</param>
        public async Task Send(byte[] command)
        {
            //TODO:  USE ASYNC COMMANDS
            // ConnectAsync and SendAsync

            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            var serverAddr = IPAddress.Parse(Ip);
            var endPoint = new IPEndPoint(serverAddr, Port);
            sock.BeginConnect(endPoint, ConnectCallback, sock);
            ConnectDone.WaitOne();

            sock.BeginSend(command, 0, command.Length, 0, SendCallback, sock);

            SendDone.WaitOne();
            //sock.SendTo(command, endPoint);
            await Task.Delay(100);
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            var client = (Socket)ar.AsyncState;
            client.EndConnect(ar);
            ConnectDone.Set();
        }

        private static void SendCallback(IAsyncResult ar)
        {
            var client = (Socket)ar.AsyncState;
            client.EndSend(ar);
            SendDone.Set();
        }
    }
}