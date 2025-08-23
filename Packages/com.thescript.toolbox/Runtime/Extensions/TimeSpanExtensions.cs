using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Mtl.Toolbox
{
    [PublicAPI]
    public static class TimeSpanExtensions
    {
        private static readonly IReadOnlyList<string> Formats = new[]
        {
            "%d'd'",
            "%h'h'",
            "%m'm'",
            "%s's'"
        };

        /// <summary>
        /// Converts a timespan into a timer string with an optional precision level
        /// </summary>
        public static string ToTimerText(this in TimeSpan span, int precision = 2)
        {
            int fromFormat;
            if (span.TotalDays >= 1)
            {
                fromFormat = 0;
            }
            else if (span.TotalHours >= 1)
            {
                fromFormat = 1;
            }
            else if (span.TotalMinutes >= 1)
            {
                fromFormat = 2;
            }
            else
            {
                fromFormat = 3;
            }

            using var selectedFormats = ListPool.Get<string>();
            for (var endFormat = Math.Min(Formats.Count, fromFormat + precision); fromFormat < endFormat; ++fromFormat)
            {
                selectedFormats.Add(Formats[fromFormat]);
            }

            return span.ToString(string.Join(string.Empty, selectedFormats));
        }
    }
}