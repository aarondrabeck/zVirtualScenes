using System.ComponentModel.DataAnnotations.Schema;

namespace zvs.DataModel
{
    [Table("AdapterSettingOptions", Schema = "ZVS")]
    public class AdapterSettingOption : BaseOption, IIdentity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int AdapterSettingId { get; set; }
        public virtual AdapterSetting Setting { get; set; }
    }
}
