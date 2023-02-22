using Notification.Wpf;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace EasySave.src.Utils
{
    public static class NotificationUtils
    {

        /// <summary>
        /// Send a notification with multiple parameters
        /// </summary>
        public static void SendNotification(string title, string message, NotificationType type = NotificationType.Error, string url = "", int time = 5)
        {
            new NotificationManager().Show(new NotificationContent
            {
                Title = title,
                Message = message,
                Type = type,
            },
            onClick: () => OpenUrl(url, url != null && url.Length > 0),
            expirationTime: TimeSpan.FromSeconds(time)
            );
        }

        private static void OpenUrl(string url, bool open = false)
        {
            if (!open) return;
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
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
                    throw;
                }
            }
        }
    }
}