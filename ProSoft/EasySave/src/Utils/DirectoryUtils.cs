using EasySave.Properties;
using EasySave.src.Models;
using EasySave.src.Models.Data;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace EasySave.src.Utils
{
    public static class DirectoryUtils
    {
        public static bool CopyFilesAndFolders(Save s)
        {
            DirectoryInfo sourceDirectory = new DirectoryInfo(s.SrcDir.path);
            DirectoryInfo destinationDirectory = new DirectoryInfo(s.DestDir.path);
            Parallel.Invoke(
                () => ConsoleUtils.CreateProgressBar(s),
                () => CopyAll(s, sourceDirectory, destinationDirectory)
            );
            return true;
        }

        private static void CopyAll(Save s, DirectoryInfo src, DirectoryInfo dest)
        {
            foreach (FileInfo file in src.GetFiles())
            {
                Thread.Sleep(50);
                file.CopyTo(Path.Combine(dest.FullName, file.Name), true);
                s.AddFileCopied();
            }

            foreach (DirectoryInfo directory in src.GetDirectories())
            {
                DirectoryInfo nextTarget = dest.CreateSubdirectory(directory.Name);
                CopyAll(s, directory, nextTarget);
            }
        }

        public static bool IsValidPath(String path)
        {
            return Directory.Exists(path);
        }

        public static void CreatePath(string path)
        {
            Directory.CreateDirectory(path);
        }
    }
}
