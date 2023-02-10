using EasySave.Properties;
using EasySave.src.Models.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spectre.Console;
using System;
using System.IO;

namespace EasySave.src.Utils
{
    public static class LogUtils
    {

        private static string _path = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\EasySave\";
        private static string _date = DateTime.Now.ToString("yyyyMMdd");

        public static void Init()
        {
            if (!DirectoryUtils.IsValidPath(_path))
            {
                AnsiConsole.Clear();
                Directory.CreateDirectory(_path);
            }
            if (File.Exists($"{_path}saves.json"))
            {
                try
                {
                    Save.Init(JObject.Parse(File.ReadAllText($"{_path}saves.json")));

                }catch
                {
                    LogSaves();
                }
            }
        }

        public static void LogError(String message)
        {
            throw new NotImplementedException();
        }
        
        public static void LogSaves()
        {
           File.WriteAllText($"{_path}saves.json", SavesToJson().ToString());
        }

        private static JObject SavesToJson()
        {
            JObject data = new JObject();
            foreach (Save s in Save.GetSaves())
            {
                JObject saveData = SaveToJson(s);
                data.Add(s.uuid.ToString(), saveData);
            }
            return data;
        }

        public static void LogTransfer(String dest, String src)
        {
            throw new NotImplementedException();
        }

        public static dynamic SaveToJson(Save s)
        {
            JobStatus status = s.GetStatus();
            dynamic data = new JObject();
            data.name = s.Name;
            data.src = s.SrcDir.path;
            data.dest = s.DestDir.path;
            data.state = status.ToString();
            data.type = s.GetSaveType() == SaveType.Full ? SaveType.Full.ToString() : SaveType.Differential.ToString();
            data.totalFiles = s.SrcDir.NbFiles;
            data.totalSize = s.SrcDir.GetSize();
            if(status != JobStatus.Waiting)
            {
                data.filesLeft = s.SrcDir.NbFiles - s.GetFilesCopied();
                data.sizeLeft = s.SrcDir.GetSize() - s.GetSizeCopied();
                data.actualTransferSourcePath = s.getActualTransfer()[0];
                data.actualTransferDestPath = s.getActualTransfer()[1];
                data.progression = s.CalculateProgress();
            }
            return data;
        }
        public static void LogTransfer(Save s, string SourcePath, string DestinationPath, long FileSize, float FileTransferTime)
        {
            dynamic transferInfo = new JObject();
            transferInfo.name = $"{s.Name} ({s.uuid})";
            transferInfo.fileSource = SourcePath;
            transferInfo.fileTarget = DestinationPath;
            transferInfo.fileSize = FileSize;
            transferInfo.transferTime = FileTransferTime;
            transferInfo.date = DateTime.Now;

            string json = JsonConvert.SerializeObject(transferInfo);
            var arrayJson = JsonConvert.SerializeObject(new[] { transferInfo }, Formatting.Indented);

            if (File.Exists($"{_path}data-{_date}.json"))
            {
                JArray newJSON = ((JArray)JsonConvert.DeserializeObject(File.ReadAllText($"{_path}data-{_date}.json")));
                newJSON.Add(JsonConvert.DeserializeObject(json));
                arrayJson = JsonConvert.SerializeObject(newJSON, Formatting.Indented);
            }
            File.WriteAllText($"{_path}data-{_date}.json", arrayJson);
        }
    }
}
