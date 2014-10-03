using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace zvs.Entities
{
    [Table("DeviceTypeSettings", Schema = "ZVS")]
    public partial class DeviceTypeSetting : BaseValue, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int DeviceTypeId { get; set; }
        public virtual DeviceType DeviceType { get; set; }

        private ObservableCollection<DeviceSettingOption> _Options = new ObservableCollection<DeviceSettingOption>();
        public virtual ObservableCollection<DeviceSettingOption> Options
        {
            get { return _Options; }
            set { _Options = value; }
        }
    }
}
