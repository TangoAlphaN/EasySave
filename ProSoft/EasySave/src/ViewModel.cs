using EasySave.src.Models.Data;
using EasySave.src.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;


namespace EasySave.src
{
    /// <summary>
    /// View model class
    /// </summary>
    public class ViewModel
    {
        
        private ICommand GoToPageHomeCommand { get; set; }
        
        private object CurrentPage { get; set; }

        public ViewModel()
        {
            GoToPageHomeCommand = new RelayCommand(GoToHomePage);
        }

        private void GoToHomePage()
        {
            CurrentPage = new HomePage();
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

    }
}
