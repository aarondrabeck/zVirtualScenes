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
    [Table("DeviceTypeSettingOptions", Schema = "ZVS")]
    public partial class DeviceTypeSettingOption : BaseOption, IIdentity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? DeviceTypeSettingId { get; set; }
        public virtual DeviceTypeSetting DeviceTypeSetting { get; set; }
    }
}
