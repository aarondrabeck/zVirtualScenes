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
        public static event ValueDataChangedEventHandler ValueDataChangedEvent;
        public delegate void ValueDataChangedEventHandler(int ObjectId, string ValueID, string label, string Value, string PreviousValue);

        public static void ValueDataChanged(int ObjectId, string ValueID, string label, string Value, string prevVal)
        {
            if (ValueDataChangedEvent != null)
                ValueDataChangedEvent(ObjectId, ValueID, label, Value, prevVal);
        }

        /// <summary>
        /// Called before the value is changed in the database
        /// </summary>
        public static event ValueChangingEventHandler ValueChangingEvent;
        public delegate void ValueChangingEventHandler(int ObjectId, string ValueID, string label, string Value);

        public static void ValueChanging(int ObjectId, string ValueID, string label, string Value)
        {
            if (ValueChangingEvent != null)
                ValueChangingEvent(ObjectId, ValueID, label, Value);
        }



        public delegate void DeviceValueChangedEventHandler();
        public static event DeviceValueChangedEventHandler DeviceValueChanged;

        partial void OnvalueChanged()
        {
            if (DeviceValueChanged != null)
                DeviceValueChanged();
        }
    }
}
