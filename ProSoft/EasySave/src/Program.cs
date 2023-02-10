using EasySave.src.Render;
using EasySave.src.Utils;

namespace EasySave.src
{
    internal class Program
    {
        protected Program()
        {
        }

        protected static void Main(string[] args)
        {
            LogUtils.Init();
            View v = new View();
            v.Start();
            v.Exit();
        }
    }
}


