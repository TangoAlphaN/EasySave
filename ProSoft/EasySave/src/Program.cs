using EasySave.src.Render;
using EasySave.src.Utils;

namespace EasySave.src
{
    /// <summary>
    /// Main program class
    /// </summary>
    static class Program
    {

        /// <summary>
        /// Main program entry point
        /// </summary>
        public static void Main()
        {
            LogUtils.Init();
            View v = new View();
            v.Start();
            v.Exit();
        }

    }
}


