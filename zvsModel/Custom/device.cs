using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace zVirtualScenesModel
{
    public delegate void onContextUpdatedEventHandler(object sender, EventArgs args);

    public partial class device 
    {
        /// <summary>
        /// This is called when a device or device value 
        /// </summary> 

        public static IQueryable<device> GetAllDevices(zvsLocalDBEntities context, bool forList)
        {
            var query = from o in context.devices
                        where o.device_types.plugin.name != "BUILTIN"
                        select o;

            if (forList)
                return query.Where(o => o.device_types.show_in_list == true).AsQueryable();
            else
                return query.AsQueryable();
        }

        public override string ToString()
        {
            return this.friendly_name;
        }

        public static event onContextUpdatedEventHandler onContextUpdated;
        public static void CallOnContextUpdated()
        {
            if (onContextUpdated != null)
                onContextUpdated(null, new EventArgs());
        }
    }
}