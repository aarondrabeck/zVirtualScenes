using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zvs.Entities;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Reflection;
using System.ComponentModel;

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
                .Include(o => o.Options)
                .FirstOrDefaultAsync(s => s.UniqueIdentifier == sceneSetting.UniqueIdentifier);

            var changed = false;
            if (setting == null)
            {
                Context.SceneSettings.Add(sceneSetting);
                changed = true;
            }
            else
            {
                //Update
                PropertyChangedEventHandler handler = (s, a) => changed = true;
                setting.PropertyChanged += handler;

                setting.Name = sceneSetting.Name;
                setting.Description = sceneSetting.Description;
                setting.ValueType = sceneSetting.ValueType;
                setting.Value = sceneSetting.Value;
                
                setting.PropertyChanged -= handler;

                foreach (var option in setting.Options)
                {
                    if (!sceneSetting.Options.Any(o => o.Name == option.Name))
                    {
                        sceneSetting.Options.Add(option);
                        changed = true;
                    }
                }

                foreach (var option in sceneSetting.Options)
                {
                    if (!setting.Options.Any(o => o.Name == option.Name))
                    {
                        Context.SceneSettingOptions.Local.Remove(option);
                        changed = true;
                    }
                }
            }

            if (changed)
            {
                var result = await Context.TrySaveChangesAsync();
                if (result.HasError)
                    Core.log.Error(result.Message);
            }
        }
    }
}
