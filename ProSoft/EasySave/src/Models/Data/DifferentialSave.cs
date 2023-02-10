using EasySave.Properties;
using EasySave.src.Utils;
using System;
using System.Diagnostics;

namespace EasySave.src.Models.Data
{
    public class DifferentialSave : Save
    {
        protected internal DifferentialSave(string name, string src, string dest, Guid guid, JobStatus status = JobStatus.Waiting) : base(name, src, dest, guid, status) { }

        public override SaveType GetSaveType()
        {
            return SaveType.Differential;
        }

        public override string Run()
        {
            Status = JobStatus.Running;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            DirectoryUtils.CopyFilesAndFolders(this, SaveType.Differential);
            sw.Stop();
            Status = JobStatus.Finished;
            return ProcessResult(sw);
        }

        public override string ToString()
        {
            return $"{GetName()} - {uuid} | {Resource.CreateSave_Type_Differential}";
        }

    }
}