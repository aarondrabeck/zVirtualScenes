using System;

namespace zvs.DataModel.Tasks
{
    public interface IOneTimeScheduledTask 
    {
        DateTime StartTime { get; set; }
    }
}
