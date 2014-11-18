using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using zvs.DataModel;
using zvs.Processor;

namespace zvsWebapi2Plugin.Controllers
{
    [ODataRoutePrefix("Commands")]
    [Authorize(Roles = "All Access")]
    public class CommandsController : WebApi2PuginController
    {
        public CommandsController(WebApi2Plugin webApi2Plugin) : base(webApi2Plugin) { }

        [ODataRoute]
        [EnableQuery(PageSize = 50)]
        public IQueryable<Command> Get()
        {
            return Context.Commands.AsQueryable();
        }

        protected override void Dispose(bool disposing)
        {
            Context.Dispose();
            base.Dispose(disposing);
        }

        [EnableQuery]
        public SingleResult<Command> Get([FromODataUri] long key)
        {
            var result = Get().Where(p => p.Id == key);
            return SingleResult.Create(result);
        }

        [AcceptVerbs("OPTIONS")]
        [HttpPost]
        public async Task<IHttpActionResult> Execute([FromODataUri] int key, ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var arg1 = (string)parameters["Argument"];
            var arg2 = (string)parameters["Argument2"];

            try
            {
                using (var context = new ZvsContext())
                {
                    var command = await context.Commands.FirstOrDefaultAsync(o => o.Id == key);

                    if (command == null)
                        return NotFound();

                    var cp = new CommandProcessor(WebApi2Plugin.ZvsEngine);
                    var result = await cp.RunCommandAsync(this, command, arg1, arg2);
                    if (result.HasErrors)
                        return BadRequest(result.Message); 

                    return Ok(result.Message);
                }

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}