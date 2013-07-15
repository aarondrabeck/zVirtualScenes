using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zvs.Entities
{
    [Table("SceneSettingOptions", Schema = "ZVS")]
    public partial class SceneSettingOption : BaseOption, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int SceneSettingId { get; set; }
        public virtual SceneSetting SceneSetting { get; set; }
    }
}
