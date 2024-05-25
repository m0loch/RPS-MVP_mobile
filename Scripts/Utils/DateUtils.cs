using System;
using System.Globalization;

public partial class Utils
{
    public static string GetDateLabel(DateTime? dateTime)
    {
        if (dateTime == null) return "<unknown>";
        return GetDateLabel(dateTime.Value);
    }
    public static string GetDateLabel(DateTime dateTime)
    {
        return dateTime.ToLocalTime().ToString(CultureInfo.CurrentCulture);
    }
    public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dateTime;
    }

    public static double DateTimeToUnixTimestamp(DateTime dateTime)
    {
        return (TimeZoneInfo.ConvertTimeToUtc(dateTime) - 
            new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;
    }
}