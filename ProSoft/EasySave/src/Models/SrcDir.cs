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

        private long _size;

        public readonly long NbFiles;

        public SrcDir(string path)
        {
            if(!DirectoryUtils.IsValidPath(path))
                throw new DirectoryNotFoundException();
            this.path = path;
            this._size = 0;
            this.NbFiles = 0;
        }

        public long CalculateSize(DirectoryInfo path = null)
        {
            if (path == null)
                path = new DirectoryInfo(this.path);
            _size = 0;
            foreach (FileInfo file in path.GetFiles())
                _size += file.Length;
            foreach (DirectoryInfo directory in path.GetDirectories())
            {
                CalculateSize(directory);
            }
            return _size; 
        } 

        public long CalculateNbFiles(DirectoryInfo path = null)
        {
            if (path == null)
                path = new DirectoryInfo(this.path);
            long i = 0;
            foreach (FileInfo file in path.GetFiles())
                i++;
            foreach (DirectoryInfo directory in path.GetDirectories())
            {
                CalculateNbFiles(directory);
            }
            return i;
        }

    }
}
