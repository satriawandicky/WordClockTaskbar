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

    // Time-of-day indicator based on the local hour in this timezone.
    // WPF TextBlock renders emoji fonts monochrome (no COLR/CPAL), so instead of
    // color emoji we return a crisp text-presentation symbol plus an explicit
    // phase color; MainWindow paints the glyph in that color for visibility.
    //   pagi/morning 05-10  -> sun, warm amber
    //   siang/day    11-15  -> sun, bright gold
    //   sore/evening 16-18  -> sun, sunset orange
    //   malam/night  19-04  -> crescent moon, cool blue
    public (string glyph, string colorHex) GetTimeOfDay()
    {
        var now = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, TimezoneInfo);
        return now.Hour switch
        {
            >= 5 and < 11 => ("☀", "#FFB74D"),   // ☀ sunrise amber - pagi
            >= 11 and < 16 => ("☀", "#FFD23F"),  // ☀ bright gold - siang
            >= 16 and < 19 => ("☀", "#FF7043"),  // ☀ sunset orange - sore
            _ => ("☾", "#82B1FF")                // ☾ moon cool blue - malam
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
