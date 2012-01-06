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
                using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
                {
                    program_options existing_option = db.program_options.FirstOrDefault(o => o.name == opt.name);

                    if (existing_option == null)
                    {
                        db.program_options.AddObject(opt);
                    }
                    else
                    {
                        //Update
                        existing_option.name = opt.name;
                        existing_option.value = opt.value;
                    }
                    db.SaveChanges();
                }
            }
        }

        public static string GetProgramOption(string optionName)
        {
            using (zvsEntities2 db = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
            {
                program_options option = db.program_options.FirstOrDefault(o => o.name == optionName);

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
}
