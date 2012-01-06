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
    public partial class scene_property_value : EntityObject
    {
        public static string GetPropertyValue(zvsEntities2 db, long sceneID, string property_value_name)
        {
            //Find the property
            scene_property property = db.scene_property.FirstOrDefault(p => p.name == property_value_name);

            if (property == null)
                return string.Empty;
            else
            {
                scene_property_value spv = db.scene_property_value.FirstOrDefault(p => p.scene_property_id == property.id && p.scene_id == sceneID);

                //Check to see if the property has been set yet, otherwise return the defualt vaule for this property.
                if (spv == null)
                {
                    return property.defualt_value;
                }
                else
                {
                    return spv.value;
                }
            }
        }
    }    
}
