using EasySave.Properties;
using EasySave.src.Models.Data;
using Newtonsoft.Json.Linq;
using Notification.Wpf;
using ProSoft.CryptoSoft;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace EasySave.src.Utils
{
    /// <summary>
    /// Static class to manage directory actions
    /// </summary>
    public static class DirectoryUtils
    {

        private static readonly JObject data = JObject.Parse(File.ReadAllText($"{LogUtils.path}config.json"));

        private static string key = data["key"].ToString();

        private static HashSet<string> extensions = data["extensions"].Select(t => t.ToString()).ToHashSet();

        private static HashSet<string> process = data["process"].Select(t => t.ToString()).ToHashSet();

        private static HashSet<string> priorityFiles = data["priorityFiles"].Select(t => t.ToString()).ToHashSet();

        private static int limitSize = int.Parse(data["limitSize"].ToString());

        private static readonly Mutex _mutex = new Mutex();

        private static CryptoSoft cs = CryptoSoft.Init(key);

        private static ManualResetEvent mre = new ManualResetEvent(true);

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
            DirectoryInfo sourceDirectory = new DirectoryInfo(s.SrcDir.Path);
            DirectoryInfo destinationDirectory = new DirectoryInfo(s.DestDir.Path);
            Dictionary<FileInfo, FileInfo> files = GetAllFiles(sourceDirectory, destinationDirectory);
            switch (CopyAll(s, files, mre))
            {
                case JobStatus.Canceled:
                    NotificationUtils.SendNotification(
                        title: $"{s.GetName()} - {s.uuid}",
                        message: Resource.Header_SavePaused,
                        type: NotificationType.Success
                    );
                    s.Cancel();
                    break;
                case JobStatus.Finished:
                    NotificationUtils.SendNotification(
                        title: $"{s.GetName()} - {s.uuid}",
                        message: Resource.Header_SaveFinished,
                        type: NotificationType.Success
                    );
                    s.MarkAsFinished();
                    break;
            }
        }

        private static Dictionary<FileInfo, FileInfo> GetAllFiles(DirectoryInfo src, DirectoryInfo dest)
        {
            Dictionary<FileInfo, FileInfo> files = new Dictionary<FileInfo, FileInfo>();
            foreach (FileInfo file in src.GetFiles())
            {
                files.Add(file, new FileInfo(Path.Combine(dest.FullName, file.Name)));
            }
            //Recursive call for subdirectories
            foreach (DirectoryInfo directory in src.GetDirectories())
            {
                DirectoryInfo nextTarget = dest.CreateSubdirectory(directory.Name);
                Dictionary<FileInfo, FileInfo> subFiles = GetAllFiles(directory, nextTarget);
                foreach (KeyValuePair<FileInfo, FileInfo> file in subFiles)
                    files.Add(file.Key, file.Value);
            }
            return files;
        }

        /// <summary>
        /// Method to copy all files and folders from a source directory to a destination directory
        /// </summary>
        /// <param name="s">concerned save</param>
        /// <param name="files">list of files</param>
        private static JobStatus CopyAll(Save s, Dictionary<FileInfo, FileInfo> files, ManualResetEvent mre)
        {
            foreach (var p in process)
            {
                Process[] processes = Process.GetProcessesByName(p.Split(".exe")[0].ToLower());
                if (processes.Length > 0 && s.GetStatus() == JobStatus.Running)
                {
                    NotificationUtils.SendNotification(title: Resource.Exception_Run_SP_Title.Replace("[NAME]", s.GetName()), message: Resource.Exception_Running_Software_Package.Replace("[PROCESS]", p));
                    s.Pause();
                    Process first = processes[0];
                    if (first != null)
                    {
                        first.EnableRaisingEvents = true;
                        first.Exited += (sender, e) =>
                        {
                            if (s.GetStatus() == JobStatus.Paused)
                            {
                                NotificationUtils.SendNotification(title: $"Reprise de {s.GetName()}", message: $"{p} est maintenant fermé, reprise de la sauvegarde");
                                s.Resume();
                            }
                        };
                    }
                    return JobStatus.Paused;
                }
            }
            foreach (KeyValuePair<FileInfo, FileInfo> data in files)
            {
                LogUtils.LogSaves();
                mre.WaitOne();
                FileInfo source = data.Key;
                FileInfo dest = data.Value;
                //Check if save is running
                if (s.GetStatus() != JobStatus.Running)
                    return JobStatus.Canceled;
                //Update json data
                bool fileCopied = true;
                bool fileExists = File.Exists(dest.FullName);
                //Proceed differential mode by comparing files data
                if (s.GetSaveType() == SaveType.Full || !fileExists || (DateTime.Compare(File.GetLastWriteTime(dest.FullName), File.GetLastWriteTime(source.FullName)) < 0))
                {
                    actualFile[0] = source.FullName;
                    actualFile[1] = dest.FullName;
                    //Stopwatch to mesure transfer time
                    var watch = new Stopwatch();
                    long encryptionTime = -2;
                    watch.Start();
                    try
                    {
                        if (extensions.Contains(source.Extension))
                            encryptionTime = cs.ProcessFile(source.FullName, Path.Combine(dest.FullName, ".enc"));
                        else
                            source.CopyTo(dest.FullName, true);
                    }
                    catch
                    {
                        fileCopied = false;
                        NotificationUtils.SendNotification(dest.FullName, Resource.AccesDenied);
                    }
                    watch.Stop();
                    //Log transfer in json
                    _mutex.WaitOne();
                    LogUtils.LogTransfer(s, source.FullName, dest.FullName, source.Length, watch.ElapsedMilliseconds, encryptionTime);
                    _mutex.ReleaseMutex();

                }
                if (fileCopied)
                    s.AddFileCopied();
                s.AddSizeCopied(source.Length);
                //s.ProgressBar = s.CalculateProgress();
            }
            return JobStatus.Finished;
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

        public static void PauseTransfer()
        {
            mre.Reset();
        }

        public static void ResumeTransfer()
        {
            mre.Set();
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

        public static void ChangePriorityFiles(HashSet<string> newPriorityFiles)
        {
            priorityFiles = newPriorityFiles;
            UpdateConfig();
        }
        public static void ChangeLimitSize(int newLimitSize)
        {
            limitSize = newLimitSize;
            UpdateConfig();
        }


        private static void UpdateConfig()
        {
            LogUtils.LogConfig(key, extensions, process, priorityFiles, limitSize);
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

        public static string GetPriorityFiles()
        {
            return string.Join("\r\n", priorityFiles);
        }

        public static int GetLimitSize()
        {
            return limitSize;
        }
    }
}