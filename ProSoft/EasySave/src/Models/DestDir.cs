using EasySave.src.Utils;

namespace EasySave.src.Models
{
    /// <summary>
    /// Destination directory, implements IDir
    /// </summary>
    public class DestDir : IDir
    {

        public string Path { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">path of dir</param>
        public DestDir(string path)
        {
            if (!DirectoryUtils.IsValidPath(path))
                DirectoryUtils.CreatePath(path);
            Path = path;
        }

    }
}
