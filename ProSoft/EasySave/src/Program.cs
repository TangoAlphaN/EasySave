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

        static Mutex mutex = null;

        /// <summary>
        /// Main program entry point
        /// </summary>
        [STAThread]
        public static void Main()
        {
            bool oldInstance;
            mutex = new Mutex(true, "EasySave", out oldInstance);
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


