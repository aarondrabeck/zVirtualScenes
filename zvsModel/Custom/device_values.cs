using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace zVirtualScenesModel
{
    public partial class device_values
    {
        //Events
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
            public int device_value_id { get; private set; }          
            public string newValue { get; private set; }
            public string oldValue { get; private set; }

            public ValueDataChangedEventArgs(int device_value_id, string newValue, string oldValue)
            {
                this.device_value_id = device_value_id;
                this.newValue = newValue;
                this.oldValue = oldValue;
            }
        }

        public static event DeviceValueAddedEventHandler DeviceValueAddedEvent;
        public delegate void DeviceValueAddedEventHandler(object sender, EventArgs e);

        public void DeviceValueAdded(EventArgs e)
        {
            if (DeviceValueAddedEvent != null)
                DeviceValueAddedEvent(this, e);
        }

        //public static event onContextUpdatedEventHandler onContextUpdated;
        //public static void CallOnContextUpdated()
        //{
        //    if (onContextUpdated != null)
        //        onContextUpdated(null, new EventArgs());
        //}
    }
}
