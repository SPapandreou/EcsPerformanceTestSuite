using System;

namespace Core.Statistics
{
    public static class TimeSpanExtensions
    {
        public static string CompoundFormat(this TimeSpan timeSpan)
        {
            return
                $"{timeSpan.Seconds}s {timeSpan.Milliseconds}ms {timeSpan.Ticks % TimeSpan.TicksPerMillisecond / 10}us";
        }
    }
}