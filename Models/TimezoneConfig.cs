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
    public List<TimezoneEntry> Timezones { get; set; } = new();
    public ThemeSettings Theme { get; set; } = new();
    public bool IsAlwaysOnTop { get; set; } = true;

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
                return JsonSerializer.Deserialize<TimezoneConfig>(json) ?? GetDefaults();
            }
        }
        catch { }
        return GetDefaults();
    }

    public void Save()
    {
        try
        {
            var dir = Path.GetDirectoryName(ConfigPath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir!);

            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigPath, json);
        }
        catch { }
    }

    private static TimezoneConfig GetDefaults()
    {
        return new TimezoneConfig
        {
            Timezones = new()
            {
                new() { Label = "US", TimezoneId = "Eastern Standard Time", Order = 0 },
                new() { Label = "UK", TimezoneId = "GMT Standard Time", Order = 1 },
                new() { Label = "IN", TimezoneId = "India Standard Time", Order = 2 }
            }
        };
    }
}

public class TimezoneEntry
{
    public string Label { get; set; } = "";
    public string TimezoneId { get; set; } = "";
    public int Order { get; set; }
}
