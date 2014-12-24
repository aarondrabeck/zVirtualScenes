using System;

namespace zvs.DataModel.Tasks
{
    public interface IMonthlyScheduledTask
    {
        DateTime StartTime { get; set; }

        int RepeatIntervalInMonths { get; set; }

        DaysOfMonth DaysOfMonthToActivate { get; set; }
    }
}
