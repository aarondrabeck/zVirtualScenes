using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.OData;
using WebAPI.DTO;
using zvs.Entities;
using zvs.Processor;
using zvs.Processor.Logging;

namespace WebAPI.Controllers.v2
{
    [Documentation(typeof(Device), "v2/Devices", 2.1, "All devices.")]
    public class DevicesController : zvsEntityController<Device>
    {
        public DevicesController(WebAPIPlugin webAPIPlugin) : base(webAPIPlugin) { }

        protected override IQueryable<Device> BaseQueryable
        {
            get
            {
                var settingUid = WebAPIPlugin.DeviceSettingUids.SHOW_IN_WEBAPI.ToString();
                var defaultSettingShouldShow = Cache.SHOW_IN_WEBAPI_DEFAULT_VALUE;

                return db.Devices
                    .Where(o => (o.DeviceSettingValues.All(p => p.DeviceSetting.UniqueIdentifier != settingUid) && defaultSettingShouldShow) || //Show all objects where no explicit setting has been create yet and the defaultSetting is to show
                                 o.DeviceSettingValues.Any(p => p.DeviceSetting.UniqueIdentifier == settingUid && p.Value.Equals("true"))); //Show all objects where an explicit setting has been create and set to show
            }
        }

        [HttpGet]
        [DTOQueryable(PageSize = 100)]
        public new Task<IQueryable<Device>> Get()
        {
            return Task.FromResult(BaseQueryable.OrderBy(o => o.Name).AsQueryable());
        }


        [HttpGet]
        public new async Task<HttpResponseMessage> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }


        [HttpPost]
        public new async Task<HttpResponseMessage> AddAsync(Device tEntityPost)
        {
            return await base.AddAsync(tEntityPost);
        }


        [HttpPatch]
        [HttpPut]
        public new async Task<HttpResponseMessage> UpdateAsync(int id, Dictionary<string, object> tEntityPatch)
        {
            DenyUnauthorized();

            //if isRunning = true is sent, lets start the scene.
            var d = await BaseQueryable.Where(o => o.Id == id).SingleOrDefaultAsync();

            if (d == null)
                return Request.CreateResponse(ResponseStatus.Error, HttpStatusCode.NotFound, "Resource not found");

            if (tEntityPatch != null)
            {
                if (tEntityPatch.ContainsKey("CurrentLevelInt"))
                    return Request.CreateResponse(ResponseStatus.Error, HttpStatusCode.BadRequest, "Cannot set CurrentLevelInt, use CurrentLevelText instead");

                double newlevel = 0;

                if (tEntityPatch.ContainsKey("CurrentLevelText"))
                {
                    if (!double.TryParse(tEntityPatch["CurrentLevelText"].ToString(), out newlevel))
                        return Request.CreateResponse(ResponseStatus.Error, HttpStatusCode.BadRequest, "CurrentLevelText Invalid");

                    DeviceCommand basicCmd = d.Commands.FirstOrDefault(o => o.UniqueIdentifier.Contains("DYNAMIC_CMD_BASIC"));
                    if (basicCmd == null)
                        return Request.CreateResponse(ResponseStatus.Error, HttpStatusCode.BadRequest, "Cannot set CurrentLevelText, cannot find basic command on device");

                    CommandProcessor cp = new CommandProcessor(this.WebAPIPlugin.Core);

                    //Marshal to another thread pool thread as to not await complete...
                    var result = await cp.RunCommandAsync(this, basicCmd, newlevel.ToString());
                    if (!result.HasErrors)
                        return Request.CreateResponse(HttpStatusCode.NoContent);
                    else
                        return Request.CreateResponse(HttpStatusCode.BadRequest, result.Message);
                }

            }
            return await base.UpdateAsync(id, tEntityPatch);
        }

        [HttpDelete]
        public new async Task<HttpResponseMessage> RemoveAsync(int id)
        {
            return await base.RemoveAsync(id);
        }


        [HttpGet]
        [DTOQueryable(PageSize = 100)]
        public new async Task<IQueryable<object>> GetNestedCollectionsAsync(Int64 parentId, string nestedCollectionName)
        {
            return await base.GetNestedCollectionsAsync(parentId, nestedCollectionName);
        }
    }
}
