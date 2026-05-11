using System.Windows;
using WordClockTaskbar.Models;
using WordClockTaskbar.ViewModels;

namespace WordClockTaskbar;

public partial class SettingsWindow : Window
{
    private SettingsViewModel ViewModel => (SettingsViewModel)DataContext;

    public SettingsWindow()
    {
        InitializeComponent();
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.Timezones.Count >= 5)
        {
            System.Windows.MessageBox.Show("Maximum 5 timezones allowed.", "Cannot Add", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var firstAvailableTimezone = ViewModel.AvailableTimezoneIds.FirstOrDefault() ?? "UTC";
        ViewModel.AddTimezone("NEW", firstAvailableTimezone);
    }

    private void RemoveButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement fe && fe.DataContext is TimezoneEntry entry)
        {
            ViewModel.RemoveTimezone(entry);
        }
    }

    private void MoveUpButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement fe && fe.DataContext is TimezoneEntry entry)
        {
            ViewModel.MoveUp(entry);
        }
    }

    private void MoveDownButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement fe && fe.DataContext is TimezoneEntry entry)
        {
            ViewModel.MoveDown(entry);
        }
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.SaveConfig();

        if (System.Windows.Application.Current.MainWindow is MainWindow mw)
        {
            mw.Topmost = ViewModel.IsAlwaysOnTop;
        }

        DialogResult = true;
        Close();
    }
}
