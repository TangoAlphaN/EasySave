using EasySave.Properties;
using EasySave.src.Models.Data;
using EasySave.src.Render;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Notifications.Wpf;
using ProSoft.CryptoSoft;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace EasySave.src.Utils
{
    /// <summary>
    /// Static class to manage directory actions
    /// </summary>
    public static class DirectoryUtils
    {
        private static string key = JObject.Parse(File.ReadAllText($"{LogUtils.path}config.json"))["key"].ToString();

        private static HashSet<string> extensions = JObject.Parse(File.ReadAllText($"{LogUtils.path}config.json"))["extensions"].Select(t => t.ToString()).ToHashSet();
        
        private static HashSet<string> process = JObject.Parse(File.ReadAllText($"{LogUtils.path}config.json"))["process"].Select(t => t.ToString()).ToHashSet();

        /// <summary>
        /// Array to store the actual file being copied
        /// </summary>
        private static readonly string[] actualFile = new string[2];

        /// <summary>
        /// Copy all files and folders from a source directory to a destination directory
        /// </summary>
        /// <param name="s">save concerned</param>
        /// <returns></returns>
        public static void CopyFilesAndFolders(Save s)
        {
            CryptoSoft cs = CryptoSoft.Init(key);
            DirectoryInfo sourceDirectory = new DirectoryInfo(s.SrcDir.Path);
            DirectoryInfo destinationDirectory = new DirectoryInfo(s.DestDir.Path);
            CopyAll(cs, s, sourceDirectory, destinationDirectory, s.GetSaveType());
        }

        /// <summary>
        /// Method to copy all files and folders from a source directory to a destination directory
        /// </summary>
        /// <param name="cs">cryptosofct instance</param>
        /// <param name="s">concerned save</param>
        /// <param name="src">source dir</param>
        /// <param name="dest">destination dir</param>
        /// <param name="type">type of save</param>
        private static void CopyAll(CryptoSoft cs, Save s, DirectoryInfo src, DirectoryInfo dest, SaveType type)
        {
            var notificationManager = new NotificationManager();

            foreach (FileInfo file in src.GetFiles())
            {
                foreach (var p in process)
                {
                    if (Process.GetProcessesByName(p).Length > 0)
                    {
                        notificationManager.Show(new NotificationContent
                        {
                            Title = "Save Error",
                            Message = "Process \"[PROCESS]\"is still running.".Replace("[PROCESS]", p.Split(".exe")[0]),
                            Type = NotificationType.Error
                        });
                        s.Stop();
                        return;
                    }
                }
                //Update json data
                LogUtils.LogSaves();
                bool fileCopied = true;
                bool fileExists = File.Exists(Path.Combine(dest.FullName, file.Name));
                //Proceed differential mode by comparing files data
                if (type == SaveType.Full || !fileExists || (DateTime.Compare(File.GetLastWriteTime(Path.Combine(dest.FullName, file.Name)), File.GetLastWriteTime(Path.Combine(src.FullName, file.Name))) < 0))
                {
                    actualFile[0] = src.FullName;
                    actualFile[1] = dest.FullName;
                    //Stopwatch to mesure transfer time
                    var watch = new System.Diagnostics.Stopwatch();
                    long encryptionTime = -2;
                    watch.Start();
                    try
                    {
                        if (extensions.Contains(file.Extension))
                            encryptionTime = cs.ProcessFile(Path.Combine(src.FullName, file.Name), Path.Combine(dest.FullName, $"{file.Name}.enc"));
                        else
                            file.CopyTo(Path.Combine(dest.FullName, file.Name), true);
                    }
                    catch
                    {
                        fileCopied = false;
                        View.WriteError($"{Path.Combine(dest.FullName, file.Name)} | {Resource.AccesDenied}");
                    }
                    watch.Stop();
                    //Log transfer in json
                    LogUtils.LogTransfer(s, Path.Combine(src.FullName, file.Name), Path.Combine(dest.FullName, file.Name), file.Length, watch.ElapsedMilliseconds, encryptionTime);
                }
                if (fileCopied)
                    s.AddFileCopied();
                s.AddSizeCopied(file.Length);
            }

            //Recursive call for subdirectories
            foreach (DirectoryInfo directory in src.GetDirectories())
            {
                DirectoryInfo nextTarget = dest.CreateSubdirectory(directory.Name);
                CopyAll(cs, s, directory, nextTarget, type);
            }
        }

        /// <summary>
        /// check if path is valid
        /// </summary>
        /// <param name="path">path to test</param>
        /// <returns>true if valid path, else otherwise</returns>
        public static bool IsValidPath(string path)
        {
            return Directory.Exists(path);
        }

        /// <summary>
        /// Create a directory
        /// </summary>
        /// <param name="path">path of dir</param>
        public static void CreatePath(string path)
        {
            Directory.CreateDirectory(path);
        }

        /// <summary>
        /// get directory size method
        /// </summary>
        /// <param name="path">directory</param>
        /// <returns>size of the directory</returns>
        public static double GetDirectorySize(DirectoryInfo path)
        {
            double size = 0;
            foreach (FileInfo file in path.GetFiles())
                size += file.Length;
            foreach (DirectoryInfo directory in path.GetDirectories())
                size += GetDirectorySize(directory);
            return size;
        }

        /// <summary>
        /// get number of files in a directory
        /// </summary>
        /// <param name="path">directory</param>
        /// <returns>files in directory</returns>
        public static long GetDirectoryFiles(DirectoryInfo path)
        {
            long nbFiles = 0;
            foreach (FileInfo file in path.GetFiles())
                nbFiles++;
            foreach (DirectoryInfo directory in path.GetDirectories())
                nbFiles += GetDirectoryFiles(directory);
            return nbFiles;
        }

        /// <summary>
        /// get actual file being copied
        /// </summary>
        /// <returns>actual file</returns>
        public static string[] GetActualFile()
        {
            return actualFile;
        }

        /// <summary>
        /// methode to update secret key
        /// </summary>
        /// <param name="newSecret">secret key</param>
        public static void ChangeKey(string newSecret)
        {
            key = newSecret;
            UpdateConfig();
        }

        public static void ChangeExtensions(HashSet<string> newExtensions)
        {
            extensions = newExtensions;
            UpdateConfig();
        }

        public static void ChangeProcess(HashSet<string> newProcess)
        {
            process = newProcess;
            UpdateConfig();
        }

        private static void UpdateConfig()
        {
            LogUtils.LogConfig(key, extensions, process);
        }

        public static string GetSecret()
        {
            try
            {
                return key;
            }
            catch
            {
                return $"Please set a key in {LogUtils.path}config.json";
            }
        }

        public static string GetExtensions()
        {
            return string.Join("\r\n", extensions);
        }

        public static string GetProcess()
        {
            return string.Join("\r\n", process);
        }

    }
}
