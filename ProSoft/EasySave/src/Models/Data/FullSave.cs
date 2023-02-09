using EasySave.Properties;
using System;

namespace EasySave.src.Models.Data
{
    public class FullSave : Save
    {
        protected internal FullSave(string name, string src, string dest, Guid guid) : base(name, src, dest, guid) { }

        public override SaveType GetSaveType()
        {
            return SaveType.Full;
        }

        public override void Run()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"{Name} - {uuid} | {Resource.CreateSave_Type_Full}";
        }

        
    }
}
