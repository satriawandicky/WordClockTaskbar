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
        TaskbarHelper.PositionNearClock(this);

        var config = WordClockTaskbar.Models.TimezoneConfig.Load();
        Topmost = config.IsAlwaysOnTop;

        Closing += (s, e) =>
        {
            e.Cancel = true;
            Hide();
        };
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
