using EasySave.src.Render;
using EasySave.src.Utils;
using System;
using System.Threading;
using System.Windows;

namespace EasySave.src
{
    /// <summary>
    /// Main program class
    /// </summary>
    static class Program
    {

        /// <summary>
        /// Mutex to prevent multi instances
        /// </summary>
        private static Mutex _mutex;

        /// <summary>
        /// Main program entry point
        /// </summary>
        [STAThread]
        public static void Main()
        {
            _mutex = new Mutex(true, "EasySave", out bool oldInstance);
            if (!oldInstance)
            {
                MessageBox.Show("EasySave is Already Running");
                Environment.Exit(-9);
            }
            LogUtils.Init();
            SocketUtils.Init();
            View v = new View();
            v.Start();
            v.Exit();
        }

    }
}


