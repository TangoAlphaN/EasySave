using EasySave.src.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using EasySave.src.Utils;
using Newtonsoft.Json.Linq;
using Spectre.Console;
using System.Diagnostics;

namespace EasySave.src.Models.Data
{
    public abstract class Save
    {

        public const int MAX_SAVES = 5;

        private static HashSet<Save> saves = new HashSet<Save>();

        public readonly Guid uuid;

        public string Name;

        private long _filesCopied;
        
        private long _sizeCopied;

        protected JobStatus Status;

        public readonly SrcDir SrcDir;

        public readonly DestDir DestDir;

        protected Save(string name, string src, string dest, Guid guid, JobStatus status = JobStatus.Waiting)
        {
            this.uuid = guid;
            this.Name = name;
            this.SrcDir = new SrcDir(src);
            this.DestDir = new DestDir(dest);
            this.Status = status;
        }

        public static HashSet<Save> GetSaves()
        {
            return saves;
        }

        public static Save CreateSave(string name, string src, string dest, SaveType type)
        {
            if (saves.Count > MAX_SAVES)
            {
                throw new Exception("Too Much Saves");
            }
            Save s;
            switch (type)
            {
                case SaveType.Differential:
                    s = new DifferentialSave(name, src, dest, Guid.NewGuid());
                    break;
                case SaveType.Full:
                    s = new FullSave(name, src, dest, Guid.NewGuid());
                    break;
                default:
                    throw new Exception("Save type not allowed");
            }
            Save.saves.Add(s);
            s.UpdateState();
            return s;
        }

        public int CalculateProgress()
        {
            return (int)(_sizeCopied / SrcDir.GetSize() * 100);
        }

        private int CalculateRemainingTime()
        {
            throw new NotImplementedException();
        }

        public void Rename(string newName)
        {
            Name = newName;
            UpdateState();
        }

        public void Pause()
        {
            Status = JobStatus.Paused;
        }

        public void Resume()
        {
            Status = JobStatus.Running;
        }

        public void Cancel() {
            Status = JobStatus.Canceled;
        }

        public void Stop()
        {
            Status = JobStatus.Waiting;
        }

        public static void Delete(Guid uuid)
        {
            Save save = saves.First(save => save.uuid == uuid);
            saves.Remove(save);
            LogUtils.LogSaves();
        }

        public abstract string Run();

        public Save GetSaveByUuid(Guid uuid)
        {
            return (Save)saves.Where(save => save.uuid == uuid);
        }

        protected string ProcessResult(Stopwatch sw)
        {
            LogUtils.LogSaves();
            dynamic result = new JObject();
            result.name = Name;
            result.status = Status.ToString();
            result.filesCopied = _filesCopied;
            result.sizeCopied = $"{_sizeCopied / (1024*1024)} Mo";
            result.duration = $"{(int)sw.Elapsed.TotalSeconds}s";
            return result.ToString();
        }

        public long GetFilesCopied()
        {
            return _filesCopied;
        }

        public JobStatus GetStatus()
        {
            return Status;
        }

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

        public override abstract string ToString();

        private void UpdateState()
        {
            LogUtils.LogSaves();
        }

        public abstract SaveType GetSaveType();

        public void AddFileCopied()
        {
            _filesCopied++;
        }

        public static void Init(JObject jObject)
        {
            foreach (var save in jObject)
            {
                if (!DirectoryUtils.IsValidPath(save.Value["src"].ToString())) return;
                if (save.Value["type"].ToString() == "Full")
                    saves.Add(new FullSave(save.Value["name"].ToString(), save.Value["src"].ToString(), save.Value["dest"].ToString(), Guid.Parse(save.Key.ToString()), Save.GetStatus(save.Value["state"].ToString())));
                else
                    saves.Add(new DifferentialSave(save.Value["name"].ToString(), save.Value["src"].ToString(), save.Value["dest"].ToString(), Guid.Parse(save.Key.ToString())));
            }
        }

        internal long GetSizeCopied()
        {
            return _sizeCopied;
        }

        internal void AddSizeCopied(long length)
        {
            _sizeCopied += length;
        }
    }
}
