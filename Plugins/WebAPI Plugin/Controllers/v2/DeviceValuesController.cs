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

using zvs.Entities;
using zvs.Processor;
using zvs.Processor.Logging;

namespace WebAPI.Controllers.v2
{
    [Documentation(typeof(DeviceValue), "v2/DeviceValues", 2.1,
    @"All available device values.

    Example query: /v2/DeviceValues?$filter=Name eq 'basic' and DeviceId eq 3
    ")]
    public class DeviceValuesController : zvsEntityController<DeviceValue>
    {
        public DeviceValuesController(WebAPIPlugin webAPIPlugin) : base(webAPIPlugin) { }
        
        [HttpGet]
        [DTOQueryable(PageSize = 100)]
        public new async Task<IQueryable<DeviceValue>> Get()
        {
            return await base.Get();
        }

        
        [HttpGet]
        public new async Task<HttpResponseMessage> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }

        
        [HttpPost]
        public new async Task<HttpResponseMessage> AddAsync(DeviceValue tEntityPost)
        {
            return await base.AddAsync(tEntityPost);
        }

        
        [HttpPatch]
        [HttpPut]
        public new async Task<HttpResponseMessage> UpdateAsync(int id, Dictionary<string, object> tEntityPatch)
        {
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
