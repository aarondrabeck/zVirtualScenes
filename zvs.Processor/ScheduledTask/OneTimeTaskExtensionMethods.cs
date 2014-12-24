using zvs.DataModel.Tasks;

namespace zvs.Processor.ScheduledTask
{
    public static class OneTimeTriggerExtensionMethods
    {
        public static bool EvalOneTimeTrigger(this IOneTimeScheduledTask task, ITimeProvider timeProvider)
        {
            return timeProvider.Time >= task.StartTime && TimeHelpers.AreTimesEqualToTheSecond(timeProvider.Time, task.StartTime);
        }
    }
}
