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
    public partial class scene_property : EntityObject
    {     
        public static void DefineOrUpdateProperty(scene_property p)
        {
            if (p != null)
            {
                scene_property existing_property = zvsEntityControl.zvsContext.scene_property.FirstOrDefault(ep => ep.name == p.name);

                if (existing_property == null)
                {
                    zvsEntityControl.zvsContext.scene_property.AddObject(p);
                }
                else
                {
                    //Update
                    existing_property.friendly_name = p.friendly_name;
                    existing_property.description = p.description;
                    existing_property.value_data_type = p.value_data_type;
                    existing_property.defualt_value = p.defualt_value;

                    foreach (var option in zvsEntityControl.zvsContext.scene_property_option.Where(o => o.scene_property_id == existing_property.id).ToArray())
                    {
                        zvsEntityControl.zvsContext.DeleteObject(option);
                    }

                    foreach (scene_property_option spo in p.scene_property_option)
                        existing_property.scene_property_option.Add(new scene_property_option { option = spo.option });

                }
                zvsEntityControl.zvsContext.SaveChanges();
            }
        }
    }
}
