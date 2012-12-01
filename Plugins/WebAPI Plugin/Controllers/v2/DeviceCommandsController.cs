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
    [Documentation("v2/DeviceCommands", 2.1, "All available device commands.")]
    public class DeviceCommandsController : zvsEntityController<DeviceCommand>
    {
        public DeviceCommandsController(WebAPIPlugin webAPIPlugin) : base(webAPIPlugin) { }

        protected override DbSet DBSet
        {
            get { return db.DeviceCommands; }
        }

        [EnableCors]
        [HttpGet]
        [DTOQueryable]
        public new IQueryable<DeviceCommand> Get()
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
        public new HttpResponseMessage Add(DeviceCommand tEntityPost)
        {
            return base.Add(tEntityPost);
        }

        [EnableCors]
        [HttpPatch]
        [HttpPut]
        public new HttpResponseMessage Update(int id, Delta<DeviceCommand> device)
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
