using EasySave.Properties;
using EasySave.src.Utils;
using EasySave.src.ViewModels;
using Notification.Wpf;
using System.Windows;
using System.Windows.Input;

namespace EasySave.src.Render.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        /// <summary>
        /// Default constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            CheckUpdate();
        }

        /// <summary>
        /// Event on mouse down
        /// Moves the window
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">args</param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        /// <summary>
        /// Check updates at startup
        /// Display a notification
        /// </summary>
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
