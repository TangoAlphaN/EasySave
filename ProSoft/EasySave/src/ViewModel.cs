using EasySave.src.Models.Data;
using EasySave.src.Render.Views;
using EasySave.src.Utils;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace EasySave.src
{
    /// <summary>
    /// View model class
    /// </summary>
    public class ViewModel
    {

        /// <summary>
        /// private instance field
        /// </summary>
        private static ViewModel _instance;

        /// <summary>
        /// private constructor to prevent multiple instances
        /// </summary>
        private ViewModel()
        {
            
        }

        /// <summary>
        /// singleton getinstance method
        /// </summary>
        /// <returns>unique instance of viewmodel</returns>
        public static ViewModel GetInstance()
        {
            if (_instance == null)
                _instance = new ViewModel();
            return _instance;
        }

        /// <summary>
        /// Create save method
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="src">source path</param>
        /// <param name="dest">destination path</param>
        /// <param name="type">type of save</param>
        /// <returns>save object</returns>
        public Save CreateSave(string name, string src, string dest, SaveType type)
        {
            return Save.CreateSave(name, src, dest, type);
        }

        /// <summary>
        /// Edit save method
        /// </summary>
        /// <param name="s">save to edit</param>
        /// <param name="name">new name</param>
        public void EditSave(Save s, string name)
        {
            s.Rename(name);
        }

        /// <summary>
        /// Delete save method
        /// </summary>
        /// <param name="s">save to deletee</param>
        public void DeleteSave(Save s)
        {
            Save.Delete(s.uuid);
        }

        /// <summary>
        /// Run save method
        /// </summary>
        /// <param name="save">save to run</param>
        /// <returns>save job result</returns>

        public string RunSave(Save save)
        {
            return save.Run();
        }

        /// <summary>
        /// Stop all saves
        /// </summary>
        internal void StopAllSaves()
        {
            foreach (Save s in Save.GetSaves())
            {
                s.Stop();
            }
            LogUtils.LogSaves();
        }

        /// <summary>
        /// check for updates
        /// </summary>
        /// <returns>bool if up to date</returns>
        public static bool IsUpToDate()
        {
            return !VersionUtils.CompareVersions();
        }

        /// <summary>
        /// Get saves by uuids
        /// </summary>
        /// <param name="names">uuids list of saves</param>
        /// <returns>list of saves</returns>
        public HashSet<Save> GetSavesByUuid(HashSet<string> names)
        {
            return new HashSet<Save>(Save.GetSaves().Where(save => names.Contains(save.ToString())).ToList());
        }

        /// <summary>
        /// Get all saves names
        /// </summary>
        /// <returns>saves names</returns>
        public HashSet<string> GetSaves()
        {
            HashSet<string> data = new HashSet<string>();
            foreach (Save s in Save.GetSaves())
                data.Add(s.ToString());
            return data;
        }

        public static void ChangeLanguage(object culture)
        {
            CultureInfo cultureInfo = new CultureInfo(culture.ToString());
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            var windows = Application.Current.MainWindow;
            Application.Current.MainWindow = new MainWindow();
            windows.Close();
            Application.Current.MainWindow.Show();
        }

        public static void ChangeSettings(object culture)
        {
            CultureInfo cultureInfo = new CultureInfo(culture.ToString());
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            var windows = Application.Current.MainWindow;
            Application.Current.MainWindow = new MainWindow();
            windows.Close();
            Application.Current.MainWindow.Show();
        }

        public static void ChangeLogsFormat(object format)
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
            var windows = Application.Current.MainWindow;
            Application.Current.MainWindow = new MainWindow();
            windows.Close();
            Application.Current.MainWindow.Show();
        }

        internal static void ChangeKey(object obj)
        {
            DirectoryUtils.ChangeKey((string)obj);
        }
    }

    public class MenuItems
    {
        public string MenuName { get; set; }
        public string MenuImage { get; set; }
    }

    // Home Page
    public class HomeItems
    {
        public string HomeName { get; set; }
        public string HomeImage { get; set; }
    }

    public class LangItems
    {
        public string LangName { get; set; }
        public string LangImage { get; set; }
        public string LangParam { get; set; }
        public ICommand ChangeLanguage { get; set; }
    }

    public class LogsItems
    {
        public string LogName { get; set; }
        public string LogImage { get; set; }
        public ICommand ChangeLogs { get; set; }
    }

    public class CryptoSoftSettingsItem
    {
        public string SettingsName { get; set; }
        public string SettingsValue { get; set; }
        public ICommand ChangeSettings { get; set; }
    }

}
