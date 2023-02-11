using EasySave.src.Models.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spectre.Console;
using System;
using System.IO;

namespace EasySave.src.Utils
{
    /// <summary>
    /// Log utils class
    /// </summary>
    public static class LogUtils
    {

        /// <summary>
        /// Path to the log file
        /// </summary>
        private static readonly string _path = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\EasySave\";

        /// <summary>
        /// Date of the day
        /// </summary>
        private static readonly string _date = DateTime.Now.ToString("yyyyMMdd");

        /// <summary>
        /// Init logs util
        /// </summary>
        public static void Init()
        {
            //Create log file if not exists
            if (!DirectoryUtils.IsValidPath(_path))
            {
                AnsiConsole.Clear();
                Directory.CreateDirectory(_path);
            }
            //If file exists, load saves
            if (File.Exists($"{_path}saves.json"))
            {
                try
                {
                    Save.Init(JObject.Parse(File.ReadAllText($"{_path}saves.json")));

                }
                catch
                {
                    LogSaves();
                }
            }
        }

        /// <summary>
        /// Method to log saves to json
        /// </summary>
        public static void LogSaves()
        {
            File.WriteAllText($"{_path}saves.json", SavesToJson().ToString());
        }

        /// <summary>
        /// Convert saves into json
        /// </summary>
        /// <returns>json object</returns>
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

        /// <summary>
        /// Convert save into json
        /// </summary>
        /// <param name="s">save</param>
        /// <returns>json object</returns>
        public static JObject SaveToJson(Save s)
        {
            JobStatus status = s.GetStatus();
            dynamic data = new JObject();
            data.name = s.GetName();
            data.src = s.SrcDir.Path;
            data.dest = s.DestDir.Path;
            data.state = status.ToString();
            data.type = s.GetSaveType() == SaveType.Full ? SaveType.Full.ToString() : SaveType.Differential.ToString();
            data.totalFiles = s.SrcDir.NbFiles;
            data.totalSize = s.SrcDir.GetSize();
            if (status != JobStatus.Waiting)
            {
                string[] actualFile = DirectoryUtils.GetActualFile();
                data.filesLeft = s.SrcDir.NbFiles - s.GetFilesCopied();
                data.sizeLeft = s.SrcDir.GetSize() - s.GetSizeCopied();
                data.actualTransferSourcePath = actualFile[0];
                data.actualTransferDestPath = actualFile[1];
                data.progression = s.CalculateProgress();
            }
            return data;
        }

        /// <summary>
        /// Log transfer
        /// </summary>
        /// <param name="s">save</param>
        /// <param name="sourcePath">source path</param>
        /// <param name="destinationPath">destination path</param>
        /// <param name="fileSize">file size</param>
        /// <param name="fileTransferTime">file transfer time</param>
        public static void LogTransfer(Save s, string sourcePath, string destinationPath, long fileSize, float fileTransferTime)
        {
            dynamic transferInfo = new JObject();
            transferInfo.name = $"{s.GetName()} ({s.uuid})";
            transferInfo.fileSource = sourcePath;
            transferInfo.fileTarget = destinationPath;
            transferInfo.fileSize = fileSize;
            transferInfo.transferTime = fileTransferTime;
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

        public static void JsonToXml(string json)
        {
            var xml = JsonConvert.DeserializeObject(json)!;
            if (xml != null) File.WriteAllText($"{_path}data-{_date}.xml", xml.ToString());
        }

    }
}
