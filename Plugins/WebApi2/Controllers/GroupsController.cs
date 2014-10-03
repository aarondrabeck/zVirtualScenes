using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using zvs.Entities;

namespace zvsWebapi2Plugin.Controllers
{
    [ODataRoutePrefix("Groups")]
    [Authorize(Roles = "All Access")]
    public class GroupsController : WebApi2PuginController
    {
        public GroupsController(WebApi2Plugin webApi2Plugin) : base(webApi2Plugin) { }

        [ODataRoute]
        [EnableQuery(PageSize = 50)]
        public IQueryable<Group> Get()
        {
            return Context.Groups;
        }

        protected override void Dispose(bool disposing)
        {
            Context.Dispose();
            base.Dispose(disposing);
        }

        [EnableQuery]
        public SingleResult<Group> Get([FromODataUri] long key)
        {
            var result = Get().Where(p => p.Id == key);
            return SingleResult.Create(result);
        }
    }
}