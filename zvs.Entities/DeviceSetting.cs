using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace zvs.DataModel
{
    [Table("DeviceSettings", Schema = "ZVS")]
    public class DeviceSetting : BaseValue, IIdentity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        private ObservableCollection<DeviceSettingOption> _options = new ObservableCollection<DeviceSettingOption>();
        public virtual ObservableCollection<DeviceSettingOption> Options
        {
            get { return _options; }
            set { _options = value; }
        }
    }
}
