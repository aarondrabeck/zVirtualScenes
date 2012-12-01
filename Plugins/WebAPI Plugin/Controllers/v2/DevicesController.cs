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
    [Documentation("v2/Devices", 2.1, "All devices.")]
    public class DevicesController : zvsEntityController<Device>
    {
        public DevicesController(WebAPIPlugin webAPIPlugin) : base(webAPIPlugin) { }

        protected override DbSet DBSet
        {
            get { return db.Devices; }
        }

        protected override IQueryable<Device> BaseQueryable
        {
            get
            {
                //We need to hide the devices the user chooses from all CRUD operations.
                List<Device> devices = new List<Device>();
                foreach (Device d in DBSet)
                {
                    bool show = true;
                    bool.TryParse(DevicePropertyValue.GetPropertyValue(db, d, "WebAPI_SHOW_DEVICE"), out show);
                    if (show) devices.Add(d);
                }
                return devices.AsQueryable();
            }
        }

        [EnableCors]
        [HttpGet]
        [DTOQueryable]
        public new IQueryable<Device> Get()
        {
            return base.Get().OrderBy(o => o.Name);
        }

        [EnableCors]
        [HttpGet]
        public new HttpResponseMessage GetById(int id)
        {
            return base.GetById(id);
        }

        [EnableCors]
        [HttpPost]
        public new HttpResponseMessage Add(Device tEntityPost)
        {
            return base.Add(tEntityPost);
        }

        [EnableCors]
        [HttpPatch]
        [HttpPut]
        public new HttpResponseMessage Update(int id, Delta<Device> device)
        {

            DenyUnauthorized();

            //if isRunning = true is sent, lets start the scene.
            Device d = BaseQueryable.Where(o => o.Id == id).SingleOrDefault();
            if (d == null)
                return Request.CreateResponse(ResponseStatus.Error, HttpStatusCode.NotFound, "Resource not found");


            if (device != null)
            {
                if (device.GetChangedPropertyNames().Contains("CurrentLevelInt"))
                    return Request.CreateResponse(ResponseStatus.Error, HttpStatusCode.BadRequest, "Cannot set CurrentLevelInt, use CurrentLevelText instead");

                double newlevel = 0;
                object CurrentLevelText;
                if (device.TryGetPropertyValue("CurrentLevelText", out CurrentLevelText))
                {
                    if (double.TryParse((string)CurrentLevelText, out newlevel))
                    {
                        DeviceCommand basicCmd = d.Commands.FirstOrDefault(o=> o.UniqueIdentifier == "DYNAMIC_CMD_BASIC"); 
                        if(basicCmd == null)
                            return Request.CreateResponse(ResponseStatus.Error, HttpStatusCode.BadRequest, "Cannot set CurrentLevelText, cannot find basic command on device");

                        CommandProcessor cp = new CommandProcessor(this.WebAPIPlugin.Core);

                        //Marshal to another thread pool thread as to not await complete...
                        Task.Run(() => cp.RunDeviceCommandAsync(basicCmd.Id, newlevel.ToString()));

                        return Request.CreateResponse(ResponseStatus.Success, HttpStatusCode.OK, "Change basic processed");
                    }
                    return Request.CreateResponse(ResponseStatus.Error, HttpStatusCode.BadRequest, "CurrentLevelText Invalid");
                }
            }
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
