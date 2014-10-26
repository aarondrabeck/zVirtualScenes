using System;

namespace zvs.DataModel.Tasks
{
    public class IntervalScheduledTask : ScheduledTask
    {
        public TimeSpan Inteval { get; set; }
    }
}
