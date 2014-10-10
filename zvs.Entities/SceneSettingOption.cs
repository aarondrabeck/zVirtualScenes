using System.ComponentModel.DataAnnotations.Schema;

namespace zvs.Entities
{
    [Table("SceneSettingOptions", Schema = "ZVS")]
    public class SceneSettingOption : BaseOption, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int SceneSettingId { get; set; }
        public virtual SceneSetting SceneSetting { get; set; }
    }
}
