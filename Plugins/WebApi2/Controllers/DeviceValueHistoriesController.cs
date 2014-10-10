using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using zvs.Entities;

namespace zvsWebapi2Plugin.Controllers
{
    [ODataRoutePrefix("DeviceValueHistories")]
    [Authorize(Roles = "All Access")]
    public class DeviceValueHistoriesController : WebApi2PuginController
    {
        public DeviceValueHistoriesController(WebApi2Plugin webApi2Plugin) : base(webApi2Plugin) { }

        [ODataRoute]
        [EnableQuery(PageSize = 50)]
        public IQueryable<DeviceValueHistory> Get()
        {
            return Context.DeviceValueHistories;
        }

        protected override void Dispose(bool disposing)
        {
            Context.Dispose();
            base.Dispose(disposing);
        }

        [EnableQuery]
        public SingleResult<DeviceValueHistory> Get([FromODataUri] long key)
        {
            var result = Get().Where(p => p.Id == key);
            return SingleResult.Create(result);
        }
    }
}