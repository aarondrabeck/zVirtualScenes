using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Dependencies;
using WebHttpAPI.Controllers;

namespace WebHttpAPI.Configuration
{
    public class zvsDependencyResolver : IDependencyResolver
    {
        public IDependencyScope BeginScope()
        {
            // This example does not support child scopes, so we simply return 'this'.
            return this;
        }
        public zvs.Processor.Core Core { get; set; }
        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(zvs.Processor.Core))
            {
                return Core;
            }
            else
            {
                if (serviceType == typeof(DevicesController) || serviceType == typeof(ScenesController))
                {
                    zvsControllerBase c = (Activator.CreateInstance(serviceType) as zvsControllerBase);
                    if (c != null)
                    {
                        c.Core = Core;
                        return c;
                    }
                }
            }
            if(!serviceType.IsAbstract) return Activator.CreateInstance(serviceType);
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
