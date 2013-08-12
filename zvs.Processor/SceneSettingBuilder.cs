using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zvs.Entities;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Reflection;

namespace zvs.Processor
{
    public class SceneSettingBuilder
    {
        public Core Core { get; private set; }
        public zvsContext Context { get; private set; }

        public SceneSettingBuilder(Core core, zvsContext context)
        {
            Core = core;
            Context = context;
        }

        public async Task RegisterAsync(SceneSetting sceneSetting)
        {
            if (sceneSetting == null)
                return;

            SceneSetting setting = await Context.SceneSettings
                .Include(o=> o.Options)
                .FirstOrDefaultAsync(s => s.UniqueIdentifier == sceneSetting.UniqueIdentifier);

            if (setting == null)
            {
                Context.SceneSettings.Add(sceneSetting);
            }
            else
            {
                //Update
                setting.Name = sceneSetting.Name;
                setting.Description = sceneSetting.Description;
                setting.ValueType = sceneSetting.ValueType;
                setting.Value = sceneSetting.Value;

                 Context.SceneSettingOptions.RemoveRange(setting.Options.ToList());
                setting.Options.Clear();
                sceneSetting.Options.ToList().ForEach(o => setting.Options.Add(o));
            }
            
            var result = await Context.TrySaveChangesAsync();
            if (result.HasError)
                Core.log.Error(result.Message);
        }
    }
}
