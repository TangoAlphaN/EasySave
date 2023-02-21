using CommunityToolkit.Mvvm.ComponentModel;
using EasyClient.Enums;
using EasyClient.Properties;
using EasyClient.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
        /// Gets or sets the save list.
        /// </summary>
        [ObservableProperty]
        private Dictionary<string, SaveInfo> _saves;

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

        //-----------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the IP address.
        /// </summary>
        public static string Ip { get; set; }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        public static int Port { get; set; }

        /// <summary>
        /// Gets path.
        /// </summary>
        private static readonly string Path = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\ProSoft\EasyClient";

        public ViewModel(INavigationService navigationService)
        {
            _saves = new Dictionary<string, SaveInfo>();
            if (!_isInitialized)
                InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            if (!Directory.Exists(@$"{Path}"))
                Directory.CreateDirectory(@$"{Path}");
            if (!File.Exists(@$"{Path}\config.json"))
                File.Create(@$"{Path}\config.json");
            try
            {
                JObject data = JObject.Parse(File.ReadAllText(@$"{Path}\config.json"));
                if (data["ip"] != null)
                    Ip = data["ip"].ToString();
                else
                    Ip = "127.0.0.1";
                if (data["port"] != null)
                    Port = int.Parse(data["port"].ToString());
                else
                    Port = 6732;
            }
            catch (JsonReaderException)
            {
                SaveSettings("127.0.0.1", "6732");
            }


            ApplicationTitle = "EasyClient - EasySave";

            AppVersion = "Version 1.0.0";

            NavigationItems = new ObservableCollection<INavigationControl>
            {
                new NavigationItem()
                {
                    Content = Resource.Home,
                    PageTag = "dashboard",
                    Icon = SymbolRegular.Home24,
                    PageType = typeof(HomePage)
                }
            };

            NavigationFooter = new ObservableCollection<INavigationControl>
            {
                new NavigationItem()
                {
                    Content = Resource.Settings,
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

        public void SaveSettings(string ip, string port)
        {
            Ip = ip;
            Port = int.Parse(port);
            JObject data = new JObject(
                new JProperty("ip", Ip),
                new JProperty("port", Port)
            );
            string json = JsonConvert.SerializeObject(data);
            File.WriteAllText($@"{Path}\config.json", json);
        }

        public static bool Connect()
        {
            if (SocketUtils.Connect(Ip, Port))
            {
                return true;
            }
            else
            {
                System.Windows.MessageBox.Show($"{Resource.ErrorConnect}");
                return false;
            }
        }

        public static void PlaySave(string uuid)
        {
            SocketUtils.SendRequest(SocketRequest.Play, uuid);
        }

        public static void PauseSave(string uuid)
        {
            SocketUtils.SendRequest(SocketRequest.Pause, uuid);
        }

        public static void StopSave(string uuid)
        {
            SocketUtils.SendRequest(SocketRequest.Stop, uuid);
        }

        public void UpdateSaves()
        {
            Saves = (Dictionary<string, SaveInfo>)SocketUtils.SendRequest(SocketRequest.GetData);
        }

        public static void Disconnect()
        {
            SocketUtils.Disconnect();
        }

        public string GetSaveByUuid(string save)
        {
            return Saves.Single(k => k.Value.SaveName == save).Value.Status;
        }
    }
}
