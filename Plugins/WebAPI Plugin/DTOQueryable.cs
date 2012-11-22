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

namespace WebAPI
{
    /// <summary>
    /// Allows the use of oData queries and return data in the LG standardized format.
    /// </summary>
    public class DTOQueryable : QueryableAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Response.Content != null)
                base.OnActionExecuted(actionExecutedContext);

            var objectContent = actionExecutedContext.Response.Content as ObjectContent;

            if (objectContent != null)
            {
                var type = objectContent.ObjectType;

                //We need to convert errors to be printed via the LG standard
                if (type == typeof(HttpError))
                {
                    var value = (HttpError)objectContent.Value;
                    string ErrorMsg = string.Empty;
                    if (value.ContainsKey("Message"))
                        ErrorMsg = value["Message"].ToString();
                    actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(ResponseStatus.Error, actionExecutedContext.Response.StatusCode, ErrorMsg);
                }

                int limit = 100;
                //We need to convert non-errors (iqueryable) to use the LG standard way to print a response
                if (type.Name.StartsWith("IQueryable"))
                {
                    Dictionary<string, object> additionalProperties = new Dictionary<string, object>();

                    var queryable = (IQueryable<object>)objectContent.Value;
                    int count = queryable.Count();


                    #region Send amount skipped back to the user
                    //A bit lame string parsing the queryable here, however it is effective.

                    int skipValue = 0;
                    var expression = (Expression)queryable.Expression;
                    int skipStartPos = expression.ToString().IndexOf(".Skip(");
                    if (skipStartPos > 0)
                    {
                        var indexOfEndSkip = expression.ToString().Substring(skipStartPos + 6).IndexOf(")");
                        var skipValueStr = expression.ToString().Substring(skipStartPos + 6, indexOfEndSkip);
                        int.TryParse(skipValueStr, out skipValue);
                    }
                    additionalProperties.Add("Skipped", skipValue);
                    #endregion

                    #region Send amount of More results back to the user
                    if (count > limit) //limit the max results globally
                    {
                        queryable = queryable.Take(limit);
                        additionalProperties.Add("More", count - queryable.Count());
                    }
                    else
                        additionalProperties.Add("More", 0);
                    #endregion

                    //showing
                    additionalProperties.Add("Showing", queryable.Count());

                    //Convert IQueryable<object> to List<objecDTO>
                    List<object> DTOS = new List<object>();
                    queryable.ToList().ForEach(o =>
                    {
                        var EntityType = GetObjType(o);

                        //DTOFactory<TEntity> dtoFactory = new DTOFactory<TEntity>(tEntity);
                        var dtoFactory = Activator.CreateInstance(typeof(DTOFactory<>).MakeGenericType(EntityType), new[] { o });

                        var dto = dtoFactory.GetType()
                        .GetMethod("getDTO")
                        .Invoke(dtoFactory, null);

                        DTOS.Add(dto);
                    });

                    var respose = actionExecutedContext.Request.CreateResponse(
                        actionExecutedContext.Response.IsSuccessStatusCode ? ResponseStatus.Success : ResponseStatus.Error,
                        actionExecutedContext.Response.StatusCode,
                        actionExecutedContext.Response.StatusCode.ToString(),
                        DTOS,
                        additionalProperties);

                    actionExecutedContext.Response.StatusCode = respose.StatusCode;
                    actionExecutedContext.Response.Content = respose.Content;

                }
            }
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
