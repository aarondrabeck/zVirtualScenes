using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zvs.Entities;
using System.Data.Entity;
using System.ComponentModel;

namespace zvs.Processor
{
    public class BuiltinCommandBuilder : AdapterBuilder
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
            var changed = false;
            if (existing_c == null)
            {
                Context.BuiltinCommands.Add(builtinCommand);
                changed = true;
            }
            else
            {
                PropertyChangedEventHandler handler = (s, a) => changed = true;
                existing_c.PropertyChanged += handler;

                existing_c.Name = builtinCommand.Name;
                existing_c.CustomData1 = builtinCommand.CustomData1;
                existing_c.CustomData2 = builtinCommand.CustomData2;
                existing_c.ArgumentType = builtinCommand.ArgumentType;
                existing_c.Description = builtinCommand.Description;
                existing_c.Help = builtinCommand.Help;

                existing_c.PropertyChanged -= handler;

                foreach (var option in builtinCommand.Options)
                {
                    if (!existing_c.Options.Any(o => o.Name == option.Name))
                    {
                        existing_c.Options.Add(option);
                        changed = true;
                    }
                }

                foreach (var option in existing_c.Options)
                {
                    if (!builtinCommand.Options.Any(o => o.Name == option.Name))
                    {
                        Context.CommandOptions.Local.Remove(option);
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
