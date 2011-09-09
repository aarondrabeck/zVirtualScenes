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
    public partial class device_property_values : EntityObject
    {
        public static string GetDevicePropertyValue(zvsEntities2 context, long DeviceId, string SettingName)
        {            
            device device = context.devices.SingleOrDefault(o => o.id == DeviceId);

            if (device != null)
            {
                device_property_values dpv = device.device_property_values.SingleOrDefault(o => o.device_propertys.name == SettingName);

                if (dpv != null)
                {
                    return dpv.value;
                }
                else
                {
                    device_propertys dp = context.device_propertys.SingleOrDefault(o => o.name == SettingName);
                    if (dp != null)
                    {
                        return dp.default_value;
                    }
                }               
            }
            return string.Empty;
        }
    }
}
