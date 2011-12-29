using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;
using zVirtualScenesCommon;
using System.Runtime.Serialization;
using System.Data.Objects;

namespace zVirtualScenesCommon.Entity
{
    public partial class program_options : EntityObject
    {
        public static void DefineOrUpdateProgramOption(program_options opt)
        {
            if (opt != null)
            {
                program_options existing_option = zvsEntityControl.zvsContext.program_options.FirstOrDefault(o => o.name == opt.name);

                if (existing_option == null)
                {
                    zvsEntityControl.zvsContext.program_options.AddObject(opt);
                }
                else
                {
                    //Update
                    existing_option.name = opt.name;
                    existing_option.value = opt.value;
                }
                zvsEntityControl.zvsContext.SaveChanges();
            }
        }

        public static string GetProgramOption(string optionName)
        {
            program_options option = zvsEntityControl.zvsContext.program_options.FirstOrDefault(o => o.name == optionName);

            if (option != null)
                return option.value;
            else
            {
                //DEFAULTS
                if (optionName.Equals("TempAbbreviation"))
                    return "F";
               
                return null;
            }
        }
    }
}
