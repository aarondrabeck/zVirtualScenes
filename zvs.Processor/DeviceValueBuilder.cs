using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zvs.Entities;
using System.Data.Entity;

namespace zvs.Processor
{
    public class DeviceValueBuilder : AdapterBuilder
    {
        public DeviceValueBuilder(zvsAdapter zvsAdapter, Core core) : base(zvsAdapter, core) { }
        public async Task RegisterAsync(DeviceValue deviceValue, Device device, zvsContext context, bool ignoreValueChange = false)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (device == null)
            {
                Core.log.Error("You must send a device when registering a device value!");
                return;
            }

            var existing_dv = await context.DeviceValues.FirstOrDefaultAsync(o => o.UniqueIdentifier == deviceValue.UniqueIdentifier
                && o.DeviceId == device.Id);

            string prev_value = string.Empty;

            if (existing_dv == null)
            {
                //NEW VALUE
                context.DeviceValues.Add(deviceValue);

                var result = await context.TrySaveChangesAsync();
                if (result.HasError)
                    Core.log.Error(result.Message);

                //Call Event
                deviceValue.DeviceValueAdded(new System.EventArgs());
            }
            else
            {
                //CHANGED VALUE
                prev_value = existing_dv.Value;

                //values come in blank sometimes.  If they are blank, keep the DB value. 
                if (!ignoreValueChange && !string.IsNullOrEmpty(deviceValue.Value))
                    existing_dv.Value = deviceValue.Value;

                existing_dv.ValueType = deviceValue.ValueType;
                existing_dv.CommandClass = deviceValue.CommandClass;
                existing_dv.Description = deviceValue.Description;
                existing_dv.Name = deviceValue.Name;
                existing_dv.Genre = deviceValue.Genre;
                existing_dv.Index = deviceValue.Index;
                existing_dv.isReadOnly = deviceValue.isReadOnly;
                existing_dv.CustomData1 = deviceValue.CustomData1;
                existing_dv.CustomData2 = deviceValue.CustomData2;

                var result = await context.TrySaveChangesAsync();
                if (result.HasError)
                    Core.log.Error(result.Message);

                if (!ignoreValueChange && !string.IsNullOrEmpty(deviceValue.Value) && (string.IsNullOrEmpty(prev_value) || !prev_value.Equals(deviceValue.Value)))
                {
                    //LOG IT
                    string device_name = "Unknown";
                    if (String.IsNullOrEmpty(device.Name))
                        device_name = "Device #" + device.Id;
                    else
                        device_name = device.Name;

                    if (!String.IsNullOrEmpty(prev_value))
                        Core.log.InfoFormat("{0} {1} changed from {2} to {3}.", device_name, deviceValue.Name, prev_value, deviceValue.Value);//event
                    else
                        Core.log.InfoFormat("{0} {1} changed to {2}.", device_name, deviceValue.Name, deviceValue.Value); //event

                    //Call Event
                    deviceValue.DeviceValueDataChanged(new DeviceValue.ValueDataChangedEventArgs(existing_dv.Id, deviceValue.Value, prev_value));
                }
            }


        }
    }
}
