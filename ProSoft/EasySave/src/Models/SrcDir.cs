using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave.src.Models
{
    public class SrcDir : IDir
    {

        public string path { get; }

        private long _size;

        private long _nbFiles;

        public SrcDir(string path)
        {
            this.path = path;
            this._size = 0;
            this._nbFiles = 0;
        }

        public long CalculateSize()
        {
            throw new NotImplementedException();
        } 

        public long CalculateNbFiles()
        {
            throw new NotImplementedException();
        } 

    }
}
