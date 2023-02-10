using EasySave.src.Utils;

namespace EasySave.src.Models
{
    public class DestDir : IDir
    {

        public string Path { get; }

        public DestDir(string path)
        {
            if (!DirectoryUtils.IsValidPath(path))
                DirectoryUtils.CreatePath(path);
            this.Path = path;
        }

    }
}
