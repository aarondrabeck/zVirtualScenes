using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using WebAPI.DTO;
using zvs.Processor.Logging;

namespace WebAPI.Controllers.v2
{
    [EnableCors("*", "*", "*")]
    [Documentation(typeof(zvs.Processor.Logging.LogItem), "v2/Log", 2.1,
     @"Current log entries. 

       Send DELETE to the collection to clear the log.")]
    public class LogItemsController : zvsController
    {
        public LogItemsController(WebAPIPlugin webAPIPlugin) : base(webAPIPlugin) { }

        [HttpGet]
        [DTOQueryable(PageSize = 100)]
        public IQueryable<zvs.Processor.Logging.LogItem> Get()
        {
            //Check authorization
            DenyUnauthorized();

            return EventedLog.Items.AsQueryable();
        }

        [HttpDelete]
        public HttpResponseMessage Remove()
        {
            //Check authorization
            DenyUnauthorized();

            EventedLog.Clear();

            return Request.CreateResponse(ResponseStatus.Success, HttpStatusCode.OK, "OK");
        }

        [HttpGet]
        public HttpResponseMessage GetByIdAsync(int id)
        {
            //Check authorization
            DenyUnauthorized();

            var item = EventedLog.Items.FirstOrDefault(o => o.Id == id);

            return Request.CreateResponse(ResponseStatus.Success, HttpStatusCode.OK, "OK", DTOFactory.GetDTO(item));
        }

    }
}
