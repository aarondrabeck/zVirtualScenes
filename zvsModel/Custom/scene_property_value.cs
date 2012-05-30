using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace zVirtualScenesModel
{
    public partial class scene_property_value 
    {
        public static string GetPropertyValue(zvsLocalDBEntities context, int sceneID, string property_value_name)
        {
            //Find the property
            scene_property property = context.scene_property.FirstOrDefault(p => p.name == property_value_name);

            if (property == null)
                return string.Empty;
            else
            {
                scene_property_value spv = context.scene_property_value.FirstOrDefault(p => p.scene_property_id == property.id && p.scene_id == sceneID);

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

        public static event onContextUpdatedEventHandler onContextUpdated;
        public static void CallOnContextUpdated()
        {
            if (onContextUpdated != null)
                onContextUpdated(null, new EventArgs());
        }
    }    
}
