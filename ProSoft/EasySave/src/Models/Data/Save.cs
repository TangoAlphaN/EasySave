using EasySave.src.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using EasySave.src.Utils;
using Newtonsoft.Json.Linq;

namespace EasySave.src.Models.Data
{
    public abstract class Save
    {

        public const int MAX_SAVES = 5;

        private static HashSet<Save> saves = new HashSet<Save>();

        public readonly Guid uuid;

        public string Name;

        private long _filesCopied;

        private JobStatus _status;

        public readonly SrcDir SrcDir;

        public readonly DestDir DestDir;

        protected Save(string name, string src, string dest, Guid guid)
        {
            this.uuid = guid;
            this.Name = name;
            this.SrcDir = new SrcDir(src);
            this.DestDir = new DestDir(dest);
        }

        public static HashSet<Save> GetSaves()
        {
            return saves;
        }

        public static Save CreateSave(string name, string src, string dest, SaveType type)
        {
            if (Save.saves.Count >= MAX_SAVES)
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
            return 0;
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
            throw new NotImplementedException();
        }

        public void Resume()
        {
            throw new NotImplementedException();
        }

        public void Cancel() {
            throw new NotImplementedException();
        }

        public static void Delete(Guid uuid)
        {
            Save save = saves.First(save => save.uuid == uuid);
            saves.Remove(save);
            LogUtils.LogSaves();
        }

        public abstract void Run();

        public Save GetSaveByUuid(Guid uuid)
        {
            return (Save)saves.Where(save => save.uuid == uuid);
        }

        public long GetFilesCopied()
        {
            return _filesCopied;
        }

        public JobStatus GetStatus()
        {
            return _status;
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
            foreach(JArray save in jObject)
            {
                Console.WriteLine(save[]);
                /*if (save.type == "Full Save")
                    saves.Add(new FullSave(save.name, save.src, save.dest));
                else
                    saves.Add(new DifferentialSave(save.name, save.src, save.dest));*/

            }
        }
    }
}
