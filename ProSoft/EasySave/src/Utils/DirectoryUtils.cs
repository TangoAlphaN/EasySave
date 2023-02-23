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

        /// <summary>
        /// Semaphore to ensure that only one save can be prioritary at a time
        /// </summary>
        private static readonly SemaphoreSlim prioritarySaveSemaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Data read from config
        /// </summary>
        private static readonly JObject data = JObject.Parse(File.ReadAllText($"{LogUtils.path}config.json"));

        /// <summary>
        /// Key to encrypt files
        /// </summary>
        private static string key = data["key"].ToString();

        /// <summary>
        /// Extensions to encrypt
        /// </summary>
        private static HashSet<string> extensionsToEncrypt = data["extensions"].Select(t => t.ToString()).ToHashSet();

        /// <summary>
        /// Process to observe (detect processes)
        /// </summary>
        private static HashSet<string> process = data["process"].Select(t => t.ToString()).ToHashSet();

        /// <summary>
        /// List of priority extensions
        /// </summary>
        private static HashSet<string> priorityExtensions = data["priorityExtensions"].Select(t => t.ToString()).ToHashSet();

        /// <summary>
        /// Size limit for voluminous files
        /// </summary>
        private static int limitSize = int.Parse(data["limitSize"].ToString());

        /// <summary>
        /// Mutex to ensure that only one large file is being copied at a time
        /// </summary>
        private static readonly Mutex _filesMutex = new Mutex();

        /// <summary>
        /// Mutex to ensure that only one log is being written at a time
        /// </summary>
        private static readonly Mutex _logMutex = new Mutex();

        /// <summary>
        /// CryptoSoft instance
        /// </summary>
        private static CryptoSoft cs;

        /// <summary>
        /// Manual reset event to pause and resume the copy
        /// </summary>
        private static readonly ManualResetEvent mre = new ManualResetEvent(true);

        /// <summary>
        /// Array to store the actual file being copied
        /// </summary>
        private static readonly string[] actualFile = new string[2];

        /// <summary>
        /// Copy all files and folders from a source directory to a destination directory
        /// </summary>
        /// <param name="s">save concerned</param>
        public static void CopyFilesAndFolders(Save s)
        {
            //Init cryptosoft instance
            try
            {
                cs = CryptoSoft.Init(key, extensionsToEncrypt.ToArray());
            }
            catch
            {
                cs = CryptoSoft.Init(key);
            }
            DirectoryInfo sourceDirectory = new DirectoryInfo(s.SrcDir.Path);
            DirectoryInfo destinationDirectory = new DirectoryInfo(s.DestDir.Path);
            //Get all files and priority files
            Dictionary<FileInfo, FileInfo> files = GetAllFiles(sourceDirectory, destinationDirectory, s);
            //Run copy and process result
            switch (CopyAll(s, files, mre))
            {
                case JobStatus.Canceled:
                    NotificationUtils.SendNotification(
                        title: $"{s.GetName()} - {s.uuid}",
                        message: Resource.Header_SaveCanceled,
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
            LogUtils.LogSaves();
        }

        /// <summary>
        /// Method to get all files from a directory
        /// </summary>
        /// <param name="src">source dir</param>
        /// <param name="dest">destination dir</param>
        /// <param name="s">save</param>
        /// <returns>key value pair with source files and dest files</returns>
        private static Dictionary<FileInfo, FileInfo> GetAllFiles(DirectoryInfo src, DirectoryInfo dest, Save s)
        {
            Dictionary<FileInfo, FileInfo> files = new Dictionary<FileInfo, FileInfo>();
            foreach (FileInfo file in src.GetFiles())
            {
                //If file is priority, add it to the beginning of the list
                if (priorityExtensions.Contains(file.Name))
                {
                    files = (new Dictionary<FileInfo, FileInfo> { { file, new FileInfo(Path.Combine(dest.FullName, file.Name)) } }).Concat(files).ToDictionary(k => k.Key, v => v.Value);
                }
                else
                {
                    files.Add(file, new FileInfo(Path.Combine(dest.FullName, file.Name)));
                }
            }
            //Recursive call for subdirectories
            foreach (DirectoryInfo directory in src.GetDirectories())
            {
                DirectoryInfo nextTarget = dest.CreateSubdirectory(directory.Name);
                Dictionary<FileInfo, FileInfo> subFiles = GetAllFiles(directory, nextTarget, s);
                foreach (KeyValuePair<FileInfo, FileInfo> file in subFiles)
                    files.Add(file.Key, file.Value);
            }
            return files;
        }

        /// <summary>
        /// Method to copy all files and folders from a source directory to a destination directory
        /// </summary>
        /// <param name="s">concerned save</param>
        /// <param name="files">dictionnary of files</param>
        /// <param name="mre">manual reset event to control transfer</param>
        /// <returns></returns>
        private static JobStatus CopyAll(Save s, Dictionary<FileInfo, FileInfo> files, ManualResetEvent mre)
        {
            foreach (KeyValuePair<FileInfo, FileInfo> fileInfo in files)
            {
                //Process priority files first
                if (priorityExtensions.Contains(fileInfo.Key.Name))
                    prioritarySaveSemaphore.Wait();
                try
                {
                    LogUtils.LogSaves();

                    FileInfo source = fileInfo.Key;
                    FileInfo dest = fileInfo.Value;
                    //Check if save is running
                    if (s.GetStatus() == JobStatus.Canceled)
                        return JobStatus.Canceled;
                    //Detect processes and wait if opened
                    foreach (var p in process)
                    {
                        Process[] processes = Process.GetProcessesByName(p.Split(".exe")[0].ToUpper());
                        if (processes.Length > 0 && s.GetStatus() == JobStatus.Running)
                        {
                            NotificationUtils.SendNotification(title: Resource.Exception_Run_SP_Title.Replace("[NAME]", s.GetName()), message: Resource.Exception_Running_Software_Package.Replace("[PROCESS]", p));
                            s.Pause();
                            LogUtils.LogSaves();
                            PauseTransfer();
                            Process first = processes[0];
                            if (first != null)
                            {
                                first.EnableRaisingEvents = true;
                                first.Exited += (sender, e) =>
                                {
                                    if (s.GetStatus() == JobStatus.Paused)
                                    {
                                        NotificationUtils.SendNotification(title: Resource.Exception_Run_SP_TitleOK.Replace("[NAME]", s.GetName()), message: Resource.Exception_Running_Software_PackageOK.Replace("[PROCESS]", p), type: NotificationType.Success);
                                        s.Resume();
                                        ResumeTransfer();
                                    }
                                };
                            }
                        }
                    }
                    //Wait for transfer to be resumed
                    mre.WaitOne();
                    //Update json data
                    bool fileCopied = true;
                    bool fileExists = File.Exists(dest.FullName);
                    //Proceed differential mode by comparing files data
                    if (s.GetSaveType() == SaveType.Full || !fileExists || DateTime.Compare(File.GetLastWriteTime(dest.FullName), File.GetLastWriteTime(source.FullName)) < 0 || DateTime.Compare(File.GetLastWriteTime($"{dest.FullName}.enc"), File.GetLastWriteTime(source.FullName)) < 0)
                    {
                        //If size limit is reached, block other large files
                        if (limitSize > 0 && source.Length / 1024 > limitSize)
                            _filesMutex.WaitOne();
                        actualFile[0] = source.FullName;
                        actualFile[1] = dest.FullName;
                        //Stopwatch to mesure transfer time
                        var watch = new Stopwatch();
                        long encryptionTime = -2;
                        watch.Start();
                        try
                        {
                            //Crypt file if extension is in the list
                            if (extensionsToEncrypt.Contains(source.Extension))
                                encryptionTime = cs.ProcessFile(source.FullName, $"{dest.FullName}.enc");
                            else
                                source.CopyTo(dest.FullName, true);
                        }
                        catch
                        {
                            fileCopied = false;
                            NotificationUtils.SendNotification(dest.FullName, Resource.AccesDenied);
                        }
                        //Release mutex
                        if (limitSize > 0 && source.Length / 1024 > limitSize)
                            _filesMutex.ReleaseMutex();
                        watch.Stop();
                        //Log transfer in json
                        _logMutex.WaitOne();
                        LogUtils.LogTransfer(s, source.FullName, dest.FullName, source.Length, watch.ElapsedMilliseconds, encryptionTime);
                        _logMutex.ReleaseMutex();

                    }
                    if (fileCopied)
                        s.AddFileCopied();
                    s.AddSizeCopied(source.Length);
                }
                finally
                {
                    if (priorityExtensions.Contains(fileInfo.Key.Name))
                    {
                        prioritarySaveSemaphore.Release();
                    }
                }
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

        /// <summary>
        /// pause transfer
        /// </summary>
        public static void PauseTransfer()
        {
            mre.Reset();
        }

        /// <summary>
        /// resume transfer
        /// </summary>
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

        /// <summary>
        /// methode to update extensions
        /// </summary>
        /// <param name="newExtensions">new extensions</param>
        public static void ChangeExtensionsToEncrypt(HashSet<string> newExtensions)
        {
            extensionsToEncrypt = newExtensions;
            UpdateConfig();
        }

        /// <summary>
        /// methode to update process
        /// </summary>
        /// <param name="newProcess">process list</param>
        public static void ChangeProcess(HashSet<string> newProcess)
        {
            process = newProcess;
            UpdateConfig();
        }

        /// <summary>
        /// methode to update priority files
        /// </summary>
        /// <param name="newPriorityExtensions">priority extensions</param>
        public static void ChangePriorityExtensions(HashSet<string> newPriorityExtensions)
        {
            priorityExtensions = newPriorityExtensions;
            UpdateConfig();
        }

        /// <summary>
        /// methode to update limit size
        /// </summary>
        /// <param name="newLimitSize">new size limit</param>
        public static void ChangeLimitSize(int newLimitSize)
        {
            limitSize = newLimitSize;
            UpdateConfig();
        }

        /// <summary>
        /// Save config in json file
        /// </summary>
        private static void UpdateConfig()
        {
            LogUtils.LogConfig(key, extensionsToEncrypt, process, priorityExtensions, limitSize);
        }

        /// <summary>
        /// get secret key
        /// </summary>
        /// <returns>secret key</returns>
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

        /// <summary>
        /// get extensions to encrypt
        /// </summary>
        /// <returns>extensions to encrypt</returns>
        public static string GetExtensionsToEncrypt()
        {
            return string.Join("\r\n", extensionsToEncrypt);
        }

        /// <summary>
        /// get processes to detect
        /// </summary>
        /// <returns>processes to detect</returns>
        public static string GetProcess()
        {
            return string.Join("\r\n", process);
        }

        /// <summary>
        /// get priority extensions
        /// </summary>
        /// <returns>priority extensions</returns>
        public static string GetPriorityExtensions()
        {
            return string.Join("\r\n", priorityExtensions);
        }

        /// <summary>
        /// get limit size
        /// </summary>
        /// <returns>limit size for large file</returns>
        public static int GetLimitSize()
        {
            return limitSize;
        }

    }

}