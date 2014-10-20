using System;

namespace zvs
{
    public class CurrentTimeProvider : ITimeProvider
    {
        public DateTime Time
        {
            get { return DateTime.Now; }
        }
    }
}
