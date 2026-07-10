namespace WordClockTaskbar.Models;

public class TimezoneClockModel
{
    public string Label { get; }
    public TimeZoneInfo TimezoneInfo { get; }

    public TimezoneClockModel(string label, string timezoneId)
    {
        Label = label;
        TimezoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
    }

    public string GetCurrentTime()
    {
        var now = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, TimezoneInfo);
        return now.ToString("HH:mm");
    }

    // Time-of-day emoji based on the local hour in this timezone.
    // pagi/morning 05-10 -> sunrise, siang/day 11-15 -> sun,
    // sore/evening 16-18 -> sunset, malam/night 19-04 -> moon.
    public string GetTimeOfDayEmoji()
    {
        var now = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, TimezoneInfo);
        return now.Hour switch
        {
            >= 5 and < 11 => "\U0001F305",   // 🌅 morning / pagi
            >= 11 and < 16 => "☀️", // sun - day / siang
            >= 16 and < 19 => "\U0001F307",  // 🌇 evening / sore
            _ => "\U0001F319"                // 🌙 night / malam
        };
    }

    public string GetGMTOffset()
    {
        var offset = TimezoneInfo.GetUtcOffset(DateTime.Now);
        var sign = offset >= TimeSpan.Zero ? "+" : "-";
        var absOffset = offset.Duration();

        if (absOffset.Minutes == 0)
            return $"GMT{sign}{(int)absOffset.TotalHours}";
        else
            return $"GMT{sign}{(int)absOffset.TotalHours}:{absOffset.Minutes:D2}";
    }
}
