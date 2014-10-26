using System;

namespace zvs.DataModel.Tasks
{
    [Flags]
    public enum DaysOfWeek
    {
        Sunday = 1 << 0,
        Monday = 1 << 1,
        Tuesday = 1 << 2,
        Wednesday = 1 << 3,
        Thursday = 1 << 4,
        Friday = 1 << 5,
        Saturday = 1 << 6,
        All = ~(-1 << 7)
    };
}
