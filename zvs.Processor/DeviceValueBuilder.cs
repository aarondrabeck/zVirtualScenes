using System;
using System.Threading;
using System.Threading.Tasks;
using zvs.DataModel;
using System.Data.Entity;

namespace zvs.Processor
{
    public class DeviceValueBuilder
    {
        private IFeedback<LogEntry> Log { get; set; }
        private ZvsContext Context { get; set; }
        public DeviceValueBuilder(IFeedback<LogEntry> log, ZvsContext zvsContext)
        {
            Context = zvsContext;
            Log = log;
        }
        public async Task RegisterAsync(DeviceValue deviceValue, Device device, CancellationToken cancellationToken, bool ignoreValueChange = false)
        {
            if (device == null)
            {
                await Log.ReportErrorAsync("You must send a device when registering a device value!", cancellationToken);
                return;
            }

            var existingDv = await Context.DeviceValues.FirstOrDefaultAsync(o => o.UniqueIdentifier == deviceValue.UniqueIdentifier
                && o.DeviceId == device.Id, cancellationToken);

            if (existingDv == null)
            {
                //NEW VALUE
                Context.DeviceValues.Add(deviceValue);

                var result = await Context.TrySaveChangesAsync(cancellationToken);
                if (result.HasError)
                    await Log.ReportErrorAsync(result.Message, cancellationToken);
            }
            else
            {
                //LOG IT
                string deviceName;
                if (String.IsNullOrEmpty(device.Name))
                    deviceName = "Device #" + device.Id;
                else
                    deviceName = device.Name;

                var changed = false;

                //CHANGED VALUE
                var prevValue = existingDv.Value;

                //values come in blank sometimes.  If they are blank, keep the DB value. 
                if (!ignoreValueChange && !string.IsNullOrEmpty(deviceValue.Value))
                {
                    if (existingDv.Value != deviceValue.Value)
                    {
                        existingDv.Value = deviceValue.Value;
                        changed = true;
                    }
                }

                if (existingDv.ValueType != deviceValue.ValueType)
                {
                    await Log.ReportInfoFormatAsync(cancellationToken, "{0} {1} ValueType changed from {2} to {3}.", deviceName, deviceValue.Name, existingDv.ValueType, deviceValue.ValueType);
                    existingDv.ValueType = deviceValue.ValueType;
                    changed = true;
                }

                if (existingDv.CommandClass != deviceValue.CommandClass)
                {
                    await Log.ReportInfoFormatAsync(cancellationToken, "{0} {1} CommandClass changed from {2} to {3}.", deviceName, deviceValue.Name, existingDv.CommandClass, deviceValue.CommandClass);
                    existingDv.CommandClass = deviceValue.CommandClass;
                    changed = true;
                }

                if (existingDv.Description != deviceValue.Description)
                {
                    await Log.ReportInfoFormatAsync(cancellationToken, "{0} {1} Description changed from {2} to {3}.", deviceName, deviceValue.Name, existingDv.Description, deviceValue.Description);
                    existingDv.Description = deviceValue.Description;
                    changed = true;
                }

                if (existingDv.Name != deviceValue.Name)
                {
                    await Log.ReportInfoFormatAsync(cancellationToken, "{0} {1} Name changed from {2} to {3}.", deviceName, deviceValue.Name, existingDv.Name, deviceValue.Name);
                    existingDv.Name = deviceValue.Name;
                    changed = true;
                }

                if (existingDv.Genre != deviceValue.Genre)
                {
                    await Log.ReportInfoFormatAsync(cancellationToken,"{0} {1} Genre changed from {2} to {3}.", deviceName, deviceValue.Name, existingDv.Genre, deviceValue.Genre);
                    existingDv.Genre = deviceValue.Genre;
                    changed = true;
                }

                if (existingDv.Index != deviceValue.Index)
                {
                    await Log.ReportInfoFormatAsync(cancellationToken,"{0} {1} Index changed from {2} to {3}.", deviceName, deviceValue.Name, existingDv.Index, deviceValue.Index);
                    existingDv.Index = deviceValue.Index;
                    changed = true;
                }

                if (existingDv.IsReadOnly != deviceValue.IsReadOnly)
                {
                    await Log.ReportInfoFormatAsync(cancellationToken,"{0} {1} isReadOnly changed from {2} to {3}.", deviceName, deviceValue.Name, existingDv.IsReadOnly, deviceValue.IsReadOnly);
                    existingDv.IsReadOnly = deviceValue.IsReadOnly;
                    changed = true;
                }

                if (existingDv.CustomData1 != deviceValue.CustomData1)
                {
                    await Log.ReportInfoFormatAsync(cancellationToken,"{0} {1} CustomData1 changed from {2} to {3}.", deviceName, deviceValue.Name, existingDv.CustomData1, deviceValue.CustomData1);
                    existingDv.CustomData1 = deviceValue.CustomData1;
                    changed = true;
                }

                if (existingDv.CustomData2 != deviceValue.CustomData2)
                {
                    await Log.ReportInfoFormatAsync(cancellationToken,"{0} {1} CustomData2 changed from {2} to {3}.", deviceName, deviceValue.Name, existingDv.CustomData2, deviceValue.CustomData2);
                    existingDv.CustomData2 = deviceValue.CustomData2;
                    changed = true;
                }

                if (changed)
                {
                    var result = await Context.TrySaveChangesAsync(cancellationToken);
                    if (result.HasError)
                        await Log.ReportErrorAsync(result.Message, cancellationToken);
                }

                if (!ignoreValueChange && !string.IsNullOrEmpty(deviceValue.Value) && (string.IsNullOrEmpty(prevValue) || !prevValue.Equals(deviceValue.Value)))
                {
                    if (!String.IsNullOrEmpty(prevValue))
                        await Log.ReportInfoFormatAsync(cancellationToken,"{0} {1} changed from {2} to {3}.", deviceName, deviceValue.Name, prevValue, deviceValue.Value);
                    else
                        await Log.ReportInfoFormatAsync(cancellationToken,"{0} {1} changed to {2}.", deviceName, deviceValue.Name, deviceValue.Value); 

                    //Call Event

                    //TODO: THIS DOESN"T BLONG HERE
                   // deviceValue.DeviceValueDataChanged(new DeviceValue.ValueDataChangedEventArgs(existingDv.Id, deviceValue.Value, prevValue));
                }
            }


        }
    }
}
