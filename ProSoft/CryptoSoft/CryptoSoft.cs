using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProSoft.CryptoSoft
{
    /// <summary>
    /// CryptoSoft Library. This library is used to encrypt and decrypt files.
    /// XOR Key Algorithm
    /// </summary>
    public class CryptoSoft
    {

        /// <summary>
        /// Key used to encrypt and decrypt files
        /// </summary>
        private readonly string _key;

        /// <summary>
        /// Instance of the CryptoSoft class
        /// </summary>
        private static CryptoSoft _instance;

        /// <summary>
        /// Stopwatch used to measure the time it takes to encrypt or decrypt a file
        /// </summary>
        private readonly Stopwatch _stopwatch;

        /// <summary>
        /// Extensions of the files that can be encrypted or decrypted
        /// </summary>
        private readonly HashSet<string> _extensions;

        /// <summary>
        /// Private constructor of the CryptoSoft class
        /// Used to prevent instanciation
        /// </summary>
        private CryptoSoft()
        {
            
        }

        /// <summary>
        /// Private constructor of the CryptoSoft class
        /// </summary>
        /// <param name="key">algorithm key, 64 bits minimum</param>
        private CryptoSoft(string key)
        {
            _key = key;
            _stopwatch = new Stopwatch();
            _extensions = new HashSet<string>();
        }

        /// <summary>
        /// Private constructor of the CryptoSoft class
        /// </summary>
        /// <param name="key">algorithm key, 64 bits minimum</param>
        /// <param name="extensions">extensions of files</param>
        private CryptoSoft(string key, HashSet<string> extensions)
        {
            _key = key;
            _stopwatch = new Stopwatch();
            _extensions = extensions;
        }

        /// <summary>
        /// Initialize the CryptoSoft class
        /// </summary>
        /// <param name="key">algorithm key, 64 bits minimum</param>
        /// <param name="extensions">extensions of files</param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException">throwed if key is less than 64 bits</exception>
        public static CryptoSoft Init(string key, string[] extensions = null)
        {
            if (key.Length < 8)
                throw new InvalidDataException("Key must be at least 64 bits long");
            if(extensions != null)
                _instance ??= new CryptoSoft(key, extensions.ToHashSet());
            else
                _instance ??= new CryptoSoft(key);
            return _instance;
        }

        /// <summary>
        /// Encrypt or decrypt file
        /// </summary>
        /// <param name="inputFile">input file path</param>
        /// <param name="outputFile">output file path, can be null (inputFile.enc)</param>
        /// <param name="largeFile">if set to true, file will be cutted for a faster process</param>
        /// <returns>time to encrypt file. -1 if error</returns>
        public long ProcessFile(string inputFile, string outputFile = null, bool largeFile = false)
        {
            try
            {
                //Create output filename automatically if no specified
                if(outputFile == null)
                {
                    outputFile = inputFile;
                    if (inputFile.EndsWith(".enc"))
                        outputFile = inputFile[..inputFile.IndexOf('.')] + "_2." + inputFile[(inputFile.IndexOf('.') + 1)..].Replace(".enc", "");
                    else
                        outputFile += ".enc";
                }
                //create filestrams
                _stopwatch.Start();
                using var fin = new FileStream(inputFile, FileMode.Open);
                using var fout = new FileStream(outputFile, FileMode.Create);
                //TODO Not Working
                /*//if file is larger than 500mb
                if (largeFile)
                {
                    //file is cutted depending of thread numbers
                    var fileLength = fin.Length;
                    var numberOfThreads = Environment.ProcessorCount;
                    var partLength = (int)(fileLength / numberOfThreads);
                    //each part is crypted in parallel
                    var tasks = new List<Task>();
                    for (var i = 0; i < numberOfThreads; i++)
                    {
                        var buffer = new byte[partLength];
                        fin.Read(buffer, 0, partLength);
                        tasks.Add(Task.Run(() =>
                        {
                            for (var j = 0; j < buffer.Length; j++)
                                buffer[j] = (byte)(buffer[j] ^ _key[j % _key.Length]);
                            fout.Write(buffer, 0, buffer.Length);
                        }));
                    }
                    //total time is returned
                    Task.WaitAll(tasks.ToArray());
                    _stopwatch.Stop();
                    return _stopwatch.ElapsedMilliseconds;
                }
                else
                {*/
                    var buffer = new byte[4096];
                    while (true)
                    {
                        var bytesRead = fin.Read(buffer);
                        if (bytesRead == 0)
                            break;
                        for (var i = 0; i < bytesRead; ++i)
                            buffer[i] = (byte)(buffer[i] ^ _key[i % _key.Length]);
                        fout.Write(buffer, 0, bytesRead);
                    }
                    _stopwatch.Stop();
                    return _stopwatch.ElapsedMilliseconds;
                //}
            }
            catch
            {
                return -1;
            }
        }

        public Dictionary<string, long> ProcessFiles(string[] inputFiles, string outputDir = null)
        {
            Dictionary<string, long> result = new Dictionary<string, long>();
            List<Task> tasks = new List<Task>();
            foreach(string file in inputFiles)
            {
                string outputFile;
                if (outputDir == null)
                {
                    outputFile = file;
                    if (file.EndsWith(".enc"))
                        outputFile = file[..file.IndexOf('.')] + "_2." + file[(file.IndexOf('.') + 1)..].Replace(".enc", "");
                    else
                        outputFile += ".enc";
                }
                else
                    outputFile = outputDir + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(file) + ".enc";
                if (_extensions.Count == 0 || _extensions.Contains(Path.GetExtension(file)))
                {
                    
                    try
                    {
                    
                        tasks.Add(Task.Run(() =>
                        {
                            try
                            {
                                long time = ProcessFile(file, outputFile, new FileInfo(file).Length > (200*1024*1024));
                                result.Add(outputFile, time);
                            }
                            catch
                            {
                                result.Add(outputFile, -1);
                            }
                        }));
                    }
                    catch
                    {
                        result.Add(outputFile, -1);
                    }
                }else
                    result.Add(outputFile, -2);
            }
            Task.WaitAll(tasks.ToArray());
            return result;
        }

        public static void Main(string[] args)
        {
            if (args.Length < 2 || args.Length > 3)
            {
                Console.WriteLine("Usage : CryptoSoft.exe source [dest] [secret_key]");
                return;
            }
            string secret;
            if (args.Length == 3)
                secret = args[2];
            else
            {
                var random = new Random();
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                var keyData = new char[64];
                for (int i = 0; i < 64; i++)
                    keyData[i] = chars[random.Next(chars.Length)];
                secret = new string(keyData);
                Console.WriteLine($"Your secret is : {secret}");
            }
            CryptoSoft cs = Init(secret);
            cs.ProcessFile(args[0], args[1]);
        }

    }

}
