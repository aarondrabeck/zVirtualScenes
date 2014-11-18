using System.ComponentModel.DataAnnotations.Schema;

namespace zvs.DataModel
{
    [Table("PluginSettingOptions", Schema = "ZVS")]
    public class PluginSettingOption : BaseOption, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int PluginSettingId { get; set; }
        public virtual PluginSetting Setting { get; set; }
    }
}
