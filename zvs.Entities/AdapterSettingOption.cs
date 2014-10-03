using System.ComponentModel.DataAnnotations.Schema;

namespace zvs.Entities
{
    [Table("AdapterSettingOptions", Schema = "ZVS")]
    public partial class AdapterSettingOption : BaseOption, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int AdapterSettingId { get; set; }
        public virtual AdapterSetting Setting { get; set; }
    }
}
