using System;
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
        private IEntityContextConnection EntityContextConnection { get; set; }

        public BuiltinCommandBuilder(IEntityContextConnection entityContextConnection)
        {
            if (entityContextConnection == null)
                throw new ArgumentNullException("entityContextConnection");

            EntityContextConnection = entityContextConnection;
        }

        public async Task<Result> RegisterAsync(BuiltinCommand builtinCommand, CancellationToken cancellationToken)
        {
            if (builtinCommand == null)
                return Result.ReportError("builtinCommand is null");

            using (var context = new ZvsContext(EntityContextConnection))
            {
                var existingC = await context.BuiltinCommands.FirstOrDefaultAsync(o => o.UniqueIdentifier == builtinCommand.UniqueIdentifier, cancellationToken);
                var wasModified = false;
                if (existingC == null)
                {
                    context.BuiltinCommands.Add(builtinCommand);
                    wasModified = true;
                }
                else
                {
                    PropertyChangedEventHandler handler = (s, a) => wasModified = true;
                    existingC.PropertyChanged += handler;

                    existingC.Name = builtinCommand.Name;
                    existingC.CustomData1 = builtinCommand.CustomData1;
                    existingC.CustomData2 = builtinCommand.CustomData2;
                    existingC.ArgumentType = builtinCommand.ArgumentType;
                    existingC.Description = builtinCommand.Description;
                    existingC.Help = builtinCommand.Help;

                    existingC.PropertyChanged -= handler;

                    var addded =
                        builtinCommand.Options.Where(option => existingC.Options.All(o => o.Name != option.Name))
                            .ToList();
                    foreach (var option in addded)
                    {
                        existingC.Options.Add(option);
                        wasModified = true;
                    }

                    var removed =
                        existingC.Options.Where(option => builtinCommand.Options.All(o => o.Name != option.Name)).ToList();
                    foreach (var option in removed)
                    {
                        context.CommandOptions.Local.Remove(option);
                        wasModified = true;
                    }
                }

                if (wasModified)
                    return await context.TrySaveChangesAsync(cancellationToken);

                return Result.ReportSuccess("Nothing to update");
            }
        }
    }
}
