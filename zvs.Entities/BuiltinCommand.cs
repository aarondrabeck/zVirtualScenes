using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zvs.Entities
{
    [Table("BuiltinCommands", Schema = "ZVS")]
    public partial class BuiltinCommand : Command
    {
        public static void AddOrEdit(BuiltinCommand c, zvsContext context)
        {
            BuiltinCommand existing_c = context.BuiltinCommands.FirstOrDefault(cmd => cmd.UniqueIdentifier == c.UniqueIdentifier);
            if (existing_c == null)
            {
                context.BuiltinCommands.Add(c);
            }
            else
            {
                existing_c.Name = c.Name;
                existing_c.CustomData1 = c.CustomData1;
                existing_c.CustomData2 = c.CustomData2;
                existing_c.ArgumentType = c.ArgumentType;
                existing_c.Description = c.Description;
                existing_c.Help = c.Help;
                existing_c.Options = c.Options;
            }
            context.SaveChanges();
        }
    }
}
