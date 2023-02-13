using EasySave.src.Models.Data;
using EasySave.src.Models.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Xml.Linq;

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

        private static LogsFormat _format;

        /// <summary>
        /// Date of the day
        /// </summary>
        private static readonly string _date = DateTime.Now.ToString("yyyyMMdd");

        /// <summary>
        /// Init logs util
        /// </summary>
        public static void Init()
        {
            //Create easysave dir if not exists
            if (!DirectoryUtils.IsValidPath(_path))
            {
                //AnsiConsole.Clear();
                Directory.CreateDirectory(_path);
            }
            //If XML file exists, load saves and set XML as default
            dynamic data;
            if (File.Exists($"{_path}saves.json") || File.Exists($"{_path}saves.xml"))
            {
                if (File.Exists($"{_path}saves.xml"))
                {
                    _format = LogsFormat.XML;
                    data = XDocument.Load($"{_path}saves.xml");
                }
                //else use JSON
                else
                {
                    _format = LogsFormat.JSON;
                    data = JObject.Parse(File.ReadAllText($"{_path}saves.json"));
                }
                try
                {
                    Save.Init(data);
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
            if(_format == LogsFormat.XML)
                new XDocument(SavesToXml()).Save($"{_path}saves.xml");
            else
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
        /// Convert saves into xml
        /// </summary>
        /// <returns>xml object</returns>
        private static XElement SavesToXml()
        {
            XElement root = new XElement("root");
            foreach (Save s in Save.GetSaves())
            {
                XElement saveData = SaveToXml(s);
                root.Add(saveData);
            }
            return root;
        }

        /// <summary>
        /// Convert save into xml
        /// </summary>
        /// <param name="s">save</param>
        /// <returns>xml object</returns>
        public static XElement SaveToXml(Save s)
        {
            JobStatus status = s.GetStatus();
            dynamic data = new XElement(
                "item",
                new XAttribute("id", s.uuid.ToString()),
                new XElement("name", s.GetName()),
                new XElement("src", s.SrcDir.Path),
                new XElement("dest", s.DestDir.Path),
                new XElement("state", status.ToString()),
                new XElement("type", s.GetSaveType() == SaveType.Full ? SaveType.Full.ToString() : SaveType.Differential.ToString()),
                new XElement("totalFiles", s.SrcDir.NbFiles),
                new XElement("totalSize", s.SrcDir.GetSize())
            );
            if (status != JobStatus.Waiting)
            {
                string[] actualFile = DirectoryUtils.GetActualFile();
                data.Add(new XElement("filesLeft", s.SrcDir.NbFiles - s.GetFilesCopied()));
                data.Add(new XElement("sizeLeft", s.SrcDir.GetSize() - s.GetSizeCopied()));
                data.Add(new XElement("actualTransferSourcePath", actualFile[0]));
                data.Add(new XElement("actualTransferDestPath", actualFile[1]));
                data.Add(new XElement("progression", s.CalculateProgress()));
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
            dynamic transferInfo;
            if (_format == LogsFormat.XML)
            {
                transferInfo = new XElement(
                    "transfer",
                    new XElement("name", $"{s.GetName()} ({s.uuid})"),
                    new XElement("fileSource", sourcePath),
                    new XElement("fileTarget", destinationPath),
                    new XElement("fileSize", fileSize),
                    new XElement("transferTime", fileTransferTime),
                    new XElement("date", DateTime.Now)
                );
                dynamic data;
                if (File.Exists($"{_path}data-{_date}.xml"))
                    data = XDocument.Load($"{_path}data-{_date}.xml");
                else
                    data = new XDocument(new XElement("transfers"));
                data.Element("transfers").Add(transferInfo);
                data.Save($"{_path}data-{_date}.xml");
            }
            else {
                transferInfo = new JObject();
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
        }


        /// <summary>
        /// static method to change the format of the logs
        /// </summary>
        /// <param name="format">format wanted</param>
        /// <exception cref="UnknownLogFormatException">throwed when format is unknown</exception>
        public static void ChangeFormat(LogsFormat format)
        {
            _format = format;
            LogSaves();
            switch (format)
            {
                case LogsFormat.JSON:
                    if (File.Exists($"{_path}saves.xml"))
                        File.Delete($"{_path}saves.xml");
                    break;
                case LogsFormat.XML:
                    if (File.Exists($"{_path}saves.json"))
                        File.Delete($"{_path}saves.json");
                    break;
                default:
                    throw new UnknownLogFormatException();
            }
        }

        /// <summary>
        /// getter for logs format
        /// </summary>
        /// <returns>logs format</returns>
        public static LogsFormat GetFormat()
        {
            return _format;
        }

    }
}
