using System;
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
            }
            else
            {
                //LOG IT
                string device_name = "Unknown";
                if (String.IsNullOrEmpty(device.Name))
                    device_name = "Device #" + device.Id;
                else
                    device_name = device.Name;

                var changed = false;

                //CHANGED VALUE
                prev_value = existing_dv.Value;

                //values come in blank sometimes.  If they are blank, keep the DB value. 
                if (!ignoreValueChange && !string.IsNullOrEmpty(deviceValue.Value))
                {
                    if (existing_dv.Value != deviceValue.Value)
                    {
                        existing_dv.Value = deviceValue.Value;
                        changed = true;
                    }
                }

                if (existing_dv.ValueType != deviceValue.ValueType)
                {
                    Core.log.InfoFormat("{0} {1} ValueType changed from {2} to {3}.", device_name, deviceValue.Name, existing_dv.ValueType, deviceValue.ValueType);
                    existing_dv.ValueType = deviceValue.ValueType;
                    changed = true;
                }

                if (existing_dv.CommandClass != deviceValue.CommandClass)
                {
                    Core.log.InfoFormat("{0} {1} CommandClass changed from {2} to {3}.", device_name, deviceValue.Name, existing_dv.CommandClass, deviceValue.CommandClass);
                    existing_dv.CommandClass = deviceValue.CommandClass;
                    changed = true;
                }

                if (existing_dv.Description != deviceValue.Description)
                {
                    Core.log.InfoFormat("{0} {1} Description changed from {2} to {3}.", device_name, deviceValue.Name, existing_dv.Description, deviceValue.Description);
                    existing_dv.Description = deviceValue.Description;
                    changed = true;
                }

                if (existing_dv.Name != deviceValue.Name)
                {
                    Core.log.InfoFormat("{0} {1} Name changed from {2} to {3}.", device_name, deviceValue.Name, existing_dv.Name, deviceValue.Name);
                    existing_dv.Name = deviceValue.Name;
                    changed = true;
                }

                if (existing_dv.Genre != deviceValue.Genre)
                {
                    Core.log.InfoFormat("{0} {1} Genre changed from {2} to {3}.", device_name, deviceValue.Name, existing_dv.Genre, deviceValue.Genre);
                    existing_dv.Genre = deviceValue.Genre;
                    changed = true;
                }

                if (existing_dv.Index != deviceValue.Index)
                {
                    Core.log.InfoFormat("{0} {1} Index changed from {2} to {3}.", device_name, deviceValue.Name, existing_dv.Index, deviceValue.Index);
                    existing_dv.Index = deviceValue.Index;
                    changed = true;
                }

                if (existing_dv.isReadOnly != deviceValue.isReadOnly)
                {
                    Core.log.InfoFormat("{0} {1} isReadOnly changed from {2} to {3}.", device_name, deviceValue.Name, existing_dv.isReadOnly, deviceValue.isReadOnly);
                    existing_dv.isReadOnly = deviceValue.isReadOnly;
                    changed = true;
                }

                if (existing_dv.CustomData1 != deviceValue.CustomData1)
                {
                    Core.log.InfoFormat("{0} {1} CustomData1 changed from {2} to {3}.", device_name, deviceValue.Name, existing_dv.CustomData1, deviceValue.CustomData1);
                    existing_dv.CustomData1 = deviceValue.CustomData1;
                    changed = true;
                }

                if (existing_dv.CustomData2 != deviceValue.CustomData2)
                {
                    Core.log.InfoFormat("{0} {1} CustomData2 changed from {2} to {3}.", device_name, deviceValue.Name, existing_dv.CustomData2, deviceValue.CustomData2);
                    existing_dv.CustomData2 = deviceValue.CustomData2;
                    changed = true;
                }

                if (changed)
                {
                    var result = await context.TrySaveChangesAsync();
                    if (result.HasError)
                        Core.log.Error(result.Message);
                }

                if (!ignoreValueChange && !string.IsNullOrEmpty(deviceValue.Value) && (string.IsNullOrEmpty(prev_value) || !prev_value.Equals(deviceValue.Value)))
                {
                    if (!String.IsNullOrEmpty(prev_value))
                        Core.log.InfoFormat("{0} {1} changed from {2} to {3}.", device_name, deviceValue.Name, prev_value, deviceValue.Value);
                    else
                        Core.log.InfoFormat("{0} {1} changed to {2}.", device_name, deviceValue.Name, deviceValue.Value); 

                    //Call Event
                    deviceValue.DeviceValueDataChanged(new DeviceValue.ValueDataChangedEventArgs(existing_dv.Id, deviceValue.Value, prev_value));
                }
            }


        }
    }
}
