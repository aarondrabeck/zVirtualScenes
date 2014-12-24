using System;
using zvs.DataModel.Tasks;

namespace zvs.Processor.ScheduledTask
{
    public static class WeeklyTriggerExtensionMethods
    {
        public static bool EvalWeeklyTrigger(this IWeeklyScheduledTask task, ITimeProvider timeProvider)
        {
            if (task.RepeatIntervalInWeeks <= 0 || (((Int32)(timeProvider.Time.Date - task.StartTime.Date).TotalDays / 7) % task.RepeatIntervalInWeeks != 0)) return false;
            var dayOfWeek = (int)timeProvider.Time.DayOfWeek;
            var daysOfWeek = (DaysOfWeek)(1 << dayOfWeek);
            return task.DaysOfWeekToActivate.HasFlag(daysOfWeek) && TimeHelpers.AreTimesEqualToTheSecond(timeProvider.Time, task.StartTime);
        }
    }
}
