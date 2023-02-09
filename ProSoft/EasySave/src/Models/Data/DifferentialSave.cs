using EasySave.Properties;
using System;

namespace EasySave.src.Models.Data
{
    public class DifferentialSave : Save
    {
        protected internal DifferentialSave(string name, string src, string dest, Guid guid) : base(name, src, dest, guid) { }

        public override SaveType GetSaveType()
        {
            return SaveType.Differential;
        }

        public override void Run()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"{Name} - {uuid} | {Resource.CreateSave_Type_Differential}";
        }
    }
}