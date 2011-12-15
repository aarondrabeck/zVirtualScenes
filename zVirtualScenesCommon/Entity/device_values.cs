using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;

namespace zVirtualScenesCommon.Entity
{
    public partial class device_values : EntityObject
    {
        /// <summary>
        /// Called after the Value has been changed in the database
        /// </summary>
        public static event ValueDataChangedEventHandler DeviceValueDataChangedEvent;
        public delegate void ValueDataChangedEventHandler(object sender, string PreviousValue);

        public void DeviceValueDataChanged(string prevVal)
        {
            if (DeviceValueDataChangedEvent != null)
                DeviceValueDataChangedEvent(this, prevVal);

        }      

        
       
    }
}
