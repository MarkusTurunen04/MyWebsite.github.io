using System;

namespace IdleFrisbeeGolf.Core
{
    public static class NumberFormatter
    {
        private static readonly string[] Suffixes =
        {
            "", "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc"
        };

        public static string Format(double value)
        {
            var abs = Math.Abs(value);
            var index = 0;
            while (abs >= 1000d && index < Suffixes.Length - 1)
            {
                abs /= 1000d;
                value /= 1000d;
                index++;
            }

            return value.ToString(value >= 100d ? "F0" : "F1") + Suffixes[index];
        }

        public static string FormatTimeSpan(TimeSpan timeSpan)
        {
            return timeSpan.ToString(timeSpan.TotalHours >= 1 ? "hh\:mm" : "mm\:ss");
        }
    }
}
