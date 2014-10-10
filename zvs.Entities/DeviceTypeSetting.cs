using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace zvs.Entities
{
    [Table("DeviceTypeSettings", Schema = "ZVS")]
    public class DeviceTypeSetting : BaseValue, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int DeviceTypeId { get; set; }
        public virtual DeviceType DeviceType { get; set; }

        private ObservableCollection<DeviceSettingOption> _options = new ObservableCollection<DeviceSettingOption>();
        public virtual ObservableCollection<DeviceSettingOption> Options
        {
            get { return _options; }
            set { _options = value; }
        }
    }
}
