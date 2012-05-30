using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace zVirtualScenesModel
{
    public partial class device_property_values
    {
        public static string GetDevicePropertyValue(zvsLocalDBEntities context, int DeviceId, string SettingName)
        {
            device device = context.devices.FirstOrDefault(o => o.id == DeviceId);

            if (device != null)
            {
                device_property_values dpv = device.device_property_values.FirstOrDefault(o => o.device_propertys.name == SettingName);

                if (dpv != null)
                {
                    return dpv.value;
                }
                else
                {
                    device_propertys dp = context.device_propertys.FirstOrDefault(o => o.name == SettingName);
                    if (dp != null)
                    {
                        return dp.default_value;
                    }
                }
            }
            return string.Empty;
        }

        public static event onContextUpdatedEventHandler onContextUpdated;
        public static void CallOnContextUpdated()
        {
            if (onContextUpdated != null)
                onContextUpdated(null, new EventArgs());
        }
    }
}
