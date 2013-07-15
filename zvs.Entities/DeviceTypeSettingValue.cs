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

namespace zvs.Entities
{
    [Table("DeviceTypeSettingValues", Schema = "ZVS")]
    public partial class DeviceTypeSettingValue : INotifyPropertyChanged, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int DeviceId { get; set; }
        public virtual Device Device { get; set; }

        public int DeviceTypeSettingId { get; set; }
        public virtual DeviceTypeSetting DeviceTypeSetting { get; set; }

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

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        //public static string GetPropertyValue(zvsContext context, Device device, string SettingName)
        //{
        //    Device d2 = context.Devices.FirstOrDefault(o => o.Id == device.Id);
        //    if (d2 == null)
        //        return string.Empty;

        //    //See if the custom value is set.
        //    DeviceSettingValue dpv = d2.SettingValues.FirstOrDefault(o => o.DeviceSetting.UniqueIdentifier == SettingName);
        //    if (dpv != null)
        //        return dpv.Value;

        //    //default value from property
        //    DeviceSetting dp = context.DeviceSettings.FirstOrDefault(o => o.UniqueIdentifier == SettingName);
        //    if (dp != null)
        //        return dp.Value;

        //    return string.Empty;
        //}
    }
}
