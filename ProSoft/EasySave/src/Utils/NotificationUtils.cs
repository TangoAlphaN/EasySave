using Notification.Wpf;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace EasySave.src.Utils
{
    /// <summary>
    /// Static notification utils class
    /// </summary>
    public static class NotificationUtils
    {

        /// <summary>
        /// Display a notification
        /// </summary>
        /// <param name="title">title</param>
        /// <param name="message">message</param>
        /// <param name="type">type of notification</param>
        /// <param name="url">url to open</param>
        /// <param name="time">time to show popup</param>
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

        /// <summary>
        /// Open an url
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="open">need to open</param>
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