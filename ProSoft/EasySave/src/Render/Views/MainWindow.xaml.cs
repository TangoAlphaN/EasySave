using EasySave.Properties;
using EasySave.src.Utils;
using EasySave.src.ViewModels;
using Notification.Wpf;
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
            bool upToDate = HomeViewModel.IsUpToDate();
            NotificationUtils.SendNotification(
                title: $"EasySave {VersionUtils.GetVersionFromLocal().ToString()[0..5]}",
                message: upToDate ? Resource.Header_UpToDate : Resource.Header_NoUpToDate + "\n" + "https://github.com/arnoux23u-CESI/EasySave/releases/latest",
                type: upToDate ? NotificationType.Success : NotificationType.Warning,
                url: upToDate ? "" : "https://github.com/arnoux23u-CESI/EasySave/releases/latest",
                time: upToDate ? 5 : 15
            );
        }
    }
}
