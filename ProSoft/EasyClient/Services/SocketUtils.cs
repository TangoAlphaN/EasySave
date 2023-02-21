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

        /// <summary>
        /// Socket instance
        /// </summary>
        private static Socket socket;

        /// <summary>
        /// Connects to the server
        /// </summary>
        /// <param name="ip">ip</param>
        /// <param name="port">port</param>
        /// <returns>true if connected</returns>
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

        /// <summary>
        /// Disconnects from the server
        /// </summary>
        public static void Disconnect()
        {
            if (socket != null)
            {
                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                catch
                {
                    socket = null;
                }
            }
        }

        /// <summary>
        /// Sends a request to the server
        /// </summary>
        /// <param name="action">action refered to SocketRequest</param>
        /// <param name="parameter">Optionnal parameter</param>
        /// <returns>Dynamic response from server</returns>
        public static object SendRequest(SocketRequest action, string parameter = null)
        {
            try
            {
                switch (action)
                {
                    //Ask for data
                    case SocketRequest.GetData:
                        //Send action
                        socket.Send(Encoding.ASCII.GetBytes("[ACTION]GETDATA"));
                        byte[] buffer = new byte[4096];
                        var memoryStream = new MemoryStream();
                        //Receive result
                        int bytesRead = socket.Receive(buffer);
                        memoryStream.Write(buffer, 0, bytesRead);
                        //Update data
                        return SaveInfo.Create(JObject.Parse(Encoding.ASCII.GetString(memoryStream.ToArray())));
                    case SocketRequest.Pause:
                        //Send action
                        socket.Send(Encoding.ASCII.GetBytes($"[ACTION]PAUSE|{parameter}"));
                        return null;
                    case SocketRequest.Cancel:
                        //Send action
                        socket.Send(Encoding.ASCII.GetBytes($"[ACTION]CANCEL|{parameter}"));
                        return null;
                    case SocketRequest.Stop:
                        //Send action
                        socket.Send(Encoding.ASCII.GetBytes($"[ACTION]STOP|{parameter}"));
                        return null;
                    case SocketRequest.Play:
                        //Send action
                        socket.Send(Encoding.ASCII.GetBytes($"[ACTION]PLAY|{parameter}"));
                        return null;
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
