using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Collections;
using System.Threading;
using System.Windows.Forms;

namespace zVirtualScenesApplication
{
    public class HttpProcessor
    {
        public TcpClient socket;
        public HttpServer srv;

        private Stream inputStream;
        public StreamWriter outputStream;

        public String http_method;
        public String http_url;
        public String http_protocol_versionstring;
        public Hashtable httpHeaders = new Hashtable();

        private zVirtualScenes mForm;

        public HttpProcessor(TcpClient s, HttpServer srv, zVirtualScenes Form1)
        {
            this.socket = s;
            this.srv = srv;
            this.mForm = Form1;
        }

        private string streamReadLine(Stream inputStream)
        {
            int next_char;
            string data = "";
            while (true)
            {
                next_char = inputStream.ReadByte();
                if (next_char == '\n') { break; }
                if (next_char == '\r') { continue; }
                if (next_char == -1) { Thread.Sleep(1); continue; };
                data += Convert.ToChar(next_char);
            }
            return data;
        }

        public void process()
        {
            inputStream = new BufferedStream(socket.GetStream());
            outputStream = new StreamWriter(new BufferedStream(socket.GetStream()));
            try
            {
                parseRequest();
            }
            catch (Exception e)
            {
                mForm.Invoke(mForm.DelegateAddLog, new Object[] { "HTTP Exception: " + e.ToString() });
            }
            outputStream.Flush();
            // bs.Flush(); // flush any remaining output
            inputStream = null; outputStream = null; // bs = null;            
            socket.Close();
        }

        public void parseRequest()
        {
            String request = streamReadLine(inputStream);
            string[] tokens = request.Split(' ');

            if (tokens.Length != 3)            
                mForm.Invoke(mForm.DelegateAddLog, new Object[] { "Invalid http request." });
            else
            {
                http_method = tokens[0].ToUpper();
                if (http_method == "GET")
                {
                    http_url = tokens[1];
                    outputStream.WriteLine("zVirtualScene recieved your command: {0} OK", http_url);
                    mForm.Invoke(mForm.DelegateAddLog, new Object[] { this.http_url });
                }
            }
        }
    }

}
