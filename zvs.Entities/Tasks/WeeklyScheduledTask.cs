namespace zvs.DataModel.Tasks
{
    public class WeeklyScheduledTask : ScheduledTask
    {
        public int EveryXWeek { get; set; }
        public DaysOfWeek ReccurDays { get; set; }
    }
}
