using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.OData;
using zvs.Entities;
using zvs.Processor;
using zvs.Processor.Logging;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Provides access to the instance of WebAPIPlugin, zvsContext management, logging and DenyUnauthorized() method.
    /// </summary>
    public abstract class zvsController : ApiController
    {
        protected zvsContext db = new zvsContext();
        protected WebAPIPlugin WebAPIPlugin;
        protected ILog log = LogManager.GetLogger<zvsController>();

        public zvsController(WebAPIPlugin webAPIPlugin)
        {
            this.WebAPIPlugin = webAPIPlugin;

        }

        /// <summary>
        /// Throws HttpResponseException if not authorized.
        /// </summary>
        protected void DenyUnauthorized()
        {
            if (!Request.Headers.Contains("X-zvsToken"))
                throw new HttpResponseException(Request.CreateResponse(System.Net.Http.ResponseStatus.Error, System.Net.HttpStatusCode.Unauthorized, "X-zvsToken invalid"));

            string tokens = WebAPIPlugin.GetSettingValue("TOKENS", db);
            List<string> authorizedTokens = new List<string>(tokens.Split(','));
            authorizedTokens.ForEach(o => o = o.Trim());

            if (!authorizedTokens.Contains(Request.Headers.GetValues("X-zvsToken").First()))
                throw new HttpResponseException(Request.CreateResponse(System.Net.Http.ResponseStatus.Error, System.Net.HttpStatusCode.Unauthorized, "Unauthorized"));
        }

        /// <summary>
        /// Writes to the debug log with extra debug information about the web api client
        /// </summary>
        /// <param name="msg"></param>
        protected void LogDebugWebAPI(string msg)
        {
            string webClientInfo = string.Format("Request from:{0}, Controller:{1}, Method:{2}, Headers:{3}", GetClientIp(this.Request), this.GetType().Name, this.Request.Method.ToString(), Request.Headers.ToString());
            log.DebugFormat("{0} ({1})", msg, webClientInfo);
        }

        /// <summary>
        /// Writes to the info log with extra information about the web api client
        /// </summary>
        /// <param name="msg"></param>
        protected void LogInfoWebAPI(string msg)
        {
            string webClientInfo = string.Format("Request from:{0}, Controller:{1}", GetClientIp(this.Request), this.GetType().Name);
            log.InfoFormat("{0} ({1})", msg, webClientInfo);
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

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
