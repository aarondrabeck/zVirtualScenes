using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using zvs.DataModel;
using System.Data.Entity;
using System.ComponentModel;

namespace zvs.Processor
{
    public class SceneSettingBuilder
    {
        private IEntityContextConnection EntityContextConnection { get; set; }

        public SceneSettingBuilder(IEntityContextConnection entityContextConnection)
        {
            if (entityContextConnection == null)
                throw new ArgumentNullException("entityContextConnection");

            EntityContextConnection = entityContextConnection;
        }

        public async Task<Result> RegisterAsync(SceneSetting sceneSetting, CancellationToken cancellationToken)
        {
            if (sceneSetting == null)
                return Result.ReportError("sceneSetting is null");

            using (var context = new ZvsContext(EntityContextConnection))
            {
                var existingSetting = await context.SceneSettings
                    .Include(o => o.Options)
                    .FirstOrDefaultAsync(s => s.UniqueIdentifier == sceneSetting.UniqueIdentifier, cancellationToken);

                var changed = false;
                if (existingSetting == null)
                {
                    context.SceneSettings.Add(sceneSetting);
                    changed = true;
                }
                else
                {
                    //Update
                    PropertyChangedEventHandler handler = (s, a) => changed = true;
                    existingSetting.PropertyChanged += handler;

                    existingSetting.Name = sceneSetting.Name;
                    existingSetting.Description = sceneSetting.Description;
                    existingSetting.ValueType = sceneSetting.ValueType;
                    existingSetting.Value = sceneSetting.Value;

                    existingSetting.PropertyChanged -= handler;

                    var added = sceneSetting.Options.Where(option => existingSetting.Options.All(o => o.Name != option.Name)).ToList();
                    foreach (var option in added)
                    {
                        existingSetting.Options.Add(option);
                        changed = true;
                    }

                    var removed = existingSetting.Options.Where(option => sceneSetting.Options.All(o => o.Name != option.Name)).ToList();
                    foreach (var option in removed)
                    {
                        context.SceneSettingOptions.Local.Remove(option);
                        changed = true;
                    }
                }

                if (changed)
                    return await context.TrySaveChangesAsync(cancellationToken);

                return Result.ReportSuccess("Nothing to update");
            }
        }
    }
}
