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
    [Table("DevicePropertyOptions", Schema = "ZVS")]
    public partial class DevicePropertyOption : BaseOption, IIdentity
    {
        public int Id { get; set; }

        public int? DevicePropertyId { get; set; }
        public virtual DeviceProperty DeviceProperty { get; set; }
    }
}
