using System.Text.Json;
using System.IO;

namespace WordClockTaskbar.Models;

public class ThemeSettings
{
    public string BackgroundColor { get; set; } = "#E6202020";
    public string TextColor { get; set; } = "#FFFFFF";
    public string LabelColor { get; set; } = "#FFFFFF";
    public bool UseDarkMode { get; set; } = true;
}

public class TimezoneConfig
{
    public const int MaxTimezones = 4;
    public const int MaxLabelLength = 3;

    public List<TimezoneEntry> Timezones { get; set; } = new();
    public ThemeSettings Theme { get; set; } = new();
    public bool IsAlwaysOnTop { get; set; } = true;
    public double? CustomLeft { get; set; }
    public double? CustomTop { get; set; }

    private const string ConfigFile = "wordclock-config.json";
    private static readonly string ConfigPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "WordClockTaskbar",
        ConfigFile);

    public static TimezoneConfig Load()
    {
        try
        {
            if (File.Exists(ConfigPath))
            {
                var json = File.ReadAllText(ConfigPath);
                var config = JsonSerializer.Deserialize<TimezoneConfig>(json) ?? GetDefaults();
                config.Normalize();
                return config;
            }
        }
        catch { }
        return GetDefaults();
    }

    public void Save()
    {
        try
        {
            Normalize();
            var dir = Path.GetDirectoryName(ConfigPath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir!);

            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigPath, json);
        }
        catch { }
    }

    public static string NormalizeLabel(string? label, int index = 0)
    {
        var normalized = (label ?? string.Empty).Trim().ToUpperInvariant();
        if (normalized.Length > MaxLabelLength)
            normalized = normalized[..MaxLabelLength];

        return string.IsNullOrWhiteSpace(normalized) ? $"T{index + 1}" : normalized;
    }

    private void Normalize()
    {
        Theme ??= new ThemeSettings();
        Timezones ??= new List<TimezoneEntry>();

        Timezones = Timezones
            .Where(entry => entry is not null)
            .OrderBy(entry => entry.Order)
            .Take(MaxTimezones)
            .ToList();

        if (Timezones.Count == 0)
        {
            Timezones = GetDefaultTimezones();
            return;
        }

        for (var i = 0; i < Timezones.Count; i++)
        {
            var entry = Timezones[i];
            entry.Label = NormalizeLabel(entry.Label, i);
            entry.TimezoneId = TimeZoneInfo.TryFindSystemTimeZoneById(entry.TimezoneId, out _)
                ? entry.TimezoneId
                : TimeZoneInfo.Utc.Id;
            entry.Order = i;
        }
    }

    private static TimezoneConfig GetDefaults()
    {
        return new TimezoneConfig
        {
            Timezones = GetDefaultTimezones()
        };
    }

    private static List<TimezoneEntry> GetDefaultTimezones() =>
    [
        new() { Label = "US", TimezoneId = "Eastern Standard Time", Order = 0 },
        new() { Label = "UK", TimezoneId = "GMT Standard Time", Order = 1 },
        new() { Label = "DIA", TimezoneId = "India Standard Time", Order = 2 },
        new() { Label = "JPN", TimezoneId = "Tokyo Standard Time", Order = 3 }
    ];
}

public class TimezoneEntry
{
    public string Label { get; set; } = "";
    public string TimezoneId { get; set; } = "";
    public int Order { get; set; }
}
