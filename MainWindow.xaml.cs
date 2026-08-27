using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WordClockTaskbar.Helpers;
using WordClockTaskbar.Models;
using WordClockTaskbar.ViewModels;

namespace WordClockTaskbar;

public partial class MainWindow : Window
{
    private const double WindowPaddingWidth = 20;
    private const double ColumnItemWidth = 72;
    private const double MinClockWidth = 80;
    private const double MaxClockWidth = 500;

    private readonly DispatcherTimer _topmostWatchdog;
    private bool _isAlwaysOnTop = true;

    public MainWindow()
    {
        InitializeComponent();
        ApplyTheme();

        _topmostWatchdog = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
        _topmostWatchdog.Tick += (_, _) => ReassertTopmost();

        Loaded += OnLoaded;
        IsVisibleChanged += (_, _) => UpdateTopmostWatchdog();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        TaskbarHelper.SetToolWindowStyle(this);
        _isAlwaysOnTop = TimezoneConfig.Load().IsAlwaysOnTop;
        EnsureVisible();

        Closing += (s, e) =>
        {
            e.Cancel = true;
            Hide();
        };
    }

    public void EnsureVisible(bool resetPosition = false)
    {
        var config = TimezoneConfig.Load();
        _isAlwaysOnTop = config.IsAlwaysOnTop;

        FitToTimezoneCount();

        if (!resetPosition && config.CustomLeft.HasValue && config.CustomTop.HasValue)
        {
            Left = config.CustomLeft.Value;
            Top = config.CustomTop.Value;
            ClampToScreen();
        }
        else
        {
            TaskbarHelper.PositionNearClock(this);
            ClampToScreen();
        }

        WindowState = WindowState.Normal;
        Visibility = Visibility.Visible;
        Show();

        ApplyAlwaysOnTop(_isAlwaysOnTop);
    }

    public void SetAlwaysOnTop(bool enabled)
    {
        _isAlwaysOnTop = enabled;
        ApplyAlwaysOnTop(enabled);
    }

    public void RefreshLayoutAndPosition()
    {
        ApplyTheme();
        FitToTimezoneCount();
        EnsureVisible(resetPosition: false);
        ReassertTopmost();
    }

    private void FitToTimezoneCount()
    {
        if (DataContext is not ClockViewModel vm)
            return;

        int columnCount = Math.Max(1, (int)Math.Ceiling(vm.Timezones.Count / 2.0));
        Width = Math.Clamp(WindowPaddingWidth + (columnCount * ColumnItemWidth), MinClockWidth, MaxClockWidth);
    }

    private void ApplyAlwaysOnTop(bool enabled)
    {
        Topmost = enabled;
        TaskbarHelper.SetTopmost(this, enabled);
        UpdateTopmostWatchdog();
    }

    private void ReassertTopmost()
    {
        if (!_isAlwaysOnTop || !IsVisible)
            return;

        Topmost = true;
        TaskbarHelper.SetTopmost(this, true);
    }

    private void UpdateTopmostWatchdog()
    {
        if (_isAlwaysOnTop && IsVisible)
        {
            if (!_topmostWatchdog.IsEnabled)
                _topmostWatchdog.Start();
        }
        else
        {
            _topmostWatchdog.Stop();
        }
    }

    private void ClampToScreen()
    {
        var virtualLeft = SystemParameters.VirtualScreenLeft;
        var virtualTop = SystemParameters.VirtualScreenTop;
        var virtualRight = virtualLeft + SystemParameters.VirtualScreenWidth;
        var virtualBottom = virtualTop + SystemParameters.VirtualScreenHeight;

        if (double.IsNaN(Left) || Left < virtualLeft || Left + Width > virtualRight)
        {
            var tb = TaskbarHelper.GetTaskbarRectDip();
            Left = Math.Max(0, tb.Left + tb.Width - Width - 220);
        }

        if (double.IsNaN(Top) || Top < virtualTop || Top + Height > virtualBottom)
        {
            var tb = TaskbarHelper.GetTaskbarRectDip();
            Top = tb.Top + Math.Max(2, (tb.Height - Height) / 2.0);
        }
    }

    private void ApplyTheme()
    {
        var config = TimezoneConfig.Load();
        var bgColor = ThemeHelper.HexToColor(config.Theme.BackgroundColor);
        var textColor = ThemeHelper.HexToColor(config.Theme.TextColor);
        var labelColor = ThemeHelper.HexToColor(config.Theme.LabelColor);
        var borderColor = System.Windows.Media.Color.FromArgb(44, labelColor.R, labelColor.G, labelColor.B);

        RootBorder.Background = CreateBrush(bgColor);
        Resources["ForegroundColor"] = CreateBrush(textColor);
        Resources["LabelColor"] = CreateBrush(labelColor);
        Resources["ChromeBorderColor"] = CreateBrush(borderColor);
    }

    private static SolidColorBrush CreateBrush(System.Windows.Media.Color color)
    {
        var brush = new SolidColorBrush(color);
        if (brush.CanFreeze)
            brush.Freeze();

        return brush;
    }

    public void SavePosition()
    {
        try
        {
            var config = TimezoneConfig.Load();
            config.CustomLeft = Left;
            config.CustomTop = Top;
            config.Save();
        }
        catch { }
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);
        if (e.ButtonState == MouseButtonState.Pressed)
        {
            DragMove();
            SavePosition();
        }
    }

    protected override void OnDeactivated(EventArgs e)
    {
        base.OnDeactivated(e);
        ReassertTopmost();
    }
}
