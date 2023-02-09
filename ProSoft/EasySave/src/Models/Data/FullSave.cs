using EasySave.Properties;
using System;

namespace EasySave.src.Models.Data
{
    public class FullSave : Save
    {
        protected internal FullSave(string name, string src, string dest) : base(name, src, dest) { }

        public override void Run()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"{name} - {uuid} | {Resource.CreateSave_Type_Full}";
        }
    }
}
