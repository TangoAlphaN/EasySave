using System;
using System.IO;

namespace EasySave.src.Utils
{
    public static class DirectoryUtils
    {
        public static void CopyDirectory(String src, String dest)
        {
            throw new NotImplementedException();
        }

        public static void CopyFile(String src, String dest)
        {
            throw new NotImplementedException();
        }

        public static bool IsValidPath(String path)
        {
            return Directory.Exists(path);
        }

    }
}
