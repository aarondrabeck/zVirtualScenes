using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zvs.DataModel
{
    [Table("DeviceValueHistories", Schema = "ZVS")]
    public class DeviceValueHistory : IIdentity
    {
        public DeviceValueHistory()
        {
            DateTime = DateTime.Now;
        }

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int DeviceValueId { get; set; }
        public virtual DeviceValue DeviceValue { get; set; }

        [StringLength(512)]
        public string Value { get; set; }

        public DateTime DateTime { get; set; }

        [NotMapped]
        public DateTimeOffset EdmDateTime
        {
            get { return new DateTimeOffset(DateTime, TimeSpan.FromHours(0)); }
            set { DateTime = value.UtcDateTime; }
        }
    }
}
