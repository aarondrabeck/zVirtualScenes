using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace zVirtualScenesModel
{
    public partial class scene_property
    {
        public static void AddOrEdit(scene_property p, zvsLocalDBEntities context)
        {
            if (p != null)
            {
                scene_property existing_property = context.scene_property.FirstOrDefault(ep => ep.name == p.name);

                    if (existing_property == null)
                    {
                        context.scene_property.Add(p);
                    }
                    else
                    {
                        //Update
                        existing_property.friendly_name = p.friendly_name;
                        existing_property.description = p.description;
                        existing_property.value_data_type = p.value_data_type;
                        existing_property.defualt_value = p.defualt_value;

                        foreach (var option in context.scene_property_option.Where(o => o.scene_property_id == existing_property.id).ToArray())
                        {
                            context.scene_property_option.Remove(option);
                        }

                        foreach (scene_property_option spo in p.scene_property_option)
                            existing_property.scene_property_option.Add(new scene_property_option { options = spo.options });

                    }
                    context.SaveChanges();
            }
        }

        
    }
}
