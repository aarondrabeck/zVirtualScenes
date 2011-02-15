using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace zVirtualScenesApplication
{
    
    
    public class GlobalFunctions
    {
        public RichTextBox log = new RichTextBox();
        public string ZCommanderIP { get; set; }


        /// <summary>
        /// Logs an event.
        /// </summary>
        /// <param name="type">1 = INFO, 2 = ERROR, DEFAULT INFO</param>
        /// <param name="message">Log Event Message</param>
        public void AddLog(int type, string message)
        {
            string datetime = DateTime.Now.ToString("s");

            string typename;
            if (type == 2)
                typename = "[ERROR]";
            else
                typename = "[INFO ]";
            
            log.Text += datetime + " " + typename + " - " +  message + "\n"; 
        }

        public string GetZComURL(Settings _settings)
        {
            return "http://" + _settings.ZcomIP + ":" + _settings.ZcomPort + "/ZwaveCommand?";
        }            

        /// <summary>
        /// SEND HTTP 
        /// </summary>
        /// <param name="URL">URL as string</param>
        /// <returns>HTML PAGE</returns>
        public string HTTPSend(string URL)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "GET";
            request.Timeout = 30000;

            string str = string.Empty;
            try
            {
                WebResponse response = request.GetResponse();
                str = new StreamReader(response.GetResponseStream(), System.Text.Encoding.Default).ReadToEnd();
                response.Close();
            }
            catch (Exception ex)
            {
                AddLog(2, "Exception occured: " + ex.Message);
            }
            return str;
        }
    }
}