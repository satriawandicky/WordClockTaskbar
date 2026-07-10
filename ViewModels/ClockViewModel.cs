using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using WordClockTaskbar.Models;

namespace WordClockTaskbar.ViewModels;

public class TimezoneDisplayModel : INotifyPropertyChanged
{
    private string _label = "";
    private string _time = "";
    private string _emoji = "";
    private string _gmtOffset = "";
    private string _timezoneId = "";
    private TimezoneClockModel? _clockModel;

    public string Label { get => _label; set { _label = value; OnPropertyChanged(); } }
    public string Time { get => _time; set { _time = value; OnPropertyChanged(); } }
    public string Emoji { get => _emoji; set { _emoji = value; OnPropertyChanged(); } }
    public string GMTOffset { get => _gmtOffset; set { _gmtOffset = value; OnPropertyChanged(); } }
    public string TimezoneId { get => _timezoneId; set { _timezoneId = value; OnPropertyChanged(); } }

    public TimezoneDisplayModel(string label, string timezoneId)
    {
        Label = label;
        TimezoneId = timezoneId;
        _clockModel = new TimezoneClockModel(label, timezoneId);
        GMTOffset = _clockModel.GetGMTOffset();
    }

    public void UpdateTime()
    {
        Time = _clockModel?.GetCurrentTime() ?? "";
        Emoji = _clockModel?.GetTimeOfDayEmoji() ?? "";
        GMTOffset = _clockModel?.GetGMTOffset() ?? "";
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public class ClockViewModel : INotifyPropertyChanged
{
    private readonly DispatcherTimer _timer;
    public ObservableCollection<TimezoneDisplayModel> Timezones { get; } = new();

    public ClockViewModel()
    {
        var config = TimezoneConfig.Load();
        foreach (var entry in config.Timezones.OrderBy(t => t.Order))
        {
            Timezones.Add(new TimezoneDisplayModel(entry.Label, entry.TimezoneId));
        }

        UpdateTimes();

        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += (_, _) => UpdateTimes();
        _timer.Start();
    }

    private void UpdateTimes()
    {
        foreach (var tz in Timezones)
            tz.UpdateTime();
    }

    public void ReloadConfig()
    {
        var config = TimezoneConfig.Load();
        Timezones.Clear();
        foreach (var entry in config.Timezones.OrderBy(t => t.Order))
        {
            Timezones.Add(new TimezoneDisplayModel(entry.Label, entry.TimezoneId));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
