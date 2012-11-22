using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using zvs.Entities;

namespace WebAPI.Controllers
{
    public abstract class zvsAuthenticatedControllerBase : ApiController
    {
        protected zvsContext db = new zvsContext();

        protected bool isAuthorized
        {
            get
            {
                if (!Request.Headers.Contains("X-zvsToken"))
                    throw new HttpResponseException(Request.CreateResponse(System.Net.Http.ResponseStatus.Error, System.Net.HttpStatusCode.Unauthorized, "X-zvsToken invalid"));

                //TODO: Finish this up...we need the dependency resolver working again 

                //string tokens = zvsPlugin.GetSettingValue("TOKENS", db);
                //List<string> authorizedTokens = new List<string>(tokens.Split(','));

                //if (!authorizedTokens.Contains(Request.Headers.GetValues("X-zvsToken").First()))
                //    throw new HttpResponseException(Request.CreateResponse(System.Net.Http.ResponseStatus.Error, System.Net.HttpStatusCode.Unauthorized, "Unauthorized"));

                return true;
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
