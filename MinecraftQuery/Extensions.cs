using System;

namespace MinecraftQuery
{
    internal static class Extensions
    {
        public static long ToUnixTime(this DateTime dateTime) 
            => (long) dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

        public static DateTime ToDateTime(this long unixTime)
            => DateTimeOffset.FromUnixTimeMilliseconds(unixTime).DateTime;
    }
}