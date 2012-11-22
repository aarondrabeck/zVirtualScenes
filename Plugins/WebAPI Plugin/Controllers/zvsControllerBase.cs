using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WebAPI.DTO;
using zvs.Entities;
using zvs.Processor;
using zvs.Processor.Logging;


namespace WebAPI.Controllers
{
    public abstract class  zvsControllerBase<TEntity> : ApiController where TEntity : class, IIdentity, new()
    {
        protected zvsContext db = new zvsContext();
        ILog log = LogManager.GetLogger<zvsControllerBase<TEntity>>();
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



        /// <summary>
        /// <para>This is a standardized way to retrieve an entities collection. It is fully DataControl compliant.</para>
        /// 
        /// <para>To use it with WEB API call it from the derived class method adding the 
        /// [HttpGet] attribute. </para>
        /// 
        /// <para>Additionally you can add the [DTOQueryable] attribute to enable oData filters.</para>
        /// </summary>
        /// <returns></returns>
        protected IQueryable<TEntity> Get()
        {
            return BaseQueryable;
        }

        protected HttpResponseMessage GetById(int id)
        {
            TEntity tEntity = BaseQueryable.Where(o => o.Id == id).SingleOrDefault();
            DTOFactory<TEntity> dtoFactory = new DTOFactory<TEntity>(tEntity);
            return Request.CreateResponse(ResponseStatus.Success, HttpStatusCode.OK, "OK", dtoFactory.getDTO());
        }

        /// <summary>
        /// Defines your DBSet. It declares where TEntity can be added and removed.
        /// </summary>
        protected abstract DbSet DBSet { get; }

        /// <summary>
        /// Defines restrictions on the DBSet when getting objects. 
        /// </summary>
        protected virtual IQueryable<TEntity> BaseQueryable
        {
            get
            {
                return DBSet.OfType<TEntity>().AsQueryable();
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
