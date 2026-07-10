using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;

namespace WordClockTaskbar.Helpers;

public record UpdateInfo(Version Version, string Tag, string AssetUrl, string AssetName);

// Checks GitHub Releases for a newer build and, on request, downloads the
// self-contained exe and swaps it in place via a small batch helper that waits
// for this process to exit, replaces the running file, and relaunches.
public static class UpdateChecker
{
    private const string Owner = "satriawandicky";
    private const string Repo = "WordClockTaskbar";
    private const string LatestApi = "https://api.github.com/repos/" + Owner + "/" + Repo + "/releases/latest";

    public static Version CurrentVersion =>
        Assembly.GetExecutingAssembly().GetName().Version ?? new Version(0, 0, 0, 0);

    // Returns update info only when the latest release is newer than the running
    // build; null when up to date or on any error (network, parse, offline).
    public static async Task<UpdateInfo?> CheckAsync(CancellationToken ct = default)
    {
        try
        {
            using var http = CreateClient();
            using var resp = await http.GetAsync(LatestApi, ct);
            resp.EnsureSuccessStatusCode();

            await using var stream = await resp.Content.ReadAsStreamAsync(ct);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
            var root = doc.RootElement;

            var tag = root.GetProperty("tag_name").GetString() ?? "";
            var latest = ParseVersion(tag);
            if (latest is null || latest <= CurrentVersion)
                return null;

            if (!root.TryGetProperty("assets", out var assets))
                return null;

            foreach (var asset in assets.EnumerateArray())
            {
                var name = asset.GetProperty("name").GetString() ?? "";
                if (name.EndsWith("win-x64.exe", StringComparison.OrdinalIgnoreCase))
                {
                    var url = asset.GetProperty("browser_download_url").GetString() ?? "";
                    if (url.Length > 0)
                        return new UpdateInfo(latest, tag, url, name);
                }
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    // Downloads the new exe to a temp file, then hands off to a batch script that
    // waits for this process to close, overwrites the current exe, and relaunches.
    // Caller should shut the app down immediately after this returns true.
    public static async Task<bool> DownloadAndApplyAsync(UpdateInfo info, IProgress<double>? progress = null, CancellationToken ct = default)
    {
        try
        {
            var currentExe = Process.GetCurrentProcess().MainModule?.FileName;
            if (string.IsNullOrEmpty(currentExe))
                return false;

            var tempExe = Path.Combine(Path.GetTempPath(), $"WordClockTaskbar-{info.Tag}.exe");

            using (var http = CreateClient())
            using (var resp = await http.GetAsync(info.AssetUrl, HttpCompletionOption.ResponseHeadersRead, ct))
            {
                resp.EnsureSuccessStatusCode();
                var total = resp.Content.Headers.ContentLength ?? -1L;

                await using var src = await resp.Content.ReadAsStreamAsync(ct);
                await using var dst = new FileStream(tempExe, FileMode.Create, FileAccess.Write, FileShare.None);

                var buffer = new byte[81920];
                long read = 0;
                int n;
                while ((n = await src.ReadAsync(buffer, ct)) > 0)
                {
                    await dst.WriteAsync(buffer.AsMemory(0, n), ct);
                    read += n;
                    if (total > 0)
                        progress?.Report((double)read / total);
                }
            }

            LaunchSwapScript(tempExe, currentExe, Process.GetCurrentProcess().Id);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static void LaunchSwapScript(string newExe, string currentExe, int pid)
    {
        var bat = Path.Combine(Path.GetTempPath(), "wct-update.bat");
        // Wait for our PID to exit, replace the exe, relaunch, then self-delete.
        var script =
            "@echo off\r\n" +
            "setlocal\r\n" +
            ":wait\r\n" +
            $"tasklist /fi \"PID eq {pid}\" 2>nul | find \"{pid}\" >nul\r\n" +
            "if not errorlevel 1 (\r\n" +
            "  timeout /t 1 /nobreak >nul\r\n" +
            "  goto wait\r\n" +
            ")\r\n" +
            $"move /y \"{newExe}\" \"{currentExe}\" >nul\r\n" +
            $"start \"\" \"{currentExe}\"\r\n" +
            "del \"%~f0\"\r\n";

        File.WriteAllText(bat, script);

        Process.Start(new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/c \"{bat}\"",
            CreateNoWindow = true,
            UseShellExecute = false,
            WindowStyle = ProcessWindowStyle.Hidden
        });
    }

    private static HttpClient CreateClient()
    {
        var http = new HttpClient { Timeout = TimeSpan.FromMinutes(5) };
        http.DefaultRequestHeaders.UserAgent.ParseAdd("WordClockTaskbar-Updater");
        http.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");
        return http;
    }

    // Parses "v1.2.3" / "1.2.3" into a 4-part Version for comparison.
    private static Version? ParseVersion(string tag)
    {
        var t = tag.TrimStart('v', 'V').Trim();
        return Version.TryParse(t, out var v) ? new Version(v.Major, v.Minor, v.Build < 0 ? 0 : v.Build, 0) : null;
    }
}
