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
        
        public delegate void ValueDataChangedEventHandler(object sender, ValueDataChangedEventArgs args);

        public void DeviceValueDataChanged(ValueDataChangedEventArgs args)
        {
            if (DeviceValueDataChangedEvent != null)
                DeviceValueDataChangedEvent(this, args);

        }

        public class ValueDataChangedEventArgs : System.EventArgs
        {
            public int device_value_id = 0;
            public string previousValue;
        }

        public static event DeviceValueAddedEventHandler DeviceValueAddedEvent;
        public delegate void DeviceValueAddedEventHandler(object sender, EventArgs e);

        public void DeviceValueAdded(EventArgs e)
        {
            if (DeviceValueAddedEvent != null)
                DeviceValueAddedEvent(this, e);

        }           
    }
}
