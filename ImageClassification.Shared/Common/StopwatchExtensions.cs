using System;
using System.Diagnostics;

namespace ImageClassification.Shared.Common
{
    public static class StopwatchExtensions
    {
        public static TimeSpan RestartPull(this Stopwatch stopwatch)
        {
            if (stopwatch is null)
                return default;

            stopwatch.Stop();
            var elapsed = stopwatch.Elapsed;
            stopwatch.Restart();
            return elapsed;
        }
    }
}
