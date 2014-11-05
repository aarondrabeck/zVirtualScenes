using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using zvs.DataModel;

namespace zvs.Processor
{
    public class DeviceValueBuilder
    {
        private IEntityContextConnection EntityContextConnection { get; set; }
        public DeviceValueBuilder( IEntityContextConnection entityContextConnection)
        {
            if (entityContextConnection == null)
                throw new ArgumentNullException("entityContextConnection");

            EntityContextConnection = entityContextConnection;
        }
        public async Task<Result> RegisterAsync(DeviceValue deviceValue, Device device, CancellationToken cancellationToken, bool ignoreValueChange = false)
        {
            if (deviceValue == null)
                return Result.ReportError("You must send a device value when registering a device value!");

            if (device == null)
                return Result.ReportError("You must send a device when registering a device value!");

            using (var context = new ZvsContext(EntityContextConnection))
            {
                var existingDv =
                    await
                        context.DeviceValues.FirstOrDefaultAsync(o => o.UniqueIdentifier == deviceValue.UniqueIdentifier
                                                                      && o.DeviceId == device.Id, cancellationToken);

                if (existingDv == null)
                {
                    //NEW VALUE
                    context.DeviceValues.Add(deviceValue);
                    return await context.TrySaveChangesAsync(cancellationToken);
                }

                var hasChanged = false;
                PropertyChangedEventHandler action = (s, a) => hasChanged = true;
                existingDv.PropertyChanged += action;
                
                existingDv.CommandClass = deviceValue.CommandClass;
                existingDv.CustomData1 = deviceValue.CustomData1;
                existingDv.CustomData2 = deviceValue.CustomData2;
                existingDv.Genre = deviceValue.Genre;
                existingDv.Index = deviceValue.Index;
                existingDv.IsReadOnly = deviceValue.IsReadOnly;
                existingDv.Description = deviceValue.Description;
                existingDv.Name = deviceValue.Name;
                existingDv.ValueType = deviceValue.ValueType;
                existingDv.Value = deviceValue.Value;
                existingDv.IsReadOnly = deviceValue.IsReadOnly;

                existingDv.PropertyChanged -= action;

                if (hasChanged)
                {
                    return await context.TrySaveChangesAsync(cancellationToken);
                }

                return Result.ReportSuccess("Nothing to update");
            }
        }
    }
}
