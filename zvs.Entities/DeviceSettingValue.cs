using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace zvs.DataModel
{
    [Table("DeviceSettingValues", Schema = "ZVS")]
    public class DeviceSettingValue : INotifyPropertyChanged, IIdentity, IValidatableObject
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int DeviceId { get; set; }
        public virtual Device Device { get; set; }

        public int DeviceSettingId { get; set; }
        public virtual DeviceSetting DeviceSetting { get; set; }

        private string _value;
        [StringLength(512)]
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value != _value)
                {
                    _value = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public static async Task<string> GetDevicePropertyValueAsync(ZvsContext context, Device device, string settingName)
        {
            var d2 = await context.Devices
                .Include(o=> o.DeviceSettingValues)
                .FirstOrDefaultAsync(o => o.Id == device.Id);
            if (d2 == null)
                return string.Empty;

            //See if the custom value is set.
            var dpv = d2.DeviceSettingValues.FirstOrDefault(o => o.DeviceSetting.UniqueIdentifier == settingName);
            if (dpv != null)
                return dpv.Value;

            //default value from property
            var dp = await context.DeviceSettings.FirstOrDefaultAsync(o => o.UniqueIdentifier == settingName);
            return dp != null ? dp.Value : string.Empty;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            using (var context = new ZvsContext())
                if (context.DeviceSettingValues.Any(o => o.DeviceId == DeviceId && 
                    o.DeviceSettingId == DeviceSettingId && 
                    o.Id != Id))  //Check o.Id != this.Id so updates do not fail
                    results.Add(new ValidationResult("Device Setting Value name already exists", new[] { "Name" }));

            return results;
        }
    }
}
