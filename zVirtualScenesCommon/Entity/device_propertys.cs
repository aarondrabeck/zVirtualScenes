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
    public partial class device_propertys : EntityObject
    {

        public static void DefineOrUpdateDeviceProperty(device_propertys dp)
        {

            device_propertys existing_dp = zvsEntityControl.zvsContext.device_propertys.FirstOrDefault(d => d.name == dp.name);

            if (existing_dp == null)
            {
                zvsEntityControl.zvsContext.device_propertys.AddObject(dp);
            }
            else
            {
                existing_dp.friendly_name = dp.friendly_name;
                existing_dp.value_data_type = dp.value_data_type;
                existing_dp.default_value = dp.default_value;

                foreach (var option in zvsEntityControl.zvsContext.device_property_options.Where(p => p.device_property_id == existing_dp.id).ToArray())
                {
                    zvsEntityControl.zvsContext.DeleteObject(option);
                }

                foreach (device_property_options dpo in dp.device_property_options)
                    existing_dp.device_property_options.Add(new device_property_options { name = dpo.name });

            }
            zvsEntityControl.zvsContext.SaveChanges();

        }
        
    }
}
