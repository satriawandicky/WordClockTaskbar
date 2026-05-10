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
