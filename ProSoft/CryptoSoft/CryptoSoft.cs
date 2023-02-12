using System.Diagnostics;
using System.IO;

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
        }

        /// <summary>
        /// Initialize the CryptoSoft class
        /// </summary>
        /// <param name="key">algorithm key, 64 bits minimum</param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException">throwed if key is less than 64 bits</exception>
        public static CryptoSoft Init(string key)
        {
            if (key.Length < 8)
                throw new InvalidDataException("Key must be at least 64 bits long");
            _instance ??= new CryptoSoft(key);
            return _instance;
        }

        /// <summary>
        /// Encrypt or decrypt file
        /// </summary>
        /// <param name="inputFile">input file path</param>
        /// <param name="outputFile">output file path, can be null (inputFile.enc)</param>
        /// <returns>time to encrypt file. -1 if error</returns>
        public long ProcessFile(string inputFile, string outputFile = null)
        {
            try
            {
                _stopwatch.Start();
                if(outputFile == null)
                {
                    outputFile = inputFile;
                    if (inputFile.EndsWith(".enc"))
                        outputFile = inputFile[..inputFile.IndexOf('.')] + "_2." + inputFile[(inputFile.IndexOf('.') + 1)..].Replace(".enc", "");
                    else
                        outputFile += ".enc";
                }
                using var fin = new FileStream(inputFile, FileMode.Open);
                using var fout = new FileStream(outputFile, FileMode.Create);
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
            }
            catch
            {
                return -1;
            }
            
        }

    }

}
