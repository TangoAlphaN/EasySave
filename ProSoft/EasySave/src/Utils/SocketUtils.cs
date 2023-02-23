using EasySave.Properties;
using EasySave.src.Models.Data;
using EasySave.src.ViewModels;
using Notification.Wpf;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace EasySave.src.Utils
{
    /// <summary>
    /// Static class to manage socket actions
    /// </summary>
    public static class SocketUtils
    {

        /// <summary>
        /// Instance of socket
        /// </summary>
        private static Socket socket;

        /// <summary>
        /// Instance of save view model
        /// </summary>
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

        /// <summary>
        /// Handle the connection
        /// </summary>
        private static void HandleConnect()
        {
            try
            {
                while (true)
                {
                    //Accept new connection
                    Socket client = socket.Accept();
                    new Thread(() =>
                    {
                        string action = "";
                        while (action != "exit")
                        {
                            //read action from client
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
                                {
                                    saveViewModel.PauseSave(s);
                                    NotificationUtils.SendNotification(
                                        title: $"{s.GetName()} - {s.uuid}",
                                        message: Resource.Header_SavePaused,
                                        type: NotificationType.Success
                                    );
                                }
                                else if (action.Contains("[ACTION]STOP") || action.Contains("[ACTION]CANCEL"))
                                    saveViewModel.CancelSave(s);
                                else if (action.Contains("[ACTION]PLAY"))
                                {
                                    switch (s.GetStatus())
                                    {
                                        case JobStatus.Finished:
                                        case JobStatus.Waiting:
                                        case JobStatus.Canceled:
                                            if (s.GetStatus() != JobStatus.Waiting)
                                                s.Stop();
                                            saveViewModel.RunSave(s);
                                            NotificationUtils.SendNotification(
                                                title: $"{s.GetName()} - {s.uuid}",
                                                message: Resource.Header_SaveLaunched,
                                                type: NotificationType.Success
                                            );
                                            break;
                                        case JobStatus.Paused:
                                            saveViewModel.ResumeSave(s);
                                            NotificationUtils.SendNotification(
                                                title: $"{s.GetName()} - {s.uuid}",
                                                message: Resource.Header_SaveResumed,
                                                type: NotificationType.Success
                                            );
                                            break;
                                    }
                                }
                                LogUtils.LogSaves();
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
