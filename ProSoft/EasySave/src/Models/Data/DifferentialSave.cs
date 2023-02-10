﻿using EasySave.Properties;
using EasySave.src.Utils;
using System;

namespace EasySave.src.Models.Data
{
    public class DifferentialSave : Save
    {
        protected internal DifferentialSave(string name, string src, string dest, Guid guid, JobStatus status = JobStatus.Waiting) : base(name, src, dest, guid, status) { }

        public override SaveType GetSaveType()
        {
            return SaveType.Differential;
        }

        public override void Run()
        {
            Status = JobStatus.Running;
            DirectoryUtils.CopyFilesAndFolders(this, SaveType.Differential);
            Status = JobStatus.Finished;
            LogUtils.LogSaves();
        }

        public override string ToString()
        {
            return $"{Name} - {uuid} | {Resource.CreateSave_Type_Differential}";
        }
    }
}