using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace zVirtualScenesModel
{
    public partial class device_propertys
    {
        public static void AddOrEdit(device_propertys dp, zvsLocalDBEntities context)
        {
            device_propertys existing_dp = context.device_propertys.FirstOrDefault(d => d.name == dp.name);

            if (existing_dp == null)
            {
                context.device_propertys.Add(dp);
            }
            else
            {
                existing_dp.friendly_name = dp.friendly_name;
                existing_dp.value_data_type = dp.value_data_type;
                existing_dp.default_value = dp.default_value;

                foreach (var option in context.device_property_options.Where(p => p.device_property_id == existing_dp.id).ToArray())
                {
                    context.device_property_options.Remove(option);
                }

                foreach (device_property_options dpo in dp.device_property_options)
                    existing_dp.device_property_options.Add(new device_property_options { name = dpo.name });

            }
            context.SaveChanges();
        }
    }
}
