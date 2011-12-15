using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;

namespace zVirtualScenesCommon.Entity
{
    public partial class builtin_commands : EntityObject
    {
        public void Run(string argument = "")
        {         
            builtin_command_que cmd = new builtin_command_que { builtin_command_id = this.id, arg = argument };
            cmd.Run();
        }

        public static void InstallBuiltInCommand(builtin_commands c)
        {
            builtin_commands existing_c = zvsEntityControl.zvsContext.builtin_commands.FirstOrDefault(cmd => cmd.name == c.name);
            if (existing_c == null)
            {
                zvsEntityControl.zvsContext.builtin_commands.AddObject(c);
            }
            else
            {
                existing_c.friendly_name = c.friendly_name;
                existing_c.custom_data1 = c.custom_data1;
                existing_c.custom_data2 = c.custom_data2;
                existing_c.show_on_dynamic_obj_list = c.show_on_dynamic_obj_list;
                existing_c.arg_data_type = c.arg_data_type;
            }
            zvsEntityControl.zvsContext.SaveChanges();
        }

        
    }
}
