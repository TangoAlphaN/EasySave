using EasySave.Properties;
using EasySave.src.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;

namespace EasySave.src.Models.Data
{
    public class FullSave : Save
    {
        protected internal FullSave(string name, string src, string dest, Guid guid, JobStatus status = JobStatus.Waiting) : base(name, src, dest, guid, status) { }

        public override SaveType GetSaveType()
        {
            return SaveType.Full;
        }

        public override string Run()
        {
            Status = JobStatus.Running;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            DirectoryUtils.CopyFilesAndFolders(this);
            sw.Stop();
            Status = JobStatus.Finished;
            return ProcessResult(sw);
        }

        public override string ToString()
        {
            return $"{Name} - {uuid} | {Resource.CreateSave_Type_Full}";
        }

        
    }
}
