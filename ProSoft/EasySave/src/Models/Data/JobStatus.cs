namespace EasySave.src.Models.Data
{
    /// <summary>
    /// Enum representing status of the save job
    /// </summary>
    public enum JobStatus
    {
        /// <summary>
        /// Save is running
        /// </summary>
        Running,

        /// <summary>
        /// Save is paused
        /// </summary>
        Paused,

        /// <summary>
        /// Save is finshed
        /// </summary>
        Finished,

        /// <summary>
        /// Save is canceled
        /// </summary>
        Canceled,

        /// <summary>
        /// An error occured
        /// </summary>
        Error,

        /// <summary>
        /// Save is ready to run
        /// </summary>
        Waiting
    }
}