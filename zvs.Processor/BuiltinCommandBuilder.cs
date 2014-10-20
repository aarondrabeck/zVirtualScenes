using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using zvs.DataModel;
using System.Data.Entity;
using System.ComponentModel;

namespace zvs.Processor
{
    public class BuiltinCommandBuilder
    {
        private IFeedback<LogEntry> Log { get; set; }
        private ZvsContext Context { get; set; }
        public BuiltinCommandBuilder(IFeedback<LogEntry> log, ZvsContext zvsContext)
        {
            Context = zvsContext;
            Log = log;
        }

        public async Task RegisterAsync(BuiltinCommand builtinCommand, CancellationToken cancellationToken)
        {
            var existingC = await Context.BuiltinCommands.FirstOrDefaultAsync(o => o.UniqueIdentifier == builtinCommand.UniqueIdentifier, cancellationToken);
            var havePropertiesChanged = false;
            if (existingC == null)
            {
                Context.BuiltinCommands.Add(builtinCommand);
                havePropertiesChanged = true;
            }
            else
            {
                PropertyChangedEventHandler handler = (s, a) => havePropertiesChanged = true;
                existingC.PropertyChanged += handler;

                existingC.Name = builtinCommand.Name;
                existingC.CustomData1 = builtinCommand.CustomData1;
                existingC.CustomData2 = builtinCommand.CustomData2;
                existingC.ArgumentType = builtinCommand.ArgumentType;
                existingC.Description = builtinCommand.Description;
                existingC.Help = builtinCommand.Help;

                existingC.PropertyChanged -= handler;

                foreach (var option in builtinCommand.Options.Where(option => existingC.Options.All(o => o.Name != option.Name)))
                {
                    existingC.Options.Add(option);
                    havePropertiesChanged = true;
                }

                foreach (var option in existingC.Options.Where(option => builtinCommand.Options.All(o => o.Name != option.Name)))
                {
                    Context.CommandOptions.Local.Remove(option);
                    havePropertiesChanged = true;
                }
            }

            if (havePropertiesChanged)
            {
                var result = await Context.TrySaveChangesAsync(cancellationToken);
                if (result.HasError)
                    await Log.ReportErrorAsync(result.Message, cancellationToken);
            }

        }
    }
}
