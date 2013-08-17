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
using WebAPI.DTO;
using zvs.Entities;
using zvs.Processor;
using zvs.Processor.Logging;

namespace WebAPI.Controllers.v2
{
    [Documentation(typeof(Scene), "v2/Scenes", 2.1, @"All scenes.

    Change scene name: PATCH /Scenes/{scene.id}
    {
      ""Name"": ""Movie Mode Bright"",
    }

    Run scene: PATCH /Scenes/{scene.id}
    {
      ""isRunning"": true
    }

")]
    public class ScenesController : zvsEntityController<Scene>
    {
        public ScenesController(WebAPIPlugin webAPIPlugin) : base(webAPIPlugin) { }

        protected override IQueryable<Scene> BaseQueryable
        {
            get
            {
                var settingUid = WebAPIPlugin.SceneSettingUids.SHOW_IN_WEBAPI.ToString();
                var defaultSettingShouldShow = Cache.SHOW_IN_WEBAPI_DEFAULT_VALUE;

                return db.Scenes
                    .Where(o => (o.SettingValues.All(p => p.SceneSetting.UniqueIdentifier != settingUid) && defaultSettingShouldShow) || //Show all objects where no explicit setting has been create yet and the defaultSetting is to show
                                 o.SettingValues.Any(p => p.SceneSetting.UniqueIdentifier == settingUid && p.Value.Equals("true"))); //Show all objects where an explicit setting has been create and set to show
            }
        }

        
        [HttpGet]
        [DTOQueryable(PageSize = 100)]
        public new async Task<IQueryable<Scene>> Get()
        {
            return await base.Get();
        }

        
        [HttpGet]
        public new async Task<HttpResponseMessage> GetByIdAsync(int id)
        {
            return await GetByIdAsync(id);
        }

        
        [HttpPost]
        public new async Task<HttpResponseMessage> AddAsync(Scene tEntityPost)
        {
            return await base.AddAsync(tEntityPost);
        }

        
        [HttpPatch]
        [HttpPut]
        public new async Task<HttpResponseMessage> UpdateAsync(int id, Dictionary<string, object> tEntityPatch)
        {
            DenyUnauthorized();

            //if isRunning = true is sent, lets start the scene.
            Scene s = await BaseQueryable.Where(o => o.Id == id).SingleOrDefaultAsync();

            if (s == null)
                return Request.CreateResponse(ResponseStatus.Error, HttpStatusCode.NotFound, "Resource not found");

            if (tEntityPatch != null)
            {
                
                bool isRunning = false;
                if (tEntityPatch.ContainsKey("isRunning"))
                {
                    bool.TryParse(tEntityPatch["isRunning"].ToString(), out isRunning);
                }

                if (isRunning)
                {
                    BuiltinCommand cmd = await Task.Run(() => db.BuiltinCommands.FirstOrDefault(c => c.UniqueIdentifier == "RUN_SCENE"));
                    if (cmd != null)
                    {
                        CommandProcessor cp = new CommandProcessor(this.WebAPIPlugin.Core);

                        //Marshal to another thread pool thread as to not await complete...
                       await cp.RunCommandAsync(this, cmd, s.Id.ToString());
                        
                        return Request.CreateResponse(ResponseStatus.Success, HttpStatusCode.OK, "Scene started.");
                    }
                }
            }

            //update standard properties such as scene.Name
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