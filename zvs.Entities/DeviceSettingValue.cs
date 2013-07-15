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
    [Table("DeviceSettingValues", Schema = "ZVS")]
    public partial class DeviceSettingValue : INotifyPropertyChanged, IIdentity
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

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        
    }
}
