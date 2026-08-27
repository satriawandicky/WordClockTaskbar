using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WordClockTaskbar.Helpers;
using WordClockTaskbar.Models;
using ImageSource = System.Windows.Media.ImageSource;

namespace WordClockTaskbar.ViewModels;

public class TimezoneDisplayModel : INotifyPropertyChanged
{
    private string _label = "";
    private string _time = "";
    private string _phase = "";
    private string _gmtOffset = "";
    private string _timezoneId = "";
    private ImageSource? _flag;
    private TimezoneClockModel? _clockModel;

    public string Label { get => _label; set { _label = value; OnPropertyChanged(); } }
    public string Time { get => _time; set { _time = value; OnPropertyChanged(); } }
    // Time-of-day phase key ("sunrise"/"noon"/"sunset"/"night"); drives the icon in XAML.
    public string Phase { get => _phase; set { _phase = value; OnPropertyChanged(); } }
    public string GMTOffset { get => _gmtOffset; set { _gmtOffset = value; OnPropertyChanged(); } }
    public string TimezoneId { get => _timezoneId; set { _timezoneId = value; OnPropertyChanged(); } }

    // Country flag for this timezone; null when unmapped -> UI shows the text label.
    public ImageSource? Flag { get => _flag; set { _flag = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasFlag)); } }
    public bool HasFlag => _flag != null;

    public TimezoneDisplayModel(string label, string timezoneId)
    {
        Label = label;
        TimezoneId = timezoneId;
        _clockModel = new TimezoneClockModel(label, timezoneId);
        GMTOffset = _clockModel.GetGMTOffset();
        Flag = LoadFlag(timezoneId);
    }

    private static ImageSource? LoadFlag(string timezoneId)
    {
        var iso = TimezoneCountry.GetIso2(timezoneId);
        if (iso is null)
            return null;
        try
        {
            var uri = new Uri($"pack://application:,,,/Resources/flags/{iso}.png", UriKind.Absolute);
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = uri;
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.EndInit();
            bmp.Freeze();
            return bmp;
        }
        catch
        {
            return null;
        }
    }

    public void UpdateTime()
    {
        Time = _clockModel?.GetCurrentTime() ?? "";
        Phase = _clockModel?.GetTimeOfDayPhase() ?? "";
        GMTOffset = _clockModel?.GetGMTOffset() ?? "";
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public class TimezoneColumnModel : INotifyPropertyChanged
{
    private TimezoneDisplayModel? _topClock;
    private TimezoneDisplayModel? _bottomClock;

    public TimezoneDisplayModel? TopClock
    {
        get => _topClock;
        set { _topClock = value; OnPropertyChanged(); }
    }

    public TimezoneDisplayModel? BottomClock
    {
        get => _bottomClock;
        set { _bottomClock = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public class ClockViewModel : INotifyPropertyChanged
{
    private readonly DispatcherTimer _timer;
    public ObservableCollection<TimezoneDisplayModel> Timezones { get; } = new();
    public ObservableCollection<TimezoneColumnModel> Columns { get; } = new();

    public ClockViewModel()
    {
        var config = TimezoneConfig.Load();
        foreach (var entry in config.Timezones.OrderBy(t => t.Order))
        {
            Timezones.Add(new TimezoneDisplayModel(entry.Label, entry.TimezoneId));
        }

        UpdateTimes();
        UpdateColumns();

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
        UpdateColumns();
    }

    private void UpdateColumns()
    {
        Columns.Clear();
        for (int i = 0; i < Timezones.Count; i += 2)
        {
            var top = Timezones[i];
            var bottom = (i + 1 < Timezones.Count) ? Timezones[i + 1] : null;
            Columns.Add(new TimezoneColumnModel { TopClock = top, BottomClock = bottom });
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
