using EasySave.src.Models.Exceptions;
using EasySave.src.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace EasySave.src.Models.Data
{
    /// <summary>
    /// Abstract class representing a save
    /// </summary>
    public abstract class Save
    {

        /// <summary>
        /// Maximum number of saves
        /// </summary>
        public const int MAX_SAVES = int.MaxValue;

        /// <summary>
        /// List of saves
        /// </summary>
        private static readonly HashSet<Save> saves = new HashSet<Save>();


        /// <summary>
        /// Save uuid
        /// </summary>
        public readonly Guid uuid;

        /// <summary>
        /// Save name
        /// </summary>
        private string _name;

        /// <summary>
        /// Number of files copied
        /// </summary>
        private long _filesCopied;

        /// <summary>
        /// Size of files copied
        /// </summary>
        private long _sizeCopied;

        /// <summary>
        /// Status of the save
        /// </summary>
        protected JobStatus Status;

        /// <summary>
        /// Source directory
        /// </summary>
        public readonly SrcDir SrcDir;

        /// <summary>
        /// Destination directory
        /// </summary>
        public readonly DestDir DestDir;

        /// <summary>
        /// Semaphore to change stat of save
        /// </summary>
        private SemaphoreSlim _semaphore = new SemaphoreSlim(0);

        /// <summary>
        /// Save constructor. Constructor is protected to prevent direct instantiation
        /// </summary>
        /// <param name="name">name of the save</param>
        /// <param name="src">source path</param>
        /// <param name="dest">destination path</param>
        /// <param name="guid">generated uuid</param>
        /// <param name="status">status, default at waiting</param>
        protected Save(string name, string src, string dest, Guid guid, JobStatus status = JobStatus.Waiting)
        {
            uuid = guid;
            _name = name;
            SrcDir = new SrcDir(src);
            DestDir = new DestDir(dest);
            Status = status;
        }

        /// <summary>
        /// Static method to instanciate a save
        /// </summary>
        /// <param name="name">name of the save</param>
        /// <param name="src">source path</param>
        /// <param name="dest">dest path</param>
        /// <param name="type">Type of save, full or differential</param>
        /// <returns>Save object</returns>
        /// <exception cref="TooMuchSavesException">When too much saves have been created</exception>
        public static Save CreateSave(string name, string src, string dest, SaveType type)
        {
            if (saves.Count > MAX_SAVES)
                throw new TooMuchSavesException();
            Save s = type switch
            {
                SaveType.Differential => new DifferentialSave(name, src, dest, Guid.NewGuid()),
                _ => new FullSave(name, src, dest, Guid.NewGuid()),
            };
            saves.Add(s);
            s.UpdateState();
            return s;
        }

        /// <summary>
        /// Method to calculate the progress of the save
        /// </summary>
        /// <returns>int between 0 and 100</returns>
        public int CalculateProgress()
        {
            return (int)(_sizeCopied / SrcDir.GetSize() * 100);
        }

        /// <summary>
        /// Method to rename a save
        /// </summary>
        /// <param name="newName">new name of the save</param>
        public void Rename(string newName)
        {
            _name = newName;
            UpdateState();
        }

        /// <summary>
        /// Method to pause a save
        /// </summary>
        public string Pause()
        {
            Status = JobStatus.Paused;
            Stopwatch sw = new Stopwatch();
            _semaphore.Wait();
            sw.Stop();
            return ProcessResult(sw);
        }

        /// <summary>
        /// Method to resume a save
        /// </summary>
        public string Resume()
        {
            Stopwatch sw = new Stopwatch();
            Status = JobStatus.Running;
            sw.Start();
            Thread thread = new Thread(() => DirectoryUtils.CopyFilesAndFolders(this));
            thread.Start();
            sw.Stop();
            Status = JobStatus.Finished;
            return ProcessResult(sw); ;
        }

        /// <summary>
        /// Method to cancel a save
        /// </summary>
        public string Cancel()
        {
            Stopwatch sw = new Stopwatch();
            _semaphore.Release();
            sw.Stop();
            Status = JobStatus.Canceled;
            return ProcessResult(sw); ;
        }

        /// <summary>
        /// Method to stop a save
        /// </summary>
        public string Stop()
        {
            Stopwatch sw = new Stopwatch();
            _semaphore.Release();
            sw.Stop();
            Status = JobStatus.Canceled;
            return ProcessResult(sw); ;
        }

        /// <summary>
        /// Static method to delete a save by uuid
        /// </summary>
        /// <param name="uuid">uuid of the save to delete</param>
        public static void Delete(Guid uuid)
        {
            Save save = saves.First(save => save.uuid == uuid);
            saves.Remove(save);
            LogUtils.LogSaves();
        }

        /// <summary>
        /// Abstract method to run a save
        /// A stopwatch is created, save is launched and stopwatch is stopped
        /// </summary>
        /// <returns>result of the save job</returns>
        public string Run()
        {
            Status = JobStatus.Running;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            DirectoryUtils.CopyFilesAndFolders(this);
            sw.Stop();
            Status = JobStatus.Finished;
            return ProcessResult(sw);
        }

        /// <summary>
        /// Protected method to get result of the save
        /// </summary>
        /// <param name="sw">stopwatch to mesure time</param>
        /// <returns>job result (json format)</returns>
        protected string ProcessResult(Stopwatch sw)
        {
            //Update of json data before returning result
            LogUtils.LogSaves();
            dynamic result = new JObject();
            result.name = _name;
            result.status = Status.ToString();
            result.filesCopied = _filesCopied;
            result.sizeCopied = $"{_sizeCopied / (1024 * 1024)} Mo";
            result.duration = $"{(int)sw.Elapsed.TotalSeconds}s";
            return result.ToString();
        }

        /// <summary>
        /// Method to actualise json data
        /// </summary>
        private void UpdateState()
        {
            LogUtils.LogSaves();
        }

        /// <summary>
        /// Method to add copied files
        /// </summary>
        /// <param name="nb">nb of copied files</param>
        public void AddFileCopied(int nb = 1)
        {
            _filesCopied += nb;
        }

        /// <summary>
        /// Static method to init save class
        /// </summary>
        /// <param name="data">json or xml data from log file</param>
        public static void Init(dynamic data)
        {
            //Data is read from json file and then saves are created
            if(LogUtils.GetFormat() == LogsFormat.XML)
            {
                foreach (var save in data.Root.Elements())
                {
                    if (!DirectoryUtils.IsValidPath(save.Element("src").Value.ToString())) return;
                    if (save.Element("type").Value.ToString() == "Full")
                        saves.Add(new FullSave(save.Element("name").Value.ToString(), save.Element("src").Value.ToString(), save.Element("dest").Value.ToString(), Guid.Parse(save.Attribute("id").Value.ToString()), Save.GetStatus(save.Element("state").Value.ToString())));
                    else
                        saves.Add(new DifferentialSave(save.Element("name").Value.ToString(), save.Element("src").Value.ToString(), save.Element("dest").Value.ToString(), Guid.Parse(save.Attribute("id").Value.ToString())));
                }
            }
            else
            {
                foreach (var save in data) {
                    if (!DirectoryUtils.IsValidPath(save.Value["src"].ToString())) return;
                    if (save.Value["type"].ToString() == "Full")
                        saves.Add(new FullSave(save.Value["name"].ToString(), save.Value["src"].ToString(), save.Value["dest"].ToString(), Guid.Parse(save.Name.ToString()), Save.GetStatus(save.Value["state"].ToString())));
                    else
                        saves.Add(new DifferentialSave(save.Value["name"].ToString(), save.Value["src"].ToString(), save.Value["dest"].ToString(), Guid.Parse(save.Name.ToString())));
                }
            }
           
        }

        /// <summary>
        /// Getter for copied size
        /// </summary>
        /// <returns>copied size</returns>
        public long GetSizeCopied()
        {
            return _sizeCopied;
        }

        /// <summary>
        /// Method to add copied size
        /// </summary>
        /// <param name="length">size of copied file to add</param>
        public void AddSizeCopied(long length)
        {
            _sizeCopied += length;
        }

        /// <summary>
        /// Getter for saves list
        /// </summary>
        /// <returns>Saves list</returns>
        public static HashSet<Save> GetSaves()
        {
            return saves;
        }

        /// <summary>
        /// Getter for save name
        /// </summary>
        /// <returns>Save name</returns>
        public string GetName()
        {
            return _name;
        }

        /// <summary>
        /// Getter for the copied files
        /// </summary>
        /// <returns>nb of copied files</returns>
        public long GetFilesCopied()
        {
            return _filesCopied;
        }

        /// <summary>
        /// Getter for save status
        /// </summary>
        /// <returns>save status</returns>
        public JobStatus GetStatus()
        {
            return Status;
        }

        /// <summary>
        /// Static method to convert a string into a status
        /// </summary>
        /// <param name="status">status (string)</param>
        /// <returns>status (JobStatus)</returns>
        public static JobStatus GetStatus(string status)
        {
            return status switch
            {
                "Running" => JobStatus.Running,
                "Paused" => JobStatus.Paused,
                "Finished" => JobStatus.Finished,
                "Canceled" => JobStatus.Canceled,
                "Error" => JobStatus.Error,
                _ => JobStatus.Waiting,
            };
        }

        /// <summary>
        /// ToString method
        /// </summary>
        /// <returns>save description</returns>
        public override abstract string ToString();

        /// <summary>
        /// Abstract method to get save type
        /// </summary>
        /// <returns>save type</returns>
        public abstract SaveType GetSaveType();

    }
}
