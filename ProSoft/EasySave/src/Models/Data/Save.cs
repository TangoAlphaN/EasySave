﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasySave.src.Models.Data
{
    public abstract class Save
    {

        public const int MAX_SAVES = 5;

        private static HashSet<Save> saves = new HashSet<Save>();

        private Guid uuid;

        private string name;

        private int progress;

        private JobStatus status;

        private readonly SrcDir srcDir;

        private readonly DestDir destDir;

        protected Save(string name, string src, string dest)
        {
            this.uuid = Guid.NewGuid();
            this.name = name;
            this.srcDir = new SrcDir(src);
            this.destDir = new DestDir(dest);
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
            //TODO save in logs
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

    }
}
