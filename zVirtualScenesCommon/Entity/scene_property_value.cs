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
       public static string GetPropertyValue(scene s, string property_value_name)
       {
           //Find the property
           scene_property property = zvsEntityControl.zvsContext.scene_property.SingleOrDefault(p => p.name == property_value_name);

           if (property == null)
               return string.Empty;
           else
           {
               scene_property_value spv = zvsEntityControl.zvsContext.scene_property_value.SingleOrDefault(p => p.id == property.id && p.scene_id == s.id);

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
