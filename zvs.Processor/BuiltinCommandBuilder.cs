using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zvs.Entities;
using System.Data.Entity;

namespace zvs.Processor
{
    public class BuiltinCommandBuilder : Builder
    {
        protected zvsContext Context { get; set; }
        public BuiltinCommandBuilder(zvsAdapter zvsAdapter, Core core, zvsContext context)
            : base(zvsAdapter, core)
        {
            Context = context;
        }
 
        public async Task RegisterAsync(BuiltinCommand builtinCommand)
        {
            var existing_c = await Context.BuiltinCommands.FirstOrDefaultAsync(o => o.UniqueIdentifier == builtinCommand.UniqueIdentifier);
            if (existing_c == null)
            {
                Context.BuiltinCommands.Add(builtinCommand);
            }
            else
            {
                existing_c.Name = builtinCommand.Name;
                existing_c.CustomData1 = builtinCommand.CustomData1;
                existing_c.CustomData2 = builtinCommand.CustomData2;
                existing_c.ArgumentType = builtinCommand.ArgumentType;
                existing_c.Description = builtinCommand.Description;
                existing_c.Help = builtinCommand.Help;
                existing_c.Options = builtinCommand.Options;
            }

            var result = await Context.TrySaveChangesAsync();
            if (result.HasError)
                Core.log.Error(result.Message);

        }
    }
}
