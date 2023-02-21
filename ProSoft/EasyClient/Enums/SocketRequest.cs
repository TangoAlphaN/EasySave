namespace EasyClient.Enums
{
    public enum SocketRequest
    {
        /// <summary>
        /// Request to get data from the server.
        /// </summary>
        GetData,

        /// <summary>
        /// Pause a save
        /// </summary>
        Pause,

        /// <summary>
        /// Play or resume a save
        /// </summary>
        Play,

        /// <summary>
        /// Stop a save
        /// </summary>
        Stop,

        /// <summary>
        /// Cancel a save
        /// </summary>
        Cancel
    }
}
