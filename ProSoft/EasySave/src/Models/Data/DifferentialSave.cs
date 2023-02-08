using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave.src.Models.Data
{
    public class DifferentialSave : Save
    {
        protected internal DifferentialSave(string name, string src, string dest) : base(name, src, dest) { }

        public override void Run()
        {
            throw new NotImplementedException();
        }
    }
}