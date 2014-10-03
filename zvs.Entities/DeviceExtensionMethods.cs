using System.Threading.Tasks;
using System.Data.Entity;

namespace zvs.Entities
{
    public static class DeviceExtensionMethods
    {
        public static async Task<string> GetDeviceTypeValueAsync(this Device device, string DeviceTypeSettingUniqueIdentifier, zvsContext context)
        {
            //check if the value has been defined by the user for this device
            var definedSetting = await context.DeviceTypeSettingValues.FirstOrDefaultAsync(o =>
                                        o.DeviceTypeSetting.UniqueIdentifier == DeviceTypeSettingUniqueIdentifier &&
                                        o.Device.Id == device.Id);

            if (definedSetting != null)
                return definedSetting.Value;

            //otherwise get default for the command
            var defaultSetting = await context.DeviceTypeSettings.FirstOrDefaultAsync(o =>
                o.UniqueIdentifier == DeviceTypeSettingUniqueIdentifier &&
                 o.DeviceTypeId == device.DeviceTypeId);

            if (defaultSetting != null)
                return defaultSetting.Value;

            return null;
        }

        public static async Task<string> GetDeviceSettingAsync(this Device device, string DeviceSettingUniqueIdentifier, zvsContext context)
        {
            //check if the value has been defined by the user for this device
            var definedSetting = await context.DeviceSettingValues.FirstOrDefaultAsync(o =>
                                        o.DeviceSetting.UniqueIdentifier == DeviceSettingUniqueIdentifier &&
                                        o.Device.Id == device.Id);

            if (definedSetting != null)
                return definedSetting.Value;

            //otherwise get default for the command
            var defaultSetting = await context.DeviceTypeSettings.FirstOrDefaultAsync(o =>
                o.UniqueIdentifier == DeviceSettingUniqueIdentifier);

            if (defaultSetting != null)
                return defaultSetting.Value;

            return null;
        }
    }
}
