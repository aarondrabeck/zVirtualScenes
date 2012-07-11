using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace zVirtualScenesModel
{
    public partial class program_options
    {
        public static void AddOrEdit(zvsLocalDBEntities context, program_options opt)
        {
            if (opt != null)
            {
                program_options existing_option = context.program_options.FirstOrDefault(o => o.name == opt.name);

                if (existing_option == null)
                {
                    context.program_options.Add(opt);
                }
                else
                {
                    //Update
                    existing_option.name = opt.name;
                    existing_option.value = opt.value;
                }
                context.SaveChanges();
            }
        }

        public static string GetProgramOption(zvsLocalDBEntities context, string optionName)
        {
            program_options option = context.program_options.FirstOrDefault(o => o.name == optionName);

            if (option != null)
                return option.value;

            return null;
        }
    }
}
