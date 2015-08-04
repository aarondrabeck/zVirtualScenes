using System.ComponentModel.DataAnnotations.Schema;

namespace zvs.DataModel
{
    [Table("DeviceSettingOptions", Schema = "ZVS")]
    public class DeviceSettingOption : BaseOption, IIdentity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? DeviceSettingId { get; set; }
        public virtual DeviceSetting DeviceSetting { get; set; }
    }
}
