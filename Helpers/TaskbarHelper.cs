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

    [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
    private static extern IntPtr GetWindowLong32(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
    private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

    private static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
    {
        return IntPtr.Size == 8 ? GetWindowLongPtr64(hWnd, nIndex) : GetWindowLong32(hWnd, nIndex);
    }

    [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
    private static extern IntPtr SetWindowLong32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
    private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    private static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
    {
        return IntPtr.Size == 8 ? SetWindowLongPtr64(hWnd, nIndex, dwNewLong) : SetWindowLong32(hWnd, nIndex, dwNewLong);
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint flags);

    private static readonly IntPtr HWND_TOPMOST = new(-1);
    private static readonly IntPtr HWND_NOTOPMOST = new(-2);

    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOACTIVATE = 0x0010;
    private const uint SWP_NOOWNERZORDER = 0x0200;

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left, Top, Right, Bottom;
    }

    public enum Position { Left, Center, Right }

    public static (double Left, double Top, double Width, double Height) GetTaskbarRectDip()
    {
        double dpiScale = GetDpiForSystem() / 96.0;
        if (dpiScale <= 0) dpiScale = 1.0;

        var taskbarHandle = FindWindow("Shell_TrayWnd", null);
        if (taskbarHandle != IntPtr.Zero && GetWindowRect(taskbarHandle, out var taskbarRect))
        {
            double l = taskbarRect.Left / dpiScale;
            double t = taskbarRect.Top / dpiScale;
            double w = (taskbarRect.Right - taskbarRect.Left) / dpiScale;
            double h = (taskbarRect.Bottom - taskbarRect.Top) / dpiScale;
            if (w > 100 && h > 20)
                return (l, t, w, h);
        }

        var workArea = SystemParameters.WorkArea;
        var primaryWidth = SystemParameters.PrimaryScreenWidth;
        var primaryHeight = SystemParameters.PrimaryScreenHeight;

        if (workArea.Bottom < primaryHeight)
        {
            // Standard bottom taskbar
            return (0, workArea.Bottom, primaryWidth, primaryHeight - workArea.Bottom);
        }
        if (workArea.Top > 0)
        {
            // Top taskbar
            return (0, 0, primaryWidth, workArea.Top);
        }
        if (workArea.Left > 0)
        {
            // Left taskbar
            return (0, 0, workArea.Left, primaryHeight);
        }
        if (workArea.Right < primaryWidth)
        {
            // Right taskbar
            return (workArea.Right, 0, primaryWidth - workArea.Right, primaryHeight);
        }

        // Fallback: bottom 48 DIPs
        return (0, primaryHeight - 48, primaryWidth, 48);
    }

    public static void PositionOnTaskbar(Window window, Position position)
    {
        var tb = GetTaskbarRectDip();
        var primaryWidth = SystemParameters.PrimaryScreenWidth;
        var primaryHeight = SystemParameters.PrimaryScreenHeight;

        // Position vertically centered inside the taskbar
        double topInDip = tb.Top + Math.Max(2, (tb.Height - window.Height) / 2.0);

        // Position horizontally
        // In Windows 11, the tray icons & clock occupy ~200-240px from the right edge
        double leftInDip = position switch
        {
            Position.Left => tb.Left + 60,
            Position.Center => tb.Left + (tb.Width - window.Width) / 2.0,
            Position.Right => tb.Left + tb.Width - window.Width - 220,
            _ => tb.Left + tb.Width - window.Width - 220
        };

        if (leftInDip < 0) leftInDip = 8;
        if (leftInDip + window.Width > primaryWidth) leftInDip = primaryWidth - window.Width - 8;
        if (topInDip < 0) topInDip = 4;
        if (topInDip + window.Height > primaryHeight) topInDip = primaryHeight - window.Height - 4;

        window.Left = leftInDip;
        window.Top = topInDip;
    }

    public static void PositionNearClock(Window window) => PositionOnTaskbar(window, Position.Right);

    public static void SetToolWindowStyle(Window window)
    {
        try
        {
            const int GWL_EXSTYLE = -20;
            const long WS_EX_TOOLWINDOW = 0x00000080L;

            var hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero)
                return;

            var exStyle = GetWindowLongPtr(hwnd, GWL_EXSTYLE).ToInt64();
            SetWindowLongPtr(hwnd, GWL_EXSTYLE, new IntPtr(exStyle | WS_EX_TOOLWINDOW));
        }
        catch
        {
            // Ignore interop errors safely to prevent crashes
        }
    }

    public static void SetTopmost(Window window, bool enabled)
    {
        try
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero)
                return;

            var targetZOrder = enabled ? HWND_TOPMOST : HWND_NOTOPMOST;
            SetWindowPos(
                hwnd,
                targetZOrder,
                0,
                0,
                0,
                0,
                SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_NOOWNERZORDER);
        }
        catch
        {
            // Ignore interop errors safely
        }
    }
}
