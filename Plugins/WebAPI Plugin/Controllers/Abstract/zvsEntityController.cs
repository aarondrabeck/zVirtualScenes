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
    /// <summary>
    /// <para>Wraps up Get, GetById, Add, Update and Remove into one abstract controller.</para>
    /// 
    /// <para>IMPORTNAT: You must choose which methods you would like to expose in the derived class and call the base method.</para>
    /// </summary>
    /// <typeparam name="TEntity">Entity that abides by the IIdentity interface.</typeparam>
    public abstract class zvsEntityController<TEntity> : zvsController 
        where TEntity : class, IIdentity, new()
    {
        public zvsEntityController(WebAPIPlugin webAPIPlugin) : base(webAPIPlugin) { }        

        /// <summary>
        /// <para>This is a standardized way to retrieve an entities collection. It is fully DataControl compliant.</para>
        /// 
        /// <para>To use it with WEB API call it from the derived class method adding the 
        /// [HttpGet] attribute. </para>
        /// 
        /// <para>Additionally you can add the [DTOQueryable(PageSize = 100)] attribute to enable oData filters.</para>
        /// </summary>
        /// <returns></returns>
        protected Task<IQueryable<TEntity>> Get()
        {
            DenyUnauthorized();

            return Task.FromResult(BaseQueryable);
        }

        /// <summary>
        /// Defines your DBSet. It declares where TEntity can be added and removed.
        /// </summary>
        protected virtual DbSet DBSet
        {
            get
            {
                return db.Set<TEntity>();
            }
        }

        protected async Task<HttpResponseMessage> GetByIdAsync(int id)
        {
            DenyUnauthorized();

            TEntity tEntity = await BaseQueryable.SingleOrDefaultAsync(o => o.Id == id);
            if (tEntity == null)
                throw new HttpResponseException(Request.CreateResponse(ResponseStatus.Error, HttpStatusCode.NotFound, "Resource not found"));

            return Request.CreateResponse(ResponseStatus.Success, HttpStatusCode.OK, "OK", DTOFactory.GetDTO(tEntity));
        }

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

        /// <summary>
        /// <para>This is a standardized way to update a single entity. It is fully DataControl compliant.</para>
        /// 
        /// <para>To use it with WEB API call it from the derived class method adding the 
        /// [HttpPut] and or [HttpPatch] attribute. </para> 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tEntityPatch"></param>
        /// <returns></returns>
        protected virtual async Task<HttpResponseMessage> UpdateAsync(int id, Dictionary<string, object> partialObject)
        {
            DenyUnauthorized();

            if (partialObject == null)
                return Request.CreateResponse(ResponseStatus.Error, HttpStatusCode.BadRequest, "DTO null or has errors");

            TEntity entity = await BaseQueryable.Where(o => o.Id == id).SingleOrDefaultAsync();
            if (entity == null)
                return Request.CreateResponse(ResponseStatus.Error, HttpStatusCode.NotFound, "Resource not found");

            ObjectPatcher<TEntity> patcher = new ObjectPatcher<TEntity>(entity, partialObject);

            if (patcher.InvalidPropertyNames.Count() > 0)
            {
                var error = string.Format("The following properties are invalid: {0}", String.Join(", ", string.Format("'{0}'", patcher.InvalidPropertyNames)));
                return Request.CreateResponse(ResponseStatus.Error, HttpStatusCode.BadRequest, error);
            }

            if (patcher.InvalidPropertyValues.Count > 0)
            {
                var error = string.Format("There are {0} invalid property values. See data for listing.", patcher.InvalidPropertyValues.Count);
                return Request.CreateResponse(ResponseStatus.Error, HttpStatusCode.BadRequest, error, patcher.InvalidPropertyValues);
            }

            patcher.Patch();
            BeforeUpdateSaveChanges(entity);
            await SaveChangesAsync(db);

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        protected virtual void BeforeUpdateSaveChanges(TEntity entity) { }

        /// <summary>
        /// <para>This is a standardized way to create a single entity. It is fully DataControl compliant.</para>
        /// 
        /// <para>To use it with WEB API call it from the derived class method adding the 
        /// [HttpPost] attribute. </para> 
        /// </summary>
        /// <param name="tEntity"></param>
        /// <returns></returns>
        protected async Task<HttpResponseMessage> AddAsync(TEntity tEntity)
        {
            DenyUnauthorized();

            if (tEntity == null)
                return Request.CreateResponse(ResponseStatus.Error, HttpStatusCode.BadRequest, "No data posted");
           
            DBSet.Add(tEntity);

            await SaveChangesAsync(db);
            
            return Request.CreateResponse(ResponseStatus.Success, HttpStatusCode.Created, "Created", DTOFactory.GetDTO(tEntity));
        }

        protected async Task SaveChangesAsync(DbContext context)
        {
            try
            {
                await context.SaveChangesAsync();
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
                throw new HttpResponseException(Request.CreateResponse(ResponseStatus.Error, HttpStatusCode.BadRequest, "Bad Request", null, ResponseDictionary));
            }
            catch (Exception ex)
            {
                string ErrMsg = (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message)) ? ex.InnerException.Message : ex.Message;
                throw new HttpResponseException(Request.CreateResponse(ResponseStatus.Error, HttpStatusCode.BadRequest, "Validation Errors", new { ValidationErrors = ErrMsg }));
            }
        }
        
        /// <summary>
        /// <para>This is a standardized way to delete a single entity. It is fully DataControl compliant.</para>
        /// 
        /// <para>To use it with WEB API call it from the derived class method adding the 
        /// [HttpPost] attribute. </para>  
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected async Task<HttpResponseMessage> RemoveAsync(int id)
        {
            DenyUnauthorized();

            TEntity entity = await BaseQueryable.Where(o => o.Id == id).SingleOrDefaultAsync();
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

        private Task<IQueryable<object>> FindCollectionPropertyAsync(TEntity entity, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new HttpResponseException(Request.CreateResponse(ResponseStatus.Error, HttpStatusCode.BadRequest, "Invalid property name"));

            if (entity == null)
                throw new HttpResponseException(Request.CreateResponse(ResponseStatus.Error, HttpStatusCode.BadRequest, "Bad Request"));

            //Let the reflection begin...

            //Find the property that matches the nestedCollectionName
            var Property = entity.GetType().GetProperties().SingleOrDefault(o =>
                o.PropertyType.IsClass &&
                o.PropertyType.IsPublic &&
                !o.PropertyType.IsAbstract &&
                typeof(ICollection).IsAssignableFrom(o.PropertyType) &&
                o.Name.ToLower().Equals(propertyName.ToLower()));

            if (Property == null)
                throw new HttpResponseException(Request.CreateResponse(ResponseStatus.Error, HttpStatusCode.NotFound, "Not Found"));

            //Get property value
            var PropertyValue = Property.GetValue(entity);

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

            return Task.FromResult((IQueryable<object>)queryable);
        }

        /// <summary>
        /// <para>This method attempts to find the nestedCollection on the base entity</para>
        /// 
        /// <para>This method checks the create data control for the nestedCollections generic type.</para>
        /// 
        /// <para>Use with this route. </para>
        /// <c>
        ///  config.Routes.MapHttpRoute(ODataRouteNames.PropertyNavigation,
        ///       routeTemplate: "Core/{controller}/{parentId}/{nestedCollectionName}",
        ///       defaults: new { id = RouteParameter.Optional },
        ///       constraints: new { namespacing = new ControllerNamespacingConstraint("LeavittWebApi.Controllers.Core") }
        ///    );
        ///    </c>
        /// 
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="nestedCollectionName"></param>
        /// <returns></returns>
        protected async Task<IQueryable<object>> GetNestedCollectionsAsync(Int64 parentId, string nestedCollectionName)
        {
            DenyUnauthorized();
            TEntity tEntity = null;

            if (BaseQueryable is IDbAsyncQueryProvider)
            {
                //if isRunning = true is sent, lets start the scene.
                tEntity = await BaseQueryable.SingleOrDefaultAsync(o => o.Id == parentId);
            }
            else
            {
                tEntity = BaseQueryable.SingleOrDefault(o => o.Id == parentId);
            }

            if (tEntity == null)
                throw new HttpResponseException(Request.CreateResponse(ResponseStatus.Error, HttpStatusCode.NotFound, "Resource not found"));

            return await FindCollectionPropertyAsync(tEntity, nestedCollectionName);
        }
    }
}
