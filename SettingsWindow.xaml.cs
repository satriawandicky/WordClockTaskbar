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
        if (ViewModel.Timezones.Count >= TimezoneConfig.MaxTimezones)
        {
            System.Windows.MessageBox.Show("Maximum 4 timezones allowed.", "Cannot Add", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var firstAvailableTimezone = ViewModel.AvailableTimezoneIds.FirstOrDefault() ?? "UTC";
        ViewModel.AddTimezone("NEW", firstAvailableTimezone);
    }

    private void RemoveButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement fe && fe.DataContext is TimezoneEntry entry)
        {
            if (ViewModel.Timezones.Count <= 1)
            {
                System.Windows.MessageBox.Show("Keep at least one timezone.", "Cannot Remove", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
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
            mw.SetAlwaysOnTop(ViewModel.IsAlwaysOnTop);
        }

        DialogResult = true;
        Close();
    }
}
