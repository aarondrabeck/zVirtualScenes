using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace zvs.DataModel.Tasks
{
    public class IntervalScheduledTask : ScheduledTask
    {
        public double RepeatIntervalInSeconds { get; set; }

        [NotMapped]
        public TimeSpan Inteval
        {
            get { return TimeSpan.FromSeconds(RepeatIntervalInSeconds); }
            set { RepeatIntervalInSeconds = value.TotalSeconds; }
        }
    }
}
