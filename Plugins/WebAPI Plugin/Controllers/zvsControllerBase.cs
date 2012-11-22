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
using WebAPI.DTO;
using zvs.Entities;
using zvs.Processor;
using zvs.Processor.Logging;


namespace WebAPI.Controllers
{
    public abstract class  zvsControllerBase<TEntity> : zvsAuthenticatedControllerBase 
        where TEntity : class, IIdentity, new()
    {
       
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
            bool isAuthorized = base.isAuthorized;
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
                bool isAuthorized = base.isAuthorized;
                return DBSet.OfType<TEntity>().AsQueryable();
            }
        }

        /// <summary>
        /// <para>This is a standardized way to update a single entity. It is fully DataControl compliant.</para>
        /// 
        /// <para>To use it with WEB API call it from the derived class method adding the 
        /// [HttpPut] and or [HttpPatch] attribute. </para> 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tEntityPatch"></param>
        /// <returns></returns>
        protected HttpResponseMessage Update(int id, Delta<TEntity> tEntityPatch)
        {
            bool isAuthorized = base.isAuthorized;

            if (tEntityPatch == null)
                return Request.CreateResponse(ResponseStatus.Error, HttpStatusCode.BadRequest, "DTO null");

            TEntity entity = BaseQueryable.Where(o => o.Id == id).SingleOrDefault();
            if (entity == null)
            {
                return Request.CreateResponse(ResponseStatus.Error, HttpStatusCode.NotFound, "Resource not found");
            }

            tEntityPatch.Patch(entity);
            db.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// <para>This is a standardized way to create a single entity. It is fully DataControl compliant.</para>
        /// 
        /// <para>To use it with WEB API call it from the derived class method adding the 
        /// [HttpPost] attribute. </para> 
        /// </summary>
        /// <param name="tEntity"></param>
        /// <returns></returns>
        protected HttpResponseMessage Add(TEntity tEntity)
        {
            bool isAuthorized = base.isAuthorized;

            if (tEntity == null)
                return Request.CreateResponse(ResponseStatus.Error, HttpStatusCode.BadRequest, "No data posted");
           
            DBSet.Add(tEntity);

            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                List<object> ValidationErrors = new List<object>();
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        ValidationErrors.Add(new
                        {
                            Property = validationError.PropertyName,
                            Error = validationError.ErrorMessage
                        });
                    }
                }

                Dictionary<string, object> ResponseDictionary = new Dictionary<string, object>();
                ResponseDictionary.Add("ValidationErrors", ValidationErrors);
                return Request.CreateResponse(ResponseStatus.Error, HttpStatusCode.BadRequest, "Bad Request", null, ResponseDictionary);
            }
            catch (Exception ex)
            {
                string ErrMsg = (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message)) ? ex.InnerException.Message : ex.Message;
                return Request.CreateResponse(ResponseStatus.Error, HttpStatusCode.BadRequest, "Validation Errors", new { ValidationErrors = ErrMsg });
            }

            DTOFactory<TEntity> dtoFactory = new DTOFactory<TEntity>(tEntity);
            return Request.CreateResponse(ResponseStatus.Success, HttpStatusCode.Created, "Created", dtoFactory.getDTO());
        }
        
        /// <summary>
        /// <para>This is a standardized way to delete a single entity. It is fully DataControl compliant.</para>
        /// 
        /// <para>To use it with WEB API call it from the derived class method adding the 
        /// [HttpPost] attribute. </para>  
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected HttpResponseMessage Remove(int id)
        {
            bool isAuthorized = base.isAuthorized;

            TEntity entity = BaseQueryable.Where(o => o.Id == id).SingleOrDefault();
            if (entity == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            DBSet.Remove(entity);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Request.CreateResponse(ResponseStatus.Error, HttpStatusCode.NotFound, "Not Found");
            }

            return Request.CreateResponse(ResponseStatus.Success, HttpStatusCode.OK, "OK");
        }

        /// <summary>
        /// <para>Given an object, this method attempts to find a ObservableCollection with the name of the passed propertyName and return it as a IQueryable.</para>
        /// 
        /// <para>If found, it then does permission checking via datacontrol, finds the appropriate DTO, and returns an IQueryable of GenericType(propertyName).</para>
        /// 
        /// <exception cref="HttpResponseException">Invalid property name</exception>
        /// <exception cref="HttpResponseException">Bad Request</exception>
        /// <exception cref="HttpResponseException">Not Found</exception>
        /// <exception cref="HttpResponseException">No suitable DTO found</exception>
        /// <exception cref="HttpResponseException">More than one DTO of this type found</exception>
        /// <exception cref="HttpResponseException">You do not have access to this resource</exception>
        /// 
        /// </summary>
        /// <param name="baseObject"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected IQueryable<object> GetNestedCollections(int id, string propertyName)
        {
            bool isAuthorized = base.isAuthorized;
            TEntity tEntity = BaseQueryable.Where(o => o.Id == id).SingleOrDefault();

            if (string.IsNullOrEmpty(propertyName))
                throw new HttpResponseException(Request.CreateResponse(ResponseStatus.Error, HttpStatusCode.BadRequest, "Invalid property name"));

            if (tEntity == null)
                throw new HttpResponseException(Request.CreateResponse(ResponseStatus.Error, HttpStatusCode.BadRequest, "Bad Request"));

            //Find the property that matches the nestedCollectionName
            var Property = tEntity.GetType().GetProperties().SingleOrDefault(o =>
                o.PropertyType.IsClass &&
                o.PropertyType.IsPublic &&
                !o.PropertyType.IsAbstract &&
                typeof(ICollection).IsAssignableFrom(o.PropertyType) &&
                o.Name.ToLower().Equals(propertyName.ToLower()));

            if (Property == null)
                throw new HttpResponseException(Request.CreateResponse(ResponseStatus.Error, HttpStatusCode.NotFound, "Not Found"));

            //Get property value
            var PropertyValue = Property.GetValue(tEntity);

            //Get the GenericType of the collection EXAMPLE:  Collection<somegenerictype>
            var PropertyGenericTypeArg = Property.PropertyType.GenericTypeArguments.FirstOrDefault();
            if (PropertyGenericTypeArg == null)
                throw new HttpResponseException(Request.CreateResponse(ResponseStatus.Error, HttpStatusCode.NotFound, "Not Found"));

            //Get all members ofType<typeof(nestedCollectionName)> ex. person.Roles.OfType<EmployeeRole>().
            var EnumerableOfType = typeof(Enumerable)
                .GetMethod("OfType")
                .MakeGenericMethod(new Type[] { PropertyGenericTypeArg })
                .Invoke(null, new object[] { PropertyValue });

            //Cast Property ex. Person.Permissions to IQueryable<Permission>
            var queryable = typeof(Queryable)
                .GetMethod("AsQueryable", BindingFlags.Static | BindingFlags.Public, null, new[] { EnumerableOfType.GetType() }, null)
                .Invoke(null, new object[] { EnumerableOfType });

            return (IQueryable<object>)queryable;
        }
        
    }
}
