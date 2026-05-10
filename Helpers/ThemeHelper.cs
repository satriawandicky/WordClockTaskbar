using Microsoft.Win32;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;

namespace WordClockTaskbar.Helpers;

public static class ThemeHelper
{
    public static bool IsDarkMode()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            var value = key?.GetValue("AppsUseLightTheme");
            return value is int i && i == 0;
        }
        catch
        {
            return true;
        }
    }

    public static Color GetBackgroundColor()
        => IsDarkMode()
            ? Color.FromArgb(230, 32, 32, 32)
            : Color.FromArgb(230, 243, 243, 243);

    public static Color GetForegroundColor()
        => IsDarkMode() ? Colors.White : Color.FromRgb(30, 30, 30);

    public static Color GetLabelColor()
        => IsDarkMode()
            ? Color.FromRgb(150, 150, 150)
            : Color.FromRgb(100, 100, 100);

    public static Color GetSeparatorColor()
        => IsDarkMode()
            ? Color.FromRgb(80, 80, 80)
            : Color.FromRgb(200, 200, 200);

    public static Color HexToColor(string hexColor)
    {
        try
        {
            hexColor = hexColor.Replace("#", "");
            if (hexColor.Length == 8)
            {
                var a = byte.Parse(hexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                var r = byte.Parse(hexColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                var g = byte.Parse(hexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                var b = byte.Parse(hexColor.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
                return Color.FromArgb(a, r, g, b);
            }
            else if (hexColor.Length == 6)
            {
                var r = byte.Parse(hexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                var g = byte.Parse(hexColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                var b = byte.Parse(hexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                return Color.FromRgb(r, g, b);
            }
        }
        catch { }
        return Colors.Black;
    }

    public static string ColorToHex(Color color)
    {
        return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
    }
}
