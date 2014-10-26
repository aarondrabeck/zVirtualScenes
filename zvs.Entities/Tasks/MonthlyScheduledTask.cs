namespace zvs.DataModel.Tasks
{
    public class MonthlyScheduledTask : ScheduledTask
    {
        public int EveryXMonth { get; set; }
        public DaysOfMonth ReccurDays { get; set; }
        
    }
}
