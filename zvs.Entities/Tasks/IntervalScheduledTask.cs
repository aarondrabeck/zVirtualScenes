using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Remoting.Messaging;

namespace zvs.DataModel.Tasks
{
    public class IntervalScheduledTask : ScheduledTask
    {
        public double IntervalInSeconds { get; set; }

        [NotMapped]
        public TimeSpan Inteval
        {
            get { return TimeSpan.FromSeconds(IntervalInSeconds); }
            set { IntervalInSeconds = value.TotalSeconds; }
        }
    }
}
