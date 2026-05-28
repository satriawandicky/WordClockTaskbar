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
    private const double WindowPaddingWidth = 28;
    private const double TimezoneItemWidth = 86;
    private const double MinClockWidth = 300;
    private const double MaxClockWidth = 488;

    private readonly DispatcherTimer _topmostWatchdog;
    private bool _isAlwaysOnTop = true;

    public MainWindow()
    {
        InitializeComponent();
        ApplyTheme();

        _topmostWatchdog = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
        _topmostWatchdog.Tick += (_, _) => ReassertTopmost();

        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        TaskbarHelper.SetToolWindowStyle(this);
        _isAlwaysOnTop = TimezoneConfig.Load().IsAlwaysOnTop;
        EnsureVisible();
        _topmostWatchdog.Start();

        Closing += (s, e) =>
        {
            e.Cancel = true;
            Hide();
        };
    }

    public void EnsureVisible()
    {
        var config = TimezoneConfig.Load();
        _isAlwaysOnTop = config.IsAlwaysOnTop;

        FitToTimezoneCount();
        TaskbarHelper.PositionNearClock(this);
        ClampToScreen();

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
        TaskbarHelper.PositionNearClock(this);
        ClampToScreen();
        ReassertTopmost();
    }

    private void FitToTimezoneCount()
    {
        if (DataContext is not ClockViewModel vm)
            return;

        var visibleCount = Math.Clamp(vm.Timezones.Count, 1, 5);
        Width = Math.Clamp(WindowPaddingWidth + (visibleCount * TimezoneItemWidth), MinClockWidth, MaxClockWidth);
    }

    private void ApplyAlwaysOnTop(bool enabled)
    {
        Topmost = enabled;
        TaskbarHelper.SetTopmost(this, enabled);
    }

    private void ReassertTopmost()
    {
        if (!_isAlwaysOnTop || !IsVisible)
            return;

        Topmost = true;
        TaskbarHelper.SetTopmost(this, true);
    }

    private void ClampToScreen()
    {
        var workArea = SystemParameters.WorkArea;
        var virtualLeft = SystemParameters.VirtualScreenLeft;
        var virtualTop = SystemParameters.VirtualScreenTop;
        var virtualRight = virtualLeft + SystemParameters.VirtualScreenWidth;
        var virtualBottom = virtualTop + SystemParameters.VirtualScreenHeight;

        if (double.IsNaN(Left) || Left < virtualLeft || Left + Width > virtualRight)
            Left = workArea.Right - Width - 16;

        if (double.IsNaN(Top) || Top < virtualTop || Top + Height > virtualBottom)
            Top = workArea.Bottom - Height - 4;
    }

    private void ApplyTheme()
    {
        var config = TimezoneConfig.Load();
        var bgColor = ThemeHelper.HexToColor(config.Theme.BackgroundColor);
        RootBorder.Background = new SolidColorBrush(bgColor);

        Resources["ForegroundColor"] = new SolidColorBrush(ThemeHelper.HexToColor(config.Theme.TextColor));
        Resources["LabelColor"] = new SolidColorBrush(ThemeHelper.HexToColor(config.Theme.LabelColor));
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);
        DragMove();
    }

    protected override void OnDeactivated(EventArgs e)
    {
        base.OnDeactivated(e);
        ReassertTopmost();
    }
}
