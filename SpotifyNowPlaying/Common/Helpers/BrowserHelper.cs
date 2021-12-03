using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SpotifyNowPlaying.Common
{
    public static class BrowserHelper
    {

        public static void OpenBrowser(string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var processInfo = new ProcessStartInfo()
                {
                    FileName = url,
                    UseShellExecute = true
                };
                Process.Start(processInfo);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else
            {
                throw new ArgumentOutOfRangeException($"Unable to determine current {nameof(OSPlatform)}.");
            }
        }

    }
}
