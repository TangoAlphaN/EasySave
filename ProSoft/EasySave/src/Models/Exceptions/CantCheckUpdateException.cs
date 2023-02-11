namespace EasySave.src.Models.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception thrown when the update can't be checked
    /// </summary>
    [Serializable]
    public class CantCheckUpdateException : Exception
    {

        //All constructors are required by [Serializable]

        public CantCheckUpdateException()
        {
        }

        public CantCheckUpdateException(string message)
            : base(message)
        {
        }

        public CantCheckUpdateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected CantCheckUpdateException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
