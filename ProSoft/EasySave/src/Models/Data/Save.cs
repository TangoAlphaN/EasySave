using System;
using System.Collections.Generic;
using System.Linq;
using EasySave.src.Utils;

namespace EasySave.src.Models.Data
{
    public abstract class Save
    {

        public const int MAX_SAVES = 5;

        private static HashSet<Save> saves = new HashSet<Save>();

        public readonly Guid uuid;

        protected string name;

        private int _progress;

        private JobStatus _status;

        private readonly SrcDir _srcDir;

        private readonly DestDir _destDir;

        protected Save(string name, string src, string dest)
        {
            this.uuid = Guid.NewGuid();
            this.name = name;
            this._srcDir = new SrcDir(src);
            this._destDir = new DestDir(dest);
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
                    s = new DifferentialSave(name, src, dest);
                    break;
                case SaveType.Full:
                    s = new FullSave(name, src, dest);
                    break;
                default:
                    throw new Exception("Save type not allowed");
            }
            Save.saves.Add(s);
            s.UpdateState();
            return s;
        }

        private int CalculateProgress()
        {
            throw new NotImplementedException();
        }

        private int CalculateRemainingTime()
        {
            throw new NotImplementedException();
        }

        public void Rename(string newName)
        {
            name = newName;
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
            throw new NotImplementedException();
        }

        public abstract void Run();

        public Save GetSaveByUuid(Guid uuid)
        {
            return (Save)Save.saves.Where(save => save.uuid == uuid);
        }

        public override abstract string ToString();

        private void UpdateState()
        {
            LogUtils.LogSave(this);
        }

    }
}
