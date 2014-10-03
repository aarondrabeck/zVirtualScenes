using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace zvs.Entities
{
    public partial class Device
    {

        [NotMapped]
        public DateTimeOffset? EdmLastHeardFrom
        {
            // Assume the CreateOn property stores UTC time.
            get
            {
                return LastHeardFrom.HasValue ? new DateTimeOffset(LastHeardFrom.Value, TimeSpan.FromHours(0)) : (DateTimeOffset?)null;
            }
            set
            {
                LastHeardFrom = value.HasValue ? value.Value.UtcDateTime : (DateTime?)null;
            }
        }
    }
}
