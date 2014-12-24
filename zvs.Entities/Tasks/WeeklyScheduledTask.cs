using System;

namespace zvs.DataModel.Tasks
{
    public interface IWeeklyScheduledTask
    {
        DateTime StartTime { get; set; }
        int RepeatIntervalInWeeks { get; set; }
        DaysOfWeek DaysOfWeekToActivate { get; set; }
    }
}
