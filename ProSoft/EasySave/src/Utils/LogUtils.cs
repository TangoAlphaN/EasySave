using EasySave.Properties;
using EasySave.src.Models.Data;
using Newtonsoft.Json.Linq;
using Spectre.Console;
using System;
using System.IO;

namespace EasySave.src.Utils
{
    public static class LogUtils
    {

        private static string _path = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\EasySave\";

        public static void Init()
        {
            if (!DirectoryUtils.IsValidPath(_path))
            {
                AnsiConsole.Clear();
                Directory.CreateDirectory(_path);
            }
            if (File.Exists($"{_path}saves.json"))
                Save.Init(JObject.Parse(File.ReadAllText($"{_path}saves.json")));
        }

        public static void LogError(String message)
        {
            throw new NotImplementedException();
        }
        
        public static bool LogSaves()
        {
            try
            {
                File.WriteAllText($"{_path}saves.json", SavesToJson().ToString());
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static JObject SavesToJson()
        {
            JObject data = new JObject();
            foreach (Save s in Save.GetSaves())
            {
                JObject saveData = SaveToJson(s);
                saveData.Add("state", s.GetStatus().ToString());
                saveData.Add("progression", s.CalculateProgress());
                data.Add(s.uuid.ToString(), saveData);
            }
            return data;
        }

        public static void LogTransfer(String dest, String src)
        {
            throw new NotImplementedException();
        }

        private static int CalculateTransferTime()
        {
            throw new NotImplementedException();
        }

        public static dynamic SaveToJson(Save s)
        {
            dynamic data = new JObject();
            data.name = s.Name;
            data.src = s.SrcDir.path;
            data.dest = s.DestDir.path;
            data.state = JobStatus.Waiting;
            data.type = s.GetSaveType() == SaveType.Full ? Resource.CreateSave_Type_Full : Resource.CreateSave_Type_Differential;
            return data;
        }
    }
}
