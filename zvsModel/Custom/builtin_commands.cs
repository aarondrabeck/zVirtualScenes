using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace zVirtualScenesModel
{
    public partial class builtin_commands
    {
        public static void AddOrEdit(builtin_commands c, zvsLocalDBEntities context)
        {
            builtin_commands existing_c = context.builtin_commands.FirstOrDefault(cmd => cmd.name == c.name);
            if (existing_c == null)
            {
                context.builtin_commands.Add(c);
            }
            else
            {
                existing_c.friendly_name = c.friendly_name;
                existing_c.custom_data1 = c.custom_data1;
                existing_c.custom_data2 = c.custom_data2;
                existing_c.show_on_dynamic_obj_list = c.show_on_dynamic_obj_list;
                existing_c.arg_data_type = c.arg_data_type;
            }
            context.SaveChanges();
        }        
    }
}

