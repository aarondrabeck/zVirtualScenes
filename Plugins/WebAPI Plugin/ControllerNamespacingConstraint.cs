using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebAPI
{
    /// <summary>
    /// <para>This is alternative way to filter by namespace since the default method is broken.</para>
    /// 
    ///  <para>The provided namespace control was broken so I devised this method. We can look at removing when Web API fixes.</para>
    /// <c>
    ///  config.Routes.MapHttpRoute("Core", 
    ///               routeTemplate: "Core/{controller}/{id}",
    ///               defaults: new { id = RouteParameter.Optional },
    ///               constraints: null,
    /// BROKEN!! -->  namespaces: new[] { "something.Private" });
    /// </c>
    /// </summary>
    public class ControllerNamespacingConstraint : IRouteConstraint, IHttpRouteConstraint
    {
        private string Namespace;
        /// <summary>
        /// <para>
        /// This forces the controller to belong to a specified namespace.
        /// 
        /// Pass in the namespace in which the controller must belong.
        /// </para>
        /// </summary>
        /// <param name="namespace"></param>
        public ControllerNamespacingConstraint(string @namespace)
        {
            Namespace = @namespace;
        }

        private static List<Type> GetSubClasses<T>(string @namespace)
        {
            return Assembly.GetCallingAssembly()
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(T)) && t.Namespace == @namespace && !t.IsAbstract)
                .ToList();
        }

        public List<string> GetControllerNames()
        {
            List<string> controllerNames = new List<string>();
            GetSubClasses<ApiController>(Namespace).ForEach(
                type => controllerNames.Add(type.Name.ToLower()));
            return controllerNames;
        }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (values.ContainsKey("Controller"))
            {
                string ControllerName = (string)values["Controller"];
                return GetControllerNames().Contains(ControllerName.ToLower() + "controller");
            }

            return false;
        }

        public bool Match(System.Net.Http.HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            if (values.ContainsKey("Controller"))
            {
                string ControllerName = (string)values["Controller"];
                return GetControllerNames().Contains(ControllerName.ToLower() + "controller");
            }

            return false;
        }
    }
}