using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace zvs.DataModel
{
    [Table("DeviceTypeSettings", Schema = "ZVS")]
    public class DeviceTypeSetting : BaseValue, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int DeviceTypeId { get; set; }
        public virtual DeviceType DeviceType { get; set; }

        private ObservableCollection<DeviceTypeSettingOption> _options = new ObservableCollection<DeviceTypeSettingOption>();
        public virtual ObservableCollection<DeviceTypeSettingOption> Options
        {
            get { return _options; }
            set { _options = value; }
        }
    }
}
