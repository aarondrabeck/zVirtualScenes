using System;

namespace zvs.DataModel.Tasks
{
    public interface IIntervalScheduledTask 
    {
        DateTime StartTime { get; set; }

        double RepeatIntervalInSeconds { get; set; }

        TimeSpan Inteval { get; set; }
    }
}
