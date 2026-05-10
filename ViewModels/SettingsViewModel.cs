using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WordClockTaskbar.Models;

namespace WordClockTaskbar.ViewModels;

public class SettingsViewModel : INotifyPropertyChanged
{
    private TimezoneConfig _config = null!;
    private string _backgroundColor = "";
    private string _textColor = "";
    private string _labelColor = "";

    public ObservableCollection<TimezoneEntry> Timezones { get; } = new();
    public List<string> AvailableTimezoneIds { get; }

    public string BackgroundColor { get => _backgroundColor; set { _backgroundColor = value; OnPropertyChanged(); } }
    public string TextColor { get => _textColor; set { _textColor = value; OnPropertyChanged(); } }
    public string LabelColor { get => _labelColor; set { _labelColor = value; OnPropertyChanged(); } }

    public SettingsViewModel()
    {
        AvailableTimezoneIds = TimeZoneInfo.GetSystemTimeZones()
            .OrderBy(tz => tz.DisplayName)
            .Select(tz => tz.Id)
            .ToList();

        LoadConfig();
    }

    public void LoadConfig()
    {
        _config = TimezoneConfig.Load();
        Timezones.Clear();
        foreach (var entry in _config.Timezones.OrderBy(t => t.Order))
        {
            Timezones.Add(entry);
        }
        BackgroundColor = _config.Theme.BackgroundColor;
        TextColor = _config.Theme.TextColor;
        LabelColor = _config.Theme.LabelColor;
    }

    public void AddTimezone(string label, string timezoneId)
    {
        if (Timezones.Count >= 5) return;
        var entry = new TimezoneEntry
        {
            Label = label,
            TimezoneId = timezoneId,
            Order = Timezones.Count
        };
        Timezones.Add(entry);
    }

    public void RemoveTimezone(TimezoneEntry entry)
    {
        Timezones.Remove(entry);
        UpdateOrder();
    }

    public void MoveUp(TimezoneEntry entry)
    {
        var idx = Timezones.IndexOf(entry);
        if (idx > 0)
        {
            Timezones.Move(idx, idx - 1);
            UpdateOrder();
        }
    }

    public void MoveDown(TimezoneEntry entry)
    {
        var idx = Timezones.IndexOf(entry);
        if (idx < Timezones.Count - 1)
        {
            Timezones.Move(idx, idx + 1);
            UpdateOrder();
        }
    }

    private void UpdateOrder()
    {
        for (int i = 0; i < Timezones.Count; i++)
            Timezones[i].Order = i;
    }

    public void SaveConfig()
    {
        _config.Timezones.Clear();
        foreach (var tz in Timezones)
            _config.Timezones.Add(tz);
        _config.Theme.BackgroundColor = BackgroundColor;
        _config.Theme.TextColor = TextColor;
        _config.Theme.LabelColor = LabelColor;
        _config.Save();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
