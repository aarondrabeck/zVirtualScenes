using System;
using System.Data;
using System.Configuration;
using System.Text;
using System.Net;
using System.IO;

namespace ThingSpeak
{
    public class ThingSpeakClient
    {
        public string APIKey { get; set; }
        private const string _url = "http://api.thingspeak.com/";
        
        zvs.Processor.Logging.ILog log = zvs.Processor.Logging.LogManager.GetLogger<ThingSpeakClient>();


        public Boolean SendDataToThingSpeak(out Int16 TSResponse, params string[] fields)
        {
            StringBuilder sbQS = new StringBuilder();

            // Build the querystring
            sbQS.Append(_url + "update?key=" + APIKey);
            for (int x = 0; x < fields.Length; x++)
            {
                int index = x + 1;
                if (!string.IsNullOrEmpty(fields[x])) sbQS.Append("&field" + index + "=" + System.Uri.EscapeDataString(fields[x]));

            }

            // The response will be a "0" if there is an error or the entry_id if > 0
            TSResponse = Convert.ToInt16(PostToThingSpeak(sbQS.ToString()));

            return (TSResponse > 0);

        }

        public Boolean UpdateThingkSpeakStatus(string status, out Int16 TSResponse)
        {
            StringBuilder sbQS = new StringBuilder();
            sbQS.Append(_url + "update?key=" + APIKey + "&status=" + System.Uri.EscapeDataString(status));

            TSResponse = Convert.ToInt16(PostToThingSpeak(sbQS.ToString()));

            return (TSResponse > 0);
        }

        public Boolean UpdateThingSpeakLocation(string TSLat, string TSLong, string TSElevation, out Int16 TSResponse)
        {
            StringBuilder sbQS = new StringBuilder();
            sbQS.Append(_url + "update?key=" + APIKey);

            if (TSLat != null) sbQS.Append("&lat=" + TSLat);
            if (TSLong != null) sbQS.Append("&long=" + TSLong);
            if (TSElevation != null) sbQS.Append("&elevation=" + TSElevation);

            TSResponse = Convert.ToInt16(PostToThingSpeak(sbQS.ToString()));

            return (TSResponse > 0);
        }

        private string PostToThingSpeak(string QueryString)
        {
            var wc = new WebClient();
            return wc.DownloadString(QueryString);
        }
    }
}