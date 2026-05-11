using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WordClockTaskbar.Helpers;

namespace WordClockTaskbar;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        ApplyTheme();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        TaskbarHelper.SetToolWindowStyle(this);
        EnsureVisible();

        Closing += (s, e) =>
        {
            e.Cancel = true;
            Hide();
        };
    }

    public void EnsureVisible()
    {
        var config = WordClockTaskbar.Models.TimezoneConfig.Load();

        TaskbarHelper.PositionNearClock(this);
        ClampToScreen();

        WindowState = WindowState.Normal;
        Visibility = Visibility.Visible;
        Show();

        Topmost = false;
        Topmost = true;
        Topmost = config.IsAlwaysOnTop;
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
        var config = WordClockTaskbar.Models.TimezoneConfig.Load();
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
}
