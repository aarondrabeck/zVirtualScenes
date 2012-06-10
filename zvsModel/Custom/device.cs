using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace zVirtualScenesModel
{


    public partial class device : INotifyPropertyChanged
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

       
    }
}