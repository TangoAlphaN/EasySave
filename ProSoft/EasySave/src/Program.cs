using EasySave.src.Render;
using EasySave.src.Utils;
using System;

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
        [STAThread]
        public static void Main()
        {
            LogUtils.Init();
            View v = new View();
            v.Start();
            v.Exit();
        }

    }
}


