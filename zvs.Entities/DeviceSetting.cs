using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace zvs.Entities
{
    [Table("DeviceSettings", Schema = "ZVS")]
    public partial class DeviceSetting : BaseValue, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        private ObservableCollection<DeviceSettingOption> _Options = new ObservableCollection<DeviceSettingOption>();
        public virtual ObservableCollection<DeviceSettingOption> Options
        {
            get { return _Options; }
            set { _Options = value; }
        }
    }
}
