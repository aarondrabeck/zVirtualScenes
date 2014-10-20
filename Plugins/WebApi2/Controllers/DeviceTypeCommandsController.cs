using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using zvs.DataModel;

namespace zvsWebapi2Plugin.Controllers
{
    [ODataRoutePrefix("DeviceTypeCommands")]
    [Authorize(Roles = "All Access")]
    public class DeviceTypeCommandsController : WebApi2PuginController
    {
        public DeviceTypeCommandsController(WebApi2Plugin webApi2Plugin) : base(webApi2Plugin) { }

        [ODataRoute]
        [EnableQuery(PageSize = 50)]
        public IQueryable<DeviceTypeCommand> Get()
        {
            return Context.DeviceTypeCommands;
        }

        protected override void Dispose(bool disposing)
        {
            Context.Dispose();
            base.Dispose(disposing);
        }

        [EnableQuery]
        public SingleResult<DeviceTypeCommand> Get([FromODataUri] long key)
        {
            var result = Get().Where(p => p.Id == key);
            return SingleResult.Create(result);
        }
    }
}