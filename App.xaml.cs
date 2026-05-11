using System.Windows;
using Microsoft.Win32;
using WordClockTaskbar.Helpers;
using WordClockTaskbar.ViewModels;
using Application = System.Windows.Application;
using StartupEventArgs = System.Windows.StartupEventArgs;
using ExitEventArgs = System.Windows.ExitEventArgs;
using Color = System.Drawing.Color;
using SolidBrush = System.Drawing.SolidBrush;
using Bitmap = System.Drawing.Bitmap;
using Graphics = System.Drawing.Graphics;
using Icon = System.Drawing.Icon;
using Pen = System.Drawing.Pen;

namespace WordClockTaskbar;

public partial class App : Application
{
    private System.Windows.Forms.NotifyIcon? _notifyIcon;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        CreateTrayIcon();
    }

    private void CreateTrayIcon()
    {
        _notifyIcon = new System.Windows.Forms.NotifyIcon
        {
            Icon = CreateClockIcon(),
            Text = "World Clock - EU | UK | IN",
            Visible = true
        };

        var menu = new System.Windows.Forms.ContextMenuStrip();

        var posMenu = new System.Windows.Forms.ToolStripMenuItem("Position");
        posMenu.DropDownItems.Add("Left", null, (_, _) =>
        {
            if (MainWindow is MainWindow mw)
                TaskbarHelper.PositionOnTaskbar(mw, TaskbarHelper.Position.Left);
        });
        posMenu.DropDownItems.Add("Center", null, (_, _) =>
        {
            if (MainWindow is MainWindow mw)
                TaskbarHelper.PositionOnTaskbar(mw, TaskbarHelper.Position.Center);
        });
        posMenu.DropDownItems.Add("Right", null, (_, _) =>
        {
            if (MainWindow is MainWindow mw)
                TaskbarHelper.PositionOnTaskbar(mw, TaskbarHelper.Position.Right);
        });
        menu.Items.Add(posMenu);

        menu.Items.Add(new System.Windows.Forms.ToolStripSeparator());

        menu.Items.Add("Show/Hide Window", null, (_, _) =>
        {
            if (MainWindow is MainWindow mw)
            {
                if (mw.Visibility == System.Windows.Visibility.Visible)
                    mw.Hide();
                else
                {
                    mw.Show();
                    mw.Activate();
                }
            }
        });

        menu.Items.Add(new System.Windows.Forms.ToolStripSeparator());

        var autoStartItem = new System.Windows.Forms.ToolStripMenuItem("Start with Windows");
        autoStartItem.Checked = IsAutoStartEnabled();
        autoStartItem.Click += (_, _) =>
        {
            ToggleAutoStart();
            autoStartItem.Checked = IsAutoStartEnabled();
        };
        menu.Items.Add(autoStartItem);

        menu.Items.Add(new System.Windows.Forms.ToolStripSeparator());

        var alwaysOnTopItem = new System.Windows.Forms.ToolStripMenuItem("Always on Top");
        var config = WordClockTaskbar.Models.TimezoneConfig.Load();
        alwaysOnTopItem.Checked = config.IsAlwaysOnTop;
        alwaysOnTopItem.Click += (_, _) =>
        {
            config.IsAlwaysOnTop = !config.IsAlwaysOnTop;
            config.Save();
            alwaysOnTopItem.Checked = config.IsAlwaysOnTop;
            if (MainWindow is MainWindow mw)
                mw.Topmost = config.IsAlwaysOnTop;
        };
        menu.Items.Add(alwaysOnTopItem);

        menu.Items.Add(new System.Windows.Forms.ToolStripSeparator());

        menu.Items.Add("Settings", null, (_, _) =>
        {
            var settingsWindow = new SettingsWindow();
            if (settingsWindow.ShowDialog() == true)
            {
                if (MainWindow is MainWindow mw && mw.DataContext is ViewModels.ClockViewModel vm)
                {
                    vm.ReloadConfig();
                }
            }
        });

        menu.Items.Add(new System.Windows.Forms.ToolStripSeparator());

        menu.Items.Add("Exit", null, (_, _) =>
        {
            _notifyIcon?.Dispose();
            Shutdown();
        });

        _notifyIcon.ContextMenuStrip = menu;

        _notifyIcon.MouseClick += (_, args) =>
        {
            if (args.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (MainWindow is MainWindow mw)
                {
                    mw.Show();
                    mw.Activate();
                }
            }
        };
    }

    private static Icon CreateClockIcon()
    {
        var bmp = new Bitmap(32, 32);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        using var bgBrush = new SolidBrush(Color.FromArgb(0, 120, 212));
        g.FillEllipse(bgBrush, 2, 2, 28, 28);

        using var pen = new Pen(Color.White, 2f);
        g.DrawLine(pen, 16, 16, 16, 7);
        g.DrawLine(pen, 16, 16, 22, 16);

        using var dotBrush = new SolidBrush(Color.White);
        g.FillEllipse(dotBrush, 14, 14, 4, 4);

        var handle = bmp.GetHicon();
        return Icon.FromHandle(handle);
    }

    private const string AutoStartKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
    private const string AppName = "WordClockTaskbar";

    private static bool IsAutoStartEnabled()
    {
        using var key = Registry.CurrentUser.OpenSubKey(AutoStartKey);
        return key?.GetValue(AppName) != null;
    }

    private static void ToggleAutoStart()
    {
        using var key = Registry.CurrentUser.OpenSubKey(AutoStartKey, writable: true);
        if (key == null) return;

        if (key.GetValue(AppName) != null)
        {
            key.DeleteValue(AppName);
        }
        else
        {
            var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName;
            if (exePath != null)
                key.SetValue(AppName, $"\"{exePath}\"");
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _notifyIcon?.Dispose();
        base.OnExit(e);
    }
}
