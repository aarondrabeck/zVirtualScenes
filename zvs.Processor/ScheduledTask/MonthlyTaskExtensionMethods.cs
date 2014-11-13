using zvs.DataModel.Tasks;

namespace zvs.Processor.ScheduledTask
{
    public static class MonthlyTriggerExtensionMethods
    {
        public static bool EvalTrigger(this MonthlyScheduledTask task, ITimeProvider timeProvider)
        {
            var monthsapart = ((timeProvider.Time.Year - task.StartTime.Year) * 12) + timeProvider.Time.Month - task.StartTime.Month;

            if (task.RepeatIntervalInMonths <= 0 || monthsapart <= -1 || monthsapart % task.RepeatIntervalInMonths != 0) return false;

            var dayOfMonth = timeProvider.Time.Day;
            var daysOfMonth = (DaysOfMonth)(1 << (dayOfMonth - 1));

            return task.DaysOfMonthToActivate.HasFlag(daysOfMonth) && TimeHelpers.AreTimesEqualToTheSecond(timeProvider.Time, task.StartTime);
        }
    }
}
