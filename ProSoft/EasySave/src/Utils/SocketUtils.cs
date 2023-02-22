using EasySave.src.Models.Data;
using EasySave.src.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace EasySave.src.Utils
{
    /// <summary>
    /// Static class to manage directory actions
    /// </summary>
    public static class SocketUtils
    {

        private static Socket socket;

        private readonly static SaveViewModel saveViewModel = new SaveViewModel();

        /// <summary>
        /// Init the socket server
        /// </summary>
        public static void Init()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress myIp = IPAddress.Parse("0.0.0.0");
            socket.Bind(new IPEndPoint(myIp, 7637));
            socket.Listen(1);
            new Thread(HandleConnect).Start();
        }

        private static void HandleConnect()
        {
            try
            {
                while (true)
                {
                    Socket client = socket.Accept();
                    new Thread(() =>
                    {
                        string action = "";
                        while (action != "exit")
                        {
                            byte[] buffer = new byte[4096];
                            int received = client.Receive(buffer);
                            if (received == 0) break;
                            action = Encoding.ASCII.GetString(buffer, 0, received);
                            if (action == "[ACTION]GETDATA")
                                client.Send(Encoding.ASCII.GetBytes(LogUtils.SavesToJson().ToString()));
                            else if (action.StartsWith("[ACTION]"))
                            {
                                string uuid = action.Split("|")[1];
                                Save s = saveViewModel.GetSavesByUuid(new HashSet<string>() { uuid }).Single();
                                if (action.Contains("[ACTION]PAUSE"))
                                    saveViewModel.PauseSave(s);
                                else if (action.Contains("[ACTION]CANCEL"))
                                    saveViewModel.CancelSave(s);
                                else if (action.Contains("[ACTION]STOP"))
                                    saveViewModel.StopSave(s);
                                else if (action.Contains("[ACTION]PLAY"))
                                    saveViewModel.RunSave(s);
                            }
                        }
                    }).Start();
                }
            }
            catch
            {
                HandleConnect();
            }
        }
    }
}
