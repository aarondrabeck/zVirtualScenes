using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace zvs.Entities
{
    [Table("DeviceSettingValues", Schema = "ZVS")]
    public partial class DeviceSettingValue : INotifyPropertyChanged, IIdentity, IValidatableObject
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int DeviceId { get; set; }
        public virtual Device Device { get; set; }

        public int DeviceSettingId { get; set; }
        public virtual DeviceSetting DeviceSetting { get; set; }

        private string _Value;
        [StringLength(512)]
        public string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                if (value != _Value)
                {
                    _Value = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //TODO: MOVE 
        public static async Task<string> GetDevicePropertyValueAsync(zvsContext context, Device device, string SettingName)
        {
            Device d2 = await context.Devices
                .Include(o=> o.DeviceSettingValues)
                .FirstOrDefaultAsync(o => o.Id == device.Id);
            if (d2 == null)
                return string.Empty;

            //See if the custom value is set.
            var dpv = d2.DeviceSettingValues.FirstOrDefault(o => o.DeviceSetting.UniqueIdentifier == SettingName);
            if (dpv != null)
                return dpv.Value;

            //default value from property
            var dp = await context.DeviceSettings.FirstOrDefaultAsync(o => o.UniqueIdentifier == SettingName);
            if (dp != null)
                return dp.Value;

            return string.Empty;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            using (var context = new zvsContext())
                if (context.DeviceSettingValues.Any(o => o.DeviceId == this.DeviceId && 
                    o.DeviceSettingId == this.DeviceSettingId && 
                    o.Id != this.Id))  //Check o.Id != this.Id so updates do not fail
                    results.Add(new ValidationResult("Device Setting Value name already exists", new[] { "Name" }));

            return results;
        }
    }
}
