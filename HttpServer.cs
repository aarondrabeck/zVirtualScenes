using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using System.Net;

namespace zVirtualScenesApplication
{
    public class HttpServer
    {
        protected int port;
        TcpListener listener;
        private volatile bool is_active = true;
        private formzVirtualScenes zVirtualScenesMain;

        public void RequestStop()
        {
            is_active = false;
            listener.Server.Close();
            listener.Stop();
            
        }

        public HttpServer(int port, formzVirtualScenes Form)
        {
            this.port = port;
            this.zVirtualScenesMain = Form;
        }
        
        public void listen()
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            while (is_active)
            {
                try
                {
                    TcpClient s = listener.AcceptTcpClient();
                    HttpProcessor processor = new HttpProcessor(s, this, zVirtualScenesMain);
                    Thread thread = new Thread(new ThreadStart(processor.process));
                    thread.Start();
                    Thread.Sleep(1);
                }
                catch { }
            }
            zVirtualScenesMain.LogThis(1, "HTTP Interface: SHUTDOWN.");
        }
               
    }

}
