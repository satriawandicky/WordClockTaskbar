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

    public static void PositionOnTaskbar(Window window, Position position)
    {
        double dpiScale = GetDpiForSystem() / 96.0;
        var workArea = SystemParameters.WorkArea;
        var primaryWidth = SystemParameters.PrimaryScreenWidth;
        var primaryHeight = SystemParameters.PrimaryScreenHeight;

        var taskbarHandle = FindWindow("Shell_TrayWnd", null);

        double topInDip;
        double taskbarLeftDip = 0;
        double taskbarRightDip = primaryWidth;
        double trayLeftDip = primaryWidth - 200;

        if (taskbarHandle != IntPtr.Zero && GetWindowRect(taskbarHandle, out var taskbarRect))
        {
            double taskbarTopDip = taskbarRect.Top / dpiScale;
            double taskbarHeightDip = (taskbarRect.Bottom - taskbarRect.Top) / dpiScale;
            taskbarLeftDip = taskbarRect.Left / dpiScale;
            taskbarRightDip = taskbarRect.Right / dpiScale;

            topInDip = taskbarTopDip + (taskbarHeightDip - window.Height) / 2.0;

            var trayHandle = FindWindowEx(taskbarHandle, IntPtr.Zero, "TrayNotifyWnd", null);
            if (trayHandle != IntPtr.Zero && GetWindowRect(trayHandle, out var trayRect))
            {
                trayLeftDip = trayRect.Left / dpiScale;
            }
            else
            {
                trayLeftDip = taskbarRightDip - 200;
            }
        }
        else
        {
            topInDip = workArea.Bottom - window.Height - 4;
            trayLeftDip = workArea.Right - 200;
            taskbarLeftDip = workArea.Left;
            taskbarRightDip = workArea.Right;
        }

        double leftInDip = position switch
        {
            Position.Left => taskbarLeftDip + 60,
            Position.Center => taskbarLeftDip + ((taskbarRightDip - taskbarLeftDip) - window.Width) / 2.0,
            Position.Right => trayLeftDip - window.Width - 8,
            _ => trayLeftDip - window.Width - 8
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
        const int GWL_EXSTYLE = -20;
        const int WS_EX_TOOLWINDOW = 0x00000080;

        var hwnd = new WindowInteropHelper(window).Handle;
        if (hwnd == IntPtr.Zero)
            return;

        var exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
        SetWindowLong(hwnd, GWL_EXSTYLE, exStyle | WS_EX_TOOLWINDOW);
    }

    public static void SetTopmost(Window window, bool enabled)
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
}
