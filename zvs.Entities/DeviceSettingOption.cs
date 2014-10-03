using System.ComponentModel.DataAnnotations.Schema;

namespace zvs.Entities
{
    [Table("DeviceSettingOptions", Schema = "ZVS")]
    public partial class DeviceSettingOption : BaseOption, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? DeviceSettingId { get; set; }
        public virtual DeviceSetting DeviceSetting { get; set; }
    }
}
