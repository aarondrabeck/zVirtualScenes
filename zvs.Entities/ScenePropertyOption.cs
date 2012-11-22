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
    [Table("ScenePropertyOptions", Schema = "ZVS")]
    public partial class ScenePropertyOption : BaseOption, IIdentity
    {
        public int Id { get; set; }

        public int ScenePropertyId { get; set; }
        [Required]
        public virtual SceneProperty SceneProperty { get; set; }
    }
}
