using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace zvs.DataModel
{
    [Table("DeviceValueHistories", Schema = "ZVS")]
    public class DeviceValueHistory : IIdentity
    {
        public DeviceValueHistory()
        {
            DateTime = DateTime.Now;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int DeviceValueId { get; set; }
        public virtual DeviceValue DeviceValue { get; set; }

        [StringLength(512)]
        public string Value { get; set; }

        public DateTime DateTime { get; set; }

        [NotMapped]
        public DateTimeOffset DateTimeOffset
        {
            get { return new DateTimeOffset(DateTime, TimeSpan.FromHours(0)); }
            set { DateTime = value.UtcDateTime; }
        }
    }
}
