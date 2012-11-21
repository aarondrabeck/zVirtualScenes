using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using zvs.Processor;
using zvs.Processor.Logging;


namespace WebHttpAPI.Controllers
{
    public class zvsControllerBase : ApiController
    {
        ILog log = LogManager.GetLogger<zvsControllerBase>();
        public Core Core { get; set; }

        protected void Log(ILog Logger, params object[] data)
        {
            string msg = string.Format("Incoming Request, From:{0}, Controller:{1}, Method:{2}, Headers:{3}", GetClientIp(this.Request), this.GetType().Name, this.Request.Method.ToString(), Request.Headers.ToString());
            if (data != null)
            {
                foreach (var d in data)
                {
                    msg += d.ToString();
                }
            }
            Logger.Info(msg);
        }
        private object GetClientVar(HttpRequestMessage request, string var)
        {
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            else if (request.Properties.ContainsKey(var))
            {
                return request.Properties[RemoteEndpointMessageProperty.Name];
            }
            else
            {
                return null;
            }
        }
        private string GetClientIp(HttpRequestMessage request)
        {
            var prop = GetClientVar(request, RemoteEndpointMessageProperty.Name);
            if (prop != null)
            {
                return (prop as RemoteEndpointMessageProperty).Address;
            }
            return "";
        }
    }
}
