using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData;
using WebAPI.Cors;
using zvs.Entities;
using zvs.Processor;
using zvs.Processor.Logging;

namespace WebAPI.Controllers.v2
{
    [Documentation("v2/JavascriptCommands", 2.1, "All available JavaScript commands.")]
    public class JavaScriptCommandsController : zvsEntityController<JavaScriptCommand>
    {
        public JavaScriptCommandsController(WebAPIPlugin webAPIPlugin) : base(webAPIPlugin) { }

        protected override DbSet DBSet
        {
            get { return db.JavaScriptCommands; }
        }

        [EnableCors]
        [HttpGet]
        [DTOQueryable]
        public new IQueryable<JavaScriptCommand> Get()
        {
            return base.Get();
        }

        [EnableCors]
        [HttpGet]
        public new HttpResponseMessage GetById(int id)
        {
            return base.GetById(id);
        }

        [EnableCors]
        [HttpPost]
        public new HttpResponseMessage Add(JavaScriptCommand tEntityPost)
        {
            return base.Add(tEntityPost);
        }

        [EnableCors]
        [HttpPatch]
        [HttpPut]
        public new HttpResponseMessage Update(int id, Delta<JavaScriptCommand> device)
        {
            return base.Update(id, device);
        }

        [EnableCors]
        [HttpDelete]
        public new HttpResponseMessage Remove(int id)
        {
            return base.Remove(id);
        }

        [EnableCors]
        [HttpGet]
        [DTOQueryable]
        public new IQueryable<object> GetNestedCollection(int parentId, string nestedCollectionName)
        {
            return base.GetNestedCollection(parentId, nestedCollectionName);
        }
    }
}
