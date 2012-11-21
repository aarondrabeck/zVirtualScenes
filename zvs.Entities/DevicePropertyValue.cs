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
    [Table("DevicePropertyValues", Schema = "ZVS")]
    public partial class DevicePropertyValue : INotifyPropertyChanged, IIdentity
    {
        public int Id { get; set; }

        
        public int DeviceId { get; set; }
        [Required]
        public virtual Device Device { get; set; }

        
        public int DevicePropertyId { get; set; }
        [Required]
        public virtual DeviceProperty DeivceProperty { get; set; }

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
                    NotifyPropertyChanged("Value");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public static string GetDevicePropertyValue(zvsContext context, Device device, string SettingName)
        {
            Device d2 = context.Devices.FirstOrDefault(o => o.Id == device.Id);
            if (d2 == null)
                return string.Empty;

            //See if the custom value is set.
            DevicePropertyValue dpv = d2.PropertyValues.FirstOrDefault(o => o.DeivceProperty.UniqueIdentifier == SettingName);
            if (dpv != null)
                return dpv.Value;

            //default value from property
            DeviceProperty dp = context.DeviceProperties.FirstOrDefault(o => o.UniqueIdentifier == SettingName);
            if (dp != null)
                return dp.Value; 

            return string.Empty;
        }
    }
}
