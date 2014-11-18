using System;

namespace zvs
{
    public interface ITimeProvider
    {
        DateTime Time { get; }
    }
}
