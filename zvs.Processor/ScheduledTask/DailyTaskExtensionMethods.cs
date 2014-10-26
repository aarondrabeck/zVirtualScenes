using System;
using zvs.DataModel.Tasks;

namespace zvs.Processor.ScheduledTask
{
    public static class DailyTaskExtensionMethods
    {
        public static bool EvalTrigger(this DailyScheduledTask task, ITimeProvider timeProvider)
        {
            if (task.EveryXDay <= 0 || ((Int32)(timeProvider.Time.Date - task.StartTime.Date).TotalDays % task.EveryXDay != 0))
                return false;

            return TimeHelpers.AreTimesEqualToTheSecond(timeProvider.Time, task.StartTime);
        }
    }
}
