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
        public static bool CopyFilesAndFolders(Save s, SaveType type = SaveType.Full)
        {
            DirectoryInfo sourceDirectory = new DirectoryInfo(s.SrcDir.path);
            DirectoryInfo destinationDirectory = new DirectoryInfo(s.DestDir.path);
            Parallel.Invoke(
                () => ConsoleUtils.CreateProgressBar(s),
                () => CopyAll(s, sourceDirectory, destinationDirectory, type)
            );
            return true;
        }

        private static void CopyAll(Save s, DirectoryInfo src, DirectoryInfo dest, SaveType type)
        {
            foreach (FileInfo file in src.GetFiles())
            {
                Thread.Sleep(50);
                bool fileExists = File.Exists(Path.Combine(dest.FullName, file.Name));
                if (type == SaveType.Full || !fileExists || (fileExists && DateTime.Compare(File.GetLastWriteTime(Path.Combine(dest.FullName, file.Name)), File.GetLastWriteTime(Path.Combine(src.FullName, file.Name))) < 0))
                {
                    file.CopyTo(Path.Combine(dest.FullName, file.Name), true);
                    s.AddFileCopied();
                }
            }

            foreach (DirectoryInfo directory in src.GetDirectories())
            {
                DirectoryInfo nextTarget = dest.CreateSubdirectory(directory.Name);
                CopyAll(s, directory, nextTarget, type);
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
