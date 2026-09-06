using System.Globalization;
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

        UtcInputTextBox.Text = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
        var preferredTimezone = TimezoneConfig.Load().Timezones.FirstOrDefault()?.TimezoneId;
        ConverterTimezoneComboBox.SelectedValue = preferredTimezone ?? TimeZoneInfo.Utc.Id;
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.Timezones.Count >= TimezoneConfig.MaxTimezones)
        {
            System.Windows.MessageBox.Show("Maximum 4 timezones allowed.", "Cannot Add", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var firstAvailableTimezone = ViewModel.AvailableTimezones.FirstOrDefault()?.Id ?? TimeZoneInfo.Utc.Id;
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

    private void ConvertUtcButton_Click(object sender, RoutedEventArgs e)
    {
        if (!DateTime.TryParse(
                UtcInputTextBox.Text,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AllowWhiteSpaces,
                out var parsedUtc))
        {
            ConversionResultTextBlock.Text = "Use format yyyy-MM-dd HH:mm, for example 2026-09-06 12:30.";
            return;
        }

        if (ConverterTimezoneComboBox.SelectedValue is not string timezoneId ||
            !TimeZoneInfo.TryFindSystemTimeZoneById(timezoneId, out var timezone))
        {
            ConversionResultTextBlock.Text = "Select a target timezone.";
            return;
        }

        var utc = DateTime.SpecifyKind(parsedUtc, DateTimeKind.Utc);
        var converted = TimeZoneInfo.ConvertTimeFromUtc(utc, timezone);
        var offset = timezone.GetUtcOffset(utc);
        var sign = offset < TimeSpan.Zero ? "-" : "+";
        var absoluteOffset = offset.Duration();

        ConversionResultTextBlock.Text =
            $"{converted:yyyy-MM-dd HH:mm}  (UTC{sign}{absoluteOffset.Hours:D2}:{absoluteOffset.Minutes:D2})";
    }

    private void UseUtcNowButton_Click(object sender, RoutedEventArgs e)
    {
        UtcInputTextBox.Text = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
        ConvertUtcButton_Click(sender, e);
    }
}
