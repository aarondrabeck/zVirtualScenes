using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using zvs.Entities;

namespace zvsWebapi2Plugin.Controllers
{
    [ODataRoutePrefix("Scenes")]
    [Authorize(Roles = "All Access")]
    public class ScenesController : WebApi2PuginController
    {
        public ScenesController(WebApi2Plugin webApi2Plugin) : base(webApi2Plugin) { }

        [ODataRoute]
        [EnableQuery(PageSize = 50)]
        public IQueryable<Scene> Get()
        {
            return Context.Scenes;
        }

        protected override void Dispose(bool disposing)
        {
            Context.Dispose();
            base.Dispose(disposing);
        }

        [EnableQuery]
        public SingleResult<Scene> Get([FromODataUri] long key)
        {
            var result = Get().Where(p => p.Id == key);
            return SingleResult.Create(result);
        }
    }
}