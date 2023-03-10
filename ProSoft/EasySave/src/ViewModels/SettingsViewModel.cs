using EasySave.Properties;
using EasySave.src.Render;
using EasySave.src.Render.Views;
using EasySave.src.Utils;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Data;

namespace EasySave.src.ViewModels
{
    /// <summary>
    /// Settings view model
    /// </summary>
    public class SettingsViewModel
    {
        //Collections of items
        private readonly CollectionViewSource _cryptoSoftSettingsItemsCollection, _langItemsCollection, _logItemsCollection, _filesItemsCollection, _sizeLimitItemsCollection;
        public ICollectionView CryptoSoftSourceCollection => _cryptoSoftSettingsItemsCollection.View;
        public ICollectionView LangSourceCollection => _langItemsCollection.View;
        public ICollectionView LogsSourceCollection => _logItemsCollection.View;
        public ICollectionView FilesSourceCollection => _filesItemsCollection.View;
        public ICollectionView SizeLimitSourceCollection => _sizeLimitItemsCollection.View;

        /// <summary>
        /// Default constructor
        /// </summary>
        public SettingsViewModel()
        {
            ObservableCollection<SettingsItem> cryptoSoftItems = new ObservableCollection<SettingsItem>
            {
                new SettingsItem { SettingsName = $"{Resource.Settings_Secret}", SettingsValue = DirectoryUtils.GetSecret(), SettingsCommand = new RelayCommand(ChangeKey) },
                new SettingsItem { SettingsName = $"{Resource.Settings_Extensions}", SettingsValue = DirectoryUtils.GetExtensionsToEncrypt(), SettingsCommand = new RelayCommand(ChangeExtensionsToEncrypt) },
            };
            ObservableCollection<SettingsItem> langItems = new ObservableCollection<SettingsItem>
            {
                new SettingsItem { SettingsName = $"{Resource.Lang_fr_FR}", SettingsImage = @"Assets/fr_FR.png", SettingsCommand = new RelayCommand(ChangeLanguage) },
                new SettingsItem { SettingsName = $"{Resource.Lang_en_US}", SettingsImage = @"Assets/en_US.png", SettingsCommand = new RelayCommand(ChangeLanguage) },
                new SettingsItem { SettingsName = $"{Resource.Lang_it_IT}", SettingsImage = @"Assets/it_IT.png", SettingsCommand = new RelayCommand(ChangeLanguage) },
                new SettingsItem { SettingsName = $"{Resource.Lang_ru_RU}", SettingsImage = @"Assets/ru_RU.png", SettingsCommand = new RelayCommand(ChangeLanguage) },
            };
            ObservableCollection<SettingsItem> logItems = new ObservableCollection<SettingsItem>
            {
                new SettingsItem { SettingsName = "JSON", SettingsImage = @"Assets/json.png", SettingsCommand = new RelayCommand(ChangeLogsFormat) },
                new SettingsItem { SettingsName = "XML", SettingsImage = @"Assets/xml.png", SettingsCommand = new RelayCommand(ChangeLogsFormat) },
            };
            ObservableCollection<SettingsItem> fileSettings = new ObservableCollection<SettingsItem>
            {
                new SettingsItem { SettingsName = $"{Resource.Settings_Software_Package}", SettingsValue = DirectoryUtils.GetProcess(), SettingsCommand = new RelayCommand(ChangeProcess) },
                new SettingsItem { SettingsName = $"{Resource.Settings_Priority_Files}", SettingsValue = DirectoryUtils.GetPriorityExtensions(), SettingsCommand = new RelayCommand(ChangePriorityExtensions) },
            };
            ObservableCollection<SettingsItem> limitSize = new ObservableCollection<SettingsItem>
            {
                new SettingsItem { SettingsName = $"{Resource.Settings_LimitSize}", SettingsValue = DirectoryUtils.GetLimitSize().ToString(), SettingsCommand = new RelayCommand(ChangeLimitSize) },
            };

            _cryptoSoftSettingsItemsCollection = new CollectionViewSource { Source = cryptoSoftItems };
            _langItemsCollection = new CollectionViewSource { Source = langItems };
            _logItemsCollection = new CollectionViewSource { Source = logItems };
            _filesItemsCollection = new CollectionViewSource { Source = fileSettings };
            _sizeLimitItemsCollection = new CollectionViewSource { Source = limitSize };
        }

        /// <summary>
        /// Change language method
        /// </summary>
        /// <param name="culture">new culture</param>
        private static void ChangeLanguage(object culture)
        {
            var cultureData = "";
            switch ((string)culture)
            {
                case var _ when (string)culture == Resource.Lang_fr_FR:
                    cultureData = "fr-FR";
                    break;
                case var _ when (string)culture == Resource.Lang_en_US:
                    cultureData = "en-US";
                    break;
                case var _ when (string)culture == Resource.Lang_it_IT:
                    cultureData = "it-IT";
                    break;
                case var _ when (string)culture == Resource.Lang_ru_RU:
                    cultureData = "ru-RU";
                    break;

            }
            CultureInfo cultureInfo = new CultureInfo(cultureData.ToString());
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            var windows = Application.Current.MainWindow;
            Application.Current.MainWindow = new MainWindow();
            windows.Close();
            Application.Current.MainWindow.Show();
        }

        /// <summary>
        /// Change logs format method
        /// </summary>
        /// <param name="format">new logs format</param>
        private static void ChangeLogsFormat(object format)
        {
            switch ((string)format)
            {
                case "JSON":
                    LogUtils.ChangeFormat(LogsFormat.JSON);
                    break;
                case "XML":
                    LogUtils.ChangeFormat(LogsFormat.XML);
                    break;
            }
        }

        /// <summary>
        /// Change secret key method
        /// </summary>
        /// <param name="obj">new key</param>
        public static void ChangeKey(object obj)
        {
            DirectoryUtils.ChangeKey((string)obj);
        }

        /// <summary>
        /// Change extensions to encrypt method
        /// </summary>
        /// <param name="obj">new list</param>
        public static void ChangeExtensionsToEncrypt(object obj)
        {
            DirectoryUtils.ChangeExtensionsToEncrypt(((string)obj).Split("\r\n").ToHashSet());
        }

        /// <summary>
        /// Change processes to detect method
        /// </summary>
        /// <param name="obj">new list</param>
        public static void ChangeProcess(object obj)
        {
            DirectoryUtils.ChangeProcess(((string)obj).Split("\r\n").ToHashSet());
        }

        /// <summary>
        /// Change priority extensions method
        /// </summary>
        /// <param name="obj">new list</param>
        public static void ChangePriorityExtensions(object obj)
        {
            DirectoryUtils.ChangePriorityExtensions(((string)obj).Split("\r\n").ToHashSet());
        }

        /// <summary>
        /// Change limit size method
        /// </summary>
        /// <param name="obj">size limit</param>
        public static void ChangeLimitSize(object obj)
        {
            try
            {
                DirectoryUtils.ChangeLimitSize(int.Parse((string)obj));
            }
            catch
            {
                DirectoryUtils.ChangeLimitSize(0);
            }
        }

    }

}