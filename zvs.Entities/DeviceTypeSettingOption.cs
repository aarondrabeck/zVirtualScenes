using System.ComponentModel.DataAnnotations.Schema;

namespace zvs.DataModel
{
    [Table("DeviceTypeSettingOptions", Schema = "ZVS")]
    public class DeviceTypeSettingOption : BaseOption, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? DeviceTypeSettingId { get; set; }
        public virtual DeviceTypeSetting DeviceTypeSetting { get; set; }
    }
}
