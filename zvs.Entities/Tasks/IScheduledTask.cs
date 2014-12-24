using System;

namespace zvs.DataModel.Tasks
{
    public interface IScheduledTask 
    {
        DateTime StartTime { get; set; }

        ZvsScheduledTask ZvsScheduledTask { get; set; }
    }
}
