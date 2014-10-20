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
        private IFeedback<LogEntry> Log { get; set; }
        private ZvsContext Context { get; set; }

        public SceneSettingBuilder(IFeedback<LogEntry> log, ZvsContext zvsContext)
        {
            Context = zvsContext;
            Log = log;
        }

        public async Task RegisterAsync(SceneSetting sceneSetting, CancellationToken cancellationToken)
        {
            if (sceneSetting == null)
                return;

            var setting = await Context.SceneSettings
                .Include(o => o.Options)
                .FirstOrDefaultAsync(s => s.UniqueIdentifier == sceneSetting.UniqueIdentifier, cancellationToken);

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

                foreach (var option in setting.Options.Where(option => sceneSetting.Options.All(o => o.Name != option.Name)))
                {
                    sceneSetting.Options.Add(option);
                    changed = true;
                }

                foreach (var option in sceneSetting.Options.Where(option => setting.Options.All(o => o.Name != option.Name)))
                {
                    Context.SceneSettingOptions.Local.Remove(option);
                    changed = true;
                }
            }

            if (changed)
            {
                var result = await Context.TrySaveChangesAsync(cancellationToken);
                if (result.HasError)
                    await Log.ReportErrorAsync(result.Message, cancellationToken);
            }
        }
    }
}
