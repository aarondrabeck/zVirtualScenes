using System;
using zvs.DataModel.Tasks;

namespace zvs.Processor.ScheduledTask
{
    public static class IntervalTaskExtensionMethods
    {
        public static bool EvalTrigger(this IntervalScheduledTask task, ITimeProvider timeProvider)
        {
            return !(task.Inteval.TotalSeconds < 1) && ((Int32) (timeProvider.Time - task.StartTime).TotalSeconds%(Int32) task.Inteval.TotalSeconds == 0);
        }
    }
}
