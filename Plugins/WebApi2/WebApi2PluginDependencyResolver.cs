using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using zvsWebapi2Plugin.Controllers;

namespace zvsWebapi2Plugin
{
    public class WebApi2PluginDependencyResolver : IDependencyResolver
    {
        public WebApi2PluginDependencyResolver(WebApi2Plugin webApi2Plugin)
        {
            WebApi2Plugin = webApi2Plugin;
        }

        public IDependencyScope BeginScope()
        {
            // This example does not support child scopes, so we simply return 'this'.
            return this;
        }

        private WebApi2Plugin WebApi2Plugin { get; set; }

        public object GetService(Type serviceType)
        {
            //We will give each controller zvsAuthenticatedControllerBase based controller reference to the WebAPIPlugin 
            if (!typeof(WebApi2PuginController).IsAssignableFrom(serviceType)) return null;
            var c = Activator.CreateInstance(serviceType, WebApi2Plugin);
            return c;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return new List<object>();
        }

        public void Dispose()
        {
            // When BeginScope returns 'this', the Dispose method must be a no-op.
        }
    }
}
