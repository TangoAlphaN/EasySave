using CommunityToolkit.Mvvm.ComponentModel;
using EasyClient.Views;
using System.Collections.ObjectModel;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace EasyClient
{
    /// <summary>
    /// View model for the main window.
    /// </summary>
    public partial class ViewModel : ObservableObject
    {

        /// <summary>
        /// Indicates if the view model is initialized.
        /// </summary>
        private bool _isInitialized = false;

        /// <summary>
        /// Gets or sets the application title.
        /// </summary>
        [ObservableProperty]
        private string _applicationTitle = string.Empty;

        /// <summary>
        /// Gets or sets the application version.
        /// </summary>
        [ObservableProperty]
        private string _appVersion = string.Empty;

        /// <summary>
        /// Gets or sets the navigation items.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<INavigationControl> _navigationItems = new ObservableCollection<INavigationControl>();

        /// <summary>
        /// Gets or sets the navigation footer.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<INavigationControl> _navigationFooter = new ObservableCollection<INavigationControl>();

        /// <summary>
        /// Gets or sets the tray menu items.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems = new ObservableCollection<MenuItem>();

        public ViewModel(INavigationService navigationService)
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            ApplicationTitle = "EasyClient - EasySave";

            AppVersion = "1.0.0";

            NavigationItems = new ObservableCollection<INavigationControl>
            {
                new NavigationItem()
                {
                    Content = "Home",
                    PageTag = "dashboard",
                    Icon = SymbolRegular.Home24,
                    PageType = typeof(HomePage)
                }
            };

            NavigationFooter = new ObservableCollection<INavigationControl>
            {
                new NavigationItem()
                {
                    Content = "Settings",
                    PageTag = "settings",
                    Icon = SymbolRegular.Settings24,
                    PageType = typeof(SettingsPage)
                }
            };

            TrayMenuItems = new ObservableCollection<MenuItem>
            {
                new MenuItem
                {
                    Header = "Home",
                    Tag = "tray_home"
                }
            };

            _isInitialized = true;
        }
    }
}
