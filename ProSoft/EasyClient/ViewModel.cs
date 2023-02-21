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
        private static volatile Dictionary<string, SaveInfo> _saves;

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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="navigationService">navigationservice</param>
        public ViewModel(INavigationService navigationService)
        {
            Saves = new Dictionary<string, SaveInfo>();
            if (!_isInitialized)
                InitializeViewModel();
        }

        /// <summary>
        /// Initializes the view model.
        /// </summary>
        private void InitializeViewModel()
        {
            //Check config file
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

        /// <summary>
        /// Save settings into JSON
        /// </summary>
        /// <param name="ip">ip</param>
        /// <param name="port">port</param>
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

        /// <summary>
        /// Connect to server
        /// </summary>
        /// <returns>true if connect ok</returns>
        public static bool Connect()
        {
            if (SocketUtils.Connect(Ip, Port))
                return true;
            else
            {
                System.Windows.MessageBox.Show($"{Resource.ErrorConnect}");
                return false;
            }
        }

        /// <summary>
        /// Play / resume a save
        /// </summary>
        /// <param name="uuid">save uuid</param>
        public static void PlaySave(string uuid)
        {
            SocketUtils.SendRequest(SocketRequest.Play, uuid);
        }

        /// <summary>
        /// Pause
        /// </summary>
        /// <param name="uuid">save uuid</param>
        public static void PauseSave(string uuid)
        {
            SocketUtils.SendRequest(SocketRequest.Pause, uuid);
        }

        /// <summary>
        /// StopSave
        /// </summary>
        /// <param name="uuid">save uuid</param>
        public static void StopSave(string uuid)
        {
            SocketUtils.SendRequest(SocketRequest.Stop, uuid);
        }

        /// <summary>
        /// CancelSave
        /// </summary>
        /// <param name="uuid">save uuid</param>
        public static void CancelSave(string uuid)
        {
            SocketUtils.SendRequest(SocketRequest.Cancel, uuid);
        }

        public void UpdateSaves()
        {
            Saves = (Dictionary<string, SaveInfo>)SocketUtils.SendRequest(SocketRequest.GetData);
        }

        /// <summary>
        /// Disconnect from server
        /// </summary>
        public static void Disconnect()
        {
            SocketUtils.Disconnect();
        }

        /// <summary>
        /// Get Save status by uuid
        /// </summary>
        /// <param name="save">save uuid</param>
        /// <returns>Save status</returns>
        public string GetStatusByUuid(string save)
        {
            return Saves.Single(k => k.Value.SaveName == save).Value.Status;
        }
    }
}
