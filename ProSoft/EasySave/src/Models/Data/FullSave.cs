using EasySave.Properties;
using EasySave.src.Utils;
using System;
using System.Diagnostics;

namespace EasySave.src.Models.Data
{
    /// <summary>
    /// Full save class
    /// </summary>
    public class FullSave : Save
    {

        protected internal FullSave(string name, string src, string dest, Guid guid, JobStatus status = JobStatus.Waiting) : base(name, src, dest, guid, status) { }

        public override SaveType GetSaveType()
        {
            return SaveType.Full;
        }

        public override string ToString()
        {
            return $"{GetName()} - {uuid} | {Resource.CreateSave_Type_Full}";
        }

    }
}
