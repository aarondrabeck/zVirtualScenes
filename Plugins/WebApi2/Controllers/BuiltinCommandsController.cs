using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.OData;
using System.Web.OData.Routing;
using zvs.DataModel;
using zvs.Processor;

namespace zvsWebapi2Plugin.Controllers
{
    [ODataRoutePrefix("BuiltinCommands")]
    [Authorize(Roles = "All Access")]
    public class BuiltinCommandsController : WebApi2PuginController
    {
        public BuiltinCommandsController(WebApi2Plugin webApi2Plugin) : base(webApi2Plugin) { }

        [ODataRoute]
        [EnableQuery(PageSize = 50)]
        public IQueryable<BuiltinCommand> Get()
        {
            return Context.BuiltinCommands;
        }

        protected override void Dispose(bool disposing)
        {
            Context.Dispose();
            base.Dispose(disposing);
        }

        [EnableQuery]
        public SingleResult<BuiltinCommand> Get([FromODataUri] long key)
        {
            var result = Get().Where(p => p.Id == key);
            return SingleResult.Create(result);
        }
    }
}