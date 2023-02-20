using EasyClient.Enums;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace EasyClient
{
    /// <summary>
    /// Utility class for sockets.
    /// </summary>
    public static class SocketUtils
    {

        private static Socket socket;

        public static bool Connect(string ip, int port)
        {
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ip, port);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void Disconnect()
        {
            if (socket != null)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        }

        public static object SendRequest(SocketRequest action)
        {
            try
            {
                switch (action)
                {
                    case SocketRequest.GetData:
                        socket.Send(Encoding.ASCII.GetBytes("[ACTION]GETDATA"));
                        byte[] buffer = new byte[4096];
                        var memoryStream = new MemoryStream();
                        int bytesRead = socket.Receive(buffer);
                        memoryStream.Write(buffer, 0, bytesRead);
                        return SaveInfo.Create(JObject.Parse(Encoding.ASCII.GetString(memoryStream.ToArray())));
                    case SocketRequest.Pause:
                        socket.Send(Encoding.ASCII.GetBytes("[ACTION]PAUSE"));
                        break;
                    case SocketRequest.Stop:
                        socket.Send(Encoding.ASCII.GetBytes("[ACTION]STOP"));
                        break;
                    case SocketRequest.Resume:
                        socket.Send(Encoding.ASCII.GetBytes("[ACTION]RESUME"));
                        break;
                }
            }
            catch
            {
                Disconnect();
            }
            return null;
        }
    }

}
