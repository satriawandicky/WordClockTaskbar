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

    // Time-of-day phase from the local hour. WPF can't render color emoji, so the
    // UI draws a distinct colored vector icon per phase (see MainWindow.xaml):
    //   sunrise = pagi/matahari terbit (05-10), noon = siang/matahari terik (11-15),
    //   sunset  = sore/matahari terbenam (16-18), night = malam/bulan sabit (19-04).
    public string GetTimeOfDayPhase()
    {
        var now = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, TimezoneInfo);
        return now.Hour switch
        {
            >= 5 and < 11 => "sunrise",
            >= 11 and < 16 => "noon",
            >= 16 and < 19 => "sunset",
            _ => "night"
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
