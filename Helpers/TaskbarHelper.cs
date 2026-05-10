using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace WordClockTaskbar.Helpers;

public static class TaskbarHelper
{
    [DllImport("user32.dll")]
    private static extern IntPtr FindWindow(string className, string? windowName);

    [DllImport("user32.dll")]
    private static extern IntPtr FindWindowEx(IntPtr parent, IntPtr childAfter, string? className, string? windowName);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    private static extern int GetDpiForSystem();

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left, Top, Right, Bottom;
    }

    public enum Position { Left, Center, Right }

    public static void PositionOnTaskbar(Window window, Position position)
    {
        var taskbarHandle = FindWindow("Shell_TrayWnd", null);
        if (taskbarHandle == IntPtr.Zero) return;

        GetWindowRect(taskbarHandle, out var taskbarRect);

        int taskbarTop = taskbarRect.Top;
        int taskbarHeight = taskbarRect.Bottom - taskbarRect.Top;
        int taskbarWidth = taskbarRect.Right - taskbarRect.Left;

        double dpiScale = GetDpiForSystem() / 96.0;
        double windowWidth = window.Width * dpiScale;
        double windowHeight = window.Height * dpiScale;

        double left = position switch
        {
            Position.Left => taskbarRect.Left + 60,
            Position.Center => taskbarRect.Left + (taskbarWidth - windowWidth) / 2.0,
            Position.Right => GetTrayLeft(taskbarHandle, taskbarRect) - windowWidth - 4,
            _ => taskbarRect.Left + (taskbarWidth - windowWidth) / 2.0
        };

        window.Left = left / dpiScale;
        window.Top = (taskbarTop + (taskbarHeight - windowHeight) / 2.0) / dpiScale;
    }

    private static int GetTrayLeft(IntPtr taskbarHandle, RECT taskbarRect)
    {
        var trayHandle = FindWindowEx(taskbarHandle, IntPtr.Zero, "TrayNotifyWnd", null);
        if (trayHandle != IntPtr.Zero)
        {
            GetWindowRect(trayHandle, out var trayRect);
            return trayRect.Left;
        }
        return taskbarRect.Right - 300;
    }

    public static void PositionNearClock(Window window) => PositionOnTaskbar(window, Position.Right);

    public static void SetToolWindowStyle(Window window)
    {
        const int GWL_EXSTYLE = -20;
        const int WS_EX_TOOLWINDOW = 0x00000080;
        const int WS_EX_NOACTIVATE = 0x08000000;

        var hwnd = new WindowInteropHelper(window).Handle;
        var exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
        SetWindowLong(hwnd, GWL_EXSTYLE, exStyle | WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE);
    }
}
