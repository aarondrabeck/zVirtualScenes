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
        public static string GetDevicePropertyValue(int DeviceId, string SettingName)
        {
            using (zvsEntities2 context = new zvsEntities2(zvsEntityControl.GetzvsConnectionString))
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

        }
    }
}
