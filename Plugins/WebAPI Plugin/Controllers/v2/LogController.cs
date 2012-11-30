using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using WebAPI.Cors;
using WebAPI.DTO;
using zvs.Processor.Logging;

namespace WebAPI.Controllers.v2
{
    [Documentation("v2/Log", 2.1, "All devices.")]
    public class LogItemsController : zvsController 
    {
        public LogItemsController(WebAPIPlugin webAPIPlugin) : base(webAPIPlugin) { }

        [EnableCors]
        [HttpGet]
        [DTOQueryable]
        public IQueryable<zvs.Processor.Logging.LogItem> Get()
        {
            //Check authorization
            DenyUnauthorized();

            return EventedLog.Items.AsQueryable();
        }

        [EnableCors]
        [HttpDelete]
        public HttpResponseMessage Remove()
        {
            DenyUnauthorized();

            EventedLog.Clear();
            
            return Request.CreateResponse(ResponseStatus.Success, HttpStatusCode.OK, "OK");
        }

        [EnableCors]
        [HttpGet]
        public HttpResponseMessage GetById(int id)
        {
            //Check authorization
            DenyUnauthorized();

            var item = EventedLog.Items.FirstOrDefault(o => o.Id == id);

            DTOFactory<LogItem> dtoFactory = new DTOFactory<LogItem>(item);
            return Request.CreateResponse(ResponseStatus.Success, HttpStatusCode.OK, "OK", dtoFactory.getDTO());
        }

    }
    //public class LogController : zvsControllerBase
    //{
    //    ILog log = LogManager.GetLogger<LogController>();

    //    public IEnumerable<zvs.Processor.Logging.LogItem> Get()
    //    {
    //        base.Log(log);
    //        return EventedLog.Items;
    //    }
    //    public void Delete()
    //    {
    //        EventedLog.Clear();
    //        base.Log(log);
    //    }
    //}
}
