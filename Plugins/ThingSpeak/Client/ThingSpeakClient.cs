using System;
using System.Net;
using System.Text;

namespace ThingSpeak.Client
{
    public class ThingSpeakClient
    {
        private string ApiKey { get; }
        private const string Url = "http://api.thingspeak.com/";

        public ThingSpeakClient(string apiKey)
        {
            ApiKey = apiKey;
        }

        public Boolean SendDataToThingSpeak(out Int16 tsResponse, params string[] fields)
        {
            var sbQs = new StringBuilder();

            // Build the querystring
            sbQs.Append(Url + "update?key=" + ApiKey);
            for (var x = 0; x < fields.Length; x++)
            {
                var index = x + 1;
                if (!string.IsNullOrEmpty(fields[x])) sbQs.Append("&field" + index + "=" + Uri.EscapeDataString(fields[x]));
            }

            // The response will be a "0" if there is an error or the entry_id if > 0
            tsResponse = Convert.ToInt16(PostToThingSpeak(sbQs.ToString()));

            return (tsResponse > 0);

        }

        public Boolean UpdateThingkSpeakStatus(string status, out Int16 tsResponse)
        {
            var sbQs = new StringBuilder();
            sbQs.Append(Url + "update?key=" + ApiKey + "&status=" + Uri.EscapeDataString(status));

            tsResponse = Convert.ToInt16(PostToThingSpeak(sbQs.ToString()));

            return (tsResponse > 0);
        }

        public Boolean UpdateThingSpeakLocation(string tsLat, string tsLong, string tsElevation, out Int16 tsResponse)
        {
            var sbQs = new StringBuilder();
            sbQs.Append(Url + "update?key=" + ApiKey);

            if (tsLat != null) sbQs.Append("&lat=" + tsLat);
            if (tsLong != null) sbQs.Append("&long=" + tsLong);
            if (tsElevation != null) sbQs.Append("&elevation=" + tsElevation);

            tsResponse = Convert.ToInt16(PostToThingSpeak(sbQs.ToString()));

            return (tsResponse > 0);
        }

        private string PostToThingSpeak(string queryString)
        {
            var wc = new WebClient();
            return wc.DownloadString(queryString);
        }
    }
}