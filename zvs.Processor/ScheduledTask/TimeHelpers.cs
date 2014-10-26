using System;

namespace zvs.Processor.ScheduledTask
{
    public static class TimeHelpers
    {
        public static bool AreTimesEqualToTheSecond(DateTime dt1, DateTime dt2)
        {
            //STRIP THE MILLISECONDS OUT
            var timeNowToTheSeconds = new TimeSpan(dt1.TimeOfDay.Hours, dt1.TimeOfDay.Minutes, dt1.TimeOfDay.Seconds);
            var triggerToTheSeconds = new TimeSpan(dt2.TimeOfDay.Hours, dt2.TimeOfDay.Minutes, dt2.TimeOfDay.Seconds);

            return timeNowToTheSeconds.Equals(triggerToTheSeconds);
        }
    }
}
