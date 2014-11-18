namespace zvs.DataModel.Tasks
{
    public class WeeklyScheduledTask : ScheduledTask
    {
        public int RepeatIntervalInWeeks { get; set; }
        public DaysOfWeek DaysOfWeekToActivate { get; set; }
    }
}
