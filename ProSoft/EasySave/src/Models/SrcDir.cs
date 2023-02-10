using EasySave.Properties;
using EasySave.src.Utils;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static System.Net.WebRequestMethods;

namespace EasySave.src.Models
{
    public class SrcDir : IDir
    {

        public string path { get; }

        private readonly double _size;

        public readonly long NbFiles;

        public SrcDir(string path)
        {
            this.path = path;
            DirectoryInfo directory = new DirectoryInfo(path);
            this._size = DirectoryUtils.GetDirectorySize(directory);
            this.NbFiles = DirectoryUtils.GetDirectoryFiles(directory);
        }

        public double GetSize()
        {
            return _size;
        }
    }
}
