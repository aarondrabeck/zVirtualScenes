using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Mvc;
using WebAPI.DTO;
using zvs.Entities;

namespace WebAPI
{
    /// <summary>
    /// Allows the use of oData queries and return data in the LG standardized format.
    /// </summary>
    public class DTOQueryable : QueryableAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            //Ignore CORS OPTION requests and others
            if (actionExecutedContext.Request.Method != HttpMethod.Get)
                return;

            base.OnActionExecuted(actionExecutedContext);

            if (actionExecutedContext.Response == null)
                return;

            //Check for errors
            IQueryable queryable = null;
            var objectContent = actionExecutedContext.Response.Content as ObjectContent;

            if (objectContent == null)
                return;

            if (objectContent.ObjectType == typeof(HttpError)
                || !actionExecutedContext.Response.IsSuccessStatusCode
                || !actionExecutedContext.Response.TryGetContentValue<IQueryable>(out queryable))
            {
                string ErrorMsg = string.Empty;

                //We need to convert errors to be printed via the standard
                if (objectContent.ObjectType == typeof(HttpError))
                {
                    var value = (HttpError)objectContent.Value;
                    if (value.ContainsKey("Message"))
                        ErrorMsg = value["Message"].ToString();
                }

                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(ResponseStatus.Error, actionExecutedContext.Response.StatusCode, ErrorMsg);
                actionExecutedContext.Response.StatusCode = HttpStatusCode.BadRequest;
                return;
            }

            if (!(queryable is IQueryable<IIdentity>))
            {
                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(ResponseStatus.Error, actionExecutedContext.Response.StatusCode, "Queryable generic type does not have a generic type of IIdentity");
                actionExecutedContext.Response.StatusCode = HttpStatusCode.BadRequest;
                return;
            }

            var lgQueryable = queryable as IQueryable<IIdentity>;

            Dictionary<string, object> additionalProperties = new Dictionary<string, object>();
            var inlineCount = actionExecutedContext.Request.GetInlineCount();
            if (inlineCount != null)
                additionalProperties.Add("Count", inlineCount);

            var nextPageLink = actionExecutedContext.Request.GetNextPageLink();
            if (nextPageLink != null)
                additionalProperties.Add("Next", nextPageLink);

            //showing
            additionalProperties.Add("Showing", lgQueryable.Count());

            var respose = actionExecutedContext.Request.CreateResponse(
                actionExecutedContext.Response.IsSuccessStatusCode ? ResponseStatus.Success : ResponseStatus.Error,
                actionExecutedContext.Response.StatusCode,
                actionExecutedContext.Response.StatusCode.ToString(),
                DTOFactory.GetDTO(lgQueryable.AsEnumerable()),
                additionalProperties);

            actionExecutedContext.Response.Content = respose.Content;
        }

        private Type GetObjType(object obj)
        {
            //Check for entity types here as they get wrapped in another dynamic type
            if (obj.GetType().Namespace != null && obj.GetType().Namespace.StartsWith("System.Data.Entity.Dynamic"))
                return obj.GetType().BaseType;
            else
                return obj.GetType();
        }
    }
}
