using EasySave.src.Models.Data;
using EasySave.src.Utils;
using System.Collections.Generic;
using System.Linq;

namespace EasySave.src
{
    public class ViewModel
    {

        public HashSet<string> GetSaves()
        {
            HashSet<string> data = new HashSet<string>();
            foreach (Save s in Save.GetSaves())
                data.Add(s.ToString());
            return data;
        }

        public Save CreateSave(string name, string src, string dest, SaveType type)
        {
            return Save.CreateSave(name, src, dest, type);
        }

        public void EditSave(Save s, string name)
        {
            s.Rename(name);
        }

        public void DeleteSave(Save s)
        {
            Save.Delete(s.uuid);
        }

        public HashSet<Save> GetSavesByUuid(HashSet<string> names)
        {
            return new HashSet<Save>(Save.GetSaves().Where(save => names.Contains(save.ToString())).ToList());
        }

        public string RunSave(Save save)
        {
            return save.Run();
        }

        internal void StopAllSaves()
        {
            foreach (Save s in Save.GetSaves())
            {
                s.Stop();
            }
            LogUtils.LogSaves();
        }

        public static bool IsUpToDate()
        {
            return !VersionUtils.CompareVersions();
        }

    }
}
