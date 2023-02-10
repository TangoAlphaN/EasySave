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

        public string Path { get; }

        private readonly double _size;

        public readonly long NbFiles;

        public SrcDir(string path)
        {
            Path = path;
            DirectoryInfo directory = new DirectoryInfo(path);
            _size = DirectoryUtils.GetDirectorySize(directory);
            NbFiles = DirectoryUtils.GetDirectoryFiles(directory);
        }

        public double GetSize()
        {
            return _size;
        }

    }
}
