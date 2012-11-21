using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zvs.Entities
{
    [Table("DeviceProperties", Schema = "ZVS")]
    public partial class DeviceProperty : BaseValue, IIdentity
    {
        public int Id { get; set; }
        public DeviceProperty()
        {
            this.Options = new ObservableCollection<DevicePropertyOption>();
        }
        
        public virtual ObservableCollection<DevicePropertyOption> Options { get; set; }


        public static void AddOrEdit(DeviceProperty deviceProperty, zvsContext context)
        {
            DeviceProperty existing_dp = context.DeviceProperties.FirstOrDefault(d => d.UniqueIdentifier == deviceProperty.UniqueIdentifier);

            if (existing_dp == null)
            {
                context.DeviceProperties.Add(deviceProperty);
            }
            else
            {
                existing_dp.Name = deviceProperty.Name;
                existing_dp.Description = deviceProperty.Description;
                existing_dp.ValueType = deviceProperty.ValueType;
                existing_dp.Value = deviceProperty.Value;

                existing_dp.Options.ToList().ForEach(o =>
                {
                    context.DevicePropertyOptions.Remove(o);
                });
                existing_dp.Options.Clear();
                deviceProperty.Options.ToList().ForEach(o => existing_dp.Options.Add(o));
            }
            context.SaveChanges();
        }     
    }
}
