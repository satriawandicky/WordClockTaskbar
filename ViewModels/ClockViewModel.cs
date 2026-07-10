using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WordClockTaskbar.Helpers;
using WordClockTaskbar.Models;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using SolidColorBrush = System.Windows.Media.SolidColorBrush;
using ImageSource = System.Windows.Media.ImageSource;

namespace WordClockTaskbar.ViewModels;

public class TimezoneDisplayModel : INotifyPropertyChanged
{
    private string _label = "";
    private string _time = "";
    private string _emoji = "";
    private Brush _emojiBrush = Brushes.Transparent;
    private string _lastColorHex = "";
    private string _gmtOffset = "";
    private string _timezoneId = "";
    private ImageSource? _flag;
    private TimezoneClockModel? _clockModel;

    public string Label { get => _label; set { _label = value; OnPropertyChanged(); } }
    public string Time { get => _time; set { _time = value; OnPropertyChanged(); } }
    public string Emoji { get => _emoji; set { _emoji = value; OnPropertyChanged(); } }
    public Brush EmojiBrush { get => _emojiBrush; set { _emojiBrush = value; OnPropertyChanged(); } }
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
        if (_clockModel is not null)
        {
            var (glyph, colorHex) = _clockModel.GetTimeOfDay();
            Emoji = glyph;
            if (_lastColorHex != colorHex)
            {
                var brush = new SolidColorBrush(ThemeHelper.HexToColor(colorHex));
                brush.Freeze();
                EmojiBrush = brush;
                _lastColorHex = colorHex;
            }
        }
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
