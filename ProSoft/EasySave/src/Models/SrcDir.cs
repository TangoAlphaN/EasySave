using EasySave.src.Utils;
using System.IO;

namespace EasySave.src.Models
{
    /// <summary>
    /// Source directory, implements IDir
    /// </summary>
    public class SrcDir : IDir
    {

        public string Path { get; }

        /// <summary>
        /// Size of the directory
        /// </summary>
        private readonly double _size;

        /// <summary>
        /// Number of files in the directory
        /// </summary>
        public readonly long NbFiles;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">path of dir</param>
        public SrcDir(string path)
        {
            Path = path;
            DirectoryInfo directory = new DirectoryInfo(path);
            _size = DirectoryUtils.GetDirectorySize(directory);
            NbFiles = DirectoryUtils.GetDirectoryFiles(directory);
        }

        /// <summary>
        /// Get the size of the directory
        /// </summary>
        /// <returns>size of the directory</returns>
        public double GetSize()
        {
            return _size;
        }

    }
}
