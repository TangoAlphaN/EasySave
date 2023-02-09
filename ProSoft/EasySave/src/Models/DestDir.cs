using EasySave.src.Utils;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave.src.Models
{
    public class DestDir : IDir
    {

        public string path { get; }

        public DestDir(string path)
        {
            if (!DirectoryUtils.IsValidPath(path))
                DirectoryUtils.CreatePath(path);
            this.path = path;
        }

    }
}
