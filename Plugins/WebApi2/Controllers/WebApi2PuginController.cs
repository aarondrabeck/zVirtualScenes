using System.Web.OData;
using zvs.Entities;

namespace zvsWebapi2Plugin.Controllers
{
    public abstract class WebApi2PuginController: ODataController
    {
        protected zvsContext Context { get; private set; }
        protected WebApi2Plugin WebApi2Plugin {get; private set; }

        protected WebApi2PuginController(WebApi2Plugin webApi2Plugin)
        {
            Context = new zvsContext();
            WebApi2Plugin = webApi2Plugin;
        }

        protected override void Dispose(bool disposing)
        {
            Context.Dispose();
            base.Dispose(disposing);
        }
    }
}
