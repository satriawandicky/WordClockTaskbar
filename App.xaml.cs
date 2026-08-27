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
    private System.Windows.Forms.ToolStripMenuItem? _updateItem;
    private UpdateInfo? _pendingUpdate;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        AppDomain.CurrentDomain.UnhandledException += (s, args) =>
        {
            LogException(args.ExceptionObject as Exception);
        };

        DispatcherUnhandledException += (s, args) =>
        {
            LogException(args.Exception);
            args.Handled = true;
        };

        try
        {
            CreateTrayIcon();
            _ = CheckForUpdatesAsync(silent: true);
        }
        catch (Exception ex)
        {
            LogException(ex);
        }
    }

    private static void LogException(Exception? ex)
    {
        if (ex is null) return;
        try
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var dir = System.IO.Path.Combine(appData, "WordClockTaskbar");
            System.IO.Directory.CreateDirectory(dir);
            var file = System.IO.Path.Combine(dir, "error.log");
            System.IO.File.AppendAllText(file, $"[{DateTime.Now}] {ex}\n\n");
        }
        catch { }
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
            {
                TaskbarHelper.PositionOnTaskbar(mw, TaskbarHelper.Position.Left);
                mw.SavePosition();
            }
        });
        posMenu.DropDownItems.Add("Center", null, (_, _) =>
        {
            if (MainWindow is MainWindow mw)
            {
                TaskbarHelper.PositionOnTaskbar(mw, TaskbarHelper.Position.Center);
                mw.SavePosition();
            }
        });
        posMenu.DropDownItems.Add("Right (Near Clock)", null, (_, _) =>
        {
            if (MainWindow is MainWindow mw)
            {
                TaskbarHelper.PositionOnTaskbar(mw, TaskbarHelper.Position.Right);
                mw.SavePosition();
            }
        });
        posMenu.DropDownItems.Add("Reset Position", null, (_, _) =>
        {
            if (MainWindow is MainWindow mw)
            {
                mw.EnsureVisible(resetPosition: true);
                mw.SavePosition();
            }
        });
        menu.Items.Add(posMenu);

        menu.Items.Add(new System.Windows.Forms.ToolStripSeparator());

        menu.Items.Add("Show/Hide Window", null, (_, _) =>
        {
            if (MainWindow is MainWindow mw)
            {
                if (mw.IsVisible)
                    mw.Hide();
                else
                    mw.EnsureVisible();
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
        alwaysOnTopItem.Checked = WordClockTaskbar.Models.TimezoneConfig.Load().IsAlwaysOnTop;
        alwaysOnTopItem.Click += (_, _) =>
        {
            var config = WordClockTaskbar.Models.TimezoneConfig.Load();
            config.IsAlwaysOnTop = !config.IsAlwaysOnTop;
            config.Save();

            alwaysOnTopItem.Checked = config.IsAlwaysOnTop;
            if (MainWindow is MainWindow mw)
                mw.SetAlwaysOnTop(config.IsAlwaysOnTop);
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
                    var config = WordClockTaskbar.Models.TimezoneConfig.Load();
                    alwaysOnTopItem.Checked = config.IsAlwaysOnTop;

                    vm.ReloadConfig();
                    mw.SetAlwaysOnTop(config.IsAlwaysOnTop);
                    mw.RefreshLayoutAndPosition();
                }
            }
        });

        menu.Items.Add(new System.Windows.Forms.ToolStripSeparator());

        _updateItem = new System.Windows.Forms.ToolStripMenuItem("Check for Updates");
        _updateItem.Click += async (_, _) =>
        {
            if (_pendingUpdate is not null)
                await PromptAndApplyAsync(_pendingUpdate);
            else
                await CheckForUpdatesAsync(silent: false);
        };
        menu.Items.Add(_updateItem);

        menu.Items.Add(new System.Windows.Forms.ToolStripSeparator());

        menu.Items.Add("Exit", null, (_, _) =>
        {
            _notifyIcon?.Dispose();
            Shutdown();
        });

        _notifyIcon.ContextMenuStrip = menu;

        _notifyIcon.BalloonTipClicked += async (_, _) =>
        {
            if (_pendingUpdate is not null)
                await PromptAndApplyAsync(_pendingUpdate);
        };

        _notifyIcon.MouseClick += (_, args) =>
        {
            if (args.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (MainWindow is MainWindow mw)
                {
                    mw.EnsureVisible();
                }
            }
        };
    }

    private async Task CheckForUpdatesAsync(bool silent)
    {
        if (_updateItem is not null && !silent)
            _updateItem.Text = "Checking for updates…";

        var info = await UpdateChecker.CheckAsync();

        if (info is null)
        {
            _pendingUpdate = null;
            if (_updateItem is not null)
                _updateItem.Text = "Check for Updates";

            if (!silent)
                System.Windows.MessageBox.Show(
                    $"You're on the latest version (v{UpdateChecker.CurrentVersion.ToString(3)}).",
                    "WordClock Taskbar", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        _pendingUpdate = info;
        if (_updateItem is not null)
            _updateItem.Text = $"Update to {info.Tag}";

        if (silent)
        {
            if (_notifyIcon is not null)
            {
                _notifyIcon.BalloonTipTitle = "Update available";
                _notifyIcon.BalloonTipText = $"WordClock Taskbar {info.Tag} is available. Click to update.";
                _notifyIcon.ShowBalloonTip(8000);
            }
        }
        else
        {
            await PromptAndApplyAsync(info);
        }
    }

    private async Task PromptAndApplyAsync(UpdateInfo info)
    {
        var result = System.Windows.MessageBox.Show(
            $"WordClock Taskbar {info.Tag} is available (you have v{UpdateChecker.CurrentVersion.ToString(3)}).\n\n" +
            "Download and update now? The app will close and reopen automatically.",
            "Update available", MessageBoxButton.OKCancel, MessageBoxImage.Information);

        if (result == MessageBoxResult.OK)
            await ApplyUpdateAsync(info);
    }

    private async Task ApplyUpdateAsync(UpdateInfo info)
    {
        if (_updateItem is not null)
        {
            _updateItem.Text = "Downloading update…";
            _updateItem.Enabled = false;
        }

        var ok = await UpdateChecker.DownloadAndApplyAsync(info);

        if (ok)
        {
            // Swap script is waiting for this process to exit before replacing the exe.
            _notifyIcon?.Dispose();
            Shutdown();
        }
        else
        {
            if (_updateItem is not null)
            {
                _updateItem.Text = $"Update to {info.Tag}";
                _updateItem.Enabled = true;
            }

            System.Windows.MessageBox.Show(
                "Update failed. Check your connection or download the latest release manually from GitHub.",
                "Update", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
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
