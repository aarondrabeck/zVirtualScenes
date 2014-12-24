using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using zvs.DataModel;

namespace zvsWebapi2Plugin.Controllers
{
    [ODataRoutePrefix("SceneStoredCommands")]
    [Authorize(Roles = "All Access")]
    public class SceneStoredCommandsController : WebApi2PuginController
    {
        public SceneStoredCommandsController(WebApi2Plugin webApi2Plugin) : base(webApi2Plugin) { }

        [ODataRoute]
        [EnableQuery(PageSize = 50)]
        public IQueryable<SceneStoredCommand> Get()
        {
            return Context.SceneStoredCommands;
        }

        protected override void Dispose(bool disposing)
        {
            Context.Dispose();
            base.Dispose(disposing);
        }

        [EnableQuery]
        public SingleResult<SceneStoredCommand> Get([FromODataUri] long key)
        {
            var result = Get().Where(p => p.Id == key);
            return SingleResult.Create(result);
        }
    }
}