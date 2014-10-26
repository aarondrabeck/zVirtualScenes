using System;
using zvs.DataModel.Tasks;

namespace zvs.Processor.ScheduledTask
{
    public static class WeeklyTriggerExtensionMethods
    {
        public static bool EvalTrigger(this WeeklyScheduledTask task, ITimeProvider timeProvider)
        {
            if (task.EveryXWeek <= 0 || (((Int32)(timeProvider.Time.Date - task.StartTime.Date).TotalDays / 7) % task.EveryXWeek != 0)) return false;
            var dayOfWeek = (int)timeProvider.Time.DayOfWeek;
            var daysOfWeek = (DaysOfWeek)(1 << dayOfWeek);
            return task.ReccurDays.HasFlag(daysOfWeek) && TimeHelpers.AreTimesEqualToTheSecond(timeProvider.Time, task.StartTime);
        }
    }
}
