using zvs.DataModel.Tasks;

namespace zvs.Processor.ScheduledTask
{
    public static class OneTimeTriggerExtensionMethods
    {
        public static bool EvalTrigger(this OneTimeScheduledTask task, ITimeProvider timeProvider)
        {
            return timeProvider.Time >= task.StartTime && TimeHelpers.AreTimesEqualToTheSecond(timeProvider.Time, task.StartTime);
        }
    }
}
