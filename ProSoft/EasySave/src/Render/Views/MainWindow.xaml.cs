using EasySave.Properties;
using EasySave.src.Utils;
using EasySave.src.ViewModels;
using Notifications.Wpf;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System;
using System.Windows;
using System.Windows.Input;

namespace EasySave.src.Render.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CheckUpdate();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void CheckUpdate()
        {
            bool upToDate = !HomeViewModel.IsUpToDate();
            new NotificationManager().Show(new NotificationContent
            {
                Title = $"EasySave {VersionUtils.GetVersionFromGit()}",
                Message = upToDate ? Resource.Header_UpToDate : Resource.Header_NoUpToDate + "\n" + "https://github.com/arnoux23u-CESI/EasySave/releases/latest",
                Type = upToDate ? NotificationType.Success : NotificationType.Warning,
            },
            onClick: () => OpenUrl("https://github.com/arnoux23u-CESI/EasySave/releases/latest", !upToDate),
            expirationTime: TimeSpan.FromSeconds(upToDate ? 5 : 30)
            );
        }

        private void OpenUrl(string url, bool open = false)
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
