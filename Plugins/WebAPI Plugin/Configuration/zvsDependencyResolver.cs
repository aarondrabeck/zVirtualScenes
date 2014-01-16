using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Dependencies;
using WebAPI.Controllers;
using zvs.Processor.Logging;

namespace WebAPI.Configuration
{
    public class zvsDependencyResolver : IDependencyResolver
    {
        private ILog _log = zvs.Processor.Logging.LogManager.GetLogger<zvsDependencyResolver>();
        public IDependencyScope BeginScope()
        {
            // This example does not support child scopes, so we simply return 'this'.
            return this;
        }
        public WebAPIPlugin WebAPIPlugin { get; set; }

        public object GetService(Type serviceType)
        {
            //We will give each controller zvsAuthenticatedControllerBase based controller reference to the WebAPIPlugin 
            if (typeof (zvsController).IsAssignableFrom(serviceType))
            {
                var c = Activator.CreateInstance(serviceType, WebAPIPlugin);
                _log.InfoFormat("Resolved: {0} to: {1}", serviceType.FullName, c.GetType().FullName);
                return c;
            }
            return null;
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
