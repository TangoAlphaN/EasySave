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

        private static string[] actualFile = new string[2];

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
                LogUtils.LogSaves();
                bool fileExists = File.Exists(Path.Combine(dest.FullName, file.Name));
                if (type == SaveType.Full || !fileExists || (DateTime.Compare(File.GetLastWriteTime(Path.Combine(dest.FullName, file.Name)), File.GetLastWriteTime(Path.Combine(src.FullName, file.Name))) < 0))
                {
                    actualFile[0] = src.FullName;
                    actualFile[1] = dest.FullName;
                    file.CopyTo(Path.Combine(dest.FullName, file.Name), true);
                }
                s.AddFileCopied();
                s.AddSizeCopied(file.Length);
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

        public static double GetDirectorySize(DirectoryInfo path)
        {
            double size = 0;
            foreach (FileInfo file in path.GetFiles())
                size += file.Length;
            foreach (DirectoryInfo directory in path.GetDirectories())
                size += GetDirectorySize(directory);
            return size;
        }

        public static long GetDirectoryFiles(DirectoryInfo path)
        {
            long nbFiles = 0;
            foreach (FileInfo file in path.GetFiles())
                nbFiles++;
            foreach (DirectoryInfo directory in path.GetDirectories())
                nbFiles += GetDirectoryFiles(directory);
            return nbFiles;
        }


    }
}
