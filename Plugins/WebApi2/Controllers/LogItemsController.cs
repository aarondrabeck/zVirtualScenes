using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using zvs.DataModel;
using zvs.Processor.Logging;

namespace zvsWebapi2Plugin.Controllers
{
    [ODataRoutePrefix("LogItems")]
    [Authorize(Roles = "All Access")]
    public class LogItemsController : WebApi2PuginController
    {
        public LogItemsController(WebApi2Plugin webApi2Plugin) : base(webApi2Plugin) { }

        [ODataRoute]
        [EnableQuery(PageSize = 50)]
        public IQueryable<LogItem> Get()
        {
            return EventedLog.Items.AsQueryable();
        }

        protected override void Dispose(bool disposing)
        {
            Context.Dispose();
            base.Dispose(disposing);
        }

        [EnableQuery]
        public SingleResult<LogItem> Get([FromODataUri] long key)
        {
            var result = Get().Where(p => p.Id == key);
            return SingleResult.Create(result);
        }
    }
}